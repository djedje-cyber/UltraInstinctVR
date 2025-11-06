#if VISUAL_SCRIPTING

using Unity.VisualScripting;

using UnityEngine;

namespace Xareus.Scenarios.Unity.VisualScripting
{
    public static class EffectorGraphUtils
    {
        #region Methods

        // Start is called before the first frame update
        public static FlowGraph CreateBaseEffectorGraph()
        {
            FlowGraph flowGraph = new();

            EffectorInput flowInput = new() { position = new Vector2(-250, -30) };
            flowGraph.units.Add(flowInput);
            return flowGraph;
        }

        #endregion
    }
}

#endif
