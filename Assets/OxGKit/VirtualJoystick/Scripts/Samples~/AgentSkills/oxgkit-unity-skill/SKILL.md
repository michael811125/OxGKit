---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity UGUI projects that use OxGKit.VirtualJoystick, including the VirtualJoystick on-screen stick component, onStickInput subscription, StickVectorMode Normalized/Delta, StickType Fixed/Floating, AxisConstraint, handleMovementRange, deadZone, show-only-when-pressed behavior, and the VirtualJoystickUI prefab.
---

# OxGKit.VirtualJoystick Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.VirtualJoystick user. The module (UPM package `com.michaelo.oxgkit.virtualjoystick`, dependency-free, adjusted from AnnulusGames' EnhancedOnScreenStick) is a UGUI on-screen joystick: a background + draggable handle that emits a `Vector2` through `onStickInput`, with fixed/floating placement, axis constraints, and dead-zone filtering.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.virtualjoystick@*` or `Assets/OxGKit/VirtualJoystick/Scripts` when embedded). Do not invent APIs.
4. Confirm the Canvas setup (render mode, scaler) and EventSystem exist before debugging "joystick not responding".

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/VirtualJoystick/Scripts`
- No dependencies (works with both input backends; it is UGUI-event driven).
- Samples (Package Manager > OxGKit.VirtualJoystick > Samples): `VirtualJoystickUI Prefab` (pre-wired stick with images), `VirtualJoystick Demo`, `AI Agent Skills` (this skill).

## Component setup

`VirtualJoystick` (`Add Component > OxGKit > VirtualJoystick > VirtualJoystick`) sits on a full/partial-screen touch area (`RectTransform` + `Image` required — the image is the touch surface; make it transparent). Serialized refs: `_background` (stick base) and `_handle` (draggable knob) RectTransforms — already wired in the `VirtualJoystickUI` prefab. A parent `Canvas` is required (error + self-disable otherwise).

## Core API (verified)

```csharp
using OxGKit.VirtualJoystick;
using UnityEngine;

VirtualJoystick joystick;

// The output event — Vector2 per input change (zero on release)
joystick.onStickInput = v => this._move = v;   // public Action<Vector2>; use += for multiple listeners

// Configuration (also inspector-editable)
joystick.stickVectorMode;    // StickVectorMode.Normalized (-1..1) | Delta (mouse-delta-like, can exceed 1)
joystick.stickType;          // StickType.Fixed | Floating (re-centers where the press lands)
joystick.axisConstraint;     // AxisConstraint.Both | Horizontal | Vertical
joystick.handleMovementRange;// handle travel radius in pixels (default 100)
joystick.deadZone;           // 0..1, inputs below are ignored (accidental-touch filter)
// _showOnlyWhenPressed (inspector): hide the stick visuals until touched (floating-style UX)
```

## Usage pattern

```csharp
public class PlayerMobileControls : MonoBehaviour
{
    [SerializeField] private VirtualJoystick _joystick;
    private Vector2 _move;

    private void Awake()
    {
        this._joystick.stickType = StickType.Floating;
        this._joystick.axisConstraint = AxisConstraint.Both;
        this._joystick.deadZone = 0.1f;
        this._joystick.onStickInput += this._OnStick;
    }

    private void _OnStick(Vector2 v) => this._move = v;

    private void Update()
    {
        this.transform.Translate(new Vector3(this._move.x, 0f, this._move.y) * this._speed * Time.deltaTime);
    }

    private void OnDestroy() => this._joystick.onStickInput -= this._OnStick;
}
```

Bridging into OxGKit.InputSystem (keep gameplay input-source-agnostic): forward `onStickInput` into your `IInputAction` dispatcher event so gameplay subscribes to one movement signal for both keyboard and touch.

## Rules & pitfalls

- `onStickInput` is a plain `Action<Vector2>` field — assignment (`=`) replaces all listeners; prefer `+=`/`-=` and unsubscribe on destroy.
- `Normalized` mode outputs magnitude ≤ 1 (use for movement); `Delta` mimics mouse delta and can exceed 1 (use for camera-look), scale it by sensitivity, and expect zero on release.
- `Floating` type re-centers the stick at the press point within the touch area; pair with `_showOnlyWhenPressed` for modern mobile UX.
- `deadZone` is a fraction of the movement range — values around 0.05–0.15 filter accidental touches without hurting responsiveness.
- The component reads UGUI pointer events: it needs an `EventSystem` in the scene, a raycastable (non-disabled) image, and must not sit under a blocked/`CanvasGroup.blocksRaycasts = false` hierarchy.
- Multi-touch: one joystick tracks one pointer area; place move/look joysticks in separate non-overlapping touch areas.
- Canvas scaling affects pixel-based `handleMovementRange` — test at target resolutions/aspect ratios.

## Verify

- Play on device/simulator: drag from inside the touch area — handle follows within range, values match the vector mode, release snaps to zero; `Floating` re-centers per press; axis constraint locks the unwanted axis.
