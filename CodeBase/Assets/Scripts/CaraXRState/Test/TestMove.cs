using UnityEngine;
using Xareus.Relations.Unity;

[TestInteractionClass]
public class TestMoveController : MonoBehaviour
{
    [InitialState]
    [Place(0)]
    protected GameObject controller;

    [SerializeField] public ScenarioLoader scenarioLoader;

    private const float MOVE_THRESHOLD = 0.1f;
    private Vector3 lastPosition;

    private void Awake()
    {
        controller = GameObject.Find("RightControllerTest");

        // Auto-find — no Inspector drag needed
        scenarioLoader = FindFirstObjectByType<ScenarioLoader>();

        if (scenarioLoader == null)
            Debug.LogError("[TestMoveController] ScenarioLoader not found in scene!");
        else
            Debug.Log($"[TestMoveController] ScenarioLoader found: {scenarioLoader.name}");

        EnsureIdentifiable(controller);
    }

    private void Start()
    {
        if (scenarioLoader == null)
        {
            Debug.LogError("[TestMoveController] ScenarioLoader is null — drag it in the Inspector!");
            return;
        }

        scenarioLoader.GenerateAndLoad<TestMoveController>();
    }





    private void EnsureIdentifiable(GameObject go)
    {
        if (go.GetComponent<IdentifiableBehaviour>() == null)
            go.AddComponent<IdentifiableBehaviour>();

        Debug.Log($"[TestMoveController] {go.name} Id: {go.GetComponent<IdentifiableBehaviour>().Id}");
    }

    [Transition(1, upstreamPlace: 0)]
    [Place(1)]
    public void MoveController()
    {
        controller.DetectInteraction(go =>
            Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD
        );

        controller.Expect(go =>
        {
            bool moved = Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD;
            lastPosition = go.transform.position;
            return moved;
        });
    }

    [Transition(2, upstreamPlace: 1)]
    [Place(0)]
    public void ControllerMovedAgain()
    {
        controller.DetectInteraction(go =>
            Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD
        );

        controller.Expect(go =>
        {
            bool moved = Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD;
            lastPosition = go.transform.position;
            return moved;
        });
    }
}