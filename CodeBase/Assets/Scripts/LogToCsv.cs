using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Globalization;
using GetStartTimeSpace;
public class LogToCsv : MonoBehaviour
{
    public string filePath = "Assets/Scripts/game_logs.txt";
    private GetStartTime gameManager;  // Référence vers GameManager
    private float startTime;

    void Start()
    {
        // Récupérer la référence du GameManager
        gameManager = UnityEngine.Object.FindFirstObjectByType<GetStartTime>();

        if (gameManager != null)
        {
            // Récupérer le startTime du GameManager
            startTime = gameManager.GetStartTimeValue();
            Debug.Log("StartTime récupéré depuis GameManager: " + startTime);
            AnalyzeLogs();
        }
        else
        {
            Debug.LogError("GameManager non trouvé !");
        }
    }

    public void AnalyzeLogs()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Le fichier game_logs.txt n'existe pas !");
            return;
        }

        int testCasesCount = 0;
        int testFailedCount = 0;
        int testPassedCount = 0;
        int errorCount = 0;
        int warningCount = 0;

        Dictionary<string, OracleDataCSV> oracleData = new Dictionary<string, OracleDataCSV>();

        foreach (string line in File.ReadLines(filePath))
        {
            if (line.Contains("TestGenerated")) testCasesCount++;
            if (line.Contains("TestFailed")) testFailedCount++;
            if (line.Contains("TestPassed")) testPassedCount++;
            if (ContainsError(line)) errorCount++;  // Vérifie spécifiquement "Error"
            if (line.Contains("Warning")) warningCount++;

            // Gestion des logs ORACLE
            if (line.Contains("ORACLE"))
            {
                Match match = Regex.Match(line, @"ORACLE\s+(\S+)");
                if (match.Success)
                {
                    string oracleName = match.Groups[1].Value;
                    if (!oracleData.ContainsKey(oracleName))
                        oracleData[oracleName] = new OracleDataCSV();

                    oracleData[oracleName].Count++;
                    if (line.Contains("TestFailed"))
                    {
                        oracleData[oracleName].Failed++;
                        oracleData[oracleName].LogsFailed.Add(line);
                    }
                    if (line.Contains("TestPassed"))
                    {
                        oracleData[oracleName].Passed++;
                        oracleData[oracleName].LogsPassed.Add(line);
                    }
                }
            }
        }

        // Calcul du temps d'exécution
        float elapsedTime = Time.time - startTime;

        // Crée le dossier 'Assets/result' s'il n'existe pas
        string resultDirectory = "Assets/result";
        if (!Directory.Exists(resultDirectory))
        {
            Directory.CreateDirectory(resultDirectory);
            Debug.Log("Dossier 'Assets/result' créé !");
        }

        string fileName = GenerateFileName();
        string fullFilePath = Path.Combine(resultDirectory, fileName);

        using (StreamWriter sw = new StreamWriter(fullFilePath))
        {
            // Écriture de l'en-tête CSV
            sw.WriteLine("Oracle, Instances, Test Passed, Test Failed, Logs Passed, Logs Failed");

            // Ajouter les résultats ORACLE dans le CSV
            foreach (var entry in oracleData)
            {
                sw.WriteLine($"{entry.Key}, {entry.Value.Count}, {entry.Value.Passed}, {entry.Value.Failed}, {entry.Value.LogsPassed.Count}, {entry.Value.LogsFailed.Count}");
            }

            // Ajouter un résumé des tests dans le CSV
            sw.WriteLine();
            sw.WriteLine($"Test Generated, {testCasesCount}");
            sw.WriteLine($"Test Failed, {testFailedCount}");
            sw.WriteLine($"Test Passed, {testPassedCount}");
            sw.WriteLine($"Error Logs, {errorCount}");
            sw.WriteLine($"Warning Logs, {warningCount}");

            // Ajouter le temps d'exécution dans le CSV
            sw.WriteLine($"Execution Time (s), {elapsedTime:F2}");
        }

        Debug.Log($"Le rapport CSV a été généré avec succès : {fullFilePath}");
    }

    string GenerateFileName()
    {
        string uuid = Guid.NewGuid().ToString().Substring(0, 5);  // UUID de 5 caractères
        string date = DateTime.Now.ToString("ddMMyyyy");  // Date au format DDMMYYYY
        return $"result_{uuid}_{date}.csv";
    }

    // Fonction pour vérifier uniquement le mot "Error"
    bool ContainsError(string line)
    {
        // Expression régulière qui correspond exactement au mot "Error" et ne capte pas "LogError" ou "ErrorMessage"
        return Regex.IsMatch(line, @"\bError\b", RegexOptions.IgnoreCase);
    }
}

public class OracleDataCSV
{
    public int Count { get; set; } = 0;
    public int Passed { get; set; } = 0;
    public int Failed { get; set; } = 0;
    public List<string> LogsPassed { get; set; } = new List<string>();
    public List<string> LogsFailed { get; set; } = new List<string>();
}
