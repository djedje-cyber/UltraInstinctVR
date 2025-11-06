#if ENABLE_INPUT_SYSTEM
#if !INPUT_SYSTEM
#warning InputSystem is enabled but the InputSystem package is not installed. Please install it to use InputSystem features.
#else

using log4net;

using System.Collections.Generic;

using UnityEngine.InputSystem;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity.Input
{
    /// <summary>
    ///
    /// </summary>
    [Renamed("Xareus.Scenarios.Unity.Input.InputActionSensor", "Xareus.Unity.Librairies")]
    [FunctionDescription("This sensor validates when the specified input action is performed/pressed or canceled/released. " +
        "Note that if the input action is not enabled when the sensor starts cheking, the cancellation will only work if the action was started after the sensor was created." +
        "One way to make sure the action is enabled is to set the input action asset to a Player Input Component")]
    public class InputActionSensor : AInUnityStepSensor
    {
        #region Fields

        [EventContextEntry()]
        public const string INPUT_ACTION_KEY = "Input Action";

        [EventContextEntry()]
        public const string INPUT_ACTION_STATE_CHANGE = "State Change";

        [EventContextEntry()]
        public const string INPUT_ACTION_STATE = "State";

        #endregion

        #region Statics

        /// <summary>
        /// The log4net logger
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(InputActionSensor));

        #endregion



        #region Fields

        [ConfigurationParameter("Input Action Asset", Necessity.Required)]
        protected InputActionAsset inputActions;

        [ConfigurationParameter("Input Action Map", Necessity.Required)]
        [Provider("Input Action Asset")]
        protected InputActionMap inputActionMap;

        [ConfigurationParameter("Input Action", Necessity.Required)]
        [Provider("Input Action Map")]
        protected InputAction inputAction;

        [ConfigurationParameter("Check State Changed",
            true,
            "If enabled or not specified, the sensor validates when the action turns on or off, if disabled, " +
            "the sensor validates depending on the current state of the action, that will be ckecked at each frame",
            Necessity.Optional)]
        protected bool checkOnStatechanged;

        [ConfigurationParameter("Check Pressed",
            true,
            "If enabled or not specified, the sensor validates when the action is/turns on. Otherwise, the sensor validates when the action is/turns off",
            Necessity.Optional)]
        protected bool checkPressed;

        private readonly SimpleDictionary eventContext = new();

        #endregion

        #region Constructors

        public InputActionSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            eventContext.Clear();
            eventContext.Add(INPUT_ACTION_KEY, inputAction);
            eventContext.Add(INPUT_ACTION_STATE_CHANGE, checkOnStatechanged);
            eventContext.Add(INPUT_ACTION_STATE, checkPressed);
            // Make sure the action is enabled
            if (!inputAction.enabled)
            {
                if (LOGGER.IsWarnEnabled)
                    LOGGER.Warn(inputAction.name + " is not enabled, enabling it");
                inputAction.Enable();
            }
        }

        public override Result UnityStepSensorCheck()
        {
            bool res = checkOnStatechanged ? CheckStateChanged() : CheckCurrentState();
            return new Result(res, eventContext);
        }

        private bool CheckCurrentState()
        {
            return checkPressed == inputAction.IsPressed();
        }

        private bool CheckStateChanged()
        {
            return checkPressed ? inputAction.WasPressedThisFrame() : inputAction.WasReleasedThisFrame();
        }

        #endregion
    }
}

#endif
#endif
