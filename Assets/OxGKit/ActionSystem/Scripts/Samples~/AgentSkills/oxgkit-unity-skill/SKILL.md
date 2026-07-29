---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.ActionSystem, including ActionRunner update loops, composing SequenceAction / ParallelAction / ParallelDelayAction / DelayAction / DelegateAction, authoring custom ActionBase subclasses with SetDuration and MarkAsDone, and building timed/chained gameplay or animation flows.
---

# OxGKit.ActionSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.ActionSystem user. The module (UPM package `com.michaelo.oxgkit.actionsystem`, UniTask-based) is a lightweight action-sequence framework: compose actions (delays, callbacks, sequences, parallels, custom actions) into trees and pump them from any update loop via an `ActionRunner`. Good for scripted flows, cutscene-ish sequencing, and stitching timed logic without coroutines.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.actionsystem@*` or `Assets/OxGKit/ActionSystem/Scripts` when embedded). Do not invent APIs.
4. Always identify who owns the runner's `OnUpdate(dt)` call and where `Release()` happens before writing action code.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/ActionSystem/Scripts`
- Auto dependencies: `com.cysharp.unitask`, `com.michaelo.oxgkit.loggingsystem`.
- Samples (Package Manager > OxGKit.ActionSystem > Samples): `ActionSystem Demo`, `AI Agent Skills` (this skill).
- Script template: right-click `Assets/Create/OxGKit/Action System/Template Action.cs` for a custom action skeleton.

## Core API (verified)

```csharp
using OxGKit.ActionSystem;

// Runner — owns and updates running actions
var runner = new ActionRunner("FeatureActions");
runner.RunAction(action);      // resets the runner, then starts this action
runner.QueueAction(action);    // chainable; queued actions start on the next OnUpdate
runner.RemoveAction(uid);      // remove by ActionBase.uid (marks it all-done)
runner.OnUpdate(dt);           // pump from Update()/RTUpdater/etc.
runner.Release();              // clear everything (call on feature exit)

// ActionBase — public surface
action.uid;  action.name;
action.IsStarted(); action.IsDone(); action.IsAllDone();
action.MarkAsDone();           // finish this action (fires OnDone)
action.MarkAllDone();          // finish including sub actions
action.SetDuration(seconds);   // -1 = runs until MarkAsDone() is called manually
action.GetTimeElapsedRatio();  // 0..1 based on duration
```

Default actions:

```csharp
// Delay then optional callback
DelayAction.CreateDelayAction(1.5f, onEndAction: null);

// Invoke a callback (optionally after a delay — returns a SequenceAction in that case)
DelegateAction.CreateDelegateAction(() => DoSomething(), delayTime: 0f);

// Sequence — runs children one by one
var seq = new SequenceAction("OpenFlow");
seq.AddAction(DelegateAction.CreateDelegateAction(this.OpenUI));
seq.AddAction(DelayAction.CreateDelayAction(1f));
seq.AddAction(DelegateAction.CreateDelegateAction(this.PlayDone));

// Parallel — runs children simultaneously, done when all are done
var par = new ParallelAction("Burst");
par.AddAction(a1).AddAction(a2);

// ParallelDelay — starts children in parallel with a stagger between starts
var stagger = new ParallelDelayAction(0.2f);
stagger.AddAction(a1).AddAction(a2);

runner.RunAction(seq);
```

## Driving the runner

```csharp
public class FeatureController : MonoBehaviour
{
    private readonly ActionRunner _runner = new ActionRunner("Feature");

    private void Update() => this._runner.OnUpdate(Time.deltaTime);

    private void OnDestroy() => this._runner.Release();
}
```

Any loop works (MonoBehaviour `Update`, OxGKit `RTUpdater`, a stage system tick) — actions only advance while `OnUpdate(dt)` is called.

## Custom actions

```csharp
using OxGKit.ActionSystem;

public class WaitForServerAction : ActionBase
{
    public WaitForServerAction() { this.name = nameof(WaitForServerAction); }

    protected override void OnStart()
    {
        this.SetDuration(-1f);                    // callback decides completion
        Server.Request(ok => this.MarkAsDone());  // finish when the callback fires
    }

    protected override void OnUpdate(float dt) { /* optional per-frame work */ }

    protected override void OnDone() { /* fired by MarkAsDone */ }
}
```

Timed custom action: call `SetDuration(seconds)` in `OnStart()`; the base class auto-marks done when elapsed. Use `GetTimeElapsedRatio()` for progress-driven effects (e.g., lerping).

## Rules & pitfalls

- `RunAction` **resets the runner** (clears running/queued actions) before starting the new action — use `QueueAction` to append instead of replace.
- A runner that nobody pumps does nothing; a runner pumped twice per frame runs double speed. One owner per runner.
- `SetDuration(-1)` actions never finish unless `MarkAsDone()` / `MarkAllDone()` is called — guarantee the callback path fires (including error paths).
- Actions are stateful one-shots; reuse only via re-running (`RunStart` resets internal state) and never share one action instance across two runners simultaneously.
- Call `runner.Release()` when the owning stage/UI/feature exits, or pending callbacks may fire into released objects.
- `DelegateAction.CreateDelegateAction(action, delayTime > 0)` returns a `SequenceAction` wrapper — its static type is `ActionBase`, don't cast it to `DelegateAction`.

## Verify

- Compile, run the flow, and confirm the sequence order/timing in logs (the runner reports done actions through OxGKit LoggingSystem: enable its logger to trace).
- Exit the feature mid-sequence and confirm no callbacks fire afterwards (Release path).
