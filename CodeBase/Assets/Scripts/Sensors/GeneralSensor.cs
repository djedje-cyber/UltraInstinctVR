using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Abstract base class for sensors that monitor a GameObject.
/// Provides helper methods for result creation and unique key generation.
/// </summary>
public abstract class GeneralSensor : AInUnityStepSensor
{
    // The GameObject to monitor — available to all child sensors
    [ConfigurationParameter("GameObject to observe", Necessity.Required)]
    protected GameObject targetObject;

    protected SimpleDictionary eventContext = new SimpleDictionary();

    public GeneralSensor(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext)
        : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext))
    { }

    // -------------------------------------------------------
    // Overrides
    // -------------------------------------------------------

    public override void SafeReset()
    {
        eventContext = new SimpleDictionary();
        OnReset();
    }

    public override Result UnityStepSensorCheck()
    {
        return OnCheck();
    }

    // -------------------------------------------------------
    // Abstract methods — must be implemented by child sensors
    // -------------------------------------------------------

    /// <summary>
    /// Called on reset — override to initialize your sensor state.
    /// </summary>
    protected abstract void OnReset();

    /// <summary>
    /// Called each Unity step — override to implement your sensor logic.
    /// </summary>
    protected abstract Result OnCheck();

    // -------------------------------------------------------
    // Helper methods
    // -------------------------------------------------------

    /// <summary>
    /// Returns a successful result with the current event context.
    /// </summary>
    protected Result Success()
    {
        return new Result(true, eventContext);
    }

    /// <summary>
    /// Returns a failed result with the current event context.
    /// </summary>
    protected Result Failure()
    {
        return new Result(false, eventContext);
    }

    /// <summary>
    /// Adds a key/value to the event context and returns a result based on the condition.
    /// The key is automatically generated from the sensor class name and a GUID.
    /// </summary>
    protected Result ResultWith(bool condition, string value)
    {
        if (condition)
            eventContext.Add(GenerateKey(), value);

        return new Result(condition, eventContext);
    }

    /// <summary>
    /// Generates a unique key using the sensor class name and a GUID.
    /// e.g. "TeleportationSensor_a1b2c3-..."
    /// </summary>
    protected string GenerateKey()
    {
        return $"{GetType().Name}_{Guid.NewGuid()}";
    }
}