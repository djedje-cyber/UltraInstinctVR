using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;
using System.Collections;

public class GrabSimulator
{
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private XRBaseInteractor interactorRef;
    [SerializeField] private XRBaseInteractable interactable;

    public void SimulateGrab()
    {
        interactionManager.SelectEnter(interactorRef as IXRSelectInteractor, interactable);
    }

    public void SimulateRelease()
    {
        interactionManager.SelectExit(interactorRef as IXRSelectInteractor, interactable);
    }


    public IEnumerator SimulateGrabRotation(Transform interactorTransform, Vector3 targetEuler, float duration)
    {
        Quaternion startRotation = interactorTransform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetEuler);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            interactorTransform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            yield return null;
        }

        interactorTransform.rotation = endRotation;
    }
}
