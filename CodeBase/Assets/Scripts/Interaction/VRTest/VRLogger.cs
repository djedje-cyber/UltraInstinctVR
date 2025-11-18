using System.IO;
using System;
using UnityEngine;




/// <summary>
/// Class <c>VRLogger</c> handles logging operations for VR interactions in Unity.
/// </summary>
public class VRLogger
{
    private string logFilePath;




    /// <summary>
    /// Method <c>ClearFoundObjectsFile</c> clears the contents of the FoundObject.txt file.
    /// </summary>
    public void ClearFoundObjectsFile()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        logFilePath = Path.Combine(projectRoot, "Logs", "game_logs.txt");

        FileStream filePath = File.Create(logFilePath);
        StreamWriter writer = new StreamWriter(filePath);
        Debug.Log("The FoundObject.txt file has been written.");
   
       

    }
}
