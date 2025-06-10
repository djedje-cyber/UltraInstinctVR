using UnityEngine;

public class GetStartTime : MonoBehaviour
{
    public static GetStartTime Instance { get; private set; }

    private float startTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startTime = Time.time;
    }

    // Renommé la méthode pour éviter la collision avec le nom de la classe
    public float GetStartTimeValue()
    {
        return startTime;
    }
}
