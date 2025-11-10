using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;



namespace ColissionEffectorSpace
{
    [FunctionDescription("Collision Detection Effect")]
    public class CollisionEffector : AUnityEffector
    {
        // Cube to monitor for collision detection
        [ConfigurationParameter("Cube", Necessity.Required)]
        protected GameObject cube;

        // Detection radius for collision
        [ConfigurationParameter("Collision Detection Radius", Necessity.Required)]
        protected float collisionDetectionRadius = 0.5f;

        public CollisionEffector(Xareus.Scenarios.Event @event,
            Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
            IContext externalContext,
            IContext scenarioContext,
            IContext sequenceContext,
            IContext eventContext)
            : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, eventContext)
        { }

        // Method called every frame to check for collisions
        public override void SafeEffectorUpdate()
        {
            Debug.Log("TestGenerated - CollisionDetection");
            DetectCollision();
        }

        private void DetectCollision()
        {
            // Detect all colliders within a sphere around the cube
            Collider[] hitColliders = Physics.OverlapSphere(cube.transform.position, collisionDetectionRadius);

            // If objects are colliding with the "cube" object
            if (hitColliders.Length > 0)
            {
                foreach (var hitCollider in hitColliders)
                {
                    // Check that the detected object is not the cube itself
                    if (hitCollider != null && hitCollider.gameObject != cube)
                    {
                        // Collision detected with another object (excluding the cube itself)
                        Debug.Log("ORACLE CollisionDetection - TestPassed - Collision successfully done with "
                                + hitCollider.name + " at: " + cube.transform.position);

                        // Return to exit the loop once the collision has been handled
                        return;
                    }
                }

                // If no valid collision was found, display a failure message
                Debug.LogError("ORACLE CollisionDetection - TestFailed - No valid collision detected at: "
                            + cube.transform.position);
            }
            else
            {
                // No collision detected
                Debug.LogError("ORACLE CollisionDetection - TestFailed - No collision detected at: "
                            + cube.transform.position);
            }
        }
    }

}