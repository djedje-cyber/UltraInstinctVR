using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;


namespace TeleportationEffectorSpace
{


    /// <summary>
    /// Class <c>TeleportationEffector</c> teleports the player when triggered by an event.
    /// </summary>
    public class TeleportationEffector : AUnityEffector
    {
        // Example of a configuration parameter
        [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
        protected float teleportDistanceThreshold = 2.0f;

        [ConfigurationParameter("GameObjectToObserve", Necessity.Required)]
        protected GameObject gameObjectToObserve;

        private Vector3 lastPosition;

          

        /// <summary>
        /// Method  <c>TeleportationEffector</c> initializes the effector with the given parameters.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="nameValueListMap"></param>
        /// <param name="externalContext"></param>
        /// <param name="scenarioContext"></param>
        /// <param name="sequenceContext"></param>
        /// <param name="anotherContext"></param>
        public TeleportationEffector(Xareus.Scenarios.Event @event,
            Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
            IContext externalContext,
            IContext scenarioContext,
            IContext sequenceContext,
            IContext anotherContext)
            : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext, anotherContext))

        { }



        /// <summary>
        /// Method <c>SafeReset</c> resets the effector's state, initializing the last known position of the player.
        /// </summary>
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

        /// <summary>
        /// Method <c>GetPlayerPosition</c> retrieves the current position of the player.
        /// </summary>
        /// <returns>Position of the player</returns>
        private Vector3 GetPlayerPosition()
        {
            // Example of getting the camera's position (the player's avatar)
            return gameObjectToObserve.transform.position;
        }
    }
}
