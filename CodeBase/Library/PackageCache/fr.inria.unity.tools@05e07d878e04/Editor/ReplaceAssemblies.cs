using InriaTools;
using InriaTools.Utils;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEditor.Compilation;

using UnityEngine;

public class ReplaceAssemblies : ScriptableSingleton<ReplaceAssemblies>
{
    #region Nested

    [System.Serializable]
    protected class GuidClassNameMap : SerializableDictionary<string, string>
    {
        #region Methods

        public override string ToString()
        {
            return string.Join("\n", this.Select(pair => pair.Key + " : " + pair.Value));
        }

        #endregion
    }

    [System.Serializable]
    protected class AssemblyNameGuidMap : SerializableDictionary<string, string>
    {
        #region Methods

        public override string ToString()
        {
            return string.Join("\n", this.Select(pair => pair.Key + " : " + pair.Value));
        }

        #endregion
    }

    [System.Serializable]
    protected class AssemblyNamePathMap : SerializableDictionary<string, string>
    {
        #region Methods

        public override string ToString()
        {
            return string.Join("\n", this.Select(pair => pair.Key + " : " + pair.Value));
        }

        #endregion
    }

    [System.Serializable]
    protected class ClassNameScriptOrderMap : SerializableDictionary<string, int>
    {
        #region Methods

        public override string ToString()
        {
            return string.Join("\n", this.Select(pair => pair.Key + " : " + pair.Value));
        }

        #endregion
    }

    [System.Serializable]
    protected class AssemblyNameClassNameScriptOrderMap : SerializableDictionary<string, ClassNameScriptOrderMap>
    {
        #region Methods

        public override string ToString()
        {
            return string.Join("\n", this.Select(pair => pair.Key + " : \n" + pair.Value));
        }

        #endregion
    }

    public const string GUID_CLASS_MAP_FILE_SUFFIX = "guid_class.json";
    public const string CLASS_ORDER_MAP_FILE_SUFFIX = "class_order.json";

    #endregion

    #region Statics

    public static string ASSEMBLY_EXTENSION = ".dll";
    public static string ASSEMBLY_DEFINITION_EXTENSION = ".asmdef";

    private static readonly string[] fileListPath = { "*.prefab", "*.unity", "*.asset" };

    #endregion

    #region Fields

    // save the guid/classname correspondence of the scripts that we will remove
    [SerializeField] private AssemblyNameClassNameScriptOrderMap oldClassNameToScriptOrderToMapPerAssembly = new();

    [SerializeField] private List<string> assembliesNamesToReplace = new();

    [SerializeField] private GuidClassNameMap oldGUIDToClassNameMap = new();

    [SerializeField] private AssemblyNameGuidMap assemblyNameToGuidMap = new();
    [SerializeField] private AssemblyNamePathMap assemblyNameToPath = new();
    [SerializeField] private List<string> pathsOfAssemblyFilesCreatedByUnity = new();

    [SerializeField] private List<string> pathsOfAssemblyFilesInAssetFolder = new();

    [SerializeField] private string tempSourceFilePath;

    #endregion

    #region Properties

    public string TempSourceFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(tempSourceFilePath))
            {
                tempSourceFilePath = FileUtil.GetUniqueTempPathInProject();
            }

            return tempSourceFilePath;
        }
    }

    public string MapFilesPrefix { get; internal set; }
    public string MapFilesPath { get; internal set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Update assembly data, meaning execution order, ids, ... using the original scripts information
    /// </summary>
    private static void UpdateAssemblyData()
    {
        Debug.Log("Assemblies to replace : " + string.Join(", ", instance.assembliesNamesToReplace));

        // Create a copy to iterate over it while being able to remove elements
        foreach (string assemblyNameToReplace in instance.assembliesNamesToReplace.ToList())
        {
            Debug.LogFormat("Looking for assembly {0} path in : \n{1} ", assemblyNameToReplace, instance.assemblyNameToPath);
            _ = instance.assemblyNameToPath.TryGetValue(assemblyNameToReplace, out string assemblyPath);
            if (string.IsNullOrEmpty(assemblyPath))
            {
                Debug.LogWarningFormat("Could not find assembly named : {0} in : {1}", assemblyNameToReplace, instance.assemblyNameToPath);
                continue;
            }

            assemblyPath = GetUnityCompatiblePath(assemblyPath);
            Object[] assetsInAssembly = AssetDatabase.LoadAllAssetsAtPath(assemblyPath);
            MonoScript[] assemblyObjects = assetsInAssembly.OfType<MonoScript>().ToArray();

            Debug.LogFormat("Imported assembly {0} (in : {1}) contains {2} mono scripts.", assemblyNameToReplace, assemblyPath, assemblyObjects.Length);

            if (assemblyObjects.Length == 0)
                continue;

            Debug.Log("Second time recovering GUID from assembly definition file from : \n" + instance.assemblyNameToGuidMap);
            string dllGuid = instance.assemblyNameToGuidMap[assemblyNameToReplace];
            ReplaceIdOfAsset(assemblyPath, dllGuid);

            // Save the new GUID and file ID for the MonoScript in the new assembly
            Dictionary<string, KeyValuePair<string, long>> newMonoScriptToIDsMap = new();
            // For each component, replace the guid and fileID file
            for (int i = 0; i < assemblyObjects.Length; i++)
            {
                // happens sometimes ... don't know why
                if (assemblyObjects[i] == null || assemblyObjects[i].GetClass() == null)
                {
                    Debug.LogWarning("MonoScript is null, bypassing ...");
                    continue;
                }
                Debug.LogFormat("Replacing guid and fileID for {0}", assemblyObjects[i].GetClass().FullName);
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assemblyObjects[i], out string wrongDllGuid, out long dllFileId))
                {
                    string fullClassName = assemblyObjects[i].GetClass().FullName;
                    newMonoScriptToIDsMap.Add(fullClassName, new KeyValuePair<string, long>(dllGuid, dllFileId));
                }
            }

            Debug.Log("Map of GUID/Class : \n" + instance.oldGUIDToClassNameMap);
            Debug.Log("Map of Class/GUId:FileId : \n" + string.Join("\n", newMonoScriptToIDsMap.Select(pair => pair.Key + " : " + pair.Value.Key + " - " + pair.Value.Value).ToArray()));

            ReplaceIdsInAssets(instance.oldGUIDToClassNameMap, newMonoScriptToIDsMap);

            // Replace execution orders of the mono scripts in the assembly file
            ChangeScriptExecutionOrder.SetExecutionOrders(instance.oldClassNameToScriptOrderToMapPerAssembly[assemblyNameToReplace], false);

            Debug.LogFormat("Removing assembly {0} from the list of assemblies to replace", assemblyNameToReplace);
            _ = instance.assembliesNamesToReplace.Remove(assemblyNameToReplace);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }

    /// <summary>
    /// Update assembly data, meaning execution order, ids, ... using the original scripts information
    /// </summary>
    private static void UpdateClassesIdsInAssets()
    {
        // list all the potential asmdef files that might need update
        List<string> fileList = new();

        fileList.AddRange(Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories));
        fileList.AddRange(Directory.GetFiles(Path.GetFullPath("Packages"), "*.dll", SearchOption.AllDirectories));
        fileList.AddRange(Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories));
        fileList.AddRange(Directory.GetFiles(Path.GetFullPath("Packages"), "*.cs", SearchOption.AllDirectories));

        // Create a copy to iterate over it while being able to remove elements
        foreach (string fileToCheck in fileList)
        {
            string localFileToCheck = GetUnityCompatiblePath(fileToCheck);
            Debug.LogFormat("Looking in : {0} ", localFileToCheck);

            Object[] assetsInAssembly = AssetDatabase.LoadAllAssetsAtPath(localFileToCheck);
            MonoScript[] assemblyObjects = assetsInAssembly.OfType<MonoScript>().ToArray();

            Debug.LogFormat("Imported file {0} contains {1} mono scripts.", localFileToCheck, assemblyObjects.Length);

            if (assemblyObjects.Length == 0)
                continue;

            string fileGuid = AssetDatabase.AssetPathToGUID(localFileToCheck);

            // Save the new GUID and file ID for the MonoScript in the new assembly
            Dictionary<string, KeyValuePair<string, long>> newMonoScriptToIDsMap = new();
            // For each component, replace the guid and fileID file
            for (int i = 0; i < assemblyObjects.Length; i++)
            {
                // happens sometimes ... don't know why
                if (assemblyObjects[i] == null || assemblyObjects[i].GetClass() == null)
                {
                    Debug.LogWarning("MonoScript is null, bypassing ...");
                    continue;
                }
                Debug.LogFormat("Replacing guid and fileID for {0}", assemblyObjects[i].GetClass().FullName);

                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assemblyObjects[i], out _, out long dllFileId))
                {
                    string fullClassName = assemblyObjects[i].GetClass().FullName;
                    newMonoScriptToIDsMap.Add(fullClassName, new KeyValuePair<string, long>(fileGuid, dllFileId));
                }
            }

            ReplaceIdsInAssets(instance.oldGUIDToClassNameMap, newMonoScriptToIDsMap);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    private static void UpdateClassesOrder()
    {
        foreach (string assemblyName in instance.oldClassNameToScriptOrderToMapPerAssembly.Keys)
        {
            // Replace execution orders of the mono scripts in the assembly file
            ChangeScriptExecutionOrder.SetExecutionOrders(instance.oldClassNameToScriptOrderToMapPerAssembly[assemblyName], false);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    #endregion

    #region Methods

    [MenuItem("Tools/Replace Assembly")]
    public static void ReplaceAssemblyMenu()
    {
        string assemblyDefinitionFilePath = EditorUtility.OpenFilePanel(
            "Select Assembly Definition File",
            Application.dataPath,
            ASSEMBLY_DEFINITION_EXTENSION[1..]);
        if (assemblyDefinitionFilePath.Length == 0)
            return;

        ReplaceAssembly(assemblyDefinitionFilePath);
    }

    [MenuItem("Tools/Revert Replace all Assemblies")]
    public static void RevertReplaceAssembliesMenu()
    {
        instance.RevertReplaceAssemblies();
    }

    public static void ReplaceAssembly(string assemblyDefinitionFilePath)
    {
        Debug.LogFormat("Replacing scripts for assembly definition file {0}", assemblyDefinitionFilePath);
        string asmdefDirectory = Path.GetDirectoryName(assemblyDefinitionFilePath);
        string assemblyName = Path.GetFileNameWithoutExtension(assemblyDefinitionFilePath);
        Assembly assemblyToReplace = CompilationPipeline.GetAssemblies().ToList().Find(assembly => assembly.name.ToLower().Equals(assemblyName.ToLower()));
        string assemblyPath = assemblyToReplace.outputPath;
        string assemblyFileName = Path.GetFileName(assemblyPath);
        // prepare the final path of the dll file
        string finalAssemblyPath = Path.Combine(asmdefDirectory, assemblyFileName);

        // We need to remove .\ when using LoadAllAssetsAtPath
        string cleanFinalAssemblyPath = finalAssemblyPath.Replace(".\\", "");
        Debug.LogFormat("Assembly path for assembly {0} will be {1}", assemblyName, cleanFinalAssemblyPath);
        instance.assemblyNameToPath.Add(assemblyName, cleanFinalAssemblyPath);

        string asmdefGuid = AssetDatabase.AssetPathToGUID(assemblyDefinitionFilePath.Replace(".\\", ""));

        instance.assemblyNameToGuidMap.Add(assemblyToReplace.name, asmdefGuid);
        Debug.Log("Map of assembly name/GUID Updated : \n" + instance.assemblyNameToGuidMap);

        string[] assemblyFilePathInAssets = Directory.GetFiles("./Assets", assemblyFileName, SearchOption.AllDirectories);

        // save the guid/classname correspondence of the scripts that we will remove
        ClassNameScriptOrderMap oldClassNameToScriptOrderToMap = new();
        instance.oldClassNameToScriptOrderToMapPerAssembly.Add(assemblyToReplace.name, oldClassNameToScriptOrderToMap);
        if (assemblyFilePathInAssets.Length <= 0)
        {
            Debug.Log("Handling assembly file : " + assemblyFileName);

            // Move all script files outside the asset folder
            foreach (string sourceFile in assemblyToReplace.sourceFiles)
            {
                string tempScriptPath = Path.Combine(ReplaceAssemblies.instance.TempSourceFilePath, sourceFile);
                _ = Directory.CreateDirectory(Path.GetDirectoryName(tempScriptPath));
                if (!File.Exists(sourceFile))
                {
                    Debug.LogErrorFormat("File {0} does not exist while the assembly {1} references it.", sourceFile, assemblyToReplace.name);
                    continue;
                }
                Debug.Log("Will move " + sourceFile + " to " + tempScriptPath);
                // save the guid of the file because we may need to replace it later
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(sourceFile);
                if (monoScript != null && monoScript.GetClass() != null)
                {
                    instance.oldGUIDToClassNameMap.Add(AssetDatabase.AssetPathToGUID(sourceFile), monoScript.GetClass().FullName);
                    if (!oldClassNameToScriptOrderToMap.ContainsKey(monoScript.GetClass().FullName))
                        oldClassNameToScriptOrderToMap.Add(monoScript.GetClass().FullName, MonoImporter.GetExecutionOrder(monoScript));
                }

                FileUtil.CopyFileOrDirectory(sourceFile, tempScriptPath);
                // delete this way so that unity forgets about the guid/asset link
                _ = AssetDatabase.DeleteAsset(sourceFile);
            }

            // remove empty directories
            foreach (string sourceFile in assemblyToReplace.sourceFiles)
                RemoveEmptyDirectories(Path.GetDirectoryName(sourceFile));

            Debug.Log("Map of GUID/Class : \n" + instance.oldGUIDToClassNameMap);
            Debug.Log("Map of Class/Order : \n" + oldClassNameToScriptOrderToMap);

            string tempAsmdefPath = Path.Combine(ReplaceAssemblies.instance.TempSourceFilePath, Path.GetFileName(assemblyDefinitionFilePath));
            // Rename the asmdef meta file to the dll meta file so that the dll guid stays the same. Do it in two steps so that unity cache is update properly
            Debug.Log("Will copy " + assemblyDefinitionFilePath + ".meta" + " to " + finalAssemblyPath + ".metatmp");
            FileUtil.CopyFileOrDirectory(assemblyDefinitionFilePath + ".meta", finalAssemblyPath + ".metatmp");
            Debug.Log("Will copy " + assemblyDefinitionFilePath + " to " + tempAsmdefPath);
            FileUtil.CopyFileOrDirectory(assemblyDefinitionFilePath, tempAsmdefPath);
            Debug.Log("Will delete " + assemblyDefinitionFilePath);
            // delete this way so that unity forgets about the guid/asset link
            _ = AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(asmdefGuid));
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log("will move " + assemblyPath + " to " + finalAssemblyPath);
            FileUtil.MoveFileOrDirectory(assemblyPath, finalAssemblyPath);
            if (File.Exists(assemblyDefinitionFilePath + ".meta"))
            {
                Debug.Log("Will remove " + assemblyDefinitionFilePath + ".meta");
                _ = FileUtil.DeleteFileOrDirectory(assemblyDefinitionFilePath + ".meta");
            }
            Debug.Log("Will move " + finalAssemblyPath + ".metatmp" + " to " + finalAssemblyPath + ".meta");
            FileUtil.MoveFileOrDirectory(finalAssemblyPath + ".metatmp", finalAssemblyPath + ".meta");

            instance.pathsOfAssemblyFilesInAssetFolder.Add(finalAssemblyPath);
            instance.pathsOfAssemblyFilesCreatedByUnity.Add(assemblyPath);

            Debug.Log("Will import asset  : " + cleanFinalAssemblyPath);
            AssetDatabase.ImportAsset(cleanFinalAssemblyPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("First time recovering GUID from assembly definition file from : \n" + instance.assemblyNameToGuidMap);
            ReplaceIdOfAsset(cleanFinalAssemblyPath, instance.assemblyNameToGuidMap[assemblyName]);
            ReplaceAssemlbyRefInAsmdef(asmdefGuid, assemblyToReplace.name, assemblyFileName);
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        }
        else
        {
            Debug.Log("Already found an assembly file named " + assemblyFileName + " in asset folder");
        }
    }

    public static void ReplaceRegisteredAssemblies()
    {
        AssemblyReloadEvents.afterAssemblyReload += () =>
        {
            string guidClassMapFile = ReplaceAssemblies.instance.MapFilesPrefix + GUID_CLASS_MAP_FILE_SUFFIX;
            string guidClassMapFileFullPath = Path.Combine(ReplaceAssemblies.instance.MapFilesPath, guidClassMapFile);
            Debug.Log("extracting GUID/Class map to : " + guidClassMapFileFullPath);
            File.WriteAllText(guidClassMapFileFullPath, JsonUtility.ToJson(instance.oldGUIDToClassNameMap));
            string classOrderMapFile = ReplaceAssemblies.instance.MapFilesPrefix + CLASS_ORDER_MAP_FILE_SUFFIX;
            string classOrderMapFileFullPath = Path.Combine(ReplaceAssemblies.instance.MapFilesPath, classOrderMapFile);
            Debug.Log("extracting Class/Order map to : " + classOrderMapFileFullPath);
            File.WriteAllText(classOrderMapFileFullPath, JsonUtility.ToJson(instance.oldClassNameToScriptOrderToMapPerAssembly));
        };
        Assembly[] assemblies = CompilationPipeline.GetAssemblies();
        foreach (string assemblyNameToReplace in instance.assembliesNamesToReplace)
        {
            Debug.Log("Looking for " + assemblyNameToReplace.ToLower() + " in : " + string.Join(", ", assemblies.Select(a => a.name.ToLower())));
            Assembly assemblyToReplace = assemblies.ToList().Find(assembly => assembly.name.ToLower().Equals(assemblyNameToReplace.ToLower()));
            if (assemblyToReplace != null)
            {
                Debug.LogFormat("Found assembly to replace : {0}", assemblyToReplace.outputPath);
                string[] assemblyDefinitionFilePaths = Directory.GetFiles(".", assemblyNameToReplace + ASSEMBLY_DEFINITION_EXTENSION, SearchOption.AllDirectories);
                if (assemblyDefinitionFilePaths.Length > 0)
                {
                    string assemblyDefinitionFilePath = assemblyDefinitionFilePaths[0];
                    ReplaceAssembly(assemblyDefinitionFilePath);
                }
                else
                {
                    Debug.LogErrorFormat("Could not find assembly definition file for assembly {0}. Tried : \n" +
                                         "{1}\n", assemblyNameToReplace, assemblyNameToReplace + ASSEMBLY_DEFINITION_EXTENSION);
                    EditorApplication.Exit(-1);
                }
            }
        }
    }

    public static string GetUnityCompatiblePath(string originalPath)
    {
        string localFileToCheck = Path.GetRelativePath(Application.dataPath, originalPath).Replace('\\', '/');
        localFileToCheck = localFileToCheck.Replace("../", "");
        localFileToCheck = localFileToCheck.Replace("./", "");
        localFileToCheck = localFileToCheck.Replace(".\\", "");
        localFileToCheck = localFileToCheck.Replace("..\\", "");
        return localFileToCheck;
    }

    public void AddAssemblyFileToReplace(string assemblyFile)
    {
        assembliesNamesToReplace.Add(assemblyFile);
    }

    internal void UpdateClassesIdsInAssets(string idsClassesMapFile)
    {
        Debug.Log("Checking for GUID/Class map file : " + idsClassesMapFile);
        if (!string.IsNullOrEmpty(idsClassesMapFile) && File.Exists(idsClassesMapFile))
        {
            oldGUIDToClassNameMap = JsonUtility.FromJson<GuidClassNameMap>(File.ReadAllText(idsClassesMapFile));
            Debug.Log("Loaded Map of GUID/Class : \n" + instance.oldGUIDToClassNameMap);
        }
        AssemblyReloadEvents.afterAssemblyReload += () =>
        {
            UpdateClassesIdsInAssets();
        };
    }

    internal void UpdateClassesOrder(string classOrderMapFile)
    {
        Debug.Log("Checking for Class/Order map file : " + classOrderMapFile);
        if (!string.IsNullOrEmpty(classOrderMapFile) && File.Exists(classOrderMapFile))
        {
            oldClassNameToScriptOrderToMapPerAssembly = JsonUtility.FromJson<AssemblyNameClassNameScriptOrderMap>(File.ReadAllText(classOrderMapFile));
            Debug.Log("Loaded map of Class/Order : \n" + instance.oldClassNameToScriptOrderToMapPerAssembly);
        }
        AssemblyReloadEvents.afterAssemblyReload += () =>
        {
            UpdateClassesOrder();
        };
    }

    private static void ReplaceAssemlbyRefInAsmdef(string asmdefGuid, string assemblyName, string assemblyFileName)
    {
        // list all the potential asmdef files that might need update
        List<string> fileList = new();

        fileList.AddRange(Directory.GetFiles(Application.dataPath, "*.asmdef", SearchOption.AllDirectories));
        fileList.AddRange(Directory.GetFiles(Path.GetFullPath("Packages"), "*.asmdef", SearchOption.AllDirectories));
        foreach (string file in fileList)
        {
            // Try to find if this asmdef must be modified
            string[] fileLines = File.ReadAllLines(file);
            bool hasReferencesOvveride = false;
            bool hasReferenceWithGuid = false;
            bool hasReferenceWithName = false;
            for (int line = 0; line < fileLines.Length; line++)
            {
                // found GUID:xxxxxxxxx
                hasReferenceWithGuid |= fileLines[line].Contains("GUID:" + asmdefGuid);
                // found "assemblyname"
                hasReferenceWithName |= fileLines[line].Contains("\"" + assemblyName + "\"");
                // has overrideReferences = true
                hasReferencesOvveride |= fileLines[line].Contains("overrideReferences") && fileLines[line].Contains("true");
            }
            // The assembly is referenced and overrideReferences is set to true. We need to modify the asmdef
            if ((hasReferenceWithGuid || hasReferenceWithName) && hasReferencesOvveride)
            {
                Debug.Log($"Assembly Definition File {file} references assembly {assemblyName} by Guid or name and has \"overrideReferences\" set to true. Adding {assemblyFileName} to the list of precompiledReferences");
                List<string> writableLines = fileLines.ToList();
                bool foundPrecompiledReferences = false;
                int precompiledReferencesStart = -1;
                int precompiledReferencesEnd = -1;
                for (int line = 0; line < writableLines.Count; line++)
                {
                    if (writableLines[line].Contains("precompiledReferences"))
                    {
                        foundPrecompiledReferences = true;
                    }
                    // we found the first opening '[' after "precompiledReferences" (can be on the same line)
                    if (foundPrecompiledReferences && precompiledReferencesStart < 0 && writableLines[line].Contains("["))
                    {
                        precompiledReferencesStart = line;
                    }
                    // we found the first closing ']' after "precompiledReferences"
                    if (precompiledReferencesEnd < 0 && precompiledReferencesStart >= 0 && writableLines[line].Contains("]"))
                    {
                        precompiledReferencesEnd = line;
                    }
                }
                // if we have [] on the same line
                if (precompiledReferencesStart == precompiledReferencesEnd)
                {
                    writableLines[precompiledReferencesEnd] = writableLines[precompiledReferencesEnd].Replace("[]", "[\"" + assemblyFileName + "\"]");
                }
                else
                {
                    // insert a line in the form "assemblyfile.dll" or "assemblyfile.dll", depending on if there is already at least on precompiled reference
                    string coma = (precompiledReferencesEnd - precompiledReferencesStart == 1) ? "" : ",";
                    writableLines.Insert(precompiledReferencesStart + 1, "\"" + assemblyFileName + "\"" + coma);
                }
                //Write the lines back to the file
                File.WriteAllLines(file, writableLines);
            }
        }
    }

    /// <summary>
    /// Replace the GUID of an asset
    /// </summary>
    /// <param name="assetPath">The asset path</param>
    /// <param name="newGuid">The new GUID to set</param>
    private static void ReplaceIdOfAsset(string assetPath, string newGuid)
    {
        string[] fileLines = File.ReadAllLines(assetPath + ".meta");
        for (int line = 0; line < fileLines.Length; line++)
        {
            //find all instances of the string "guid: " and grab the next 32 characters as the old GUID
            if (fileLines[line].Contains("guid: "))
            {
                int index = fileLines[line].IndexOf("guid: ") + 6;
                string oldGUID = fileLines[line].Substring(index, 32); // GUID has 32 characters.
                if (oldGUID == newGuid)
                {
                    Debug.Log($"Asset at {assetPath} was already {newGuid}");
                    return;
                }
                fileLines[line] = fileLines[line].Replace(oldGUID, newGuid);
                Debug.Log($"Replaced asset at {assetPath} GUID from {oldGUID} to {newGuid}");
            }
        }
        //Write the lines back to the file
        File.WriteAllLines(assetPath + ".meta", fileLines);
    }

    /// <summary>
    /// Get the GUID of an asset
    /// </summary>
    /// <param name="assetPath">The asset path</param>
    private static GUID GUIDFromAssetPath(string assetPath)
    {
        string[] fileLines = File.ReadAllLines(assetPath + ".meta");
        for (int line = 0; line < fileLines.Length; line++)
        {
            //find all instances of the string "guid: " and grab the next 32 characters as the old GUID
            if (fileLines[line].Contains("guid: "))
            {
                int index = fileLines[line].IndexOf("guid: ") + 6;
                string guid = fileLines[line].Substring(index, 32); // GUID has 32 characters.
                {
                    Debug.Log($"Asset at {assetPath} has GUID {guid}");
                    return new GUID(guid);
                }
            }
        }
        return new GUID();
    }

    /// <summary>
    /// Replace ids in all asset files using the given maps
    /// </summary>
    /// <param name="oldGUIDToClassNameMap">Maps GUID to be replaced => FullClassName</param>
    /// <param name="newMonoScriptToIDsMap">Maps FullClassName => new GUID, new FileID</param>
    private static void ReplaceIdsInAssets(Dictionary<string, string> oldGUIDToClassNameMap, Dictionary<string, KeyValuePair<string, long>> newMonoScriptToIDsMap)
    {
        StringBuilder output = new("Report of replaced ids : \n");
        // list all the potential files that might need guid and fileID update
        List<string> fileList = new();
        foreach (string extension in fileListPath)
        {
            fileList.AddRange(Directory.GetFiles(Application.dataPath, extension, SearchOption.AllDirectories));
            fileList.AddRange(Directory.GetFiles(Path.GetFullPath("Packages"), extension, SearchOption.AllDirectories));
        }

        foreach (string file in fileList)
        {
            string[] fileLines = File.ReadAllLines(file);

            for (int line = 0; line < fileLines.Length; line++)
            {
                //find all instances of the string "guid: " and grab the next 32 characters as the old GUID
                if (fileLines[line].Contains("guid: "))
                {
                    int index = fileLines[line].IndexOf("guid: ") + 6;
                    string oldGUID = fileLines[line].Substring(index, 32); // GUID has 32 characters.
                    if (oldGUIDToClassNameMap.ContainsKey(oldGUID) && newMonoScriptToIDsMap.ContainsKey(oldGUIDToClassNameMap[oldGUID]))
                    {
                        fileLines[line] = fileLines[line].Replace(oldGUID, newMonoScriptToIDsMap[oldGUIDToClassNameMap[oldGUID]].Key);
                        _ = output.AppendFormat("File {0} : Found GUID {1} of class {2}. Replaced with new GUID {3}.", file, oldGUID, oldGUIDToClassNameMap[oldGUID],
                            newMonoScriptToIDsMap[oldGUIDToClassNameMap[oldGUID]].Key);
                        if (fileLines[line].Contains("fileID: "))
                        {
                            index = fileLines[line].IndexOf("fileID: ") + 8;
                            int index2 = fileLines[line].IndexOf(",", index);
                            string oldFileID = fileLines[line][index..index2]; // GUID has 32 characters.
                            fileLines[line] = fileLines[line].Replace(oldFileID, newMonoScriptToIDsMap[oldGUIDToClassNameMap[oldGUID]].Value.ToString());
                            _ = output.AppendFormat("Replaced fileID {0} with {1}", oldGUID, newMonoScriptToIDsMap[oldGUIDToClassNameMap[oldGUID]].Value.ToString());
                        }

                        _ = output.Append("\n");
                    }
                }
            }

            //Write the lines back to the file
            File.WriteAllLines(file, fileLines);
        }

        Debug.Log(output.ToString());
    }

    /// <summary>
    /// Recursively remove empty directories
    /// </summary>
    /// <param name="directory"></param>
    private static void RemoveEmptyDirectories(string directory)
    {
        if (Directory.Exists(directory) &&
            Directory.GetFiles(directory).Length == 0 &&
           Directory.GetDirectories(directory).Length == 0)
        {
            Debug.Log("Removing empty directory : " + directory);
            Directory.Delete(directory, true);
            string parentDirectory = Directory.GetParent(directory).FullName;
            // delete directory meta file
            File.Delete(directory + ".meta");
            RemoveEmptyDirectories(parentDirectory);
        }
    }

    private void RevertReplaceAssemblies()
    {
        Debug.Log(pathsOfAssemblyFilesInAssetFolder.Count);
        for (int i = 0; i < pathsOfAssemblyFilesInAssetFolder.Count; ++i)
        {
            Debug.Log("Will move " + pathsOfAssemblyFilesInAssetFolder[i] + " back to " + pathsOfAssemblyFilesCreatedByUnity[i]);
            FileUtil.MoveFileOrDirectory(pathsOfAssemblyFilesInAssetFolder[i], pathsOfAssemblyFilesCreatedByUnity[i]);
        }

        if (Directory.Exists(TempSourceFilePath))
        {
            string[] scriptFilesInTempDir = Directory.GetFiles(TempSourceFilePath, "*", SearchOption.AllDirectories);
            foreach (string scriptFileInTempDir in scriptFilesInTempDir)
            {
                // remove the temp directories prefix and the directory separator character
                string originalScriptFilePath = scriptFileInTempDir[(TempSourceFilePath.Length + 1)..];
                Debug.Log("Will move " + scriptFileInTempDir + " back to " + originalScriptFilePath);
                FileUtil.MoveFileOrDirectory(scriptFileInTempDir, originalScriptFilePath);
            }
        }

        pathsOfAssemblyFilesInAssetFolder = new List<string>();
        pathsOfAssemblyFilesCreatedByUnity = new List<string>();
    }

    #endregion
}
