using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

[InitializeOnLoad]
public static class CommandLinePackageManager
{
    #region Statics

    private static readonly string BATCH_MODE_PARAM = "-batchmode";
    private static readonly string PACKAGE_PARAM = "-package";

    private static readonly char[] VERSION_SPLIT = { '@' };

    private static readonly List<string> args;

    private static readonly Queue<string> packagesToRemove = new Queue<string>();
    private static readonly Queue<string> packagesToAdd = new Queue<string>();
    private static AddRequest addRequest;
    private static RemoveRequest removeRequest;
    private static ListRequest listRequest;
    private static SearchRequest searchRequest;

    #endregion

    #region Constructors

    static CommandLinePackageManager()
    {
        bool mustHandlePackages = false;
        if (Environment.GetCommandLineArgs().Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
        {
            Debug.LogFormat(
                "CommandLinePackageManager will try to parse the command line to add or remove packages.\n" +
                "\t Use {0} \"[+-]PACKAGE\"\n" +
                "\t +PACKAGE will add the PACKAGE to the project\n" +
                "\t -PACKAGE will remove the PACKAGE from the project\n" +
                "\t Do not use the -quit command line option or this script won't work. Unity editor will exit when all packages have been handled"
                , PACKAGE_PARAM);
        }

        args = Environment.GetCommandLineArgs().ToList();
        for (int i = 0; i < args.Count; i++)
        {
            if (args[i].ToLower().Equals(PACKAGE_PARAM) && i + 1 < args.Count)
            {
                mustHandlePackages = true;
                HandlePackage(args[i + 1]);
            }
        }

        if (mustHandlePackages)
            HandlePackages();
    }

    #endregion

    #region Methods

    private static void HandlePackage(string packageName)
    {
        if (packageName.StartsWith("-"))
        {
            string packageToRemove = packageName.Substring(1);
            Debug.LogFormat("Will remove package {0}", packageToRemove);
            packagesToRemove.Enqueue(packageToRemove);
        }
        else if (packageName.StartsWith("+"))
        {
            string packageToAdd = packageName.Substring(1);
            Debug.LogFormat("Will add package {0}", packageToAdd);
            packagesToAdd.Enqueue(packageToAdd);
        }
    }

    private static void HandlePackages()
    {
        Debug.LogFormat("Packages will be added/removed");
        searchRequest = Client.SearchAll();
        EditorApplication.update += ProgressSearch;
    }

    private static void ProgressList()
    {
        try
        {
            if (listRequest.IsCompleted)
            {
                EditorApplication.update -= ProgressList;

                if (listRequest.Status == StatusCode.Success)
                {
                    Debug.LogFormat("Packages in the project : {0}",
                        string.Join(";", listRequest.Result.Select(package => package.name + "@" + package.version)));
                    while (packagesToRemove.Count > 0)
                    {
                        string[] packageToRemove = packagesToRemove.Dequeue().Split(VERSION_SPLIT);
                        string packageName = packageToRemove[0];
                        string packageVersion = packageToRemove.Length > 1 ? packageToRemove[1] : null;

                        if (listRequest.Result.Any(package =>
                            package.name == packageName &&
                            (string.IsNullOrEmpty(packageVersion) || package.version == packageVersion)))
                        {
                            removeRequest = Client.Remove(packageName);
                            EditorApplication.update += ProgressRemove;
                            return;
                        }

                        Debug.LogFormat("Package {0} is not present and cannot be removed", packageToRemove);
                    }

                    while (packagesToAdd.Count > 0)
                    {
                        string packageToAddString = packagesToAdd.Dequeue();
                        string[] packageToAdd = packageToAddString.Split(VERSION_SPLIT);
                        string packageName = packageToAdd[0];
                        string packageVersion = packageToAdd.Length > 1 ? packageToAdd[1] : null;

                        PackageInfo packageInfo = listRequest.Result.FirstOrDefault(package => package.name == packageName);
                        if (packageInfo != null)
                        {
                            if (packageInfo.version == packageVersion)
                            {
                                Debug.LogFormat("Package {0}:{1} is already present and cannot be added again", packageToAddString, packageInfo.version);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(packageVersion)) // no version specified so try to update
                                {
                                    PackageInfo availablePackageInfo = searchRequest.Result.FirstOrDefault(package => package.name == packageName);
                                    if (availablePackageInfo == null)
                                    {
                                        Debug.LogFormat("Package {0}:{1} is present but cannot be found on the repository and cannot be updated", packageToAddString, packageInfo.version);
                                        // Don't return and check the next package
                                    }
                                    else if (availablePackageInfo.version != packageInfo.version)
                                    {
                                        Debug.LogFormat("Package {0} is already present but with version {1}. Newer version {2} is available. It will be updated to the newer version", packageName, packageInfo.version,
                                        availablePackageInfo.version);

                                        // We edit the manifest file directly to prevent issues when the Tools package is being updated
                                        string text = File.ReadAllText("Packages/manifest.json");
                                        text = text.Replace("\"" + packageName + "\": \"" + packageInfo.version + "\"", "\"" + packageName + "\": \"" + availablePackageInfo.version + "\"");
                                        File.WriteAllText("Packages/manifest.json", text);

                                        listRequest = Client.List();
                                        EditorApplication.update += ProgressList;
                                        return; // return to prevent exiting
                                    }
                                    else
                                    {
                                        Debug.LogFormat("Package {0}:{1} is already present with the last version and cannot be updated", packageToAddString, packageInfo.version);
                                        // Don't return and check the next package
                                    }
                                }
                                else // version was specified and not present
                                {
                                    Debug.LogFormat("Package {0} is already present but with version {1} instead of {2}. It will be removed before adding the specified version", packageName, packageInfo.version,
                                        packageVersion);
                                    packagesToRemove.Enqueue(packageName);
                                    listRequest = Client.List();
                                    EditorApplication.update += ProgressList;
                                    return; // return to prevent exiting
                                }
                            }
                        }
                        else
                        {
                            addRequest = Client.Add(packageToAddString);
                            EditorApplication.update += ProgressAdd;
                            return; // return to prevent exiting
                        }
                    }

                    EditorApplication.Exit(0);
                }
                else if (listRequest.Status >= StatusCode.Failure)
                {
                    Debug.Log(listRequest.Error.message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            EditorApplication.Exit(-1);
        }
    }

    private static void ProgressRemove()
    {
        try
        {
            if (removeRequest.IsCompleted)
            {
                EditorApplication.update -= ProgressRemove;

                if (removeRequest.Status == StatusCode.Success)
                    Debug.Log("Removed: " + removeRequest.PackageIdOrName);
                else if (removeRequest.Status >= StatusCode.Failure)
                    Debug.Log(removeRequest.Error.message);

                listRequest = Client.List();
                EditorApplication.update += ProgressList;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            EditorApplication.Exit(-1);
        }
    }

    private static void ProgressAdd()
    {
        try
        {
            if (addRequest.IsCompleted)
            {
                EditorApplication.update -= ProgressAdd;

                if (addRequest.Status == StatusCode.Success)
                    Debug.Log("Added: " + addRequest.Result.name + "@" + addRequest.Result.version);
                else if (addRequest.Status >= StatusCode.Failure)
                    Debug.Log(addRequest.Error.message);

                listRequest = Client.List();
                EditorApplication.update += ProgressList;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            EditorApplication.Exit(-1);
        }
    }

    private static void ProgressSearch()
    {
        try
        {
            if (searchRequest.IsCompleted)
            {
                EditorApplication.update -= ProgressSearch;
                listRequest = Client.List();
                EditorApplication.update += ProgressList;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            EditorApplication.Exit(-1);
        }
    }

    #endregion
}