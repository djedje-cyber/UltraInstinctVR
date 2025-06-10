using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.IO; 

public class OutsideSceneEffector : AUnityEffector
{
    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 1.0f;


    [ConfigurationParameter("GameObjectToObserve", Necessity.Required)]
    protected GameObject gameObjectToObserve;

    private Vector3 lastPosition;
    private Bounds sceneBounds; 
    private string sceneBoundsFilePath = "Assets/Scripts/CoveredObjects/sceneBounds.txt"; // Chemin du fichier

    public OutsideSceneEffector(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext,
        IContext anotherContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, anotherContext) 
    { }

    public override void SafeReset()
    {
        LoadSceneBoundsFromFile(); 

        // Initialiser la position du joueur
        lastPosition = GetPlayerPosition();
    }

    private void LoadSceneBoundsFromFile()
    {
        if (File.Exists(sceneBoundsFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(sceneBoundsFilePath);
                if (lines.Length > 0)
                {
                    string[] values = lines[0].Split(','); // Supposons un format "x,y,z"
                    if (values.Length == 3)
                    {
                        float x = float.Parse(values[0]);
                        float y = float.Parse(values[1]);
                        float z = float.Parse(values[2]);

                        sceneBounds = new Bounds(Vector3.zero, new Vector3(x, y, z));
                        Debug.Log($"SceneBounds chargés : {sceneBounds.size}");
                    }
                    else
                    {
                        Debug.LogError("Format invalide dans sceneBounds.txt !");
                    }
                }
                else
                {
                    Debug.LogError("sceneBounds.txt est vide !");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Erreur lors de la lecture de sceneBounds.txt : {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Fichier non trouvé : {sceneBoundsFilePath}");
        }
    }

    public override void SafeEffectorUpdate()
    {
        Vector3 currentPosition = GetPlayerPosition();
        Debug.Log("TestGenerated - OutsideSceneTeleportation");
        Debug.Log("SceneBounds : " + sceneBounds.extents);
        // Vérifier si le joueur est en dehors des bornes
        if (currentPosition.x < -250.00 || currentPosition.x > 250.00 ||
            currentPosition.y < -250.00 || currentPosition.y > 250.00 ||
            currentPosition.z < -250.00 || currentPosition.z > 250.00)
        {
            Debug.LogError("ORACLE OutsideSceneTeleportation - TestFailed - A player cannot teleport outside the scene at position: " + currentPosition);
        }
        else
        {
            Debug.Log("ORACLE OutsideSceneTeleportation - TestPassed - A player can teleport inside the scene at position: " + currentPosition);
        }
    }

    private Vector3 GetPlayerPosition()
    {
        return gameObjectToObserve.transform.position;
    }
}
