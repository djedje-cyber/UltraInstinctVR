using log4net;

using System.Collections.Generic;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

namespace Xareus.Unity
{
    [Renamed("AutoTriggerSensor", "Assembly-CSharp")]
    public class AutoTriggerSensor : AUnitySensor
    {
        #region Fields

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(AutoTriggerSensor));

        #endregion

        #region Constructors

        protected AutoTriggerSensor(Xareus.Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
            LOGGER.Debug("Constructeur");
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            LOGGER.Debug("SafeReset");
        }

        public override Result SafeSensorCheck()
        {
            LOGGER.Debug("SafeSensorCheck");
            return new Result(true, null);
        }

        #endregion
    }
}
