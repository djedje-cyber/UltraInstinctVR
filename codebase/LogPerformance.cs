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
        // Initialiser le startTime
        startTime = Time.time;

        // Définir le chemin du dossier "Assets/result"
        string resultDirectory = Path.Combine(Application.dataPath, "result");
        
        // Vérifier si le dossier existe, sinon le créer
        if (!Directory.Exists(resultDirectory))
        {
            Directory.CreateDirectory(resultDirectory);
        }

        // Générer le nom du fichier CSV
        string fileName = GenerateFileName();
        logFilePath = Path.Combine(resultDirectory, fileName);

        // Créer ou vider le fichier CSV
        using (StreamWriter writer = new StreamWriter(logFilePath, false))
        {
            writer.WriteLine("ERROR_NUMBER, ERROR, TIMESTAMP (s)");
        }

        // Écouter les logs de la console
        Application.logMessageReceived += LogToCsvMethod;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= LogToCsvMethod;
    }

    private void LogToCsvMethod(string logString, string stackTrace, LogType type)
    {
        // Enregistrer uniquement les erreurs ou exceptions
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            // Vérifier si le message d'erreur contient "NullReferenceException"
            if (logString.Contains("NullReferenceException"))
            {
                logString = "NullReferenceException: " + logString;
            }

            // Incrémenter le numéro d'erreur et calculer le timestamp
            errorCount++; // Incrémenter le numéro d'erreur
            float timestamp = Time.time - startTime; // Calculer le timestamp en secondes

            // Créer un message d'erreur formaté
            string errorMessage = logString.Replace(",", " "); // Remplacer les virgules pour ne pas casser le CSV

            // Enregistrer l'erreur dans le fichier CSV
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{errorCount}, \"{errorMessage}\", {timestamp:F2}");
            }
        }
        else if (type == LogType.Log && logString.Contains("NullReferenceException"))
        {
            errorCount++; // Incrémenter le numéro d'erreur
            float timestamp = Time.time - startTime; // Calculer le timestamp en secondes

            // Créer un message d'erreur formaté
            string errorMessage = logString.Replace(",", " "); // Remplacer les virgules pour ne pas casser le CSV

            // Enregistrer l'erreur dans le fichier CSV
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{errorCount}, \"{errorMessage}\", {timestamp:F2}");
            }
        }
    }

    string GenerateFileName()
    {
        string uuid = Guid.NewGuid().ToString().Substring(0, 5);  // UUID de 5 caractères
        string date = DateTime.Now.ToString("ddMMyyyy");  // Date au format DDMMYYYY
        return $"result_{uuid}_{date}.csv";  // Nom du fichier CSV
    }
}
