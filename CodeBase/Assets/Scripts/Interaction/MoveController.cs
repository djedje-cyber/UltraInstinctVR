using System.Collections;
using UnityEngine;

public class RandomSmoothMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float rangeMin = -10f;
    [SerializeField] private float rangeMax = 10f;

    private void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            Vector3 target = new Vector3(
                Random.Range(rangeMin, rangeMax),
                Random.Range(rangeMin, rangeMax),
                Random.Range(rangeMin, rangeMax)
            );

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
        }
    }
}