using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class AutoSelectOnCollision : MonoBehaviour
{


    // Liste des positions à atteindre
    private List<Vector3> positions = new List<Vector3>();

    // Index actuel de la position où l'objet doit se téléporter
    private int currentPositionIndex = 0;

    // Temps d'attente entre chaque téléportation (en secondes)
    public float teleportDelay = 2f;

    // Chemin du fichier contenant les positions
    public string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt";




    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    public XRInteractionManager interactionManager;

    void Start()
    {

        // Lire les positions depuis le fichier
        ReadPositionsFromFile();

        // Commencer la téléportation si des positions ont été lues
        if (positions.Count > 0)
        {
            StartCoroutine(TeleportToNextPosition());
        }
        else
        {
            Debug.LogWarning("Aucune position trouvée dans le fichier.");
        }
        



        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur " + gameObject.name);
        }
    }





    private void ReadPositionsFromFile()
    {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                // Expression régulière pour capturer les valeurs entre parenthèses
                string pattern = @"\(([^)]+)\)";

                foreach (string line in lines)
                {
                    // Chercher les coordonnées entre parenthèses
                    Match match = Regex.Match(line, pattern);

                    if (match.Success)
                    {
                        string positionData = match.Groups[1].Value; // Récupérer le contenu entre parenthèses
                        string[] coordinates = positionData.Split(',');

                        if (coordinates.Length == 3)
                        {
                            try
                            {
                                // Nettoyer les espaces avant et après chaque valeur de coordonnées
                                string xStr = coordinates[0].Trim();
                                string yStr = coordinates[1].Trim();
                                string zStr = coordinates[2].Trim();
                                float x = float.Parse(xStr, CultureInfo.InvariantCulture.NumberFormat);
                                float y = float.Parse(yStr, CultureInfo.InvariantCulture.NumberFormat);
                                float z = float.Parse(zStr, CultureInfo.InvariantCulture.NumberFormat);


                                // Ajouter la position à la liste
                                positions.Add(new Vector3(x, y, z));
                    
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError("Erreur lors de la lecture des coordonnées à la ligne : " + line + "\n" + e.Message);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("La ligne ne contient pas 3 coordonnées : " + line);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Pas de coordonnées entre parenthèses trouvées à la ligne : " + line);
                    }
                }

                Debug.Log("Positions lues depuis le fichier : " + positions.Count);
            }
            else
            {
                Debug.LogError("Le fichier de positions n'existe pas à l'emplacement : " + filePath);
            }
    }


        private IEnumerator TeleportToNextPosition()
        {
            // Tant qu'il y a des positions à atteindre
            while (currentPositionIndex < positions.Count)
            {
                transform.position = positions[currentPositionIndex];

                
                //Debug.Log("Téléportation à la position : " + positions[currentPositionIndex]);

                // Attendre avant de se téléporter à la suivante
                yield return new WaitForSeconds(teleportDelay);

                // Passer à la prochaine position
                currentPositionIndex++;
            }

            // Message lorsque toutes les positions ont été atteintes
            Debug.Log("Toutes les positions ont été atteintes !");
        }




    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>();

        if (interactable != null)
        {
            Debug.Log("✋ Sélection automatique de : " + other.name);
            interactionManager.SelectEnter(interactor, interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable>();

        if (interactable != null)
        {
            Debug.Log("🛑 Désélection automatique de : " + other.name);
            interactionManager.SelectExit(interactor, interactable);
        }
    }
}
