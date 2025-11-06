using System.Collections.Generic;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Context")]
    public class CheckContextValueSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string VARIABLE = "Variable";
        public const string VALUE = "value";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry]
        public static readonly string VARIABLE_ENTRY = "variable";

        [EventContextEntry]
        public static readonly string VALUE_ENTRY = "value";

        #endregion

        #region Fields

        [ContextVariable(VARIABLE)]
        protected ContextVariable<object> variable;

        [ConfigurationParameter(VALUE, Necessity.Required)]
        protected object expectedValue;


        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckContextValueSensor(Xareus.Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object variableValue = variable.Get();

            bool result = Equals(variableValue, expectedValue);

            eventContext.Clear();
            eventContext.Add(VARIABLE_ENTRY, variableValue);
            eventContext.Add(VALUE_ENTRY, expectedValue);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
