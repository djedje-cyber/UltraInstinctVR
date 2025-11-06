using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [Renamed("Xareus.Scenarios.Unity.SetComponentValueInContextEffector", "Xareus.Unity.Libraries")]
    [FunctionDescription("", "Context")]
    public class SetContextValueFromMemberValueEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT = "Component";
        public const string MEMBER = "Member";
        public const string CONTEXT_OUTPUT = "Output";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Field

        [ConfigurationParameter(COMPONENT, Necessity.Required)]
        protected Component component;

        [ConfigurationParameter(MEMBER, Necessity.Required)]
        [Provider(COMPONENT)]
        protected MemberInfo member;

        [ContextVariable(CONTEXT_OUTPUT, "The output/context entry to set")]
        protected ContextVariable<object> output;

        #endregion

        #region Constructors

        public SetContextValueFromMemberValueEffector(Event @event, Dictionary<string, Parameter> nameValueListMap, ContextHolder contexts)
            : base(@event, nameValueListMap, contexts) { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (member == null)
                throw new NullReferenceException($"{nameof(member)} is null in {nameof(SetContextValueFromMemberValueEffector)}");
        }

        public override void SafeEffectorUpdate()
        {
            object memberValue = member.GetInstanceValue(component);

            // Set the variable of the context with the new variable
            output.Set(memberValue);
        }

        #endregion
    }
}
