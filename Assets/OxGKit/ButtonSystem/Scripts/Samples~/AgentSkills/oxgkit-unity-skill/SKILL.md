---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity UGUI projects that use OxGKit.ButtonSystem, including the ButtonPlus extended Button (long-click Once/Continuous/PressedAndReleased modes, triggerTime/intervalTime, scale transition, onLongClickPressed/onLongClickReleased) and the InputFieldPlaceholderRemover helper.
---

# OxGKit.ButtonSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.ButtonSystem user. The module (UPM package `com.michaelo.oxgkit.buttonsystem`, dependency-free) extends UGUI: **`ButtonPlus`** inherits `UnityEngine.UI.Button` and adds long-click behaviors and a scale press-transition, and **`InputFieldPlaceholderRemover`** hides an InputField/TMP_InputField placeholder while it is focused.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.buttonsystem@*` or `Assets/OxGKit/ButtonSystem/Scripts` when embedded). Do not invent APIs.
4. Respect the target project's UI event-wiring conventions (helper extensions, bind-once lifecycles) before adding listeners.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/ButtonSystem/Scripts`
- No dependencies.
- Samples (Package Manager > OxGKit.ButtonSystem > Samples): `ButtonPlus Demo`, `AI Agent Skills` (this skill).
- Create menu: `GameObject/UI/Button Plus - TextMeshPro` and `GameObject/UI/Legacy/Button Plus`.

## Core API (verified)

`ButtonPlus : UnityEngine.UI.Button` — everything from `Button` (`onClick`, `interactable`, transitions, ...) plus:

```csharp
using OxGKit.ButtonSystem;

ButtonPlus btn;

// Extended press transition
btn.extdTransition;        // ExtdTransition.None | Scale
btn.transScale.size;       // pressed scale factor (default 0.95)

// Long click
btn.extdLongClick;         // ExtdLongClick.None | Once | Continuous | PressedAndReleased
btn.ignoreTimeScale;       // long-click timing unaffected by Time.timeScale (default true)
btn.triggerTime;           // seconds held before long click triggers (default 1)
btn.intervalTime;          // repeat interval for Continuous (default 0.1)
btn.onLongClickPressed;    // ButtonClickedEvent
btn.onLongClickReleased;   // ButtonClickedEvent
```

Long-click modes:

| Mode | Behavior |
| --- | --- |
| `Once` | after holding `triggerTime`, fires `onLongClickPressed` once |
| `Continuous` | after `triggerTime`, fires `onLongClickPressed` every `intervalTime` while held |
| `PressedAndReleased` | fires `onLongClickPressed` at `triggerTime`; fires `onLongClickReleased` on pointer up/exit |

Click interplay: when a long click has triggered, the subsequent pointer-up does **not** invoke `onClick` (the component temporarily swaps the event out) — a hold is either a long click or a click, never both. Pressed-state visuals (including the Scale transition) reset on click/exit.

## Usage patterns

```csharp
// Normal click (inherited)
btn.onClick.AddListener(this.OnConfirm);

// Hold-to-repeat (e.g. +1 per 0.1s while held)
btn.extdLongClick = ButtonPlus.ExtdLongClick.Continuous;
btn.triggerTime = 0.5f;
btn.intervalTime = 0.1f;
btn.onLongClickPressed.AddListener(this.OnIncrease);

// Hold-to-preview (press/release pair)
btn.extdLongClick = ButtonPlus.ExtdLongClick.PressedAndReleased;
btn.onLongClickPressed.AddListener(this.OnShowPreview);
btn.onLongClickReleased.AddListener(this.OnHidePreview);

// Cleanup on release/destroy
btn.onClick.RemoveAllListeners();
btn.onLongClickPressed.RemoveAllListeners();
btn.onLongClickReleased.RemoveAllListeners();
```

`InputFieldPlaceholderRemover`: add the component onto the same GameObject as a `UnityEngine.UI.InputField` or `TMP_InputField`; the placeholder hides on select/focus and restores on deselect. No API calls needed.

## Rules & pitfalls

- Register listeners once (bind phase), not in repeated show/refresh callbacks — `ButtonClickedEvent` accumulates duplicates.
- `extdTransition = Scale` multiplies the transform scale on press; it composes with (does not replace) the inherited UGUI `transition` — designers may use both. Do not overwrite designer-authored `Button.colors`/transition settings from runtime refresh code.
- `PressedAndReleased` fires the released event on pointer **exit** as well; handlers must tolerate release-without-move-back.
- `ignoreTimeScale` defaults to true — long clicks still work in pause menus; set it false only when hold timing should freeze with gameplay.
- In pooled/persistent UI, remove listeners on release, or route wiring through the project's existing button helper if one exists.
- ButtonPlus works with any UGUI setup (Legacy Text or TMP); the create-menu items just scaffold the matching label type.

## Verify

- Enter Play mode: short-press fires `onClick` only; hold past `triggerTime` fires the long-click events and suppresses `onClick`; visual scale returns to normal after release.
- With `Time.timeScale = 0`, confirm long-click behavior matches the `ignoreTimeScale` setting.
