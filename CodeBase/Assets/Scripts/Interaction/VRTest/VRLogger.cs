using System.IO;
using UnityEngine;




/// <summary>
/// Class <c>VRLogger</c> handles logging operations for VR interactions in Unity.
/// </summary>
public class VRLogger
{
    private string folderPath = "Assets/Scripts/CoveredObjects";



    /// <summary>
    /// Method <c>ClearFoundObjectsFile</c> clears the contents of the FoundObject.txt file.
    /// </summary>
    public void ClearFoundObjectsFile()
    {
        string filePath = Path.Combine(folderPath, "FoundObject.txt");
        if (File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath, false)) { }
            Debug.Log("The FoundObject.txt file has been emptied.");
        }
        else
        {
            Debug.LogWarning("The FoundObject.txt file does not exist");
        }
    }
}
