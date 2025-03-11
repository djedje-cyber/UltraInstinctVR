using System.Collections;
using UnityEngine;





public class SizeScene : MonoBehaviour
{
    public int teleportCount = 100; // Nombre de téléportations
    public float outOfBoundsMultiplier = 1000; // Distance pour être hors de la scène
    public float delayBetweenTeleports = 0.1f; // Délai optionnel entre les téléportations

    public Bounds sceneBounds;


    public IEnumerator Execute()
    {
        // Implementation of the Execute method
        Debug.Log("Executing SizeScene script...");
        yield return null;
    }




    void Start()
    {
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
        Vector3 randomDirection = Random.onUnitSphere; // Génère une direction aléatoire
        randomDirection.y = 0; // On reste sur le plan horizontal

        float distance = sceneBounds.size.magnitude * outOfBoundsMultiplier; // Distance en dehors de la scène
        Vector3 newPosition = sceneBounds.center + randomDirection * distance;

        transform.position = newPosition;
    }


}
