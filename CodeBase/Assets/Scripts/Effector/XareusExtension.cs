using UnityEngine;
using System;

/// <summary>
/// Extension methods for DetectInteraction and Expect.
/// </summary>
public static class XareusExtensions
{
    // -------------------------------------------------------
    // GameObject extensions
    // -------------------------------------------------------

    public static void DetectInteraction(this GameObject go, Func<GameObject, bool> condition)
    {
        if (go == null)
        {
            Debug.LogError("DetectInteraction: GameObject is null.");
            return;
        }

        bool result = condition(go);

        DetectInteractionInterceptor.Record(result);

        // ← Log every call with position info
        Debug.Log($"SENSOR {go.name} - DetectInteraction called - Result: {result} - Position: {go.transform.position}");
    }

    public static void Expect(this GameObject go, Func<GameObject, bool> condition)
    {
        if (go == null) return;

        bool result = condition(go);

        ExpectInterceptor.Record(result); // ignored during polling

        if (!ExpectInterceptor.IsIgnored) //  only log when not polling
        {
            if (result)
                Debug.Log($"ORACLE {go.name} - TestPassed - Expectation met.");
            else
                Debug.LogError($"ORACLE {go.name} - TestFailed - Expectation not met.");
        }
    }

    // -------------------------------------------------------
    // Component extensions (Button, Slider, etc.)
    // -------------------------------------------------------

    public static void DetectInteraction(this Component c, Func<GameObject, bool> condition)
        => c.gameObject.DetectInteraction(condition);

    public static void Expect(this Component c, Func<GameObject, bool> condition)
        => c.gameObject.Expect(condition);
}