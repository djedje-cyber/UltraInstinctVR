#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class LogErrorListener : MonoBehaviour
{
    private int errorCount = 0;
    private float timer = 0f;
    private bool errorTriggered = false;
    public float maxTime = 20f * 60f;

    private float startTime;

    public string filePath = "Assets/Scripts/game_logs.txt";

    void Start()
    {
        var gameManager = FindObjectOfType<GetStartTime>();
        if (gameManager != null)
        {
            startTime = gameManager.GetStartTimeValue();
            Debug.Log("StartTime récupéré depuis GameManager: " + startTime);
        }
        else
        {
            Debug.LogWarning("GameManager non trouvé. Le startTime sera considéré comme 0.");
            startTime = 0f;
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        if (!errorTriggered)
        {
            timer += Time.deltaTime;

            if (timer >= maxTime)
            {
                Debug.LogWarning("20 minutes écoulées sans 15 erreurs. L'application va se fermer.");
                Debug.Log("ACTION_DONE");
                StopApplication();
            }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            errorCount++;
            Debug.Log("Erreur détectée. Total : " + errorCount);

            if (errorCount >= 15 && !errorTriggered)
            {
                errorTriggered = true;
                Debug.LogError("15 erreurs détectées.");

                // 🔴 1. Stopper le script LogToFile s’il est présent
                LogToFile logToFile = GetComponent<LogToFile>();
                if (logToFile != null)
                {
                    logToFile.enabled = false;
                    Debug.Log("Script LogToFile désactivé.");
                }

                // 🔵 2. Génération du rapport CSV
                Debug.Log("Génération du rapport CSV...");
                AnalyzeLogs(isTimeout: false);

                // 🔵 3. Lancement du délai avant fermeture
                StartCoroutine(QuitAfterDelay(0f));
            }
        }
    }

    IEnumerator QuitAfterDelay(float delay)
    {
        Debug.Log("Attente de " + delay + " secondes avant de fermer l'application...");
        yield return new WaitForSeconds(delay);
        Debug.Log("Fermeture de l'application...");
        Debug.Log("ACTION_DONE");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void StopApplication()
    {
        AnalyzeLogs(isTimeout: true);
        StartCoroutine(QuitAfterDelay(0f));
    }

    public void AnalyzeLogs(bool isTimeout = false)
    {
        float elapsedTime = Time.time - startTime;

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
            sw.WriteLine("Execution Time");

            if (isTimeout)
            {
                sw.WriteLine("TIMEOUT");
            }
            else
            {
                sw.WriteLine($"{elapsedTime:F2}");
            }
        }

        Debug.Log($"Le rapport CSV a été généré avec succès : {fullFilePath}");
    }

    string GenerateFileName()
    {
        string uuid = Guid.NewGuid().ToString().Substring(0, 5);
        string date = DateTime.Now.ToString("ddMMyyyy");
        return $"result_{uuid}_{date}.csv";
    }
}
