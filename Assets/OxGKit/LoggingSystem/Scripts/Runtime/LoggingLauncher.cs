using UnityEngine;

namespace OxGKit.LoggingSystem
{
    public class LoggingLauncher : MonoBehaviour
    {
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
            this.InitLoggers();
        }

        public void InitLoggers()
        {
            if (this.loggerSetting != null && !Logging.isLauncherInitialized)
            {
#if UNITY_EDITOR || OXGKIT_LOGGER_ON
                // Init loggers
                Logging.InitLoggers();
                // Load activity state from setting after init
                if (this._LoadLoggerSetting())
                {
                    // Mark flag
                    Logging.isLauncherInitialized = true;

                    Debug.Log($"<color=#00ffa2>[{nameof(LoggingSystem)}] is Initialized.</color>");
                }
#endif
            }
            else
            {
                Logging.ClearLoggers();
#if UNITY_EDITOR || OXGKIT_LOGGER_ON
                Debug.Log($"<color=#ff2763>{nameof(LoggerSetting)} is null, please create a {nameof(LoggerSetting)}. [{nameof(LoggingSystem)}] launch failed!!!</color>");
#endif
            }
        }

        public void ReloadLoggerSetting()
        {
            if (this._LoadLoggerSetting())
            {
                Debug.Log($"<color=#00ffe5>[{nameof(LoggingSystem)}] is reloaded setting in runtime.</color>");
            }
        }

        private bool _LoadLoggerSetting()
        {
            if (this.loggerSetting != null)
            {
                // Set loggers active from setting
                foreach (var loggerData in this.loggerSetting.loggerConfigs)
                {
                    var logger = Logging.GetLogger(loggerData.loggerName);
                    if (logger != null) logger.logActive = loggerData.logActive;
                }

                // Set main active from setting
                Logging.logMainActive = this.loggerSetting.logMainActive;

                return true;
            }

            return false;
        }
    }
}
