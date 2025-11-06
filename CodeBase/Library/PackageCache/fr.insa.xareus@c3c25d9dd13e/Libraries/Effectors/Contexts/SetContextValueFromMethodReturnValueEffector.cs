using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Context")]
    public class SetContextValueFromMethodReturnValueEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT = "Component";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";
        public const string CONTEXT_OUTPUT = "Output";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Field

        [ConfigurationParameter(COMPONENT, Necessity.Required)]
        protected Component component;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [Provider(COMPONENT)]
        protected MethodInfo method;

        [ConfigurationParameter(PARAMETERS, Necessity.Required)]
        [Provider(METHOD)]
        protected MethodParameters parameters = new();

        [ContextVariable(CONTEXT_OUTPUT, "The output/context entry to set")]
        protected ContextVariable<object> output;

        #endregion

        #region Constructors

        public SetContextValueFromMethodReturnValueEffector(Event @event, Dictionary<string, Parameter> nameValueListMap, ContextHolder contexts)
            : base(@event, nameValueListMap, contexts) { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method == null)
                throw new NullReferenceException($"{nameof(method)} is null in {nameof(SetContextValueFromMethodReturnValueEffector)}");
        }

        public override void SafeEffectorUpdate()
        {
            object returnedValue = method.Invoke(component, parameters.ToArray());

            // Set the variable of the context with the returned value
            output.Set(returnedValue);
        }

        #endregion
    }
}
