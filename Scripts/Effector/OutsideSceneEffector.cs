using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class OutsideSceneEffector : AUnityEffector
{



    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f;

    private Vector3 lastPosition;
    private Bounds sceneBounds; // Nouvelle variable pour stocker les limites de la scène


     private void CalculateSceneBounds()
    {
        sceneBounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            sceneBounds.Encapsulate(renderer.bounds);
        }

        Debug.Log("Taille de la scène : " + sceneBounds.size);
        Debug.Log("Centre de la scène : " + sceneBounds.center);
        Debug.Log("Taille X:" + sceneBounds.size.x);
    }


    public OutsideSceneEffector(Xareus.Scenarios.Event @event, 
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext,
        IContext anotherContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, anotherContext) 
    { }

    public override void SafeReset()
    {
        // Détecter automatiquement les limites de la scène
        CalculateSceneBounds();

        // Initialiser la position du joueur
        lastPosition = GetPlayerPosition();
    }

    public override void SafeEffectorUpdate()
    {
        Vector3 currentPosition = GetPlayerPosition();

        if(CurrentPosition.x > sceneBounds.size.x | CurrentPosition.y > sceneBounds.size.y | CurrentPosition.z > sceneBounds.size.z) {

        Debug.Log("Xareus Current Position : " + currentPosition.x);       

    }

    private Vector3 GetPlayerPosition()
    {
        return Camera.main.transform.position;
    }


}