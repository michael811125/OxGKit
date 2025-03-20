using OxGKit.LoggingSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[LoggerName("LoggingDemo.Logger1")]
public class LoggingDemoLogger1 : Logging
{
    public LoggingDemoLogger1() { }
}

[LoggerName("LoggingDemo.Logger2")]
public class LoggingDemoLogger2 : Logging
{
    public LoggingDemoLogger2() { }
}

[LoggerName("LoggingDemo.Logger3")]
public class LoggingDemoLogger3 : Logging
{
    public LoggingDemoLogger3() { }
}

[LoggerName("LoggingDemo.Logger3", true)]
public class LoggingDemoLogger4 : Logging
{
    public LoggingDemoLogger4() { }

    public override void Log(object message)
    {
        Debug.Log($"[Override] logger <color=#1dabf7>3</color> is overridden by logger <color=#eec905>4</color>: {message}");
    }

    public override void LogWarning(object message)
    {
        Debug.LogWarning($"[Override] logger <color=#1dabf7>3</color> is overridden by logger <color=#eec905>4</color>: {message}");
    }

    public override void LogError(object message)
    {
        Debug.LogError($"[Override] logger <color=#1dabf7>3</color> is overridden by logger <color=#eec905>4</color>: {message}");
    }

    public override void LogException(Exception exception)
    {
        Debug.LogException(new Exception($"[Override] logger <color=#1dabf7>3</color> is overridden by logger <color=#eec905>4</color>: {exception.Message}"));
    }
}

public class LoggingDemo : MonoBehaviour
{
    private void Awake()
    {
        /**
         * If you have hotfix procedure but want to initialize manually you can do the following 
         * (Must unchecked InitializedOnAwake trigger)
         * 
         * LoggingLauncher.CreateLogger<LoggingDemoLogger1>();
         * LoggingLauncher.CreateLogger<LoggingDemoLogger2>(); 
         * LoggingLauncher.CreateLogger<LoggingDemoLogger3>(); 
         * LoggingLauncher.CreateLogger<LoggingDemoLogger4>(); 
         * LoggingLauncher.TryLoadLoggers();
         * 
         */
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Use logger1 to print
            Logging.Print<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger<color=#ff2a66>1</color>!!!");
            Logging.PrintWarning<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger<color=#ff2a66>1</color>!!!");
            Logging.PrintError<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger<color=#ff2a66>1</color>!!!");
            Logging.PrintException<LoggingDemoLogger1>(new Exception("Implement Logger by LoggingDemoLogger<color=#ff2a66>1</color>!!!"));
            // Use Logger2 to print
            Logging.Print<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger<color=#1df735>2</color>!!!");
            Logging.PrintWarning<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger<color=#1df735>2</color>!!!");
            Logging.PrintError<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger<color=#1df735>2</color>!!!");
            Logging.PrintException<LoggingDemoLogger2>(new Exception("Implement Logger by LoggingDemoLogger<color=#1df735>2</color>!!!"));
            // Use Logger3 to print
            Logging.Print<LoggingDemoLogger3>("Implement Logger by LoggingDemoLogger<color=#1dabf7>3</color>!!!");
            Logging.PrintWarning<LoggingDemoLogger3>("Implement Logger by LoggingDemoLogger<color=#1dabf7>3</color>!!!");
            Logging.PrintError<LoggingDemoLogger3>("Implement Logger by LoggingDemoLogger<color=#1dabf7>3</color>!!!");
            Logging.PrintException<LoggingDemoLogger3>(new Exception("Implement Logger by LoggingDemoLogger<color=#1dabf7>3</color>!!!"));
        }
        // Switch the logger's active state at runtime (1 and 3)
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            var loggersConfig = new LoggersConfig
            (
                new LoggerSetting("LoggingDemo.Logger1", true, LogLevel.Log),
                new LoggerSetting("LoggingDemo.Logger2", true, LogLevel.LogWarning),
                new LoggerSetting("LoggingDemo.Logger3", true, LogLevel.Off)
            );
            LoggingLauncher.SetLoggersConfig(loggersConfig);
        }
        // Switch the logger's active state at runtime (2)
        else if (Mouse.current.middleButton.wasReleasedThisFrame)
        {
            LoggingLauncher.ConfigureLogger("LoggingDemo.Logger1", false);
            LoggingLauncher.ConfigureLogger("LoggingDemo.Logger2", true, LogLevel.LogWarning | LogLevel.LogError);
            LoggingLauncher.ConfigureLogger("LoggingDemo.Logger3", false);
        }
    }
}
