using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using UnityEngine;

namespace OxGKit.InfiniteScrollView
{
    public class HorizontalInfiniteScrollView : InfiniteScrollView
    {
        public float spacing;

        #region Override
        protected override void DoRefreshVisibleCells()
        {
            // Reset visible count
            this.visibleCount = 0;

            // Viewport
            float viewportInterval = this.scrollRect.viewport.rect.width;

            // Check content direction pivot
            if (this._contentDirCoeff == 0) this._contentDirCoeff = this.scrollRect.content.pivot.x > 0 ? 1f : -1f;

            // Set content direction
            float minViewport = this.scrollRect.content.anchoredPosition.x * this._contentDirCoeff;
            Vector2 viewportRange = new Vector2(minViewport - this.extendVisibleRange, minViewport + viewportInterval + this.extendVisibleRange);

            // Hide
            float contentWidth = this.padding.left;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < this._dataList.Count; i++)
                    {
                        var visibleRange = new Vector2(contentWidth, contentWidth + this._dataList[i].cellSize.x);
                        if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                        {
                            this.RecycleCell(i);
                        }
                        contentWidth += this._dataList[i].cellSize.x + this.spacing;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = this._dataList.Count - 1; i >= 0; i--)
                    {
                        var visibleRange = new Vector2(contentWidth, contentWidth + this._dataList[i].cellSize.x);
                        if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                        {
                            this.RecycleCell(i);
                        }
                        contentWidth += this._dataList[i].cellSize.x + this.spacing;
                    }
                    break;
            }

            // Show
            contentWidth = this.padding.left;
            float lastVisibleWidth = 0f;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < this._dataList.Count; i++)
                    {
                        var visibleRange = new Vector2(contentWidth, contentWidth + this._dataList[i].cellSize.x);
                        if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                        {
                            // Calculate visible count
                            this.visibleCount++;
                            lastVisibleWidth = visibleRange.y;

                            InfiniteCell cell = null;
                            if (this._cellList[i] == null)
                            {
                                if (this._cellPool.Count > 0) cell = this._cellPool.Dequeue();
                                else Logging.Print<Logger>("<color=#ff4242>The cell display error occurred, not enough cells in the cell pool!!!</color>");
                            }
                            // Check cell direction pivot
                            float dirCoeff = 1f;
                            if (cell != null) dirCoeff = cell.rectTransform.pivot.x > 0 ? -1f : 1f;
                            this.SetupCell(cell, i, new Vector2(contentWidth * dirCoeff, -(this.padding.top - this.padding.bottom)));
                            if (visibleRange.y >= viewportRange.x)
                                this._cellList[i]?.transform.SetAsLastSibling();
                            else
                                this._cellList[i]?.transform.SetAsFirstSibling();
                        }
                        contentWidth += this._dataList[i].cellSize.x + this.spacing;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = this._dataList.Count - 1; i >= 0; i--)
                    {
                        var visibleRange = new Vector2(contentWidth, contentWidth + this._dataList[i].cellSize.x);
                        if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                        {
                            // Calculate visible count
                            this.visibleCount++;
                            lastVisibleWidth = visibleRange.y;

                            InfiniteCell cell = null;
                            if (this._cellList[i] == null)
                            {
                                if (this._cellPool.Count > 0) cell = this._cellPool.Dequeue();
                                else Logging.Print<Logger>("<color=#ff4242>The cell display error occurred, not enough cells in the cell pool!!!</color>");
                            }
                            // Check cell direction pivot
                            float dirCoeff = 1f;
                            if (cell != null) dirCoeff = cell.rectTransform.pivot.x > 0 ? -1f : 1f;
                            this.SetupCell(cell, i, new Vector2(contentWidth * dirCoeff, -(this.padding.top - this.padding.bottom)));
                            if (visibleRange.y >= viewportRange.x)
                                this._cellList[i]?.transform.SetAsLastSibling();
                            else
                                this._cellList[i]?.transform.SetAsFirstSibling();
                        }
                        contentWidth += this._dataList[i].cellSize.x + this.spacing;
                    }
                    break;
            }

            // Calculate fill status
            float visibleRangeWidth = viewportRange.y;
            float visibleRangeSize = viewportRange.y - viewportRange.x;
            this.isVisibleRangeFilled = lastVisibleWidth >= visibleRangeWidth;
            if (this.visibleCount > this.lastMaxVisibleCount ||
                visibleRangeSize != this.lastVisibleRangeSize)
            {
                this.lastMaxVisibleCount = this.visibleCount;
                this.lastVisibleRangeSize = visibleRangeSize;
            }

            // Check scroll position
            if (this.scrollRect.content.sizeDelta.x > viewportInterval)
            {
                this._isAtLeft = viewportRange.x + this.extendVisibleRange + this._dataList[0].cellSize.x <= this._dataList[0].cellSize.x;
                this._isAtRight = this.scrollRect.content.sizeDelta.x - viewportRange.y + this.extendVisibleRange + this._dataList[this._dataList.Count - 1].cellSize.x <= this._dataList[this._dataList.Count - 1].cellSize.x;
            }
            else
            {
                this._isAtTop = false;
                this._isAtBottom = false;
                this._isAtLeft = true;
                this._isAtRight = true;
            }
        }

        public override void Snap(int index, float duration)
        {
            if (!this.IsInitialized())
                return;
            if (index >= this._dataList.Count ||
                index < 0)
                return;
            if (this.scrollRect.content.rect.width < this.scrollRect.viewport.rect.width)
                return;

            // Adjust snap index
            switch (this.dataOrder)
            {
                case DataOrder.Reverse:
                    index = this._dataList.Count - 1 - index;
                    break;
            }

            float width = this.padding.left;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < index; i++)
                    {
                        // Normal index
                        int tempIndex = i;
                        width += this._dataList[tempIndex].cellSize.x + this.spacing;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = 0; i < index; i++)
                    {
                        // Reverse index
                        int tempIndex = this._dataList.Count - 1 - i;
                        width += this._dataList[tempIndex].cellSize.x + this.spacing;
                    }
                    break;
            }

            var cellData = this._dataList[index];
            width = this.CalculateSnapPos(ScrollType.Horizontal, this.snapAlign, width, cellData);
            // Check content direction pivot
            this.DoSnapping(index, new Vector2(width * this._contentDirCoeff, 0), duration);
        }

        public override bool Remove(int index, bool withRefresh = true)
        {
            if (!this.IsInitialized())
                return false;
            if (index >= this._dataList.Count ||
                index < 0)
                return false;

            var removeCell = this._dataList[index];
            bool result = base.Remove(index, withRefresh);
            this.scrollRect.content.anchoredPosition -= new Vector2(removeCell.cellSize.x + this.spacing, 0);
            return result;
        }
        #endregion

        #region Sealed Override
        public sealed override void Refresh(bool disabledRefreshCells = false)
        {
            if (!IsInitialized()) return;

            if (this.scrollRect.viewport.rect.width == 0)
            {
                this.DoDelayRefresh(disabledRefreshCells).Forget();
            }
            else
            {
                this.DoRefresh(disabledRefreshCells);
            }
        }

        protected sealed override void DoRefresh(bool disabledRefreshCells)
        {
            if (this.scrollRect == null) return;

            if (!disabledRefreshCells)
            {
                // Refresh content size
                float width = this.padding.left;
                for (int i = 0; i < this._dataList.Count; i++)
                {
                    width += this._dataList[i].cellSize.x + this.spacing;
                }
                width += this.padding.right;
                this.scrollRect.content.sizeDelta = new Vector2(width, this.scrollRect.content.sizeDelta.y);

                // Recycle all cells first
                for (int i = 0; i < this._cellList.Count; i++)
                {
                    this.RecycleCell(i);
                }

                // Refresh cells view
                this.DoRefreshVisibleCells();

                // Invoke onRefresh callback
                this.onRefreshed?.Invoke();
            }
            // Mark flag for refresh at next scrolling
            else this._disabledRefreshCells = true;
        }

        protected sealed override async UniTask DoDelayRefresh(bool disabledRefreshCells)
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            this.DoRefresh(disabledRefreshCells);
        }
        #endregion
    }
}