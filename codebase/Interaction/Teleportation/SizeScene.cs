using System.Collections;
using UnityEngine;
using System.IO;
using System;



public class SizeScene : MonoBehaviour
{
    public int teleportCount = 100; // Nombre de téléportations
    public float outOfBoundsMultiplier = 1000; // Distance pour être hors de la scène
    public float delayBetweenTeleports = 0.1f; // Délai optionnel entre les téléportations
    private string replayFilePath; // Chemin du fichier de log
    public Bounds sceneBounds;


    public IEnumerator Execute()
    {
        // Implementation of the Execute method
        Debug.Log("Executing SizeScene script...");
        yield return null;
    }




    void Start()
    {

        InitializeLogFile();
        // Diffère le calcul des limites de la scène jusqu'à la fin du frame
        StartCoroutine(DelayCalculateSceneBounds());
        StartCoroutine(TeleportOutOfBounds());
    }


    private IEnumerator DelayCalculateSceneBounds()
    {
        // Attends la fin du frame pour être sûr que tous les objets sont initialisés
        yield return new WaitForEndOfFrame();

        // Maintenant on peut calculer les limites de la scène après que tout soit initialisé
        CalculateSceneBounds();
    }

    private void CalculateSceneBounds()
    {
        sceneBounds = new Bounds(Vector3.zero, Vector3.zero);

        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            sceneBounds.Encapsulate(renderer.bounds);
        }

    }

    private IEnumerator TeleportOutOfBounds()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            TeleportToRandomOutOfBoundsPosition();
            yield return new WaitForSeconds(delayBetweenTeleports); // Pause entre chaque téléportation
        }
    }

    private void TeleportToRandomOutOfBoundsPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.onUnitSphere; // Génère une direction aléatoire
        randomDirection.y = 0; // On reste sur le plan horizontal

        float distance = sceneBounds.size.magnitude * outOfBoundsMultiplier; // Distance en dehors de la scène
        Vector3 newPosition = sceneBounds.center + randomDirection * distance;
        string logText = $"{newPosition.x}, {newPosition.y}, {newPosition.z}";
        File.AppendAllText(replayFilePath,logText + Environment.NewLine);
        transform.position = newPosition;
    }



    // Initialiser le fichier de log pour TESTREPLAY
    private void InitializeLogFile()
    {
        // Générer un UUID unique pour le fichier
        string uuid = Guid.NewGuid().ToString();
        string Date = DateTime.Now.ToString("yyyy-MM-dd");

        // Créer le chemin du fichier dans le dossier TESTREPLAY
        string replayFolderPath = "Assets/Scripts/TESTREPLAY";
        if (!Directory.Exists(replayFolderPath))
        {
            Directory.CreateDirectory(replayFolderPath);
        }

        // Générer le chemin du fichier
        replayFilePath = Path.Combine(replayFolderPath, $"TESTREPLAY_SizeScene_{uuid}_{Date}.txt");

        // Créer et vider le fichier si nécessaire
        File.WriteAllText(replayFilePath, "");
    }


}
