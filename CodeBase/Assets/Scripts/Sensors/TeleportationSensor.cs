using System;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;



[FunctionDescription("A Teleportation Detection Sensor")]

/// <summary>
/// Class <c>TeleportSensor</c> detects when the player teleports by monitoring significant changes in position.
/// </summary>
public class TeleportationSensor : AInUnityStepSensor
{
    // Key to add it to the event context
    [EventContextEntry()]
    public static readonly string TELEPORT_KEY = "TeleportPosition";

    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f; // Distance in Unity units to consider as teleportation

    [ConfigurationParameter("ObjectToObserve", Necessity.Required)]
    protected GameObject objectToObserve;

    private Vector3 lastPosition;
    private SimpleDictionary eventContext = new SimpleDictionary();



    /// <summary>
    /// Method <c>TeleportationSensor</c> initializes the sensor with the given parameters.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="nameValueListMap"></param>
    /// <param name="externalContext"></param>
    /// <param name="scenarioContext"></param>
    /// <param name="sequenceContext"></param>

    public TeleportationSensor(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext)
       :base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext))

    { } 


    /// <summary>
    /// Method <c>SafeReset</c> resets the sensor's state, initializing the last known position of the player.
    /// </summary>
    public override void SafeReset()
    {
        // Initialize the last position to the user's initial position
        lastPosition = GetPlayerPosition();
    }


    /// <summary>
    /// Method <c>UnityStepSensorCheck</c> checks for teleportation by comparing the current position with the last known position.
    /// </summary>
    /// <returns></returns>
    public override Result UnityStepSensorCheck()
    {
        Vector3 currentPosition = GetPlayerPosition();
        if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
        {

            //Assign eventContext
            string teleportKey = $"{TELEPORT_KEY}_{Guid.NewGuid()}";

            // Detect teleportation
            eventContext.Add(teleportKey, currentPosition.ToString());
            lastPosition = currentPosition; 
            return new Result(true, eventContext);
        }

        return new Result(false, eventContext);
    }


    /// <summary>
    /// Method <c>GetPlayerPosition</c> retrieves the current position of the player or the observed object.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerPosition()
    {

        return objectToObserve.transform.position; 
    }
}




