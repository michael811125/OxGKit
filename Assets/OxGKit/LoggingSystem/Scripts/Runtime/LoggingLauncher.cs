using MyBox;
using System;
using System.Collections;
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
                    if (_instance == null)
                        _instance = new GameObject(typeof(LoggingLauncher).Name).AddComponent<LoggingLauncher>();
                }
            }
            return _instance;
        }

        [HideInInspector]
        public bool initLoggersOnAwake = true;

        [Separator("Loggers")]
        [SerializeField]
        public LoggersConfig loggersConfig = new LoggersConfig();

        private void Awake()
        {
            string newName = $"[{nameof(LoggingLauncher)}]";
            this.gameObject.name = newName;
            if (this.gameObject.transform.root.name == newName)
            {
                var container = GameObject.Find(nameof(OxGKit));
                if (container == null)
                    container = new GameObject(nameof(OxGKit));
                this.gameObject.transform.SetParent(container.transform);
                DontDestroyOnLoad(container);
            }
            else
                DontDestroyOnLoad(this.gameObject.transform.root);

            // Init and read values from config
            if (this.initLoggersOnAwake)
                this.StartCoroutine(this._TryInitLoggers());
        }

        #region Logging APIs
        /// <summary>
        /// Init all loggers
        /// </summary>
        public static void InitLoggers()
        {
            Logging.InitLoggers();
        }

        /// <summary>
        /// Clear all loggers
        /// </summary>
        public static void ClearLoggers()
        {
            Logging.ClearLoggers();
        }

        /// <summary>
        /// Create a logger
        /// </summary>
        /// <typeparam name="TLogging"></typeparam>
        public static void CreateLogger<TLogging>() where TLogging : Logging, new()
        {
            Logging.CreateLogger<TLogging>();
        }
        #endregion

        #region LoggersConfig APIs
        /// <summary>
        /// Toggles the master logging switch to enable or disable all logger
        /// </summary>
        /// <param name="logMainActive"></param>
        public static void ToggleMasterLogging(bool logMainActive)
        {
            GetInstance().StartCoroutine(GetInstance()._LoadLoggersConfig((result) =>
            {
                result.ToggleMasterLogging(logMainActive);
                GetInstance().StartCoroutine(GetInstance()._TryLoadLoggers(true));
            }, false, true));
        }

        /// <summary>
        /// Configures the specified logger's active state and log level
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logActive"></param>
        /// <param name="logLevel"></param>
        public static void ConfigureLogger(string loggerName, bool logActive, LogLevel logLevel = LogLevel.All)
        {
            GetInstance().StartCoroutine(GetInstance()._LoadLoggersConfig((result) =>
            {
                result.ConfigureLogger(loggerName, logActive, logLevel);
                GetInstance().StartCoroutine(GetInstance()._TryLoadLoggers(true));
            }, false, true));
        }

        /// <summary>
        /// Configures all loggers' active state and log level
        /// </summary>
        /// <param name="logActive"></param>
        /// <param name="logLevel"></param>
        public static void ConfigureAllLoggers(bool logActive, LogLevel logLevel = LogLevel.All)
        {
            GetInstance().StartCoroutine(GetInstance()._LoadLoggersConfig((result) =>
            {
                result.ConfigureAllLoggers(logActive, logLevel);
                GetInstance().StartCoroutine(GetInstance()._TryLoadLoggers(true));
            }, false, true));
        }

        /// <summary>
        /// Set configuration file
        /// </summary>
        /// <param name="loggersConfig"></param>
        public static void SetLoggersConfig(LoggersConfig loggersConfig)
        {
            GetInstance().loggersConfig = null;
            GetInstance().loggersConfig = loggersConfig;
            GetInstance().StartCoroutine(GetInstance()._TryLoadLoggers(true));
        }

        private IEnumerator _LoadLoggersConfig(Action<LoggersConfig> result, bool forceReset, bool isCustom)
        {
            if (isCustom)
            {
                result?.Invoke(this.loggersConfig);
                yield break;
            }

            if (!forceReset)
            {
                string url = LoggersConfig.GetStreamingAssetsConfigRequestPath();
                yield return WebRequester.RequestText(url, (json) =>
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        this.loggersConfig = JsonUtility.FromJson<LoggersConfig>(json);
                        result?.Invoke(this.loggersConfig);
                    }
                },
                () =>
                {
                    var loggersConfig = new LoggersConfig();
                    this.loggersConfig = loggersConfig;
                    ReloadLoggersConfigAndSet(loggersConfig, result);
                });
            }
            else
            {
                var loggersConfig = new LoggersConfig();
                this.loggersConfig = loggersConfig;
                result?.Invoke(this.loggersConfig);
            }
        }

        /// <summary>
        /// Attempts to find and initialize all loggers with their settings
        /// </summary>
        public static void TryInitLoggers()
        {
            GetInstance().StartCoroutine(GetInstance()._TryInitLoggers());
        }

        private IEnumerator _TryInitLoggers()
        {
            // Init loggers
            Logging.InitLoggers();
            // Load logger settings from config after init
            yield return this._LoadLoggers(false);
            Debug.Log($"<color=#00ffa2>[{nameof(LoggingSystem)}] is Initialized.</color>");
        }

        /// <summary>
        /// Attempts to load loggers based on the configuration
        /// </summary>
        public static void TryLoadLoggers()
        {
            GetInstance().StartCoroutine(GetInstance()._TryLoadLoggers(false));
        }

        private IEnumerator _TryLoadLoggers(bool isCustom)
        {
            yield return this._LoadLoggers(isCustom);
            Debug.Log($"<color=#00ffe5>[{nameof(LoggingSystem)}] is reloaded setting in runtime.</color>");
        }

        private IEnumerator _LoadLoggers(bool isCustom)
        {
#if UNITY_EDITOR || OXGKIT_LOGGER_ON
            yield return this._LoadLoggersConfig((loggersConfig) =>
            {
                if (loggersConfig != null)
                {
                    // Reset all logger settings first
                    Logging.ResetLoggers();

                    // Set loggers active from config
                    foreach (var loggerSetting in loggersConfig.loggerSettings)
                    {
                        var logger = Logging.GetLogger(loggerSetting.loggerName);
                        if (logger != null)
                        {
                            logger.logActive = loggerSetting.logActive;
                            logger.logLevel = loggerSetting.logLevel;
                        }
                    }

                    // Set main active from config
                    Logging.logMainActive = loggersConfig.logMainActive;
                }
            }, false, isCustom);
#else
            Debug.Log($"<color=#ff2763>Not enabled {nameof(LoggingSystem)} by symbol [OXGKIT_LOGGER_ON].</color>");
            yield break;
#endif
        }
        #endregion

        /// <summary>
        /// Reload loggers again
        /// </summary>
        public void ReloadLoggersConfig(Action<LoggersConfig> result = null)
        {
            this.StartCoroutine(this._ReloadLoggersConfig(false, result));
        }

        public void ReloadFromLoggersConfig(Action<LoggersConfig> result = null)
        {
            this.StartCoroutine(this._LoadLoggersConfig(result, false, false));
        }

        /// <summary>
        /// Clear loggers and reload again
        /// </summary>
        public void ResetLoggersConfig(Action<LoggersConfig> result = null)
        {
            this.StartCoroutine(this._ResetLoggersConfig(result));
        }

        private IEnumerator _ResetLoggersConfig(Action<LoggersConfig> result)
        {
            this.loggersConfig = null;
            yield return this._ReloadLoggersConfig(true, result);
        }

        private IEnumerator _ReloadLoggersConfig(bool forceReset, Action<LoggersConfig> result)
        {
            // Initialized loggers
            Logging.InitLoggers();
            yield return this._LoadLoggersConfig((loggersConfig) =>
            {
                ReloadLoggersConfigAndSet(loggersConfig, result);
            }, forceReset, false);
        }

        public static void ReloadLoggersConfigAndSet(LoggersConfig loggersConfig, Action<LoggersConfig> result)
        {
            if (loggersConfig.loggerSettings.Count == 0)
            {
                // Init settings
                var loggers = Logging.GetLoggers();
                foreach (var logger in loggers)
                {
                    loggersConfig.loggerSettings.Add(new LoggerSetting(logger.Key, logger.Value.logActive, logger.Value.logLevel));
                }
                result?.Invoke(loggersConfig);
            }
            else
            {
                // Remove not exist implements (backward)
                for (int i = loggersConfig.loggerSettings.Count - 1; i >= 0; i--)
                {
                    if (!Logging.HasLogger(loggersConfig.loggerSettings[i].loggerName))
                    {
                        loggersConfig.loggerSettings.RemoveAt(i);
                    }
                }

                // Check if there are new implements
                var loggers = Logging.GetLoggers();
                foreach (var logger in loggers)
                {
                    bool found = false;
                    for (int i = 0; i < loggersConfig.loggerSettings.Count; i++)
                    {
                        if (logger.Key == loggersConfig.loggerSettings[i].loggerName)
                        {
                            found = true;
                            break;
                        }
                    }

                    // If not found represents the new logger
                    if (!found)
                        loggersConfig.loggerSettings.Add(new LoggerSetting(logger.Key, logger.Value.logActive, logger.Value.logLevel));
                }
                result?.Invoke(loggersConfig);
            }
        }
    }
}
