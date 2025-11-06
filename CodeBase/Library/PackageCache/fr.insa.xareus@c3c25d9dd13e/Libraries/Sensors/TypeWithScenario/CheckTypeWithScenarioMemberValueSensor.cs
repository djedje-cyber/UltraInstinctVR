using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Relations.Scenarios.Unity;
using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// A sensors that checks the value of a member in a TypeWithScenario
    /// </summary>
    [Renamed("SEVEN.CheckFiveTypeWithScenarioMemberValueSensor", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.CheckFiveTypeWithScenarioMemberValueSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Relations.Scenarios.CheckTypeWithScenarioMemberValueSensor", "Xareus.Unity.Librairies")]
    [Renamed("Xareus.Relations.Scenarios.CheckTypeWithScenarioMemberValueSensor", "Xareus.Unity.Libraries")]
    [FunctionDescription("", "TypeWithScenario")]
    public class CheckTypeWithScenarioMemberValueSensor : AInUnityStepSensor
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
        private const string TYPE = "type";

        private const string MEMBER = "memberInfo";

        private const string VALUE
            = "value";

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string XUTYPE_ENTRY = "type";

        [EventContextEntry]
        public static readonly string MEMBER_INFO_ENTRY = "member";

        [EventContextEntry()]
        public static readonly string VALUE_ENTRY = "value";

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE, Necessity.Required)]
        [ProvideConstraint(typeof(XUType))]
        protected Type type;

        [ConfigurationParameter(MEMBER, Necessity.Required)]
        [Provider(TYPE)]
        protected MemberInfo member;

        [ConfigurationParameter(VALUE, Necessity.Required)]
        protected object expectedValue;

        /// <summary>
        /// Instance of XUType to affect
        /// </summary>
        private XUType affectedType;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckTypeWithScenarioMemberValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (Contexts.ExternalContext is not TypeWithScenario)
                throw new InvalidOperationException(nameof(CheckTypeWithScenarioMemberValueSensor) + " must only be use in a scenario for " + nameof(TypeWithScenario));

            // Get affected Type
            affectedType = (Contexts.ExternalContext as TypeWithScenario).XuObject.GetXuType(type);

            // Check that the local object has the given Type
            if (affectedType == null)
                throw new ArgumentException(string.Format("The object with internal scenario doesn't have the given XUType '{0}'", type.Name));

            // Compute expected value
            if (member.MemberType is not MemberTypes.Field and not MemberTypes.Property)
            {
                throw new ArgumentException("MemberInfo is neither a FieldInfo nor a PropertyInfo");
            }

            eventContext = new();
        }

        public override Result UnityStepSensorCheck()
        {
            object value = member.GetInstanceValue(affectedType);

            bool result = Equals(value, expectedValue);

            eventContext.Clear();
            eventContext.Add(XUTYPE_ENTRY, affectedType);
            eventContext.Add(MEMBER_INFO_ENTRY, member);
            eventContext.Add(VALUE_ENTRY, value);

            return new Result(result, eventContext);
        }

        #endregion
    }
}
