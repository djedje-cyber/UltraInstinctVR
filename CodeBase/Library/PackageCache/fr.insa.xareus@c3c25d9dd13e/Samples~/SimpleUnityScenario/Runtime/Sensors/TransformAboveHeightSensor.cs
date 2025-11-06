using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

namespace Xareus.Samples.SimpleUnityScenario
{
    public class TransformAboveHeightSensor : AInUnityStepSensor
    {
        #region Fields

        [ConfigurationParameter("Transform to check", Necessity.Required)]
        public Transform transformToCheck;

        [ConfigurationParameter("Height", initialValue: 0f)]
        public float height;

        #endregion

        #region Constructors

        public TransformAboveHeightSensor(Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result UnityStepSensorCheck()
        {
            if (transformToCheck.position.y >= height)
            {
                return new Result(true, null);
            }
            else
            {
                return new Result(false, null);
            }
        }

        #endregion
    }
}
