using UnityEngine;

namespace OxGKit.SingletonSystem
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        /// <summary>
        /// Don't destroy on Awake
        /// </summary>
        public bool dontDestroyOnLoad = true;

        public static bool isCreated { get; private set; }
        public static bool isStarted { get; private set; }
        public static bool isReleased { get; private set; }

        private static readonly object _locker = new object();
        private static T _instance;

        /// <summary>
        /// <para> If not in the scene beforehand also don't need a DontDestroyOnLoad MonoSingleton, can set param dontDestroyOnLoad = false once. </para>
        /// <para> If not in the scene beforehand also need a DontDestroyOnLoad MonoSingleton, can set param dontDestroyOnLoad = true once. </para>
        /// </summary>
        /// <param name="dontDestroyOnLoad"></param>
        /// <returns></returns>
        public static T GetInstance(bool dontDestroyOnLoad = true)
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (Application.isPlaying)
                    {
                        _instance = _FindExistingInstance();
                        if (_instance == null)
                            _instance = _CreateNewInstance();
                        if (dontDestroyOnLoad)
                        {
                            // Mark as dontDestroyOnLoad
                            _instance.dontDestroyOnLoad = dontDestroyOnLoad;
                            DontDestroyOnLoad(_instance);
                        }
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// Initialize instance
        /// </summary>
        public static void InitInstance(bool dontDestroyOnLoad = true)
        {
            GetInstance(dontDestroyOnLoad);
        }

        /// <summary>
        /// Check if an instance exists
        /// </summary>
        /// <returns></returns>
        public static bool CheckInstanceExists()
        {
            return _instance != null;
        }

        private static T _FindExistingInstance()
        {
            T[] existingInstances = FindObjectsOfType<T>();

            // No instance found
            if (existingInstances == null || existingInstances.Length == 0)
                return null;

            return existingInstances[0];
        }

        private static T _CreateNewInstance()
        {
            return new GameObject(typeof(T).Name).AddComponent<T>();
        }

        /// <summary>
        /// Call by Unity.Awake
        /// <para>
        /// Note: Except Awake(), Start() and OnDestroy(), other Unity methods can be override.
        /// </para>
        /// </summary>
        protected abstract void OnCreate();

        /// <summary>
        /// Call by Unity.Start
        /// <para>
        /// Note: Except Awake(), Start() and OnDestroy(), other Unity methods can be override.
        /// </para>
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Call by Unity.OnDestroy
        /// <para>
        /// Note: Except Awake(), Start() and OnDestroy(), other Unity methods can be override.
        /// </para>
        /// </summary>
        protected abstract void OnRelease();

        /// <summary>
        /// Destroy singleton instance
        /// </summary>
        /// <param name="gameObjectIncluded"></param>
        public static void DestroyInstance(bool gameObjectIncluded = true)
        {
            GameObject go = null;
            if (gameObjectIncluded)
                go = _instance?.gameObject;
            Component.Destroy(_instance);
            GameObject.Destroy(go);
        }

        #region Unity Methods
        /// <summary>
        /// Don't override (Except Awake(), Start() and OnDestroy(), other Unity methods can be override.)
        /// </summary>
        private void Awake()
        {
            // Reset destroy flag
            isReleased = false;

            // If MonoSingleton already in the scene
            if (_instance == null)
            {
                _instance = _FindExistingInstance();
                if (this.dontDestroyOnLoad)
                    DontDestroyOnLoad(_instance);
            }

            if (!isCreated)
            {
                isCreated = true;
                this.OnCreate();
            }
        }

        /// <summary>
        /// Don't override (Except Awake(), Start() and OnDestroy(), other Unity methods can be override.)
        /// </summary>
        private void Start()
        {
            if (!isStarted)
            {
                isStarted = true;
                this.OnStart();
            }
        }

        /// <summary>
        /// Don't override (Except Awake(), Start() and OnDestroy(), other Unity methods can be override.)
        /// </summary>
        private void OnDestroy()
        {
            isCreated = false;
            isStarted = false;
            isReleased = true;
            this.OnRelease();
            _instance = null;
        }
        #endregion
    }
}