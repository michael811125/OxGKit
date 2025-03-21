using System;

namespace OxGKit.LoggingSystem
{
    internal interface ILogging
    {
        void Log(object message);

        void LogWarning(object message);

        void LogError(object message);

        void LogException(Exception exception);
    }
}