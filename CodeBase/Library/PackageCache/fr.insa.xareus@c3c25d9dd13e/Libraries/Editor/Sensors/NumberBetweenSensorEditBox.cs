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
    public partial class NumberBetweenIncludeSignField : MappingDropdownField<bool>
    {
#if !UNITY_2023_2_OR_NEWER

        #region Classes

        public new class UxmlFactory : UxmlFactory<NumberBetweenIncludeSignField, UxmlTraits> { }

        #endregion

#endif
    }

    [FunctionEditBox(typeof(NumberBetweenSensor), "Simplified", false)]
    public class NumberBetweenSensorEditBox : DefaultEditBox
    {
        #region Fields

        private readonly ConstantVariableModeGroup lowerBoundField;
        private readonly NumberBetweenIncludeSignField includeLowerBoundField;
        private readonly ConstantVariableModeGroup operandField;
        private readonly NumberBetweenIncludeSignField includeUpperBoundField;
        private readonly ConstantVariableModeGroup upperBoundField;

        #endregion

        #region Properties

        protected override IEnumerable<ABaseField> Fields => new List<ABaseField> { lowerBoundField, includeLowerBoundField, operandField, includeUpperBoundField, upperBoundField };

        #endregion

        #region Constructors

        public NumberBetweenSensorEditBox() : base()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("NumberBetweenSensorEditBox");
            visualTree.CloneTree(this);

            lowerBoundField = this.Q<ConstantVariableModeGroup>("lower-bound");

            includeLowerBoundField = this.Q<NumberBetweenIncludeSignField>("include-lower-bound");

            operandField = this.Q<ConstantVariableModeGroup>("operand");

            includeUpperBoundField = this.Q<NumberBetweenIncludeSignField>("include-upper-bound");

            upperBoundField = this.Q<ConstantVariableModeGroup>("upper-bound");
        }

        #endregion

        #region Methods

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            // lower bound
            lowerBoundField.Bind(function, function.GetParameter(NumberBetweenSensor.LOWER_BOUND));

            // lower bound operator
            includeLowerBoundField.GetStringForValue = (value) =>
            {
                return value switch
                {
                    false => "<",
                    true => "≤",
                };
            };
            includeLowerBoundField.Bind(function, function.GetParameter("Include Lower"));
            includeLowerBoundField.SetValues(new List<bool>()
            {
                false,
                true,
            });

            // operand
            operandField.Bind(function, function.GetParameter(NumberBetweenSensor.OPERAND));

            // upper bound operator
            includeUpperBoundField.GetStringForValue = (value) =>
            {
                return value switch
                {
                    false => "<",
                    true => "≤",
                };
            };
            includeUpperBoundField.Bind(function, function.GetParameter("Include Higher"));
            includeUpperBoundField.SetValues(new List<bool>()
            {
                false,
                true,
            });

            // upper bound
            upperBoundField.Bind(function, function.GetParameter(NumberBetweenSensor.HIGHER_BOUND));

            UpdateAllFieldsFromParameters();
        }

        #endregion
    }
}
