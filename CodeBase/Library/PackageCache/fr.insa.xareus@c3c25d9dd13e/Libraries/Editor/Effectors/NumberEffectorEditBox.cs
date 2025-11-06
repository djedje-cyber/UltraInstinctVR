using System;

using UnityEngine.UIElements;

using Xareus.Scenarios.Arithmetics;
using Xareus.Unity.Editor.Scenarios.UI.Containers;

using static Xareus.Scenarios.Arithmetics.NumberEffector;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(NumberEffector), "Enhanced", true)]
    public class NumberEffectorEditBox : DefaultEditBox
    {
        private Toggle operand2Toggle;
        private EnumField operatorField;

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            // Apply default Bindings
            base.Bind(selectedFunction, function);

            operand2Toggle = this.Q<FieldContainer>("operand2").Q<Toggle>("Toggle");

            operatorField = this.Q<FieldContainer>("operator").Q<EnumField>();

            // executed next frame to ensure everything is setup
            schedule.Execute(UpdateField).Resume();

            operatorField.RegisterValueChangedCallback(ev =>
            {
                UpdateField();
            });
        }

        private void UpdateField()
        {
            // depending on the Operator, we Enable or Disable the Operand2
            operand2Toggle.value = (ArithmeticalOperator)operatorField.value switch
            {
                ArithmeticalOperator.NEGATION => false,
                ArithmeticalOperator.INCREMENT => false,
                ArithmeticalOperator.DECREMENT => false,
                _ => true,
            };
        }
    }
}
