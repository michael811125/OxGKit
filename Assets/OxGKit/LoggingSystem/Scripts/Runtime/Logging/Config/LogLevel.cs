using System;

namespace OxGKit.LoggingSystem
{
    [Flags]
    public enum LogLevel
    {
        Off = 0,
        Log = 1 << 0,
        LogWarning = 1 << 1,
        LogError = 1 << 2,
        LogException = 1 << 3,
        All = ~0
    }
}