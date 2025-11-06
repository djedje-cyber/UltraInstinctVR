using UnityEngine;


public class AutoSelectWithRay : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

    void Start()
    {
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
    }

    void Update()
    {
        if (rayInteractor != null && rayInteractor.TryGetHitInfo(out Vector3 pos, out Vector3 normal, out int hitIndex, out bool valid))
        {
            if (valid)
            {
                RaycastHit hit;
                if (Physics.Raycast(rayInteractor.transform.position, rayInteractor.transform.forward, out hit))
                {
                    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = hit.collider.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

                    if (interactable != null)
                    {
                        rayInteractor.interactionManager.SelectEnter(
                            (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)rayInteractor, 
                            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
                        );

                        Debug.Log("🔦 Objet sélectionné par le raycast : " + interactable.name);
                    }
                }
            }
        }
    }
}
