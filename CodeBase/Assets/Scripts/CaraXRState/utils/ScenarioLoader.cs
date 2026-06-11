using System.IO;
using System.Reflection;
using UnityEngine;
using Xareus.Scenarios.Unity;

public class ScenarioLoader : MonoBehaviour
{
    private ScenarioEngineKernel scenarioEngineKernel;
    private ScenarioRunner scenarioRunner;

    private void Awake()
    {
        scenarioEngineKernel = FindFirstObjectByType<ScenarioEngineKernel>();
        scenarioRunner = FindFirstObjectByType<ScenarioRunner>();

        if (scenarioEngineKernel == null)
            Debug.LogError("[ScenarioLoader] ScenarioEngineKernel not found!");
        if (scenarioRunner == null)
            Debug.LogError("[ScenarioLoader] ScenarioRunner not found!");
    }

    public void GenerateAndLoad<T>() where T : class
    {
        string xml = PetriNetXmlGenerator.Generate<T>();
        string path = Path.Combine(Application.dataPath, "Scenarios", $"{typeof(T).Name}.xml");

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, xml);

        Debug.Log($"[ScenarioLoader] Generated XML:\n{xml}");

        LoadFromPath(path);
    }

    public void LoadFromPath(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[ScenarioLoader] File not found: {fullPath}");
            return;
        }

        // Try ScenarioRunner first
        if (TrySetOnScenarioRunner(fullPath))
            return;

        // Fallback — try ScenarioEngineKernel
        TrySetOnKernel(fullPath);
    }

    private bool TrySetOnScenarioRunner(string fullPath)
    {
        if (scenarioRunner == null) return false;

        // Log all string fields on ScenarioRunner
        foreach (FieldInfo field in scenarioRunner.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.FieldType != typeof(string)) continue;

            Debug.Log($"[ScenarioLoader] ScenarioRunner string field: {field.Name}");

            if (field.Name.ToLower().Contains("scenario") || field.Name.ToLower().Contains("file"))
            {
                field.SetValue(scenarioRunner, fullPath);
                Debug.Log($"[ScenarioLoader] Set ScenarioRunner.{field.Name} → {fullPath}");

                // Restart kernel
                scenarioEngineKernel.enabled = false;
                scenarioEngineKernel.enabled = true;
                return true;
            }
        }

        return false;
    }

    private void TrySetOnKernel(string fullPath)
    {
        if (scenarioEngineKernel == null) return;

        foreach (FieldInfo field in scenarioEngineKernel.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.FieldType != typeof(string)) continue;

            Debug.Log($"[ScenarioLoader] ScenarioEngineKernel string field: {field.Name}");

            if (field.Name.ToLower().Contains("scenario") || field.Name.ToLower().Contains("file"))
            {
                field.SetValue(scenarioEngineKernel, fullPath);
                Debug.Log($"[ScenarioLoader] Set ScenarioEngineKernel.{field.Name} → {fullPath}");

                scenarioEngineKernel.enabled = false;
                scenarioEngineKernel.enabled = true;
                return;
            }
        }

        Debug.LogWarning("[ScenarioLoader] Could not find scenario file field on either component.");
    }
}