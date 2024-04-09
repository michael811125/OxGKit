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
        UnityEngine.Debug.Log("logger 3 is overridden by logger 4: " + message);
    }

    public override void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning("logger 3 is overridden by logger 4: " + message);
    }

    public override void LogError(object message)
    {
        UnityEngine.Debug.LogError("logger 3 is overridden by logger 4: " + message);
    }

    public override void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }
}

public class LoggingDemo : MonoBehaviour
{
    private void Awake()
    {
        /**
         * If you have hotfix procedure but want to initialize manually you can do the following (must unchecked InitLoggersOnAwake trigger)
         * 
         * Logging.CreateLogger<LoggingDemoLogger1>();
         * Logging.CreateLogger<LoggingDemoLogger2>();
         * LoggingLauncher.ReloadLoggerSetting();
         * 
         */
    }

    private void Start()
    {
        // Use logger1 to print
        Logging.Print<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger1!!!");
        // Use Logger2 to print
        Logging.Print<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger2!!!");
        // Use Logger3 to print
        Logging.Print<LoggingDemoLogger3>("[Override] Implement Logger by LoggingDemoLogger3!!!");
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Use logger1 to print
            Logging.Print<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger1!!!");
            // Use Logger2 to print
            Logging.Print<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger2!!!");
            // Use Logger3 to print
            Logging.Print<LoggingDemoLogger3>("[Override] Implement Logger by LoggingDemoLogger3!!!");
        }
    }
}
