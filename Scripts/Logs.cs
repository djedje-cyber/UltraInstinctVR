using System;
using System.IO;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        // Définir le chemin du fichier log (dans Assets/Scripts/)
        logFilePath = Path.Combine(Application.dataPath, "Scripts", "game_logs.txt");

        // Vérifier si le dossier existe, sinon le créer
        string directoryPath = Path.GetDirectoryName(logFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Vérifier si le fichier existe, sinon le créer
        if (!File.Exists(logFilePath))
        {
            using (StreamWriter writer = File.CreateText(logFilePath))
            {
                writer.WriteLine("=== Nouvelle Session ===");
            }
        }
        else
        {
            File.AppendAllText(logFilePath, "\n=== Nouvelle Session ===\n");
        }

        // Écouter les logs de la console
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
