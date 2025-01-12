using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        /// <summary>
        /// Reset the trigger flag
        /// </summary>
        public void ResetTrigger()
        {
            this._lastTrigger = false;
        }

        /// <summary>
        /// Set the PlayMode
        /// </summary>
        /// <param name="playMode"></param>
        public void SetPlayMode(PlayMode playMode)
        {
            this.playMode = playMode;
        }

        /// <summary>
        /// Add a tween to the tweens list
        /// </summary>
        /// <param name="doTweenAnims"></param>
        /// <returns></returns>
        public DoTweenAnimEvent AddDoTweenAnim(params DoTweenAnim[] doTweenAnims)
        {
            if (doTweenAnims != null && doTweenAnims.Length > 0)
            {
                this.doTweenAnims.AddRange(doTweenAnims);
            }

            return this;
        }

        /// <summary>
        /// Remove a specific tween from the tweens list
        /// </summary>
        /// <param name="doTweenAnim"></param>
        public void RemoveDoTweenAnim(DoTweenAnim doTweenAnim)
        {
            if (this.doTweenAnims.Count == 0) return;

            if (this.doTweenAnims.Contains(doTweenAnim))
            {
                this.doTweenAnims.Remove(doTweenAnim);
            }
        }

        /// <summary>
        /// Clear the tweens list
        /// </summary>
        public void ClearDoTweenAnims()
        {
            this.doTweenAnims.Clear();
        }

        /// <summary>
        /// Plays in normal order
        /// </summary>
        public void PlayNormal()
        {
            this.PlayTweens(true, null);
        }

        /// <summary>
        /// Plays in normal order
        /// </summary>
        /// <param name="endCallback"></param>
        public void PlayNormal(TweenCallback endCallback = null)
        {
            this.PlayTweens(true, endCallback);
        }

        /// <summary>
        /// Plays in reverse order
        /// </summary>
        public void PlayReverse()
        {
            this.PlayTweens(false, null);
        }

        /// <summary>
        /// Plays in reverse order
        /// </summary>
        /// <param name="endCallback"></param>
        public void PlayReverse(TweenCallback endCallback = null)
        {
            this.PlayTweens(false, endCallback);
        }

        /// <summary>
        /// Automatically record the trigger: when the trigger is true, it will be in normal mode; when the trigger is false, it will be in reverse mode
        /// </summary>
        public void PlayTrigger()
        {
            this._lastTrigger = !this._lastTrigger;
            this.PlayTrigger(this._lastTrigger, null);
        }

        /// <summary>
        /// Automatically record the trigger: when the trigger is true, it will be in normal mode; when the trigger is false, it will be in reverse mode
        /// </summary>
        /// <param name="endCallback"></param>
        public void PlayTrigger(TweenCallback endCallback = null)
        {
            this._lastTrigger = !this._lastTrigger;
            this.PlayTrigger(this._lastTrigger, endCallback);
        }

        /// <summary>
        /// When trigger is set to true, it will be in normal mode; when set to false, it will be in reverse mode
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="endCallback"></param>
        public void PlayTrigger(bool trigger, TweenCallback endCallback = null)
        {
            this.PlayTweens(trigger, endCallback);
        }

        /// <summary>
        /// Allow the trigger to play only once. When the trigger is set to true, it will play in normal mode; when set to false, it will play in reverse mode
        /// </summary>
        /// <param name="trigger"></param>
        public void PlayTriggerOnce(bool trigger)
        {
            this.PlayTriggerOnce(trigger, null);
        }

        /// <summary>
        /// Allow the trigger to play only once. When the trigger is set to true, it will play in normal mode; when set to false, it will play in reverse mode
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="endCallback"></param>
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
                        // Durations list
                        List<float> durations = new List<float>();

                        // Seq tweens
                        var seq = DOTween.Sequence();

                        for (int i = 0; i < this.doTweenAnims.Count; i++)
                        {
                            int idx = i;
                            var tween = this.doTweenAnims[idx];
                            seq.AppendCallback(() => tween?.PlayTween(trigger));

                            // Create a list of the maximum durations for all tweens
                            if (tween != null) durations.Add(tween.GetMaxDurationTween().duration);
                        }

                        // Find the maximum value from the list to use as the interval time for the endCallback
                        float maxDuration = durations.Aggregate((a, b) => a > b ? a : b);
                        seq.AppendInterval(maxDuration);

                        // Add endCallback to the end
                        if (endCallback != null) seq.AppendCallback(endCallback);
                        seq.AppendCallback(() => seq.Kill());
                    }
                    break;
                case PlayMode.Sequence:
                    {
                        // Seq tweens
                        var seq = DOTween.Sequence();

                        for (int i = 0; i < this.doTweenAnims.Count; i++)
                        {
                            int idx = i;
                            var tween = this.doTweenAnims[idx];

                            // Get the maximum duration of the tween
                            float duration = (tween != null) ? tween.GetMaxDurationTween().duration : 0f;
                            seq.AppendCallback(() => tween?.PlayTween(trigger));

                            // The time interval of the tween
                            seq.AppendInterval(duration);
                        }

                        // Add endCallback to the end
                        if (endCallback != null) seq.AppendCallback(endCallback);
                        seq.AppendCallback(() => seq.Kill());
                    }
                    break;
            }
        }
    }
}