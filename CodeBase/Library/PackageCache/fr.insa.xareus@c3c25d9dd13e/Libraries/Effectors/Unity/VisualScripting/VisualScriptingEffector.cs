using System.Collections.Generic;

#if VISUAL_SCRIPTING

using Unity.VisualScripting;

using Xareus.Scenarios.Utilities;

#else

using UnityEngine;

#endif

using Xareus.Scenarios.Context;

namespace Xareus.Scenarios.Unity.VisualScripting
{
#if VISUAL_SCRIPTING

    public class VisualScriptingEffector : AUnityEffector
    {
        #region Fields

        public const string GRAPH_PARAM = "Flow Graph";

        [ConfigurationParameter(GRAPH_PARAM, "The graph that the sensor will use", Necessity.Required)]
        public ScriptGraphAsset graphAsset;

        private ScriptMachine scriptMachine;

        #endregion

        #region Constructors

        public VisualScriptingEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts) :
            base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (scriptMachine == null)
            {
                CreateScriptMachine();
            }
        }

        public override void SafeEffectorUpdate()
        {
            EventBus.Trigger(EffectorInput.INPUT_EVENT, scriptMachine.gameObject);
        }

        private void CreateScriptMachine()
        {
            FlowGraph graph = graphAsset.graph;
            graph.title = Event.label + " Effector";
            scriptMachine = VisualScriptHolder.InstantiateGraph(graph, graphAsset);
            GraphReference graphReference = GraphReference.New(scriptMachine, true);
        }

        #endregion
    }

#else

    public class VisualScriptingEffector : AUnityEffector
    {
        #region Constructors

        public VisualScriptingEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts) : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            Debug.LogError(Constants.MISSING_VISUAL_SCRIPTING);
        }

        #endregion
    }

#endif
}
