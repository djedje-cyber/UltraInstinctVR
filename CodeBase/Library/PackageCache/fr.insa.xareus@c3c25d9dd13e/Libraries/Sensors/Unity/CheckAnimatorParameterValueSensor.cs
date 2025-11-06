using log4net;

using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("Sensor to check the value of an animator parameter", "Unity/Animation")]
    public class CheckAnimatorParameterValueSensor : AInUnityStepSensor
    {
        #region Static

        /// <summary>
        /// The log4net logger
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(CheckAnimatorParameterValueSensor));

        #endregion

        #region Fields

        [ConfigurationParameter("Animator", Necessity.Required)]
        protected Animator animator;

        [ConfigurationParameter("Parameter Name", Necessity.Required)]
        protected string animatorParameterName;

        [ConfigurationParameter("Expected Value", "The accepted types are int, float, and bool", Necessity.Required)]
        protected object expectedValue;

        #endregion

        #region Constructors

        public CheckAnimatorParameterValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods
        public override void SafeReset()
        {
            if (expectedValue is not bool or float or int)
            {
                LOGGER.Error($"{expectedValue.GetType()} is not supported for {nameof(expectedValue)}. Only bool, float and int are supported by {nameof(CheckAnimatorParameterValueSensor)}");
            }
        }

        public override Result UnityStepSensorCheck()
        {
            bool result = false;

            if (expectedValue is bool desiredBoolValue)
            {
                result = desiredBoolValue == animator.GetBool(animatorParameterName);
            }
            else if (expectedValue is float desiredFloatValue)
            {
                result = desiredFloatValue == animator.GetFloat(animatorParameterName);
            }
            else if (expectedValue is int desiredIntValue)
            {
                result = desiredIntValue == animator.GetInteger(animatorParameterName);
            }

            return new Result(result, null);
        }

        #endregion
    }
}
