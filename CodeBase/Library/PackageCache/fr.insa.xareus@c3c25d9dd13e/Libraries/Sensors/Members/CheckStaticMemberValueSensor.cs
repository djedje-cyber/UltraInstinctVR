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
    [Renamed("SEVEN.StaticClassMemberValueSensor", "Assembly-CSharp")]
    [Renamed("Xareus.StaticClassMemberValueSensor", "Xareus.Unity.Librairies")]
    [Renamed("Xareus.StaticClassMemberValueSensor", "Xareus.Unity.Libraries")]
    [FunctionDescription("", "Members")]
    public class CheckStaticMemberValueSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string TYPE = "Type";
        public const string MEMBER = "Member";
        public const string VALUE = "Value";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry]
        public static readonly string TYPE_ENTRY = "type";

        [EventContextEntry]
        public static readonly string MEMBER_INFO_ENTRY = "member";

        [EventContextEntry]
        public static readonly string VALUE_ENTRY = "value";

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        [ConfigurationParameter(MEMBER, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE)]
        protected MemberInfo member;

        [ConfigurationParameter(VALUE, Necessity.Required)]
        [Provider(MEMBER)]
        protected object expectedValue;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckStaticMemberValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (member == null)
                throw new NullReferenceException($"{nameof(member)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object value = member.GetStaticValue();

            bool result = Equals(value, expectedValue);

            eventContext.Clear();
            eventContext.Add(TYPE_ENTRY, type);
            eventContext.Add(MEMBER_INFO_ENTRY, member);
            eventContext.Add(VALUE_ENTRY, value);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
