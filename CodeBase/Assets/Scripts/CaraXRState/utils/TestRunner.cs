using System;
using System.Collections;

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
        // Get or create instance — Awake() handles field assignment
        object instance = GetOrCreateInstance(testType);

        int currentPlace = 0;

        result.StartTime = Time.time;

        while (true)
        {
            MethodInfo transition = FindEnabledTransition(testType, currentPlace);

            if (transition == null)
            {
                result.Success = true;
                result.Message = $"Completed at Place_{currentPlace}";
                break;
            }

            bool detected = false;
            float timeout = 10f;
            float elapsed = 0f;

            while (!detected && elapsed < timeout)
            {
                detected = EvaluateSensor(instance, transition);
                elapsed += pollInterval;
                yield return new WaitForSeconds(pollInterval);
            }

            if (!detected)
            {
                result.Success = false;
                result.Message = $"Timeout on: {transition.Name} at Place_{currentPlace}";
                break;
            }

            try
            {
                transition.Invoke(instance, null);
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = $"Exception in {transition.Name}: {e.InnerException?.Message ?? e.Message}";
                break;
            }

            PlaceAttribute nextPlace = transition.GetCustomAttribute<PlaceAttribute>();
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

        // No [Sensor] — use DetectInteraction lambda
        if (sensorAttr == null)
        {
            try
            {
                DetectInteractionInterceptor.Reset();
                transition.Invoke(instance, null);
                return DetectInteractionInterceptor.LastResult;
            }
            catch (Exception e)
            {
                Debug.LogError($"[TestSuite] DetectInteraction error: {e.Message}");
                return false;
            }
        }

        // [Sensor] attribute approach
        try
        {
            foreach (FieldInfo field in GetInitialStateFields(instance.GetType()))
            {
                GameObject go = field.GetValue(instance) as GameObject;
                if (go == null) continue;

                Component sensor = go.GetComponent(sensorAttr.ClassName.Split(',')[0]);
                if (sensor == null) continue;

                MethodInfo runCheck = sensor.GetType().GetMethod(
                    "RunCheck",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                );

                if (runCheck != null)
                {
                    object result = runCheck.Invoke(sensor, null);
                    PropertyInfo successProp = result?.GetType().GetProperty("Success");
                    if (successProp != null)
                        return (bool)successProp.GetValue(result);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[TestSuite] Sensor error: {e.Message}");
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

    public static void Reset()
    {
        LastResult = true; // Default true — pass unless explicitly failed
    }

    public static void Record(bool result)
    {
        LastResult = result;
    }
}