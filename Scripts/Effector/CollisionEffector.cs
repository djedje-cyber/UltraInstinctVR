using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;

[FunctionDescription("Collision Detection Effect")]
public class CollisionEffector : AUnityEffector
{
    // Cube à surveiller pour la détection de collision
    [ConfigurationParameter("Cube", Necessity.Required)]
    protected GameObject cube;

    // Distance de détection pour la collision
    [ConfigurationParameter("Collision Detection Radius", Necessity.Required)]
    protected float collisionDetectionRadius = 0.5f;

    public CollisionEffector(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext,
        IContext eventContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, eventContext)
    { }

    // Méthode appelée à chaque frame pour vérifier les collisions
    public override void SafeEffectorUpdate()
    {
        Debug.Log("TestGenerated - CollisionDetection");
        DetectCollision();
    }


    private void DetectCollision()
    {
        // Détecter tous les colliders dans une sphère autour du cube
        Collider[] hitColliders = Physics.OverlapSphere(cube.transform.position, collisionDetectionRadius);

        // Si des objets sont en collision avec l'objet "cube"
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                // Vérifie que l'objet détecté n'est pas le même que le cube
                if (hitCollider != null && hitCollider.gameObject != cube)
                {
                    // Détection de collision avec un autre objet (excluant le cube lui-même)
                    Debug.Log("ORACLE CollisionDetection - TestPassed - Collision successfully done with " 
                            + hitCollider.name + " at : " + cube.transform.position);
                    
                    // Retourner pour sortir de la boucle une fois la collision traitée
                    return;
                }
            }

            // Si aucune collision valide n'a été trouvée, afficher un message d'échec
            Debug.LogError("ORACLE CollisionDetection - TestFailed - No valid collision detected at : " 
                        + cube.transform.position);
        }
        else
        {
            // Aucune collision détectée
            Debug.LogError("ORACLE CollisionDetection - TestFailed - No collision detected at : " 
                        + cube.transform.position);
        }
    }

}
