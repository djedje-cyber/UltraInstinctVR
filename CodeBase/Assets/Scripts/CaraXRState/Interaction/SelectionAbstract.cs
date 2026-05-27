using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// Class <c>Teleport</c>> teleports an object to a series of positions read from a text file.
/// </summary>
public abstract class SelectionAbstract : MonoBehaviour
{
    // List of positions to reach
    private List<Vector3> positions = new List<Vector3>();
    public XRInteractionManager interactionManager;

    // Current index of the position where the object should teleport
    private int currentPositionIndex = 0;

    // Waiting time between each teleportation (in seconds)
    [SerializeField]
    private float teleportDelay = 2f;

    [SerializeField]
    private string filePath = "Logs/FoundObject.txt";


    /// <summary>
    /// Method <c>ReadPositionsFromFile</c> reads positions from a specified text file and stores them in a list.
    /// </summary>
    private void ReadPositionsFromFile(GameObject target)
    {
        List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable> interactables = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>();
        interactionManager.GetRegisteredInteractables(interactables);

        positions.Clear();

        foreach (var interactable in interactables)
        {
            if (interactable.transform.gameObject == target)
            {
                positions.Add(interactable.transform.position);
                break; // No need to continue, target found
            }
        }
    }

 
    /// <summary>
    /// Method <c>TeleportToNextPosition</c> teleports the object to each position in the list with a delay in between.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TeleportToNextPosition()
    {
        // While there are still positions to reach
        while (currentPositionIndex < positions.Count)
        {
            transform.position = positions[currentPositionIndex];

            // Wait before teleporting to the next one
            yield return new WaitForSeconds(teleportDelay);

            // Move to the next position
            currentPositionIndex++;
        }

        // All positions reached
        Debug.Log("All positions have been reached!");
    }
}
