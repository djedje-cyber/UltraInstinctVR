using System.Collections;
using UnityEngine;

public class SizeScene : MonoBehaviour
{
    public int teleportCount = 100; // Nombre de téléportations
    public float outOfBoundsMultiplier = 1.5f; // Distance pour être hors de la scène
    public float delayBetweenTeleports = 0.1f; // Délai optionnel entre les téléportations

    private Bounds sceneBounds;

    void Start()
    {
        CalculateSceneBounds();
        StartCoroutine(TeleportOutOfBounds());
    }

    private void CalculateSceneBounds()
    {
        sceneBounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (Renderer renderer in FindObjectsOfType<Renderer>())
        {
            sceneBounds.Encapsulate(renderer.bounds);
        }

        Debug.Log("Taille de la scène : " + sceneBounds.size);
        Debug.Log("Centre de la scène : " + sceneBounds.center);
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
        Vector3 randomDirection = Random.onUnitSphere; // Génère une direction aléatoire
        randomDirection.y = 0; // On reste sur le plan horizontal

        float distance = sceneBounds.size.magnitude * outOfBoundsMultiplier; // Distance en dehors de la scène
        Vector3 newPosition = sceneBounds.center + randomDirection * distance;

        transform.position = newPosition;
        Debug.Log("Téléportation en dehors : " + newPosition);
    }
}
