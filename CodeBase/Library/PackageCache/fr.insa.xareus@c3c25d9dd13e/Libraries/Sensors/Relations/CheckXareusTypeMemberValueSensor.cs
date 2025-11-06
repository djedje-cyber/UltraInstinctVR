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
    [Renamed("SEVEN.FiveTypeMemberValueSensor", "Assembly-CSharp", typeof(CheckXareusTypeMemberValueSensorConverter))]
    [Renamed("SEVEN.FIVE.FiveTypeMemberValueSensor", "Assembly-CSharp", typeof(CheckXareusTypeMemberValueSensorConverter))]
    [Renamed("Xareus.Scenarios.Relations.CheckXareusTypeMemberValueSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("Check the value of the given member in the given type.")]
    public class CheckXareusTypeMemberValueSensor : AInUnityStepSensor
    {
        #region Const

        private const string TYPE = "Type";
        private const string MEMBER = "Member";
        private const string XAREUS_TYPE = "Xareus Type";
        private const string VALUE_PARAM = "Value";

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string XUTYPE = "type";

        [EventContextEntry()]
        public static readonly string VALUE = "value";

        #endregion

        #region Fields

        // Type of first member to compare
        [ProvideConstraint(typeof(XUType))]
        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        // First object to compare
        [Provider(TYPE)]
        [ConfigurationParameter(XAREUS_TYPE, Necessity.Required)]
        protected XUType xuType;

        // Member to check
        [Provider(TYPE)]
        [ConfigurationParameter(MEMBER, Necessity.Required)]
        protected MemberInfo member;

        // Expected value
        [Provider(MEMBER)]
        [ConfigurationParameter(VALUE_PARAM, Necessity.Required)]
        protected object expectedValue;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckXareusTypeMemberValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result UnityStepSensorCheck()
        {
            object value;

            if (member == null)
                throw new ArgumentException($"member's memberInfo is null in {typeof(CheckXareusTypeMemberValueSensor).FullName}");

            // Comparison between member value and desired value
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo fieldInfo = member as FieldInfo;
                value = fieldInfo.GetValue(xuType);
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = member as PropertyInfo;
                value = propertyInfo.GetValue(xuType, null);
            }
            else
            {
                throw new ArgumentException("member's memberInfo is neither a FieldInfo nor a PropertyInfo");
            }

            bool result = Equals(value, expectedValue);

            if (result)
            {
                eventContext = new SimpleDictionary
                {
                    { XUTYPE, xuType },
                    { VALUE, value }
                };
            }

            return new Result(result, eventContext);
        }

        #endregion
    }
}
