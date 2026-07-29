---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.LocalizationSystem, including the Localization static class, LangType languages, the required onAddSupportedLanguages / onParsingLanguageData / onChangeLanguage callbacks, ChangeLanguage flow, GetStringByCode text lookup, system-language detection, and persisting the selected language.
---

# OxGKit.LocalizationSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.LocalizationSystem user. The module (UPM package `com.michaelo.oxgkit.localizationsystem`, dependency-free) is a callback-driven localization core: the project supplies the language table (from JSON, spreadsheet exports, a server, a game DB, ...) through callbacks, and `Localization` handles supported-language bookkeeping, language switching, and code→text lookup.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.localizationsystem@*` or `Assets/OxGKit/LocalizationSystem/Scripts` when embedded) and inspect how the target project stores its language table before writing code.
4. Never hardcode display strings that need localization; route them through `Localization.GetStringByCode`.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/LocalizationSystem/Scripts`
- No dependencies.
- Samples (Package Manager > OxGKit.LocalizationSystem > Samples): `Localization Demo`, `AI Agent Skills` (this skill).

## Core API (verified)

```csharp
using OxGKit.LocalizationSystem;

// Required callbacks (assign BEFORE ChangeLanguage)
Localization.onAddSupportedLanguages; // Action<HashSet<LangType>>: declare supported languages
Localization.onParsingLanguageData;   // Func<LangType, Dictionary<string, string>, bool>: fill code→text for a language, return true on success
Localization.onChangeLanguage;        // Action<LangType>: notified after a successful switch (refresh UI here)

// State & queries
Localization.currentLanguage;                       // LangType (read-only)
Localization.systemLanguage;                        // OS language mapped to LangType
Localization.GetSystemLanguageToLangType();
Localization.GetSupportedLanguages();               // HashSet<LangType>
Localization.IsSupportedLanguage(langType);
Localization.GetAndCheckIsSupportedLanguage(langType); // falls back when unsupported
Localization.GetSupportedLanguagesMappingByLangType(); // Dictionary<LangType, string> (desc)
Localization.GetSupportedLanguagesMappingByLangDesc(); // Dictionary<string, LangType>

// Actions
Localization.ChangeLanguage(langType);   // parses table via onParsingLanguageData, then fires onChangeLanguage
Localization.GetStringByCode("ui.start"); // text for current language ("Unknown Text" when the code is missing)
```

`LangType` is a `byte` enum of world languages (`Unspecified`, `Arabic`, `ChineseSimplified`, `ChineseTraditional`, `Dutch`, `English`, `French`, `German`, `Italian`, `Portuguese`, `Spanish`, `Japanese`, `Korean`, `Russian`, ... — check `Languages.cs` for the full list). `LanguageMapping.GetLanguageDesc(langType)` returns a human-readable description.

## Standard setup flow

```csharp
using System.Collections.Generic;
using OxGKit.LocalizationSystem;

public static class LocalizationInitializer
{
    public static void Init()
    {
        // 1) Declare supported languages
        Localization.onAddSupportedLanguages = supported =>
        {
            supported.Add(LangType.English);
            supported.Add(LangType.ChineseTraditional);
            supported.Add(LangType.ChineseSimplified);
            supported.Add(LangType.Japanese);
        };

        // 2) Provide table parsing (from your own storage: JSON, GameDB, server, ...)
        Localization.onParsingLanguageData = (langType, langData) =>
        {
            var sheet = LoadYourLanguageSheet(); // Dictionary<code, Dictionary<langKey, text>>
            foreach (var row in sheet)
                langData.TryAdd(row.Key, row.Value.GetValueOrDefault(langType.ToString()));
            return true; // false / unassigned callback => ChangeLanguage throws
        };

        // 3) React to switches (refresh all visible localized UI)
        Localization.onChangeLanguage = langType => RefreshAllLocalizedViews();

        // 4) Restore persisted choice (fall back to system language)
        var saved = (LangType)PlayerPrefs.GetInt("language", (int)Localization.systemLanguage);
        Localization.ChangeLanguage(Localization.GetAndCheckIsSupportedLanguage(saved));
    }
}
```

UI reads text only through codes:

```csharp
this.titleText.text = Localization.GetStringByCode("ui.title");
```

Language menu switching + persistence (pair well with OxGKit.SaverSystem or your settings facade):

```csharp
Localization.ChangeLanguage(LangType.Japanese);
PlayerPrefs.SetInt("language", (int)Localization.currentLanguage);
```

## Rules & pitfalls

- Assign `onAddSupportedLanguages` and `onParsingLanguageData` **before** the first `ChangeLanguage`; an unassigned/failing parser makes `ChangeLanguage` throw.
- Load whatever data source the parser needs (game DB, downloaded tables) **before** initializing localization.
- `GetStringByCode` throws if no language table has been parsed yet, and returns `"Unknown Text"` for missing codes — treat that string in QA as "missing key".
- `ChangeLanguage` with an unsupported `LangType` only logs a warning and keeps the current language; use `GetAndCheckIsSupportedLanguage` to sanitize saved/system values first.
- `onChangeLanguage` handlers must be repeat-safe and only redraw text/data (no one-time event registration inside).
- The module does not persist the selection — the project owns saving/restoring the chosen `LangType`.

## Verify

- Enter Play mode, switch through every supported language, and confirm all visible text refreshes and no `"Unknown Text"` shows up.
- Cold-start with a saved unsupported/legacy language value and confirm the fallback path works.
