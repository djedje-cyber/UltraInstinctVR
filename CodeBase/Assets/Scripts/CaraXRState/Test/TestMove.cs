using UnityEngine;

[TestInteractionClass]
public class TestMoveController : MonoBehaviour
{
    [InitialState][SerializeField][Place(0)]
    protected GameObject controller;

    // Threshold to consider the controller has moved
    private const float MOVE_THRESHOLD = 0.1f;

    // Last known position of the controller
    private Vector3 lastPosition;



    private void Awake()
    {
        // Auto-assign if not set in Inspector
        if (controller == null)
            controller = GameObject.Find("RightControllerTest");
    }



    // -------------------------------------------------------
    // Transition 1 — Controller starts moving
    // Detect : controller has moved beyond threshold from last position
    // Expect : controller is now at a different position
    // -------------------------------------------------------
    [Transition(1, upstreamPlace: 0)]
    [Place(1)]
    public void MoveController()
    { 

        // Sensor — detect the controller has moved beyond threshold
        controller.DetectInteraction(go =>
            Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD
        );

        // Effector/Oracle — verify the controller is at a new position
        controller.Expect(go =>
        {
            bool moved = Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD;
            lastPosition = go.transform.position; // Update last position
            return moved;
        });
    }

    // -------------------------------------------------------
    // Transition 2 — Controller reached anywhere valid
    // Detect : controller has moved again beyond threshold
    // Expect : controller stayed within any valid bounds
    // -------------------------------------------------------
    [Transition(2, upstreamPlace: 1)]
    [Place(0)]
    public void ControllerMovedAgain()
    {
        // Sensor — detect another movement
        controller.DetectInteraction(go =>
            Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD
        );

        // Effector/Oracle — verify movement was valid
        controller.Expect(go =>
        {
            bool moved = Vector3.Distance(go.transform.position, lastPosition) > MOVE_THRESHOLD;
            lastPosition = go.transform.position; // Update for next transition
            return moved;
        });
    }
}