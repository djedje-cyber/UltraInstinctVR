using System.IO;
using System.Reflection;
using UnityEngine;
using Xareus.Scenarios.Unity;

public class ScenarioLoader : MonoBehaviour
{
    [SerializeField] public ScenarioEngineKernel scenarioEngineKernel;

    private void Start()
    {
        //LogAllFields();
    }

    public void LoadFromPath(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[ScenarioLoader] File not found: {fullPath}");
            return;
        }

        SetScenarioFile(fullPath);

        // Restart the kernel to pick up the new scenario
        scenarioEngineKernel.enabled = false;
        scenarioEngineKernel.enabled = true;

        Debug.Log($"[ScenarioLoader] Loaded: {fullPath}");
    }

    public void GenerateAndLoad<T>() where T : class
    {
        string xml = PetriNetXmlGenerator.Generate<T>();
        string path = Path.Combine(Application.dataPath, "Scenarios", $"{typeof(T).Name}.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, xml);

        LoadFromPath(path);
    }

    private void SetScenarioFile(string fullPath)
    {
        string[] possibleFields = { "CreateLoadingParameters", "CreateExternalContext", "localExternalContextEntries", "ScenarioXml", "LoadFromFile", "ScenarioFileToLoad", "RunInSeparateThread", "UnityExecutionSteps", "ScenarioUpdateTime", "RunScenarioAfterLoad", "scenario", "context"};

        foreach (string fieldName in possibleFields)
        {
            FieldInfo field = scenarioEngineKernel.GetType().GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
            );

            if (field != null)
            {
                field.SetValue(scenarioEngineKernel, fullPath);
                Debug.Log($"[ScenarioLoader] Set '{fieldName}' → {fullPath}");
                return;
            }
        }

        Debug.LogWarning("[ScenarioLoader] Could not find scenario field. Call LogAllFields() to find the correct name.");
    }

    /// <summary>
    /// Call this once to find the exact field name in your version of Xareus.
    /// </summary>
    public void LogAllFields()
    {
        foreach (FieldInfo field in scenarioEngineKernel.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            Debug.Log($"[ScenarioLoader] Field: {field.Name} ({field.FieldType})");
        }
    }
}