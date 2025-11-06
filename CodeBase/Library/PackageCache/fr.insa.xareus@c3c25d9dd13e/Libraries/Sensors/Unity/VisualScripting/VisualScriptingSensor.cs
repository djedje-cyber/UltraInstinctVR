using System.Collections.Generic;
using System;

using Xareus.Utils;

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

    public class VisualScriptingSensor : AInUnityStepSensor
    {
        #region Fields

        public const string GRAPH_PARAM = "Flow Graph";
        public const string SENSOR_VARIABLE_NAME_PARAM = "Sensor Variable Name";

        #endregion

        public const string SENSOR_VARIABLE_DEFAULT_NAME = "Sensor";

        #region Fields

        [ConfigurationParameter(GRAPH_PARAM, "The graph that the sensor will use", Necessity.Required)]
        public ScriptGraphAsset graphAsset;

        [ConfigurationParameter(SENSOR_VARIABLE_NAME_PARAM, SENSOR_VARIABLE_DEFAULT_NAME,
            "The name of the graph variable that will contain this sensor instance.\n" +
            "When instantiating the graph, the sensor will set the graph variable value to itself (this) so that it can be used in the graph as a parameter of the " + nameof(SensorResult) + " node",
            Necessity.Required)]
        public string sensorVariable;

        private bool result;
        private ScriptMachine scriptMachine;

        #endregion

        #region Constructors

        public VisualScriptingSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
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
            scriptMachine.enabled = true;
        }

        public override void SafeStopCheck()
        {
            scriptMachine.enabled = false;
        }

        public void SetResult(bool result)
        {
            this.result = result;
        }

        public override Result UnityStepSensorCheck()
        {
            return new Result(result, null);
        }

        protected virtual void SetGraphVariables(FlowGraph graph)
        {
            GraphReference graphReference = GraphReference.New(scriptMachine, true);
            foreach (VariableDeclaration variable in graph.variables)
            {
                Parameters.TryGetValue(variable.name, out Parameter parameter);
                if (parameter != null)
                {
                    string typeName = TypeUtils.CleanTypeName(variable.typeHandle.Identification);
                    Type fieldType = ValueParser.ConvertFromString<Type>(typeName);
                    global::Unity.VisualScripting.Variables.Graph(graphReference).Set(variable.name, ValueParser.ConvertFromParameter(parameter, fieldType));
                }
            }
            global::Unity.VisualScripting.Variables.Graph(graphReference).Set(sensorVariable, this);
        }

        private void CreateScriptMachine()
        {
            FlowGraph graph = graphAsset.graph;
            graph.title = Event.label + " Sensor";

            scriptMachine = VisualScriptHolder.InstantiateGraph(graph);
            GraphReference graphReference = GraphReference.New(scriptMachine, true);
            global::Unity.VisualScripting.Variables.Graph(graphReference).Set(sensorVariable, this);
            VisualScriptingHelper.SetGraphVariables(Parameters, graphReference);
        }

        #endregion
    }

#else

    public class VisualScriptingSensor : AInUnityStepSensor
    {
        #region Constructors

        public VisualScriptingSensor(Xareus.Scenarios.Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts) :
            base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result UnityStepSensorCheck()
        {
            Debug.LogError(Constants.MISSING_VISUAL_SCRIPTING);
            return new Result(false, null);
        }

        #endregion
    }

#endif
}
