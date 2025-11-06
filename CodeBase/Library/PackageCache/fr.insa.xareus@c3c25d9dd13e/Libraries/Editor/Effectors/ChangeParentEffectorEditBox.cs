using System;

using UnityEngine;
using UnityEngine.UIElements;

using Xareus.Scenarios.Unity;
using Xareus.Unity.Editor.Scenarios.UI.Containers;
using Xareus.Unity.Editor.Scenarios.UI.Group;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(ChangeParentEffector), "Enhanced", true)]

    public class ChangeParentEffectorEditBox : DefaultEditBox
    {
        private ConstantVariableModeGroup parentGroup;
        private Toggle parentToggle;
        private Fields.ObjectDropField<Transform> parentField;
        private Label labelAlternative;

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            // Apply default Bindings
            base.Bind(selectedFunction, function);

            FieldContainer parentContainer = this.Q<FieldContainer>("Parent");

            parentToggle = parentContainer.Q<Toggle>("Toggle");

            parentGroup = parentContainer.Q<ConstantVariableModeGroup>();

            parentField = parentGroup.Q<Fields.ObjectDropField<Transform>>();

            labelAlternative = new("Scene's root");
            labelAlternative.style.marginLeft = 5;
            labelAlternative.style.marginTop = 3;
            parentField.parent.Add(labelAlternative);

            // executed next frame to be after the ConstantVariableModeGroup setting the display of the field
            schedule.Execute(UpdateField).Resume();

            parentToggle.RegisterValueChangedCallback(ev => UpdateField());
        }

        private void UpdateField()
        {
            // Constant mode
            if (parentGroup.EditMode == 0)
            {
                if (parentToggle.value)
                {
                    parentField.style.display = DisplayStyle.Flex;
                    labelAlternative.style.display = DisplayStyle.None;
                }
                else
                {
                    parentField.style.display = DisplayStyle.None;
                    labelAlternative.style.display = DisplayStyle.Flex;
                }
            }
            // Variable mode
            else
            {
                labelAlternative.style.display = DisplayStyle.None;
            }
        }
    }
}
