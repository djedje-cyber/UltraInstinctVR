using System;
using System.Threading;

using UnityEngine;

namespace InriaTools.Samples
{
    public class ThreadTest : MonoBehaviour
    {
        #region Fields

        public int ExecuteEveryXFrame = 20;

        public bool TestExceptionInThread = true;

        public bool UseShortThread = true;
        public bool UseLongThread = true;
        public bool UseVeryLongThread = true;

        public bool UseAnyUpdateForVeryLongThread = true;
        public bool UseUpdateForVeryLongThread = false;
        public bool UseLateUpdateForVeryLongThread = false;
        public bool UseFixedUpdateForVeryLongThread = false;

        public bool RegisterRegisterThread = true;

        public bool TestExceptionInTask = true;
        public bool UseShortTask = true;
        public bool UseLongTask = true;
        public bool UseVeryLongTask = true;

        private static float startingAngle = 0f;
        private bool threadRegistered = false;
        private int threadId = 0;

        #endregion

        #region Methods

        private static Action CrashingMethod()
        {
            return () =>
            {
                // wait for 1.1sec
                Thread.Sleep(1100);
                DateTime time = DateTime.Now;

                UnityThreadExecute.InvokeNextUpdate(() => Debug.Log("Thread/Task should crash"));

                throw new Exception("Crashing Thread/Task. that's Normal !");
            };
        }

        private void OnEnable()
        {
            if (RegisterRegisterThread)
                RegisterThread();
            WorkingThread();
            CrashingThread();
            WorkingTask();
            CrashingTask();
        }

        private void Awake()
        {
            WorkingThread();
            CrashingThread();
            CrashingTask();
        }

        private void Start()
        {
            WorkingThread();
            CrashingThread();
            WorkingTask();
            CrashingTask();
        }

        // may not work
        private void OnDisable()
        {
            WorkingThread();
            CrashingThread();
            WorkingTask();
            CrashingTask();
        }

        // Update is called once per frame
        private void Update()
        {
            if (RegisterRegisterThread && !threadRegistered)
                RegisterThread();
            if (!RegisterRegisterThread && threadRegistered)
                UnRegisterThread();
            if (Time.frameCount % ExecuteEveryXFrame == 0)
            {
                WorkingThread();
                CrashingThread();
                WorkingTask();
                CrashingTask();
            }
        }

        private void CrashingThread()
        {
            if (!TestExceptionInThread)
                return;
            SeparateThread.Instance.ExecuteInThread(CrashingMethod(),
                 () =>
                 {
                     Debug.LogError("Thread executed but should not !!");
                     // no error
                     _ = GameObject.Find("ThreadTest");
                 },
                 () =>
                 {
                     Debug.LogWarning("Thread Failed as expected");
                 }
                 );
        }

        private void CrashingTask()
        {
            if (!TestExceptionInTask)
                return;
            SeparateThread.Instance.ExecuteInTask(CrashingMethod(),
                 () =>
                 {
                     Debug.LogError("Task executed but should not !!");
                     // no error
                     _ = GameObject.Find("ThreadTest");
                 },
                 () =>
                 {
                     Debug.LogWarning("Task Failed as expected");
                 }
                 );
        }

        private void RegisterThread()
        {
            threadRegistered = true;
            SeparateThread.Instance.ExecuteInThread(() => RegisterThread(RegisteredThreadAction),
                 () =>
                 {
                     Debug.Log("Registered Thread executed");
                     // no error
                     _ = GameObject.Find("ThreadTest");
                 }
                 );
        }

        private void UnRegisterThread()
        {
            threadRegistered = false;
            SeparateThread.Instance.ExecuteInThread(() => UnRegisterThread(RegisteredThreadAction),
                 () =>
                 {
                     Debug.Log("Unregistered Thread executed");
                     // no error
                     _ = GameObject.Find("ThreadTest");
                 }
                 );
        }

        private void WorkingThread()
        {
            if (UseLongThread)
            {
                System.Diagnostics.Stopwatch timer = new();
                timer.Start();
                SeparateThread.Instance.ExecuteInThread(() =>
                {
                    LongMethod(threadId++);
                    timer.Stop();
                },
                     () =>
                     {
                         Debug.Log("Long Thread executed in : " + timer.ElapsedMilliseconds);
                         // no error
                         _ = GameObject.Find("ThreadTest");
                     }
                     );
            }

            if (UseShortThread)
            {
                System.Diagnostics.Stopwatch timer = new();
                timer.Start();
                SeparateThread.Instance.ExecuteInThread(() =>
                {
                    ShortMethod();
                    timer.Stop();
                },
                  () =>
                  {
                      Debug.Log("Short Thread executed in : " + timer.ElapsedMilliseconds);
                      // no error
                      _ = GameObject.Find("ThreadTest");
                  }
                  );
            }

            if (UseVeryLongThread)
            {
                System.Diagnostics.Stopwatch timer = new();
                timer.Start();
                SeparateThread.Instance.ExecuteInThread(() =>
                {
                    VeryLongMethod(threadId++);
                    timer.Stop();
                },
                    () =>
                    {
                        Debug.Log("Very Long Thread executed in : " + timer.ElapsedMilliseconds);
                        // no error
                        _ = GameObject.Find("ThreadTest");
                    }
                    );
            }
        }

        private void WorkingTask()
        {
            if (UseLongTask)
            {
                System.Diagnostics.Stopwatch timer = new();
                timer.Start();
                SeparateThread.Instance.ExecuteInTask(() =>
                {
                    LongMethod(threadId++);
                    timer.Stop();
                },
                     () =>
                     {
                         Debug.Log("Long Task executed in : " + timer.ElapsedMilliseconds);
                         // no error
                         _ = GameObject.Find("ThreadTest");
                     }, null, true
                     );
            }

            if (UseShortTask)
            {
                System.Diagnostics.Stopwatch timer = new();
                timer.Start();
                SeparateThread.Instance.ExecuteInTask(() =>
                {
                    ShortMethod();
                    timer.Stop();
                },
                  () =>
                  {
                      Debug.Log("Short Task executed in : " + timer.ElapsedMilliseconds);
                      // no error
                      _ = GameObject.Find("ThreadTest");
                  }
                  );
            }

            if (UseVeryLongTask)
            {
                System.Diagnostics.Stopwatch timer = new();
                timer.Start();
                SeparateThread.Instance.ExecuteInTask(() =>
                {
                    VeryLongMethod(threadId++);
                    timer.Stop();
                },
                    () =>
                    {
                        Debug.Log("Very Long Task executed in : " + timer.ElapsedMilliseconds);
                        // no error
                        _ = GameObject.Find("ThreadTest");
                    }, null, true
                    );
            }
        }

        private void RegisterThread(Action registeredThreadAction)
        {
            // error should be displayed
            // GameObject.Find("ThreadTest");
            UnityThreadExecute.RegisterActionForExecutionSteps(registeredThreadAction, UnityThreadExecute.UnityExecutionStep.OnRenderObject);
        }

        private void UnRegisterThread(Action registeredThreadAction)
        {
            // error should be displayed
            // GameObject.Find("ThreadTest");
            UnityThreadExecute.UnRegisterActionForExecutionSteps(registeredThreadAction, UnityThreadExecute.UnityExecutionStep.OnRenderObject);
        }

        private void LongMethod(int threadId)
        {
            // wait for 1sec
            Thread.Sleep(1000);
            DateTime time = DateTime.Now;

            Debug.Assert(!SeparateThread.IsInUnityThread);

            // error should be displayed
            // GameObject.Find("ThreadTest");
            // use unity thread for that part
            EventWaitHandle handle = new(false, EventResetMode.AutoReset);
            UnityThreadExecute.InvokeNextUpdate(() =>
            {
                Debug.Assert(SeparateThread.IsInUnityThread);
                Debug.Log("Update " + threadId + " : Time is " + time.Hour + ":" + time.Minute);
                // no error
                _ = GameObject.Find("ThreadTest");
                _ = handle.Set();
            });
            _ = handle.WaitOne();

            // wait for 1sec
            Thread.Sleep(1000);
        }

        private void VeryLongMethod(int threadId)
        {
            // wait for 10sec
            Thread.Sleep(10000);
            DateTime time = DateTime.Now;

            UnityThreadExecute.UnityExecutionStep stepFlag = UnityThreadExecute.UnityExecutionStep.None;
            if (UseUpdateForVeryLongThread)
                stepFlag |= UnityThreadExecute.UnityExecutionStep.Update;
            if (UseLateUpdateForVeryLongThread)
                stepFlag |= UnityThreadExecute.UnityExecutionStep.LateUpdate;
            if (UseFixedUpdateForVeryLongThread)
                stepFlag |= UnityThreadExecute.UnityExecutionStep.FixedUpdate;
            if (UseAnyUpdateForVeryLongThread)
                stepFlag |= UnityThreadExecute.UnityExecutionStep.Any;

            // error should be displayed
            // GameObject.Find("ThreadTest");
            // use unity thread for that part
            EventWaitHandle handle = new(false, EventResetMode.AutoReset);
            UnityThreadExecute.InvokeActionForNextExecutionSteps(() =>
            {
                Debug.Log("Any " + threadId + ": Time is " + time.Hour + ":" + time.Minute);
                // no error
                _ = GameObject.Find("ThreadTest");

                _ = handle.Set();
            }, stepFlag);
            _ = handle.WaitOne();

            // wait for 10sec
            Thread.Sleep(10000);
        }

        private void ShortMethod()
        {
            DateTime time = DateTime.Now;

            Debug.Log("Time is " + time.Hour + ":" + time.Minute);
        }

        private void RegisteredThreadAction()
        {
            DateTime time = DateTime.Now;
            Debug.Log("Update registered : Time is " + time.Hour + ":" + time.Minute);
            // no error
            _ = GameObject.Find("ThreadTest");
            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            //GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);
            for (int i = 0; i < 5; ++i)
            {
                float a = i / (float)5;
                float angle = (a * Mathf.PI * 2) + startingAngle;
                startingAngle++;
                // Vertex colors change from red to green
                GL.Color(new Color(a, 1 - a, 0, 0.8F));
                // One vertex at transform position
                GL.Vertex3(0, 0, 0);
                // Another vertex at edge of circle
                GL.Vertex3(Mathf.Cos(angle) * 5, Mathf.Sin(angle) * 5, 0);
            }
            GL.End();
            GL.PopMatrix();
        }

        #endregion
    }
}
