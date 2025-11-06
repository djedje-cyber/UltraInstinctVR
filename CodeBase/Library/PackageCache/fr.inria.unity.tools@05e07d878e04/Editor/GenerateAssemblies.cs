using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Compilation;

using UnityEngine;

[InitializeOnLoad]
public static class GenerateAssemblies
{
    #region Fields

    private const string REPLACE_ASSEMBLY_PARAM = "-replaceassembly";
    private const string MAP_FILES_PREFIX_PARAM = "-mapfilesprefix";
    private const string MAP_FILES_PREFIX_DEFAULT = "map_";
    private const string MAP_FILES_PATH_PARAM = "-mapfilespath";
    private const string MAP_FILES_PATH_DEFAULT = "./";
    private const string GUIDS_CLASSES_MAP_FILE = "-guidclassesmapfile";
    private const string GUIDS_CLASSES_MAP_FILE_DEFAULT = MAP_FILES_PREFIX_DEFAULT + ReplaceAssemblies.GUID_CLASS_MAP_FILE_SUFFIX;
    private const string CLASS_ORDER_MAP_FILE = "-classordermapfile";
    private const string CLASS_ORDER_MAP_FILE_DEFAULT = MAP_FILES_PREFIX_DEFAULT + ReplaceAssemblies.CLASS_ORDER_MAP_FILE_SUFFIX;

    private const string UPDATE_IDS_PARAM = "-updateids";
    private static readonly string BATCH_MODE_PARAM = "-batchmode";

    private static string mapFilesPrefix;
    private static string mapFilesPath;

    #endregion

    #region Constructors

    static GenerateAssemblies()
    {
        List<string> args = Environment.GetCommandLineArgs().ToList();

        if (args.Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
        {
            Debug.LogFormat("GenerateAssemblies will try to parse the command line to replace assemblies or update ids.\n" +
                            "\t\t Use {0} \"assemblyname\" for every assembly you wish to replace \n" +
                            "\t\t Use {1} \"map_files_prefix\" to optionnaly set a prefix to the map files that will be read/written. Default is {2} \n" +
                            "\t\t Use {3} \"path\" to optionnaly set the path where the map files will be read/written. Default is {4} \n" +
                            "\n" +
                            "\t Use {5} to update the ids of the classes in the assets. \n" +
                            "\t\t Use {6} \"path\" to optionnaly set the path where the GUID to Class map file is located. Default is {7} \n" +
                            "\t\t Use {8} \"path\" to optionnaly set the path where the Class to Order map file is located. Default is {9} \n"
                , REPLACE_ASSEMBLY_PARAM, MAP_FILES_PREFIX_PARAM, MAP_FILES_PREFIX_DEFAULT, MAP_FILES_PATH_PARAM, MAP_FILES_PATH_DEFAULT,
                UPDATE_IDS_PARAM, GUIDS_CLASSES_MAP_FILE, GUIDS_CLASSES_MAP_FILE_DEFAULT, CLASS_ORDER_MAP_FILE, CLASS_ORDER_MAP_FILE_DEFAULT);
        }

        mapFilesPrefix = MAP_FILES_PREFIX_DEFAULT;
        int mapFilesPrefixIndex = args.FindIndex(arg => arg.ToLower().Equals(MAP_FILES_PREFIX_PARAM));
        if (mapFilesPrefixIndex >= 0 && mapFilesPrefixIndex + 1 < args.Count)
        {
            mapFilesPrefix = args[mapFilesPrefixIndex + 1];
        }
        mapFilesPath = MAP_FILES_PATH_DEFAULT;
        int mapFilesPathIndex = args.FindIndex(arg => arg.ToLower().Equals(MAP_FILES_PATH_PARAM));
        if (mapFilesPathIndex >= 0 && mapFilesPathIndex + 1 < args.Count)
        {
            mapFilesPath = args[mapFilesPathIndex + 1];
        }

        DoReplaceAssemblies();
        UpdateClassesIdsInAssets();
    }

    #endregion

    #region Methods

    public static void DoReplaceAssemblies()
    {
        List<string> args = Environment.GetCommandLineArgs().ToList();
        if (args.Exists(arg => arg.ToLower().Equals(REPLACE_ASSEMBLY_PARAM))) // is a replacement requested ?
        {
            int lastIndex = 0;

            while (lastIndex != -1)
            {
                lastIndex = args.FindIndex(lastIndex, arg => arg.ToLower().Equals(REPLACE_ASSEMBLY_PARAM));
                if (lastIndex >= 0 && lastIndex + 1 < args.Count)
                {
                    string assemblyToReplace = args[lastIndex + 1];
                    ReplaceAssemblies.instance.AddAssemblyFileToReplace(assemblyToReplace);
                    Debug.LogFormat("Added assembly {0} to the list of assemblies to replace.", assemblyToReplace);
                    lastIndex++;
                }
            }

            Debug.Log("Availalbe assemblies are : \n " + string.Join("\n", CompilationPipeline.GetAssemblies().Select(a => a.name)));
            ReplaceAssemblies.instance.MapFilesPrefix = mapFilesPrefix;
            ReplaceAssemblies.instance.MapFilesPath = mapFilesPath;
            ReplaceAssemblies.ReplaceRegisteredAssemblies();
        }
    }

    public static void UpdateClassesIdsInAssets()
    {
        List<string> args = Environment.GetCommandLineArgs().ToList();
        if (args.Exists(arg => arg.ToLower().Equals(UPDATE_IDS_PARAM))) // is an update requested ?
        {
            string idsClassesMapFile;
            int idsClassesMapFileIndex = args.FindIndex(arg => arg.ToLower().Equals(GUIDS_CLASSES_MAP_FILE));
            if (idsClassesMapFileIndex >= 0 && idsClassesMapFileIndex + 1 < args.Count)
            {
                idsClassesMapFile = args[idsClassesMapFileIndex + 1];
            }
            else
            {
                idsClassesMapFile = mapFilesPath + mapFilesPrefix + ReplaceAssemblies.GUID_CLASS_MAP_FILE_SUFFIX;
            }
            string classOrderMapFile;
            int classOrderMapFileIndex = args.FindIndex(arg => arg.ToLower().Equals(CLASS_ORDER_MAP_FILE));
            if (classOrderMapFileIndex >= 0 && classOrderMapFileIndex + 1 < args.Count)
            {
                classOrderMapFile = args[classOrderMapFileIndex + 1];
            }
            else
            {
                classOrderMapFile = mapFilesPath + mapFilesPrefix + ReplaceAssemblies.CLASS_ORDER_MAP_FILE_SUFFIX;
            }

            ReplaceAssemblies.instance.UpdateClassesIdsInAssets(idsClassesMapFile);
            ReplaceAssemblies.instance.UpdateClassesOrder(classOrderMapFile);
        }
    }

    #endregion
}
