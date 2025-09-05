using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OxGKit.InfiniteScrollView
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class InfiniteScrollView : UIBehaviour
    {
        public enum DataOrder
        {
            Normal,
            Reverse
        }

        public enum SnapAlign
        {
            Start = 1,
            Center = 2,
            End = 3,
        }

        protected enum ScrollType
        {
            Vertical = 1,
            Horizontal = 2
        }

        [Serializable]
        public class Padding
        {
            public int top;
            public int bottom;
            public int left;
            public int right;
        }

        [Header("------ Cell Pool Options ------")]
        public bool initializePoolOnAwake = false;
        public int cellPoolSize = 20;
        public InfiniteCell cellPrefab;

        [Space()]

        [Header("------ Cell Data Options ------")]
        [Tooltip("If reverse is selected, these data indexes will be reversed.")]
        public DataOrder dataOrder = DataOrder.Normal;

        [Space()]

        [Header("------ Cell View Options ------")]
        public float extendVisibleRange;
        public ScrollRect scrollRect { get; protected set; }
        public SnapAlign snapAlign = SnapAlign.Start;
        public Padding padding;

        // Cell info
        protected List<InfiniteCellData> _dataList = new List<InfiniteCellData>();
        protected List<InfiniteCell> _cellList = new List<InfiniteCell>();
        protected Queue<InfiniteCell> _cellPool = new Queue<InfiniteCell>();

        // Visible info
        protected bool _disabledRefreshCells = false;
        public int visibleCount { get; protected set; } = 0;
        public int lastMaxVisibleCount { get; protected set; } = 0;
        public float lastVisibleRangeSize { get; protected set; } = 0f;
        public bool isVisibleRangeFilled { get; protected set; } = false;

        // Direction pivot 
        protected float _contentDirCoeff = 0;

        // Scroll status
        protected bool _isAtTop = false;
        protected bool _isAtBottom = false;
        protected bool _isAtLeft = false;
        protected bool _isAtRight = false;

        // Callbacks
        public Action<Vector2> onValueChanged;
        public Action onRectTransformDimensionsChanged;
        public Action onRefreshed;
        public Action<InfiniteCell> onCellSelected;

        // Task cancellation
        private CancellationTokenSource _cts;

        // Snapping
        private DateTime _lastSnappingDurationTime;

        public bool isInitialized
        {
            get;
            protected set;
        }

        #region UIBehaviour
        protected override async void Awake()
        {
            if (this.initializePoolOnAwake)
            {
                await this.InitializePool();
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.onRectTransformDimensionsChanged?.Invoke();
        }
        #endregion

        #region Data Info
        /// <summary>
        /// Get data count (Equals to cell count)
        /// </summary>
        /// <returns></returns>
        public int DataCount()
        {
            return this._dataList.Count;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Init infinite-cell of scrollView
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask InitializePool(object args = null)
        {
            if (this.isInitialized)
                return;

            if (this.cellPrefab == null)
            {
                Logging.PrintError<Logger>("[InfiniteScrollView] Initialization failed. Cell prefab is null!!!");
                return;
            }

            if (this.scrollRect == null) this.scrollRect = this.GetComponent<ScrollRect>();
            this.scrollRect.onValueChanged.RemoveAllListeners();
            this.scrollRect.onValueChanged.AddListener(OnValueChanged);

            // Clear children
            foreach (Transform trans in this.scrollRect.content)
            {
                Destroy(trans.gameObject);
            }

            this._dataList.Clear();
            this._cellList.Clear();
            this._cellPool.Clear();

            for (int i = 0; i < this.cellPoolSize; i++)
            {
                var newCell = Instantiate(this.cellPrefab, this.scrollRect.content);
                await newCell.OnCreate(args);
                newCell.gameObject.SetActive(false);
                this._cellPool.Enqueue(newCell);
            }
            this.isInitialized = true;
        }
        #endregion

        #region Refresh Visibility
        protected void OnValueChanged(Vector2 normalizedPosition)
        {
            // If ever set to false, must refresh all once
            if (this._disabledRefreshCells)
            {
                this._disabledRefreshCells = false;
                this.Refresh();
            }
            else this.RefreshVisibleCells();

            // Invoke callback
            this.onValueChanged?.Invoke(normalizedPosition);
        }

        /// <summary>
        /// Refresh visible cells
        /// </summary>
        public void RefreshVisibleCells()
        {
            if (!this.IsInitialized()) return;
            this.DoRefreshVisibleCells();
        }

        protected abstract void DoRefreshVisibleCells();

        /// <summary>
        /// Refresh scrollView
        /// </summary>
        /// <param name="disabledRefreshCells">Disable refresh cells, when disabled will mark flag to refresh all at next scrolling.</param>
        /// <returns></returns>
        public abstract void Refresh(bool disabledRefreshCells = false);

        protected abstract void DoRefresh(bool disabledRefreshCells);

        protected abstract UniTask DoDelayRefresh(bool disabledRefreshCells);

        protected bool IsInitialized()
        {
            if (!this.isInitialized)
            {
                Logging.PrintWarning<Logger>("[InfiniteScrollView] Please InitializePool first!!!");
                return false;
            }
            return true;
        }
        #endregion

        #region Cell Operation
        /// <summary>
        /// Add cell data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="autoRefresh"></param>
        /// <returns></returns>
        public virtual void Add(InfiniteCellData data, bool autoRefresh = false)
        {
            if (!this.IsInitialized()) return;

            this._dataList.Add(data);
            this._cellList.Add(null);
            this._RefreshCellDataIndex(this._dataList.Count - 1);
            if (autoRefresh) this.Refresh();
        }

        /// <summary>
        /// Insert cell data
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool Insert(int index, InfiniteCellData data)
        {
            if (!this.IsInitialized()) return false;

            // Insert including max count
            if (index > this._dataList.Count ||
                index < 0)
                return false;

            this._dataList.Insert(index, data);
            this._cellList.Insert(index, null);
            this._RefreshCellDataIndex(index);
            return true;
        }

        /// <summary>
        /// Remove cell data
        /// </summary>
        /// <param name="index"></param>
        /// <param name="autoRefresh"></param>
        /// <returns></returns>
        public virtual bool Remove(int index, bool autoRefresh = true)
        {
            if (!this.IsInitialized()) return false;

            if (index >= this._dataList.Count ||
                index < 0)
                return false;

            this.RecycleCell(index);
            this._cellList.RemoveAt(index);
            this._dataList[index].Dispose();
            this._dataList.RemoveAt(index);
            this._RefreshCellDataIndex(index);
            if (autoRefresh) this.Refresh();
            return true;
        }

        /// <summary>
        /// Clear cell data
        /// </summary>
        /// <returns></returns>
        public virtual void Clear()
        {
            if (!this.IsInitialized()) return;

            this.scrollRect.velocity = Vector2.zero;
            this.scrollRect.content.anchoredPosition = Vector2.zero;
            for (int i = 0; i < this._cellList.Count; i++)
            {
                this.RecycleCell(i);
            }
            this._cellList.Clear();
            for (int i = 0; i < this._dataList.Count; i++)
            {
                this._dataList[i].Dispose();
            }
            this._dataList.Clear();
            this.Refresh();
        }
        #endregion

        #region Scroll Operation
        /// <summary>
        /// Scroll to top
        /// </summary>
        public void ScrollToTop()
        {
            if (this.scrollRect == null) return;
            this.scrollRect.verticalNormalizedPosition = 1;
        }

        /// <summary>
        /// Scroll to bottom
        /// </summary>
        public void ScrollToBottom()
        {
            if (this.scrollRect == null) return;
            this.scrollRect.verticalNormalizedPosition = 0;
        }

        /// <summary>
        /// Scroll to left
        /// </summary>
        public void ScrollToLeft()
        {
            if (this.scrollRect == null) return;
            this.scrollRect.horizontalNormalizedPosition = 0;
        }

        /// <summary>
        /// Scroll to right
        /// </summary>
        public void ScrollToRight()
        {
            if (this.scrollRect == null) return;
            this.scrollRect.horizontalNormalizedPosition = 1;
        }

        public float VerticalNormalizedPosition()
        {
            if (this.scrollRect == null) return -1;
            return this.scrollRect.verticalNormalizedPosition;
        }

        public float HorizontalNormalizedPosition()
        {
            if (this.scrollRect == null) return -1;
            return this.scrollRect.horizontalNormalizedPosition;
        }

        /// <summary>
        /// Check view to top 
        /// </summary>
        /// <param name="topDistance"></param>
        /// <returns></returns>
        public bool IsAtTop()
        {
            if (this.scrollRect == null) return false;
            // Adjust direction (Vertical = 1, Vertical Reverse = -1)
            bool result = this._contentDirCoeff > 0 ? this._isAtTop : this._isAtBottom;
            return result;
        }

        /// <summary>
        /// Check view to bottom
        /// </summary>
        /// <param name="bottomDistance"></param>
        /// <returns></returns>
        public bool IsAtBottom()
        {
            if (this.scrollRect == null) return false;
            // Adjust direction (Vertical = 1, Vertical Reverse = -1)
            bool result = this._contentDirCoeff > 0 ? this._isAtBottom : this._isAtTop;
            return result;
        }

        /// <summary>
        /// Check view to left
        /// </summary>
        /// <returns></returns>
        public bool IsAtLeft()
        {
            if (this.scrollRect == null) return false;
            // Adjust direction (Horizontal = -1, Horizontal Reverse = 1)
            bool result = this._contentDirCoeff > 0 ? this._isAtRight : this._isAtLeft;
            return result;
        }

        /// <summary>
        /// Check view to right
        /// </summary>
        /// <returns></returns>
        public bool IsAtRight()
        {
            if (this.scrollRect == null) return false;
            // Adjust direction (Horizontal = -1, Horizontal Reverse = 1)
            bool result = this._contentDirCoeff > 0 ? this._isAtLeft : this._isAtRight;
            return result;
        }
        #endregion

        #region Snapping
        /// <summary>
        /// Move to specific cell by index
        /// <para> duration &lt;= 0 is sync </para>
        /// <para> duration &gt; 0 is async </para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="duration"></param>
        public abstract void Snap(int index, float duration);

        /// <summary>
        /// Move to first cell
        /// <para> duration &lt;= 0 is sync </para>
        /// <para> duration &gt; 0 is async </para>
        /// </summary>
        /// <param name="duration"></param>
        public void SnapFirst(float duration)
        {
            int index = 0;
            this.Snap(index, duration);
        }

        /// <summary>
        /// Move to middle cell
        /// <para> duration &lt;= 0 is sync </para>
        /// <para> duration &gt; 0 is async </para>
        /// </summary>
        /// <param name="duration"></param>
        public void SnapMiddle(float duration)
        {
            this.Snap((this._dataList.Count - 1) >> 1, duration);
        }

        /// <summary>
        /// Move to last cell
        /// <para> duration &lt;= 0 is sync </para>
        /// <para> duration &gt; 0 is async </para>
        /// </summary>
        /// <param name="duration"></param>
        public void SnapLast(float duration)
        {
            int index = this._dataList.Count - 1;
            this.Snap(index, duration);
        }

        protected void DoSnapping(int index, Vector2 target, float duration)
        {
            this._StopSnapping();
            this._cts = new CancellationTokenSource();
            this._ProcessSnapping(index, target, duration).Forget();
        }

        private void _StopSnapping()
        {
            if (this._cts != null)
            {
                this._cts.Cancel();
                this._cts.Dispose();
                this._cts = null;
            }
        }

        private async UniTask _ProcessSnapping(int index, Vector2 target, float duration)
        {
            this.scrollRect.velocity = Vector2.zero;

            if (duration <= 0)
            {
                this.scrollRect.content.anchoredPosition = target;
                if (this._disabledRefreshCells)
                {
                    this._disabledRefreshCells = false;
                    this.Refresh();
                }
                else this.RefreshVisibleCells();
            }
            else
            {
                Vector2 startPos = this.scrollRect.content.anchoredPosition;
                this._lastSnappingDurationTime = DateTime.Now;
                float t = 0f;
                while (t < 1f)
                {
                    if (this.scrollRect == null ||
                        this.scrollRect.IsDestroyed())
                        break;
                    double currentElapsedTime = DateTime.Now.Subtract(this._lastSnappingDurationTime).TotalSeconds;
                    t = (float)currentElapsedTime / duration;
                    this.scrollRect.content.anchoredPosition = Vector2.Lerp(startPos, target, t);
                    var normalizedPos = this.scrollRect.normalizedPosition;
                    if (normalizedPos.y < 0 || normalizedPos.x > 1) break;
                    await UniTask.Yield(PlayerLoopTiming.Update, this._cts.Token);
                }

                /**
                 * When scrolling, OnValueChanged will be called
                 */
            }

            if (this._dataList.Count > 0)
            {
                if (index >= this._dataList.Count) index = this._dataList.Count - 1;
                if (index < 0) index = 0;
                if (this._dataList.Count == this._cellList.Count)
                {
                    var cell = this._cellList[index];
                    if (cell != null) cell.OnSnap();
                }
            }

            // After snap end to release cts
            this._cts.Cancel();
            this._cts.Dispose();
            this._cts = null;
        }

        protected float CalculateSnapPos(ScrollType scrollType, SnapAlign snapPosType, float originValue, InfiniteCellData cellData)
        {
            float newValue = 0;
            float viewPortRectSizeValue = 0;
            float contentRectSizeValue = 0;
            float cellSizeValue = 0;

            switch (scrollType)
            {
                case ScrollType.Horizontal:
                    viewPortRectSizeValue = this.scrollRect.viewport.rect.width;
                    contentRectSizeValue = this.scrollRect.content.rect.width;
                    cellSizeValue = cellData.cellSize.x;
                    break;

                case ScrollType.Vertical:
                    viewPortRectSizeValue = this.scrollRect.viewport.rect.height;
                    contentRectSizeValue = this.scrollRect.content.rect.height;
                    cellSizeValue = cellData.cellSize.y;
                    break;
            }

            // Cannot scroll, if Content size < Viewport size return 0 directly
            if (contentRectSizeValue < viewPortRectSizeValue) return 0;

            switch (snapPosType)
            {
                case SnapAlign.Start:
                    newValue = Mathf.Clamp(originValue, 0, contentRectSizeValue - viewPortRectSizeValue);
                    break;
                case SnapAlign.Center:
                    newValue = Mathf.Clamp(originValue - (viewPortRectSizeValue / 2 - cellSizeValue / 2), 0, contentRectSizeValue - viewPortRectSizeValue);
                    break;
                case SnapAlign.End:
                    newValue = Mathf.Clamp(originValue - (viewPortRectSizeValue - cellSizeValue), 0, contentRectSizeValue - viewPortRectSizeValue);
                    break;
            }

            return newValue;
        }
        #endregion

        #region Pool Operation
        protected void SetupCell(InfiniteCell cell, int index, Vector2 pos)
        {
            if (cell != null)
            {
                this._cellList[index] = cell;
                cell.cellData = this._dataList[index];
                cell.rectTransform.anchoredPosition = pos;
                cell.onSelected += this._OnCellSelected;
                cell.gameObject.SetActive(true);
            }
        }

        protected void RecycleCell(int index)
        {
            if (this._cellList[index] != null)
            {
                var cell = this._cellList[index];
                this._cellList[index] = null;
                cell.onSelected -= this._OnCellSelected;
                cell.gameObject.SetActive(false);
                cell.OnRecycle();
                this._cellPool.Enqueue(cell);
            }
        }

        private void _OnCellSelected(InfiniteCell selectedCell)
        {
            this.onCellSelected?.Invoke(selectedCell);
        }

        private void _RefreshCellDataIndex(int beginIndex)
        {
            // Optimized refresh efficiency
            for (int i = beginIndex; i < this._dataList.Count; i++)
            {
                this._dataList[i].index = i;
            }
        }
        #endregion
    }
}