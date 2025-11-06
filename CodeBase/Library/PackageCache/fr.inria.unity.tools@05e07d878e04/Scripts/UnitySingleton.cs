using UnityEngine;

namespace InriaTools
{
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    ///
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Fields

        public static bool ShouldNotDestroyOnLoad = true;

        private static readonly object _lock = new();
        private static T _instance;
        private static bool applicationIsQuitting = false;

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[UnitySingleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[UnitySingleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            if (ShouldNotDestroyOnLoad)
                                DontDestroyOnLoad(singleton);

                            Debug.Log("[UnitySingleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created " +
                                (ShouldNotDestroyOnLoad ? "with DontDestroyOnLoad." : ""));
                        }
                    }

                    return _instance;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed,
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
            _instance = null;
        }

        public void OnDestroy()
        {
            _instance = null;
        }

        #endregion
    }
}