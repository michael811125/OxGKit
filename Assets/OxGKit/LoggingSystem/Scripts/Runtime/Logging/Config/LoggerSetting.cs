using MyBox;
using System;

namespace OxGKit.LoggingSystem
{
    /// <summary>
    /// 單一日誌器配置
    /// </summary>
    [Serializable]
    public class LoggerSetting
    {
        /// <summary>
        /// 日誌器名稱
        /// </summary>
        [ReadOnly]
        public string loggerName;

        /// <summary>
        /// 日誌器開關
        /// </summary>
        public bool logActive;

        /// <summary>
        /// 日誌器等級
        /// </summary>
        public LogLevel logLevel = (LogLevel)~0;

        /// <summary>
        /// 日誌器顏色模式
        /// </summary>
        public LogColor logColor = LogColor.EditorOnly;

        public LoggerSetting(string loggerName, bool logActive)
        {
            this.loggerName = loggerName;
            this.logActive = logActive;
        }

        public LoggerSetting(string loggerName, bool logActive, LogLevel logLevel) : this(loggerName, logActive)
        {
            this.logLevel = logLevel;
        }

        public LoggerSetting(string loggerName, bool logActive, LogLevel logLevel, LogColor logColor) : this(loggerName, logActive, logLevel)
        {
            this.logColor = logColor;
        }
    }
}