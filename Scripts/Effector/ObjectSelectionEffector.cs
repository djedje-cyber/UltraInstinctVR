using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;

public class ObjectSelectionEffector : AUnityEffector
{
    [ConfigurationParameter("Selection Trigger Button", Necessity.Required)]
    protected InputFeatureUsage<bool> selectionButton = CommonUsages.triggerButton;

    private InputDevice rightHandController;
    private Collider rightHandCollider;  
    private Collider selectedObjectCollider;  
    private bool isSelecting = false;

    public ObjectSelectionEffector(Xareus.Scenarios.Event @event, 
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext,
        IContext anotherContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, anotherContext) 
    { 
        // Initialiser le contrôleur de la main droite
        rightHandController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    // Implémentation de la méthode SafeEffectorUpdate (obligatoire)
    public override void SafeEffectorUpdate()
    {
        // Vérifier si le contrôleur de la main droite est actif
        if (rightHandController.isValid)
        {
            // Vérifier si le bouton de sélection est activé
            bool isPressed = false;
            rightHandController.TryGetFeatureValue(selectionButton, out isPressed);

            if (isPressed)
            {
                // Si un bouton est pressé, détecter la collision avec un objet
                DetectObjectCollision();
            }
        }
    }

    // Méthode pour gérer la détection de collision
    private void DetectObjectCollision()
    {
        if (rightHandCollider != null && selectedObjectCollider != null)
        {
            // Vérifier si le collider de la main droite entre en collision avec un objet
            if (rightHandCollider.bounds.Intersects(selectedObjectCollider.bounds))
            {
                Debug.Log("ORACLE ObjectSceneSelection TestPassed " + selectedObjectCollider.gameObject.name);
            }
            else{
                Debug.LogError("ORACLE ObjectSceneSelection TestFailed " + selectedObjectCollider.gameObject.name);
            }
        }
    }

    // Méthode pour récupérer le collider attaché au contrôleur de la main droite
    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si la collision est avec un objet sélectionnable
        selectedObjectCollider = other;
    }

    // Méthode pour obtenir la position actuelle du joueur
    private Vector3 GetPlayerPosition()
    {
        // Exemple de récupération de la position de la caméra (l'avatar du joueur)
        return Camera.main.transform.position;
    }
}
