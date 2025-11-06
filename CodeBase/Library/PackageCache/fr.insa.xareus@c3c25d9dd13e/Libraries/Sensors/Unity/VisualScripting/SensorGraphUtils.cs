#if VISUAL_SCRIPTING
using Unity.VisualScripting;

using UnityEngine;

namespace Xareus.Scenarios.Unity.VisualScripting
{
    public static class SensorGraphUtils
    {
        #region Methods

        // Start is called before the first frame update
        public static FlowGraph CreateBaseSensorGraph()
        {
            FlowGraph flowGraph = new();

            flowGraph.variables.Set(VisualScriptingSensor.SENSOR_VARIABLE_DEFAULT_NAME, null);
            Update flowInput = new() { position = new Vector2(-250, -30) };
            flowGraph.units.Add(flowInput);
            SensorResult sensorResult = new() { position = new Vector2(305, -30) };
            flowGraph.units.Add(sensorResult);
            GetVariable getSensor = new()
            {
                position = new Vector2(-250, 80),
                kind = VariableKind.Graph
            };

            ValueInput nameInput = new(nameof(UnifiedVariableUnit.name), typeof(string));
            getSensor.valueInputs.Add(nameInput);
            nameInput.SetDefaultValue(VisualScriptingSensor.SENSOR_VARIABLE_DEFAULT_NAME);
            typeof(UnifiedVariableUnit).GetProperty(nameof(UnifiedVariableUnit.name)).SetValue(getSensor, nameInput);
            flowGraph.units.Add(getSensor);
            flowGraph.controlConnections.Add(new ControlConnection(flowInput.trigger, sensorResult.inputTrigger));
            flowGraph.valueConnections.Add(new ValueConnection(getSensor.value, sensorResult.sensorInput));

            return flowGraph;
        }

        #endregion
    }
}
#endif
