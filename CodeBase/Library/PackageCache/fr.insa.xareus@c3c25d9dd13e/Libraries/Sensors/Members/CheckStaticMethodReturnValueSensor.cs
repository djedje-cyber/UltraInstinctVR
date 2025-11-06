using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Unity
{
    [FunctionDescription("", "Members")]
    public class CheckStaticMethodReturnValueSensor : AInUnityStepSensor
    {
        #region Fields

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string TYPE = "Type";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";
        public const string VALUE = "Value";
#pragma warning restore S2339 // Public constant fields should not be use

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string TYPE_ENTRY = "type";

        [EventContextEntry()]
        public static readonly string METHOD_INFO_ENTRY = "method";

        [EventContextEntry()]
        public static readonly string RETURNED_VALUE_ENTRY = "returnedValue";

        #endregion



        #region Fields

        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE)]
        protected MethodInfo method;

        [ConfigurationParameter(PARAMETERS, Necessity.Required)]
        [Provider(METHOD)]
        protected MethodParameters parameters = new();

        [ConfigurationParameter(VALUE, Necessity.Required)]
        [Provider(METHOD)]
        protected object expectedValue;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckStaticMethodReturnValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method == null)
                throw new NullReferenceException($"{nameof(method)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            if (!method.IsStatic)
                throw new InvalidOperationException($"{nameof(method)} is not a static MethodInfo in transition {Event.Parent.label} ({Event.Parent.id})");

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object returnedValue = method.Invoke(null, parameters.ToArray());

            bool result = Equals(returnedValue, expectedValue);

            eventContext.Clear();
            eventContext.Add(TYPE_ENTRY, type);
            eventContext.Add(METHOD_INFO_ENTRY, method);
            eventContext.Add(RETURNED_VALUE_ENTRY, returnedValue);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
