using System.Collections.Generic;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [OverrideClass("Xareus.Scenarios.SetValueInContextEffector", "Xareus.Scenarios.Extra")]
    [Renamed("Xareus.Scenarios.Unity.SetValueInContextEffector", "Xareus.Unity.Librairies")]
    [Renamed("Xareus.Unity.SetValueInContextEffector", "Xareus.Unity.Libraries")]
    [FunctionDescription("Effector to put a value into a context", "Context")]
    public class SetContextValueEffector : AUnityEffector
    {
        #region Fields

        /// <summary>
        /// The value
        /// </summary>
        [ConfigurationParameter(SetValueInContextEffector.VALUE_PARAMETER, "The value to set in the output", Necessity.Required)]
        protected object value;

        /// <summary>
        /// The output
        /// </summary>
        [ContextVariable(SetValueInContextEffector.OUTPUT_PARAMETER, "The output/context entry to set")]
        protected ContextVariable<object> output;

        #endregion

        #region Constructors

        public SetContextValueEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            output.Set(value);
        }

        #endregion
    }
}
