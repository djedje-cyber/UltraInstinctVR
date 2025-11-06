using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.CheckMemberValueSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.CheckMemberValueSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("", "Members")]
    public class CheckMemberValueSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT = "Component";
        public const string MEMBER = "Member";
        public const string VALUE = "value";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry]
        public static readonly string COMPONENT_ENTRY = "component";

        [EventContextEntry]
        public static readonly string MEMBER_INFO_ENTRY = "member";

        [EventContextEntry]
        public static readonly string VALUE_ENTRY = "value";

        #endregion

        #region Fields

        [ConfigurationParameter(COMPONENT, Necessity.Required)]
        protected Component component;

        [ConfigurationParameter(MEMBER, Necessity.Required)]
        [Provider(COMPONENT)]
        protected MemberInfo member;

        [ConfigurationParameter(VALUE, Necessity.Required)]
        [Provider(MEMBER)]
        protected object expectedValue;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckMemberValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

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
            object value = member.GetInstanceValue(component);

            bool result = Equals(value, expectedValue);

            eventContext.Clear();
            eventContext.Add(COMPONENT_ENTRY, component);
            eventContext.Add(MEMBER_INFO_ENTRY, member);
            eventContext.Add(VALUE_ENTRY, value);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
