using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    [CreateAssetMenu(fileName = nameof(LoggerSetting), menuName = "OxGKit/Logging System/Create Logger Setting", order = 0)]
    public class LoggerSetting : ScriptableObject
    {
        [Serializable]
        public class LoggerConfig
        {
            [ReadOnly]
            public string loggerName;
            public bool logActive;

            public LoggerConfig(string loggerName, bool logActive)
            {
                this.loggerName = loggerName;
                this.logActive = logActive;
            }
        }

        [SerializeField]
        [OverrideLabel("Enabled All Loggers")]
        public bool logMainActive = true;
        [SerializeField]
        public List<LoggerConfig> loggerConfigs = new List<LoggerConfig>();

        [ButtonClicker(nameof(RefreshAndLoadLoggers), "Reload Loggers", "#00ffd1")]
        public bool loadLoggers;
        [Space(5f)]
        [ButtonClicker(nameof(ResetSettingData), "Reset Loggers", "#ff5786")]
        public bool resetLoggers;

        /// <summary>
        /// If add new logger or remove logger must call refresh and load
        /// </summary>
        public void RefreshAndLoadLoggers()
        {
            Logging.InitLoggers();
            this.CheckAnSetSettingData();
        }

        public void ResetSettingData()
        {
            Logging.InitLoggers();
            this.loggerConfigs.Clear();
            this.CheckAnSetSettingData();
        }

        public void CheckAnSetSettingData()
        {
            if (this.loggerConfigs.Count == 0)
            {
                // Init setting 
                var loggers = Logging.GetLoggers();
                foreach (var logger in loggers)
                {
                    this.loggerConfigs.Add(new LoggerConfig(logger.Key, logger.Value.logActive));
                }
            }
            else
            {
                // Remove not exisit implements (backward)
                for (int i = this.loggerConfigs.Count - 1; i >= 0; i--)
                {
                    if (!Logging.HasLogger(this.loggerConfigs[i].loggerName))
                    {
                        this.loggerConfigs.RemoveAt(i);
                    }
                }

                // Check if there are new implements
                var loggers = Logging.GetLoggers();
                foreach (var logger in loggers)
                {
                    bool found = false;
                    for (int i = 0; i < this.loggerConfigs.Count; i++)
                    {
                        if (logger.Key == this.loggerConfigs[i].loggerName)
                        {
                            found = true;
                            break;
                        }
                    }

                    // If not found represents the new logger
                    if (!found) this.loggerConfigs.Add(new LoggerConfig(logger.Key, logger.Value.logActive));
                }
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif
        }
    }
}
