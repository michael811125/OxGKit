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

            Logging.isLauncherInitialized = false;

            if (this.loggerSetting != null)
            {
                Logging.InitLoggers();

                // Set loggers active from setting
                foreach (var loggerData in this.loggerSetting.loggerConfigs)
                {
                    var logger = Logging.GetLogger(loggerData.loggerName);
                    if (logger != null) logger.logActive = loggerData.logActive;
                }

                // Set main active from setting
                Logging.logMainActive = this.loggerSetting.logMainActive;

                // Mark flag
                Logging.isLauncherInitialized = true;

                Debug.Log($"<color=#00ffa2>[{nameof(LoggingSystem)}] is Initialized.</color>");
            }
            else
            {
                Logging.ClearLoggers();
                Debug.Log($"<color=#ff2763>{nameof(LoggerSetting)} is null, please create a {nameof(LoggerSetting)}. [{nameof(LoggingSystem)}] launch failed!!!</color>");
            }
        }
    }
}
