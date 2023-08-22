using UnityEngine;

namespace OxGKit.Utilities.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static readonly object _locker = new object();
        private static T _instance;

        public static T GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (Application.isPlaying)
                    {
                        _instance = FindObjectOfType(typeof(T)) as T;
                        if (_instance == null) _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                        DontDestroyOnLoad(_instance);
                    }
                }
            }
            return _instance;
        }
    }
}