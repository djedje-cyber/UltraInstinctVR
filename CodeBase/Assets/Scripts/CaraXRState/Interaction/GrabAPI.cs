using System;
using System.IO;
using UnityEngine;

public class GrabAPI : MonoBehaviour
{

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
            $"TESTREPLAY_GrabAction_{uuid}_{date}.txt"
        );
    }

}
