using UnityEngine;



namespace GetStartTimeSpace
{

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

        // Renamed the method to avoid collision with the class name
        public float GetStartTimeValue()
        {
            return startTime;
        }
    }
}