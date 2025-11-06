using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Set the enabled property of the given behaviour to the enabledValue.
    /// </summary>
    [Renamed("SEVEN.Unity.SetEnabledEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.SetEnabledEffector", "Xareus.Unity.Librairies")]
    [FunctionDescription("Enable or disable the behaviour component of an object\n" +
        "Same as `behaviour.enabled = enabledValue;`", "Unity")]
    public class SetEnabledEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Behaviour", Necessity.Required)]
        protected Behaviour behaviour;

        [ConfigurationParameter("Enabled Value", Necessity.Required)]
        protected bool enabledValue;

        #endregion

        #region Constructors

        protected SetEnabledEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            behaviour.enabled = enabledValue;
        }

        #endregion
    }
}
