using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace OxGKit.TweenSystem
{
    [AddComponentMenu("OxGKit/TweenSystem/" + nameof(DoTweenAnimEvent))]
    public class DoTweenAnimEvent : MonoBehaviour
    {
        public enum PlayMode
        {
            Parallel,
            Sequence
        }

        [SerializeField, Tooltip("Play Mode")]
        public PlayMode playMode = PlayMode.Parallel;
        [SerializeField, Tooltip("Set DoTweenAnims")]
        public List<DoTweenAnim> doTweenAnims = new List<DoTweenAnim>();

        private bool _lastTrigger = false;

        public void ResetTrigger()
        {
            this._lastTrigger = false;
        }

        public void SetPlayMode(PlayMode playMode)
        {
            this.playMode = playMode;
        }

        public DoTweenAnimEvent AddDoTweenAnim(params DoTweenAnim[] doTweenAnims)
        {
            if (doTweenAnims != null && doTweenAnims.Length > 0)
            {
                this.doTweenAnims.AddRange(doTweenAnims);
            }

            return this;
        }

        public void RemoveDoTweenAnim(DoTweenAnim doTweenAnim)
        {
            if (this.doTweenAnims.Count == 0) return;

            if (this.doTweenAnims.Contains(doTweenAnim))
            {
                this.doTweenAnims.Remove(doTweenAnim);
            }
        }

        public void ClearDoTweenAnims()
        {
            this.doTweenAnims.Clear();
        }

        public void PlayNormal()
        {
            this.PlayTweens(true, null);
        }

        public void PlayReverse()
        {
            this.PlayTweens(false, null);
        }

        public void PlayTrigger()
        {
            this._lastTrigger = !this._lastTrigger;
            this.PlayTrigger(this._lastTrigger);
        }

        public void PlayTrigger(bool trigger, TweenCallback endCallback = null)
        {
            this.PlayTweens(trigger, endCallback);
        }

        public void PlayTriggerOnce(bool trigger)
        {
            this.PlayTriggerOnce(trigger, null);
        }

        public void PlayTriggerOnce(bool trigger, TweenCallback endCallback = null)
        {
            // trigger = true
            if (!this._lastTrigger && trigger)
            {
                this._lastTrigger = trigger;
                this.PlayTweens(trigger, endCallback);
            }
            // trigger = false
            else if (this._lastTrigger && !trigger)
            {
                this._lastTrigger = trigger;
                this.PlayTweens(trigger, endCallback);
            }
        }

        protected void PlayTweens(bool trigger, TweenCallback endCallback)
        {
            if (this.doTweenAnims.Count == 0) return;

            switch (this.playMode)
            {
                case PlayMode.Parallel:
                    {
                        var seq = DOTween.Sequence();
                        for (int i = 0; i < this.doTweenAnims.Count; i++)
                        {
                            int idx = i;
                            seq.AppendCallback(() => this.doTweenAnims[idx]?.PlayTween(trigger));
                        }
                        if (endCallback != null) seq.AppendCallback(endCallback);
                        seq.AppendCallback(() => seq.Kill());
                    }
                    break;
                case PlayMode.Sequence:
                    {
                        var seq = DOTween.Sequence();
                        for (int i = 0; i < this.doTweenAnims.Count; i++)
                        {
                            int idx = i;
                            float duration = (this.doTweenAnims[i] == null) ? 0f : this.doTweenAnims[i].GetMaxDurationTween().duration;
                            seq.AppendCallback(() => this.doTweenAnims[idx]?.PlayTween(trigger));
                            seq.AppendInterval(duration);
                        }
                        if (endCallback != null) seq.AppendCallback(endCallback);
                        seq.AppendCallback(() => seq.Kill());
                    }
                    break;
            }
        }
    }
}