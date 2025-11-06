using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.ObjectCollisionSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.ObjectCollisionSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("When the second object is not defined, the event context will list all corresponding objects in the 'objects' path " +
        "while only one of the colliding object will be in the 'object2' path")]
    public class ObjectCollisionSensor : AInUnityStepSensor
    {
        #region Fields

        public const string IN_COLLISION = "In Collision";

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string OBJECT1 = "object1";

        [EventContextEntry()]
        public static readonly string OBJECT2 = "object2";

        [EventContextEntry()]
        public static readonly string OBJECTS = "objects";

        #endregion



        #region Fields

        /// <summary>
        /// Must the objects be in collision?
        /// </summary>
        [AdvancedConfigurationParameter(IN_COLLISION, true, "This parameter will be removed in a later version. Use the Negate Parameter instead", Necessity.Optional)]
        protected readonly bool inCollision;

        /// <summary>
        /// The first object
        /// </summary>
        [ConfigurationParameter("Object 1", Necessity.Required)]
        protected GameObject object1;

        /// <summary>
        /// The second object
        /// </summary>
        [ConfigurationParameter("Object 2", "The second object to check the collision or abscence of collision with (Any object will trigger the sensor if not set)", Necessity.Optional)]
        protected GameObject object2;

        private readonly SimpleDictionary eventContext = new();

        #endregion

        #region Constructors

        protected ObjectCollisionSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            UFEOtherTrigger obj1Trigger = object1.GetComponent<UFEOtherTrigger>();
            if (obj1Trigger == null)
                _ = object1.AddComponent<UFEOtherTrigger>();

            eventContext.Clear();
            eventContext.Add(OBJECT1, object1);

            if (object2 != null)
            {
                UFEOtherTrigger obj2Trigger = object2.GetComponent<UFEOtherTrigger>();
                if (obj2Trigger == null)
                    _ = object2.AddComponent<UFEOtherTrigger>();

                eventContext.Add(OBJECT2, object2);
            }
            else
            {
                eventContext.Add(OBJECTS, new List<GameObject>());
            }
        }

        /// <summary>
        /// Checks the collision state
        /// </summary>
        public override Result UnityStepSensorCheck()
        {
            if (object2 == null && UFEOtherTrigger.IsColliding(object1))
            {
                IEnumerable<GameObject> collidingObjects = UFEOtherTrigger.GetColliding(object1);
                eventContext.SetValue(OBJECTS, collidingObjects); // note that this will be a collection !
                eventContext.SetValue(OBJECT2, collidingObjects.FirstOrDefault());
            }
            bool collisionCheckValidated = inCollision == (object2 != null ? UFEOtherTrigger.AreColliding(object1, object2) : UFEOtherTrigger.IsColliding(object1));

            return new Result(collisionCheckValidated, eventContext);
        }

        #endregion
    }
}
