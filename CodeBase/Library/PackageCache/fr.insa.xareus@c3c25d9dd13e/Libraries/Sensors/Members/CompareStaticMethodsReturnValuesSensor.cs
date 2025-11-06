using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Members")]
    public class CompareStaticMethodsReturnValuesSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string TYPE_1 = "Left Type";
        public const string METHOD_1 = "Left Method";
        public const string PARAMETERS_1 = "Left Parameters";
        public const string TYPE_2 = "Right Type";
        public const string METHOD_2 = "Right Method";
        public const string PARAMETERS_2 = "Right Parameters";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string TYPE1_ENTRY = "type1";

        [EventContextEntry()]
        public static readonly string TYPE2_ENTRY = "type2";

        [EventContextEntry]
        public static readonly string METHOD1_INFO_ENTRY = "method1";

        [EventContextEntry]
        public static readonly string METHOD2_INFO_ENTRY = "method2";

        [EventContextEntry()]
        public static readonly string VALUE1_ENTRY = "value1";

        [EventContextEntry()]
        public static readonly string VALUE2_ENTRY = "value2";

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE_1, Necessity.Required)]
        protected Type type1;

        [ConfigurationParameter(METHOD_1, Necessity.Required)]
        [Provider(TYPE_1)]
        protected MethodInfo method1;

        [ConfigurationParameter(PARAMETERS_1, Necessity.Required)]
        [Provider(METHOD_1)]
        protected MethodParameters parameters1 = new();

        [ConfigurationParameter(TYPE_2, Necessity.Required)]
        protected Type type2;

        [ConfigurationParameter(METHOD_2, Necessity.Required)]
        [Provider(TYPE_2)]
        protected MethodInfo method2;

        [ConfigurationParameter(PARAMETERS_2, Necessity.Required)]
        [Provider(METHOD_2)]
        protected MethodParameters parameters2 = new();

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CompareStaticMethodsReturnValuesSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method1 == null)
                throw new NullReferenceException($"{nameof(method1)} is null intransition {Event.Parent.label} ({Event.Parent.id})");

            if (method2 == null)
                throw new NullReferenceException($"{nameof(method2)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            if (!method1.IsStatic)
                throw new InvalidOperationException($"{nameof(method1)} is not a static MethodInfo in transition {Event.Parent.label} ({Event.Parent.id})");

            if (!method2.IsStatic)
                throw new InvalidOperationException($"{nameof(method2)} is not a static MethodInfo in transition {Event.Parent.label} ({Event.Parent.id})");

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object returnedValue1 = method1.Invoke(null, parameters1.ToArray());
            object returnedValue2 = method2.Invoke(null, parameters2.ToArray());

            bool result = Equals(returnedValue1, returnedValue2);

            eventContext.Clear();
            eventContext.Add(TYPE1_ENTRY, type1);
            eventContext.Add(TYPE2_ENTRY, type2);
            eventContext.Add(METHOD1_INFO_ENTRY, method1);
            eventContext.Add(METHOD2_INFO_ENTRY, method2);
            eventContext.Add(VALUE1_ENTRY, returnedValue1);
            eventContext.Add(VALUE2_ENTRY, returnedValue2);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
