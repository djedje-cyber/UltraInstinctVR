using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Xareus.Unity
{
    /// <summary>
    /// Detects trigger collisions.
    /// </summary>
    public class UFEOtherTrigger : MonoBehaviour
    {
        #region Statics

        /// <summary>
        /// Maps triggers with their colliding triggers.
        /// </summary>
        private static readonly Dictionary<Collider, HashSet<Collider>> triggerTriggerSetMap = new Dictionary<Collider, HashSet<Collider>>();

        /// <summary>
        /// Maps game objects with their colliding game objects.
        /// </summary>
        private static readonly Dictionary<GameObject, HashSet<GameObject>> collidingGameObjects = new Dictionary<GameObject, HashSet<GameObject>>();

        private static readonly Dictionary<Collider, GameObject> collidersObjects = new Dictionary<Collider, GameObject>();

        #endregion

        #region Fields

        public List<Collider> colliders = new List<Collider>();

        #endregion

        #region Methods

        /// <summary>
        /// Indicates whether a trigger collides another trigger.
        /// </summary>
        /// <param name="trigger1">The first trigger</param>
        /// <param name="trigger2">The second trigger</param>
        /// <returns><c>true</c> whether the triggers are colliding; <c>false</c> otherwise</returns>
        public static bool AreColliding(Collider trigger1, Collider trigger2)
        {
            return triggerTriggerSetMap.ContainsKey(trigger1) && triggerTriggerSetMap[trigger1].Contains(trigger2);
        }

        /// <summary>
        /// Indicates whether an object collides another object.
        /// </summary>
        /// <param name="obj1">The object trigger</param>
        /// <param name="obj2">The object trigger</param>
        /// <returns><c>true</c> whether the objects are colliding; <c>false</c> otherwise</returns>
        public static bool AreColliding(GameObject obj1, GameObject obj2)
        {
            return collidingGameObjects.ContainsKey(obj1) && collidingGameObjects[obj1].Contains(obj2);
        }

        /// <summary>
        /// Get all triggers colliding with the given one
        /// </summary>
        /// <param name="trigger">The trigger</param>
        /// <returns>All the triggers colliding with the given one. Empty IEnumerable if the given trigger is not present</returns>
        public static IEnumerable<Collider> GetColliding(Collider trigger)
        {
            return triggerTriggerSetMap.FirstOrDefault(pair => pair.Key == trigger).Value;
        }

        /// <summary>
        /// Get all objects colliding with the given one
        /// </summary>
        /// <param name="trigger">The trigger</param>
        /// <returns>All the triggers colliding with the given one. Empty IEnumerable if the given trigger is not present</returns>
        public static IEnumerable<GameObject> GetColliding(GameObject obj)
        {
            return collidingGameObjects.FirstOrDefault(pair => pair.Key == obj).Value;
        }

        /// <summary>
        /// Check if the given trigger if colliding with any other one
        /// </summary>
        /// <param name="trigger">The trigger</param>
        /// <returns>True if this trigger is colliding with another one, false otherwise</returns>
        public static bool IsColliding(Collider trigger)
        {
            return triggerTriggerSetMap.Any(pair => pair.Key == trigger);
        }

        /// <summary>
        /// Check if the given object if colliding with any other one
        /// </summary>
        /// <param name="obj">The object holding the trigger</param>
        /// <returns>True if this object is colliding with another one, false otherwise</returns>
        public static bool IsColliding(GameObject obj)
        {
            return collidingGameObjects.ContainsKey(obj) && collidingGameObjects[obj].Count > 0;
        }

        private void Awake()
        {
            if (!triggerTriggerSetMap.ContainsKey(GetComponent<Collider>()))
                triggerTriggerSetMap.Add(GetComponent<Collider>(), new HashSet<Collider>());

            foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
                collidersObjects[collider] = gameObject;

            collidingGameObjects[gameObject] = new HashSet<GameObject>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            triggerTriggerSetMap[GetComponent<Collider>()].Add(collider);
            colliders.Add(collider);

            collidingGameObjects[gameObject].Add(collider.gameObject);
            if (collidersObjects.ContainsKey(collider))
            {
                collidingGameObjects[collidersObjects[collider]].Add(gameObject);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            triggerTriggerSetMap[GetComponent<Collider>()].Remove(collider);
            colliders.Remove(collider);

            collidingGameObjects[gameObject].Remove(collider.gameObject);
            if (collidersObjects.ContainsKey(collider))
            {
                collidingGameObjects[collidersObjects[collider]].Remove(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            triggerTriggerSetMap[GetComponent<Collider>()].Add(collision.collider);
            colliders.Add(collision.collider);

            collidingGameObjects[gameObject].Add(collision.collider.gameObject);
            if (collidersObjects.ContainsKey(collision.collider))
            {
                collidingGameObjects[collidersObjects[collision.collider]].Add(gameObject);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            triggerTriggerSetMap[GetComponent<Collider>()].Remove(collision.collider);
            colliders.Remove(collision.collider);

            collidingGameObjects[gameObject].Remove(collision.collider.gameObject);
            if (collidersObjects.ContainsKey(collision.collider))
            {
                collidingGameObjects[collidersObjects[collision.collider]].Remove(gameObject);
            }
        }

        #endregion
    }
}
