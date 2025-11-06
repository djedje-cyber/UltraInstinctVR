using System;

using UnityEngine.UIElements;

using Xareus.Scenarios.Boolean;
using Xareus.Unity.Editor.Scenarios.UI.Containers;

using static Xareus.Scenarios.Boolean.BooleanEffector;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(BooleanEffector), "Enhanced", true)]
    public class BooleanEffectorEditBox : DefaultEditBox
    {
        private Toggle operand2Toggle;
        private EnumField operatorField;

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            // Apply default Bindings
            base.Bind(selectedFunction, function);

            operand2Toggle = this.Q<FieldContainer>("Operand 2").Q<Toggle>("Toggle");

            operatorField = this.Q<FieldContainer>("Operator").Q<EnumField>();

            // executed next frame to ensure the toggle is setup
            schedule.Execute(UpdateField).Resume();

            operatorField.RegisterValueChangedCallback(ev =>
            {
                UpdateField();
            });
        }

        private void UpdateField()
        {
            // depending on the Operator, we Enable or Disable the Operand2
            operand2Toggle.value = (LogicalOperator)operatorField.value switch
            {
                LogicalOperator.NOT => false,
                _ => true,
            };
        }
    }
}
