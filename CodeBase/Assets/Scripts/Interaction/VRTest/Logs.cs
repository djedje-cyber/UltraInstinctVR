using System;
using System.IO;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        // Define the path of the log file (inside Assets/Scripts/)
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;

        logFilePath = Path.Combine(projectRoot, "Logs", "game_logs.txt");

        // Check if the folder exists, otherwise create it
        string directoryPath = Path.GetDirectoryName(logFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // If the file already exists, clear it to start a new session
        if (File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, string.Empty);  // Clears the file
        }
        else
        {
            // If the file does not exist, create it with an initial message
            using (StreamWriter writer = File.CreateText(logFilePath))
            {
                writer.WriteLine("=== New Session ===");
            }
        }

        // Add a new entry at the beginning of the file
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine("\n=== New Session ===");
        }

        // Listen to Unity console logs
        Application.logMessageReceived += LogToFileMethod;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= LogToFileMethod;
    }

    private void LogToFileMethod(string logString, string stackTrace, LogType type)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"[{DateTime.Now}] {type}: {logString}");
            if (type == LogType.Exception || type == LogType.Error)
            {
                writer.WriteLine(stackTrace);
            }
        }
    }
}
