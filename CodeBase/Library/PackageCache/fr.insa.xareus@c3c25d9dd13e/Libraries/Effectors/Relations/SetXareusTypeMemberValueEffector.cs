using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Relations
{
    /// <summary>
    /// An effector that sets the member of a type
    /// </summary>
    [Renamed("SEVEN.FiveTypeMemberValueEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.SetFiveTypeMemberValueEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Relations.SetXareusTypeMemberValueEffector", "Xareus.Unity.Librairies")]
    public class SetXareusTypeMemberValueEffector : AUnityEffector
    {
        #region Const

        private const string MEMBER_TYPE = "memberType";
        private const string MEMBER = "member";
        private const string OBJECT = "_object";
        private const string DESIRED_VALUE = "desiredValue";

        #endregion

        #region Fields

        // Member to check
        [ProvideConstraint(typeof(XUType))]
        [ConfigurationParameter(MEMBER_TYPE, Necessity.Required)]
        protected Type memberType;

        // First object to compare
        [Provider(MEMBER_TYPE)]
        [ConfigurationParameter(OBJECT, Necessity.Required)]
        protected XUType _object;

        // Member to check
        [Provider(MEMBER_TYPE)]
        [ConfigurationParameter(MEMBER, Necessity.Required)]
        protected MemberInfo member;

        // Desired value
        [Provider(MEMBER)]
        [ConfigurationParameter(DESIRED_VALUE, Necessity.Required)]
        protected object desiredValue;

        #endregion

        #region Constructors

        protected SetXareusTypeMemberValueEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            // Verification of arguments values
            if (member == null)
                throw new ArgumentException("Member is null in " + nameof(SetXareusTypeMemberValueEffector) + " : failed to load from parameter");

            object convertedValue = ValueParser.ConvertTo(member.ReflectedType, desiredValue);

            member.SetInstanceValue(_object, desiredValue);
        }

        #endregion
    }
}
