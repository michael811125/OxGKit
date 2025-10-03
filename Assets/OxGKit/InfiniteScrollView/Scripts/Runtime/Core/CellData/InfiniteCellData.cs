using System;
using UnityEngine;

namespace OxGKit.InfiniteScrollView
{
    public class InfiniteCellData : IDisposable
    {
        public int index { get; internal set; }

        /// <summary>
        /// Cell width and height (x, y)
        /// </summary>
        public Vector2 cellSize;

        /// <summary>
        /// Data required to render this cell
        /// </summary>
        public object data;

        public InfiniteCellData() { }

        public InfiniteCellData(Vector2 cellSize)
        {
            this.cellSize = cellSize;
        }

        public InfiniteCellData(Vector2 cellSize, object data)
        {
            this.cellSize = cellSize;
            this.data = data;
        }

        public void Dispose()
        {
            this.data = null;
        }
    }
}