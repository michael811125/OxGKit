---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.InputSystem, including the Inputs.CM control-map registry (Unity New Input System IInputActionCollection), the Inputs.IA input-action registry (IInputAction signal dispatchers), Inputs.IA.DriveUpdate loops, custom Binding Composites, and decoupling gameplay/UI from device input via event subscription.
---

# OxGKit.InputSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.InputSystem user. The module (UPM package `com.michaelo.oxgkit.inputsystem`) is a thin input-dispatch layer: **Control Maps** wrap Unity New Input System generated classes, and **Input Actions** (`IInputAction`) re-dispatch input as C# events, so gameplay/UI code subscribes to game-level signals and never touches devices or platforms directly. Input Actions can also wrap any other input plugin — they are plain signal dispatchers.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.inputsystem@*` or `Assets/OxGKit/InputSystem/Scripts` when embedded) and inspect the project's generated control-map classes before writing code. Do not invent APIs or asset names.
4. Check the project actually has Unity New Input System enabled (Player Settings > Active Input Handling) when Control Maps are involved.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/InputSystem/Scripts`
- Auto dependencies: `com.unity.inputsystem`, `com.michaelo.oxgkit.loggingsystem`.
- Samples (Package Manager > OxGKit.InputSystem > Samples): `InputSystem Demo`, `AI Agent Skills` (this skill).
- Script templates: right-click `Assets/Create/OxGKit/Input System/Template Input Action.cs (Input Interface For Any)` and `.../New Input System (Extension)/Template Input Binding Composite.cs (For Unity New Input System)`.

## Core API (verified)

```csharp
using OxGKit.InputSystem;

// Control Maps registry (for Unity New Input System generated classes)
Inputs.CM.RegisterControlMap<TIInputActionCollection>();   // new() + Enable()
Inputs.CM.GetControlMap<TIInputActionCollection>();        // null if not registered
Inputs.CM.SetActive<TIInputActionCollection>(bool active); // Enable()/Disable() the collection
Inputs.CM.IsActive<TIInputActionCollection>();

// Input Actions registry (game-level signal dispatchers)
Inputs.IA.RegisterInputAction<TInputAction>();             // new() + OnCreate()
Inputs.IA.GetInputAction<TInputAction>();
Inputs.IA.DriveUpdate(float dt);                           // pumps every IInputAction.OnUpdate(dt)

public interface IInputAction
{
    void OnCreate();             // called on register: bind to control maps / plugins here
    void OnUpdate(float dt);     // polled by Inputs.IA.DriveUpdate(dt)
    void RemoveAllListeners();   // clear subscribed events
}
```

## Usage pattern

Register order matters — Control Maps first, then Input Actions (their `OnCreate` usually fetches a control map):

```csharp
using OxGKit.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

// 1) An IInputAction that converts device input into game events
public class PlayerAction : IInputAction
{
    public event System.Action<Vector2> onMove;
    public event System.Action onAttack;

    public void OnCreate()
    {
        var controls = Inputs.CM.GetControlMap<PlayerControls>();
        if (controls == null) return;
        controls.Player.Move.performed += this._OnMove;
        controls.Player.Move.canceled  += this._OnMove;
        controls.Player.Attack.performed += _ => this.onAttack?.Invoke();
    }

    public void OnUpdate(float dt) { /* optional polling */ }

    public void RemoveAllListeners()
    {
        this.onMove = null;
        this.onAttack = null;
    }

    private void _OnMove(InputAction.CallbackContext ctx) => this.onMove?.Invoke(ctx.ReadValue<Vector2>());
}

// 2) Bootstrap (once)
Inputs.CM.RegisterControlMap<PlayerControls>();
Inputs.IA.RegisterInputAction<PlayerAction>();

// 3) One owner drives the update loop
private void Update() => Inputs.IA.DriveUpdate(Time.deltaTime);

// 4) Gameplay/UI subscribes and unsubscribes
Inputs.IA.GetInputAction<PlayerAction>().onMove += this.OnMove;
Inputs.IA.GetInputAction<PlayerAction>().onMove -= this.OnMove;
```

Enable/disable input during UI modes or cutscenes:

```csharp
Inputs.CM.SetActive<PlayerControls>(false);                    // whole collection
Inputs.CM.GetControlMap<PlayerControls>()?.Player.Disable();   // single action map
Inputs.CM.GetControlMap<PlayerControls>()?.Player.Enable();
```

## Binding Composites (Unity New Input System)

For composite bindings (e.g., a 2D vector from four keys with custom processing), create one from the template menu; it produces an `InputBindingComposite<Vector2>`-style class registered via `[RuntimeInitializeOnLoadMethod]`/`InputSystem.RegisterBindingComposite`. The demo's `MoveInput` composite plus `PlayerControls.inputactions` shows the full setup — read them (`Samples~/InputDemo`) before authoring composites.

## Rules & pitfalls

- Do not register an `IInputAction` before the Control Map it reads is registered — `GetControlMap` returns null and bindings are silently skipped.
- Exactly one runtime owner should call `Inputs.IA.DriveUpdate(dt)`; forgetting it disables every `OnUpdate`-based action, calling it from several places double-ticks them.
- Gameplay/feature code subscribes to `IInputAction` events only; do not scatter raw `InputAction` reads through the project.
- Unsubscribe events when a feature exits; call `RemoveAllListeners()` (or re-register) on scene/domain transitions to avoid stale delegates.
- Registries are static: registering the same type twice is a no-op (first registration wins) — restart flows should account for that.

## Verify

- Compile, enter Play mode, confirm subscribed events fire for keyboard/gamepad, and that `SetActive<T>(false)` mutes input.
- If an `OnUpdate`-based behavior is dead, first check `DriveUpdate` is being called each frame.
