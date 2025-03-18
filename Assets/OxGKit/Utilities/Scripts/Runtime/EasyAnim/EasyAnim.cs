using System;
using UnityEngine;

namespace OxGKit.Utilities.EasyAnim
{
    public abstract class EasyAnim : MonoBehaviour
    {
        protected Action _animEnd = null;

        public abstract void Play(string name, Action animEnd);

        public abstract bool HasAnim(string name);

        protected void SetAnimEnd(Action animEnd)
        {
            this._animEnd = animEnd;
        }

        /// <summary>
        /// Anim event name (Set anim event on clip called AnimEnd)
        /// </summary>
        protected virtual void AnimEnd()
        {
            this._animEnd?.Invoke();
            this._animEnd = null;

            Debug.Log($"<color=#b6ff75>[EasyAnim] Root: {this.transform.root.name} trigger AnimEnd event function</color>");
        }
    }
}