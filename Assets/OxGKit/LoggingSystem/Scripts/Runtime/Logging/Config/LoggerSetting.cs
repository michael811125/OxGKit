using MyBox;
using System;

namespace OxGKit.LoggingSystem
{
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

        public LoggerSetting(string loggerName, bool logActive)
        {
            this.loggerName = loggerName;
            this.logActive = logActive;
        }

        public LoggerSetting(string loggerName, bool logActive, LogLevel logLevel) : this(loggerName, logActive)
        {
            this.logLevel = logLevel;
        }
    }
}