using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Members")]
    public class CompareStaticMembersValuesSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string TYPE_1 = "Left Type";
        public const string MEMBER_1 = "Left Member";
        public const string TYPE_2 = "Right Type";
        public const string MEMBER_2 = "Right Member";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string TYPE1_ENTRY = "type1";

        [EventContextEntry()]
        public static readonly string TYPE2_ENTRY = "type2";

        [EventContextEntry]
        public static readonly string MEMBER1_INFO_ENTRY = "member1";

        [EventContextEntry]
        public static readonly string MEMBER2_INFO_ENTRY = "member2";

        [EventContextEntry()]
        public static readonly string VALUE1_ENTRY = "value1";

        [EventContextEntry()]
        public static readonly string VALUE2_ENTRY = "value2";

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE_1, Necessity.Required)]
        protected Type type1;

        [ConfigurationParameter(MEMBER_1, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE_1)]
        protected MemberInfo member1;

        [ConfigurationParameter(TYPE_2, Necessity.Required)]
        protected Type type2;

        [ConfigurationParameter(MEMBER_2, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE_2)]
        protected MemberInfo member2;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CompareStaticMembersValuesSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (member1 == null)
                throw new NullReferenceException($"{nameof(member1)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            if (member2 == null)
                throw new NullReferenceException($"{nameof(member2)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object value1 = member1.GetStaticValue();
            object value2 = member2.GetStaticValue();

            bool result = Equals(value1, value2);

            eventContext.Clear();
            eventContext.Add(TYPE1_ENTRY, type1);
            eventContext.Add(TYPE2_ENTRY, type2);
            eventContext.Add(MEMBER1_INFO_ENTRY, member1);
            eventContext.Add(MEMBER2_INFO_ENTRY, member2);
            eventContext.Add(VALUE1_ENTRY, value1);
            eventContext.Add(VALUE2_ENTRY, value2);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
