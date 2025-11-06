using System;

#if VISUAL_SCRIPTING

using System.Linq;

using Unity.VisualScripting;

using UnityEditor;

#endif

using Xareus.Scenarios.Unity.VisualScripting;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(VisualScriptingEffector))]
    public class VisualScriptingEffectorEditBox : AVisualScriptingBaseEditBox
    {
        #region Constructors

        public VisualScriptingEffectorEditBox(Type selectedFunction, ScenarioFunction function) : base(selectedFunction, function)
        {
        }

        #endregion

#if VISUAL_SCRIPTING

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
                bool hasEffectorInput = false;
                // graph must contain a SensorResult unit
                foreach (Unit unit in Graph.units.OfType<Unit>())
                {
                    if (unit is EffectorInput)
                    {
                        hasEffectorInput = true;
                    }
                }
                if (!hasEffectorInput)
                {
                    EditorGUILayout.HelpBox("The graph must contain a " + nameof(EffectorInput) + " node", MessageType.Error);
                }
            }
        }

        protected override FlowGraph CreateBaseGraph()
        {
            return EffectorGraphUtils.CreateBaseEffectorGraph();
        }

        #endregion

#endif
    }
}
