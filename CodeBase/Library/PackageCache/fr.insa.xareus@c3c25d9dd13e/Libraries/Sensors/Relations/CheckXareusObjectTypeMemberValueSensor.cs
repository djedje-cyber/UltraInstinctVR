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
    [Renamed("TypeValueSensor", "Assembly-CSharp", typeof(TypeValueSensorParametersConverter))]
    [Renamed("SEVEN.FiveObjectTypeMemberValueSensor", "Assembly-CSharp", typeof(TypeValueSensorParametersConverter))]
    [Renamed("SEVEN.FIVE.FiveObjectTypeMemberValueSensor", "Assembly-CSharp", typeof(TypeValueSensorParametersConverter))]
    [Renamed("Xareus.Scenarios.Relations.CheckXareusObjectTypeMemberValueSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("Check the value of the given member in one of the types hold by the specified Xareus Object. Note that if multiple correponding types are present, only one will be checked")]
    public class CheckXareusObjectTypeMemberValueSensor : AInUnityStepSensor
    {
        #region Const

        private const string TYPE = "Type";
        private const string MEMBER = "Member";
        private const string VALUE_PARAM = "Value";

        #endregion

        #region Statics

        [EventContextEntry()]
        public static readonly string XUTYPE = "type";

        [EventContextEntry()]
        public static readonly string VALUE = "value";

        #endregion

        #region Fields

        /// <summary>
        /// The object containing the type
        /// </summary>
        [ConfigurationParameter("Xareus Object", "The Xareus Object", Necessity.Required)]
        protected XUObject @object;

        // Type of first member to compare
        [ProvideConstraint(typeof(XUType))]
        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        // Member to check
        [Provider(TYPE)]
        [ConfigurationParameter(MEMBER, Necessity.Required)]
        protected MemberInfo member;

        // Expected value
        [Provider(MEMBER)]
        [ConfigurationParameter(VALUE_PARAM, Necessity.Required)]
        protected object expectedValue;

        protected XUType xuType;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected CheckXareusObjectTypeMemberValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result UnityStepSensorCheck()
        {
            // get the XUType (i.e. component)
            xuType = @object.GetXuType(type);

            if (xuType == null)
                return new Result(false, null);

            if (member == null)
                throw new ArgumentException("Member's memberInfo is null in {0}", typeof(CheckXareusObjectTypeMemberValueSensor).FullName);

            object value;

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
                throw new ArgumentException("Member's memberInfo is neither a FieldInfo nor a PropertyInfo");
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
