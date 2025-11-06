using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

using InriaTools;

using UnityEngine.InputSystem.Controls;

#if VRTOOLS
using Hybrid.VRTools;
#endif

#if MIDDLEVR
using SEVEN.Input.MiddleVR;
#endif

#if ENABLE_INPUT_SYSTEM
#if !INPUT_SYSTEM
#warning InputSystem is enabled but the InputSystem package is not installed. Please install it to use InputSystem features.
#else

#if INPUT_SYSTEM_0_2_0

using UnityEngine.InputSystem;

#else
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.LowLevel;
#endif
#endif
#endif

namespace Xareus.Scenarios.Input
{
    /// <summary>
    /// Checks for a keyboard input. Note that inputs happening between two scenario update will activate the sensor
    /// </summary>
    [Renamed("Xareus.Scenario.Input.KeyPressedSensor", "Xareus.Scenario.Input")]
    [Renamed("Xareus.Scenarios.Input.KeyPressedSensor", "Xareus.Unity.Librairies.Input")]
    public class KeyPressedSensor : AUnitySensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string KEY = "key";

        #endregion



        #region Fields

        [ConfigurationParameter("Key", Necessity = Necessity.Required)]
        // legacy input system
        protected KeyCode keyCode;

        [ConfigurationParameter("Convert To Layout", "Converts the key to the current layout of the keyboard (can be costly!)", InitialValue = true, Necessity = Necessity.Optional)]
        protected bool convertToLayout;

        // MiddleVR input system
        protected uint mvr_keycode;

        /// <summary>
        /// The result
        /// </summary>
        protected SimpleDictionary eventContext;

#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM

        // New input system
        protected Key key;

#endif
        private bool keyPressed;

        #endregion

        #region Constructors

        public KeyPressedSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeStopCheck()
        {
#if VRTOOLS
            UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckVrToolsInput, UnityThreadExecute.UnityExecutionStep.Update);
#elif ENABLE_INPUT_SYSTEM && INPUT_SYSTEM
            UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckNewUnityInput, UnityThreadExecute.UnityExecutionStep.Update);
#elif MIDDLEVR
            UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckMVRInput, UnityThreadExecute.UnityExecutionStep.Update);
#elif ENABLE_LEGACY_INPUT_MANAGER || !UNITY_2019_2_OR_NEWER
            UnityThreadExecute.UnRegisterActionForExecutionSteps(CheckUnityInput, UnityThreadExecute.UnityExecutionStep.Update);
#endif
        }

        public override Result SafeSensorCheck()
        {
            return new Result(keyPressed, eventContext);
        }

        public override void SafeReset()
        {
            base.SafeReset();
            //Use VRTools if present
#if VRTOOLS
            UnityThreadExecute.RegisterActionForExecutionSteps(CheckVrToolsInput, UnityThreadExecute.UnityExecutionStep.Update);
#elif ENABLE_INPUT_SYSTEM && INPUT_SYSTEM
            key = KeyCodeToKey(keyCode, convertToLayout);
            UnityThreadExecute.RegisterActionForExecutionSteps(CheckNewUnityInput, UnityThreadExecute.UnityExecutionStep.Update);
#elif MIDDLEVR
            mvr_keycode = TranslateKeyCode(keyCode);
            UnityThreadExecute.RegisterActionForExecutionSteps(CheckMVRInput, UnityThreadExecute.UnityExecutionStep.Update);
#elif ENABLE_LEGACY_INPUT_MANAGER || !UNITY_2019_2_OR_NEWER
            UnityThreadExecute.RegisterActionForExecutionSteps(CheckUnityInput, UnityThreadExecute.UnityExecutionStep.Update);
#endif

            // Prepare the event context
            eventContext = new SimpleDictionary
            {
                { KEY, keyCode }
            };
        }

        #endregion

        #region CheckInput

#if VRTOOLS
        private void CheckVrToolsInput()
        {
            if (keyPressed != VRTool.GetKeyDown(keyCode))
            {
                keyPressed = !keyPressed;
                UpdateScenario();
            }
        }
#elif ENABLE_INPUT_SYSTEM && INPUT_SYSTEM

        protected void CheckNewUnityInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                CheckKeyboardState(keyboard);
            }
        }

        private void CheckKeyboardState(Keyboard keyboard)
        {
            if (keyPressed != keyboard[key].isPressed)
            {
                keyPressed = !keyPressed;
                UpdateScenario();
            }
        }

#elif MIDDLEVR

        private void CheckMVRInput()
        {
            if (keyPressed != (Mvr2Adapter.VRDeviceMgr.IsKeyPressed(mvr_keycode) || Mvr2Adapter.VRDeviceMgr.IsKeyToggled(mvr_keycode)))
            {
                keyPressed = !keyPressed;
                UpdateScenario();
            }
        }

#elif ENABLE_LEGACY_INPUT_MANAGER || !UNITY_2019_2_OR_NEWER

        private void CheckUnityInput()
        {
            if (keyPressed != UnityEngine.Input.GetKeyDown(keyCode))
            {
                keyPressed = !keyPressed;
                UpdateScenario();
            }
        }

#endif

        #endregion

        // For MIDDLEVR when VRTools not present
#if !VRTOOLS && MIDDLEVR
        /// <summary>
        /// Translates Unity key code to the MiddleVR KeyCode with respect to azerty keyboard
        /// </summary>
        /// <returns>The MiddleVR key code.</returns>
        /// <param name="k">Unity Keycode to translate</param>
#pragma warning disable CS0618 // obsolete warning

        private static uint TranslateKeyCode(KeyCode k)
        {
            switch (k)
            {
                case KeyCode.A:
                    return Mvr2Adapter.VRK_Q;

                case KeyCode.B:
                    return Mvr2Adapter.VRK_B;

                case KeyCode.C:
                    return Mvr2Adapter.VRK_C;

                case KeyCode.D:
                    return Mvr2Adapter.VRK_D;

                case KeyCode.E:
                    return Mvr2Adapter.VRK_E;

                case KeyCode.F:
                    return Mvr2Adapter.VRK_F;

                case KeyCode.G:
                    return Mvr2Adapter.VRK_G;

                case KeyCode.H:
                    return Mvr2Adapter.VRK_H;

                case KeyCode.I:
                    return Mvr2Adapter.VRK_I;

                case KeyCode.J:
                    return Mvr2Adapter.VRK_J;

                case KeyCode.K:
                    return Mvr2Adapter.VRK_K;

                case KeyCode.L:
                    return Mvr2Adapter.VRK_L;

                case KeyCode.M:
                    return Mvr2Adapter.VRK_SEMICOLON;

                case KeyCode.N:
                    return Mvr2Adapter.VRK_N;

                case KeyCode.O:
                    return Mvr2Adapter.VRK_O;

                case KeyCode.P:
                    return Mvr2Adapter.VRK_P;

                case KeyCode.Q:
                    return Mvr2Adapter.VRK_A;

                case KeyCode.R:
                    return Mvr2Adapter.VRK_R;

                case KeyCode.S:
                    return Mvr2Adapter.VRK_S;

                case KeyCode.T:
                    return Mvr2Adapter.VRK_T;

                case KeyCode.U:
                    return Mvr2Adapter.VRK_U;

                case KeyCode.V:
                    return Mvr2Adapter.VRK_V;

                case KeyCode.W:
                    return Mvr2Adapter.VRK_Z;

                case KeyCode.X:
                    return Mvr2Adapter.VRK_X;

                case KeyCode.Y:
                    return Mvr2Adapter.VRK_Y;

                case KeyCode.Z:
                    return Mvr2Adapter.VRK_W;

                case KeyCode.Alpha1:
                    return Mvr2Adapter.VRK_1;

                case KeyCode.Alpha2:
                    return Mvr2Adapter.VRK_2;

                case KeyCode.Alpha3:
                    return Mvr2Adapter.VRK_3;

                case KeyCode.Alpha4:
                    return Mvr2Adapter.VRK_4;

                case KeyCode.Alpha5:
                    return Mvr2Adapter.VRK_5;

                case KeyCode.Alpha6:
                    return Mvr2Adapter.VRK_6;

                case KeyCode.Alpha7:
                    return Mvr2Adapter.VRK_7;

                case KeyCode.Alpha8:
                    return Mvr2Adapter.VRK_8;

                case KeyCode.Alpha9:
                    return Mvr2Adapter.VRK_9;

                case KeyCode.Alpha0:
                    return Mvr2Adapter.VRK_0;

                case KeyCode.Space:
                    return Mvr2Adapter.VRK_SPACE;

                case KeyCode.UpArrow:
                    return Mvr2Adapter.VRK_UP;

                case KeyCode.DownArrow:
                    return Mvr2Adapter.VRK_DOWN;

                case KeyCode.LeftArrow:
                    return Mvr2Adapter.VRK_LEFT;

                case KeyCode.RightArrow:
                    return Mvr2Adapter.VRK_RIGHT;

                case KeyCode.Keypad0:
                    return Mvr2Adapter.VRK_NUMPAD0;

                case KeyCode.Keypad1:
                    return Mvr2Adapter.VRK_NUMPAD1;

                case KeyCode.Keypad2:
                    return Mvr2Adapter.VRK_NUMPAD2;

                case KeyCode.Keypad3:
                    return Mvr2Adapter.VRK_NUMPAD3;

                case KeyCode.Keypad4:
                    return Mvr2Adapter.VRK_NUMPAD4;

                case KeyCode.Keypad5:
                    return Mvr2Adapter.VRK_NUMPAD5;

                case KeyCode.Keypad6:
                    return Mvr2Adapter.VRK_NUMPAD6;

                case KeyCode.Keypad7:
                    return Mvr2Adapter.VRK_NUMPAD7;

                case KeyCode.Keypad8:
                    return Mvr2Adapter.VRK_NUMPAD8;

                case KeyCode.Keypad9:
                    return Mvr2Adapter.VRK_NUMPAD9;

                case KeyCode.KeypadDivide:
                    return Mvr2Adapter.VRK_DIVIDE;

                case KeyCode.KeypadMultiply:
                    return Mvr2Adapter.VRK_MULTIPLY;

                case KeyCode.KeypadMinus:
                    return Mvr2Adapter.VRK_SUBTRACT;

                case KeyCode.KeypadPlus:
                    return Mvr2Adapter.VRK_ADD;

                case KeyCode.KeypadEnter:
                    return Mvr2Adapter.VRK_NUMPADENTER;

                case KeyCode.KeypadPeriod:
                    return Mvr2Adapter.VRK_DECIMAL;

                case KeyCode.Insert:
                    return Mvr2Adapter.VRK_INSERT;

                case KeyCode.Delete:
                    return Mvr2Adapter.VRK_DELETE;

                case KeyCode.Home:
                    return Mvr2Adapter.VRK_HOME;

                case KeyCode.End:
                    return Mvr2Adapter.VRK_END;

                case KeyCode.PageUp:
                    return Mvr2Adapter.VRK_PRIOR;

                case KeyCode.PageDown:
                    return Mvr2Adapter.VRK_NEXT;

                case KeyCode.Escape:
                    return Mvr2Adapter.VRK_ESCAPE;

                case KeyCode.LeftControl:
                    return Mvr2Adapter.VRK_LCONTROL;

                case KeyCode.RightControl:
                    return Mvr2Adapter.VRK_RCONTROL;

                case KeyCode.LeftAlt:
                    return Mvr2Adapter.VRK_ALTLEFT;

                case KeyCode.RightAlt:
                    return Mvr2Adapter.VRK_ALTRIGHT;

                case KeyCode.LeftShift:
                    return Mvr2Adapter.VRK_LSHIFT;

                case KeyCode.RightShift:
                    return Mvr2Adapter.VRK_RSHIFT;

                case KeyCode.Less:
                    return Mvr2Adapter.VRK_OEM_102;

                case KeyCode.Comma:
                    return Mvr2Adapter.VRK_M;

                case KeyCode.Semicolon:
                    return Mvr2Adapter.VRK_COMMA;

                case KeyCode.Colon:
                    return Mvr2Adapter.VRK_PERIOD;

                case KeyCode.Exclaim:
                    return Mvr2Adapter.VRK_SLASH;

                case KeyCode.Return:
                    return Mvr2Adapter.VRK_RETURN;

                case KeyCode.Equals:
                    return Mvr2Adapter.VRK_EQUALS;

                case KeyCode.Backspace:
                    return Mvr2Adapter.VRK_BACK;

                case KeyCode.RightBracket:
                    return Mvr2Adapter.VRK_MINUS;

                case KeyCode.Tab:
                    return Mvr2Adapter.VRK_TAB;

                case KeyCode.F1:
                    return Mvr2Adapter.VRK_F1;

                case KeyCode.F2:
                    return Mvr2Adapter.VRK_F2;

                case KeyCode.F3:
                    return Mvr2Adapter.VRK_F3;

                case KeyCode.F4:
                    return Mvr2Adapter.VRK_F4;

                case KeyCode.F5:
                    return Mvr2Adapter.VRK_F5;

                case KeyCode.F6:
                    return Mvr2Adapter.VRK_F6;

                case KeyCode.F7:
                    return Mvr2Adapter.VRK_F7;

                case KeyCode.F8:
                    return Mvr2Adapter.VRK_F8;

                case KeyCode.F9:
                    return Mvr2Adapter.VRK_F9;

                case KeyCode.F10:
                    return Mvr2Adapter.VRK_F10;

                case KeyCode.F11:
                    return Mvr2Adapter.VRK_F11;

                case KeyCode.F12:
                    return Mvr2Adapter.VRK_F12;

                case KeyCode.ScrollLock:
                    return Mvr2Adapter.VRK_SCROLL;

                case KeyCode.Pause:
                    return Mvr2Adapter.VRK_PAUSE;

                default:
                    return Mvr2Adapter.VRK_ESCAPE;
            }
        }

#pragma warning restore CS0618 // obsolete warning

#endif

#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM

        private static Key ConvertLayout(Key key)
        {
            if (Keyboard.current != null)
            {
                KeyControl keyControl = Keyboard.current.FindKeyOnCurrentKeyboardLayout(Keyboard.current[key].name);
                return keyControl != null ? keyControl.keyCode : key;
            }
            else
            {
                // in batchmode
                return key;
            }
        }

        private static Key KeyCodeToKey(KeyCode keyCode, bool convertToLayout,
            Key unknownKey = Key.None,
            Key mouseKey = Key.None,
            Key joystickKey = Key.None)
        {
            Key usLayoutKey = keyCode switch
            {
                KeyCode.None => Key.None,
                KeyCode.Backspace => Key.Backspace,
                KeyCode.Delete => Key.Delete,
                KeyCode.Tab => Key.Tab,
                KeyCode.Clear => unknownKey,// Conversion unknown.
                KeyCode.Return => Key.Enter,
                KeyCode.Pause => Key.Pause,
                KeyCode.Escape => Key.Escape,
                KeyCode.Space => Key.Space,
                KeyCode.Keypad0 => Key.Numpad0,
                KeyCode.Keypad1 => Key.Numpad1,
                KeyCode.Keypad2 => Key.Numpad2,
                KeyCode.Keypad3 => Key.Numpad3,
                KeyCode.Keypad4 => Key.Numpad4,
                KeyCode.Keypad5 => Key.Numpad5,
                KeyCode.Keypad6 => Key.Numpad6,
                KeyCode.Keypad7 => Key.Numpad7,
                KeyCode.Keypad8 => Key.Numpad8,
                KeyCode.Keypad9 => Key.Numpad9,
                KeyCode.KeypadPeriod => Key.NumpadPeriod,
                KeyCode.KeypadDivide => Key.NumpadDivide,
                KeyCode.KeypadMultiply => Key.NumpadMultiply,
                KeyCode.KeypadMinus => Key.NumpadMinus,
                KeyCode.KeypadPlus => Key.NumpadPlus,
                KeyCode.KeypadEnter => Key.NumpadEnter,
                KeyCode.KeypadEquals => Key.NumpadEquals,
                KeyCode.UpArrow => Key.UpArrow,
                KeyCode.DownArrow => Key.DownArrow,
                KeyCode.RightArrow => Key.RightArrow,
                KeyCode.LeftArrow => Key.LeftArrow,
                KeyCode.Insert => Key.Insert,
                KeyCode.Home => Key.Home,
                KeyCode.End => Key.End,
                KeyCode.PageUp => Key.PageUp,
                KeyCode.PageDown => Key.PageDown,
                KeyCode.F1 => Key.F1,
                KeyCode.F2 => Key.F2,
                KeyCode.F3 => Key.F3,
                KeyCode.F4 => Key.F4,
                KeyCode.F5 => Key.F5,
                KeyCode.F6 => Key.F6,
                KeyCode.F7 => Key.F7,
                KeyCode.F8 => Key.F8,
                KeyCode.F9 => Key.F9,
                KeyCode.F10 => Key.F10,
                KeyCode.F11 => Key.F11,
                KeyCode.F12 => Key.F12,
                KeyCode.F13 => unknownKey,// Conversion unknown.
                KeyCode.F14 => unknownKey,// Conversion unknown.
                KeyCode.F15 => unknownKey,// Conversion unknown.
                KeyCode.Alpha0 => Key.Digit0,
                KeyCode.Alpha1 => Key.Digit1,
                KeyCode.Alpha2 => Key.Digit2,
                KeyCode.Alpha3 => Key.Digit3,
                KeyCode.Alpha4 => Key.Digit4,
                KeyCode.Alpha5 => Key.Digit5,
                KeyCode.Alpha6 => Key.Digit6,
                KeyCode.Alpha7 => Key.Digit7,
                KeyCode.Alpha8 => Key.Digit8,
                KeyCode.Alpha9 => Key.Digit9,
                KeyCode.Exclaim => unknownKey,// Conversion unknown.
                KeyCode.DoubleQuote => unknownKey,// Conversion unknown.
                KeyCode.Hash => unknownKey,// Conversion unknown.
                KeyCode.Dollar => unknownKey,// Conversion unknown.
                KeyCode.Percent => unknownKey,// Conversion unknown.
                KeyCode.Ampersand => unknownKey,// Conversion unknown.
                KeyCode.Quote => Key.Quote,
                KeyCode.LeftParen => unknownKey,// Conversion unknown.
                KeyCode.RightParen => unknownKey,// Conversion unknown.
                KeyCode.Asterisk => unknownKey,// Conversion unknown.
                KeyCode.Plus => Key.None,// TODO
                KeyCode.Comma => Key.Comma,
                KeyCode.Minus => Key.Minus,
                KeyCode.Period => Key.Period,
                KeyCode.Slash => Key.Slash,
                KeyCode.Colon => unknownKey,// Conversion unknown.
                KeyCode.Semicolon => Key.Semicolon,
                KeyCode.Less => Key.None,
                KeyCode.Equals => Key.Equals,
                KeyCode.Greater => unknownKey,// Conversion unknown.
                KeyCode.Question => unknownKey,// Conversion unknown.
                KeyCode.At => unknownKey,// Conversion unknown.
                KeyCode.LeftBracket => Key.LeftBracket,
                KeyCode.Backslash => Key.Backslash,
                KeyCode.RightBracket => Key.RightBracket,
                KeyCode.Caret => Key.None,// TODO
                KeyCode.Underscore => unknownKey,// Conversion unknown.
                KeyCode.BackQuote => Key.Backquote,
                KeyCode.A => Key.A,
                KeyCode.B => Key.B,
                KeyCode.C => Key.C,
                KeyCode.D => Key.D,
                KeyCode.E => Key.E,
                KeyCode.F => Key.F,
                KeyCode.G => Key.G,
                KeyCode.H => Key.H,
                KeyCode.I => Key.I,
                KeyCode.J => Key.J,
                KeyCode.K => Key.K,
                KeyCode.L => Key.L,
                KeyCode.M => Key.M,
                KeyCode.N => Key.N,
                KeyCode.O => Key.O,
                KeyCode.P => Key.P,
                KeyCode.Q => Key.Q,
                KeyCode.R => Key.R,
                KeyCode.S => Key.S,
                KeyCode.T => Key.T,
                KeyCode.U => Key.U,
                KeyCode.V => Key.V,
                KeyCode.W => Key.W,
                KeyCode.X => Key.X,
                KeyCode.Y => Key.Y,
                KeyCode.Z => Key.Z,
                KeyCode.LeftCurlyBracket => unknownKey,// Conversion unknown.
                KeyCode.Pipe => unknownKey,// Conversion unknown.
                KeyCode.RightCurlyBracket => unknownKey,// Conversion unknown.
                KeyCode.Tilde => unknownKey,// Conversion unknown.
                KeyCode.Numlock => Key.NumLock,
                KeyCode.CapsLock => Key.CapsLock,
                KeyCode.ScrollLock => Key.ScrollLock,
                KeyCode.RightShift => Key.RightShift,
                KeyCode.LeftShift => Key.LeftShift,
                KeyCode.RightControl => Key.RightCtrl,
                KeyCode.LeftControl => Key.LeftCtrl,
                KeyCode.RightAlt => Key.RightAlt,
                KeyCode.LeftAlt => Key.LeftAlt,
                KeyCode.LeftCommand => Key.LeftCommand,
                // case KeyCode.LeftApple: (same as LeftCommand)
                KeyCode.LeftWindows => Key.LeftWindows,
                KeyCode.RightCommand => Key.RightCommand,
                // case KeyCode.RightApple: (same as RightCommand)
                KeyCode.RightWindows => Key.RightWindows,
                KeyCode.AltGr => Key.AltGr,
                KeyCode.Help => unknownKey,// Conversion unknown.
                KeyCode.Print => Key.PrintScreen,
                KeyCode.SysReq => unknownKey,// Conversion unknown.
                KeyCode.Break => unknownKey,// Conversion unknown.
                KeyCode.Menu => Key.ContextMenu,
                KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2 or KeyCode.Mouse3 or KeyCode.Mouse4 or KeyCode.Mouse5 or KeyCode.Mouse6 => mouseKey,// Not supported anymore.
                                                                                                                                                       // All other keys are joystick keys which do not
                                                                                                                                                       // exist anymore in the new input system.
                _ => joystickKey,// Not supported anymore.
            };
            return convertToLayout ? ConvertLayout(usLayoutKey) : usLayoutKey;
        }

#endif
    }
}
