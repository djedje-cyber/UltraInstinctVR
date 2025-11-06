#if VISUAL_SCRIPTING

using Unity.VisualScripting;

namespace Xareus.Scenarios.Unity.VisualScripting
{
    public class SensorResult : Unit
    {
        #region Fields

        [PortLabelHidden] // Hide the port label, as we normally hide the label for default Input and Output triggers.
        [DoNotSerialize] // No need to serialize ports.
        public ControlInput inputTrigger; //Adding the ControlInput port variable

        [PortLabelHidden] // Hide the port label, as we normally hide the label for default Input and Output triggers.
        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize] // No need to serialize ports
        public ValueInput result;

        [DoNotSerialize] // No need to serialize ports
        public ValueInput sensorInput;

        #endregion

        // Adding the ValueInput variable for myValueA

        #region Methods

        protected override void Definition() //The method to set what our node will be doing.
        {
            //Making the myValueA input value port visible, setting the port label name to myValueA and setting its default value to Hello.
            sensorInput = ValueInput<VisualScriptingSensor>("Sensor");

            //Making the ControlInput port visible, setting its key and running the anonymous action method to pass the flow to the outputTrigger port.
            inputTrigger = ControlInput("inputTrigger", (flow) =>
            {
                VisualScriptingSensor sensor = flow.GetValue<VisualScriptingSensor>(sensorInput);
                sensor.SetResult(flow.GetValue<bool>(result));
                return outputTrigger;
            });

            //Making the myValueA input value port visible, setting the port label name to myValueA and setting its default value to Hello.
            result = ValueInput<bool>("Sensor Result", true);

            outputTrigger = ControlOutput("outputTrigger");

            Succession(inputTrigger, outputTrigger); //Setting the succession of the inputTrigger and outputTrigger ports.
        }

        #endregion
    }
}

#endif
