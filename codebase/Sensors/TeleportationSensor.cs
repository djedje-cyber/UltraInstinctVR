using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System;
[FunctionDescription("A Teleportation Detection Sensor")]
public class TeleportationSensor : AInUnityStepSensor
{
    // Clé pour l'ajouter dans le contexte d'événement
    [EventContextEntry()]
    public static readonly string TELEPORT_KEY = "TeleportPosition";

    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f;

    [ConfigurationParameter("ObjectToObserve", Necessity.Required)]
    protected GameObject objectToObserve;

    private Vector3 lastPosition;
    private SimpleDictionary eventContext = new SimpleDictionary();

    public TeleportationSensor(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, // Corriger le type ici
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext)
    { }

    public override void SafeReset()
    {
        // Initialisation de la dernière position à la position initiale de l'utilisateur
        lastPosition = GetPlayerPosition();
    }

    public override Result UnityStepSensorCheck()
    {
        Vector3 currentPosition = GetPlayerPosition();
        if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
        {

            //Assign eventContext
            string teleportKey = $"{TELEPORT_KEY}_{Guid.NewGuid()}";

            // Détecter une téléportation
            eventContext.Add(teleportKey, currentPosition.ToString());
            lastPosition = currentPosition; 
            return new Result(true, eventContext);
        }

        return new Result(false, eventContext);
    }

    private Vector3 GetPlayerPosition()
    {

        return objectToObserve.transform.position; 
    }
}




