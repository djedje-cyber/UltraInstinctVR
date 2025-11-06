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
    /// A effector that sets the member in a TypeWithScenario
    /// </summary>
    [Renamed("SEVEN.AffectFiveTypeWithScenarioMemberValueEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.SetFiveTypeWithScenarioMemberValueEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Relations.Scenarios.SetTypeWithScenarioMemberValueEffector", "Xareus.Unity.Librairies")]
    [Renamed("Xareus.Relations.Scenarios.SetTypeWithScenarioMemberValueEffector", "Xareus.Unity.Libraries")]
    [FunctionDescription("", "TypeWithScenario")]
    public class SetTypeWithScenarioMemberValueEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
        private const string TYPE = "type";

        private const string MEMBER = "member";
        private const string VALUE = "value";

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE, Necessity.Required)]
        [ProvideConstraint(typeof(XUType))]
        protected Type type;

        [ConfigurationParameter(MEMBER, Necessity.Required)]
        [Provider(TYPE)]
        protected MemberInfo member;

        [ConfigurationParameter(VALUE, Necessity.Required)]
        [Provider(MEMBER)]
        protected object desiredValue;

        /// <summary>
        /// Instance of XUType to affect
        /// </summary>
        private XUType affectedType;

        #endregion

        #region Constructors

        protected SetTypeWithScenarioMemberValueEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (Contexts.ExternalContext is not TypeWithScenario)
                throw new InvalidOperationException(nameof(SetTypeWithScenarioMemberValueEffector) + " must only be use in a scenario for " + nameof(TypeWithScenario));

            // Get affected Xareus Type
            affectedType = (Contexts.ExternalContext as TypeWithScenario).XuObject.GetXuType(type);

            // Check that the local object has the given Xareus Type
            if (affectedType == null)
                throw new ArgumentException(string.Format("The object with internal scenario doesn't have the given " + nameof(XUType) + " '{0}'", type.Name));

            // Compute desired value
            if (member.MemberType is not MemberTypes.Field and not MemberTypes.Property)
            {
                throw new ArgumentException("MemberInfo is neither a FieldInfo nor a PropertyInfo");
            }
        }

        public override void SafeEffectorUpdate()
        {
            member.SetInstanceValue(affectedType, desiredValue);
        }

        #endregion
    }
}
