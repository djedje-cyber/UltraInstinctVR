using log4net;

using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Checks if the distance between the given transforms is less than the provided distance (in meter).
    /// </summary>
    [Renamed("SEVEN.Unity.DistanceSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.DistanceSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("When height is ignored, the distance is calculated using X and Z axes", "Unity")]
    public class DistanceSensor : AInUnityStepSensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string DISTANCE = "distance";

        #endregion



        #region Fields

        [ConfigurationParameter("transform1", Necessity.Required)]
        protected Transform transform1;

        [ConfigurationParameter("transform2", Necessity.Required)]
        protected Transform transform2;

        [ConfigurationParameter("distance", Necessity.Required, Description = "Distance must be >= 0")]
        protected float distance;

        [ConfigurationParameter("ignore height", Necessity.Required, Description = "Enable to ignore the height (y) while checking the distance")]
        protected bool ignoreHeight;

        /// <summary>
        /// The log4net logger
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(DistanceSensor));

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected DistanceSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (distance < 0)
                LOGGER.Error("The provided distance must be positive");
        }

        public override Result UnityStepSensorCheck()
        {
            float calculatedDistance = ignoreHeight
                ? Vector3.Distance(new Vector3(transform1.position.x, 0, transform1.position.z),
                    new Vector3(transform2.position.x, 0, transform2.position.z))
                : Vector3.Distance(transform1.position, transform2.position);

            bool distanceValidated = calculatedDistance <= distance;

            if (distanceValidated)
            {
                eventContext = new SimpleDictionary
                {
                    { DISTANCE, calculatedDistance }
                };
            }
            return new Result(distanceValidated, eventContext);
        }

        #endregion
    }
}
