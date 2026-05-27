using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[TestInteractionClass]
public class TestUndoRedoCube
{
    [InitialState] [Place(0)] 
    private GameObject controller;

    [InitialState]
    [Place(0)]
    private GameObject cube;

    [InitialState]
    [Place(0)]

    private GameObject buttonUndo;

    [InitialState]
    [Place(0)]

    private GameObject buttonRedo;

    private Vector3 cubeInitialPosition;

    // -------------------------------------------------------
    // Transition 1 — Grab the cube
    // Detect : controller is hovering the cube
    // Expect : cube is grabbed (isSelected)
    // -------------------------------------------------------
    [Transition(1)]
    [Place(1)]
    public void GrabCube()
    {
        cube.DetectInteraction(go => go.GetComponent<XRBaseInteractable>().isHovered);
        cube.Expect(go => go.GetComponent<XRBaseInteractable>().isSelected);
    }

    // -------------------------------------------------------
    // Transition 2 — Move the cube
    // Detect : cube position has changed
    // Expect : cube is at a different position than initial
    // -------------------------------------------------------
    [Transition(2)]
    [Place(2)]
    public void MoveCube()
    {
        cube.DetectInteraction(go =>
            Vector3.Distance(go.transform.position, cubeInitialPosition) > 0.1f
        );
        cube.Expect(go =>
            go.transform.position != cubeInitialPosition
        );
    }

    // -------------------------------------------------------
    // Transition 3 — Release the cube
    // Detect : cube is no longer grabbed
    // Expect : cube stayed at new position
    // -------------------------------------------------------
    [Transition(3)]
    [Place(3)]
    public void ReleaseCube()
    {
        cube.DetectInteraction(go =>
            !go.GetComponent<XRBaseInteractable>().isSelected
        );
        cube.Expect(go =>
            go.transform.position != cubeInitialPosition
        );
    }

    // -------------------------------------------------------
    // Transition 4 — Hover the Undo button
    // Detect : controller ray is hovering the Undo button
    // -------------------------------------------------------
    [Transition(4)]
    [Place(4)]
    public void HoverUndo()
    {
        buttonUndo.DetectInteraction(go =>
            go.GetComponent<XRBaseInteractable>().isHovered
        );
    }

    // -------------------------------------------------------
    // Transition 5 — Click Undo
    // Detect : Undo button is clicked
    // Expect : cube is back to its initial position
    // -------------------------------------------------------
    [Transition(5)]
    [Place(5)]
    public void ClickUndo()
    {
        buttonUndo.DetectInteraction(go =>
            go.GetComponent<XRBaseInteractable>().isSelected
        );
        cube.Expect(go =>
            Vector3.Distance(go.transform.position, cubeInitialPosition) < 0.01f
        );
    }

    // -------------------------------------------------------
    // Transition 6 — Release Undo button
    // Detect : Undo button is no longer clicked
    // Expect : cube is still at initial position
    // -------------------------------------------------------
    [Transition(6)]
    [Place(6)]
    public void ReleaseUndo()
    {
        buttonUndo.DetectInteraction(go =>
            !go.GetComponent<XRBaseInteractable>().isSelected
        );
        cube.Expect(go =>
            Vector3.Distance(go.transform.position, cubeInitialPosition) < 0.01f
        );
    }

    // -------------------------------------------------------
    // Transition 7 — Click Redo
    // Detect : Redo button is clicked
    // Expect : cube is back to moved position
    // -------------------------------------------------------
    [Transition(7)]
    [Place(1)]
    public void ClickRedo()
    {
        buttonRedo.DetectInteraction(go =>
            go.GetComponent<XRBaseInteractable>().isSelected
        );
        cube.Expect(go =>
            Vector3.Distance(go.transform.position, cubeInitialPosition) > 0.1f
        );
    }
}