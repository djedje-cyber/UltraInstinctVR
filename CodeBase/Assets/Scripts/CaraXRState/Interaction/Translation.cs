using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;
using System.Collections;

public class Translation
{
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private XRBaseInteractor interactorRef;
    [SerializeField] private XRBaseInteractable interactable;


    public enum TranslationDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward,
        Custom
    }



    public void SimulateGrab()
    {
        interactionManager.SelectEnter(interactorRef as IXRSelectInteractor, interactable);
    }

    public void SimulateRelease()
    {
        interactionManager.SelectExit(interactorRef as IXRSelectInteractor, interactable);
    }

    public IEnumerator SimulateGrabTranslation(Transform interactorTransform, TranslationDirection direction, float distance, float duration, Vector3 customDirection = default)
    {
        Vector3 directionVector = direction switch
        {
            TranslationDirection.Up => Vector3.up,
            TranslationDirection.Down => Vector3.down,
            TranslationDirection.Left => Vector3.left,
            TranslationDirection.Right => Vector3.right,
            TranslationDirection.Forward => Vector3.forward,
            TranslationDirection.Backward => Vector3.back,
            TranslationDirection.Custom => customDirection.normalized,
            _ => Vector3.zero
        };

        Vector3 startPosition = interactorTransform.position;
        Vector3 targetPosition = startPosition + directionVector * distance;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            interactorTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        interactorTransform.position = targetPosition;
    }
}