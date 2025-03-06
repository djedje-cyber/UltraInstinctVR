using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine.XR.Interaction.Toolkit;

public class Collision: MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    public string targetTag = "Interactable";  // Les objets attrapables doivent avoir ce tag
    public string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt";  // Fichier contenant les positions
    private List<Vector3> positions = new List<Vector3>();
    private int currentIndex = 0;
    public float moveSpeed = 1.5f; // Vitesse de déplacement
    public float grabDelay = 1.0f; // Temps avant d’attraper l'objet

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbedObject = null;

    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor == null)
        {
            Debug.LogError("❌ XRDirectInteractor non trouvé sur " + gameObject.name);
            return;
        }

        ReadPositionsFromFile();
        if (positions.Count > 0)
        {
            StartCoroutine(GrabAndMoveObjects());
        }
        else
        {
            Debug.LogError("📄 Aucun objet détecté dans FoundObject.txt !");
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

    IEnumerator GrabAndMoveObjects()
    {
        while (currentIndex < positions.Count)
        {
            Vector3 grabPosition = positions[currentIndex];

            Debug.Log("🚀 Recherche d'un objet à attraper à : " + grabPosition);
            yield return MoveToPosition(grabPosition); // Déplacer la main vers la position d'un objet

            yield return new WaitForSeconds(grabDelay);

            TryGrabObject(); // Attraper un objet

            if (grabbedObject != null)
            {
                currentIndex++;
                if (currentIndex < positions.Count)
                {
                    Vector3 moveToPosition = positions[currentIndex];
                    Debug.Log("✋ Déplacement de l'objet vers : " + moveToPosition);
                    yield return MoveObjectToPosition(moveToPosition); // Déplacer l'objet
                }

                ReleaseObject(); // Relâcher l'objet
            }

            yield return new WaitForSeconds(2f);
            currentIndex++;
        }

        Debug.Log("✅ Tous les objets ont été déplacés.");
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void TryGrabObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (Collider col in colliders)
        {
             UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable = col.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (interactable != null && col.CompareTag(targetTag))
            {
                Debug.Log("🤖 Objet attrapé : " + col.name);
                interactor.StartManualInteraction(interactable as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable);
                grabbedObject = interactable;
                return;
            }
        }

        Debug.Log("❌ Aucun objet interactif trouvé à cette position.");
    }

    IEnumerator MoveObjectToPosition(Vector3 targetPos)
    {
        while (grabbedObject != null && Vector3.Distance(grabbedObject.transform.position, targetPos) > 0.05f)
        {
            grabbedObject.transform.position = Vector3.MoveTowards(grabbedObject.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            Debug.Log("🛑 Objet relâché : " + grabbedObject.name);
            interactor.EndManualInteraction();
            grabbedObject = null;
        }
    }
}
