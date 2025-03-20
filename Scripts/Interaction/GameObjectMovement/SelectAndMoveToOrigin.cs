using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SelectAndMoveToOrigin : MonoBehaviour
{
    private GameObject virtualHand; 
    private List<Vector3> positions = new List<Vector3>();
    private int currentPositionIndex = 0; 

    public float teleportDelay = 2f; 
    public string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt"; 

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    public XRInteractionManager interactionManager;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable currentInteractable;

    void Start()
    {
        virtualHand = gameObject; 
        ReadPositionsFromFile();

        if (positions.Count > 0)
        {
            StartCoroutine(TeleportToNextPosition());
        }
        else
        {
            Debug.LogWarning("Aucune position trouvée dans le fichier.");
        }

        interactor = virtualHand.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur la main virtuelle !");
        }
    }

    private void ReadPositionsFromFile()
    {
        if (File.Exists(filePath))
        {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            Debug.Log("✋ Sélection automatique de : " + other.name);

            if (interactor != null && interactionManager != null)
            {
                var interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

                if (interactable != null)
                {
                    interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);
                    interactor.StartManualInteraction((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable); // 👈 Correction ici

                    Debug.Log("✅ Objet sélectionné et attrapé : " + other.name);

                    currentInteractable = interactable;
                    StartCoroutine(TeleportAndUnselect());
                }
                else
                {
                    Debug.LogWarning("⚠️ L'objet détecté n'est pas un interactable valide.");
                }
            }
            else
            {
                Debug.LogError("❌ L'interacteur ou l'InteractionManager n'est pas assigné !");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && currentInteractable != null && other.gameObject == currentInteractable.gameObject)
        {
            Debug.Log("🛑 Désélection automatique de : " + other.name);

            if (interactor != null && interactionManager != null)
            {
                interactor.EndManualInteraction(); // 👈 Relâcher l'objet
                interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)currentInteractable);
                currentInteractable = null;
            }
            else
            {
                Debug.LogWarning("❌ L'interacteur ou l'InteractionManager n'est pas assigné !");
            }
        }
    }

    private IEnumerator TeleportAndUnselect()
    {
        yield return new WaitForSeconds(1f);

        if (currentInteractable != null)
        {
            currentInteractable.transform.position = Vector3.zero;
            Debug.Log("🚀 Objet téléporté au point (0,0,0) : " + currentInteractable.name);

            yield return new WaitForSeconds(0.5f);

            if (interactionManager != null && interactor != null)
            {
                interactor.EndManualInteraction(); // 👈 Relâcher avant désélection
                interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)currentInteractable);
                Debug.Log("🛑 Objet désélectionné : " + currentInteractable.name);
                currentInteractable = null;
            }
        }
    }
}
