using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Build.Reporting;

using UnityEngine;

[InitializeOnLoad]
public static class CommandLineBuilder
{
    #region Fields

    private static readonly string BATCH_MODE_PARAM = "-batchmode";
    private static readonly string SET_RUMTIME_COMMAND = "-executeMethod CommandLineBuilder.SetRuntime";
    private static readonly string BUILD_COMMAND = "-executeMethod CommandLineBuilder.Build";
    private static readonly string BUILD_PARAM = "-build";
    private static readonly string SCENE_PARAM = "-scenes";
    private static readonly string TARGET_PARAM = "-target";
    private static readonly string RUNTIME_PARAM = "-runtime";
    private static readonly string API_LEVEL_PARAM = "-api_level";
    private static readonly string BACKEND_PARAM = "-backend";

    private static List<string> args;

    #endregion

    #region Constructors

    static CommandLineBuilder()
    {
        if (System.Environment.GetCommandLineArgs().Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
        {
            Debug.LogFormat("CommandLineBuilder will try to parse the command line to automate the build. If you need to change the runtime, you need to do it before on a separate run of unity editor\n" +
                "\t To set the runtime, use {0}\n" +
                "\t\t Use the {5} parameter to specify the runtime (default: .NET 4.x)\n" +
                "\t Use {1} {2} \"executable pathname\"\n" +
                "\t\t Use the {3} \"scenefile1\";\"scenefile2\";... parameter to specify the scenes to include in the build\n" +
                "\t\t Use the {4} or -buildTarget [win32|win64] parameter to specify the targeted platform (default: win64)\n" +
                "\t\t Use the {6} parameter to specify the API level (default: .NET .2.0 for 3.5 runtime and .NET 4.x for 4.x runtime)\n" +
                "\t\t Use the {7} parameter to specify the scripting scripting backend (default: mono)\n"
                , SET_RUMTIME_COMMAND, BUILD_COMMAND, BUILD_PARAM, SCENE_PARAM, TARGET_PARAM, RUNTIME_PARAM, API_LEVEL_PARAM, BACKEND_PARAM);
        }
    }

    #endregion

    #region Methods

    public static bool BuildWithOptions(string buildPath, string[] scenes, BuildTarget buildTarget, ApiCompatibilityLevel apiLevel, ScriptingImplementation scriptingBackend)
    {
        BuildPlayerOptions buildPlayerOptions = new()
        {
            options = BuildOptions.None,

            locationPathName = buildPath,
            scenes = scenes,
            target = buildTarget,
            targetGroup = BuildTargetGroup.Standalone
        };

        PlayerSettings.SetApiCompatibilityLevel(buildPlayerOptions.targetGroup, apiLevel);
        PlayerSettings.SetScriptingBackend(buildPlayerOptions.targetGroup, scriptingBackend);

        string preBuildMessage = string.Format("******************************\n" +
                "Starting build with the following parameters : \n" +
                "executable : {0} - scene(s) : {1} - target : {2}\n" +
                "API Compatibility Level : {3} - Scripting Backend : {4}\n" +
                "******************************", buildPlayerOptions.locationPathName, string.Join(" ", buildPlayerOptions.scenes), buildPlayerOptions.target,
                PlayerSettings.GetApiCompatibilityLevel(buildPlayerOptions.targetGroup), PlayerSettings.GetScriptingBackend(buildPlayerOptions.targetGroup));

        Debug.Log(preBuildMessage);

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result != BuildResult.Succeeded)
        {
            string res = "";
            foreach (BuildStep step in report.steps)
            {
                foreach (BuildStepMessage message in step.messages)
                    //if (message.type == LogType.Error || message.type == LogType.Exception)
                    res += message.content + "\n";
            }

            Debug.LogError("Build report : \n" + res);

            Debug.LogErrorFormat("Buidling failed : {0}", report.summary.result);
            return false;
        }
        return true;
    }

    public static void Build()
    {
        args = System.Environment.GetCommandLineArgs().ToList();

        string buildPath = string.Empty;

        int build_index = args.IndexOf(BUILD_PARAM);
        if (build_index >= 0 && build_index < args.Count - 1)
        {
            buildPath = args[build_index + 1].Replace("\"", "");
        }
        else
        {
            Debug.LogErrorFormat("CommandLineBuilder could not parse the {0} parameter in the command line", BUILD_PARAM);
        }

        ApiCompatibilityLevel apiLevel = GetApiLevel();

        ScriptingImplementation scriptingBackend = GetScriptingBackend();

        if (!BuildWithOptions(buildPath, GetScenes(), GetTarget(), apiLevel, scriptingBackend))
            EditorApplication.Exit(1);
    }

    private static string[] GetScenes()
    {
        string[] scenes = { };
        int scene_index = args.IndexOf(SCENE_PARAM);
        if (scene_index >= 0 && scene_index < args.Count - 1)
        {
            scenes = args[scene_index + 1].Split(';').Select(entry => entry.Replace("\"", "")).ToArray();
        }
        else
        {
            Debug.LogErrorFormat("CommandLineBuilder could not parse the {0} parameter in the command line", SCENE_PARAM);
        }

        return scenes;
    }

    private static BuildTarget GetTarget()
    {
        BuildTarget target = BuildTarget.StandaloneWindows64;
        int target_index = args.IndexOf(TARGET_PARAM);
        if (target_index >= 0 && target_index < args.Count - 1)
        {
            target = args[target_index + 1] switch
            {
                "win32" => BuildTarget.StandaloneWindows,
                "win64" => BuildTarget.StandaloneWindows64,
                _ => BuildTarget.StandaloneWindows64,
            };
        }
        else
        {
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. Default is win64", TARGET_PARAM);
        }

        return target;
    }

    private static ApiCompatibilityLevel GetApiLevel()
    {
        ApiCompatibilityLevel apiLevel = ApiCompatibilityLevel.NET_4_6;
        int api_level_index = args.IndexOf(API_LEVEL_PARAM);
        if (api_level_index >= 0 && api_level_index < args.Count - 1)
        {
            apiLevel = args[api_level_index + 1] switch
            {
                "4.x" => ApiCompatibilityLevel.NET_4_6,
                "standard2.0" => ApiCompatibilityLevel.NET_Standard_2_0,
                _ => ApiCompatibilityLevel.NET_4_6,
            };
        }
        else
        {
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. " +
                "Valid value are : \n" +
                "For .NET 4.x (Default is 4.x)\n" +
                "\t\"4.x\" for .NET 4.x\n"
                + "\t\"standard2.0\" for .NET standard 2.0"
                , API_LEVEL_PARAM);
        }

        return apiLevel;
    }

    private static ScriptingImplementation GetScriptingBackend()
    {
        ScriptingImplementation scriptingBackend = ScriptingImplementation.Mono2x;
        int backend_index = args.IndexOf(BACKEND_PARAM);
        if (backend_index >= 0 && backend_index < args.Count - 1)
        {
            scriptingBackend = args[backend_index + 1] switch
            {
                "il2cpp" => ScriptingImplementation.IL2CPP,
                "winrt" => ScriptingImplementation.WinRTDotNET,
                "mono" => ScriptingImplementation.Mono2x,
                _ => ScriptingImplementation.Mono2x,
            };
        }
        else
        {
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. " +
                "Valid value are : \n" +
                "\"mono\" for Mono backend\n" +
                "\"il2cpp\" for IL2CPP backend\n" +
                "\"winrt\" for WinRTDotNET backend\n" +
                "Default is : mono", BACKEND_PARAM);
        }

        return scriptingBackend;
    }

    #endregion
}
