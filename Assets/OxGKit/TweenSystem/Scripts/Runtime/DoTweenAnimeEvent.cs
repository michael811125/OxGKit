using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace OxGKit.TweenSystem
{
    [AddComponentMenu("OxGKit/TweenSystem/" + nameof(DoTweenAnimeEvent))]
    public class DoTweenAnimeEvent : MonoBehaviour
    {
        public enum PlayMode
        {
            Parallel,
            Sequence
        }

        [SerializeField, Tooltip("Play Mode")]
        public PlayMode playMode = PlayMode.Parallel;
        [SerializeField, Tooltip("Set DoTweenAnimes")]
        public List<DoTweenAnime> doTweenAnimes = new List<DoTweenAnime>();

        private bool _lastTrigger = false;

        public void ResetTrigger()
        {
            this._lastTrigger = false;
        }

        public void SetPlayMode(PlayMode playMode)
        {
            this.playMode = playMode;
        }

        public DoTweenAnimeEvent AddDoTweenAnime(params DoTweenAnime[] doTweenAnimes)
        {
            if (doTweenAnimes != null && doTweenAnimes.Length > 0)
            {
                this.doTweenAnimes.AddRange(doTweenAnimes);
            }

            return this;
        }

        public void RemoveDoTweenAnime(DoTweenAnime doTweenAnime)
        {
            if (this.doTweenAnimes.Count == 0) return;

            if (this.doTweenAnimes.Contains(doTweenAnime))
            {
                this.doTweenAnimes.Remove(doTweenAnime);
            }
        }

        public void ClearDoTweenAnimes()
        {
            this.doTweenAnimes.Clear();
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
            if (this.doTweenAnimes.Count == 0) return;

            switch (this.playMode)
            {
                case PlayMode.Parallel:
                    {
                        Sequence seq = DOTween.Sequence();
                        for (int i = 0; i < this.doTweenAnimes.Count; i++)
                        {
                            int idx = i;
                            seq.AppendCallback(() => this.doTweenAnimes[idx]?.PlayTween(trigger));
                        }
                        if (endCallback != null) seq.AppendCallback(endCallback);
                        seq.AppendCallback(() => seq.Kill());
                    }
                    break;
                case PlayMode.Sequence:
                    {
                        Sequence seq = DOTween.Sequence();
                        for (int i = 0; i < this.doTweenAnimes.Count; i++)
                        {
                            int idx = i;
                            float duration = (this.doTweenAnimes[i] == null) ? 0f : this.doTweenAnimes[i].GetMaxDurationTween().duration;
                            seq.AppendCallback(() => this.doTweenAnimes[idx]?.PlayTween(trigger));
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