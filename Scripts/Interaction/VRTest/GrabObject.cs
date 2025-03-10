using UnityEngine;


public class GrabObject : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    public string targetTag = "Interactable";  // Assurez-vous que vos objets interactifs ont ce tag

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
            interactor.StartManualInteraction(interactable as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable);  // Démarrer l'interaction
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
            interactor.EndManualInteraction();  // Arrêter l'interaction
        }
    }
}
