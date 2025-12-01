using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

[FunctionDescription("A Teleportation Detection Sensor at precise object position")]


/// <summary>
/// Class <c>SelectionSensor</c> detects when the selected cube teleports by monitoring significant changes in its position.
/// </summary>

public class SelectionSensor : AInUnityStepSensor
{
    // Clé pour l'ajouter dans le contexte d'événement
    [EventContextEntry()]
    public static readonly string TELEPORT_KEY = "TeleportPosition";

    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f;

    // Le cube à surveiller
    [ConfigurationParameter("Cube to observe", Necessity.Required)]
    protected GameObject cube;

    private Vector3 lastPosition;
    private SimpleDictionary eventContext = new SimpleDictionary();

    public SelectionSensor(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, // Corriger le type ici
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext)
        : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext))
    { }




    /// <summary>
    /// Method <c>SafeReset</c> resets the sensor's state, initializing the last known position of the cube.
    /// </summary>
    public override void SafeReset()
    {

        Debug.Log("Cube position: " + cube.transform.position);

        lastPosition = GetPlayerPosition();
    }


    /// <summary>
    /// Method <c>UnityStepSensorCheck</c> checks if the cube has teleported by comparing its current position to the last known position.
    /// </summary>
    /// <returns></returns>

    public override Result UnityStepSensorCheck()
    {
        Vector3 currentPosition = GetPlayerPosition();  // Utilise la position du cube

        if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
        {
            // Créer une clé unique pour l'événement de téléportation
            string teleportKey = $"{TELEPORT_KEY}_{Guid.NewGuid()}";

            // Ajouter l'événement de téléportation dans le contexte
            eventContext.Add(teleportKey, currentPosition.ToString());
            lastPosition = currentPosition; // Mettre à jour la position du cube
            return new Result(true, eventContext);  // Retourne un résultat positif avec les données de l'événement
        }

        return new Result(false, eventContext);  // Retourne un résultat négatif si pas de téléportation
    }

    /// <summary>
    /// Method <c>GetPlayerPosition</c> retrieves the current position of the cube.
    /// </summary>
    /// <returns></returns>

    private Vector3 GetPlayerPosition()
    {
        return cube.transform.position;
    }
}
