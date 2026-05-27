using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Runs multiple [TestInteractionClass] tests sequentially or in parallel.
/// </summary>
public class TestSuiteRunner : MonoBehaviour
{
    // -------------------------------------------------------
    // Inspector
    // -------------------------------------------------------

    [Header("Test Suite Configuration")]
    [SerializeField] private ExecutionMode executionMode = ExecutionMode.Sequential;
    [SerializeField] private float pollInterval = 0.1f;
    [SerializeField] private float delayBetween = 0.5f;
    [SerializeField] private bool autoDiscover = true;
    [SerializeField] private bool runOnStart = true;

    [Header("Manual Test List (if autoDiscover = false)")]
    [SerializeField] private List<SerializableType> testClasses = new List<SerializableType>();
    [Header("Scene GameObjects — matched by field name")]
    [SerializeField] private List<GameObjectBinding> bindings = new List<GameObjectBinding>();

    // -------------------------------------------------------
    // State
    // -------------------------------------------------------

    private List<TestResult> results = new List<TestResult>();
    private int totalTests = 0;
    private int passed = 0;
    private int failed = 0;
    private bool isRunning = false;

    public bool IsRunning => isRunning;

    // -------------------------------------------------------
    // Execution mode
    // -------------------------------------------------------

    public enum ExecutionMode
    {
        Sequential, // One test at a time
        Parallel    // All tests at the same time
    }

    // -------------------------------------------------------
    // Unity
    // -------------------------------------------------------

    private void Start()
    {
        if (runOnStart)
            StartSuite();
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public void StartSuite()
    {
        if (isRunning)
        {
            Debug.LogWarning("[TestSuite] Already running.");
            return;
        }

        List<Type> testTypes = autoDiscover
            ? DiscoverAllTestClasses()
            : ResolveTestClasses(testClasses); // Updated

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

    // -------------------------------------------------------
    // Sequential execution
    // -------------------------------------------------------

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

    // -------------------------------------------------------
    // Parallel execution
    // -------------------------------------------------------

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

        // Start all coroutines simultaneously
        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (IEnumerator routine in parallelRoutines)
            coroutines.Add(StartCoroutine(routine));

        // Wait for all to finish
        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;

        // Collect results
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

    // -------------------------------------------------------
    // Single test execution
    // -------------------------------------------------------

    private IEnumerator RunTest(Type testType, TestResult result)
    {
        object instance = Activator.CreateInstance(testType);
        int currentPlace = 0;

        InjectInitialStates(instance);

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

            // Wait for sensor
            bool detected = false;
            float timeout = 10f; // Timeout per transition
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
                result.Message = $"Timeout on transition: {transition.Name} at Place_{currentPlace}";
                break;
            }

            // Fire transition
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

            // Advance place
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

    // -------------------------------------------------------
    // Sensor evaluation
    // -------------------------------------------------------

    private bool EvaluateSensor(object instance, MethodInfo transition)
    {
        SensorAttribute sensorAttr = transition.GetCustomAttribute<SensorAttribute>();

        // No sensor — fire immediately
        if (sensorAttr == null) return true;

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

    // -------------------------------------------------------
    // Field injection
    // -------------------------------------------------------

    private void InjectInitialStates(object instance)
    {
        foreach (FieldInfo field in GetInitialStateFields(instance.GetType()))
        {
            GameObjectBinding binding = bindings.Find(b =>
                string.Equals(b.FieldName, field.Name, StringComparison.OrdinalIgnoreCase)
            );

            GameObject go = binding?.GameObject ?? GameObject.Find(field.Name);

            if (go != null)
            {
                field.SetValue(instance, go);
                Debug.Log($"[TestSuite] Injected '{field.Name}' → {go.name}");
            }
            else
            {
                Debug.LogWarning($"[TestSuite] Could not inject field: '{field.Name}'");
            }
        }
    }

    // -------------------------------------------------------
    // Discovery
    // -------------------------------------------------------

    private List<Type> DiscoverAllTestClasses()
    {
        List<Type> types = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (Type type in assembly.GetTypes())
                if (type.GetCustomAttribute<TestInteractionClassAttribute>() != null)
                    types.Add(type);

        return types;
    }

    private List<Type> ResolveTestClasses(List<SerializableType> serializableTypes)
    {
        List<Type> types = new List<Type>();

        foreach (SerializableType st in serializableTypes)
        {
            Type type = st.Resolve();

            if (type != null)
                types.Add(type);
        }

        return types;
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

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
        foreach (FieldInfo field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            if (field.GetCustomAttribute<InitialStateAttribute>() != null)
                yield return field;
    }

    // -------------------------------------------------------
    // Logging
    // -------------------------------------------------------

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

// -------------------------------------------------------
// Test Result
// -------------------------------------------------------

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

// -------------------------------------------------------
// GameObject Binding
// -------------------------------------------------------

[Serializable]
public class GameObjectBinding
{
    public string FieldName;
    public GameObject GameObject;
}