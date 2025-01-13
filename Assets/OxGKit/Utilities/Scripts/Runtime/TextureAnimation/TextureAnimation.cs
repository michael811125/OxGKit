using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OxGKit.Utilities.TextureAnim
{
    [AddComponentMenu("OxGKit/Utilities/TextureAnim/" + nameof(TextureAnimation))]
    [ExecuteInEditMode]
    public class TextureAnimation : MonoBehaviour
    {
        enum PlayMode
        {
            Normal,
            Reverse,
            PingPong,
            PingPongReverse
        }

        [SerializeField]
        private List<Sprite> _sprites = new List<Sprite>();
        [SerializeField]
        private bool _isLoop = false;
        [SerializeField]
        private PlayMode _playMode = PlayMode.Normal;
        [SerializeField]
        private int _frameRate = 30;
        [SerializeField]
        private bool _ignoreTimeScale = true;

        private float _dt = 0;
        private int _spIdx = 0;
        private bool _isAnimationComplete = false;
        private bool _pingPongStart = false;
        private int _pingPongCount = 0;

        private SpriteRenderer _spr = null;
        private Image _image = null;

        private Action _onAnimationComplete;

        private void Awake()
        {
            do
            {
                this._spr = this.transform.GetComponent<SpriteRenderer>();
                if (this._spr != null) break;
                this._image = this.transform.GetComponent<Image>();
                if (this._image != null) break;
            } while (false);
        }

        private void Start()
        {
            this.ResetAnim();
        }

        private void Update()
        {
            if (this._sprites == null || this._sprites.Count == 0) return;
            if (!this._isLoop && this._isAnimationComplete) return;

            this._UpdateTextureAnimation(this._ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
        }

        private void OnValidate()
        {
            this.ResetAnim();
        }
#endif

        private void OnDisable()
        {
            this.ResetAnim();
        }

        #region Public Methods
        /// <summary>
        /// Checks if the animation has completed.
        /// </summary>
        /// <returns>Returns true if the animation is complete; otherwise, false.</returns>
        public bool IsAnimationComplete()
        {
            return this._isAnimationComplete;
        }

        /// <summary>
        /// Sets a callback to be invoked when the animation ends.
        /// </summary>
        /// <param name="endCallback">The callback to invoke upon animation completion.</param>
        public void SetAnimationEnd(Action endCallback)
        {
            this._onAnimationComplete = endCallback;
        }

        /// <summary>
        /// Sets whether to ignore the time scale when playing the animation.
        /// </summary>
        /// <param name="ignore">True to ignore time scale, false otherwise.</param>
        public void SetIgnoreScale(bool ignore)
        {
            this._ignoreTimeScale = ignore;
        }

        /// <summary>
        /// Sets the frame rate for the animation playback.
        /// </summary>
        /// <param name="frameRate">The desired frame rate (minimum is 0).</param>
        public void SetFrameRate(int frameRate)
        {
            this._frameRate = Mathf.Max(0, frameRate);
        }

        /// <summary>
        /// Resets the animation to its initial state.
        /// </summary>
        public void ResetAnim()
        {
            this._dt = 0;
            this._spIdx = 0;
            this._isAnimationComplete = false;
            this._pingPongCount = 0;

            // Initialize ping-pong animation start direction based on play mode
            if (this._playMode == PlayMode.PingPong)
                this._pingPongStart = true;
            else if (this._playMode == PlayMode.PingPongReverse)
                this._pingPongStart = false;

            // Ensure the first frame is displayed when resetting
            if (this._sprites != null && this._sprites.Count > 0)
            {
                this._AutoRefreshSprite(this._GetStartingSprite());
            }
        }
        #endregion

        private Sprite _GetStartingSprite()
        {
            if (this._playMode == PlayMode.Reverse ||
               (this._playMode == PlayMode.PingPongReverse && !this._pingPongStart))
            {
                return this._sprites[this._sprites.Count - 1];
            }
            return this._sprites[0];
        }

        private void _AutoRefreshSprite(Sprite sp)
        {
            if (sp == null) return;
            if (this._spr != null) this._spr.sprite = sp;
            else if (this._image != null) this._image.sprite = sp;
        }

        private void _UpdateTextureAnimation(float dt)
        {
            if (this._frameRate <= 0) return;

            this._dt += dt;
            float frameTime = 1f / this._frameRate;
            int newSpIdx = Mathf.FloorToInt(this._dt / frameTime);

            // Update only when the frame changes
            if (newSpIdx != this._spIdx)
            {
                this._spIdx = newSpIdx;

                switch (this._playMode)
                {
                    case PlayMode.Normal:
                        this._ModeNormal();
                        break;
                    case PlayMode.Reverse:
                        this._ModeReverse();
                        break;
                    case PlayMode.PingPong:
                        this._ModePingPong();
                        break;
                    case PlayMode.PingPongReverse:
                        this._ModePingPongReverse();
                        break;
                }
            }
        }

        private void _ModeNormal()
        {
            if (!this._isLoop && this._spIdx >= this._sprites.Count)
            {
                this._CompleteAnimation(this._sprites[this._sprites.Count - 1]);
                return;
            }

            int safeIdx = this._spIdx % this._sprites.Count;
            this._AutoRefreshSprite(this._sprites[safeIdx]);
        }

        private void _ModeReverse()
        {
            if (!this._isLoop && this._spIdx >= this._sprites.Count)
            {
                this._CompleteAnimation(this._sprites[0]);
                return;
            }

            int safeIdx = this._spIdx % this._sprites.Count;
            int revSpIdx = this._sprites.Count - 1 - safeIdx;
            this._AutoRefreshSprite(this._sprites[revSpIdx]);
        }

        private void _ModePingPong()
        {
            if (!this._isLoop && this._pingPongCount >= 2)
            {
                // Ensure the animation ends at the correct frame
                this._CompleteAnimation(this._sprites[0]);
                return;
            }

            if (this._pingPongStart)
            {
                if (this._spIdx >= (this._sprites.Count - 1))
                {
                    this._pingPongStart = false;
                    this._dt = 0;
                    if (!this._isLoop) this._pingPongCount++;
                }

                int safeIdx = Mathf.Min(this._spIdx, this._sprites.Count - 1);
                this._AutoRefreshSprite(this._sprites[safeIdx]);
            }
            else
            {
                if (this._spIdx >= (this._sprites.Count - 1))
                {
                    this._pingPongStart = true;
                    this._dt = 0;
                    if (!this._isLoop) this._pingPongCount++;
                }

                int safeIdx = Mathf.Min(this._spIdx, this._sprites.Count - 1);
                int reverseSpIdx = this._sprites.Count - 1 - safeIdx;
                this._AutoRefreshSprite(this._sprites[reverseSpIdx]);
            }
        }

        private void _ModePingPongReverse()
        {
            if (!this._isLoop && this._pingPongCount >= 2)
            {
                // Ensure the animation ends at the correct frame
                this._CompleteAnimation(this._sprites[this._sprites.Count - 1]);
                return;
            }

            if (this._pingPongStart)
            {
                if (this._spIdx >= (this._sprites.Count - 1))
                {
                    this._pingPongStart = false;
                    this._dt = 0;
                    if (!this._isLoop) this._pingPongCount++;
                }

                int safeIdx = Mathf.Min(this._spIdx, this._sprites.Count - 1);
                this._AutoRefreshSprite(this._sprites[safeIdx]);
            }
            else
            {
                if (this._spIdx >= (this._sprites.Count - 1))
                {
                    this._pingPongStart = true;
                    this._dt = 0;
                    if (!this._isLoop) this._pingPongCount++;
                }

                int safeIdx = Mathf.Min(this._spIdx, this._sprites.Count - 1);
                int reverseSpIdx = this._sprites.Count - 1 - safeIdx;
                this._AutoRefreshSprite(this._sprites[reverseSpIdx]);
            }
        }

        private void _CompleteAnimation(Sprite finalSprite)
        {
            if (this._isAnimationComplete) return;
            this._isAnimationComplete = true;
            this._AutoRefreshSprite(finalSprite);
            this._onAnimationComplete?.Invoke();
        }
    }
}