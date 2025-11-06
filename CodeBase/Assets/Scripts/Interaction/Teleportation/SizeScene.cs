using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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

    private void Start()
    {
        // Initialize the log file
        InitializeLogFile();

        // Calculate the scene bounds before starting teleportations
        StartCoroutine(SetupAndTeleport());
    }

    // Main coroutine: calculates bounds and performs teleportation
    private IEnumerator SetupAndTeleport()
    {
        yield return StartCoroutine(CalculateSceneBoundsDeferred());

        yield return StartCoroutine(TeleportOutOfBounds());

        // Write the log after all teleportations
        WriteLogsToFile();
    }

    // Wait until the end of the frame to ensure all objects are initialized
    private IEnumerator CalculateSceneBoundsDeferred()
    {
        yield return new WaitForEndOfFrame();
        CalculateSceneBounds();
    }

    // Calculate the bounding box of the scene based on all renderers
    private void CalculateSceneBounds()
    {
        sceneBounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            sceneBounds.Encapsulate(renderer.bounds);
        }
    }

    // Coroutine to teleport the object out of bounds multiple times
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

    // Calculate a random position outside the scene bounds
    private Vector3 CalculateRandomOutOfBoundsPosition()
    {
        Vector3 randomDir = UnityEngine.Random.onUnitSphere;
        randomDir.y = 0f; // Keep on the horizontal plane
        float distance = sceneBounds.size.magnitude * outOfBoundsMultiplier;
        return sceneBounds.center + randomDir * distance;
    }

    // Move the object to the given position
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

    // Add the teleportation position to the log buffer
    private void LogTeleport(Vector3 position)
    {
        teleportLogs.Add($"{position.x}, {position.y}, {position.z}");
    }

    // Write all buffered teleport positions to the log file
    private void WriteLogsToFile()
    {
        File.WriteAllLines(replayFilePath, teleportLogs);
        Debug.Log($"Teleport log written to: {replayFilePath}");
    }

    // Initialize the log file with a unique filename
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
