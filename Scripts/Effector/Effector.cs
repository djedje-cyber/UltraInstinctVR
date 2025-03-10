using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;

public class TeleportationEffector : AUnityEffector
{
    // Exemple de paramètre de configuration
    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f;

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

        // Si la distance entre la dernière position et la position actuelle dépasse un seuil, on peut considérer cela comme une téléportation
        if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
        {
            Debug.Log("Oracle Covered - Teleportation");
            lastPosition = currentPosition;  // Mettre à jour la position du joueur
        }
    }

    // Méthode pour obtenir la position actuelle du joueur
    private Vector3 GetPlayerPosition()
    {
        // Exemple de récupération de la position de la caméra (l'avatar du joueur)
        return Camera.main.transform.position;
    }
}




