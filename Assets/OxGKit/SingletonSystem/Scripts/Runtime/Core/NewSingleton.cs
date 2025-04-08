namespace OxGKit.SingletonSystem
{
    public class NewSingleton<T> where T : class, new()
    {
        private static readonly object _locker = new object();
        private static T _instance = null;

        public static T GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    _instance = new T();
                }
            }
            return _instance;
        }

        /// <summary>
        /// Initialize instance
        /// </summary>
        public static void InitInstance()
        {
            GetInstance();
        }

        /// <summary>
        /// Check if an instance exists
        /// </summary>
        /// <returns></returns>
        public static bool CheckInstanceExists()
        {
            return _instance != null;
        }

        /// <summary>
        /// Destroy singleton instance
        /// </summary>
        public static void DestroyInstance()
        {
            _instance = null;
        }
    }
}
