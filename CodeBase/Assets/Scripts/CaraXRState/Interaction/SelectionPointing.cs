using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class SelectionSimulator : MonoBehaviour
{
    [SerializeField] private XRInteractionManager interactionManager;

    [Header("Interactors")]
    [SerializeField] private XRRayInteractor rayInteractor;       // Controller or Hand ray
    [SerializeField] private XRGazeInteractor gazeInteractor;     // Gaze
    [SerializeField] private XRDirectInteractor directInteractor; // Direct/Touch

    [Header("Target")]
    [SerializeField] private XRBaseInteractable interactable;

    // -------------------------------------------------------
    // RAY (Controller / Hand)
    // -------------------------------------------------------

    public void SimulateRaySelect()
    {
        if (rayInteractor == null || interactable == null) return;
        interactionManager.SelectEnter(rayInteractor as IXRSelectInteractor, interactable);
    }

    public void SimulateRayDeselect()
    {
        if (rayInteractor == null || interactable == null) return;
        interactionManager.SelectExit(rayInteractor as IXRSelectInteractor, interactable);
    }

    // -------------------------------------------------------
    // GAZE
    // -------------------------------------------------------

    public void SimulateGazeSelect()
    {
        if (gazeInteractor == null || interactable == null) return;
        interactionManager.SelectEnter(gazeInteractor as IXRSelectInteractor, interactable);
    }

    public void SimulateGazeDeselect()
    {
        if (gazeInteractor == null || interactable == null) return;
        interactionManager.SelectExit(gazeInteractor as IXRSelectInteractor, interactable);
    }

    // -------------------------------------------------------
    // DIRECT (Touch)
    // -------------------------------------------------------

    public void SimulateDirectSelect()
    {
        if (directInteractor == null || interactable == null) return;
        interactionManager.SelectEnter(directInteractor as IXRSelectInteractor, interactable);
    }

    public void SimulateDirectDeselect()
    {
        if (directInteractor == null || interactable == null) return;
        interactionManager.SelectExit(directInteractor as IXRSelectInteractor, interactable);
    }

    // -------------------------------------------------------
    // HOVER (works for all interactors)
    // -------------------------------------------------------

    public void SimulateHoverEnter(XRBaseInteractor interactor)
    {
        interactionManager.HoverEnter(interactor as IXRHoverInteractor, interactable);
    }

    public void SimulateHoverExit(XRBaseInteractor interactor)
    {
        interactionManager.HoverExit(interactor as IXRHoverInteractor, interactable);
    }

    // -------------------------------------------------------
    // GENERIC (pass any interactor)
    // -------------------------------------------------------

    public void SimulateSelect(XRBaseInteractor interactor)
    {
        if (interactor == null || interactable == null) return;
        interactionManager.SelectEnter(interactor as IXRSelectInteractor, interactable);
    }

    public void SimulateDeselect(XRBaseInteractor interactor)
    {
        if (interactor == null || interactable == null) return;
        interactionManager.SelectExit(interactor as IXRSelectInteractor, interactable);
    }

    // -------------------------------------------------------
    // TIMED SELECTION (e.g. gaze dwell time)
    // -------------------------------------------------------

    public IEnumerator SimulateGazeDwell(float dwellTime)
    {
        SimulateHoverEnter(gazeInteractor);         // Start hovering
        yield return new WaitForSeconds(dwellTime); // Dwell duration
        SimulateGazeSelect();                       // Select after dwell
    }
}