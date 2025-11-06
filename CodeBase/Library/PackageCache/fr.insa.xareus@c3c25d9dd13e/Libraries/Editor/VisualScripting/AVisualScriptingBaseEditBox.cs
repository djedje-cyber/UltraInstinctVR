using System;

#if VISUAL_SCRIPTING

using System.Collections.Generic;

using Unity.VisualScripting;

#else

using UnityEditor.PackageManager;

#endif

using UnityEditor;

using UnityEngine;

using Xareus.Scenarios;
using Xareus.Scenarios.Unity.VisualScripting;
using Xareus.Utils;

using System.Collections;
using System.Linq;

namespace Xareus.Unity.Editor.Scenarios.UI
{
#if VISUAL_SCRIPTING

    public abstract class AVisualScriptingBaseEditBox : DefaultFunctionEditBox
    {
        #region Properties

        protected Dictionary<string, FunctionEditContainer> graphVariablesEditContainers = new();
        protected CVSAlternativeGroup CVSGraphField => functionEditContainers[VisualScriptingSensor.GRAPH_PARAM].EditField as CVSAlternativeGroup;
        protected FunctionEditFieldGeneric<ScriptGraphAsset> GraphField => CVSGraphField.GetField(0) as FunctionEditFieldGeneric<ScriptGraphAsset>;
        protected FlowGraph Graph => GraphField.SelectedValue?.graph;

        protected virtual int StartingFieldIndex => 1;

        #endregion

        #region Constructors

        protected AVisualScriptingBaseEditBox(Type selectedFunction, ScenarioFunction function) : base(selectedFunction, function)
        {
        }

        #endregion

        #region Methods

        protected IEnumerator CreateVariablesEditFields()
        {
            yield return null;
            UpdateVariablesEditFields();
        }

        protected override void DrawSpecific()
        {
            UpdateVariablesEditFields();
            base.DrawSpecific();
            // New Graph Button
            OnNewButtonGUI();
            // Edit Graph Button
            OnEditButtonGUI();
            CheckSensorGraph();
        }

        protected virtual void CheckSensorGraph()
        {
        }

        protected abstract FlowGraph CreateBaseGraph();

        protected void UpdateVariablesEditFields()
        {
            // remove all fields if no graph is present
            if (Graph == null)
            {
                foreach (KeyValuePair<string, FunctionEditContainer> container in graphVariablesEditContainers)
                {
                    elementEditContainers.RemoveAll(elem => elem.Item1 == container.Key);
                }
                graphVariablesEditContainers.Clear();
                return;
            }
            // remove fields that are not in the graph anymore
            List<string> toRemove = new();
            foreach (KeyValuePair<string, FunctionEditContainer> container in graphVariablesEditContainers)
            {
                if (!Graph.variables.Any(v => v.name == container.Key))
                {
                    toRemove.Add(container.Key);
                }
            }
            foreach (string key in toRemove)
            {
                elementEditContainers.RemoveAll(elem => elem.Item1 == key);
                graphVariablesEditContainers.Remove(key);
            }

            int skippedFields = 0;
            // go through all variables of the graph and add the missing fields at the correct position
            for (int i = 0; i < Graph.variables.Count(); i++)
            {
                VariableDeclaration variable = Graph.variables.ElementAt(i);
                // if there is no type, hide the field
                if (string.IsNullOrEmpty(variable.typeHandle.Identification))
                {
                    skippedFields++;
                    continue;
                }

                string typeName = TypeUtils.CleanTypeName(variable.typeHandle.Identification);
                Type fieldType = ValueParser.ConvertFromString<Type>(typeName);
                graphVariablesEditContainers.TryGetValue(variable.name, out FunctionEditContainer container);
                // if the field is present but the type has changed, remove it
                if (container != null && container.EditField.ParamType != ValueParser.ConvertToString<Type>(fieldType))
                {
                    elementEditContainers.Remove((variable.name, container));
                    graphVariablesEditContainers.Remove(variable.name);
                    container = null;
                }
                // if the field is not present, add it
                if (container == null)
                {
                    FunctionEditContainer newContainer = new(fieldType, variable.value, function, variable.name, variable.name, false, string.Empty);
                    graphVariablesEditContainers.Add(variable.name, newContainer);
                    elementEditContainers.Insert(StartingFieldIndex + i - skippedFields, (variable.name, newContainer));
                }
            }
        }

        private void OnNewButtonGUI()
        {
            if (GUILayout.Button("New Graph"))
            {
                FlowGraph flowGraph = CreateBaseGraph();
                ScriptableObject graphAsset = ScriptableObject.CreateInstance(typeof(ScriptGraphAsset));
                IMacro macro = (IMacro)graphAsset;
                UnityEngine.Object macroObject = (UnityEngine.Object)macro;
                macro.graph = flowGraph;

                string path = EditorUtility.SaveFilePanelInProject("Save Graph", macroObject.name, "asset", null);

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(macroObject, path);
                    GraphField.SelectedValue = (ScriptGraphAsset)graphAsset;
                }
            }
        }

        private void OnEditButtonGUI()
        {
            ScriptGraphAsset currentGraph = GraphField.SelectedValue;
            EditorGUI.BeginDisabledGroup(currentGraph == null);

            if (GUILayout.Button("Edit Graph"))
            {
                GraphWindow.OpenActive(GraphReference.New(currentGraph, false));
            }

            EditorGUI.EndDisabledGroup();
        }

        #endregion
    }

#else

    public abstract class AVisualScriptingBaseEditBox : DefaultFunctionEditBox
    {
        protected virtual int StartingFieldIndex => 1;

        #region Constructors

        protected AVisualScriptingBaseEditBox(Type selectedFunction, ScenarioFunction function) : base(selectedFunction, function)
        {
        }

        #endregion

        #region Methods

        protected override void DrawSpecific()
        {
            base.DrawSpecific();
            if (GUILayout.Button("Install Visual Scripting Package"))
            {
                Client.Add("com.unity.visualscripting");
                EditorUtility.RequestScriptReload();
            }
            EditorGUILayout.HelpBox(Constants.MISSING_VISUAL_SCRIPTING, MessageType.Error);
        }

        #endregion
    }

#endif
}
