using MyBox;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    [Serializable]
    public class LoggersConfig
    {
        /// <summary>
        /// 日誌器總開關
        /// </summary>
        [OverrideLabel("Master Logging Toggle")]
        [SerializeField]
        public bool logMainActive = true;

        /// <summary>
        /// 各日誌器配置
        /// </summary>
        [SerializeField]
        public List<LoggerSetting> loggerSettings;

        /// <summary>
        /// 配置文件輸出名稱
        /// </summary>
        public const string LOGGERS_CONFIG_FILE_NAME = "loggersconfig.json";

        public LoggersConfig()
        {
            this.logMainActive = true;
            this.loggerSettings = new List<LoggerSetting>();
        }

        public LoggersConfig(params LoggerSetting[] loggerSettings) : this()
        {
            this.loggerSettings.AddRange(loggerSettings);
        }

        ~LoggersConfig()
        {
            loggerSettings.Clear();
            loggerSettings = null;
        }

        /// <summary>
        /// 清除日誌器配置
        /// </summary>
        public void ClearLoggerSettings()
        {
            this.loggerSettings.Clear();
        }

        /// <summary>
        /// 添加日誌器配置
        /// </summary>
        /// <param name="loggerSetting"></param>
        public void AddLoggerSetting(LoggerSetting loggerSetting)
        {
            this.loggerSettings.Add(loggerSetting);
        }

        /// <summary>
        /// 移除日誌器配置
        /// </summary>
        /// <param name="loggerSetting"></param>
        public void RemoveLoggerSetting(LoggerSetting loggerSetting)
        {
            this.loggerSettings.Remove(loggerSetting);
        }

        /// <summary>
        /// 日誌器總開關
        /// </summary>
        /// <param name="isEnabled"></param>
        internal void ToggleMasterLogging(bool isEnabled)
        {
            this.logMainActive = isEnabled;
        }

        /// <summary>
        /// 配置單一日誌器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="isEnabled"></param>
        /// <param name="logLevel"></param>
        internal void ConfigureLogger(string loggerName, bool isEnabled, LogLevel logLevel)
        {
            foreach (var loggerSetting in this.loggerSettings)
            {
                if (loggerSetting.loggerName == loggerName)
                {
                    loggerSetting.logActive = isEnabled;
                    loggerSetting.logLevel = logLevel;
                    break;
                }
            }
        }

        /// <summary>
        /// 配置全部日誌器
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <param name="logLevel"></param>
        internal void ConfigureAllLoggers(bool isEnabled, LogLevel logLevel)
        {
            foreach (var loggerSetting in this.loggerSettings)
            {
                loggerSetting.logActive = isEnabled;
                loggerSetting.logLevel = logLevel;
            }
        }

        /// <summary>
        /// 獲取 StreamingAssets 配置文件請求路徑
        /// </summary>
        /// <returns></returns>
        internal static string GetStreamingAssetsConfigRequestPath()
        {
            return Path.Combine(WebRequester.GetRequestStreamingAssetsPath(), LOGGERS_CONFIG_FILE_NAME);
        }
    }
}