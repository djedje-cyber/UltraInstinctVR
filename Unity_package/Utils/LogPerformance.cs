using System;
using System.IO;
using UnityEngine;

public class LogPerformance : MonoBehaviour
{
    private string logFilePath;
    private int errorCount = 0;
    private float startTime;

    void Awake()
    {
        // Initialize startTime
        startTime = Time.time;

        // Define the path to the "Logs/result" folder at project root
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string resultDirectory = Path.Combine(projectRoot, "Logs", "result");

        // Check if the folder exists, otherwise create it
        if (!Directory.Exists(resultDirectory))
        {
            Directory.CreateDirectory(resultDirectory);
        }

        // Generate the CSV file name
        string fileName = GenerateFileName();
        logFilePath = Path.Combine(resultDirectory, fileName);

        // Create or clear the CSV file with new header
        using (StreamWriter writer = new StreamWriter(logFilePath, false))
        {
            writer.WriteLine("ERROR_NUMBER, ERROR, TIMESTAMP (s), STACKTRACE");
        }

        // Listen to Unity log messages
        Application.logMessageReceived += LogToCsvMethod;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= LogToCsvMethod;
    }

    private void LogToCsvMethod(string logString, string stackTrace, LogType type)
    {
        // Log only errors, exceptions or asserts
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert
            || (type == LogType.Log && logString.Contains("NullReferenceException")))
        {
            // Optional: prefix for NullReferenceException logs
            if (logString.Contains("NullReferenceException"))
            {
                logString = "NullReferenceException: " + logString;
            }

            // Increment error number and calculate timestamp
            errorCount++;
            float timestamp = Time.time - startTime; // Timestamp in seconds

            // Format the error message and stacktrace for CSV
            string errorMessage = logString.Replace(",", " ");
            string stack = stackTrace.Replace(",", " ").Replace("\n", " ").Replace("\r", " ");

            // Write the error to the CSV file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{errorCount}, \"{errorMessage}\", {timestamp:F2}, \"{stack}\"");
            }
        }
    }

    string GenerateFileName()
    {
        string uuid = Guid.NewGuid().ToString().Substring(0, 5);  
        string date = DateTime.Now.ToString("ddMMyyyy");          
        return $"result_{uuid}_{date}.csv";                        
    }
}
