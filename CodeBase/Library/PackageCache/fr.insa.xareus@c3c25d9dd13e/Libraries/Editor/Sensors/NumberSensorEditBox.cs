using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using Xareus.Scenarios.Arithmetics;
using Xareus.Unity.Editor.Scenarios.UI.Fields;
using Xareus.Unity.Editor.Scenarios.UI.Group;

namespace Xareus.Unity.Editor.Scenarios.UI
{
#if UNITY_2023_2_OR_NEWER

    [UxmlElement]
#endif
    public partial class NumberSensorComparisonOperatorField : MappingDropdownField<NumberSensor.ComparisonOperator>
    {
#if !UNITY_2023_2_OR_NEWER

        #region Classes

        public new class UxmlFactory : UxmlFactory<NumberSensorComparisonOperatorField, UxmlTraits> { }

        #endregion

#endif
    }

    [FunctionEditBox(typeof(NumberSensor), "Simplified", false)]
    public class NumberSensorEditBox : DefaultEditBox
    {
        #region Fields

        private readonly ConstantVariableModeGroup leftOperandField;
        private readonly NumberSensorComparisonOperatorField operatorField;
        private readonly ConstantVariableModeGroup rightOperandField;

        #endregion

        #region Properties

        protected override IEnumerable<ABaseField> Fields => new List<ABaseField> { leftOperandField, operatorField, rightOperandField };

        #endregion

        #region Constructors

        public NumberSensorEditBox() : base()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("NumberSensorEditBox");
            visualTree.CloneTree(this);

            leftOperandField = this.Q<ConstantVariableModeGroup>("left-operand");

            operatorField = this.Q<NumberSensorComparisonOperatorField>("operator");

            rightOperandField = this.Q<ConstantVariableModeGroup>("right-operand");
        }

        #endregion

        #region Methods

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            // Left operand
            leftOperandField.Bind(function, function.GetParameter(NumberSensor.OPERAND_1));

            // Operator
            operatorField.GetStringForValue = (value) =>
            {
                return value switch
                {
                    NumberSensor.ComparisonOperator.EQUAL => "=",
                    NumberSensor.ComparisonOperator.NOT_EQUAL => "≠",
                    NumberSensor.ComparisonOperator.GREATER_THAN => ">",
                    NumberSensor.ComparisonOperator.LESS_THAN => "<",
                    NumberSensor.ComparisonOperator.GREATER_THAN_OR_EQUAL_TO => "≥",
                    NumberSensor.ComparisonOperator.LESS_THAN_OR_EQUAL_TO => "≤",
                    _ => "?"
                };
            };
            operatorField.Bind(function, function.GetParameter("operator"));
            operatorField.SetValues(new List<NumberSensor.ComparisonOperator>()
            {
                NumberSensor.ComparisonOperator.EQUAL,
                NumberSensor.ComparisonOperator.NOT_EQUAL,
                NumberSensor.ComparisonOperator.GREATER_THAN,
                NumberSensor.ComparisonOperator.LESS_THAN,
                NumberSensor.ComparisonOperator.GREATER_THAN_OR_EQUAL_TO,
                NumberSensor.ComparisonOperator.LESS_THAN_OR_EQUAL_TO
            });

            // Right operand
            rightOperandField.Bind(function, function.GetParameter(NumberSensor.OPERAND_2));

            UpdateAllFieldsFromParameters();
        }

        #endregion
    }
}
