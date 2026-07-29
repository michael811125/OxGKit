---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.Utilities, including Requester web requests (audio/texture2d/sprite/bytes/text with ARC/LRU caching), LRUCache/LRUKCache/ARCCache, UISafeAreaAdapter, EasyAnim (EasyAnimation/EasyAnimator with the AnimEnd event), TextureAnimation image-sequence playback, DontDestroy, UnityMainThread UMT dispatcher, and the RectTransformAdjuster / MissingScriptsFinder / SymlinkUtility editor tools.
---

# OxGKit.Utilities Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.Utilities user. The module (UPM package `com.michaelo.oxgkit.utilities`, UniTask-based) is a grab-bag of essential runtime helpers — each in its own `OxGKit.Utilities.<Sub>` namespace — plus editor tools. Pick the sub-tool that fits; they are independent.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.utilities@*` or `Assets/OxGKit/Utilities/Scripts` when embedded). Do not invent APIs; signatures below are the verified core surface.
4. On WebGL, call out download/audio limitations and avoid thread-based paths.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/Utilities/Scripts`
- Auto dependencies: `com.cysharp.unitask`, `com.domybest.lwmybox`, `com.michaelo.oxgkit.loggingsystem`.
- Samples (Package Manager > OxGKit.Utilities > Samples): `AI Agent Skills` (this skill).

## Module map

| Namespace | Types | Use for |
| --- | --- | --- |
| `OxGKit.Utilities.Requester` | `Requester` | UnityWebRequest downloads with in-memory caching |
| `OxGKit.Utilities.Cacher` | `LRUCache<K,V>`, `LRUKCache<K,V>`, `ARCCache<K,V>`, `IRemoveCacheHandler`, `UnityObjectRemoveCacheHandler` | capacity-bounded caches |
| `OxGKit.Utilities.Adapter` | `UISafeAreaAdapter` | notch-safe UI panels |
| `OxGKit.Utilities.EasyAnim` | `EasyAnim` (base), `EasyAnimation`, `EasyAnimator` | play-then-callback animation wrapper |
| `OxGKit.Utilities.TextureAnim` | `TextureAnimation` | CPU image-sequence (flipbook) playback |
| `OxGKit.Utilities.DontDestroy` | `DontDestroy` | mark a root object persistent + runtime rename |
| `OxGKit.Utilities.UnityMainThread` | `UMT` | run jobs/coroutines on the Unity main thread |

## Requester (verified)

Static async download helpers with optional per-type caching (ARC or LRU; keyed by URL):

```csharp
using OxGKit.Utilities.Requester;
using UnityEngine;

// Optional: initialize ONE cache flavor per resource type at boot (ARC or LRU, not both)
Requester.InitARCCacheCapacityForAudio(20);
Requester.InitARCCacheCapacityForTexture2d(60);
Requester.InitARCCacheCapacityForText(100);
// or Requester.InitLRUCacheCapacityFor...(...)

AudioClip clip  = await Requester.RequestAudio(url, AudioType.MPEG, successAction, errorAction, cts, cached: true, timeoutSeconds: null);
Texture2D tex   = await Requester.RequestTexture2D(url, successAction, errorAction, cts, cached: true);
Sprite sprite   = await Requester.RequestSprite(url, successAction, errorAction, position, pivot, pixelPerUnit: 100, ...);
byte[] bytes    = await Requester.RequestBytes(url, successAction, errorAction, cts);   // never cached
string text     = await Requester.RequestText(url, successAction, errorAction, cts, cached: true);

// Cache eviction
Requester.RemoveFromARCCacheForTexture2d(url);   // per-flavor removes
Requester.AutoRemoveFromCaches(url);             // try every cache
Requester.ClearARCCacheCapacityForText();        // clear + drop a cache
```

Notes: errors arrive via `errorAction(ErrorInfo)` and the task returns null; pass a `CancellationTokenSource` for lifecycle-bound requests; default request timeout guard is 180s. Instance variants (`SelfRequest*` on `new Requester()`) exist for isolated cache scopes.

## Cacher (verified)

```csharp
using OxGKit.Utilities.Cacher;

var cache = new LRUCache<string, Texture2D>(capacity: 60);            // or LRUKCache(capacity, k), ARCCache(capacity)
var cache2 = new LRUCache<string, Texture2D>(60, new UnityObjectRemoveCacheHandler<string, Texture2D>()); // destroys evicted Unity objects
cache.Add(key, value);
Texture2D v = cache.Get(key);      // null/default when missing
cache.Contains(key); cache.Remove(key); cache.Clear(); cache.Count; cache.GetKeys();
```

Use `ARCCache` for mixed recency/frequency patterns, `LRUCache` for plain recency, `LRUKCache` for scan-resistant promotion after K hits. Provide an `IRemoveCacheHandler` when evicted values need explicit destruction (Unity objects).

## EasyAnim (verified)

```csharp
using OxGKit.Utilities.EasyAnim;

EasyAnim ea = go.GetComponent<EasyAnim>();  // EasyAnimation (legacy Animation) or EasyAnimator (Animator)
ea.Play("Open", () => this.OnOpened());     // EasyAnimation: clip name; EasyAnimator: TRIGGER parameter name
bool has = ea.HasAnim("Open");
```

**Required setup**: the clip must contain an Animation Event named `AnimEnd` — that event invokes the completion callback. Missing param/clip names call the callback immediately (fail-open).

## Other runtime helpers (verified)

```csharp
// UISafeAreaAdapter — put on a panel; fits `panel` RectTransform to Screen.safeArea.
// refreshAlways = true re-applies every frame (orientation changes); v1.4.8+ logs only when
// the safe area actually changed (no per-frame string allocations); RefreshViewSize() manual.

// TextureAnimation — flipbook on a Renderer/RawImage from Texture2D frames:
texAnim.SetFrameRate(12);
texAnim.SetAnimationEnd(() => Done());     // fires when a non-looping run completes
texAnim.IsAnimationComplete(); texAnim.ResetAnim(); texAnim.SetIgnoreScale(true);

// DontDestroy — component: marks the object DontDestroyOnLoad in Awake and renames it; SetRuntimeName(name).

// UMT — main-thread dispatcher (auto-creates a persistent [UMT] object):
using OxGKit.Utilities.UnityMainThread;
UMT.worker.AddJob(() => { /* runs on next Update, safe from any thread */ });
UMT.worker.RunCoroutine(routine);          // + CancelCoroutine / CancelAllCoroutines / Clear
```

## Editor tools

- **RectTransformAdjuster** — `GameObject > Adjust RectTransform Anchors (Shift+R)`: snaps the selection's anchors to its current rect (great for prefab layout cleanup).
- **MissingScriptsFinder** — find/clean missing MonoBehaviours in scenes/prefabs.
- **SymlinkUtility** — `Assets/Create > Folder (Absolute/Relative Symlink, Junction)` for linking shared asset folders.

## Rules & pitfalls

- Initialize Requester caches once at boot; init calls with a different capacity re-create the cache (dropping entries). Uncached or evicted Unity objects you no longer need must be destroyed by the caller — cached ones belong to the cache.
- `RequestAudio` needs the right `AudioType` for the file; WebGL streaming audio has platform limits — verify formats there.
- Cancel in-flight requests (`cts.Cancel()`) when the owner dies, or callbacks fire into released objects.
- `EasyAnim` without the `AnimEnd` event never completes-by-event — the callback simply won't fire; this is the first thing to check when "animation callback doesn't run".
- `UMT.worker` lazily creates a DontDestroy object — acceptable in gameplay, but avoid first-touching it from teardown paths.
- Editor tools live in the Editor assembly — never reference them from runtime code.

## Verify

- Compile, then exercise the specific helper: a cached second `RequestTexture2D` returns instantly (no network log), safe-area panel fits on a notched device simulator, `AnimEnd`-tagged clips fire callbacks, and UMT jobs queued from a worker thread execute on the main thread.
