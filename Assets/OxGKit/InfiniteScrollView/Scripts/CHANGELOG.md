## CHANGELOG

## [1.6.0] - 2023-12-29
- Organized code.
- Organized plugin.

## [1.5.1] - 2023-11-08
- Added OnSnap method in InfiniteCell.
```C#
    public virtual void OnSnap()
```
- Modified OnClick can override in InfiniteCell.
```C#
    public virtual void OnClick()
```

## [1.5.0] - 2023-11-07
- Added DataOrder.
```C#
    public enum DataOrder
    {
        Normal,
        Reverse
    }
```
- Added lastVisibleRangeSize param in InfiniteScrollView.
```C#
    public float lastVisibleRangeSize { get; protected set; }
```
- Renamed RefreshCellVisibilityWithCheck to RefreshVisibleCells in InfiniteScrollView.
```C#
    public void RefreshVisibleCells()
```
- Renamed RefreshCellVisibility to DoRefreshVisibleCells in InfiniteScrollView.
```C#
    protected abstract void DoRefreshVisibleCells();
```
- Removed RefreshAndCheckVisibleInfo from InfiniteScrollView.
- Optimized code and refresh efficiency.

## [1.4.1] - 2023-11-03
- Fixed Refresh method default value bug issue (disabledRefreshCells = false).
- Added SnapFirst and SnapMiddle in InfiniteScrollView.
```C#
    public void SnapFirst(float duration)
    public void SnapMiddle(float duration)
```

## [1.4.0] - 2023-11-03 (Breaking Changes)
- Modified namespace HowTungTung to InfiniteScrollViews.
- Modified RefreshCellVisibility access modifier to protected.
```C#
    protected abstract void RefreshCellVisibility();
```
- Modified Add, Insert, Remove, Refresh, Clear methods (Removed async behaviour, won't auto InitializePool).
- Added RefreshCellVisibilityWithCheck method in InfiniteScrollView.
```C#
    public void RefreshCellVisibilityWithCheck()
```
- Added visibleCount param in InfiniteScrollView.
```C#
    public int visibleCount { get; protected set; }
```
- Added lastMaxVisibleCount param in InfiniteScrollView.
```C#
    public int lastMaxVisibleCount { get; protected set; }
```
- Added isVisibleRangeFilled param in InfiniteScrollView.
```C#
    public bool isVisibleRangeFilled { get; protected set; }
```
- Added DataCount() method in InfiniteScrollView.
```C#
    public int DataCount()
```
- Added param for Refresh(bool disabledRefreshCells = false) method of InfiniteScrollView.
```C#
    /// <summary>
    /// Refresh scrollView (doesn't need to await, if scrollView already initialized)
    /// </summary>
    /// <param name="disabledRefreshCells">Disable refresh cells, when disabled will mark flag to refresh all at next scrolling.</param>
    /// <returns></returns>
    public abstract UniTask Refresh(bool disabledRefreshCells = false)
```
- Optimized code.

※Note: If you add data and don't want to refresh cells every times. You can determines infiniteScrollView.isVisibleRangeFilled == true and set disabledRefreshCells = true, will help you to mark flag and refresh once all at next scrolling.
```C#
    public void AddCellData() 
    {
        var data = new InfiniteCellData(new Vector2(100, 100));
        infiniteScrollView.Add(data);
        if(!infiniteScrollView.isVisibleRangeFilled) infiniteScrollView.Refresh();
        else infiniteScrollView.Refresh(true);
    }
```

## [1.3.1] - 2023-11-01
- Fixed determines.
- Modified Samples.
- Optimzied Recycle and SetupCell procedure.

## [1.3.0] - 2023-10-31
- Modified All infiniteScrollViews can auto calculate direction by content and cell pivot.
- Modified Samples (Normal direction and Reverse direction).
- Added ScrollToLeft and ScrollToRight (Horizontal).
```C#
    public void ScrollToLeft()
    public void ScrollToRight()
```
- Added InfiniteScrollView IsAtLeft and IsAtRight.
```C#
    public bool IsAtLeft()
    public bool IsAtRight()
```
- Rename InfiniteScrollView IsScrollToTop method name to IsAtTop.
```C#
    public bool IsAtTop()
```
- Rename InfiniteScrollView IsScrollToBottom method name to IsAtBottom.
```C#
    public bool IsAtBottom()
```
- Removed ScrollToTarget method from InfiniteScrollView.
- Optimized code.

## [1.2.1] - 2023-10-24
- Modified InfiniteCell method name (OnUpdate change to OnRefresh more clear).
```C#
    public virtual void OnRefresh() { }
```
- Modified callback names in InfiniteScrollView.
  - onRectTransformUpdate change to onRectTransformDimensionsChanged.
  - onRefresh change onRefreshed.

## [1.2.0] - 2023-10-20
- Added [initializePoolOnAwake] trigger for InfiniteScrollView.
- Added OnClick in InfiniteCell for button event (Can assign event on button click).
- Modified InfiniteScrollView method name (Initialize change to InitializePool).
```C#
    public virtual async UniTask InitializePool(object args = null)
```
- Modified InfiniteCell method name (Initialize change to OnCreate).
```C#
    public virtual async UniTask OnCreate(object args) { }
```
- Modified InfiniteCellData index access modifier (Only internal can set).
- Optimizd index determines.

## [1.1.0] - 2023-10-17
- Added Cell script editor.

## [1.0.1] - 2023-10-17
- Fixed cellList count increase bug issue.
- Optimized InfiniteScrollView.

## [1.0.0] - 2023-10-16
- Added InfiniteScrollView with UniTask.