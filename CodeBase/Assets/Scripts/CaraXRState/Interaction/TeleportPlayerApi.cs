using System;
using System.IO;
using UnityEngine;

public class TeleportPlayerAPI : MonoBehaviour
{
    [SerializeField]
    private Vector2 teleportRange = new Vector2(10f, 10f);

    private string logFilePath;

    private void Awake()
    {
        InitializeLogFile();
    }

    private void InitializeLogFile()
    {
        string uuid = Guid.NewGuid().ToString();
        string date = DateTime.Now.ToString("yyyy-MM-dd");

        string folderPath = "Logs/TESTREPLAY";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        logFilePath = Path.Combine(
            folderPath,
            $"TESTREPLAY_TeleportPlayer_{uuid}_{date}.txt"
        );
    }

    /// <summary>
    /// Teleports to a specific position (BEST for tests / Action()).
    /// </summary>
    public void TeleportTo(Vector3 position)
    {
        ApplyTeleport(position);
    }

    /// <summary>
    /// Teleports randomly once (optional utility).
    /// </summary>
    public void TeleportRandom()
    {
        Vector3 position = new Vector3(
            UnityEngine.Random.Range(-teleportRange.x, teleportRange.x),
            transform.position.y,
            UnityEngine.Random.Range(-teleportRange.y, teleportRange.y)
        );

        ApplyTeleport(position);
    }

    /// <summary>
    /// Teleports multiple times (useful for stress tests, NOT unit tests).
    /// </summary>
    public void TeleportSequence(int count, float delaySeconds)
    {
        StartCoroutine(TeleportRoutine(count, delaySeconds));
    }

    private System.Collections.IEnumerator TeleportRoutine(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            TeleportRandom();
            yield return new WaitForSeconds(delay);
        }
    }

    private void ApplyTeleport(Vector3 position)
    {
        Debug.Log($"Teleporting to: {position}");

        transform.position = position;

        LogTeleportation(position);
    }

    private void LogTeleportation(Vector3 position)
    {
        if (string.IsNullOrEmpty(logFilePath))
            InitializeLogFile();

        string logEntry = $"{position.x}, {position.y}, {position.z}\n";
        File.AppendAllText(logFilePath, logEntry);
    }
}