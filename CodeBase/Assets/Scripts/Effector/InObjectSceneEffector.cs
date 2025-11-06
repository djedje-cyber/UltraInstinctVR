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

    private List<Vector3> coveredObjectPositions = new List<Vector3>();  // List to store positions

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

        // Tolerance in position
        float tolerance = 4f;
        Debug.Log("TestGenerated - InObjectSceneTeleportation");
        bool flag = true;
        foreach (Vector3 coveredPosition in coveredObjectPositions)
        {
            float distance = Vector3.Distance(currentPosition, coveredPosition);
            // If the distance between the current player position and the covered position is less than tolerance
            if (distance < tolerance)
            {
                Debug.LogError("ORACLE InObjectScene - TestFailed - Teleportation into covered object detected: " + currentPosition);
                flag = false;
                break;
            }
        }

        if (flag)
        {
            Debug.Log("ORACLE InObjectScene - TestPassed - No teleportation into covered object detected: " + currentPosition);
        }
        flag = true;
    }

    private void LoadCoveredObjectPositions()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Position file does not exist at path: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        string pattern = @"\(([^)]+)\)";

        foreach (string line in lines)
        {
            ParseAndAddPosition(line, pattern);
        }
    }

    private void ParseAndAddPosition(string line, string pattern)
    {
        Match match = Regex.Match(line, pattern);
        if (!match.Success)
        {
            Debug.LogWarning("No coordinates found in parentheses on line: " + line);
            return;
        }

        string[] coordinates = match.Groups[1].Value.Split(',');
        if (coordinates.Length != 3)
        {
            Debug.LogWarning("Line does not contain 3 coordinates: " + line);
            return;
        }

        TryAddPosition(coordinates, line);
    }

    private void TryAddPosition(string[] coordinates, string line)
    {
        try
        {
            float x = float.Parse(coordinates[0].Trim(), CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(coordinates[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(coordinates[2].Trim(), CultureInfo.InvariantCulture.NumberFormat);

            coveredObjectPositions.Add(new Vector3(x, y, z));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading coordinates on line: " + line + "\n" + e.Message);
        }
    }


    private Vector3 GetPlayerPosition()
    {
        return gameObjectToObserve.transform.position;
    }
}