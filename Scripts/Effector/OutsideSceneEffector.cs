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
    private SizeScene sizeScene;

     private void CalculateSceneBounds()
    {
        sceneBounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            sceneBounds.Encapsulate(renderer.bounds);
        }

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

        sizeScene = Object.FindObjectOfType<SizeScene>(); // Recherche le composant dans la scène

        if (sizeScene == null)
        {
            Debug.LogError("SizeScene introuvable dans la scène !");
            return;
        }

        Debug.Log("Effector - Taille de la scène récupérée : " + sizeScene.sceneBounds.size);

            // Initialiser la position du joueur
            lastPosition = GetPlayerPosition();
    }

    public override void SafeEffectorUpdate()
    {
        Vector3 currentPosition = GetPlayerPosition();
        Debug.Log("Xareus Current Position : " + currentPosition);       

        if(currentPosition.x > sceneBounds.size.x | currentPosition.y > sceneBounds.size.y | currentPosition.z > sceneBounds.size.z) {

            Debug.LogError("Failed - A player cannot teleport outside the scene");
        }
    }

    private  Vector3 GetPlayerPosition()
    {
        return Camera.main.transform.position;
    }



}
