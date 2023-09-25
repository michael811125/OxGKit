using OxGKit.LoggingSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[LoggerName("LoggingDemo.Logger1")]
public class LoggingDemoLogger1 : Logging
{
    public LoggingDemoLogger1() : base() { }
}

[LoggerName("LoggingDemo.Logger2")]
public class LoggingDemoLogger2 : Logging
{
    public LoggingDemoLogger2() : base() { }
}

public class LoggingDemo : MonoBehaviour
{
    private void Start()
    {
        // Use logger1 to print
        Logging.Print<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger1!!!");
        // Use Logger2 to print
        Logging.Print<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger2!!!");
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Use logger1 to print
            Logging.Print<LoggingDemoLogger1>("Implement Logger by LoggingDemoLogger1!!!");
            // Use Logger2 to print
            Logging.Print<LoggingDemoLogger2>("Implement Logger by LoggingDemoLogger2!!!");
        }
    }
}
