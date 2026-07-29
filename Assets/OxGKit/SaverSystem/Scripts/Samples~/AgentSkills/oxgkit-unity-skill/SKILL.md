---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.SaverSystem, including the ISaver interface, the abstract Saver base with text-content data maps (SaveData/GetData/DeleteData/DeleteContext/ParsingDataMap), PlayerPrefsSaver, the editor-only EditorPrefsSaver, and custom Saver implementations for settings persistence.
---

# OxGKit.SaverSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.SaverSystem user. The module (UPM package `com.michaelo.oxgkit.saversystem`, dependency-free) is a small key/value persistence layer: `ISaver` defines string/int/float storage, the abstract `Saver` adds a **text-content data map** feature (many `key value` pairs packed into one stored string, with parse caching), and two implementations ship: `PlayerPrefsSaver` (runtime) and `EditorPrefsSaver` (editor). It targets settings and small data — not full save-game databases.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.saversystem@*` or `Assets/OxGKit/SaverSystem/Scripts` when embedded). Do not invent APIs.
4. Check whether the target project wraps saving in a facade (e.g., a `GameSettings` class with a shared `Saver` instance and centralized keys) and extend that instead of scattering raw calls.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/SaverSystem/Scripts`
- No dependencies. Includes editor tests.
- Samples (Package Manager > OxGKit.SaverSystem > Samples): `AI Agent Skills` (this skill).

## Core API (verified)

```csharp
using OxGKit.SaverSystem;

// Runtime implementation (PlayerPrefs-backed; every Save* also calls PlayerPrefs.Save())
Saver saver = new PlayerPrefsSaver();

// ISaver surface
saver.SaveString("name", "michael");
saver.GetString("name", defaultValue: "");
saver.SaveInt("language", 2);
saver.GetInt("language", defaultValue: 0);
saver.SaveFloat("bgmVolume", 0.8f);
saver.GetFloat("bgmVolume", defaultValue: 0f);
saver.HasKey("language");
saver.DeleteKey("language");
saver.DeleteAll();

// Text-content data map (many key/value pairs inside ONE stored string)
saver.SaveData("appPrefs", "lastLogin", "2026-07-29");  // contentKey, key, value
saver.GetData("appPrefs", "lastLogin", defaultValue: null); // parse-cached (dirty-flag) lookups
saver.DeleteData("appPrefs", "lastLogin");
saver.DeleteContext("appPrefs");                         // remove the whole content + its cache

// Static parser for the same "key value" line format
Dictionary<string, string> map = Saver.ParsingDataMap(contentText);

saver.Dispose(); // drops the parse caches
```

Editor-only implementation (for editor tools/windows):

```csharp
using OxGKit.SaverSystem.Editor;
Saver editorSaver = new EditorPrefsSaver(); // EditorPrefs-backed
```

## Text-content format

`SaveData` stores lines of `key value` inside one string value:

```
# comment lines start with '#'
lastLogin 2026-07-29
resolution 1920x1080
```

- Split on the **first space**: keys must not contain spaces; values may contain spaces.
- Lines starting with `#` are ignored by `ParsingDataMap` — the format doubles as a hand-editable config format (OxGFrame uses `Saver.ParsingDataMap` for its media URL config files).
- `GetData` caches the parsed map per `contentKey` and re-parses only after a `SaveData`/`DeleteData` (dirty flag).

## Custom Saver

Persist anywhere (file, encrypted storage, cloud) by overriding the abstract members — the data-map feature comes free:

```csharp
public class FileSaver : Saver
{
    public override void SaveString(string key, string value) { /* write file */ }
    public override string GetString(string key, string defaultValue = "") { /* read */ return defaultValue; }
    public override void SaveInt(string key, int value) => this.SaveString(key, value.ToString());
    public override int GetInt(string key, int defaultValue = 0) => int.TryParse(this.GetString(key), out var v) ? v : defaultValue;
    public override void SaveFloat(string key, float value) => this.SaveString(key, value.ToString());
    public override float GetFloat(string key, float defaultValue = 0f) => float.TryParse(this.GetString(key), out var v) ? v : defaultValue;
    public override bool HasKey(string key) { /* ... */ return false; }
    public override void DeleteKey(string key) { /* ... */ }
    public override void DeleteAll() { /* ... */ }
}
```

## Recommended project pattern

Centralize keys and the saver instance behind a settings facade:

```csharp
public static class GameSettings
{
    private static readonly Saver _saver = new PlayerPrefsSaver();
    private const string KEY_LANGUAGE = "language";

    public static int Language
    {
        get => _saver.GetInt(KEY_LANGUAGE, 0);
        set => _saver.SaveInt(KEY_LANGUAGE, value);
    }
}
```

## Rules & pitfalls

- Scope: settings and small key/value data. For real save games (large/structured/versioned data), use a stronger storage layer; don't grow data maps into a database.
- There is no bool/JSON API — store bools as int (0/1) and objects as JSON strings via `SaveString` if needed.
- Data-map keys must not contain spaces; values must not contain newlines (line-based format).
- `EditorPrefsSaver` lives in the `OxGKit.SaverSystem.Editor` namespace/assembly — never reference it from runtime code.
- PlayerPrefs platform caveats apply (registry on Windows, browser storage on WebGL — WebGL persistence needs the browser to flush; avoid huge strings).
- Reuse one `Saver` instance (data-map caching is per instance); creating a new saver per call defeats the cache.

## Verify

- Editor tests exist under the package's `Tests` folder (SaverTests) — run them via Test Runner after modifying save logic.
- Play, set values, stop, replay: values persist; `DeleteContext` clears the whole map and `GetData` falls back to defaults.
