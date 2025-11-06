using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

[FunctionDescription("Object Selection Detection Effect")]
public class ObjectSelectionEffector : AUnityEffector
{
    // Le cube à surveiller
    [ConfigurationParameter("Cube", Necessity.Required)]
    protected GameObject cube;

    // La distance de détection
    [ConfigurationParameter("Selection Distance Threshold", Necessity.Required)]
    protected float selectionDistanceThreshold = 0.1f; // Distance à partir de laquelle on considère la sélection

    [ConfigurationParameter("Interactor", Necessity.Required)]

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    
    [ConfigurationParameter("Interaction Manager", Necessity.Required)]
    public XRInteractionManager interactionManager;

    // Variables pour vérifier la dernière position
    private Vector3 lastPosition;

    public ObjectSelectionEffector(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext,
        IContext eventContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, eventContext)
    { }

    // Méthode appelée à chaque frame pour vérifier les collisions avec le cube
    public override void SafeEffectorUpdate()
    {
        Debug.Log("TestGenerated - Object Selection");
        DetectCollisionSelection();
    }

private void DetectCollisionSelection()
{
    Collider[] hitColliders = Physics.OverlapSphere(cube.transform.position, selectionDistanceThreshold);

    bool objectSelected = false;

    foreach (var hitCollider in hitColliders)
    {
        if (hitCollider != null)
        {

            if (interactor != null && interactionManager != null)
            {
                var interactable = hitCollider.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>();

                if (interactable != null)
                {
                    // Si l'objet est détecté, et il n'est pas déjà sélectionné
                    if (!objectSelected)
                    {
                        interactionManager.SelectEnter(interactor, interactable);
                        objectSelected = true; // Marque que l'objet est sélectionné
                        interactionManager.SelectExit(interactor,interactable);
                        break;

                    }
                }

            }
        }

    }
    if(objectSelected)
    {
        Debug.Log("ORACLE CanSelect - TestPassed - Object selected successfully at " + cube.transform.position);
    }
    else
    {
        Debug.LogError("ORACLE CanSelect - TestFailed - Object couldn't be selected at " + cube.transform.position);
    }

}

}
