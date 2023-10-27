using System;

namespace OxGKit.LoggingSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LoggerNameAttribute : Attribute
    {
        public string loggerName;

        public LoggerNameAttribute(string loggerName)
        {
            this.loggerName = loggerName;
        }
    }

    internal interface ILogging
    {
        void Log(object message);

        void LogWarning(object message);

        void LogError(object message);

        void LogException(Exception exception);
    }
}