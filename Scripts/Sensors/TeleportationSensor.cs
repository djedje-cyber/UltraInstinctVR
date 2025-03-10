using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;

[FunctionDescription("A Teleportation Detection Sensor")]
public class TeleportationSensor : AInUnityStepSensor
{
    // Clé pour l'ajouter dans le contexte d'événement
    [EventContextEntry()]
    public static readonly string TELEPORT_KEY = "teleport_position";

    // Le paramètre de configuration
    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f;

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
        // Vérification à chaque étape de Unity si une téléportation a eu lieu
        Vector3 currentPosition = GetPlayerPosition();
        Debug.Log("coucou");
        // Vérification si la position a changé au-delà d'un certain seuil
        if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
        {
            // Détecter une téléportation
            eventContext.Add(TELEPORT_KEY, currentPosition.ToString());
            lastPosition = currentPosition;  // Mettre à jour la dernière position
            return new Result(true, eventContext);
        }

        return new Result(false, eventContext);
    }

    // Méthode pour obtenir la position actuelle du joueur dans l'environnement Unity
    private Vector3 GetPlayerPosition()
    {
        Debug.Log("Hello");
        // Exemple de récupération de la position de la caméra (l'avatar du joueur)
        // Vous pouvez ajuster cette logique selon la façon dont votre système de téléportation fonctionne dans Unity.
        return Camera.main.transform.position; 
    }
}




