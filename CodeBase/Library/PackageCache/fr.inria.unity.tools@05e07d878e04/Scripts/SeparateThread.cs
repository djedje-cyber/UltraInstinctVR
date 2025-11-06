using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace InriaTools
{
    /// <summary>
    /// Executes actions in a different thread than the Unity thread.
    /// We use a singleton to avoid having too many threads to be launched. We use a ThreadPool here.
    /// <example>
    /// <code>
    /// SeparateThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
    /// </code>
    /// </example>
    /// </summary>
    public class SeparateThread : UnitySingleton<SeparateThread>
    {
        #region Classes

        /// <summary>
        /// Keeps track on each launched thread
        /// </summary>
        private class ThreadStatus
        {
            #region Fields

            public bool IsExecuted = false;
            public bool HasFailed = false;
            public Action Action = null;
            public Action Callback = null;
            public Action FailingCallback = null;
            public bool ToBeCleaned = true;
            public StackTrace CallingStackTrace = null;

            #endregion
        }

        private class TaskStatus
        {
            #region Fields

            public Task Task;
            public Action Callback = null;
            public Action FailingCallback = null;
            public bool ToBeCleaned = false;

            #endregion
        }

        #endregion

        #region Fields

        public const string SHOW_FULL_STACK_PREF_KEY = "Tools.ShowFullThreadStackTrace";
        private static bool FullStackTraceException = false;

        private static Thread unityThread;

        #endregion

        #region Properties

        /// <summary>
        /// Checks if the code calling this property is in Unity's thread
        /// </summary>
        public static bool IsInUnityThread => unityThread == Thread.CurrentThread;

        #endregion



        #region Fields

        /// <summary>
        /// Keep track of running threads
        /// </summary>
        private readonly List<ThreadStatus> threads = new();

        /// <summary>
        /// Store done threads to be able to reuse them later
        /// </summary>
        private readonly ConcurrentQueue<ThreadStatus> doneThreads = new();

        /// <summary>
        /// Keep track of running tasks
        /// </summary>
        private readonly List<TaskStatus> tasks = new();

        /// <summary>
        /// Store done tasks to be able to reuse them later
        /// </summary>
        private readonly ConcurrentQueue<TaskStatus> doneTasks = new();

        #endregion

        #region Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            // save the unity thread info
            unityThread = Thread.CurrentThread;
        }

        /// <summary>
        /// Check if the code is in Unity's thread
        /// </summary>
        /// <returns>True if the call to this method was made from Unity's thread</returns>
        [Obsolete("Use " + nameof(IsInUnityThread) + " instead")]
        public bool InUnityThread()
        {
            return IsInUnityThread;
        }

        /// <summary>
        /// Executes an action in a separate thread and fires a callback once the action has been executed.
        /// </summary>
        /// <example>
        /// <code>
        /// ExecuteInThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
        /// </code>
        /// </example>
        /// <param name="action">The action to execute in a thread</param>
        /// <param name="callback">The callback which is fired in Unity's thread when the action is done</param>
        /// <param name="failingCallback">The callback which is fired in Unity's thread if the thread fails</param>
        public void ExecuteInThread(Action action, Action callback = null, Action failingCallback = null)
        {
            // threadStatus synchronizes/joins the separate thread and the Unity thread.
            _ = doneThreads.TryDequeue(out ThreadStatus threadStatus);
            threadStatus ??= new ThreadStatus();

            threadStatus.Action = action;
            threadStatus.Callback = callback;
            threadStatus.FailingCallback = failingCallback;
            threadStatus.CallingStackTrace = FullStackTraceException ? new System.Diagnostics.StackTrace(true) : null;
            threadStatus.HasFailed = false;
            threadStatus.IsExecuted = false;
            threadStatus.ToBeCleaned = false;
            // Starts the separate thread.
            _ = ThreadPool.QueueUserWorkItem(ThreadProc, threadStatus);

            threads.Add(threadStatus);
        }

        /// <summary>
        /// Executes and action in a task and fires a callbacl once the action has been executed
        /// </summary>
        /// <param name="action">The action to execute in a thread</param>
        /// <param name="callback">The callback which is fired in Unity's thread when the action is done</param>
        /// <param name="failingCallback">The callback which is fired in Unity's thread if the thread fails</param>
        /// <param name="longTask">Indicates that the task is expected to be long so that the system can adapt its management</param>
        public void ExecuteInTask(Action action, Action callback = null, Action failingCallback = null, bool longTask = false)
        {
            _ = doneTasks.TryDequeue(out TaskStatus newTask);
            newTask ??= new TaskStatus();
            newTask.Task = RunTask(action, longTask ? TaskCreationOptions.LongRunning : TaskCreationOptions.None, FullStackTraceException ? new System.Diagnostics.StackTrace(true) : null);
            newTask.Callback = callback;
            newTask.FailingCallback = failingCallback;
            newTask.ToBeCleaned = false;
            tasks.Add(newTask);
        }

        /// <summary>
        /// The thread procedure performs the actions and updates the threads status
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void ThreadProc(object stateInfo)
        {
            ThreadStatus threadStatus = (ThreadStatus)stateInfo;

            try
            {
                threadStatus.Action();
            }
            catch (Exception e)
            {
                threadStatus.HasFailed = true;
                if (FullStackTraceException)
                    UnityEngine.Debug.LogError("Exception in thread  " + e + "\n" + threadStatus.CallingStackTrace);
                else
                    UnityEngine.Debug.LogError("Exception in thread (Enable Full Thread Stack Trace option to see the calling stack) : " + e);
            }
            finally
            {
                threadStatus.IsExecuted = true;
            }
        }

        private async Task RunTask(Action action, TaskCreationOptions options, StackTrace stackTrace)
        {
            try
            {
                await Task.Factory.StartNew(action, options);
            }
            catch (Exception e)
            {
                if (FullStackTraceException)
                    UnityEngine.Debug.LogError("Exception in threaded task : " + e + "\n" + stackTrace);
                else
                    UnityEngine.Debug.LogError("Exception in threaded task (Enable Full Thread Stack Trace option to see the calling stack) : " + e);
                throw;
            }
        }

        private void Awake()
        {
#if DEVELOPMENT_BUILD
            FullStackTraceException = true;
#else
            FullStackTraceException = false;
#endif
#if UNITY_EDITOR
            FullStackTraceException = EditorPrefs.GetBool(SHOW_FULL_STACK_PREF_KEY);
#endif

            // force the creation of UnityThreadExecute if needed
            UnityThreadExecute unityThreadExecute = UnityThreadExecute.Instance;
            // Waits for the separate thread to be executed from the Unity thread.
            StartCoroutine(WaitForThreadExecution());
        }

        /// <summary>
        /// Main loop checking on the threads
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForThreadExecution()
        {
            while (true)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    ThreadStatus thread = threads[i];
                    if (thread.IsExecuted && !thread.ToBeCleaned)
                    {
                        if (thread.HasFailed)
                        {
                            thread.FailingCallback?.Invoke();
                        }
                        else // This can start a new thread !
                        {
                            thread.Callback?.Invoke();
                        }

                        thread.ToBeCleaned = true;
                        doneThreads.Enqueue(thread);
                    }
                }
                _ = threads.RemoveAll(handle => handle.ToBeCleaned);
                for (int i = 0; i < tasks.Count; i++)
                {
                    TaskStatus task = tasks[i];
                    if (task.Task.IsCompleted && !task.Task.IsFaulted && !task.ToBeCleaned)
                    {
                        task.Callback?.Invoke();

                        task.ToBeCleaned = true;
                        doneTasks.Enqueue(task);
                    }
                    if (task.Task.IsFaulted)
                    {
                        task.FailingCallback?.Invoke();

                        task.ToBeCleaned = true;
                        doneTasks.Enqueue(task);
                    }
                }
                _ = tasks.RemoveAll(task => task.ToBeCleaned);
                yield return null;
            }
        }

        #endregion
    }
}
