using System;
using UnityEngine;

namespace OxGKit.InfiniteScrollView
{
    public class InfiniteCellData : IDisposable
    {
        public int index { get; internal set; }
        public Vector2 cellSize;
        public object data;

        public InfiniteCellData()
        {

        }

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