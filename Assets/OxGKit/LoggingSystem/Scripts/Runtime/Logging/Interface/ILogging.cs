using System;

namespace OxGKit.LoggingSystem
{
    internal interface ILogging
    {
        void Log(object message);

        void LogInfo(object message);

        void LogWarning(object message);

        void LogError(object message);

        void LogException(Exception exception);

        void Log(object message, UnityEngine.Object context);

        void LogInfo(object message, UnityEngine.Object context);

        void LogWarning(object message, UnityEngine.Object context);

        void LogError(object message, UnityEngine.Object context);

        void LogException(Exception exception, UnityEngine.Object context);
    }
}