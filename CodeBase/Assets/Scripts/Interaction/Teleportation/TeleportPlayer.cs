using System;
using System.Collections;
using System.IO;
using UnityEngine;




/// <summary>
/// Class <c>TeleportPlayer</c> allows to teleport the player to random positions within a defined range multiple times,
/// </summary>


public class TeleportPlayer : MonoBehaviour
{

    [SerializeField]
    private int teleportCount = 10; // Number of teleportation
    [SerializeField]
    private Vector2 teleportRange = new Vector2(10f, 10f); // Teleportation range on X and Z axes
    [SerializeField]
    private float delayBetweenTeleports = 0.1f; // Delay between each teleportation in seconds

    private string logFilePath;



    /// <summary>
    /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {

        // Generate unique filename with UUID and current date for logging teleportation positions to allow replaying later
        string uuid = Guid.NewGuid().ToString();
        string Date = DateTime.Now.ToString("yyyy-MM-dd");

        string folderPath = Path.Combine(Application.dataPath, "/TESTREPLAY");
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        logFilePath = Path.Combine(folderPath, $"TESTREPLAY_TeleportPlayer_{uuid}_{Date}.txt");
        
        Debug.Log("🚀 Téléportation of the player");
        StartCoroutine(TeleportRoutine());
    }


    /// <summary>
    /// Method <c>TeleportRoutine</c> handles the teleportation process, teleporting the player multiple times with a delay in between.
    /// </summary>
    /// <returns>Brake between teleportation</returns>
    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            TeleportToRandomPosition();
            yield return new WaitForSeconds(delayBetweenTeleports); // Brake between teleports
        }
    }



    /// <summary>
    /// Method <c>TeleportToRandomPosition</c> teleports the player to a random position within the defined range.
    /// </summary>
    private void TeleportToRandomPosition()
    {
        float randomX = UnityEngine.Random.Range(-teleportRange.x, teleportRange.x);
        float randomZ = UnityEngine.Random.Range(-teleportRange.y, teleportRange.y);
        Vector3 newPosition = new Vector3(randomX, transform.position.y, randomZ);
        
        Debug.Log("Trying to teleport at : " + newPosition);
        LogTeleportation(newPosition);
        transform.position = newPosition;
        
    }


    /// <summary>
    /// Method <c>LogTeleportation</c> logs the teleportation position to the predifined in Start() for later replay.
    /// </summary>
    /// <param name="position"></param>
    private void LogTeleportation(Vector3 position)
    {
        string logEntry = $"{position.x}, {position.y}, {position.z}\n";
        File.AppendAllText(logFilePath, logEntry);
    }
}
