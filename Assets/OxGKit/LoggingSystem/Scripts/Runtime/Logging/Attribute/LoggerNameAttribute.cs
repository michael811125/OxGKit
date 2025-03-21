using System;

namespace OxGKit.LoggingSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LoggerNameAttribute : Attribute
    {
        public string loggerName;
        public bool isOverride;

        public LoggerNameAttribute(string loggerName)
        {
            this.loggerName = loggerName;
        }

        public LoggerNameAttribute(string loggerName, bool isOverride)
        {
            this.loggerName = loggerName;
            this.isOverride = isOverride;
        }
    }
}