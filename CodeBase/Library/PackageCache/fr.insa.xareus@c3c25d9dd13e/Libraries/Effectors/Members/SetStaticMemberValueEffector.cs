using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.SetMemberValueEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.SetMemberValueEffector", "Xareus.Unity.Librairies")]
    [FunctionDescription("", "Members")]
    public class SetStaticMemberValueEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string TYPE = "Type";
        public const string MEMBER = "Member";
        public const string VALUE = "Value";
#pragma warning restore S2339 // Public constant fields should not be used

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
        protected object desiredValue;

        #endregion

        #region Constructors

        protected SetStaticMemberValueEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (member == null)
                throw new NullReferenceException($"{nameof(member)} is null in transition {Event.Parent.label} ({Event.Parent.id})");
        }

        public override void SafeEffectorUpdate()
        {
            member.SetStaticValue(desiredValue);
        }

        #endregion
    }
}
