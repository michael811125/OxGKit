## CHANGELOG

## [1.2.3] - 2025-03-24
- Added Async methods (can await).

## [1.2.2] - 2025-03-21
- Added loggersconfig.conf.
- Added support for Cipher and Plaintext formats (JSON, BYTES), allowing for conversion in either direction, with automatic parsing.
- Added global-level control.
- Added Logging -> CheckLogActive method (to check the log level).
- Removed Logging -> LogActive method.
- Removed loggersconfig.json.

## [1.2.1] - 2025-03-21
- Modified LoggingLauncherEditor.

## [1.2.0] - 2025-03-21 (Breaking Changes)
- Added new LogLevel levels.
- Added LoggingLauncher API methods.
- Added a new loggersconfig.json storage method (stored in StreamingAssets).
- Added LoggingHelper Editor, which can be used for BuildTool.
- Updated LoggingDemo.
- Removed the ScriptableObject storage method for loggers.

## [1.1.2] - 2024-11-20
- Added SetMainActive in LoggerSetting.
- Added SetLoggerActive in LoggerSetting.
- Added SetAllLoggersActive in LoggerSetting.
- Organized code.

## [1.1.1] - 2024-10-08
- Added SetSetting method.

## [1.1.0] - 2024-04-09
- Added override feature, can override an existing Logger.
```C#
// Already existing Logger
[LoggerName("LoggingDemo.Logger3")]
public class LoggingDemoLogger3 : Logging


// Override Logger 3
[LoggerName("LoggingDemo.Logger3", true)]
public class LoggingDemoLogger4 : Logging
```
- Added LoggingLauncher.ClearLoggers() method.
- Updated LoggingDemo.
- Obsoleted LoggingLauncher.InitLoggers() is obsolete. Use LoggingLauncher.TryInitLoggers().
- Obsoleted LoggingLauncher.ReloadLoggerSetting() is obsolete. Use LoggingLauncher.TryLoadLoggerSetting().

## [1.0.0] - 2024-02-02
- Stabled

## [0.0.11-preview] - 2023-12-18
- Added Select all and Deselect all buttons on editor inspector.
- Fixed Logging editor dirty bug issue.

## [0.0.10-preview] - 2023-10-27
- Modified log message type (string change to object).

## [0.0.9-preview] - 2023-09-27
- Added CreateLogger<TLogging> method in Logging (can create logger and reload logger setting by yourself).
- Added GetSetting static method in LoggingLauncher.
```C#
// Init by yourself
Logging.CreateLogger<YourLogger>();
LoggingLauncher.ReloadLoggerSetting();
```

## [0.0.8-preview] - 2023-09-26
- Added InitLoggersOnAwake trigger and create a singleton in LoggingLauncher.

## [0.0.7-preview] - 2023-09-26
- Adjusted Activator.CreateInstance params.

## [0.0.6-preview] - 2023-09-22
- Added default constructor.

## [0.0.5-preview] - 2023-09-21
- Modified can directly override the interface method without check LogActive.

## [0.0.4-preview] - 2023-09-21
- Modified buttons color (must upgrade LWMyBox to v1.1.4).

## [0.0.3-preview] - 2023-09-10
- Optimized runtime reload setting efficiency.

## [0.0.2-preview] - 2023-09-09
- Added LoggingLauncher Editor (can runtime reload setting).

## [0.0.1-preview] - 2023-08-27
- Added LoggingSystem.