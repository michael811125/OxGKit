using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace OxGKit.InfiniteScrollView
{
    public class InfiniteCell : MonoBehaviour
    {
        internal event Action<InfiniteCell> onSelected;

        private RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    this._rectTransform = transform as RectTransform;
                return this._rectTransform;
            }
        }

        private InfiniteCellData _cellData;
        public InfiniteCellData cellData
        {
            set
            {
                this._cellData = value;
                this.OnRefresh();
            }
            get
            {
                return this._cellData;
            }
        }

        public virtual async UniTask OnCreate(object args) { }

        public virtual void OnRefresh() { }

        public virtual void OnRecycle() { }

        public virtual void OnSnap() { }

        /// <summary>
        /// Button event
        /// </summary>
        public virtual void OnClick()
        {
            this.InvokeSelected();
        }

        protected void InvokeSelected()
        {
            this.onSelected?.Invoke(this);
        }
    }
}

