using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public int teleportCount = 10; // Nombre de téléportations
    public Vector2 teleportRange = new Vector2(10f, 10f); // Zone de téléportation (X, Z)
    public float delayBetweenTeleports = 0.1f; // Délai entre chaque téléportation (optionnel)

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
        
        Debug.Log("🚀 Téléportation du joueur...");
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            TeleportToRandomPosition();
            yield return new WaitForSeconds(delayBetweenTeleports); // Pause entre chaque téléportation
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
