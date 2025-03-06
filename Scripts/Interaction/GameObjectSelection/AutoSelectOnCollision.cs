using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoSelectOnCollision : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    public XRInteractionManager interactionManager;

    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>();

        if (interactable != null)
        {
            Debug.Log("✋ Sélection automatique de : " + other.name);
            interactionManager.SelectEnter(interactor, interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>();

        if (interactable != null)
        {
            Debug.Log("🛑 Désélection automatique de : " + other.name);
            interactionManager.SelectExit(interactor, interactable);
        }
    }
}
