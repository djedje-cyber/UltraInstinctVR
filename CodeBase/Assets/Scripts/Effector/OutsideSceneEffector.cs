using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.IO;


namespace OutsideSceneEffectorSpace
{

    public class OutsideSceneEffector : AUnityEffector
    {
        [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
        protected float teleportDistanceThreshold = 1.0f;


        [ConfigurationParameter("GameObjectToObserve", Necessity.Required)]
        protected GameObject gameObjectToObserve;

        private Vector3 lastPosition;
        private readonly string sceneBoundsFilePath = "Assets/Scripts/CoveredObjects/sceneBounds.txt"; // Chemin du fichier

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

            // Initialiser la position du joueur
            lastPosition = GetPlayerPosition();
        }



        public static Bounds GetSceneBounds()
        {
            // Get all renderers in the scene
            Renderer[] renderers = Object.FindObjectsOfType<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogWarning("No renderers found in the scene.");
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            // Initialize bounds to the first renderer
            Bounds sceneBounds = renderers[0].bounds;

            // Encapsulate all other renderers
            for (int i = 1; i < renderers.Length; i++)
            {
                sceneBounds.Encapsulate(renderers[i].bounds);
            }

            return sceneBounds;
        }


        public override void SafeEffectorUpdate()
        {
            Vector3 currentPosition = GetPlayerPosition();
            Vector3 sceneBounds = GetSceneBounds().extents;
            Debug.Log("TestGenerated - OutsideSceneTeleportation");

            // Check if the player is outside the boundaries
            if (currentPosition.x < -sceneBounds.x || currentPosition.x > sceneBounds.x ||
                currentPosition.y < -sceneBounds.y || currentPosition.y > sceneBounds.y ||
                currentPosition.z < -sceneBounds.z || currentPosition.z > sceneBounds.z)
            {
                Debug.LogError("ORACLE OutsideSceneTeleportation - TestFailed - A player cannot teleport outside the scene at position: " + currentPosition);
            }
            else
            {
                Debug.Log("ORACLE OutsideSceneTeleportation - TestPassed - A player can teleport inside the scene at position: " + currentPosition);
            }
        }

        private Vector3 GetPlayerPosition()
        {
            return gameObjectToObserve.transform.position;
        }
    }

}