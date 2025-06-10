using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[FunctionDescription("Effector to move an object to origin and manage its selection")]
public class MoveObjectToOriginEffector : AUnityEffector
{
    [ConfigurationParameter("Object to Move", Necessity.Required)]
    private GameObject virtualHand;

    [ConfigurationParameter("Teleport Delay", Necessity.Optional)]
    public float teleportDelay = 2f;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    private XRInteractionManager interactionManager;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable currentInteractable;

    public MoveObjectToOriginEffector(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext,
        IContext eventContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, eventContext)
    { }

    public override void SafeReset()
    {
        // Optionally reset something if needed
    }

    public override void SafeEffectorUpdate()
    {
        Debug.Log("ORACLE MoveObjectToOrigin - TestGenerated");


        if (virtualHand == null)
        {
            Debug.LogError(" ORACLE MoveObjectToOrigin - TestFailed - virtualHand is not assigned!");
            return;
        }

        if (!virtualHand.TryGetComponent(out interactor))
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - XRDirectInteractor component missing on virtualHand!");
            return;
        }

        if (!virtualHand.TryGetComponent(out interactionManager))
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - XRInteractionManager component missing on virtualHand!");
            return;
        }

        // Récupérer tous les objets interactables SAUF le virtualHand
        var interactables = Object.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        bool movedAnyObject = false;



        foreach (var interactable in interactables)
        {


            if (interactable.gameObject == virtualHand)
                continue; // On ignore le virtualHand lui-même

            // Vérifier si l'objet est déjà sélectionné (ajouter un contrôle pour éviter plusieurs sélections)
            if (interactable.isSelected)
            {
                continue;  // Si l'objet est déjà sélectionné, on passe à l'objet suivant
            }

            // Sélectionner et déplacer l'objet (uniquement si non sélectionné)
            interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, 
                                           (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);
            interactor.StartManualInteraction((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);


            // Déplacement de l'objet à l'origine
            interactable.transform.position = Vector3.zero;

            // Vérification du déplacement
            if (Vector3.Distance(interactable.transform.position, Vector3.zero) < 0.01f)
            {
                Debug.Log($" ORACLE MoveObjectToOrigin - TestPassed - {interactable.name} a été déplacé à l'origine.");
                movedAnyObject = true;
            }
            else
            {
                Debug.LogError($" ORACLE MoveObjectToOrigin - TestFailed - {interactable.name} n'a pas bougé !");
            }

            // Désélectionner après déplacement
            if (interactor.hasSelection && interactable.isSelected)
            {
                interactor.EndManualInteraction();
                interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, 
                                              (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);
               
            }
            else
            {
                Debug.LogError($" ORACLE MoveObjectToOrigin - TestFailed - {interactable.name} n'était pas sélectionné.");
            }
        }

        if (!movedAnyObject)
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - Aucun objet n'a été déplacé !");
        }
    }
}
