using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Members")]
    public class CompareMethodsReturnValuesSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT_1 = "Left Component";
        public const string METHOD_1 = "Left Method";
        public const string PARAMETERS_1 = "Left Parameters";
        public const string COMPONENT_2 = "Right Component";
        public const string METHOD_2 = "Right Method";
        public const string PARAMETERS_2 = "Right Parameters";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string COMPONENT1_ENTRY = "component1";

        [EventContextEntry()]
        public static readonly string COMPONENT2_ENTRY = "component2";

        [EventContextEntry]
        public static readonly string METHOD1_INFO_ENTRY = "method1";

        [EventContextEntry]
        public static readonly string METHOD2_INFO_ENTRY = "method2";

        [EventContextEntry()]
        public static readonly string RETURNED_VALUE1_ENTRY = "value1";

        [EventContextEntry()]
        public static readonly string RETURNED_VALUE2_ENTRY = "value2";

        #endregion

        #region Fields

        [ConfigurationParameter(COMPONENT_1, Necessity.Required)]
        protected Component component1;

        [ConfigurationParameter(METHOD_1, Necessity.Required)]
        [Provider(COMPONENT_1)]
        protected MethodInfo method1;

        [ConfigurationParameter(PARAMETERS_1, Necessity.Required)]
        [Provider(METHOD_1)]
        protected MethodParameters parameters1 = new();

        [ConfigurationParameter(COMPONENT_2, Necessity.Required)]
        protected Component component2;

        [ConfigurationParameter(METHOD_2, Necessity.Required)]
        [Provider(COMPONENT_2)]
        protected MethodInfo method2;

        [ConfigurationParameter(PARAMETERS_2, Necessity.Required)]
        [Provider(METHOD_2)]
        protected MethodParameters parameters2 = new();

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CompareMethodsReturnValuesSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method1 == null)
                throw new NullReferenceException($"{nameof(method1)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            if (method2 == null)
                throw new NullReferenceException($"{nameof(method2)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object returnedValue1 = method1.Invoke(component1, parameters1.ToArray());
            object returnedValue2 = method2.Invoke(component2, parameters2.ToArray());

            bool result = Equals(returnedValue1, returnedValue2);

            eventContext.Clear();
            eventContext.Add(COMPONENT1_ENTRY, component1);
            eventContext.Add(COMPONENT2_ENTRY, component2);
            eventContext.Add(METHOD1_INFO_ENTRY, method1);
            eventContext.Add(METHOD2_INFO_ENTRY, method2);
            eventContext.Add(RETURNED_VALUE1_ENTRY, returnedValue1);
            eventContext.Add(RETURNED_VALUE2_ENTRY, returnedValue2);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
