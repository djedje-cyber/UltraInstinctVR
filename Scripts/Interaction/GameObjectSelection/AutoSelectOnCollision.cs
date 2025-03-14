using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AutoSelectWithVirtualHand : MonoBehaviour
{
    private GameObject virtualHand; // La main virtuelle (boule)
    
    // Liste des positions à atteindre
    private List<Vector3> positions = new List<Vector3>();
    private int currentPositionIndex = 0; // Index actuel pour le déplacement

    public float teleportDelay = 2f; // Temps entre chaque téléportation
    public string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt"; // Fichier des positions

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    public XRInteractionManager interactionManager;

    void Start()
    {
        // Utiliser le gameObject actuel comme main virtuelle
        virtualHand = gameObject; // Le gameObject auquel le script est attaché

        // Lire les positions depuis le fichier
        ReadPositionsFromFile();

        // Lancer la téléportation si des positions sont trouvées
        if (positions.Count > 0)
        {
            StartCoroutine(TeleportToNextPosition());
        }
        else
        {
            Debug.LogWarning("Aucune position trouvée dans le fichier.");
        }

        // Récupérer le XRDirectInteractor du gameObject actuel
        interactor = virtualHand.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur la main virtuelle !");
        }
    }

    // Lire les positions depuis le fichier
    private void ReadPositionsFromFile()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            string pattern = @"\(([^)]+)\)"; // Regex pour extraire les coordonnées

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
                            Debug.LogError("Erreur lors de la lecture des coordonnées : " + e.Message);
                        }
                    }
                }
            }
            Debug.Log("Positions chargées : " + positions.Count);
        }
        else
        {
            Debug.LogError("Fichier non trouvé : " + filePath);
        }
    }

    // Déplacement de la main virtuelle vers les positions
    private IEnumerator TeleportToNextPosition()
    {
        while (currentPositionIndex < positions.Count)
        {
            virtualHand.transform.position = positions[currentPositionIndex];

            yield return new WaitForSeconds(teleportDelay);
            currentPositionIndex++;
        }
        Debug.Log("Toutes les positions ont été atteintes !");
    }

    // Sélection automatique des objets au contact
    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet qui entre en collision a un collider (pour détecter la présence de n'importe quel objet)
        if (other != null)
        {
            // Debug pour indiquer que l'objet a été sélectionné
            Debug.Log("✋ Sélection automatique de : " + other.name);

            // Utiliser l'InteractionManager pour sélectionner l'objet via l'interacteur
            if (interactor != null && interactionManager != null)
            {
                // Sélectionner l'objet avec l'XRDirectInteractor (pas de condition spécifique ici)
                interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>());
            }
            else
            {
                Debug.LogWarning("❌ L'interacteur ou l'InteractionManager n'est pas assigné !");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Vérifier si l'objet qui sort de la collision a un collider
        if (other != null)
        {
            // Debug pour indiquer que l'objet a été désélectionné
            Debug.Log("🛑 Désélection automatique de : " + other.name);

            // Utiliser l'InteractionManager pour désélectionner l'objet via l'interacteur
            if (interactor != null && interactionManager != null)
            {
                // Désélectionner l'objet avec l'XRDirectInteractor (pas de condition spécifique ici)
                interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>());
            }
            else
            {
                Debug.LogWarning("❌ L'interacteur ou l'InteractionManager n'est pas assigné !");
            }
        }
    }
}
