using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

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
        // Optionally reset state if needed
    }

    public override void SafeEffectorUpdate()
    {
        Debug.Log("ORACLE MoveObjectToOrigin - TestGenerated");

        if (!ValidateVirtualHand())
            return;

        var interactables = GetAllInteractablesExceptVirtualHand();
        bool movedAnyObject = false;

        foreach (var interactable in interactables)
        {
            if (TryMoveInteractableToOrigin(interactable))
            {
                movedAnyObject = true;
            }
        }

        if (!movedAnyObject)
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - No object was moved!");
        }
    }

    #region Helper Methods

    private bool ValidateVirtualHand()
    {
        if (virtualHand == null)
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - virtualHand is not assigned!");
            return false;
        }

        if (!virtualHand.TryGetComponent(out interactor))
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - XRDirectInteractor component missing on virtualHand!");
            return false;
        }

        if (!virtualHand.TryGetComponent(out interactionManager))
        {
            Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - XRInteractionManager component missing on virtualHand!");
            return false;
        }

        return true;
    }

    private IEnumerable<XRGrabInteractable> GetAllInteractablesExceptVirtualHand()
    {
        foreach (var interactable in Object.FindObjectsOfType<XRGrabInteractable>())
        {
            if (interactable.gameObject != virtualHand && !interactable.isSelected)
            {
                yield return interactable;
            }
        }
    }

    private bool TryMoveInteractableToOrigin(XRGrabInteractable interactable)
    {
        try
        {
            SelectInteractable(interactable);
            MoveInteractable(interactable);

            if (VerifyMovement(interactable))
            {
                DeselectInteractable(interactable);
                return true;
            }

            DeselectInteractable(interactable);
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ORACLE MoveObjectToOrigin - TestFailed - {interactable.name}: {ex.Message}");
            return false;
        }
    }

    private void SelectInteractable(XRGrabInteractable interactable)
    {
        interactionManager.SelectEnter(
            (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor,
            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
        );

        interactor.StartManualInteraction(
            (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
        );
    }

    private void MoveInteractable(XRGrabInteractable interactable)
    {
        interactable.transform.position = Vector3.zero;
    }

    private bool VerifyMovement(XRGrabInteractable interactable)
    {
        if (Vector3.Distance(interactable.transform.position, Vector3.zero) < 0.01f)
        {
            Debug.Log($"ORACLE MoveObjectToOrigin - TestPassed - {interactable.name} was moved to the origin.");
            return true;
        }

        Debug.LogError($"ORACLE MoveObjectToOrigin - TestFailed - {interactable.name} did not move!");
        return false;
    }

    private void DeselectInteractable(XRGrabInteractable interactable)
    {
        if (interactor.hasSelection && interactable.isSelected)
        {
            interactionManager.SelectEnter(
                (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor,
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
            );

            interactor.StartManualInteraction(
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
            );
        }
        else
        {
            Debug.LogError($"ORACLE MoveObjectToOrigin - TestFailed - {interactable.name} was not selected.");
        }
    }

    #endregion

}
