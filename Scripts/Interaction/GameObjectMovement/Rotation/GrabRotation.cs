using UnityEngine;

using System.Collections;

public class GrabRotation : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    private bool isGrabbing = false;
    private Transform grabbedObject;
    
    // Vitesse initiale de rotation sur l'axe Y
    private Vector3 rotationSpeedY = new Vector3(0f, 90f, 0f);  
    // Vitesse de rotation sur l'axe Z après la rotation sur Y
    private Vector3 rotationSpeedZ = new Vector3(0f, 0f, 90f);

    public string targetTag = "Interactable";  // Assurez-vous que vos objets interactifs ont ce tag
    private bool isControllerRotating = false;  // Flag pour savoir si on doit ignorer la rotation du contrôleur
    private bool isRotatingY = true;  // Détermine si on fait la rotation sur l'axe Y ou Z
    private float grabDelay = 60f;  // Durée en secondes pendant laquelle les mouvements du contrôleur sont bloqués
    private float rotationTimer = 0f;  // Timer pour suivre combien de temps la rotation a duré

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
        // Vérifier si l'objet possède un XRGrabInteractable
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (interactable != null && other.CompareTag(targetTag))  // Assurez-vous que les objets à attraper ont ce tag
        {
            Debug.Log("👋 Objet détecté : " + other.name);
            StartGrab(interactable);
        }
        else
        {
            Debug.Log("❌ L'objet n'a pas un XRGrabInteractable ou le tag ne correspond pas.");
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

    // Démarrer l'interaction avec l'objet
    private void StartGrab(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable)
    {
        // Cast l'interactable en IXRSelectInteractable avant de démarrer l'interaction manuelle
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectInteractable = interactable as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable;
        if (selectInteractable != null)
        {
            interactor.StartManualInteraction(selectInteractable);
            isGrabbing = true;
            grabbedObject = interactable.transform; // Référence à l'objet attrapé
            isControllerRotating = true;  // Bloque les mouvements du contrôleur

            // Lancer la coroutine pour bloquer les mouvements du contrôleur pendant 15 secondes
            StartCoroutine(DisableControllerRotationForDelay(grabDelay));
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
            isControllerRotating = false;  // Réactive les mouvements du contrôleur
        }
    }

    // Coroutine qui désactive les mouvements du contrôleur pendant un certain temps
    private IEnumerator DisableControllerRotationForDelay(float delay)
    {
        // Attendre pendant la durée du délai
        yield return new WaitForSeconds(delay);

        // Après le délai, réactiver les mouvements du contrôleur
        isControllerRotating = false;
        Debug.Log("⏱️ Rotation contrôleur réactivée après " + delay + " secondes");

        // Relâcher automatiquement l'objet après la rotation
        EndGrab();
        Debug.Log("🔓 L'objet a été relâché après 15 secondes.");
    }

    // Appliquer une rotation sur l'objet
    void Update()
    {
        if (isGrabbing && grabbedObject != null && isControllerRotating)
        {
            rotationTimer += Time.deltaTime;

            if (isRotatingY)
            {
                // Appliquer une rotation sur l'axe Y
                grabbedObject.Rotate(rotationSpeedY * Time.deltaTime);
                if (rotationTimer >= grabDelay)
                {
                    // Après la rotation sur l'axe Y, passer à la rotation sur l'axe Z
                    isRotatingY = false;
                    rotationTimer = 0f;  // Réinitialiser le timer pour la rotation sur Z
                    Debug.Log("✅ Rotation sur l'axe Y terminée. Passage à la rotation sur l'axe Z.");
                }
            }
            else
            {
                // Appliquer une rotation sur l'axe Z
                grabbedObject.Rotate(rotationSpeedZ * Time.deltaTime);
            }

            Debug.Log("Rotating");
        }
    }
}
