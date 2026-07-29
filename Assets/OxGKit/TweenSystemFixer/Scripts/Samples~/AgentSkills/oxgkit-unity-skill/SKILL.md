---
name: oxgkit-unity-skill
description: Use when fixing OxGKit.TweenSystem assembly reference errors caused by the DOTween.Modules asmdef GUID, including the OxGKit.TweenSystemFixer editor tool, the fixed GUID fdf3e181e62e9d243a7fee5ce890ab71, and manual DOTween.Modules.asmdef.meta repair after importing or upgrading DOTween Pro.
---

# OxGKit.TweenSystemFixer Unity Skill

## Purpose

Make the agent able to diagnose and repair the one problem this module solves. `OxGKit.TweenSystem` references the `DOTween.Modules` assembly by GUID, but DOTween Pro generates `DOTween.Modules.asmdef` with a **random GUID** on each machine/setup — so the reference breaks (missing-assembly compile errors, `MenuItem`s not appearing). `OxGKit.TweenSystemFixer` (UPM package `com.michaelo.oxgkit.tweensystemfixer`, editor-only) reassigns the fixed GUID that `OxGKit.TweenSystem` expects.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Confirm the actual symptom first (missing `DOTween.Modules` reference in `OxGKit.TweenSystem`, GUID warnings, compile failure right after importing/upgrading DOTween) before prescribing the fix.
4. Warn about GUID-reassignment side effects: any other asmdef that referenced the old `DOTween.Modules` GUID must be updated to the fixed GUID too.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/TweenSystemFixer/Scripts`
- Editor-only; no dependencies. Only needed alongside `OxGKit.TweenSystem` + DOTween Pro.

## The fixed GUID

```
fdf3e181e62e9d243a7fee5ce890ab71
```

`OxGKit.TweenSystem`'s asmdef references `DOTween.Modules` by this GUID.

## Prerequisites

1. DOTween Pro imported.
2. DOTween assemblies created: `Tools > Demigiant > DOTween Utility Panel > Create ASMDEF` (this generates `Plugins/Demigiant/DOTween/Modules/DOTween.Modules.asmdef`).

## Fix option A — tool (this package)

1. In the Project window, select the folder containing the DOTween modules (or `Assets`).
2. Run `Assets > OxGKit TweenSystem GUID Fixer > Search And Reassign DOTween.Modules GUID`.
3. The tool finds `DOTween.Modules.asmdef` under `Assets/` and reassigns its GUID to the fixed value, then saves/refreshes the AssetDatabase.
4. If it logs `Cannot found DOTween.Modules.asmdef, repair failed`: the ASMDEF was never created (see Prerequisites) or lives outside `Assets/`.

## Fix option B — manual

1. Close Unity (or ensure no import in progress).
2. Open `Assets/Plugins/Demigiant/DOTween/Modules/DOTween.Modules.asmdef.meta` in a text editor.
3. Replace the `guid:` value with `fdf3e181e62e9d243a7fee5ce890ab71`.
4. If other asmdefs in the project referenced the old GUID, update those references to the fixed GUID as well.
5. Reopen Unity / reimport.

## Rules & pitfalls

- This must be redone whenever DOTween is re-imported/upgraded in a way that regenerates the asmdef with a new GUID.
- Reassigning a GUID rewrites which asset existing references resolve to — apply it only to `DOTween.Modules.asmdef`, never as a general "fix GUIDs" habit.
- The tool searches `Assets/` only; UPM-packaged DOTween locations are out of scope for it (use the manual fix).
- Source-control note: commit the changed `.meta` file, or teammates hit the same breakage.

## Verify

- After the fix, `OxGKit.TweenSystem` compiles, its `Add Component > OxGKit > TweenSystem > ...` entries appear, and Console shows no missing-GUID/assembly warnings.
