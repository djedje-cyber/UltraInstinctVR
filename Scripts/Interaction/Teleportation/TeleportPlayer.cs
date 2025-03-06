using System.Collections;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public int teleportCount = 1000; // Nombre de téléportations
    public Vector2 teleportRange = new Vector2(10f, 10f); // Zone de téléportation (X, Z)
    public float delayBetweenTeleports = 0.1f; // Délai entre chaque téléportation (optionnel)

    private void Start()
    {
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            TeleportToRandomPosition();
            yield return new WaitForSeconds(delayBetweenTeleports); // Pause entre chaque téléportation
        }
    }

    private void TeleportToRandomPosition()
    {
        float randomX = Random.Range(-teleportRange.x, teleportRange.x);
        float randomZ = Random.Range(-teleportRange.y, teleportRange.y);
        Vector3 newPosition = new Vector3(randomX, transform.position.y, randomZ);
        transform.position = newPosition;
    }
}
