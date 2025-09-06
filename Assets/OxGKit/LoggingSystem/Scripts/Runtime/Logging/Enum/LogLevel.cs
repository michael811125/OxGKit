using System;

namespace OxGKit.LoggingSystem
{
    [Flags]
    public enum LogLevel
    {
        Off = 0,
        LogDebug = 1 << 0,
        LogInfo = 1 << 1,
        LogWarning = 1 << 2,
        LogError = 1 << 3,
        LogException = 1 << 4,
        All = ~0
    }
}