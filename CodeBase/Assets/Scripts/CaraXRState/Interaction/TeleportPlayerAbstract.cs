using System;
using System.Collections;
using System.IO;
using UnityEngine;




/// <summary>
/// Class <c>TeleportPlayer</c> allows to teleport the player to random positions within a defined range multiple times,
/// </summary>


public abstract class TeleportPlayerAbstract 
{

    [SerializeField]
    public  int teleportCount; // Number of teleportation
    [SerializeField]
    public Vector2 teleportRange; // Teleportation range on X and Z axes
    [SerializeField]
    public  float delayBetweenTeleports; // Delay between each teleportation in seconds

    private string logFilePath;



    public void utils()
    {
        
        // Generate unique filename with UUID and current date for logging teleportation positions to allow replaying later
        string uuid = Guid.NewGuid().ToString();
        string Date = DateTime.Now.ToString("yyyy-MM-dd");

        string folderPath =  "Logs/TESTREPLAY";
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        logFilePath = Path.Combine(folderPath, $"TESTREPLAY_TeleportPlayer_{uuid}_{Date}.txt");
        
    }

    /// <summary>
    /// Method <c>TeleportRoutine</c> handles the teleportation process, teleporting the player multiple times with a delay in between.
    /// </summary>
    /// <returns>Brake between teleportation</returns>
    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            TeleportStrategy();
            yield return new WaitForSeconds(delayBetweenTeleports); // Brake between teleports
        }
    }



    /// <summary>
    /// Method <c>TeleportToRandomPosition</c> teleports the player to a random position within the defined range.
    /// </summary>
    public virtual void TeleportStrategy()
    {

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
