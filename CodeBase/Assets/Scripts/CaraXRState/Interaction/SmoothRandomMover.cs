using System.Collections;
using UnityEngine;

public class SmoothRandomMover : MonoBehaviour
{
    [SerializeField] public int teleportCount = 5;
    [SerializeField] public Vector2 teleportRange = new Vector2(5f, 5f);
    [SerializeField] public float delayBetweenTeleports = 1.0f;
    [SerializeField] public float moveSpeed = 1.0f;

    private void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            Vector3 destination = GetRandomPosition();
            yield return StartCoroutine(MoveToPosition(destination));
            yield return new WaitForSeconds(delayBetweenTeleports);
        }
    }

    private IEnumerator MoveToPosition(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = destination;
        Debug.Log($"[SmoothRandomMover] Reached {destination}");
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-teleportRange.x, teleportRange.x),
            transform.position.y,
            Random.Range(-teleportRange.y, teleportRange.y)
        );
    }
}