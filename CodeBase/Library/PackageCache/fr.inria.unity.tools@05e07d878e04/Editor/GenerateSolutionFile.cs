using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

[InitializeOnLoad]
public static class GenerateSolutionFile
{

    private static readonly string BATCH_MODE_PARAM = "-batchmode";
    private static readonly string GENERATE_SOLUTION_PARAM = "-generatesolution";


    static GenerateSolutionFile()
    {
        List<string> args = Environment.GetCommandLineArgs().ToList();

        if (args.Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
        {
            UnityEngine.Debug.LogFormat(
                "GenerateSolutionFile will try to parse the command line to generate the solution file.\n" +
                "\t Use {0} or -executeMethod GenerateSolutionFile.GenerateFiles"
                , GENERATE_SOLUTION_PARAM);
        }

        args = Environment.GetCommandLineArgs().ToList();
        for (int i = 0 ; i < args.Count ; i++)
        {
            if (args[i].ToLower().Equals(GENERATE_SOLUTION_PARAM))
            {
                GenerateFiles();
            }
        }
    }

    public static void GenerateFiles()
    {
        UnityEngine.Debug.LogFormat("Will generate solution file");
#if UNITY_2020_1_OR_NEWER
        UnityEngine.Debug.LogFormat("Using Unity >= 2020.1");
        var sync_vs_type = Type.GetType("UnityEditor.SyncVS,UnityEditor");
        var sync_solution_mi = sync_vs_type.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
        sync_solution_mi.Invoke(null, null);
#elif UNITY_2019_3_OR_NEWER
        UnityEngine.Debug.LogFormat("Using Unity >= 2019.3");
        Type sync_vs_type = Type.GetType("UnityEditor.SyncVS,UnityEditor");

        FieldInfo synchronizer_field = sync_vs_type.GetField("Synchronizer", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo sync_solution_mi = sync_vs_type.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);

        object synchronizer_object = synchronizer_field.GetValue(sync_vs_type);
        Type synchronizer_type = synchronizer_object.GetType();
        MethodInfo synchronizer_sync_mi = synchronizer_type.GetMethod("Sync", BindingFlags.Public | BindingFlags.Instance);

        sync_solution_mi.Invoke(null, null);
        synchronizer_sync_mi.Invoke(synchronizer_object, null);

#else
        // OLD way (before 2019.3)
        UnityEngine.Debug.LogFormat("Using Unity < 2019.3");
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
#endif
    }
}