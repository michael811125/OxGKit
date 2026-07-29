---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.PoolSystem, including the NodePool GameObject pool component, across-frames (load-balanced) initialization, Get/Put recycling, autoPut growth, maxSize limiting, and pool lifecycle/cleanup.
---

# OxGKit.PoolSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.PoolSystem user. The module (UPM package `com.michaelo.oxgkit.poolsystem`, UniTask-based) provides **`NodePool`** — a simple component-based GameObject pool with smoothed (across-frames) instantiation for load balancing, optional auto-growth, and a max-size cap.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.poolsystem@*` or `Assets/OxGKit/PoolSystem/Scripts` when embedded). Do not invent APIs.
4. Identify the pool owner (which scene object/system hosts the `NodePool`) and its destroy path before wiring pooled objects.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/PoolSystem/Scripts`
- Auto dependencies: `com.cysharp.unitask`, `com.domybest.lwmybox`, `com.michaelo.oxgkit.loggingsystem`.
- Samples (Package Manager > OxGKit.PoolSystem > Samples): `NodePool Demo`, `AI Agent Skills` (this skill).
- Component: `Add Component > OxGKit > PoolSystem > NodePool`.

## Inspector fields (verified)

| Field | Meaning |
| --- | --- |
| `go` | source GameObject to instantiate (required) |
| `initializeOnStart` | auto-`Initialize()` on `Start` (default true) |
| `initSize` | initial pooled count (default 5) |
| `initLoadAcrossFrames` | spread initial instantiation across frames (default true) |
| `initDelayFrameAfterSpawnCount` | spawn N objects, then wait (default 1) |
| `initDelayFrame` | frames to wait between batches (default 1) |
| `autoPut` | when `Get()` finds the pool empty, auto-create more (default false) |
| `autoPutSize` | how many to create per auto-grow (default 1) |
| `autoPutLoadAcrossFrames` (+ batch/delay fields) | across-frames settings for auto-grow |
| `maxSize` | `0` / `-1` = unlimited; `> 0` caps total pooled count (init + autoPut; excess `Put` destroys) |

## Core API (verified)

```csharp
using OxGKit.PoolSystem;
using UnityEngine;

NodePool pool;

pool.Initialize();          // Clear() then (re)create initSize objects (async when across-frames)
pool.IsLoadFinished();      // init/auto-put instantiation finished?
pool.Count();               // current pooled count

GameObject a = pool.Get();                                 // null if empty and no autoPut
GameObject b = pool.Get(parent);
GameObject c = pool.Get(parent, localPosition);
GameObject d = pool.Get(parent, localPosition, rotation);  // rotation is world-space

pool.Put(a);                // deactivate + re-parent under the pool node
pool.Clear();               // cancel pending creation, destroy pooled objects (also runs OnDestroy)
```

Behavior notes:

- `Get()` activates the object and detaches it (parentless overload sets parent null); `Put()` deactivates and re-parents it under the pool's transform.
- With `autoPut` on and the pool empty, `Get()` synchronously creates at least the first object of the growth batch (the rest may fill across later frames), so it still returns an object immediately.
- `Put()` beyond `maxSize` destroys the object instead of pooling it (a log line notes it).

## Usage pattern

```csharp
public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private NodePool _pool;   // NodePool on a stable owner object; go = bullet prefab

    private void Fire(Transform muzzle)
    {
        GameObject bullet = this._pool.Get(null, Vector3.zero);
        if (bullet == null) return;                    // pool empty, no autoPut
        bullet.transform.position = muzzle.position;
        bullet.GetComponent<Bullet>().Launch(b => this._pool.Put(b.gameObject)); // recycle on death
    }
}
```

Reset pooled-object state on reuse — either in an `OnEnable` on the pooled object (activation happens on `Get`) or explicitly right after `Get()`.

## Rules & pitfalls

- One `NodePool` pools exactly one source prefab; use one pool per prefab type.
- Pooled objects must be **stateless on reuse**: reset velocity/trail/coroutines on `Get`/`OnEnable`; never assume freshly-instantiated state.
- Do not `Destroy` a pooled object yourself — always `Put()` it back; destroyed objects silently shrink the pool.
- Gate heavy startup on `IsLoadFinished()` when `initLoadAcrossFrames` is on and you need the full pool warm (e.g., before a wave spawns).
- The pool destroys its contents in `OnDestroy` — pooled objects must not be needed after the owner dies; conversely, `Put()` after the owner died would re-parent onto a dead transform (guard feature shutdown order).
- `maxSize` counts pooled (idle) objects; in-flight objects returning to a full pool get destroyed — size the cap to peak concurrency, not average.

## Verify

- Enter Play mode: watch the pool node fill across frames, `Get`/`Put` round-trips keep `Count()` stable, and no `Instantiate` spikes appear in the Profiler during steady state.
- With `maxSize` set, confirm excess `Put` logs and destroys rather than leaking.
