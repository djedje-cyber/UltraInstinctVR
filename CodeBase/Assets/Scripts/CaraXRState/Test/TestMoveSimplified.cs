using UnityEngine;
using Xareus.Relations.Unity;

[TestInteractionClass]
public class TestMoveSimplified : MonoBehaviour
{
    [InitialState]
    [Place(0)]
    protected GameObject controller;

    private const float MOVE_THRESHOLD = 1f;

    private Vector3 sensorPosition; // ← used by DetectInteraction
    private Vector3 expectPosition; // ← used by Expect
    [SerializeField] public ScenarioLoader scenarioLoader;
    private void Awake()
    {
        controller = GameObject.Find("RightControllerTest");

        if (controller != null)
        {
            // ← Initialize both to current position
            sensorPosition = controller.transform.position;
            expectPosition = controller.transform.position;
            Debug.Log($"[TestMoveController] Initial position: {sensorPosition}");
        }

        scenarioLoader = FindFirstObjectByType<ScenarioLoader>();
        EnsureIdentifiable(controller);
    }

    private void Start()
    {
        scenarioLoader?.GenerateAndLoad<TestMoveController>();
    }

    private void EnsureIdentifiable(GameObject go)
    {
        if (go == null) return;
        if (go.GetComponent<IdentifiableBehaviour>() == null)
            go.AddComponent<IdentifiableBehaviour>();
    }

    [Transition(1, upstreamPlace: 0)]
    [FinalState]
    public void MoveController()
    {
        // Sensor — checks against sensorPosition (never updated during polling)
        controller.DetectInteraction(go =>
        {
            bool moved = Vector3.Distance(go.transform.position, sensorPosition) > MOVE_THRESHOLD;
            return moved;
        });

        // Expect — only runs when transition fires, updates expectPosition
        controller.Expect(go =>
        {
            bool moved = Vector3.Distance(go.transform.position, expectPosition) > MOVE_THRESHOLD;
            if (moved)
            {
                sensorPosition = go.transform.position; // ← update for next transition
                expectPosition = go.transform.position;
            }
            return moved;
        });
    }

}