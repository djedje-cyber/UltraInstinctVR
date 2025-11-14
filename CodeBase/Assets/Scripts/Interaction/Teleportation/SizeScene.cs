using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;



/// <summary>
/// Class <c>SizeScene</c> teleports an object out of the scene bounds multiple times and logs the positions to a file.
/// </summary>



public class SizeScene : MonoBehaviour
{
    [Header("Teleportation Settings")]
    public int teleportCount = 100;
    public float outOfBoundsMultiplier = 1000f;
    public float delayBetweenTeleports = 0.1f;
    public GameObject objectToTeleport;

    [Header("Scene Bounds")]
    public Bounds sceneBounds;

    [Header("Replay Logging")]
    public string replayFolder = "Assets/Scripts/TESTREPLAY";

    private string replayFilePath;
    private List<string> teleportLogs = new List<string>();



    /// <summary>
    /// Method <c>Start</c> initializes the log file, calculates scene bounds, and starts the teleportation coroutine.
    /// </summary>
    private void Start()
    {
        // Initialize the log file
        InitializeLogFile();

        // Calculate the scene bounds before starting teleportations
        StartCoroutine(SetupAndTeleport());
    }

    private IEnumerator SetupAndTeleport()
    {
        yield return StartCoroutine(CalculateSceneBoundsDeferred());

        yield return StartCoroutine(TeleportOutOfBounds());

        // Write the log after all teleportations
        WriteLogsToFile();
    }



    /// <summary>
    /// Method <c>CalculateSceneBoundsDeferred</c> waits for the end of the frame before calculating scene bounds.
    /// </summary>
    /// <returns>CalculateSceneBounds()</returns>
    private IEnumerator CalculateSceneBoundsDeferred()
    {
        yield return new WaitForEndOfFrame();
        CalculateSceneBounds();
    }



    /// <summary>
    /// Method <c>CalculateSceneBounds</c> calculates the bounds of all renderers in the scene.
    /// </summary>
    private void CalculateSceneBounds()
    {
        sceneBounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            sceneBounds.Encapsulate(renderer.bounds);
        }
    }

    /// <summary>
    /// Method <c>TeleportOutOfBounds</c> teleports the object out of bounds multiple times and logs the positions.
    /// </summary>
    /// <returns> Break the program to let the game object to teleport</returns>

    private IEnumerator TeleportOutOfBounds()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            Vector3 newPos = CalculateRandomOutOfBoundsPosition();
            TeleportObject(newPos);
            LogTeleport(newPos);

            yield return new WaitForSeconds(delayBetweenTeleports);
        }
    }

    /// <summary>
    /// Method <c>CalculateRandomOutOfBoundsPosition</c> calculates a random position outside the scene bounds.
    /// </summary>
    /// <returns>The sceneBounds</returns>
    private Vector3 CalculateRandomOutOfBoundsPosition()
    {
        Vector3 randomDir = UnityEngine.Random.onUnitSphere;
        randomDir.y = 0f; // Keep on the horizontal plane
        float distance = sceneBounds.size.magnitude * outOfBoundsMultiplier;
        return sceneBounds.center + randomDir * distance;
    }


    /// <summary>
    /// Method <c>TeleportObject</c> teleports the specified object to the new position.
    /// </summary>
    /// <param name="newPosition"></param>
    private void TeleportObject(Vector3 newPosition)
    {
        if (objectToTeleport != null)
        {
            objectToTeleport.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Object to teleport is not assigned!");
        }
    }


    /// <summary>
    /// Method <c>LogTeleport</c> logs the teleportation position to the list.
    /// </summary>
    /// <param name="position"></param>
    private void LogTeleport(Vector3 position)
    {
        teleportLogs.Add($"{position.x}, {position.y}, {position.z}");
    }


    /// <summary>
    /// Method <c>WriteLogsToFile</c> writes the teleportation logs to the replay file.
    /// </summary>
    private void WriteLogsToFile()
    {
        File.WriteAllLines(replayFilePath, teleportLogs);
        Debug.Log($"Teleport log written to: {replayFilePath}");
    }



    /// <summary>
    /// Method <c>InitializeLogFile</c> initializes the replay log file with a unique name.
    /// </summary>
    private void InitializeLogFile()
    {
        if (!Directory.Exists(replayFolder))
            Directory.CreateDirectory(replayFolder);

        string uuid = Guid.NewGuid().ToString();
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        replayFilePath = Path.Combine(replayFolder, $"TESTREPLAY_SizeScene_{uuid}_{date}.txt");

        // Create and clear the file
        File.WriteAllText(replayFilePath, string.Empty);
    }
}
