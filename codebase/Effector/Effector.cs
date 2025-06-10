using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;

public class TeleportationEffector : AUnityEffector
{
    // Exemple de paramètre de configuration
    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 2.0f;

    [ConfigurationParameter("GameObjectToObserve", Necessity.Required)]
    protected GameObject gameObjectToObserve;

    private Vector3 lastPosition;

    public TeleportationEffector(Xareus.Scenarios.Event @event, 
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext,
        IContext anotherContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, anotherContext) 
    { }

    // Implémentation de la méthode SafeEffectorUpdate (obligatoire)
    public override void SafeEffectorUpdate()
    {

        Vector3 currentPosition = GetPlayerPosition();
        Debug.Log("TestGenerated - Teleportation");
        if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
        {
            Debug.Log("ORACLE CanTeleport - TestPassed - Teleportation successfully done at " + lastPosition );
            lastPosition = currentPosition;  // Mettre à jour la position du joueur
        }
        else
        {
            Debug.LogError("ORACLE CanTeleport - Failed - Teleportation couldn't be done at " + lastPosition);
        }
    }

    // Méthode pour obtenir la position actuelle du joueur
    private Vector3 GetPlayerPosition()
    {
        // Exemple de récupération de la position de la caméra (l'avatar du joueur)
        return gameObjectToObserve.transform.position;
    }
}