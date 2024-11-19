using MyBox;
using System;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    [DisallowMultipleComponent]
    public class LoggingLauncher : MonoBehaviour
    {
        private static readonly object _locker = new object();
        private static LoggingLauncher _instance;

        internal static LoggingLauncher GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    _instance = FindObjectOfType(typeof(LoggingLauncher)) as LoggingLauncher;
                    if (_instance == null) _instance = new GameObject(typeof(LoggingLauncher).Name).AddComponent<LoggingLauncher>();
                }
            }
            return _instance;
        }

        [Separator("Options")]
        [Tooltip("If checked, all loggers in the application domain will be automatically found and loaded. (HybridCLR is not supported. Must be initialized manually.)")]
        [InfoBox("HybridCLR is not supported. Must be initialized manually.", EInfoBoxType.Warning)]
        [OverrideLabel("Initialized On Awake")]
        public bool initLoggersOnAwake = true;
        [Separator("Setting Data")]
        [InfoBox("If not assigned, it will be automatically generated.", EInfoBoxType.Normal)]
        public LoggerSetting loggerSetting;

        public void Awake()
        {
            string newName = $"[{nameof(LoggingLauncher)}]";
            this.gameObject.name = newName;
            if (this.gameObject.transform.root.name == newName)
            {
                var container = GameObject.Find(nameof(OxGKit));
                if (container == null) container = new GameObject(nameof(OxGKit));
                this.gameObject.transform.SetParent(container.transform);
                DontDestroyOnLoad(container);
            }
            else DontDestroyOnLoad(this.gameObject.transform.root);

            // Init and read values from setting
            if (this.initLoggersOnAwake)
                TryInitLoggers();
        }

        public static void SetSetting(LoggerSetting loggerSetting)
        {
            GetInstance().loggerSetting = loggerSetting;
        }

        public static LoggerSetting GetSetting()
        {
            if (GetInstance().loggerSetting == null)
                _LoadSettingData();
            return GetInstance().loggerSetting;
        }

        private static void _LoadSettingData()
        {
            GetInstance().loggerSetting = Resources.Load<LoggerSetting>(nameof(LoggerSetting));
            if (GetInstance().loggerSetting == null)
                GetInstance().loggerSetting = ScriptableObject.CreateInstance<LoggerSetting>();
        }

        /// <summary>
        /// Clear all loggers
        /// </summary>
        public static void ClearLoggers()
        {
            Logging.ClearLoggers();
        }

        /// <summary>
        /// Try to find all loggers and load logger setting
        /// </summary>
        public static void TryInitLoggers()
        {
            if (GetSetting() != null)
            {
                // Init loggers
                Logging.InitLoggers();
                // Load activity state from setting after init
                if (_LoadLoggerSetting())
                {
                    Debug.Log($"<color=#00ffa2>[{nameof(LoggingSystem)}] is Initialized.</color>");
                }
            }
        }

        /// <summary>
        ///  Try to reload logger setting
        /// </summary>
        public static void TryLoadLoggerSetting()
        {
            if (_LoadLoggerSetting())
            {
                Debug.Log($"<color=#00ffe5>[{nameof(LoggingSystem)}] is reloaded setting in runtime.</color>");
            }
        }

        private static bool _LoadLoggerSetting()
        {
#if UNITY_EDITOR || OXGKIT_LOGGER_ON
            if (GetSetting() != null)
            {
                // Set loggers active from setting
                foreach (var loggerConfig in GetSetting().loggerConfigs)
                {
                    var logger = Logging.GetLogger(loggerConfig.loggerName);
                    if (logger != null)
                        logger.logActive = loggerConfig.logActive;
                }

                // Set main active from setting
                Logging.logMainActive = GetSetting().logMainActive;

                return true;
            }
#else
            Debug.Log($"<color=#ff2763>Not enabled {nameof(LoggingSystem)} by symbol [OXGKIT_LOGGER_ON].</color>");
#endif

            return false;
        }

        #region Obsolete
        [Obsolete("LoggingLauncher.InitLoggers() is obsolete. Use LoggingLauncher.TryInitLoggers()")]
        public static void InitLoggers()
        {
            if (GetSetting() != null)
            {
                // Init loggers
                Logging.InitLoggers();
                // Load activity state from setting after init
                if (_LoadLoggerSetting())
                {
                    Debug.Log($"<color=#00ffa2>[{nameof(LoggingSystem)}] is Initialized.</color>");
                }
            }
        }

        [Obsolete("LoggingLauncher.ReloadLoggerSetting() is obsolete. Use LoggingLauncher.TryLoadLoggerSetting()")]
        public static void ReloadLoggerSetting()
        {
            if (_LoadLoggerSetting())
            {
                Debug.Log($"<color=#00ffe5>[{nameof(LoggingSystem)}] is reloaded setting in runtime.</color>");
            }
        }
        #endregion
    }
}
