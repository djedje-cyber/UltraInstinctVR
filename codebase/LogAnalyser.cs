using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class LogAnalyzer : MonoBehaviour
{
    public string filePath = "Assets/Scripts/game_logs.txt";

    void Start()
    {
        AnalyzeLogs();
    }

    void AnalyzeLogs()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Le fichier game_logs.txt n'existe pas !");
            return;
        }

        int testCasesCount = 0;
        int testFailedCount = 0;
        int testPassedCount = 0;
        Dictionary<string, OracleData> oracleData = new Dictionary<string, OracleData>();

        foreach (string line in File.ReadLines(filePath))
        {
            if (line.Contains("TestGenerated")) testCasesCount++;
            if (line.Contains("TestFailed")) testFailedCount++;
            if (line.Contains("TestPassed")) testPassedCount++;

            if (line.Contains("ORACLE"))
            {
                Match match = Regex.Match(line, @"ORACLE\s+(\S+)");
                if (match.Success)
                {
                    string oracleName = match.Groups[1].Value;
                    if (!oracleData.ContainsKey(oracleName))
                        oracleData[oracleName] = new OracleData();

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

        StringBuilder htmlReport = new StringBuilder();
        htmlReport.AppendLine("<html><head><title>Unity Log Report</title>");
        htmlReport.AppendLine("<style>");
        htmlReport.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        htmlReport.AppendLine("h1 { color: #333; }");
        htmlReport.AppendLine("table { border-collapse: collapse; width: 100%; }");
        htmlReport.AppendLine("table, th, td { border: 1px solid black; }");
        htmlReport.AppendLine("th, td { padding: 10px; text-align: center; }");
        htmlReport.AppendLine("th { background-color: #f2f2f2; }");
        htmlReport.AppendLine(".log-box { background: #f8f8f8; padding: 10px; border-radius: 5px; margin: 5px 0; }");
        htmlReport.AppendLine(".log-passed { color: green; }");
        htmlReport.AppendLine(".log-failed { color: red; }");
        htmlReport.AppendLine("</style></head><body>");
        htmlReport.AppendLine("<h1>Unity Log Analysis Report</h1>");
        htmlReport.AppendLine("<h2>Summary</h2>");
        htmlReport.AppendLine($"<p><strong>Nombre de logs contenant 'TestGenerated':</strong> {testCasesCount}</p>");
        htmlReport.AppendLine($"<p><strong>Nombre de logs contenant 'TestFailed':</strong> {testFailedCount}</p>");
        htmlReport.AppendLine($"<p><strong>Nombre de logs contenant 'TestPassed':</strong> {testPassedCount}</p>");
        
        htmlReport.AppendLine("<h2>ORACLE Test Results</h2>");
        htmlReport.AppendLine("<table>");
        htmlReport.AppendLine("<tr><th>ORACLE</th><th>Instances</th><th>Test Passed</th><th>Test Failed</th></tr>");
        
        foreach (var entry in oracleData)
        {
            htmlReport.AppendLine($"<tr><td>{entry.Key}</td><td>{entry.Value.Count}</td><td>{entry.Value.Passed}</td><td>{entry.Value.Failed}</td></tr>");
        }
        htmlReport.AppendLine("</table>");
        
        foreach (var entry in oracleData)
        {
            htmlReport.AppendLine($"<h2>Logs pour ORACLE: {entry.Key}</h2>");
            if (entry.Value.LogsPassed.Count > 0)
            {
                htmlReport.AppendLine("<h3 style='color: green;'>Tests Réussis</h3>");
                foreach (string log in entry.Value.LogsPassed)
                {
                    htmlReport.AppendLine($"<div class='log-box log-passed'>{log}</div>");
                }
            }
            if (entry.Value.LogsFailed.Count > 0)
            {
                htmlReport.AppendLine("<h3 style='color: red;'>Tests Échoués</h3>");
                foreach (string log in entry.Value.LogsFailed)
                {
                    htmlReport.AppendLine($"<div class='log-box log-failed'>{log}</div>");
                }
            }
        }
        htmlReport.AppendLine("</body></html>");

        File.WriteAllText("report_HTML.html", htmlReport.ToString(), Encoding.UTF8);
        Debug.Log("Le rapport HTML a été généré avec succès : report_HTML.html");
    }
}

public class OracleData
{
    public int Count { get; set; } = 0;
    public int Passed { get; set; } = 0;
    public int Failed { get; set; } = 0;
    public List<string> LogsPassed { get; set; } = new List<string>();
    public List<string> LogsFailed { get; set; } = new List<string>();
}
