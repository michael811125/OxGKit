---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.TweenSystem (DOTween Pro based), including the DoTweenAnim component (position/rotation/scale/size/alpha/image-color/sprite-color tweens with Normal/Reverse/PingPong/Sequence play modes), the DoTweenAnimEvent aggregator (PlayNormal/PlayReverse/PlayTrigger with end callbacks), DOTween assembly setup, and the DOTween.Modules GUID fix.
---

# OxGKit.TweenSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.TweenSystem user. The module (UPM package `com.michaelo.oxgkit.tweensystem`) provides designer-authorable tween components on top of **DOTween Pro**: `DoTweenAnim` holds per-transform tween tracks configured in the inspector (with editor preview), and `DoTweenAnimEvent` groups multiple `DoTweenAnim`s and plays them from code or UnityEvents with end callbacks.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.tweensystem@*` or `Assets/OxGKit/TweenSystem/Scripts` when embedded) and check the DOTween setup state before diagnosing compile errors.
4. Prefer inspector-authored `DoTweenAnim` tracks for reusable prefab animation; write raw DOTween code only when code-driven animation is genuinely simpler.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/TweenSystem/Scripts`
- Auto dependencies: `com.domybest.lwmybox`, `com.michaelo.oxgkit.timesystem`.
- Manual dependency: **DOTween Pro** (paid asset) must be imported, and DOTween assemblies created via `Tools > Demigiant > DOTween Utility Panel > Create ASMDEF`.
- GUID note: `OxGKit.TweenSystem` references `DOTween.Modules` by a fixed GUID (`fdf3e181e62e9d243a7fee5ce890ab71`). If the assembly reference breaks (DOTween regenerated its asmdef with a new GUID), fix it by editing `DOTween/Modules/DOTween.Modules.asmdef.meta` to that GUID, or install `OxGKit.TweenSystemFixer` and run its menu tool.
- Samples (Package Manager > OxGKit.TweenSystem > Samples): `TweenSystem Demo`, `AI Agent Skills` (this skill).
- Components: `Add Component > OxGKit > TweenSystem > DoTweenAnim` / `DoTweenAnimEvent`.

## DoTweenAnim (per-object tween tracks)

Inspector-driven; each enabled track tweens one property:

- Tracks: `tPositionOn`, `tRotationOn`, `tScaleOn`, `tSizeOn` (RectTransform size), `tAlphaOn` (CanvasGroup alpha), `tImgColorOn` (Image color), `tSprColorOn` (SpriteRenderer color).
- Per track: `from`/`to` (or a `Sequence` list for `PlayMode.Sequence`), `duration`, `easeMode`, `loopTimes`/`loopType`, `playMode` (`Normal`/`Reverse`/`PingPong`/`Sequence`), optional interval, `ignoreTimeScale` (default true), `endCallback`, `autoActive`.
- `driveMode`:
  - `Active` — plays automatically when the GameObject becomes active (SetActive-driven UI open/close animation).
  - `Event` — plays only when told to (via `DoTweenAnimEvent` or code).
- Editor **preview mode** is supported on `DoTweenAnim` (inspector Play/Stop preview); `DoTweenAnimEvent` plays only at runtime.

Code surface (usually you call `DoTweenAnimEvent` instead):

```csharp
using OxGKit.TweenSystem;

doTweenAnim.InitTweens();
doTweenAnim.PlayTween(trigger: true, endCallback);  // true = forward, false = backward
doTweenAnim.ResetTweens();
```

## DoTweenAnimEvent (group player)

```csharp
using OxGKit.TweenSystem;
using DG.Tweening;

DoTweenAnimEvent anim; // holds List<DoTweenAnim> doTweenAnims

anim.SetPlayMode(DoTweenAnimEvent.PlayMode.Parallel); // Parallel | Sequence
anim.AddDoTweenAnim(a1, a2);                          // chainable
anim.RemoveDoTweenAnim(a1);
anim.ClearDoTweenAnims();

anim.PlayNormal();                        // forward
anim.PlayNormal(this.OnOpenEnd);          // forward + end callback (all anims done)
anim.PlayReverse(this.OnCloseEnd);        // backward + end callback
anim.PlayTrigger();                       // flip direction each call
anim.PlayTrigger(true, this.OnEnd);       // explicit direction + callback
anim.PlayTriggerOnce(true);               // play only if direction changes
anim.ResetTrigger();
anim.Kill();                              // kill running tweens
```

Typical UI open/close:

```csharp
// Open
this._panelAnim.PlayNormal(() => this.OnPanelShown());
// Close
this._panelAnim.PlayReverse(() => this.gameObject.SetActive(false));
```

## Rules & pitfalls

- DOTween Pro + created ASMDEF assemblies are prerequisites; without them the package cannot compile. After every DOTween re-import/upgrade, re-check the `DOTween.Modules` GUID (see Install).
- `driveMode = Active` replays on every activation — do not also drive the same `DoTweenAnim` from a `DoTweenAnimEvent` (pick one drive mode per component).
- End callbacks fire when **all** grouped anims finish; guard callbacks against destroyed/released UI (`Kill()` or clear callbacks on release).
- `ignoreTimeScale` defaults to true per track — tweens run in pause menus unless you disable it.
- For `Sequence` play mode, author the waypoint list in the track's sequence; `from`/`to` are ignored for that mode.
- Editor preview only works on `DoTweenAnim`; runtime behavior can differ slightly (canvas layout, timeScale) — always verify in Play mode.

## Verify

- Enter Play mode: open/close the UI and confirm forward/reverse playback and end callbacks fire exactly once per play.
- Toggle `Time.timeScale = 0` and confirm per-track `ignoreTimeScale` behavior.
- If types like `DOTween` are missing: DOTween ASMDEF not created or GUID broken — fix setup before touching code.
