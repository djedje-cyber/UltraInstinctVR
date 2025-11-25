using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;





/// <summary>
/// Class <c>TeleportReplay</c> reads a list of 3D positions from a text file and teleports the GameObject
/// </summary>
public class TeleportReplay : MonoBehaviour
{

    [SerializeField]
    private TextAsset positionFile; // Allows assignment of the position file in the inspector
    [SerializeField] 
    private float teleportDelay = 2f; // Time delay between teleports
    private readonly List<Vector3> positions = new List<Vector3>();
    private int currentPositionIndex = 0;



    /// <summary>
    /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        ReadPositionsFromFile();

        if (positions.Count > 0)
        {
            StartCoroutine(TeleportToNextPosition());
        }
        else
        {
            Debug.LogWarning("No position found in the file");
        }
    }



    /// <summary>
    /// Method <c>ReadPositionsFromFile</c> reads positions from the assigned text file and stores them in the positions list.
    /// </summary>
    private void ReadPositionsFromFile()
    {
        if (positionFile == null)
        {
            Debug.LogError("❌ No position file assigned!");
            return;
        }

        string[] lines = positionFile.text.Split('\n');
        positions.Clear();

        foreach (string line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
        {
            if (!TryParsePosition(line, out Vector3 position))
            {
                Debug.LogWarning("⚠️ Incorrect format: " + line);
                continue;
            }

            positions.Add(position);
            Debug.Log($"📌 Position Loaded: ({position.x}, {position.y}, {position.z})");
        }

        Debug.Log($"✅ Loading terminated: {positions.Count} positions registered!");
    }

    /// <summary>
    /// Tries to parse a line into a Vector3 position.
    /// Returns false if parsing fails or the format is invalid.
    /// </summary>
    private static bool TryParsePosition(string line, out Vector3 position)
    {
        position = default;

        // Match numbers (supports decimals and optional minus sign)
        var matches = Regex.Matches(line, @"-?\d+([.,]\d+)?");
        if (matches.Count != 3) return false;

        try
        {
            float x = float.Parse(matches[0].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
            float y = float.Parse(matches[1].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
            float z = float.Parse(matches[2].Value.Replace(',', '.'), CultureInfo.InvariantCulture);

            position = new Vector3(x, y, z);
            return true;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Teleports the object to the next position in the sequence, pausing for a specified delay between each teleport.
    /// </summary>
    /// <remarks>This method iterates through a predefined list of positions, updating the object's position
    /// to the next one in the sequence. The teleportation process pauses for the duration of the specified delay after
    /// each teleport. The method completes once all positions in the list have been visited.</remarks>
    /// <returns></returns>

    private IEnumerator TeleportToNextPosition()
    {
        while (currentPositionIndex < positions.Count)
        {
            transform.position = positions[currentPositionIndex];
            Debug.Log("Teleport at : " + positions[currentPositionIndex]);

            yield return new WaitForSeconds(teleportDelay);
            currentPositionIndex++;
        }

        Debug.Log("All positions has been reached !");
    }
}
