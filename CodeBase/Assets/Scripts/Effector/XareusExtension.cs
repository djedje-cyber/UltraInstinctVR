using UnityEngine;
using System;

public static class XareusExtensions
{
    // -------------------------------------------------------
    // DetectInteraction — Sensor
    // Evaluates the condition and logs whether the interaction
    // was detected or not.
    // -------------------------------------------------------

    public static void DetectInteraction(this GameObject go, Func<GameObject, bool> condition)
    {
        if (go == null)
        {
            Debug.LogError("DetectInteraction: GameObject is null.");
            return;
        }

        bool result = condition(go);

        if (result)
            Debug.Log($"SENSOR {go.name} - Interaction detected.");
        else
            Debug.LogWarning($"SENSOR {go.name} - No interaction detected.");
    }

    public static void DetectInteraction(this Component component, Func<GameObject, bool> condition)
    {
        component.gameObject.DetectInteraction(condition);
    }

    // -------------------------------------------------------
    // Expect — Effector / Oracle
    // Evaluates the expected condition and logs pass or fail.
    // -------------------------------------------------------

    public static void Expect(this GameObject go, Func<GameObject, bool> condition)
    {
        if (go == null)
        {
            Debug.LogError("Expect: GameObject is null.");
            return;
        }

        bool result = condition(go);

        if (result)
            Debug.Log($"ORACLE {go.name} - TestPassed - Expectation met.");
        else
            Debug.LogError($"ORACLE {go.name} - TestFailed - Expectation not met.");
    }

    public static void Expect(this Component component, Func<GameObject, bool> condition)
    {
        component.gameObject.Expect(condition);
    }
}