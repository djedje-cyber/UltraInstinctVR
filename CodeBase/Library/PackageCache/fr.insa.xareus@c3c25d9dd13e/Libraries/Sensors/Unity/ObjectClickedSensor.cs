using System.Collections.Generic;

using UnityEngine;

#if ENABLE_INPUT_SYSTEM
#if !INPUT_SYSTEM
#warning InputSystem is enabled but the InputSystem package is not installed. Please install it to use InputSystem features.
#else

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

#endif
#endif

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.ObjectClickedSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.ObjectClickedSensor", "Xareus.Unity.Librairies")]
    public class ObjectClickedSensor : AInUnityStepSensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string TARGET = "Target";

        [EventContextEntry()]
        public static readonly string MOUSE_BUTTON = "Mouse Button";

        [EventContextEntry()]
        public static readonly string ON_RELEASE = "On Release";

        [ConfigurationParameter("Target", "The target object that will trigger the sensor", Necessity.Required)]
        protected GameObject target_object;

        [ConfigurationParameter("Mouse Button", "The mouse button to check", Necessity.Required)]
#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM
        protected MouseButton button;

#else
        protected int button;
#endif

        [ConfigurationParameter("On Release", "Trigger when the click is released while on the target", Necessity.Required)]
        protected bool onRelease;

        /// <summary>
        /// The result
        /// </summary>
        protected SimpleDictionary eventContext;

        #endregion

        #region Constructors

        public ObjectClickedSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            eventContext = new SimpleDictionary
            {
                { TARGET , target_object },
                { MOUSE_BUTTON , button },
                {ON_RELEASE, onRelease }
            };
        }

        /// <summary>
        /// Checks the raycast and the button state
        /// </summary>
        public override Result UnityStepSensorCheck()
        {
            bool objectTriggered = false;
#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM
            bool mouseClicked = false;
            if (Mouse.current != null)
            {
                mouseClicked = button switch
                {
                    MouseButton.Right => Mouse.current.rightButton.isPressed,
                    MouseButton.Middle => Mouse.current.middleButton.isPressed,
                    MouseButton.Forward => Mouse.current.forwardButton.isPressed,
                    MouseButton.Back => Mouse.current.backButton.isPressed,
                    _ => Mouse.current.leftButton.isPressed,
                };
            }
            else
            {
                // in batchmode
                mouseClicked = false;
            }
#else
            bool mouseClicked = UnityEngine.Input.GetMouseButton(button);
#endif
            if (!(!onRelease ^ mouseClicked))
            {
#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
                Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
#endif
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    objectTriggered = true;
                }
            }

            return new Result(objectTriggered, eventContext);
        }

        #endregion
    }
}
