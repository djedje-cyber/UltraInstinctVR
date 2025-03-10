using UnityEngine;

using System.Collections;

public class MoveRightToLeftZ : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    private Transform grabbedObject;
    public string targetTag = "Interactable";  
    private bool isGrabbing = false;

    public float moveDistance = 5f;  // Distance à parcourir sur l'axe Z
    public float moveSpeed = 2f;  // Vitesse du déplacement

    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur " + gameObject.name);
        }
    }

    // Lorsque la main entre en collision avec un objet
    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (interactable != null && other.CompareTag(targetTag))  
        {
            Debug.Log("👋 Objet détecté : " + other.name);
            StartGrab(interactable);
        }
        else
        {
            Debug.Log("❌ L'objet n'est pas interactif ou le tag ne correspond pas.");
        }
    }

    // Lorsque la main quitte l'objet
    private void OnTriggerExit(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (interactable != null)
        {
            Debug.Log("🛑 Objet relâché : " + other.name);
            EndGrab();
        }
    }

    // Démarrer l'interaction avec l'objet et commencer le mouvement
    private void StartGrab(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectInteractable = interactable as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable;
        if (selectInteractable != null)
        {
            interactor.StartManualInteraction(selectInteractable);
            grabbedObject = interactable.transform;
            isGrabbing = true;

            // Démarrer le mouvement avant-arrière (axe Z)
            StartCoroutine(MoveObjectForwardBackward());
        }
        else
        {
            Debug.LogError("❌ Impossible de caster l'objet en IXRSelectInteractable");
        }
    }

    // Arrêter l'interaction avec l'objet
    private void EndGrab()
    {
        if (grabbedObject != null)
        {
            interactor.EndManualInteraction();
            isGrabbing = false;
            grabbedObject = null;
        }
    }

    // Coroutine pour déplacer l'objet en avant et en arrière sur l'axe Z
    private IEnumerator MoveObjectForwardBackward()
    {
        if (grabbedObject == null) yield break;

        Vector3 startPosition = grabbedObject.position;
        Vector3 forwardPosition = startPosition + Vector3.forward * moveDistance;
        Vector3 backwardPosition = startPosition;  // Revenir à la position d'origine

        // Déplacement vers l'avant (Z+)
        yield return MoveToPosition(grabbedObject, forwardPosition);
        Debug.Log("➡️ Déplacement terminé vers l'avant");

        // Déplacement vers l'arrière (Z-)
        yield return MoveToPosition(grabbedObject, backwardPosition);
        Debug.Log("⬅️ Déplacement terminé vers l'arrière");

        // Une fois le mouvement terminé, relâcher l'objet
        EndGrab();
    }

    // Fonction de déplacement fluide vers une position donnée
    private IEnumerator MoveToPosition(Transform obj, Vector3 targetPosition)
    {
        while (Vector3.Distance(obj.position, targetPosition) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
