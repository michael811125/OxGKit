using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using UnityEngine;

namespace OxGKit.InfiniteScrollView
{
    public class VerticalInfiniteScrollView : InfiniteScrollView
    {
        public float spacing;

        #region Override
        protected override void DoRefreshVisibleCells()
        {
            // Reset visible count
            this.visibleCount = 0;

            // Viewport
            float viewportInterval = this.scrollRect.viewport.rect.height;

            // Check content direction pivot
            if (this._contentDirCoeff == 0) this._contentDirCoeff = this.scrollRect.content.pivot.y > 0 ? 1f : -1f;

            // Set content direction
            float minViewport = this.scrollRect.content.anchoredPosition.y * this._contentDirCoeff;
            Vector2 viewportRange = new Vector2(minViewport - this.extendVisibleRange, minViewport + viewportInterval + this.extendVisibleRange);

            // Hide
            float contentHeight = this.padding.top;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < this._dataList.Count; i++)
                    {
                        var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[i].cellSize.y);
                        if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                        {
                            this.RecycleCell(i);
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = this._dataList.Count - 1; i >= 0; i--)
                    {
                        var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[i].cellSize.y);
                        if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                        {
                            this.RecycleCell(i);
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing;
                    }
                    break;
            }

            // Show
            contentHeight = this.padding.top;
            float lastVisibleHeight = 0f;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < this._dataList.Count; i++)
                    {
                        var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[i].cellSize.y);
                        if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                        {
                            // Calculate visible count
                            this.visibleCount++;
                            lastVisibleHeight = visibleRange.y;

                            InfiniteCell cell = null;
                            if (this._cellList[i] == null)
                            {
                                if (this._cellPool.Count > 0) cell = this._cellPool.Dequeue();
                                else Logging.Print<Logger>("<color=#ff4242>The cell display error occurred, not enough cells in the cell pool!!!</color>");
                            }
                            // Check cell direction pivot
                            float dirCoeff = 1f;
                            if (cell != null) dirCoeff = cell.rectTransform.pivot.y > 0 ? -1f : 1f;
                            this.SetupCell(cell, i, new Vector2(this.padding.left - this.padding.right, contentHeight * dirCoeff));
                            if (visibleRange.y >= viewportRange.x)
                                this._cellList[i]?.transform.SetAsLastSibling();
                            else
                                this._cellList[i]?.transform.SetAsFirstSibling();
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = this._dataList.Count - 1; i >= 0; i--)
                    {
                        var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[i].cellSize.y);
                        if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                        {
                            // Calculate visible count
                            this.visibleCount++;
                            lastVisibleHeight = visibleRange.y;

                            InfiniteCell cell = null;
                            if (this._cellList[i] == null)
                            {
                                if (this._cellPool.Count > 0) cell = this._cellPool.Dequeue();
                                else Logging.Print<Logger>("<color=#ff4242>The cell display error occurred, not enough cells in the cell pool!!!</color>");
                            }
                            // Check cell direction pivot
                            float dirCoeff = 1f;
                            if (cell != null) dirCoeff = cell.rectTransform.pivot.y > 0 ? -1f : 1f;
                            this.SetupCell(cell, i, new Vector2(this.padding.left - this.padding.right, contentHeight * dirCoeff));
                            if (visibleRange.y >= viewportRange.x)
                                this._cellList[i]?.transform.SetAsLastSibling();
                            else
                                this._cellList[i]?.transform.SetAsFirstSibling();
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing;
                    }
                    break;
            }

            // Calculate fill status
            float visibleRangeHeight = viewportRange.y;
            float visibleRangeSize = viewportRange.y - viewportRange.x;
            this.isVisibleRangeFilled = lastVisibleHeight >= visibleRangeHeight;
            if (this.visibleCount > this.lastMaxVisibleCount ||
                visibleRangeSize != this.lastVisibleRangeSize)
            {
                this.lastMaxVisibleCount = this.visibleCount;
                this.lastVisibleRangeSize = visibleRangeSize;
            }

            // Check scroll position
            if (this.scrollRect.content.sizeDelta.y > viewportInterval)
            {
                this._isAtTop = viewportRange.x + this.extendVisibleRange <= 0.001f;
                this._isAtBottom = this.scrollRect.content.sizeDelta.y - viewportRange.y + this.extendVisibleRange <= 0.001f;
            }
            else
            {
                this._isAtTop = true;
                this._isAtBottom = true;
                this._isAtLeft = false;
                this._isAtRight = false;
            }
        }

        public override void Snap(int index, float duration)
        {
            if (!this.IsInitialized())
                return;
            if (index >= this._dataList.Count ||
                index < 0)
                return;
            if (this.scrollRect.content.rect.height < this.scrollRect.viewport.rect.height)
                return;

            // Adjust snap index
            switch (this.dataOrder)
            {
                case DataOrder.Reverse:
                    index = this._dataList.Count - 1 - index;
                    break;
            }

            float height = this.padding.top;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < index; i++)
                    {
                        // Normal index
                        int tempIndex = i;
                        height += this._dataList[tempIndex].cellSize.y + this.spacing;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = 0; i < index; i++)
                    {
                        // Reverse index
                        int tempIndex = this._dataList.Count - 1 - i;
                        height += this._dataList[tempIndex].cellSize.y + this.spacing;
                    }
                    break;
            }

            var cellData = this._dataList[index];
            height = this.CalculateSnapPos(ScrollType.Vertical, this.snapAlign, height, cellData);
            // Check content direction pivot
            this.DoSnapping(index, new Vector2(0, height * this._contentDirCoeff), duration);
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
            this.scrollRect.content.anchoredPosition -= new Vector2(0, removeCell.cellSize.y + this.spacing);
            return result;
        }
        #endregion

        #region Sealed Override
        public sealed override void Refresh(bool disabledRefreshCells = false)
        {
            if (!this.IsInitialized()) return;

            if (this.scrollRect.viewport.rect.height == 0)
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
                float height = this.padding.top;
                for (int i = 0; i < this._dataList.Count; i++)
                {
                    height += this._dataList[i].cellSize.y + this.spacing;
                }
                height += this.padding.bottom;
                this.scrollRect.content.sizeDelta = new Vector2(this.scrollRect.content.sizeDelta.x, height);

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