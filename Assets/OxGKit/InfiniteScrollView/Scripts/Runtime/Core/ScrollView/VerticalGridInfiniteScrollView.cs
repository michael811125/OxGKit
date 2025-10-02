using Cysharp.Threading.Tasks;
using OxGKit.LoggingSystem;
using UnityEngine;

namespace OxGKit.InfiniteScrollView
{
    public class VerticalGridInfiniteScrollView : InfiniteScrollView
    {
        public Vector2 spacing;
        public int columeCount = 1;

        #region Override
        protected override void DoRefreshVisibleCells()
        {
            if (this.columeCount <= 0)
            {
                this.columeCount = 1;
            }

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
                    for (int i = 0; i < this._dataList.Count; i += this.columeCount)
                    {
                        for (int j = 0; j < this.columeCount; j++)
                        {
                            int index = i + j;
                            if (index >= this._dataList.Count)
                                break;
                            var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[index].cellSize.y);
                            if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                            {
                                this.RecycleCell(index);
                            }
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing.y;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = this._dataList.Count - 1; i >= 0; i -= this.columeCount)
                    {
                        for (int j = 0; j < this.columeCount; j++)
                        {
                            int index = i - j;
                            if (index < 0 ||
                                index >= this._dataList.Count)
                                break;
                            var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[index].cellSize.y);
                            if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                            {
                                this.RecycleCell(index);
                            }
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing.y;
                    }
                    break;
            }

            // Show
            contentHeight = this.padding.top;
            float lastVisibleHeight = 0f;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < this._dataList.Count; i += this.columeCount)
                    {
                        for (int j = 0; j < this.columeCount; j++)
                        {
                            int index = i + j;
                            if (index >= this._dataList.Count)
                                break;
                            var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[index].cellSize.y);
                            if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                            {
                                // Calculate visible count
                                this.visibleCount++;
                                if (this.visibleCount % this.columeCount == 0) lastVisibleHeight = visibleRange.y;

                                InfiniteCell cell = null;
                                if (this._cellList[index] == null)
                                {
                                    if (this._cellPool.Count > 0) cell = this._cellPool.Dequeue();
                                    else Logging.PrintError<Logger>("The cell display error occurred, not enough cells in the cell pool!!!");
                                }
                                // Check cell direction pivot
                                float dirCoeff = 1f;
                                if (cell != null) dirCoeff = cell.rectTransform.pivot.y > 0 ? -1f : 1f;
                                this.SetupCell(cell, index, new Vector2((this._dataList[index].cellSize.x + this.spacing.x) * j + (this.padding.left - this.padding.right), contentHeight * dirCoeff));
                                if (visibleRange.y >= viewportRange.x)
                                    this._cellList[index]?.transform.SetAsLastSibling();
                                else
                                    this._cellList[index]?.transform.SetAsFirstSibling();
                            }
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing.y;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = this._dataList.Count - 1; i >= 0; i -= this.columeCount)
                    {
                        for (int j = 0; j < this.columeCount; j++)
                        {
                            int index = i - j;
                            if (index < 0 ||
                              index >= this._dataList.Count)
                                break;
                            var visibleRange = new Vector2(contentHeight, contentHeight + this._dataList[index].cellSize.y);
                            if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                            {
                                // Calculate visible count
                                this.visibleCount++;
                                if (this.visibleCount % this.columeCount == 0) lastVisibleHeight = visibleRange.y;

                                InfiniteCell cell = null;
                                if (this._cellList[index] == null)
                                {
                                    if (this._cellPool.Count > 0) cell = this._cellPool.Dequeue();
                                    else Logging.PrintError<Logger>("The cell display error occurred, not enough cells in the cell pool!!!");
                                }
                                // Check cell direction pivot
                                float dirCoeff = 1f;
                                if (cell != null) dirCoeff = cell.rectTransform.pivot.y > 0 ? -1f : 1f;
                                this.SetupCell(cell, index, new Vector2((this._dataList[index].cellSize.x + this.spacing.x) * j + (this.padding.left - this.padding.right), contentHeight * dirCoeff));
                                if (visibleRange.y >= viewportRange.x)
                                    this._cellList[index]?.transform.SetAsLastSibling();
                                else
                                    this._cellList[index]?.transform.SetAsFirstSibling();
                            }
                        }
                        contentHeight += this._dataList[i].cellSize.y + this.spacing.y;
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
                this._isAtTop = viewportRange.x + this.extendVisibleRange <= this._dataList[0].cellSize.y;
                this._isAtBottom = this.scrollRect.content.sizeDelta.y - viewportRange.y + this.extendVisibleRange <= this._dataList[this._dataList.Count - 1].cellSize.y;
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

            // Adjust snap index
            switch (this.dataOrder)
            {
                case DataOrder.Reverse:
                    index = this._dataList.Count - 1 - index;
                    break;
            }

            var rowNumber = index / this.columeCount;
            float height = this.padding.top;
            switch (this.dataOrder)
            {
                case DataOrder.Normal:
                    for (int i = 0; i < rowNumber; i++)
                    {
                        // Normal index
                        int tempIndex = i * this.columeCount;
                        height += this._dataList[tempIndex].cellSize.y + this.spacing.y;
                    }
                    break;
                case DataOrder.Reverse:
                    for (int i = 0; i < rowNumber; i++)
                    {
                        // Reverse index
                        int tempIndex = this._dataList.Count - 1 - (i * this.columeCount);
                        height += this._dataList[tempIndex].cellSize.y + this.spacing.y;
                    }
                    break;
            }

            var cellData = this._dataList[index];
            height = this.CalculateSnapPos(ScrollType.Vertical, this.snapAlign, height, cellData);
            // Check content direction pivot
            this.DoSnapping(index, new Vector2(0, height * this._contentDirCoeff), duration);
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
                for (int i = 0; i < this._dataList.Count; i += this.columeCount)
                {
                    height += this._dataList[i].cellSize.y + this.spacing.y;
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