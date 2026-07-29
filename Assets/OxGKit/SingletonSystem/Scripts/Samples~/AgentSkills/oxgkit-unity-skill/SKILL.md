---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.SingletonSystem, including MonoSingleton<T> (MonoBehaviour singleton with OnCreate/OnStart/OnRelease lifecycle, GetInstance/InitInstance, dontDestroyOnLoad, DestroyInstance) and NewSingleton<T> (plain-class singleton).
---

# OxGKit.SingletonSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.SingletonSystem user. The module (UPM package `com.michaelo.oxgkit.singletonsystem`, dependency-free) provides two singleton bases: **`MonoSingleton<T>`** for MonoBehaviour systems (find-or-create, optional DontDestroyOnLoad, guarded lifecycle hooks) and **`NewSingleton<T>`** for plain C# classes.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.singletonsystem@*` or `Assets/OxGKit/SingletonSystem/Scripts` when embedded). Do not invent APIs.
4. Challenge singleton usage that doesn't fit: short-lived UI, pooled cells, and per-stage temporary state should not be singletons.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/SingletonSystem/Scripts`
- No dependencies.
- Samples (Package Manager > OxGKit.SingletonSystem > Samples): `MonoSingleton Demo` (two scenes showing cross-scene persistence), `AI Agent Skills` (this skill).
- Script template: right-click `Assets/Create/OxGKit/SingletonSystem/Template Mono Singleton.cs`.

## MonoSingleton&lt;T&gt; (verified)

```csharp
using OxGKit.SingletonSystem;

public class GameSystem : MonoSingleton<GameSystem>
{
    protected override void OnCreate()  { /* Unity Awake */ }
    protected override void OnStart()   { /* Unity Start */ }
    protected override void OnRelease() { /* Unity OnDestroy */ }
}

// Access / lifecycle
GameSystem.GetInstance();                    // find-or-create; param dontDestroyOnLoad = true by default
GameSystem.InitInstance();                   // alias for warm-up
GameSystem.CheckInstanceExists();
GameSystem.DestroyInstance(gameObjectIncluded: true);

// Static state flags
GameSystem.isCreated; GameSystem.isStarted; GameSystem.isReleased;

// Instance field
instance.dontDestroyOnLoad;                  // also settable on a scene-placed instance in the inspector
```

Behavior:

- `GetInstance(dontDestroyOnLoad = true)` (play mode only): finds an existing scene instance or creates a `new GameObject(typeof(T).Name)` with the component; applies `DontDestroyOnLoad` when requested.
- A scene-placed instance registers itself in `Awake` and honors its serialized `dontDestroyOnLoad` field.
- Lifecycle hooks are one-shot-guarded: `OnCreate()` from `Awake`, `OnStart()` from `Start`, `OnRelease()` from `OnDestroy` (flags reset so a new instance can be created later).
- **Do not override `Awake()`, `Start()`, or `OnDestroy()`** — they are private in the base; every other Unity message (`Update`, `OnEnable`, ...) is yours to implement normally.

## NewSingleton&lt;T&gt; (verified)

```csharp
using OxGKit.SingletonSystem;

public class ScoreService
{
    // Access via the generic holder:
}
ScoreService svc = NewSingleton<ScoreService>.GetInstance();  // lazy new T()
NewSingleton<ScoreService>.InitInstance();
NewSingleton<ScoreService>.CheckInstanceExists();
NewSingleton<ScoreService>.DestroyInstance();                 // just clears the static ref
```

`T` needs a public parameterless constructor (`where T : class, new()`).

## Usage patterns

Boot warm-up (explicit creation order beats lazy surprises):

```csharp
private void Awake()
{
    GameSystem.InitInstance();
    AudioSystem.InitInstance();
}
```

Scene-scoped singleton (dies with the scene):

```csharp
StageSystem.GetInstance(dontDestroyOnLoad: false);
```

Safe access from teardown paths:

```csharp
if (GameSystem.CheckInstanceExists())
    GameSystem.GetInstance().Save();
```

## Rules & pitfalls

- The `dontDestroyOnLoad` decision is applied on **first creation** — decide it at the first `GetInstance`/`InitInstance` call site (or on the scene-placed instance) and keep it consistent.
- `GetInstance()` from `OnApplicationQuit`/`OnDestroy` teardown can re-create objects mid-quit — guard with `CheckInstanceExists()` (or check `isReleased`).
- Registering event subscriptions in `OnCreate`/`OnStart` requires unsubscribing in `OnRelease`; singletons outlive scenes and accumulate stale delegates otherwise.
- Two scene-placed instances of the same `T` are not auto-deduplicated — the first found wins as `_instance`; avoid placing duplicates across additively-loaded scenes.
- `MonoSingleton` is for stable core systems (audio, save, input hub). UI panels, pooled items, per-stage helpers: plain components instead.
- `NewSingleton<T>.DestroyInstance()` only nulls the holder — the old object survives if referenced elsewhere; it is not a disposal mechanism.

## Verify

- Play the two demo scenes: the singleton persists across scene loads when `dontDestroyOnLoad` is true, `OnCreate/OnStart` fire once, and `OnRelease` fires on destroy.
- Domain-reload check: with Enter Play Mode Options (no domain reload), statics persist — confirm `DestroyInstance`/flags behave across repeated plays.
