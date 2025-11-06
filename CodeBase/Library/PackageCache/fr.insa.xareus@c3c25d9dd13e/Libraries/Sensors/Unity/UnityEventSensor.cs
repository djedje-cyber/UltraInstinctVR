using System.Collections.Generic;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// A Sensor that can be activated by calling its Activate Method
    /// </summary>
    [Renamed("Xareus.Scenarios.Unity.UnityEventSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("This sensor can be triggered by calling its " + nameof(Activate) + "method")]
    public class UnityEventSensor : AUnitySensor
    {
        #region Fields

        protected bool activate;

        #endregion

        #region Constructors

        public UnityEventSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result SafeSensorCheck()
        {
            return new Result(activate, null);
        }

        /// <summary>
        /// Call this method to activate the sensor
        /// </summary>
        public void Activate(bool value = true)
        {
            activate = value;
            UpdateScenario();
        }

        #endregion
    }
}
