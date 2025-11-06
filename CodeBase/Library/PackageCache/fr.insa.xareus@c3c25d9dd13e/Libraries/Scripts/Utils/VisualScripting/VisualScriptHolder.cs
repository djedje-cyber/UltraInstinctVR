using InriaTools;

#if VISUAL_SCRIPTING

using System;

using Unity.VisualScripting;

using UnityEngine;

#endif

namespace Xareus.Scenarios.Unity.VisualScripting
{
    public class VisualScriptHolder : UnitySingleton<VisualScriptHolder>
    {
        #region Methods

#if VISUAL_SCRIPTING

        public static ScriptMachine InstantiateGraph(FlowGraph graph, ScriptGraphAsset graphAsset = null)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));
            GameObject graphHolder = new(graph.title);
            ScriptMachine scriptMachine = graphHolder.AddComponent<ScriptMachine>();
            if (graphAsset == null)
                scriptMachine.nest.SwitchToEmbed(graph);
            else
                scriptMachine.nest.SwitchToMacro(graphAsset);
            graphHolder.transform.SetParent(Instance.transform);
            return scriptMachine;
        }

#endif

        #endregion
    }
}
