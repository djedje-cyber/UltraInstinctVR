using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("", "Context")]
    public class SetContextValueFromStaticMemberValueEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
        public const string TYPE = "Type";
        public const string MEMBER = "Member";
        public const string CONTEXT_OUTPUT = "Output";

        #endregion

        #region Field

        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        [ConfigurationParameter(MEMBER, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE)]
        protected MemberInfo member;

        [ContextVariable(CONTEXT_OUTPUT, "The output/context entry to set")]
        protected ContextVariable<object> output;

        #endregion

        #region Constructors

        public SetContextValueFromStaticMemberValueEffector(Event @event, Dictionary<string, Parameter> nameValueListMap, ContextHolder contexts)
            : base(@event, nameValueListMap, contexts) { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (member == null)
                throw new NullReferenceException($"{nameof(member)} is null in {nameof(SetContextValueFromStaticMemberValueEffector)}");
        }

        public override void SafeEffectorUpdate()
        {
            object memberValue = member.GetStaticValue();

            // Set the variable of the context with the new variable
            output.Set(memberValue);
        }

        #endregion
    }
}
