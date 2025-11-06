using System;

using Xareus.Scenarios.Unity.VisualScripting;

#if VISUAL_SCRIPTING

using System.Linq;

using Unity.VisualScripting;

using UnityEditor;

#endif

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(VisualScriptingSensor))]
    public class VisualScriptingSensorEditBox : AVisualScriptingBaseEditBox
    {
        #region Properties

        protected override int StartingFieldIndex => 2;

        #endregion

        #region Constructors

        public VisualScriptingSensorEditBox(Type selectedFunction, ScenarioFunction function) : base(selectedFunction, function)
        {
        }

        #endregion

#if VISUAL_SCRIPTING



        #region Properties

        protected CVSAlternativeGroup CVSSensorVariableField => functionEditContainers[VisualScriptingSensor.SENSOR_VARIABLE_NAME_PARAM].EditField as CVSAlternativeGroup;

        protected FunctionEditFieldGeneric<string> SensorVariableNameField => CVSSensorVariableField.GetField(0) as FunctionEditFieldGeneric<string>;

        #endregion

        #region Methods

        protected override void CheckSensorGraph()
        {
            if (GraphField.SelectedValue == null)
            {
                return;
            }

            // If in constant mode
            if (CVSGraphField.EditMode == 0)
            {
                bool hasSensorResult = false;
                // graph must contain a SensorResult unit
                foreach (Unit unit in Graph.units.OfType<Unit>())
                {
                    if (unit is SensorResult)
                    {
                        hasSensorResult = true;
                    }
                }
                if (!hasSensorResult)
                {
                    EditorGUILayout.HelpBox("The graph must contain a " + nameof(SensorResult) + " node", MessageType.Error);
                }
            }

            // If in constant mode
            if (CVSSensorVariableField.EditMode == 0)
            {
                bool hasCorrectVariable = false;
                // graph must contain a variable with the correct name
                foreach (VariableDeclaration variable in Graph.variables)
                {
                    if (variable.name == SensorVariableNameField.SelectedValue)
                    {
                        hasCorrectVariable = true;
                    }
                }
                if (!hasCorrectVariable)
                {
                    EditorGUILayout.HelpBox("The graph must contain a variable with the name " + SensorVariableNameField.SelectedValue, MessageType.Error);
                }
            }
        }

        protected override FlowGraph CreateBaseGraph()
        {
            return SensorGraphUtils.CreateBaseSensorGraph();
        }

        #endregion

#endif
    }
}
