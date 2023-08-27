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

    public interface ILogging
    {
        void Log(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogException(Exception exception);
    }
}