using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Globalization;


public class Collision : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    private Transform grabbedObject;
    private List<Vector3> positions = new List<Vector3>();
    private int currentPositionIndex = 0;
    public float teleportDelay = 2f;
    public string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt";
    public string targetTag = "Interactable";

    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur " + gameObject.name);
        }

        ReadPositionsFromFile();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbedObject == null) // Vérifie si aucun objet n'est déjà attrapé
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (interactable != null && other.CompareTag(targetTag))
            {
                Debug.Log("👋 Objet détecté et attrapé : " + other.name);
                StartGrab(interactable);
                StartCoroutine(TeleportWithObject()); // Démarre la téléportation après le grab
            }
        }
    }

    private void StartGrab(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectInteractable = interactable as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable;
        if (selectInteractable != null)
        {
            interactor.StartManualInteraction(selectInteractable);
            grabbedObject = interactable.transform;
        }
    }

    private void EndGrab()
    {
        if (grabbedObject != null)
        {
            interactor.EndManualInteraction();
            Debug.Log("🛑 Objet relâché après téléportation !");
            grabbedObject = null;
        }
    }

    private IEnumerator TeleportWithObject()
    {
        if (grabbedObject == null || positions.Count == 0) yield break;

        while (currentPositionIndex < positions.Count)
        {
            grabbedObject.position = positions[currentPositionIndex];
            Debug.Log("📍 Téléportation à : " + positions[currentPositionIndex]);
            yield return new WaitForSeconds(teleportDelay);
            currentPositionIndex++;
        }

        EndGrab(); // Relâcher l'objet après la dernière téléportation
    }

    private void ReadPositionsFromFile()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("🚨 Fichier de positions introuvable : " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        string pattern = @"\(([^)]+)\)";

        foreach (string line in lines)
        {
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                string[] coordinates = match.Groups[1].Value.Split(',');
                if (coordinates.Length == 3)
                {
                    try
                    {
                        float x = float.Parse(coordinates[0].Trim(), CultureInfo.InvariantCulture);
                        float y = float.Parse(coordinates[1].Trim(), CultureInfo.InvariantCulture);
                        float z = float.Parse(coordinates[2].Trim(), CultureInfo.InvariantCulture);
                        positions.Add(new Vector3(x, y, z));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("⚠️ Erreur de lecture : " + e.Message);
                    }
                }
            }
        }
    }
}
