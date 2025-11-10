using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public int teleportCount = 10; // Number of teleportation
    public Vector2 teleportRange = new Vector2(10f, 10f); // Teleportation range on X and Z axes
    public float delayBetweenTeleports = 0.1f; // Delay between each teleportation in seconds

    private string logFilePath;

    private void Start()
    {
        string uuid = Guid.NewGuid().ToString();
        string Date = DateTime.Now.ToString("yyyy-MM-dd");

        string folderPath = Path.Combine(Application.dataPath, "Scripts/TESTREPLAY");
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        logFilePath = Path.Combine(folderPath, $"TESTREPLAY_TeleportPlayer_{uuid}_{Date}.txt");
        
        Debug.Log("🚀 Téléportation of the player");
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            TeleportToRandomPosition();
            yield return new WaitForSeconds(delayBetweenTeleports); // Brake between teleports
        }
    }

    private void TeleportToRandomPosition()
    {
        float randomX = UnityEngine.Random.Range(-teleportRange.x, teleportRange.x);
        float randomZ = UnityEngine.Random.Range(-teleportRange.y, teleportRange.y);
        Vector3 newPosition = new Vector3(randomX, transform.position.y, randomZ);
        
        Debug.Log("Trying to teleport at : " + newPosition);
        LogTeleportation(newPosition);
        transform.position = newPosition;
        
    }

    private void LogTeleportation(Vector3 position)
    {
        string logEntry = $"{position.x}, {position.y}, {position.z}\n";
        File.AppendAllText(logFilePath, logEntry);
    }
}
