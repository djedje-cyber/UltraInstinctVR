using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

using Xareus.Scenarios;
using Xareus.Scenarios.Unity;
using Xareus.Unity.Editor.Utils;
using Xareus.Utils;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(UnityNumberSensor))]
    public class UnityNumberSensorEditBox : DefaultFunctionEditBox
    {
        #region Enums

        public enum EditMode
        {
            ObjectFirst,
            TypeFirst
        }

        #endregion

        #region Fields

        /// <summary>
        /// The current edit mode
        /// </summary>
        public EditMode EditBoxMode;

        /// <summary>
        /// Dropbox to select a member (field or property)
        /// </summary>
        protected CustomDropBox<MemberInfo> memberSelection;

        /// <summary>
        /// temporary parameter to be able to use edit fields
        /// </summary>
        private readonly Parameter objectParameter = new("object");

        private readonly ObjectDropField<GameObject> objectEditField;
        private GameObject targetedObject;

        #endregion

        #region Properties

        public override bool ProperlyFilled => EditBoxMode == EditMode.ObjectFirst
                  ? TargetedObject != null && memberSelection.Selected != null
                  : base.ProperlyFilled;

        public GameObject TargetedObject
        {
            get => targetedObject;
            set
            {
                if (value != targetedObject)
                {
                    targetedObject = value;
                    memberSelection.Update(GetMemberList());
                }
            }
        }

        #endregion

        #region Constructors

        public UnityNumberSensorEditBox(Type selectedFunction, ScenarioFunction function) : base(selectedFunction, function)
        {
            memberSelection = new CustomDropBox<MemberInfo>(GetMemberList(), "Select a member", ConvertName);
            objectEditField = EditFieldFactory.CreateField(typeof(GameObject), function, objectParameter) as ObjectDropField<GameObject>;
            objectEditField.ValueChangedEvent += (source, newValue) =>
            {
                TargetedObject = newValue.newValue;
            };

            objectEditField.SaveByRefChanged += (source, newValue) =>
            {
                ((functionEditContainers[UnityNumberSensor.COMPONENT].EditField as CVSAlternativeGroup).GetField(0) as ObjectDropField<Component>).SaveByRef = newValue.newValue;
            };

            // Enable the field so that it creates the associated parameter because in this view we do not display the optional checkbox
            functionEditContainers[UnityNumberSensor.NEGATE_PARAMETER].EditField.Enable();
            UpdateState();
        }

        #endregion

        #region Methods

        public string ConvertName(MemberInfo arg)
        {
            return NameConverter.ConvertName(arg);
        }

        protected override void DrawSpecific()
        {
            EditMode previousEditMode = EditBoxMode;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Toggle(EditBoxMode == EditMode.ObjectFirst, "Object First", EditorStyles.toolbarButton))
                EditBoxMode = EditMode.ObjectFirst;
            if (GUILayout.Toggle(EditBoxMode == EditMode.TypeFirst, "Type First", EditorStyles.toolbarButton))
                EditBoxMode = EditMode.TypeFirst;
            EditorGUILayout.EndHorizontal();
            bool changedMode = EditBoxMode != previousEditMode;
            switch (EditBoxMode)
            {
                case EditMode.ObjectFirst:
                    if (changedMode)
                        UpdateState();
                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Target", FieldStyle.LABELWIDTH);
                    objectEditField.Draw();
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.BeginDisabledGroup(TargetedObject == null);
                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Member", FieldStyle.LABELWIDTH);
                    memberSelection.Draw();
                    if (memberSelection.HasChanged)
                        MemberSelected();
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField("Operand2", FieldStyle.LABELWIDTH);
                    functionEditContainers[UnityNumberSensor.OPERAND2].EditField.Draw();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField("Operator", FieldStyle.LABELWIDTH);
                    functionEditContainers[UnityNumberSensor.OPERATOR].EditField.Draw();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Negate", FieldStyle.LABELWIDTH);
                    functionEditContainers[UnityNumberSensor.NEGATE_PARAMETER].EditField.Draw();
                    EditorGUILayout.EndHorizontal();
                    break;

                default:
                    if (changedMode)
                        base.UpdateAllFieldsFromParameters();
                    base.DrawSpecific();
                    break;
            }
        }

        private IEnumerable<MemberInfo> GetMemberList()
        {
            if (TargetedObject == null)
                return Enumerable.Empty<MemberInfo>();

            IEnumerable<MemberInfo> res = new List<MemberInfo>();
            foreach (Component component in TargetedObject.GetComponents<Component>())
            {
                FieldsAndPropertiesContainer fieldsAndProperties = component.GetType().GetFieldsAndProperties();
                res = res.Union(fieldsAndProperties.Fields.Where(member => member.IsPublic && typeof(float).IsAssignableFrom(member.FieldType)));
                res = res.Union(fieldsAndProperties.Properties.Where(member => member.GetGetMethod() != null && member.GetGetMethod().IsPublic && typeof(float).IsAssignableFrom(member.PropertyType)));
            }
            return res;
        }

        private void MemberSelected()
        {
            MemberInfo selected = memberSelection.Selected;

            ((functionEditContainers[UnityNumberSensor.COMPONENT_TYPE].EditField as CVSAlternativeGroup).GetField(0) as FunctionEditFieldGeneric<Type>).SelectedValue = selected.DeclaringType;

            ((functionEditContainers[UnityNumberSensor.COMPONENT].EditField as CVSAlternativeGroup).GetField(0) as ObjectDropField<Component>).SaveByRef = objectEditField.SaveByRef;
            if (TargetedObject != null)
                ((functionEditContainers[UnityNumberSensor.COMPONENT].EditField as CVSAlternativeGroup).GetField(0) as ObjectDropField<Component>).SelectedValue = TargetedObject.GetComponent(selected.ReflectedType);

            ((functionEditContainers[UnityNumberSensor.MEMBER].EditField as CVSAlternativeGroup).GetField(0) as FunctionEditFieldGeneric<MemberInfo>).SelectedValue = selected;
        }

        private void UpdateState()
        {
            Component selected = ((functionEditContainers[UnityNumberSensor.COMPONENT].EditField as CVSAlternativeGroup).GetField(0) as ObjectDropField<Component>).SelectedValue;
            objectEditField.SelectedValue = selected?.gameObject;
            objectEditField.SaveByRef = ((functionEditContainers[UnityNumberSensor.COMPONENT].EditField as CVSAlternativeGroup).GetField(0) as ObjectDropField<Component>).SaveByRef;

            memberSelection.Selected = ((functionEditContainers[UnityNumberSensor.MEMBER].EditField as CVSAlternativeGroup).GetField(0) as FunctionEditFieldGeneric<MemberInfo>).SelectedValue;
        }

        #endregion
    }
}
