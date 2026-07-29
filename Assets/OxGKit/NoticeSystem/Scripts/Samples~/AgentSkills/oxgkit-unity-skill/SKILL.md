---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity projects that use OxGKit.NoticeSystem (red-dot / notification badge system), including custom NoticeCondition classes, NoticeManager.RegisterCondition and Notify, the NoticeItem component and prefab, NoticeInfo registration with value/reference data, and data-driven red-dot visibility refresh.
---

# OxGKit.NoticeSystem Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.NoticeSystem user. The module (UPM package `com.michaelo.oxgkit.noticesystem`) is a red-dot/notification system: you define **conditions** (pure classes answering "should the badge show for this data?"), attach a **NoticeItem** component to each badge icon, register condition+data pairs on it, and call **Notify** when data changes — every NoticeItem holding a notified condition re-evaluates and toggles its own GameObject.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.noticesystem@*` or `Assets/OxGKit/NoticeSystem/Scripts` when embedded). Do not invent APIs.
4. Before adding conditions, check how the target project registers them (static register class vs `RuntimeInitializeOnLoadMethod` template) and follow that pattern.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/NoticeSystem/Scripts`
- Auto dependency: `com.michaelo.oxgkit.loggingsystem`.
- Samples (Package Manager > OxGKit.NoticeSystem > Samples): `NoticeItem Prefab` (badge prefab), `NoticeSystem Demo`, `AI Agent Skills` (this skill).
- Script templates: right-click `Assets/Create/OxGKit/Notice System/Template Notice Condition.cs (Manually)`, `... (RuntimeInitializeLoadType.BeforeSceneLoad)`, and `Template Notice Condition Registers.cs (Manually)`.

## Core API (verified)

```csharp
using OxGKit.NoticeSystem;

// Condition: pure logic class
public class NewMailCond : NoticeCondition
{
    // Convention: expose the auto-assigned id
    public static int id => NoticeManager.GetConditionId<NewMailCond>();

    public override bool ShowCondition(object data)
    {
        return data is MailBox box && box.unreadCount > 0;
    }
}

// Manager
NoticeManager.RegisterCondition<TNoticeCondition>(); // assigns the condition id
NoticeManager.GetConditionId<TNoticeCondition>();
NoticeManager.Notify(params int[] conditionIds);     // re-evaluate every NoticeItem holding these conditions
NoticeManager.Notify(params NoticeItem[] items);     // re-evaluate specific items

// NoticeItem (MonoBehaviour on the badge icon GameObject)
noticeItem.RegisterNotice(params NoticeInfo[] infos); // register + immediately evaluates visibility
noticeItem.RenewNotice(NoticeInfo info);              // update the data snapshot (auto-registers if new)
noticeItem.DeregisterNotice(params int[] conditionIds); // no args = deregister all of its conditions
noticeItem.HasCondition(int conditionId);
noticeItem.Notify();                                  // re-notify all conditions this item holds

// NoticeInfo: condition id + the data handed to ShowCondition
new NoticeInfo(NewMailCond.id, mailBox);
```

Visibility rule: a `NoticeItem` sets its **own GameObject active** when **any** of its registered conditions returns `true`, inactive when none do. On `OnDestroy` it auto-deregisters.

## Standard flow

```csharp
// 1) Register conditions once at startup (static register class pattern)
public static class NoticeConditionRegister
{
    static NoticeConditionRegister()
    {
        NoticeManager.RegisterCondition<NewMailCond>();
        NoticeManager.RegisterCondition<CanUpgradeCond>();
    }
    public static void Init() { } // manually trigger the static ctor
}

// 2) Attach the NoticeItem prefab (badge icon) under a button/tab, then register data on it
this.mailNoticeItem.RegisterNotice(new NoticeInfo(NewMailCond.id, this._mailBox));

// 3) When data changes, notify the condition — all items holding it refresh
this._mailBox.unreadCount++;
NoticeManager.Notify(NewMailCond.id);

// 4) On UI close (optional; OnDestroy also cleans up)
this.mailNoticeItem.DeregisterNotice();
```

Value-type data caveat: `NoticeInfo.data` stores a snapshot for value types (int, struct). Re-register the current value with `RenewNotice` before notifying:

```csharp
this.coinNoticeItem.RenewNotice(new NoticeInfo(CoinIsEvenCond.id, this._coin)).Notify();
```

Reference types (class instances) stay live — `Notify` alone is enough after mutating the object.

## Rules & pitfalls

- Register a condition class (`RegisterCondition<T>`) **before** using its id; `GetConditionId<T>` on an unregistered condition cannot resolve.
- `ShowCondition(object data)` must be defensive: `data` can be null or a different type when several UIs reuse a condition.
- One NoticeItem may hold several conditions (OR semantics). For AND logic, put the whole predicate inside one condition class.
- Value-type data is snapshotted — use `RenewNotice` (method-chainable) + `Notify`; forgetting this is the classic "red dot never updates" bug.
- `NoticeItem` toggles its own GameObject — make the badge a **child** icon object, not the button itself, or the button will disappear.
- Conditions are plain logic classes: no MonoBehaviour state, no scene references inside `ShowCondition`.
- Deregistration is automatic on destroy, but for pooled UI call `DeregisterNotice()` on recycle to avoid stale registrations.

## Verify

- Enter Play mode, mutate the driving data, call `Notify`, and confirm badges appear/disappear on every UI that holds the condition.
- Destroy/close the UI and notify again — no errors and no orphaned refreshes should occur.
