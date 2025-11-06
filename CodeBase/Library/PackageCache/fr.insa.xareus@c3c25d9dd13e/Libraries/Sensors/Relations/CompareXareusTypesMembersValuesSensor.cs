using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Relations.ParametersConverter;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Relations
{
    /// <summary>
    /// A sensor that checks the member of a given object
    /// </summary>
    [Renamed("SEVEN.FiveTypeMemberCompareSensor", "Assembly-CSharp", typeof(CompareXareusTypesMembersValuesSensorConverter))]
    [Renamed("SEVEN.FIVE.FiveTypeMemberCompareSensor", "Assembly-CSharp", typeof(CompareXareusTypesMembersValuesSensorConverter))]
    [Renamed("Xareus.Scenarios.Relations.CompareXareusTypesMembersValuesSensor", "Xareus.Unity.Librairies")]
    public class CompareXareusTypesMembersValuesSensor : AInUnityStepSensor
    {
        #region Const

        private const string TYPE_1 = "Type 1";
        private const string XAREUS_TYPE_1 = "Xareus Type 1";
        private const string MEMBER_1 = "Member 1";
        private const string TYPE_2 = "Type 2";
        private const string XAREUS_TYPE_2 = "Xareus Type 2";
        private const string MEMBER_2 = "Member 2";

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string XUTYPE1 = "Type 1";

        [EventContextEntry()]
        public static readonly string XUTYPE2 = "Type 2";

        [EventContextEntry()]
        public static readonly string VALUE = "value";

        #endregion

        #region Fields

        // Type of first member to compare
        [ProvideConstraint(typeof(XUType))]
        [ConfigurationParameter(TYPE_1, Necessity.Required)]
        protected Type type1;

        // First object to compare
        [Provider(TYPE_1)]
        [ConfigurationParameter(XAREUS_TYPE_1, Necessity.Required)]
        protected XUType xuType1;

        // First member to compare
        [Provider(TYPE_1)]
        [ConfigurationParameter(MEMBER_1, Necessity.Required)]
        protected MemberInfo member1;

        // Type of second member to compare
        [ProvideConstraint(typeof(XUType))]
        [ConfigurationParameter(TYPE_2, Necessity.Required)]
        protected Type type2;

        // Second object to compare
        [Provider(TYPE_2)]
        [ConfigurationParameter(XAREUS_TYPE_2, Necessity.Required)]
        protected XUType xuType2;

        // Second member to compare
        [Provider(TYPE_2)]
        [ConfigurationParameter(MEMBER_2, Necessity.Required)]
        protected MemberInfo member2;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CompareXareusTypesMembersValuesSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (member1 == null)
                throw new ArgumentException($"First member's memberInfo is null in {typeof(CompareXareusTypesMembersValuesSensor).FullName}");

            if (member2 == null)
                throw new ArgumentException($"Second member's memberInfo is null in {typeof(CompareXareusTypesMembersValuesSensor).FullName}");

            if (member1.MemberType is not MemberTypes.Property and not MemberTypes.Field)
                throw new ArgumentException("First member's memberInfo is neither a FieldInfo or a PropertyInfo");

            if (member2.MemberType is not MemberTypes.Property and not MemberTypes.Field)
                throw new ArgumentException("Second member's memberInfo is neither a FieldInfo or a PropertyInfo");
        }

        public override Result UnityStepSensorCheck()
        {
            // Retriving member values
            object value1;
            object value2;

            value1 = member1.MemberType == MemberTypes.Field
                ? ((FieldInfo)member1).GetValue(xuType1)
                : ((PropertyInfo)member1).GetValue(xuType1, null);

            value2 = member2.MemberType == MemberTypes.Field
                ? ((FieldInfo)member2).GetValue(xuType2)
                : ((PropertyInfo)member2).GetValue(xuType2, null);

            // Comparing values
            bool result = value1.Equals(value2);

            if (result)
            {
                eventContext = new SimpleDictionary
                {
                    { XUTYPE1, xuType1 },
                    { XUTYPE2, xuType2 },
                    { VALUE, value1 }
                };
            }

            return new Result(result, eventContext);
        }

        #endregion
    }
}
