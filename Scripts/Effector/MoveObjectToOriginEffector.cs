    using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

[FunctionDescription("Effector to move an object to origin and manage its selection")]
public class SelectAndMoveToOriginEffector : AUnityEffector
{
    [ConfigurationParameter("Object to Move", Necessity.Required)]
    private GameObject virtualHand;

    [ConfigurationParameter("Teleport Delay", Necessity.Optional)]
    public float teleportDelay = 2f;

    [ContextVariable("Result", "Indicates the result of the operation")]
    protected ContextVariable<string> result;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    private XRInteractionManager interactionManager;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable currentInteractable;

    public SelectAndMoveToOriginEffector(Xareus.Scenarios.Event @event, 
        Dictionary<string, List<string>> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext, 
        IContext eventContext) 
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, eventContext)
    { }

    public override void SafeReset()
    {
        // Reset any variables if necessary (no reset logic here for now)
    }

    public override void SafeEffectorUpdate()
    {
        // Handling selection and teleportation
        if (virtualHand != null)
        {
            interactor = virtualHand.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
            interactionManager = virtualHand.GetComponent<XRInteractionManager>();

            if (interactor == null || interactionManager == null)
            {
                result.Value = "Failed";
                Debug.LogError("❌ L'interacteur ou l'InteractionManager n'est pas assigné !");
                return;
            }

            // For now, let's assume we have a valid interactable object in range.
            // You can add logic here to select and interact with a specific object.
            // Assuming you already have the interactable object.

            var interactable = virtualHand.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(); 

            if (interactable != null)
            {
                // Simulating the interaction for this example
                interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);
                interactor.StartManualInteraction((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);

                currentInteractable = interactable;
                StartCoroutine(TeleportAndUnselect());
            }
            else
            {
                result.Value = "Failed";
                Debug.LogWarning("⚠️ Aucune interaction valide avec l'objet.");
                return;
            }
        }
    }

    private IEnumerator TeleportAndUnselect()
    {
        // Teleportation to (0, 0, 0)
        yield return new WaitForSeconds(teleportDelay);

        if (currentInteractable != null)
        {
            // Déplacer l'objet à l'origine
            currentInteractable.transform.position = Vector3.zero;
            Debug.Log("🚀 Objet téléporté au point (0,0,0) : " + currentInteractable.name);

            // Vérifier si le déplacement a bien eu lieu
            if (currentInteractable.transform.position == Vector3.zero)
            {
                Debug.Log("ok");
                result.Value = "Success";
            }
            else
            {
                Debug.Log("failed");
                result.Value = "Failed";
            }

            // Relâcher l'objet
            interactor.EndManualInteraction();
            interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)currentInteractable);
            Debug.Log("🛑 Objet désélectionné : " + currentInteractable.name);
            currentInteractable = null;
        }
        else
        {
            result.Value = "Failed";
            Debug.LogWarning("⚠️ Aucun objet à déplacer.");
        }
    }
}
