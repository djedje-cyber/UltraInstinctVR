using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstract base class for effectors that act upon a GameObject.
/// Provides helper methods for logging and automatic message generation.
/// </summary>
public abstract class AGameObjectEffector : AUnityEffector
{
    [ConfigurationParameter("GameObject to observe", Necessity.Required)]
    protected GameObject targetObject;

    public AGameObjectEffector(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext,
        IContext eventContext)
        : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext, eventContext))
    { }

    // -------------------------------------------------------
    // Overrides
    // -------------------------------------------------------

    public override void SafeReset()
    {
        OnReset();
    }

    public override void SafeEffectorUpdate()
    {
        OnUpdate();
    }

    // -------------------------------------------------------
    // Abstract methods — must be implemented by child effectors
    // -------------------------------------------------------

    /// <summary>
    /// Called on reset — override to initialize your effector state.
    /// </summary>
    protected abstract void OnReset();

    /// <summary>
    /// Called when the transition fires — override to implement your effector logic.
    /// </summary>
    protected abstract void OnUpdate();

    // -------------------------------------------------------
    // Helper methods
    // -------------------------------------------------------

    /// <summary>
    /// Logs a success message with the ORACLE format.
    /// Automatically includes the class name, target object name and position
    /// </summary>
    protected void LogSuccess()
    {
        Debug.Log($"ORACLE {GetType().Name} - TestPassed - {targetObject.name} at {targetObject.transform.position}");
    }

    /// <summary>
    /// Logs a failure message with the ORACLE format.
    /// Automatically includes the class name, target object name and position.
    /// </summary>
    protected void LogFailure()
    {
        Debug.LogError($"ORACLE {GetType().Name} - Failed - {targetObject.name} at {targetObject.transform.position}");
    }

    /// <summary>
    /// Logs a success or failure message based on the condition.
    /// </summary>
    protected void LogResult(bool success)
    {
        if (success) LogSuccess();
        else LogFailure();
    }
}