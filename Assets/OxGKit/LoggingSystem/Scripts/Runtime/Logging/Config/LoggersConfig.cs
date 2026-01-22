using MyBox;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    /// <summary>
    /// 全部日誌器配置
    /// </summary>
    [Serializable]
    public class LoggersConfig
    {
        /// <summary>
        /// 日誌器全域開關
        /// </summary>
        [OverrideLabel("Master Logging Toggle")]
        [SerializeField]
        public bool logMainActive = true;

        /// <summary>
        /// 日誌器全域級別
        /// </summary>
        [OverrideLabel("Master Logging Level")]
        [SerializeField]
        public LogLevel logMainLevel = LogLevel.All;

        /// <summary>
        /// 日誌器全域顏色模式
        /// </summary>
        [OverrideLabel("Master Logging Color")]
        [SerializeField]
        public LogColor logMainColor = LogColor.EditorOnly;

        /// <summary>
        /// 各日誌器配置
        /// </summary>
        [SerializeField]
        public List<LoggerSettings> loggerSettings;

        public LoggersConfig()
        {
            this.logMainActive = true;
            this.logMainLevel = LogLevel.All;
            this.logMainColor = LogColor.EditorOnly;
            this.loggerSettings = new List<LoggerSettings>();
        }

        public LoggersConfig(params LoggerSettings[] loggerSettings) : this()
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
        /// <param name="loggerSettings"></param>
        public void AddLoggerSettings(LoggerSettings loggerSettings)
        {
            this.loggerSettings.Add(loggerSettings);
        }

        /// <summary>
        /// 移除日誌器配置
        /// </summary>
        /// <param name="loggerSettings"></param>
        public void RemoveLoggerSettings(LoggerSettings loggerSettings)
        {
            this.loggerSettings.Remove(loggerSettings);
        }

        /// <summary>
        /// 日誌器全域開關
        /// </summary>
        /// <param name="isEnabled"></param>
        internal void ToggleMasterLogging(bool isEnabled)
        {
            this.logMainActive = isEnabled;
        }

        /// <summary>
        /// 日誌器全域級別
        /// </summary>
        /// <param name="logLevel"></param>
        internal void LevelMasterLogging(LogLevel logLevel)
        {
            this.logMainLevel = logLevel;
        }

        /// <summary>
        /// 日誌器全域顏色模式
        /// </summary>
        /// <param name="logColor"></param>
        internal void ColorMasterLogging(LogColor logColor)
        {
            this.logMainColor = logColor;
        }

        /// <summary>
        /// 配置單一日誌器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="isEnabled"></param>
        /// <param name="logLevel"></param>
        internal void ConfigureLogger(string loggerName, bool isEnabled, LogLevel logLevel)
        {
            foreach (var loggerSettings in this.loggerSettings)
            {
                if (loggerSettings.loggerName == loggerName)
                {
                    loggerSettings.logActive = isEnabled;
                    loggerSettings.logLevel = logLevel;
                    break;
                }
            }
        }

        /// <summary>
        /// 配置單一日誌器
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="isEnabled"></param>
        /// <param name="logLevel"></param>
        /// <param name="logColor"></param>
        internal void ConfigureLogger(string loggerName, bool isEnabled, LogLevel logLevel, LogColor logColor)
        {
            foreach (var loggerSettings in this.loggerSettings)
            {
                if (loggerSettings.loggerName == loggerName)
                {
                    loggerSettings.logActive = isEnabled;
                    loggerSettings.logLevel = logLevel;
                    loggerSettings.logColor = logColor;
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
            foreach (var loggerSettings in this.loggerSettings)
            {
                loggerSettings.logActive = isEnabled;
                loggerSettings.logLevel = logLevel;
            }
        }

        /// <summary>
        /// 配置全部日誌器
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <param name="logLevel"></param>
        internal void ConfigureAllLoggers(bool isEnabled, LogLevel logLevel, LogColor logColor)
        {
            foreach (var loggerSettings in this.loggerSettings)
            {
                loggerSettings.logActive = isEnabled;
                loggerSettings.logLevel = logLevel;
                loggerSettings.logColor = logColor;
            }
        }

        /// <summary>
        /// 獲取 StreamingAssets 配置文件請求路徑
        /// </summary>
        /// <returns></returns>
        internal static string GetStreamingAssetsConfigRequestPath()
        {
            return Path.Combine(WebRequester.GetRequestStreamingAssetsPath(), $"{LoggingSettings.settings.loggerCfgName}{LoggingSettings.settings.loggerCfgExtension}");
        }
    }
}