using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEngine;

namespace InriaTools
{
    /// <summary>
    /// Place this object on any game object to deffer code execution from other threads to unity's thread
    /// <example>
    /// <code>
    /// var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
    /// UnityThreadExecute.InvokeNextUpdate(() =>
    /// {
    ///     *Unity dependent code *
    ///     handle.Set();
    /// });
    /// handle.WaitOne();
    /// </code>
    /// the handle variable ensures that the thread waits for the code to be executed in unity's thread
    /// </example>
    /// </summary>
    public class UnityThreadExecute : UnitySingleton<UnityThreadExecute>
    {
        #region Classes

        protected class ActionEntry
        {
            #region Properties

            public Action Action { get; set; }
            public UnityExecutionStep ExecutionSteps { get; set; }
            public bool InvokeOnce { get; set; }
            public bool Invoked { get; set; }

            #endregion
        }

        #endregion

        #region Enums

        [Flags]
        public enum UnityExecutionStep
        {
            None = 0,
            Update = 1,
            LateUpdate = 2,

            /// <summary>
            /// Update or LateUpdate
            /// </summary>
            GraphicsUpdate = Update | LateUpdate,

            FixedUpdate = 4,
            OnRenderObject = 8,
            OnGUI = 16,

            /// <summary>
            /// Can only be used in the editor
            ///
            EditorUpdate = 32,

            /// <summary>
            /// Any of the execution steps except EditorUpdate
            /// </summary>
            Any = Update | LateUpdate | FixedUpdate | OnRenderObject | OnGUI,

            /// <summary>
            /// Any of the execution steps including EditorUpdate
            /// </summary>
            AnyOrEditor = Any | EditorUpdate
        };

        #endregion

        #region Fields

        private static readonly object _lock = new();
        private static Array checkTypeValues;

        // This object needs to exist from the start otherwise we will attempt to create it through the first threaded call and it will fail
        private static UnityThreadExecute _instance;

        private static List<ActionEntry> allActions = new();

        private static List<ActionEntry> actionsToInvoke_Update = new();

        private static List<ActionEntry> actionsToInvoke_LateUpdate = new();

        private static List<ActionEntry> actionsToInvoke_FixedUpdate = new();

        private static List<ActionEntry> actionsToInvoke_OnRenderObject = new();

        private static List<ActionEntry> actionsToInvoke_OnGui = new();

        private static List<ActionEntry> actionsToInvoke_EditorUpdate = new();

        private static Dictionary<UnityExecutionStep, List<ActionEntry>> actionsToInvoke;

        #endregion

        #region Properties

        /// <summary>
        /// Checks if the code calling this property is in Unity's thread
        /// </summary>
        public static bool IsInUnityThread => SeparateThread.IsInUnityThread;

        private static Array CheckTypeValues
        {
            get
            {
                checkTypeValues ??= Enum.GetValues(typeof(UnityExecutionStep));
                return checkTypeValues;
            }
        }

        #endregion

        #region Constructors

        protected UnityThreadExecute() { }

        #endregion

        #region Methods

        /// <summary>
        /// Deffer code execution from other threads to unity's thread
        /// <example>
        /// <code>
        /// var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
        /// UnityThreadExecute.InvokeNextUpdate(() =>
        /// {
        ///     *Unity dependent code *
        ///     handle.Set();
        /// });
        /// handle.WaitOne();
        /// </code>
        /// the handle variable ensures that the thread waits for the code to be executed in unity's thread
        /// </example>
        /// </summary>
        public static void InvokeNextUpdate(System.Action action)
        {
            InvokeActionForNextExecutionSteps(action, UnityExecutionStep.Update);
        }

        /// <summary>
        /// Invokes the given action at the first execution step that will occur amongst the ones specified. The action is invoked once
        /// This method waits for the action to be executed in unity's thread before returning and will return immediately if called from unity's thread
        /// </summary>
        /// <param name="action">The action that must be performed</param>
        /// <param name="executionSteps">The execution steps in which the action can be performed (The first one happening will be used)</param>
        /// <param name="timeout">Set a timeout before cancelling the action</param>
        public static void InvokeActionForNextExecutionStepsAndWait(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any, int timeout = 5000)
        {
            if (!IsInUnityThread)
            {
                EventWaitHandle handle = new(false, EventResetMode.AutoReset);

                UnityThreadExecute.InvokeActionForNextExecutionSteps(() =>
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        handle.Set();
                    }
                }, executionSteps);
                if (!handle.WaitOne(timeout))
                {
                    Debug.LogWarning($"WaitHandle timed out after {timeout}ms, the action in Unity's thread may not have completed correctly. This is expected to happen when stopping playmode but should not in any other usecase.");
                }
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Invokes the given action at the first execution step that will occur, either in Unity Editor or Unity engine thread . The action is invoked once
        /// The action will be executed immediately if called from unity's thread
        /// </summary>
        /// <param name="action"></param>
        public static void InvokeActionInUnityThread(System.Action action)
        {
            if (!IsInUnityThread)
            {
                UnityThreadExecute.InvokeActionForNextExecutionSteps(() =>
                {
                    action();
                }, UnityExecutionStep.AnyOrEditor);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Invokes the given action at the first execution step that will occur amongst the ones specified. The action is invoked once
        /// </summary>
        /// <param name="action"></param>
        /// <param name="executionSteps"></param>
        public static void InvokeActionForNextExecutionSteps(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            ActionEntry newAction = new()
            {
                Action = action,
                ExecutionSteps = executionSteps,
                InvokeOnce = true
            };

            lock (_lock)
            {
                if (!allActions.Contains(newAction))
                    allActions.Add(newAction);
                foreach (UnityExecutionStep value in CheckTypeValues)
                {
                    if (actionsToInvoke.ContainsKey(value) && (executionSteps & value) == value && !actionsToInvoke[value].Contains(newAction))
                        actionsToInvoke[value].Add(newAction);
                }
            }
        }

        /// <summary>
        /// Invokes the given action at EVERY execution step specified. The action is invoked while it stays registered
        /// </summary>
        /// <param name="action"></param>
        /// <param name="executionSteps"></param>
        public static void RegisterActionForExecutionSteps(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            ActionEntry newAction = new()
            {
                Action = action,
                ExecutionSteps = executionSteps,
                InvokeOnce = false
            };

            lock (_lock)
            {
                if (!allActions.Contains(newAction))
                    allActions.Add(newAction);
                foreach (UnityExecutionStep value in CheckTypeValues)
                {
                    if (actionsToInvoke.ContainsKey(value) && (executionSteps & value) == value && !actionsToInvoke[value].Contains(newAction))
                        actionsToInvoke[value].Add(newAction);
                }
            }
        }

        public static void UnRegisterActionForExecutionSteps(System.Action action, UnityExecutionStep executionSteps = UnityExecutionStep.Any)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            UnityThreadExecute h = _instance;

            lock (_lock)
            {
                ActionEntry actionToChange = allActions.FirstOrDefault(actionEntry => actionEntry.Action.Equals(action) && !actionEntry.InvokeOnce);
                if (actionToChange != null)
                {
                    UnityExecutionStep newFlags = actionToChange.ExecutionSteps & ~executionSteps;
                    // remove from old steps
                    foreach (UnityExecutionStep value in CheckTypeValues)
                    {
                        if (actionsToInvoke.ContainsKey(value) && (executionSteps & value) == value && actionsToInvoke[value].Contains(actionToChange))
                            _ = actionsToInvoke[value].Remove(actionToChange);
                    }
                    // remove from allActions if resulting flag is none
                    if (newFlags == UnityExecutionStep.None && allActions.Contains(actionToChange))
                        _ = allActions.Remove(actionToChange);
                }
            }
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        public static void RegisterToEditorUpdate()
        {
            UnityEditor.EditorApplication.update += OnEditorUpdate;
        }

#endif

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
#else

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void CreateActionsQueues()
        {
            actionsToInvoke = new Dictionary<UnityExecutionStep, List<ActionEntry>>
            {
                { UnityExecutionStep.Update, actionsToInvoke_Update},
                { UnityExecutionStep.LateUpdate, actionsToInvoke_LateUpdate},
                { UnityExecutionStep.FixedUpdate, actionsToInvoke_FixedUpdate},
                { UnityExecutionStep.OnRenderObject, actionsToInvoke_OnRenderObject},
                { UnityExecutionStep.OnGUI, actionsToInvoke_OnGui},
                { UnityExecutionStep.EditorUpdate, actionsToInvoke_EditorUpdate}
            };
        }

        protected static void OnEditorUpdate()
        {
            InvokeActions(UnityExecutionStep.EditorUpdate);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateInstance()
        {
            if (_instance == null)
            {
                _instance = Instance;
            }
        }

        private static void InvokeActions(UnityExecutionStep flag)
        {
            List<ActionEntry> actions = actionsToInvoke[flag];
            lock (_lock)
            {
                // execute and remove from the dictionaries if needed. We iterate forward to ensure that actions are executed in the order they where registered
                for (int i = 0; i < actions.Count; i++)
                {
                    ActionEntry actionEntry = actions[i];
                    if (!actionEntry.InvokeOnce || !actionEntry.Invoked)
                    {
                        actionEntry.Action();
                        actionEntry.Invoked = true;
                    }
                    else // if already invoked, remove it
                    {
                        actions.RemoveAt(i);
                        if (allActions.Contains(actionEntry))
                            _ = allActions.Remove(actionEntry);
                    }
                }
            }
        }

        private void Awake()
        {
            _instance = Instance;
        }

        private void Update()
        {
            InvokeActions(UnityExecutionStep.Update);
        }

        private void LateUpdate()
        {
            InvokeActions(UnityExecutionStep.LateUpdate);
        }

        private void FixedUpdate()
        {
            InvokeActions(UnityExecutionStep.FixedUpdate);
        }

        private void OnRenderObject()
        {
            InvokeActions(UnityExecutionStep.OnRenderObject);
        }

        private void OnGUI()
        {
            InvokeActions(UnityExecutionStep.OnGUI);
        }

        #endregion
    }
}
