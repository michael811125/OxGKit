---
name: oxgkit-unity-skill
description: Use when developing or reviewing Unity UGUI projects that use OxGKit.InfiniteScrollView, including Vertical/Horizontal/Grid infinite scroll views, InfiniteCell subclasses and their OnCreate/OnRefresh/OnRecycle/OnSnap/OnClick lifecycle, InfiniteCellData with dynamic cell sizes, pool initialization, Add/Insert/Remove/Refresh data operations, snapping, and chat-list or tab-page patterns.
---

# OxGKit.InfiniteScrollView Unity Skill

## Purpose

Make the agent behave like an experienced OxGKit.InfiniteScrollView user. The module (UPM package `com.michaelo.oxgkit.infinitescrollview`, UniTask-based, forked/improved from howtungtung's InfiniteScrollView) renders large lists with few GameObjects on native UGUI `ScrollRect`: cells are pooled, data lives in `InfiniteCellData`, and concrete layouts are `VerticalInfiniteScrollView`, `HorizontalInfiniteScrollView`, `VerticalGridInfiniteScrollView`, `HorizontalGridInfiniteScrollView`.

## First response rules

1. Reply in the user's language; keep code and identifiers in English.
2. Start with the conclusion, then implementation details.
3. Verify APIs against the installed package (`Library/PackageCache/com.michaelo.oxgkit.infinitescrollview@*` or `Assets/OxGKit/InfiniteScrollView/Scripts` when embedded). Do not invent APIs.
4. Before coding, identify layout variant, whether cell sizes are uniform or dynamic, and the data order (`Normal`/`Reverse`) the UI needs.

## Install

- UPM git URL: `https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/InfiniteScrollView/Scripts`
- Auto dependencies: `com.cysharp.unitask`, `com.michaelo.oxgkit.loggingsystem`.
- Samples (Package Manager > OxGKit.InfiniteScrollView > Samples): `01_Vertical`, `02_Horizontal`, `03_Grid`, `04_TabPage`, `05_ChatRoom`, `AI Agent Skills` (this skill).
- Templates: right-click `Assets/Create/OxGKit/Infinite ScrollView/Template Infinite Cell.cs (Script)` and `Template Infinite Cell (RectTransform Prefab)`.

## Building a list

1. Create a UGUI Scroll View; put one of the scroll-view components on the `ScrollRect` object (`VerticalInfiniteScrollView`, etc.).
2. Author a cell prefab whose root is a `RectTransform` with your `InfiniteCell` subclass; assign it to `cellPrefab`.
3. Configure: `cellPoolSize`, `initializePoolOnAwake` (or call `InitializePool()` yourself), `dataOrder` (`Normal`/`Reverse`), `snapAlign` (`Start`/`Center`/`End`), `extendVisibleRange`, `padding`; grid variants add `spacing` (Vector2) plus `columnCount` (vertical grid; renamed from `columeCount` in v1.7.1, old serialized data auto-migrates) / `rowCount` (horizontal grid).
4. Fill data with `InfiniteCellData` and `Refresh()`.

## Core API (verified)

```csharp
using OxGKit.InfiniteScrollView;
using Cysharp.Threading.Tasks;
using UnityEngine;

InfiniteScrollView sv;

// Pool init — REQUIRED before any data op (or initializePoolOnAwake)
await sv.InitializePool(args: null);      // args forwarded to every cell's OnCreate
bool ready = sv.isInitialized;

// Data ops (refresh flags control immediate visual update)
sv.Add(new InfiniteCellData(new Vector2(0, 100), payload), refresh: false);
sv.Insert(index, cellData, refresh: true);
sv.Remove(index, refresh: true);
sv.Clear();
int count = sv.DataCount();

// Refreshing
sv.Refresh(refreshOnNextScroll: false, recycleActiveCells: false);
sv.RefreshVisibleCells();

// Scrolling / state
sv.ScrollToTop(); sv.ScrollToBottom(); sv.ScrollToLeft(); sv.ScrollToRight();
sv.IsAtTop(); sv.IsAtBottom(); sv.IsAtLeft(); sv.IsAtRight();
float v = sv.VerticalNormalizedPosition(); float h = sv.HorizontalNormalizedPosition();

// Snapping (duration-based tween to a cell)
sv.Snap(index, duration);
sv.SnapFirst(duration); sv.SnapMiddle(duration); sv.SnapLast(duration);

// Events
sv.onRefreshed;                      // Action
sv.onValueChanged;                   // Action<Vector2> (scroll pos)
sv.onCellSelected;                   // Action<InfiniteCell> (from cell OnClick)
sv.onRectTransformDimensionsChanged; // Action
```

Cell + data:

```csharp
public class ItemCell : InfiniteCell   // MonoBehaviour on the cell prefab
{
    public override async UniTask OnCreate(object args) { /* one-time setup; args from InitializePool */ }
    public override void OnRefresh() { var d = (ItemData)this.cellData.data; /* redraw from data */ }
    public override void OnRecycle() { /* cleared from view */ }
    public override void OnSnap()    { /* became the snapped/selected cell */ }
    public override void OnClick()   { base.OnClick(); /* fires onCellSelected */ }
}

var data = new InfiniteCellData(cellSize: new Vector2(0, 80), data: payload);
// cellSize axis that matters: y for vertical lists, x for horizontal; grids use both.
```

## Patterns from the official samples

- **Vertical/Horizontal (01/02)**: uniform cells, `Normal` vs `Reverse` order, add/remove with GUI buttons.
- **Grid (03)**: `VerticalGridInfiniteScrollView` fills `columnCount` columns per row; `HorizontalGridInfiniteScrollView` fills `rowCount` rows per column; set `spacing`.
- **TabPage (04)**: horizontal page cells + `Snap(index, duration)` for tab switching; `snapAlign` centers pages.
- **ChatRoom (05)**: dynamic cell heights — measure text (`Text.preferredHeight`) into `InfiniteCellData.cellSize`, then `Add` + `Refresh()` + `SnapLast(0.1f)` for auto-scroll-to-newest.

```csharp
// ChatRoom-style dynamic height append
this.heightInstrument.text = msg;
var cellData = new InfiniteCellData(new Vector2(0, this.heightInstrument.preferredHeight + basePad), new ChatCellData(user, msg));
this.chatScrollView.Add(cellData);
this.chatScrollView.Refresh();
this.chatScrollView.SnapLast(0.1f);
```

## Rules & pitfalls

- `InitializePool()` must complete before `Add`/`Refresh`/`Snap`; when `initializePoolOnAwake` is off, await it in your own init.
- Cells are **recycled**: `OnRefresh` must be idempotent and fully driven by `cellData` — never store per-item state in the cell (store it in the data object).
- After batch `Add`s, call `Refresh()` once (cheaper than per-add refresh flags).
- `Remove`/`Insert` shift indices — re-read `cellData.index` inside cells instead of caching indices.
- Dynamic sizes: compute `cellSize` **before** adding; changing a size later requires updating the data and a full `Refresh`.
- `extendVisibleRange` pre-instantiates off-screen rows for smoothness at the cost of more live cells; tune with the pool size (`cellPoolSize` should cover visible + extended range).
- `onCellSelected` only fires if the cell's click path calls `base.OnClick()` (button on the cell wired to `OnClick`).
- Don't fight UGUI layout groups on the content node — the scroll view positions cells itself; content needs no LayoutGroup/ContentSizeFitter.

## Verify

- Play: scroll through thousands of items with a stable cell count in the Hierarchy; add/remove/snap behave; no layout warnings.
- For dynamic lists, resize the window (or trigger `onRectTransformDimensionsChanged`) and confirm re-layout.
