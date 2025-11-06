#if MIDDLEVR
#if MIDDLEVR2
using MiddleVR;

#endif
namespace Xareus.Scenarios.Input.MiddleVR
{
    /// <summary>
    /// Adapter class that supports MiddleVR 1 API for MiddleVR 2. Used when VRTools is not included
    /// </summary>
    public static class Mvr2Adapter
    {
#region Properties

#if MIDDLEVR2
        /// <summary>
        /// Main components
        /// </summary>
        public static vrClusterManager VRClusterMgr => MVR.ClusterMgr;
        public static vrDeviceManager VRDeviceMgr => MVR.DeviceMgr;
        public static vrDisplayManager VRDisplayMgr => MVR.DisplayMgr;
        public static vrKernel VRKernel => MVR.Kernel;

        /// <summary>
        /// Key codes
        /// </summary>
        public static uint VRK_Q => MVR.VRK_Q;
        public static uint VRK_B => MVR.VRK_B;
        public static uint VRK_C => MVR.VRK_C;
        public static uint VRK_D => MVR.VRK_D;
        public static uint VRK_E => MVR.VRK_E;
        public static uint VRK_F => MVR.VRK_F;
        public static uint VRK_G => MVR.VRK_G;
        public static uint VRK_H => MVR.VRK_H;
        public static uint VRK_I => MVR.VRK_I;
        public static uint VRK_J => MVR.VRK_J;
        public static uint VRK_K => MVR.VRK_K;
        public static uint VRK_L => MVR.VRK_L;
#if MIDDLEVR2_1
        public static uint VRK_SEMICOLON => MVR.VRK_OEM_1;
#else
    public static uint VRK_SEMICOLON => MVR.VRK_SEMICOLON;
#endif
        public static uint VRK_N => MVR.VRK_N;
        public static uint VRK_O => MVR.VRK_O;
        public static uint VRK_P => MVR.VRK_P;
        public static uint VRK_A => MVR.VRK_A;
        public static uint VRK_R => MVR.VRK_R;
        public static uint VRK_S => MVR.VRK_S;
        public static uint VRK_T => MVR.VRK_T;
        public static uint VRK_U => MVR.VRK_U;
        public static uint VRK_V => MVR.VRK_V;
        public static uint VRK_Z => MVR.VRK_Z;
        public static uint VRK_X => MVR.VRK_X;
        public static uint VRK_Y => MVR.VRK_Y;
        public static uint VRK_W => MVR.VRK_W;
        public static uint VRK_1 => MVR.VRK_1;
        public static uint VRK_2 => MVR.VRK_2;
        public static uint VRK_3 => MVR.VRK_3;
        public static uint VRK_4 => MVR.VRK_4;
        public static uint VRK_5 => MVR.VRK_5;
        public static uint VRK_6 => MVR.VRK_6;
        public static uint VRK_7 => MVR.VRK_7;
        public static uint VRK_8 => MVR.VRK_8;
        public static uint VRK_9 => MVR.VRK_9;
        public static uint VRK_0 => MVR.VRK_0;
        public static uint VRK_SPACE => MVR.VRK_SPACE;
        public static uint VRK_UP => MVR.VRK_UP;
        public static uint VRK_DOWN => MVR.VRK_DOWN;
        public static uint VRK_LEFT => MVR.VRK_LEFT;
        public static uint VRK_RIGHT => MVR.VRK_RIGHT;
        public static uint VRK_NUMPAD0 => MVR.VRK_NUMPAD0;
        public static uint VRK_NUMPAD1 => MVR.VRK_NUMPAD1;
        public static uint VRK_NUMPAD2 => MVR.VRK_NUMPAD2;
        public static uint VRK_NUMPAD3 => MVR.VRK_NUMPAD3;
        public static uint VRK_NUMPAD4 => MVR.VRK_NUMPAD4;
        public static uint VRK_NUMPAD5 => MVR.VRK_NUMPAD5;
        public static uint VRK_NUMPAD6 => MVR.VRK_NUMPAD6;
        public static uint VRK_NUMPAD7 => MVR.VRK_NUMPAD7;
        public static uint VRK_NUMPAD8 => MVR.VRK_NUMPAD8;
        public static uint VRK_NUMPAD9 => MVR.VRK_NUMPAD9;
        public static uint VRK_DIVIDE => MVR.VRK_DIVIDE;
        public static uint VRK_MULTIPLY => MVR.VRK_MULTIPLY;
        public static uint VRK_SUBTRACT => MVR.VRK_SUBTRACT;
        public static uint VRK_ADD => MVR.VRK_ADD;
#if MIDDLEVR2_1
#pragma warning disable CS0618 // Disable warning caused by itself
        [Obsolete("Key " + nameof(VRK_NUMPADENTER) + " does not exist anymore")]
#pragma warning restore CS0618
        public static uint VRK_NUMPADENTER
        {
            get
            {
                Debug.LogError("Key " + nameof(VRK_NUMPADENTER) + " does not exist anymore");
                return 0;
            }
        }
#else
    public static uint VRK_NUMPADENTER => MVR.VRK_NUMPADENTER;
#endif
        public static uint VRK_DECIMAL => MVR.VRK_DECIMAL;
        public static uint VRK_INSERT => MVR.VRK_INSERT;
        public static uint VRK_DELETE => MVR.VRK_DELETE;
        public static uint VRK_HOME => MVR.VRK_HOME;
        public static uint VRK_END => MVR.VRK_END;
        public static uint VRK_PRIOR => MVR.VRK_PRIOR;
        public static uint VRK_NEXT => MVR.VRK_NEXT;
        public static uint VRK_ESCAPE => MVR.VRK_ESCAPE;
        public static uint VRK_LCONTROL => MVR.VRK_LCONTROL;
        public static uint VRK_RCONTROL => MVR.VRK_RCONTROL;
        public static uint VRK_ALTLEFT => MVR.VRK_ALTLEFT;
        public static uint VRK_ALTRIGHT => MVR.VRK_ALTRIGHT;
        public static uint VRK_LSHIFT => MVR.VRK_LSHIFT;
        public static uint VRK_RSHIFT => MVR.VRK_RSHIFT;
#if MIDDLEVR2_1
#pragma warning disable CS0618 // obsolete warning
        [Obsolete("Key " + nameof(VRK_OEM_102) + " does not exist anymore")]
#pragma warning restore CS0618
        public static uint VRK_OEM_102
        {
            get
            {
                Debug.LogError("Key " + nameof(VRK_OEM_102) + " does not exist anymore");
                return 0;
            }
        }
#else
    public static uint VRK_OEM_102 => MVR.VRK_OEM_102;
#endif
        public static uint VRK_M => MVR.VRK_M;
#if MIDDLEVR2_1
        public static uint VRK_COMMA => MVR.VRK_OEM_COMMA;
#else
    public static uint VRK_COMMA => MVR.VRK_COMMA;
#endif

#if MIDDLEVR2_1
        public static uint VRK_PERIOD => MVR.VRK_OEM_PERIOD;
#else
    public static uint VRK_PERIOD => MVR.VRK_PERIOD;
#endif

#if MIDDLEVR2_1
        public static uint VRK_SLASH => MVR.VRK_OEM_2;
#else
    public static uint VRK_SLASH => MVR.VRK_SLASH;
#endif
        public static uint VRK_RETURN => MVR.VRK_RETURN;
#if MIDDLEVR2_1
        public static uint VRK_EQUALS => MVR.VRK_OEM_NEC_EQUAL;
#else
    public static uint VRK_EQUALS => MVR.VRK_EQUALS;
#endif
        public static uint VRK_BACK => MVR.VRK_BACK;
#if MIDDLEVR2_1
        public static uint VRK_MINUS => MVR.VRK_OEM_MINUS;
#else
    public static uint VRK_MINUS => MVR.VRK_MINUS;
#endif
#if MIDDLEVR2_1
        public static uint VRK_PLUS => MVR.VRK_OEM_PLUS;
#endif
        public static uint VRK_TAB => MVR.VRK_TAB;
        public static uint VRK_F1 => MVR.VRK_F1;
        public static uint VRK_F2 => MVR.VRK_F2;
        public static uint VRK_F3 => MVR.VRK_F3;
        public static uint VRK_F4 => MVR.VRK_F4;
        public static uint VRK_F5 => MVR.VRK_F5;
        public static uint VRK_F6 => MVR.VRK_F6;
        public static uint VRK_F7 => MVR.VRK_F7;
        public static uint VRK_F8 => MVR.VRK_F8;
        public static uint VRK_F9 => MVR.VRK_F9;
        public static uint VRK_F10 => MVR.VRK_F10;
        public static uint VRK_F11 => MVR.VRK_F11;
        public static uint VRK_F12 => MVR.VRK_F12;
        public static uint VRK_SCROLL => MVR.VRK_SCROLL;
        public static uint VRK_PAUSE => MVR.VRK_PAUSE;

#else
        /// <summary>
        /// Main components
        /// </summary>
        public static vrClusterManager VRClusterMgr => MiddleVR.VRClusterMgr;
        public static vrDeviceManager VRDeviceMgr => MiddleVR.VRDeviceMgr;
        public static vrDisplayManager VRDisplayMgr => MiddleVR.VRDisplayMgr;
        public static vrKernel VRKernel => MiddleVR.VRKernel;

        /// <summary>
        /// Key codes
        /// </summary>
        public static uint VRK_Q = MiddleVR.VRK_Q;
        public static uint VRK_B => MiddleVR.VRK_B;
        public static uint VRK_C => MiddleVR.VRK_C;
        public static uint VRK_D => MiddleVR.VRK_D;
        public static uint VRK_E => MiddleVR.VRK_E;
        public static uint VRK_F => MiddleVR.VRK_F;
        public static uint VRK_G => MiddleVR.VRK_G;
        public static uint VRK_H => MiddleVR.VRK_H;
        public static uint VRK_I => MiddleVR.VRK_I;
        public static uint VRK_J => MiddleVR.VRK_J;
        public static uint VRK_K => MiddleVR.VRK_K;
        public static uint VRK_L => MiddleVR.VRK_L;
        public static uint VRK_SEMICOLON => MiddleVR.VRK_SEMICOLON;
        public static uint VRK_N => MiddleVR.VRK_N;
        public static uint VRK_O => MiddleVR.VRK_O;
        public static uint VRK_P => MiddleVR.VRK_P;
        public static uint VRK_A => MiddleVR.VRK_A;
        public static uint VRK_R => MiddleVR.VRK_R;
        public static uint VRK_S => MiddleVR.VRK_S;
        public static uint VRK_T => MiddleVR.VRK_T;
        public static uint VRK_U => MiddleVR.VRK_U;
        public static uint VRK_V => MiddleVR.VRK_V;
        public static uint VRK_Z => MiddleVR.VRK_Z;
        public static uint VRK_X => MiddleVR.VRK_X;
        public static uint VRK_Y => MiddleVR.VRK_Y;
        public static uint VRK_W => MiddleVR.VRK_W;
        public static uint VRK_1 => MiddleVR.VRK_1;
        public static uint VRK_2 => MiddleVR.VRK_2;
        public static uint VRK_3 => MiddleVR.VRK_3;
        public static uint VRK_4 => MiddleVR.VRK_4;
        public static uint VRK_5 => MiddleVR.VRK_5;
        public static uint VRK_6 => MiddleVR.VRK_6;
        public static uint VRK_7 => MiddleVR.VRK_7;
        public static uint VRK_8 => MiddleVR.VRK_8;
        public static uint VRK_9 => MiddleVR.VRK_9;
        public static uint VRK_0 => MiddleVR.VRK_0;
        public static uint VRK_SPACE => MiddleVR.VRK_SPACE;
        public static uint VRK_UP => MiddleVR.VRK_UP;
        public static uint VRK_DOWN => MiddleVR.VRK_DOWN;
        public static uint VRK_LEFT => MiddleVR.VRK_LEFT;
        public static uint VRK_RIGHT => MiddleVR.VRK_RIGHT;
        public static uint VRK_NUMPAD0 => MiddleVR.VRK_NUMPAD0;
        public static uint VRK_NUMPAD1 => MiddleVR.VRK_NUMPAD1;
        public static uint VRK_NUMPAD2 => MiddleVR.VRK_NUMPAD2;
        public static uint VRK_NUMPAD3 => MiddleVR.VRK_NUMPAD3;
        public static uint VRK_NUMPAD4 => MiddleVR.VRK_NUMPAD4;
        public static uint VRK_NUMPAD5 => MiddleVR.VRK_NUMPAD5;
        public static uint VRK_NUMPAD6 => MiddleVR.VRK_NUMPAD6;
        public static uint VRK_NUMPAD7 => MiddleVR.VRK_NUMPAD7;
        public static uint VRK_NUMPAD8 => MiddleVR.VRK_NUMPAD8;
        public static uint VRK_NUMPAD9 => MiddleVR.VRK_NUMPAD9;
        public static uint VRK_DIVIDE => MiddleVR.VRK_DIVIDE;
        public static uint VRK_MULTIPLY => MiddleVR.VRK_MULTIPLY;
        public static uint VRK_SUBTRACT => MiddleVR.VRK_SUBTRACT;
        public static uint VRK_ADD => MiddleVR.VRK_ADD;
        public static uint VRK_NUMPADENTER => MiddleVR.VRK_NUMPADENTER;
        public static uint VRK_DECIMAL => MiddleVR.VRK_DECIMAL;
        public static uint VRK_INSERT => MiddleVR.VRK_INSERT;
        public static uint VRK_DELETE => MiddleVR.VRK_DELETE;
        public static uint VRK_HOME => MiddleVR.VRK_HOME;
        public static uint VRK_END => MiddleVR.VRK_END;
        public static uint VRK_PRIOR => MiddleVR.VRK_PRIOR;
        public static uint VRK_NEXT => MiddleVR.VRK_NEXT;
        public static uint VRK_ESCAPE => MiddleVR.VRK_ESCAPE;
        public static uint VRK_LCONTROL => MiddleVR.VRK_LCONTROL;
        public static uint VRK_RCONTROL => MiddleVR.VRK_RCONTROL;
        public static uint VRK_ALTLEFT => MiddleVR.VRK_ALTLEFT;
        public static uint VRK_ALTRIGHT => MiddleVR.VRK_ALTRIGHT;
        public static uint VRK_LSHIFT => MiddleVR.VRK_LSHIFT;
        public static uint VRK_RSHIFT => MiddleVR.VRK_RSHIFT;
        public static uint VRK_OEM_102 => MiddleVR.VRK_OEM_102;
        public static uint VRK_M => MiddleVR.VRK_M;
        public static uint VRK_COMMA => MiddleVR.VRK_COMMA;
        public static uint VRK_PERIOD => MiddleVR.VRK_PERIOD;
        public static uint VRK_SLASH => MiddleVR.VRK_SLASH;
        public static uint VRK_RETURN => MiddleVR.VRK_RETURN;
        public static uint VRK_EQUALS => MiddleVR.VRK_EQUALS;
        public static uint VRK_BACK => MiddleVR.VRK_BACK;
        public static uint VRK_MINUS => MiddleVR.VRK_MINUS;
        public static uint VRK_TAB => MiddleVR.VRK_TAB;
        public static uint VRK_F1 => MiddleVR.VRK_F1;
        public static uint VRK_F2 => MiddleVR.VRK_F2;
        public static uint VRK_F3 => MiddleVR.VRK_F3;
        public static uint VRK_F4 => MiddleVR.VRK_F4;
        public static uint VRK_F5 => MiddleVR.VRK_F5;
        public static uint VRK_F6 => MiddleVR.VRK_F6;
        public static uint VRK_F7 => MiddleVR.VRK_F7;
        public static uint VRK_F8 => MiddleVR.VRK_F8;
        public static uint VRK_F9 => MiddleVR.VRK_F9;
        public static uint VRK_F10 => MiddleVR.VRK_F10;
        public static uint VRK_F11 => MiddleVR.VRK_F11;
        public static uint VRK_F12 => MiddleVR.VRK_F12;
        public static uint VRK_SCROLL => MiddleVR.VRK_SCROLL;
        public static uint VRK_PAUSE => MiddleVR.VRK_PAUSE;
#endif

#endregion
    }
}
#endif