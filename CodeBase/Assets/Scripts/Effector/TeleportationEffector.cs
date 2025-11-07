using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;


namespace TeleportationEffectorSpace
{
    public class TeleportationEffector : AUnityEffector
    {
        // Example of a configuration parameter
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

        // Implementation of the SafeEffectorUpdate method (mandatory)
        public override void SafeEffectorUpdate()
        {
            Vector3 currentPosition = GetPlayerPosition();
            Debug.Log("TestGenerated - Teleportation");
            if (Vector3.Distance(currentPosition, lastPosition) > teleportDistanceThreshold)
            {
                Debug.Log("ORACLE CanTeleport - TestPassed - Teleportation successfully done at " + lastPosition);
                lastPosition = currentPosition;  // Update the player's position
            }
            else
            {
                Debug.LogError("ORACLE CanTeleport - Failed - Teleportation couldn't be done at " + lastPosition);
            }
        }

        // Method to get the current player position
        private Vector3 GetPlayerPosition()
        {
            // Example of getting the camera's position (the player's avatar)
            return gameObjectToObserve.transform.position;
        }
    }
}
