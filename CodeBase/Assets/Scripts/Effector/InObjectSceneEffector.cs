using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

public class InObjectSceneEffector : AUnityEffector
{
    [ConfigurationParameter("Teleport Distance Threshold", Necessity.Required)]
    protected float teleportDistanceThreshold = 0.0f;

    [ConfigurationParameter("GameObjectToObserve", Necessity.Required)]
    protected GameObject gameObjectToObserve;


    private Vector3 lastPosition;

    private string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt";

    private List<Vector3> coveredObjectPositions = new List<Vector3>();  // Liste pour stocker les positions

    public InObjectSceneEffector(Xareus.Scenarios.Event @event, 
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext,
        IContext anotherContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext, anotherContext) 
    { }

    public override void SafeReset()
    {
        LoadCoveredObjectPositions();

        lastPosition = GetPlayerPosition();
    }

    public override void SafeEffectorUpdate()
    {
        Vector3 currentPosition = GetPlayerPosition();

        //Tolerance in position
        float tolerance = 4f;
        Debug.Log("TestGenerated - InObjectSceneTeleportation");
        bool flag = true;
        foreach (Vector3 coveredPosition in coveredObjectPositions)
        {
            float distance = Vector3.Distance(currentPosition, coveredPosition);
            // Si la distance entre la position actuelle du joueur et la position couverte est inférieure à la tolérance
            if (distance < tolerance)
            {
                Debug.LogError("ORACLE InObjectScene - TestFailed - Teleportation into covered object detected: " + currentPosition);
                flag = false;
                break;
            }
    
        }


        if(flag==true){
            Debug.Log("ORACLE InObjectScene - TestPassed - No teleportation into covered object detected"+ currentPosition);
        }
        flag=true;

    }


    private void LoadCoveredObjectPositions()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Expression régulière pour capturer les valeurs entre parenthèses
                string pattern = @"\(([^)]+)\)";

                foreach (string line in lines)
                {
                    // Chercher les coordonnées entre parenthèses
                    Match match = Regex.Match(line, pattern);

                    if (match.Success)
                    {
                        string positionData = match.Groups[1].Value; // Récupérer le contenu entre parenthèses
                        string[] coordinates = positionData.Split(',');

                        if (coordinates.Length == 3)
                        {
                            try
                            {
                                // Nettoyer les espaces avant et après chaque valeur de coordonnées
                                string xStr = coordinates[0].Trim();
                                string yStr = coordinates[1].Trim();
                                string zStr = coordinates[2].Trim();
                                float x = float.Parse(xStr, CultureInfo.InvariantCulture.NumberFormat);
                                float y = float.Parse(yStr, CultureInfo.InvariantCulture.NumberFormat);
                                float z = float.Parse(zStr, CultureInfo.InvariantCulture.NumberFormat);


                                
                                coveredObjectPositions.Add(new Vector3(x, y, z));
                    
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError("Erreur lors de la lecture des coordonnées à la ligne : " + line + "\n" + e.Message);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("La ligne ne contient pas 3 coordonnées : " + line);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Pas de coordonnées entre parenthèses trouvées à la ligne : " + line);
                    }
                }

            }
            else
            {
                Debug.LogError("Le fichier de positions n'existe pas à l'emplacement : " + filePath);
            }
    }

    private Vector3 GetPlayerPosition()
    {
        return gameObjectToObserve.transform.position;
    }
}
