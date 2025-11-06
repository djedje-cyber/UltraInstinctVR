using log4net;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;
using Xareus.Unity.TypeDescriptorContext;

namespace Xareus.Unity
{
    [Renamed("CollisionSensor", "Assembly-CSharp")]
    [ObsoleteFunction("Use " + nameof(ObjectCollisionSensor) + " instead", typeof(ObjectCollisionSensor), typeof(CollisionSensor), nameof(ConvertParameters))]
    public class CollisionSensor : AUnitySensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string OBJECT1 = "object1";

        [EventContextEntry()]
        public static readonly string OBJECT2 = "object2";

        /// <summary>
        /// The log4net logger
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(CollisionSensor));

#pragma warning disable 0649

        private readonly SimpleDictionary eventContext = new();

        /// <summary>
        /// Must the objects be in collision on appart ?
        /// </summary>
        [ConfigurationParameter("inCollision", "Should the sensor check for a collision or absence if it ?")]
        private bool inCollision = true;

        /// <summary>
        /// The first object
        /// </summary>
        [ConfigurationParameter("object1", Necessity.Required)]
        private string obj1Name;

        /// <summary>
        /// The second object
        /// </summary>
        [ConfigurationParameter("object2", Necessity.Required)]
        private string obj2Name;

#pragma warning restore 0649

        private UFEOtherTrigger obj1Trigger;
        private UFEOtherTrigger obj2Trigger;

        #endregion

        #region Constructors

        protected CollisionSensor(Xareus.Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Conversion from obsolete Sensor
        /// </summary>
        /// <param name="inParameters"></param>
        /// <returns></returns>
        public static List<Parameter> ConvertParameters(List<Parameter> inParameters)
        {
            List<Parameter> res = new();
            if (inParameters == null)
                return res;
            if (inParameters.Any(param => param.name == "object1"))
            {
                Parameter previousParameter = inParameters.Find(param => param.name == "object1");
                GameObject gameObject1 = GameObject.Find(previousParameter.value);
                Parameter newParameter = ValueParser.ConvertToParameter(gameObject1, new UnityObjectDescriptorContext(false));
                newParameter.name = "Object 1";
                res.Add(newParameter);
            }
            if (inParameters.Any(param => param.name == "object2"))
            {
                Parameter previousParameter = inParameters.Find(param => param.name == "object2");
                GameObject gameObject2 = GameObject.Find(previousParameter.value);
                Parameter newParameter = ValueParser.ConvertToParameter(gameObject2, new UnityObjectDescriptorContext(false));
                newParameter.name = "Object 2";
                res.Add(newParameter);
            }
            if (inParameters.Any(param => param.name == "inCollision"))
            {
                Parameter previousParameter = inParameters.Find(param => param.name == "inCollision");
                previousParameter.name = "In Collision";
                res.Add(previousParameter);
            }
            return res;
        }

        public override void SafeReset()
        {
            GameObject gameObject1 = null;
            GameObject gameObject2 = null;

            gameObject1 = GameObject.Find(obj1Name);
            if (gameObject1 == null)
            {
                LOGGER.ErrorFormat("Could not find gameobject at path {0}", obj1Name);
                return;
            }

            obj1Trigger = gameObject1.GetComponent<UFEOtherTrigger>();
            if (obj1Trigger == null)
            {
                obj1Trigger = gameObject1.AddComponent<UFEOtherTrigger>();
            }

            gameObject2 = GameObject.Find(obj2Name);
            if (gameObject2 == null)
            {
                LOGGER.ErrorFormat("Could not find gameobject at path {0}", obj2Name);
                return;
            }

            obj2Trigger = gameObject2.GetComponent<UFEOtherTrigger>();
            if (obj2Trigger == null)
            {
                obj2Trigger = gameObject2.AddComponent<UFEOtherTrigger>();
            }

            eventContext.Clear();
            eventContext.Add(OBJECT1, gameObject1);
            eventContext.Add(OBJECT2, gameObject2);
        }

        public override Result SafeSensorCheck()
        {
            if (obj1Trigger == null || obj2Trigger == null)
            {
                LOGGER.WarnFormat("Could not find one of the required object. Check errors above");

                return new Result(false, eventContext);
            }
            bool colliding = false;
            colliding = UFEOtherTrigger.AreColliding(obj1Trigger.GetComponent<Collider>(), obj2Trigger.GetComponent<Collider>());
            bool res = colliding == inCollision;
            if (res && LOGGER.IsDebugEnabled)
            {
                LOGGER.DebugFormat("Detecting collision in {0}", Event.id);
            }
            return new Result(res, eventContext);
        }

        #endregion
    }
}
