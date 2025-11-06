using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using Xareus.Scenarios.Boolean;
using Xareus.Unity.Editor.Scenarios.UI.Fields;
using Xareus.Unity.Editor.Scenarios.UI.Group;

namespace Xareus.Unity.Editor.Scenarios.UI
{
#if UNITY_2023_2_OR_NEWER

    [UxmlElement]
#endif
    public partial class BooleanSensorComparisonOperatorDropdownField : MappingDropdownField<BooleanSensor.ComparisonOperator>
    {
#if !UNITY_2023_2_OR_NEWER

        #region Classes

        public new class UxmlFactory : UxmlFactory<BooleanSensorComparisonOperatorDropdownField, UxmlTraits> { }

        #endregion

#endif
    }

    [FunctionEditBox(typeof(BooleanSensor), "Simplified", false)]
    public class BooleanSensorEditBox : DefaultEditBox
    {
        #region Fields

        private readonly ConstantVariableModeGroup operand1Field;
        private readonly ConstantVariableModeGroup operand2Field;
        private readonly BooleanSensorComparisonOperatorDropdownField operatorField;

        #endregion

        #region Properties

        protected override IEnumerable<ABaseField> Fields => new List<ABaseField> { operand1Field, operand2Field, operatorField };

        #endregion

        #region Constructors

        public BooleanSensorEditBox() : base()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("BooleanSensorEditBox");
            visualTree.CloneTree(this);

            operand1Field = this.Q<ConstantVariableModeGroup>("operand1");

            operand2Field = this.Q<ConstantVariableModeGroup>("operand2");

            operatorField = this.Q<BooleanSensorComparisonOperatorDropdownField>("operator");
        }

        #endregion

        #region Methods

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            operand1Field.Bind(function, function.GetParameter(BooleanSensor.OPERAND_1));
            operand2Field.Bind(function, function.GetParameter(BooleanSensor.OPERAND_2));

            operatorField.GetStringForValue = (value) =>
            {
                return value switch
                {
                    BooleanSensor.ComparisonOperator.NOT_EQUAL => "≠",
                    _ => "=",
                };
            };
            operatorField.Bind(function, function.GetParameter(BooleanSensor.OPERATOR));
            operatorField.SetValues(new List<BooleanSensor.ComparisonOperator> {
                BooleanSensor.ComparisonOperator.EQUAL,
                BooleanSensor.ComparisonOperator.NOT_EQUAL
            });

            UpdateAllFieldsFromParameters();
        }

        #endregion
    }
}
