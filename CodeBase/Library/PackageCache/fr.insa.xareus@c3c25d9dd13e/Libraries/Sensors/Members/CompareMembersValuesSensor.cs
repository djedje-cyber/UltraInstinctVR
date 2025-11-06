using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.CompareMembersValuesSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.CompareMembersValuesSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("", "Members")]
    public class CompareMembersValuesSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT_1 = "Left Component";
        public const string MEMBER_1 = "Left Member";
        public const string COMPONENT_2 = "Right Component";
        public const string MEMBER_2 = "Right Member";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string COMPONENT1_ENTRY = "component1";

        [EventContextEntry()]
        public static readonly string COMPONENT2_ENTRY = "component2";

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

        [ConfigurationParameter(COMPONENT_1, Necessity.Required)]
        protected Component component1;

        [ConfigurationParameter(MEMBER_1, Necessity.Required)]
        [Provider(COMPONENT_1)]
        protected MemberInfo member1;

        [ConfigurationParameter(COMPONENT_2, Necessity.Required)]
        protected Component component2;

        [ConfigurationParameter(MEMBER_2, Necessity.Required)]
        [Provider(COMPONENT_2)]
        protected MemberInfo member2;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CompareMembersValuesSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
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
            object value1 = member1.GetInstanceValue(component1);
            object value2 = member2.GetInstanceValue(component2);

            bool result = Equals(value1, value2);

            eventContext.Clear();
            eventContext.Add(COMPONENT1_ENTRY, component1);
            eventContext.Add(COMPONENT2_ENTRY, component2);
            eventContext.Add(MEMBER1_INFO_ENTRY, member1);
            eventContext.Add(MEMBER2_INFO_ENTRY, member2);
            eventContext.Add(VALUE1_ENTRY, value1);
            eventContext.Add(VALUE2_ENTRY, value2);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
