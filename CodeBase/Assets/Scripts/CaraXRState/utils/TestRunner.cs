using System;
using System.Collections;

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Xareus.Scenarios.Unity;

public class TestSuiteRunner : MonoBehaviour
{
    [Header("Test Suite Configuration")]
    [SerializeField] protected ExecutionMode executionMode = ExecutionMode.Sequential;
    [SerializeField] protected float pollInterval = 0.1f;
    [SerializeField] protected float delayBetween = 0.5f;
    [SerializeField] protected bool autoDiscover = false;
    [SerializeField] protected bool runOnStart = true;

    [Header("Manual Test List (if autoDiscover = false)")]
    [SerializeField] protected List<UnityEngine.Object> testClasses = new List<UnityEngine.Object>();

    // ← bindings removed entirely

    private List<TestResult> results = new List<TestResult>();
    private int totalTests = 0;
    private int passed = 0;
    private int failed = 0;
    private bool isRunning = false;

    public bool IsRunning => isRunning;

    public enum ExecutionMode { Sequential, Parallel }

    private void Start()
    {
        if (runOnStart)
            StartSuite();
    }

    public void StartSuite()
    {
        if (isRunning)
        {
            Debug.LogWarning("[TestSuite] Already running.");
            return;
        }

        List<Type> testTypes = autoDiscover
            ? DiscoverAllTestClasses()
            : ResolveTestClasses(testClasses);

        if (testTypes.Count == 0)
        {
            Debug.LogWarning("[TestSuite] No test classes found.");
            return;
        }

        totalTests = testTypes.Count;
        passed = 0;
        failed = 0;
        results.Clear();

        Debug.Log($"[TestSuite] ▶ Starting {totalTests} test(s) in {executionMode} mode.");

        if (executionMode == ExecutionMode.Sequential)
            StartCoroutine(RunSequential(testTypes));
        else
            StartCoroutine(RunParallel(testTypes));
    }

    public void StopSuite()
    {
        isRunning = false;
        StopAllCoroutines();
        Debug.Log("[TestSuite] Stopped.");
    }

    private IEnumerator RunSequential(List<Type> testTypes)
    {
        isRunning = true;

        foreach (Type testType in testTypes)
        {
            Debug.Log($"[TestSuite] ▶ [{testType.Name}] Starting...");

            TestResult result = new TestResult(testType.Name);
            yield return StartCoroutine(RunTest(testType, result));

            results.Add(result);
            if (result.Success) passed++;
            else failed++;

            LogResult(result);
            yield return new WaitForSeconds(delayBetween);
        }

        isRunning = false;
        LogSummary();
    }

    private IEnumerator RunParallel(List<Type> testTypes)
    {
        isRunning = true;

        List<TestResult> parallelResults = new List<TestResult>();
        List<IEnumerator> parallelRoutines = new List<IEnumerator>();

        foreach (Type testType in testTypes)
        {
            TestResult result = new TestResult(testType.Name);
            parallelResults.Add(result);
            parallelRoutines.Add(RunTest(testType, result));
        }

        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (IEnumerator routine in parallelRoutines)
            coroutines.Add(StartCoroutine(routine));

        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;

        foreach (TestResult result in parallelResults)
        {
            results.Add(result);
            if (result.Success) passed++;
            else failed++;
            LogResult(result);
        }

        isRunning = false;
        LogSummary();
    }

    private IEnumerator RunTest(Type testType, TestResult result)
    {
        object instance = GetOrCreateInstance(testType);
        int currentPlace = 0;

        result.StartTime = Time.time;

        while (true)
        {
            MethodInfo transition = FindEnabledTransition(testType, currentPlace);

            if (transition == null)
            {
                // No more transitions — test completed successfully
                result.Success = true;
                result.Message = $"Completed at Place_{currentPlace}";
                break;
            }

            // -------------------------------------------------------
            // Poll sensor — Expect ignored during polling
            // -------------------------------------------------------
            bool detected = false;
            float timeout = 10f;
            float elapsed = 0f;

            while (!detected && elapsed < timeout)
            {
                ExpectInterceptor.SetIgnore(true);      // ← ignore Expect
                DetectInteractionInterceptor.Reset();
                transition.Invoke(instance, null);
                ExpectInterceptor.SetIgnore(false);

                detected = DetectInteractionInterceptor.LastResult;
                elapsed += pollInterval;

                if (!detected)
                    yield return new WaitForSeconds(pollInterval);
            }

            if (!detected)
            {
                result.Success = false;
                result.Message = $"Timeout on: {transition.Name} at Place_{currentPlace}";
                break;
            }

            // -------------------------------------------------------
            // Fire transition — Expect runs for real
            // -------------------------------------------------------
            try
            {
                ExpectInterceptor.Reset();              // ← enable Expect
                DetectInteractionInterceptor.Reset();
                transition.Invoke(instance, null);      // ← Expect fires here

                if (!ExpectInterceptor.LastResult)
                {
                    result.Success = false;
                    result.Message = $"Expect failed on: {transition.Name} at Place_{currentPlace}";
                    break;
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = $"Exception in {transition.Name}: {e.InnerException?.Message ?? e.Message}";
                break;
            }

            // -------------------------------------------------------
            // Advance place
            // -------------------------------------------------------
            PlaceAttribute nextPlace = transition.GetCustomAttribute<PlaceAttribute>();
            FinalStateAttribute finalAttr = transition.GetCustomAttribute<FinalStateAttribute>();

            if (finalAttr != null)
            {
                // [FinalState] reached — test passed
                result.Success = true;
                result.Message = $"[FinalState] reached at {transition.Name}";
                break;
            }

            if (nextPlace == null)
            {
                result.Success = true;
                result.Message = $"No next place after {transition.Name}. Done.";
                break;
            }

            currentPlace = nextPlace.Id;
            yield return new WaitForSeconds(pollInterval);
        }

        result.EndTime = Time.time;
    }

    private object GetOrCreateInstance(Type testType)
    {
        // Reuse existing instance if already in scene
        MonoBehaviour existing = UnityEngine.Object.FindFirstObjectByType(testType) as MonoBehaviour;
        if (existing != null)
        {
            Debug.Log($"[TestSuite] Using existing instance of {testType.Name}");
            return existing;
        }

        // Create new — Awake() will assign GameObjects automatically
        GameObject go = new GameObject($"[Test] {testType.Name}");
        return go.AddComponent(testType);
    }

    private bool EvaluateSensor(object instance, MethodInfo transition)
    {
        SensorAttribute sensorAttr = transition.GetCustomAttribute<SensorAttribute>();

        if (sensorAttr == null)
        {
            try
            {
                // Only capture DetectInteraction — ignore Expect
                DetectInteractionInterceptor.Reset();
                ExpectInterceptor.SetIgnore(true); // ← ignore Expect during polling
                transition.Invoke(instance, null);
                ExpectInterceptor.SetIgnore(false);
                return DetectInteractionInterceptor.LastResult;
            }
            catch (Exception e)
            {
                ExpectInterceptor.SetIgnore(false);
                Debug.LogError($"[TestSuite] DetectInteraction error: {e.Message}");
                return false;
            }
        }

        return false;
    }


    private List<Type> DiscoverAllTestClasses()
    {
        List<Type> types = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (Type type in assembly.GetTypes())
                if (type.GetCustomAttribute<TestInteractionClassAttribute>() != null)
                    types.Add(type);

        return types;
    }

    private List<Type> ResolveTestClasses(List<UnityEngine.Object> objects)
    {
        List<Type> types = new List<Type>();

        foreach (UnityEngine.Object obj in objects)
        {
            if (obj == null)
            {
                Debug.LogWarning("[TestSuite] Null entry — skipping.");
                continue;
            }

            Type type = null;

            if (obj is MonoBehaviour mb)
                type = mb.GetType();

#if UNITY_EDITOR
            else if (obj is UnityEditor.MonoScript script)
                type = script.GetClass();
#endif

            if (type == null)
            {
                Debug.LogWarning($"[TestSuite] Could not resolve type from: {obj.name} — skipping.");
                continue;
            }

            if (type.GetCustomAttribute<TestInteractionClassAttribute>() == null)
            {
                Debug.LogWarning($"[TestSuite] {type.Name} missing [TestInteractionClass] — skipping.");
                continue;
            }

            Debug.Log($"[TestSuite] Resolved: {type.Name}");
            types.Add(type);
        }

        return types;
    }

    private MethodInfo FindEnabledTransition(Type type, int place)
    {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            TransitionAttribute t = method.GetCustomAttribute<TransitionAttribute>();
            if (t != null && t.UpstreamPlace == place)
                return method;
        }
        return null;
    }

    private IEnumerable<FieldInfo> GetInitialStateFields(Type type)
    {
        foreach (FieldInfo field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            if (field.GetCustomAttribute<InitialStateAttribute>() != null)
                yield return field;
    }

    private void LogResult(TestResult result)
    {
        float duration = result.EndTime - result.StartTime;

        if (result.Success)
            Debug.Log($"[TestSuite] ✔ PASS [{result.TestName}] ({duration:F2}s) — {result.Message}");
        else
            Debug.LogError($"[TestSuite] ✘ FAIL [{result.TestName}] ({duration:F2}s) — {result.Message}");
    }

    private void LogSummary()
    {
        Debug.Log($"[TestSuite] ══════════════════════════════════");
        Debug.Log($"[TestSuite] Results: {passed}/{totalTests} passed, {failed} failed.");
        Debug.Log($"[TestSuite] ══════════════════════════════════");

        StopXareus();


    }




    private void StopXareus()
    {
        ScenarioEngineKernel kernel = FindFirstObjectByType<ScenarioEngineKernel>();

        if (kernel != null)
        {
            kernel.enabled = false;
            Debug.Log("[TestSuite] Xareus engine stopped.");
        }
        else
        {
            Debug.LogWarning("[TestSuite] ScenarioEngineKernel not found in scene.");
        }
    }




}

public class TestResult
{
    public string TestName { get; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public float StartTime { get; set; }
    public float EndTime { get; set; }

    public TestResult(string testName)
    {
        TestName = testName;
        Success = false;
        Message = string.Empty;
    }
}




public static class DetectInteractionInterceptor
{
    public static bool LastResult { get; private set; }

    public static void Reset()
    {
        LastResult = false;
    }

    public static void Record(bool result)
    {
        LastResult = result;
    }
}


public static class ExpectInterceptor
{
    public static bool LastResult { get; private set; }
    private static bool _ignore = false;

    public static bool IsIgnored => _ignore;

    public static void Reset()
    {
        LastResult = false; // ← default false — must be explicitly set to true
        _ignore = false;
    }

    public static void SetIgnore(bool ignore)
    {
        _ignore = ignore;
    }

    public static void Record(bool result)
    {
        if (_ignore) return;
        LastResult = result;
    }
}