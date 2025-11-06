using System.Collections.Generic;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Context")]
    public class CompareContextsValuesSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
        public const string VARIABLE_1 = "Left Variable";
        public const string VARIABLE_2 = "Right Variable";

        #endregion

        #region Statics

        [EventContextEntry]
        public static readonly string VARIABLE1_ENTRY = "variable1";

        [EventContextEntry]
        public static readonly string VARIABLE2_ENTRY = "variable2";

        #endregion

        #region Fields

        [ContextVariable(VARIABLE_1)]
        protected ContextVariable<object> variable1;

        [ContextVariable(VARIABLE_2)]
        protected ContextVariable<object> variable2;


        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CompareContextsValuesSensor(Xareus.Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
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
            object variableValue1 = variable1.Get();
            object variableValue2 = variable2.Get();

            bool result = Equals(variableValue1, variableValue2);

            eventContext.Clear();
            eventContext.Add(VARIABLE1_ENTRY, variableValue1);
            eventContext.Add(VARIABLE2_ENTRY, variableValue2);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
