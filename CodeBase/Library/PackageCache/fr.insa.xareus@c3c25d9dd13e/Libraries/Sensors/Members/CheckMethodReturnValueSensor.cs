using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Unity
{
    [Renamed("Xareus.Scenarios.Unity.CheckMethodReturnValueSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("", "Members")]
    public class CheckMethodReturnValueSensor : AInUnityStepSensor
    {
        #region Fields

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT = "Component";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";
        public const string VALUE = "Value";
#pragma warning restore S2339 // Public constant fields should not be use

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string COMPONENT_ENTRY = "component";

        [EventContextEntry()]
        public static readonly string METHOD_INFO_ENTRY = "method";

        [EventContextEntry()]
        public static readonly string RETURNED_VALUE_ENTRY = "returnedValue";

        #endregion



        #region Fields

        [ConfigurationParameter(COMPONENT, Necessity.Required)]
        protected Component component;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [Provider(COMPONENT)]
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

        protected CheckMethodReturnValueSensor(Xareus.Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts) { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method == null)
                throw new NullReferenceException($"{nameof(method)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object returnedValue = method.Invoke(component, parameters.ToArray());

            bool result = Equals(returnedValue, expectedValue);

            eventContext.Clear();
            eventContext.Add(COMPONENT_ENTRY, component);
            eventContext.Add(METHOD_INFO_ENTRY, method);
            eventContext.Add(RETURNED_VALUE_ENTRY, returnedValue);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
