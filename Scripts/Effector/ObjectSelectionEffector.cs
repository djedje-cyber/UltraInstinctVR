using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;

[FunctionDescription("Object Selection Detection Effect")]
public class ObjectSelectionEffector : AUnityEffector
{
    // Le cube à surveiller
    [ConfigurationParameter("Cube", Necessity.Required)]
    protected GameObject cube;

    // La distance de détection
    [ConfigurationParameter("Selection Distance Threshold", Necessity.Required)]
    protected float selectionDistanceThreshold = 0.1f; // Distance à partir de laquelle on considère la sélection

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
        // Vérifier si le cube a été sélectionné par collision
        DetectCollisionSelection();
    }

    // Méthode pour détecter la sélection par collision
    private void DetectCollisionSelection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(cube.transform.position, selectionDistanceThreshold);      
        Debug.Log("TestGenerated - Teleportation");

        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                // Si l'objet en collision est le cube, afficher "Selection ok"
                if (hitCollider.gameObject == cube)
                {
                    Debug.Log("ORACLE ObjectSelection - TestPassed - Selection ok");
                    return; // La sélection a eu lieu, on arrête la boucle
                }
                 else
                {
                    // Si aucun objet en collision n'est le cube, afficher "Selection not ok"
                    Debug.LogError("ORACLE ObjectSelection - Failed - Selection not ok");
                }                
            }
        }
        else
        {
            // Si aucun objet en collision n'est le cube, afficher "Selection not ok"
            Debug.LogError("ORACLE ObjectSelection - Failed - Selection not ok");
        }
    }
}
