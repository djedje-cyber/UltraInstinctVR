using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Context")]
    public class SetContextValueFromStaticMethodReturnValueEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
        public const string TYPE = "Type";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";
        public const string CONTEXT_OUTPUT = "Output";

        #endregion

        #region Field

        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE)]
        protected MethodInfo method;

        [ConfigurationParameter(PARAMETERS, Necessity.Required)]
        [Provider(METHOD)]
        protected MethodParameters parameters = new();

        [ContextVariable(CONTEXT_OUTPUT, "The output/context entry to set")]
        protected ContextVariable<object> output;

        #endregion

        #region Constructors

        public SetContextValueFromStaticMethodReturnValueEffector(Event @event, Dictionary<string, Parameter> nameValueListMap, ContextHolder contexts)
            : base(@event, nameValueListMap, contexts) { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method == null)
                throw new NullReferenceException($"{nameof(method)} is null in {nameof(SetContextValueFromStaticMethodReturnValueEffector)}");

            if (!method.IsStatic)
                throw new ArgumentException($"{nameof(method)} is not a static MethodInfo in {nameof(SetContextValueFromStaticMethodReturnValueEffector)}");
        }

        public override void SafeEffectorUpdate()
        {
            object returnedValue = method.Invoke(null, parameters.ToArray());

            // Set the variable of the context with the returned value
            output.Set(returnedValue);
        }

        #endregion
    }
}
