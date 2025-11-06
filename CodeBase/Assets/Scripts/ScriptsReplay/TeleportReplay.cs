using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class TeleportReplay : MonoBehaviour
{
    public TextAsset positionFile; // Allows assignment of the position file in the inspector
    public float teleportDelay = 2f; // Time delay between teleports
    private List<Vector3> positions = new List<Vector3>();
    private int currentPositionIndex = 0;

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

    private void ReadPositionsFromFile()
    {
        if (positionFile == null)
        {
            Debug.LogError("❌ No position file assigned !");
            return;
        }

        string[] lines = positionFile.text.Split('\n'); // Read each line from the TextAsset

        foreach (string line in lines)
        {
            string cleanedLine = line.Trim();
            if (string.IsNullOrEmpty(cleanedLine)) continue; // Ignore empty lines

            // Regular expression to extract numbers (supports commas and decimal points)
            MatchCollection matches = Regex.Matches(cleanedLine, @"-?\d+([.,]\d+)?");

            if (matches.Count == 3) // Verify that we have 3 numbers (X, Y, Z)
            {
                try
                {
                    // Replaces commas with periods for float compatibility
                    string xStr = matches[0].Value.Replace(',', '.');
                    string yStr = matches[1].Value.Replace(',', '.');
                    string zStr = matches[2].Value.Replace(',', '.');

                    float x = float.Parse(xStr, CultureInfo.InvariantCulture);
                    float y = float.Parse(yStr, CultureInfo.InvariantCulture);
                    float z = float.Parse(zStr, CultureInfo.InvariantCulture);
                    positions.Add(new Vector3(x, y, z));

                    Debug.Log($"📌 Position Loaded : ({x}, {y}, {z})");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("❌ Error parsing line : " + cleanedLine + " - " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Incorect format : " + cleanedLine);
            }
        }

        Debug.Log($"✅ Loading terminated: {positions.Count} positions registered!");
    }



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
