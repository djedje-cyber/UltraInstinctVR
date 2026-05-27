using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Serializable wrapper for a Type — stores the class name
/// and resolves it at runtime.
/// </summary>
[Serializable]
public class SerializableType
{
    [SerializeField] private string className;

    public string ClassName => className;

    /// <summary>
    /// Resolves the stored class name to an actual Type at runtime.
    /// </summary>
    public Type Resolve()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(className);
            if (type != null && type.GetCustomAttribute<TestInteractionClassAttribute>() != null)
                return type;
        }

        Debug.LogWarning($"[SerializableType] Could not resolve type: {className}");
        return null;
    }
}