using System.Collections.Generic;
using System.Linq;

#if ENABLE_INPUT_SYSTEM
#if !INPUT_SYSTEM
#warning InputSystem is enabled but the InputSystem package is not installed. Please install it to use InputSystem features.
#else

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

#endif
#endif
#if !ENABLE_INPUT_SYSTEM || !INPUT_SYSTEM
using InriaTools;
#endif

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("AnyInputSensor", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.AnyInputSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.AnyInputSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("New Input System : Check if any device key or button is currently pressed or not.\n" +
        "Old Input System : check if any keyboard key or mouse button was just pressed at each frame.")]
    public class AnyInputSensor : AUnitySensor
    {
        #region Fields

        protected bool triggered;

        protected bool valueMustBeChecked;

        #endregion

        #region Constructors

        public AnyInputSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeStopCheck()
        {
#if !ENABLE_INPUT_SYSTEM || !INPUT_SYSTEM
            UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckInput, UnityThreadExecute.UnityExecutionStep.Update);
#endif
        }

        public override void SafeReset()
        {
#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM
            // check the current state first
            triggered = InputSystem.devices.Any(d => d.allControls.Where(control => control is ButtonControl or KeyControl).Any(c => c.IsPressed()));
            if (!triggered)
            {
                InputSystem.onEvent.Where(e => e.HasButtonPress()).CallOnce(_ =>
                {
                    triggered = true;
                    UpdateScenario();
                });
            }
#else
            UnityThreadExecute.RegisterActionForExecutionSteps(CheckInput, UnityThreadExecute.UnityExecutionStep.Update);
#endif
        }

        public override Result SafeSensorCheck()
        {
            bool tmpTriggered = triggered;
            triggered = false;
            valueMustBeChecked = false;
            return new Result(tmpTriggered, null);
        }

#if !ENABLE_INPUT_SYSTEM || !INPUT_SYSTEM
        private void CheckInput()
        {
            if (valueMustBeChecked)
                return;

            triggered = UnityEngine.Input.anyKeyDown;

            if (triggered)
            {
                valueMustBeChecked = true;
                UpdateScenario();
            }
        }
#endif

        #endregion
    }
}
