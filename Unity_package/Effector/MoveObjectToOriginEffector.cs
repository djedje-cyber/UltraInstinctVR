using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;



namespace MoveObjectToOriginEffectorSpace
{

    [FunctionDescription("Effector to move an object to origin and manage its selection")]


    /// <summary> 
    /// Class <c>MoveObjectToOriginEffector</c> moves a specified interactable object to the origin (0,0,0) in a Unity XR environment.
    /// </summary>
    public class MoveObjectToOriginEffector : AUnityEffector
    {
        [ConfigurationParameter("Object to Move", Necessity.Required)]
        private GameObject virtualHand;

        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
        private XRInteractionManager interactionManager;
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable currentInteractable;

        public MoveObjectToOriginEffector(Xareus.Scenarios.Event @event,
            Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
            IContext externalContext,
            IContext scenarioContext,
            IContext sequenceContext,
            IContext eventContext)
            : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext))
        { }


        /// <summary>
        /// Method <c>SafeReset</c> resets the effector state before execution.
        /// </summary>
        public override void SafeReset()
        {
            // Optionally reset state if needed
        }


        /// <summary>
        /// Method <c>SafeEffectorUpdate</c> performs the action of moving the object to the origin.
        /// </summary>
        public override void SafeEffectorUpdate()
        {
            Debug.Log("ORACLE MoveObjectToOrigin - TestGenerated");

            if (!ValidateVirtualHand())
                return;

            var interactables = GetAllInteractablesExceptVirtualHand();
            bool movedAnyObject = false;

            movedAnyObject = interactables.Any(TryMoveInteractableToOrigin);

            if (!movedAnyObject)
            {
                Debug.LogError("ORACLE MoveObjectToOrigin - TestFailed - No object was moved!");
            }
        }

        #region Helper Methods


        /// <summary>
        /// Method <c>ValidateVirtualHand</c> checks if the virtual hand and its components are properly assigned.
        /// </summary>
        /// <returns></returns>
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



        /// <summary>
        /// Method <c>GetAllInteractablesExceptVirtualHand</c> retrieves all XRGrabInteractable objects in the scene except the virtual hand and those currently selected.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<XRGrabInteractable> GetAllInteractablesExceptVirtualHand()
        {
            foreach (var interactable in Object.FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None))
            {
                if (interactable.gameObject != virtualHand && !interactable.isSelected)
                {
                    yield return interactable;
                }
            }
        }
         

        /// <summary>
        /// Method <c>TryMoveInteractableToOrigin</c> attempts to select, move, and verify the movement of a given interactable object to the origin.
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method <c>SelectInteractable</c> selects the specified interactable object using the interactor.
        /// </summary>
        /// <param name="interactable"></param>

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





        /// <summary>
        /// Method <c>MoveInteractable</c> moves the specified interactable object to the origin (0,0,0).
        /// </summary>
        /// <param name="interactable"></param>
        private static void MoveInteractable(XRGrabInteractable interactable)
        {
            interactable.transform.position = Vector3.zero;
        }

        private static bool VerifyMovement(XRGrabInteractable interactable)
        {
            if (Vector3.Distance(interactable.transform.position, Vector3.zero) < 0.01f)
            {
                Debug.Log($"ORACLE MoveObjectToOrigin - TestPassed - {interactable.name} was moved to the origin.");
                return true;
            }

            Debug.LogError($"ORACLE MoveObjectToOrigin - TestFailed - {interactable.name} did not move!");
            return false;
        }

        /// <summary>
        /// Method <c>DeselectInteractable</c> deselects the specified interactable object using the interactor.
        /// </summary>
        /// <param name="interactable"></param>
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

}
