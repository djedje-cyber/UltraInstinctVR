using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

using static Xareus.Scenarios.Arithmetics.NumberSensor;

namespace Xareus.Scenarios.Unity
{
    [Renamed("Xareus.Scenarios.Unity.UnityNumberSensor", "Xareus.Unity.Librairies")]
    public class UnityNumberSensor : AInUnityStepSensor
    {
        #region Fields

        public const string COMPONENT_TYPE = "Component Type";
        public const string COMPONENT = "Component";
        public const string MEMBER = "Member";
        public const string OPERAND2 = "operand2";
        public const string PRECISION = "precision";
        public const string OPERATOR = "operator";

        [EventContextEntry()]
        public static readonly string COMPONENT_ENTRY = "component";

        [EventContextEntry()]
        public static readonly string VALUE_ENTRY = "value";

        // Member to check
        [ProvideConstraint(typeof(UnityEngine.Component))]
        [ConfigurationParameter(COMPONENT_TYPE, Necessity.Required)]
        protected Type componentType;

        // object to compare
        [Provider(COMPONENT_TYPE)]
        [ConfigurationParameter(COMPONENT, Necessity.Required)]
        protected UnityEngine.Component component;

        // Member to check
        [Provider(COMPONENT)]
        [ConfigurationParameter(MEMBER, Necessity.Required)]
        protected MemberInfo member;

        /// <summary>
        /// The second operand
        /// </summary>
        [ConfigurationParameter(OPERAND2, Necessity.Required)]
        protected float operand2;

        /// <summary>
        /// The comparison precision
        /// </summary>
        [ConfigurationParameter(PRECISION, "The precision to use for the comparisons (default 0.0000001)", Necessity.Optional, InitialValue = 0.0000001f)]
        protected float precision;

        /// <summary>
        /// The relational operator
        /// </summary>
        [ConfigurationParameter(OPERATOR, Necessity.Required, InitialValue = ComparisonOperator.LESS_THAN)]
        protected ComparisonOperator @operator;

        private SimpleDictionary eventContext;

        #endregion

        #region Constructors

        protected UnityNumberSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result UnityStepSensorCheck()
        {
            float value;

            // Comparison between member value and desired value
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo fieldInfo = member as FieldInfo;
                value = (float)fieldInfo.GetValue(component);
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = member as PropertyInfo;
                value = (float)propertyInfo.GetValue(component);
            }
            else
            {
                throw new ArgumentException("member's memberInfo is neither a FieldInfo nor a PropertyInfo");
            }
            bool result = false;
            result = @operator switch
            {
                ComparisonOperator.EQUAL => Arithmetics.Utils.IsApproximatelyEqual(value, operand2, precision),
                ComparisonOperator.NOT_EQUAL => !Arithmetics.Utils.IsApproximatelyEqual(value, operand2, precision),
                ComparisonOperator.GREATER_THAN => value > operand2,
                ComparisonOperator.GREATER_THAN_OR_EQUAL_TO => value >= operand2,
                ComparisonOperator.LESS_THAN => value < operand2,
                ComparisonOperator.LESS_THAN_OR_EQUAL_TO => value <= operand2,
                _ => throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(ComparisonOperator)),
            };
            if (result)
            {
                eventContext = new SimpleDictionary
                {
                    { COMPONENT_ENTRY, component },
                    { VALUE_ENTRY, value }
                };
            }

            return new Result(result, eventContext);
        }

        #endregion
    }
}
