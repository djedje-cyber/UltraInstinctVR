using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Globalization;

/// <summary>
/// Class <c>Teleport</c>> teleports an object to a series of positions read from a text file.
/// </summary>
public class TeleportAtObject : MonoBehaviour
{
    // List of positions to reach
    private List<Vector3> positions = new List<Vector3>();

    // Current index of the position where the object should teleport
    private int currentPositionIndex = 0;

    // Waiting time between each teleportation (in seconds)
    [SerializeField]
    private float teleportDelay = 2f;

    [SerializeField]
    private string filePath = "Logs/FoundObject.txt";


    /// <summary>
    /// Method <c>Start</c> initializes the teleportation process by reading positions from a file and starting the teleportation coroutine.
    /// </summary>
    void Start()
    {
        // Read positions from the file
        ReadPositionsFromFile();

        // Start teleporting if any positions were loaded
        if (positions.Count > 0)
        {
            StartCoroutine(TeleportToNextPosition());
        }
        else
        {
            Debug.LogWarning("No positions found in the file.");
        }
    }

    /// <summary>
    /// Method <c>ReadPositionsFromFile</c> reads positions from a specified text file and stores them in a list.
    /// </summary>
    private void ReadPositionsFromFile()
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
            ProcessLine(line, pattern);
        }

        Debug.Log("Positions read from file: " + positions.Count);
    }


    /// <summary>
    /// Method <c>ProcessLine</c> processes a single line from the file to extract coordinates and add them to the positions list.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="pattern"></param>

    private void ProcessLine(string line, string pattern)
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


    /// <summary>
    /// Method <c>TryAddPosition</c> attempts to parse coordinates and add them as a Vector3 to the positions list.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="line"></param>
    private void TryAddPosition(string[] coordinates, string line)
    {
        try
        {
            float x = float.Parse(coordinates[0].Trim(), CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(coordinates[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(coordinates[2].Trim(), CultureInfo.InvariantCulture.NumberFormat);

            positions.Add(new Vector3(x, y, z));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading coordinates on line: " + line + "\n" + e.Message);
        }
    }



    /// <summary>
    /// Method <c>TeleportToNextPosition</c> teleports the object to each position in the list with a delay in between.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TeleportToNextPosition()
    {
        // While there are still positions to reach
        while (currentPositionIndex < positions.Count)
        {
            transform.position = positions[currentPositionIndex];

            // Wait before teleporting to the next one
            yield return new WaitForSeconds(teleportDelay);

            // Move to the next position
            currentPositionIndex++;
        }

        // All positions reached
        Debug.Log("All positions have been reached!");
    }
}
