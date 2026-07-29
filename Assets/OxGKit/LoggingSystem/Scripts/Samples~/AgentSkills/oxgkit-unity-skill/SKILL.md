---
name: oxgkit-unity-skill
description: Use when developing, reviewing, or debugging Unity projects that use OxGKit.LoggingSystem, including declaring loggers with [LoggerName] and the Logging base class, Logging.Print/PrintInfo/PrintWarning/PrintError/PrintException calls, the LoggingLauncher prefab and LoggerConfig.dat (StreamingAssets) configuration, LogLevel/LogColor rules, the OXGKIT_LOGGER_ON build symbol, runtime reconfiguration (ConfigureLogger/SetLoggersConfig), and HybridCLR AOT/Hotfix logger initialization.
---

# OxGKit.LoggingSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.LoggingSystem user. LoggingSystem is a standalone logging module (UPM package `com.michaelo.oxgkit.loggingsystem`) with named loggers, per-logger and global on/off + level + color control, and an encrypted/plaintext runtime config file for tuning logs on device without rebuilding.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Before writing code, verify APIs against the installed package source (`Library/PackageCache/com.michaelo.oxgkit.loggingsystem@*` or `Assets/OxGKit/LoggingSystem/Scripts` when embedded). Do not invent APIs; if a member is not listed here, confirm it in the source first.
4. For player builds, always check whether the `OXGKIT_LOGGER_ON` scripting define symbol is set — without it all logging is stripped from builds (the Editor works without it).

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/LoggingSystem/Scripts`
- Auto dependency: `com.domybest.lwmybox` (LWMyBox, inspector attributes).
- Samples (Package Manager > OxGKit.LoggingSystem > Samples): `LoggingLauncher Prefab` (bootstrap prefab), `LoggingSystem Demo`, `AI Agent Skills` (this skill).

## Core concepts

- A **logger** is a class inheriting `Logging`, named by `[LoggerName("Name")]` (falls back to the class name when the attribute is missing).
- **`LoggingLauncher`** is a MonoBehaviour that discovers/initializes loggers and loads their settings. Import the `LoggingLauncher Prefab` sample and drop it into the boot scene once (`initLoggersOnAwake` is on by default).
- **`LoggerConfig.dat`** in `StreamingAssets` overrides the inspector settings at runtime, so QA can toggle logs on device. Created via right-click `Create/OxGKit/Logging System/[JSON - Plaintext] or [BYTES - Cipher] Create LoggerConfig.dat (In StreamingAssets)`; convertible both ways via `Assets/OxGKit/Logging System/Convert LoggerConfig.dat (BYTES [Cipher] <-> JSON [Plaintext])`. Ship releases with the cipher format. The file name, extension, and cipher key are customizable through the `LoggingSettings` asset (v1.4.0+; older versions used `loggersconfig.conf`).
- A log call is emitted only when the **global (master)** settings AND the **per-logger** settings both allow it (bitwise intersection for levels; see tables below).

## Core API (verified)

```csharp
using OxGKit.LoggingSystem;

// Declare a logger (public default constructor recommended for HybridCLR/reflection)
[LoggerName("App.Runtime")]
public class RuntimeLogger : Logging
{
    public RuntimeLogger() { }
}

// Override an existing logger's behavior by re-using its name with isOverride = true
[LoggerName("App.Runtime", true)]
public class OverrideRuntimeLogger : Logging
{
    public OverrideRuntimeLogger() { }
    public override void Log(object message) { UnityEngine.Debug.Log("[Override] " + message); }
    // Also overridable: LogInfo, LogWarning, LogError, LogException(Exception)
}

// Emit logs (context is optional, same as Unity's)
Logging.Print<RuntimeLogger>("debug message");
Logging.PrintInfo<RuntimeLogger>("info message");
Logging.PrintWarning<RuntimeLogger>("warning message");
Logging.PrintError<RuntimeLogger>("error message");
Logging.PrintException<RuntimeLogger>(exception);
```

`LoggingLauncher` static API:

```csharp
LoggingLauncher.InitLoggers();                    // Discover and instantiate all Logging subclasses
LoggingLauncher.CreateLogger<TLogging>();         // Manually create one logger (HybridCLR flow)
LoggingLauncher.ClearLoggers();
LoggingLauncher.TryInitLoggers();                 // InitLoggers + load settings from config (coroutine wrapper)
LoggingLauncher.TryLoadLoggers();                 // (Re)load logger settings from LoggerConfig.dat
// Async (IEnumerator) variants: TryInitLoggersAsync / TryLoadLoggersAsync

// Master (global) switches
LoggingLauncher.ToggleMasterLogging(true);
LoggingLauncher.LevelMasterLogging(LogLevel.All);
LoggingLauncher.ColorMasterLogging(LogColor.EditorOnly);

// Per-logger runtime configuration (by logger name)
LoggingLauncher.ConfigureLogger("App.Runtime", true, LogLevel.LogWarning | LogLevel.LogError);
LoggingLauncher.ConfigureAllLoggers(true, LogLevel.All, LogColor.EditorOnly);

// Replace the whole config at runtime
LoggingLauncher.SetLoggersConfig(new LoggersConfig(
    new LoggerSettings("App.Runtime", true, LogLevel.All)
));
```

Enums:

```csharp
[Flags] LogLevel { Off, LogDebug, LogInfo, LogWarning, LogError, LogException, All }
LogColor { Disabled, Enabled, EditorOnly }   // EditorOnly strips rich-text color in players
ConfigFileType { Json, Bytes }               // LoggingLauncher.currentConfigFileType
```

## Level and color resolution

Levels — a level outputs only when both Global (G) and Per-logger (P) contain it (`G ∩ P`):

| Global (G) | Per-logger (P) | Effective |
| --- | --- | --- |
| Off | any | none |
| any | Off | none |
| All | All | all levels |
| All | single Px | Px |
| single Gx | All | Gx |
| set G | set P | G ∩ P (none if empty) |

Colors — effective mode is the more restrictive of the two; `EditorOnly` colors in the Editor but not in players:

| Global | Per-logger | Effective |
| --- | --- | --- |
| Disabled | any | Disabled |
| Enabled | Enabled | Enabled |
| Enabled/EditorOnly | EditorOnly (or vice versa) | EditorOnly |

## Standard setup flow

1. Import the `LoggingLauncher Prefab` sample; put the prefab in the boot scene (once).
2. Declare `[LoggerName]` logger classes near each subsystem.
3. Press Play — loggers are auto-discovered on Awake (`initLoggersOnAwake`), settings load from `StreamingAssets/LoggerConfig.dat` when present.
4. Add `OXGKIT_LOGGER_ON` to Scripting Define Symbols for any build that must log.
5. To tune logs on device, edit/replace `LoggerConfig.dat` (convert to JSON for hand editing, back to BYTES for release).

## HybridCLR (AOT + Hotfix) flow

Auto discovery scans assemblies and may miss hotfix-loaded loggers, so:

1. Uncheck `Initialize On Awake` on the `LoggingLauncher` prefab.
2. In the AOT (main) startup: `LoggingLauncher.CreateLogger<TAotLogger>()` for every AOT logger.
3. In the hotfix startup: `CreateLogger<THotfixLogger>()` for every hotfix logger.
4. Call `LoggingLauncher.TryLoadLoggers()` after creating loggers (each time the logger set changes).
5. Give every logger a public default constructor (`Activator.CreateInstance` is used).

## Rules & pitfalls

- Adding or removing a logger at runtime requires `LoggingLauncher.TryLoadLoggers()` to reload settings.
- README/older docs may show `LoggerSetting`; the actual class is `LoggerSettings`.
- `Logging.Print<T>` maps to LogDebug; `PrintInfo` to LogInfo, etc. Choose levels so release builds can keep only `LogWarning | LogError | LogException`.
- Do not wrap Logging calls in your own `#if` guards for players; the package already compiles them out without `OXGKIT_LOGGER_ON`.
- The config file in `StreamingAssets` is loaded via web request (works on Android/WebGL); if absent, inspector/default settings apply.

## Verify

- Editor: enter Play mode and confirm `[LoggingSystem] is Initialized.` appears and logger output respects the launcher settings.
- Player: build with `OXGKIT_LOGGER_ON`, then toggle levels via `LoggerConfig.dat` and re-run to confirm filtering.
