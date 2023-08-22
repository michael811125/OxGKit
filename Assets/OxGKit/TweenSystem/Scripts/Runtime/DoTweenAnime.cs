using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using DG.Tweening;
using UnityEngine.UI;
using OxGKit.Utilities.Timer;
using System.Linq;

namespace OxGKit.TweenSystem
{
    [DisallowMultipleComponent]
    [AddComponentMenu("OxGKit/TweenSystem/" + nameof(DoTweenAnime))]
    public class DoTweenAnime : MonoBehaviour
    {
        public enum PlayMode
        {
            Normal,
            Reverse,
            PingPong,
            Sequence
        }

        public enum DriveMode
        {
            Active,
            Event
        }

        [Separator("Options")]
        [SerializeField, Tooltip("Ignore time scale")]
        private bool _ignoreTimeScale = true;
        [SerializeField]
        public DriveMode driveMode = DriveMode.Active;
        [SerializeField, Tooltip("Auto control active"), ConditionalField(nameof(driveMode), false, DriveMode.Event)]
        private bool _autoActive = false;

        #region tPosition, tRotation, tScale
        [Separator("Transform")]
        [SerializeField, Tooltip("Enable tween Position")]
        public bool tPositionOn = false;
        [SerializeField, Tooltip("Enable tween Rotation")]
        public bool tRotationOn = false;
        [SerializeField, Tooltip("Enable tween Scale")]
        public bool tScaleOn = false;
        #endregion

        #region tSize
        [Separator("RectTransform")]
        [SerializeField, Tooltip("Enable tween Size (Auto find RectTransform Component)")]
        public bool tSizeOn = false;
        #endregion

        #region tAlpha
        [Separator("CanvasGroup")]
        [SerializeField, Tooltip("Enable tween CanvasGroup Alpha (Auto find CanvasGroup Component)")]
        public bool tAlphaOn = false;
        #endregion

        #region tImageColor
        [Separator("Image")]
        [SerializeField, Tooltip("Enable tween Image Color (Auto find Image Component)")]
        public bool tImgColorOn = false;
        #endregion

        #region tSpriteColor
        [Separator("SpriteRenderer")]
        [SerializeField, Tooltip("Enable tween Sprite Color (Auto find SpriteRenderer Component)")]
        public bool tSprColorOn = false;
        #endregion

        #region TweenBase
        [Serializable]
        public class TweenBase
        {
            [SerializeField, Tooltip("Play by interval")]
            public bool isInterval = false;
            [SerializeField, ConditionalField(nameof(isInterval)), Tooltip("Interval time")]
            public float intervalTime = 0.0f;
            [HideInInspector]
            public DeltaTimer intervalTimer = null;

            [SerializeField, ConditionalField(nameof(isInterval), inverse: true), Tooltip("Loop times, -1 = Infinitely")]
            public int loopTimes = 0;
            [SerializeField, ConditionalField(nameof(isInterval), inverse: true)]
            public LoopType loopType = LoopType.Restart;

            [SerializeField, Tooltip("Play mode")]
            public PlayMode playMode = PlayMode.Normal;
            [SerializeField, SearchableEnum, Tooltip("Ease mode")]
            public Ease easeMode = Ease.Linear;

            [SerializeField, Tooltip("Duration time")]
            public float duration = 0.1f;

            [HideInInspector]
            public bool ignoreTimeScale = true;

            [HideInInspector]
            public TweenCallback endCallback = null;
            [HideInInspector]
            public bool autoActive = false;

            [HideInInspector]
            public Sequence seq = null;

            public TweenBase()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public virtual void Reset()
            {
                this.seq?.Kill();
                this.intervalTimer.Stop();
                this.endCallback = null;
                this.autoActive = false;
            }

            public virtual void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false) { }

            public virtual void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false) { }

            public virtual void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false) { }

            public virtual void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false) { }

            public virtual void TickPlay() { }
        }
        #endregion

        #region TweenPosition
        [Serializable]
        public class TweenPosition : TweenBase
        {
            [HideInInspector]
            public Transform transform = null;

            [HideInInspector]
            public Vector3 originPosition = new Vector3(0, 0, 0);

            [SerializeField, Tooltip("Based on own position with tween move")]
            public bool relative = false;
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector3 from = new Vector3(0, 0, 0);
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector3 to = new Vector3(0, 0, 0);

            [Serializable]
            public class Sequence
            {
                public List<Vector3> sequence = new List<Vector3>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence posSeq = new Sequence();

            public TweenPosition()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(Vector3 vec3)
            {
                this.originPosition = vec3;
            }

            public void Init()
            {
                Vector3 withOwnPos = Vector3.zero;

                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + this.to.x, this.transform.localPosition.y + this.to.y, this.transform.position.z + this.to.z);
                        this.originPosition = (this.relative) ? withOwnPos : this.to;
                        break;
                    case PlayMode.Sequence:
                        if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + this.posSeq.sequence[0].x, this.transform.localPosition.y + this.posSeq.sequence[0].y, this.transform.position.z + this.posSeq.sequence[0].z);
                        this.originPosition = (this.relative) ? withOwnPos : this.posSeq.sequence[0];
                        break;
                    default:
                        if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + this.from.x, this.transform.localPosition.y + this.from.y, this.transform.position.z + this.from.z);
                        this.originPosition = (this.relative) ? withOwnPos : this.from;
                        break;
                }
            }

            public override void Reset()
            {
                base.Reset();
                this.transform.localPosition = this.originPosition;
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                Vector3 fromWithOwnPos = Vector3.zero;
                Vector3 toWithOwnPos = Vector3.zero;
                if (this.relative)
                {
                    fromWithOwnPos = new Vector3(this.transform.localPosition.x + this.from.x, this.transform.localPosition.y + this.from.y, this.transform.position.z + this.from.z);
                    toWithOwnPos = new Vector3(this.transform.localPosition.x + this.to.x, this.transform.localPosition.y + this.to.y, this.transform.position.z + this.to.z);
                }

                if (!inverse)
                {
                    this.transform.localPosition = (this.relative) ? fromWithOwnPos : this.from;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? toWithOwnPos : this.to, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localPosition = (this.relative) ? toWithOwnPos : this.to;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? fromWithOwnPos : this.from, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }

            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                Vector3 fromWithOwnPos = Vector3.zero;
                Vector3 toWithOwnPos = Vector3.zero;
                if (this.relative)
                {
                    fromWithOwnPos = new Vector3(this.transform.localPosition.x + this.from.x, this.transform.localPosition.y + this.from.y, this.transform.position.z + this.from.z);
                    toWithOwnPos = new Vector3(this.transform.localPosition.x + this.to.x, this.transform.localPosition.y + this.to.y, this.transform.position.z + this.to.z);
                }

                if (!inverse)
                {
                    this.transform.localPosition = (this.relative) ? toWithOwnPos : this.to;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? fromWithOwnPos : this.from, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localPosition = (this.relative) ? fromWithOwnPos : this.from;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? toWithOwnPos : this.to, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                Vector3 fromWithOwnPos = Vector3.zero;
                Vector3 toWithOwnPos = Vector3.zero;
                if (this.relative)
                {
                    fromWithOwnPos = new Vector3(this.transform.localPosition.x + this.from.x, this.transform.localPosition.y + this.from.y, this.transform.position.z + this.from.z);
                    toWithOwnPos = new Vector3(this.transform.localPosition.x + this.to.x, this.transform.localPosition.y + this.to.y, this.transform.position.z + this.to.z);
                }

                if (!inverse)
                {
                    this.transform.localPosition = (this.relative) ? fromWithOwnPos : this.from;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? toWithOwnPos : this.to, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? fromWithOwnPos : this.from, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localPosition = (this.relative) ? toWithOwnPos : this.to;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? fromWithOwnPos : this.from, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.transform.DOLocalMove((this.relative) ? toWithOwnPos : this.to, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.posSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    Vector3 withOwnPos = Vector3.zero;
                    if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + this.posSeq.sequence[0].x, this.transform.localPosition.y + this.posSeq.sequence[0].y, this.transform.localPosition.z + this.posSeq.sequence[0].z);
                    this.transform.localPosition = (this.relative) ? withOwnPos : this.posSeq.sequence[0];

                    int count = this.posSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.posSeq.sequence.ForEach(pos =>
                    {
                        if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + pos.x, this.transform.localPosition.y + pos.y, this.transform.localPosition.z + pos.z);
                        this.seq.Append(this.transform.DOLocalMove((this.relative) ? withOwnPos : pos, this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    Vector3 withOwnPos = Vector3.zero;
                    if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + this.posSeq.sequence[this.posSeq.sequence.Count - 1].x, this.transform.localPosition.y + this.posSeq.sequence[this.posSeq.sequence.Count - 1].y, this.transform.localPosition.z + this.posSeq.sequence[this.posSeq.sequence.Count - 1].z);
                    this.transform.localPosition = (this.relative) ? withOwnPos : this.posSeq.sequence[this.posSeq.sequence.Count - 1];

                    int count = this.posSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.posSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        if (this.relative) withOwnPos = new Vector3(this.transform.localPosition.x + this.posSeq.sequence[i].x, this.transform.localPosition.y + this.posSeq.sequence[i].y, this.transform.localPosition.z + this.posSeq.sequence[i].z);
                        this.seq.Append(this.transform.DOLocalMove((this.relative) ? withOwnPos : this.posSeq.sequence[i], this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [Separator("TweenValues")]
        [SerializeField, ConditionalField(nameof(tPositionOn))]
        private TweenPosition _tPosition = new TweenPosition();
        public TweenPosition tPosition => _tPosition;

        #region TweenRotation
        [Serializable]
        public class TweenRotation : TweenBase
        {
            [HideInInspector]
            public Transform transform = null;

            [HideInInspector]
            public Vector3 originAngle = new Vector3();

            [SerializeField]
            public RotateMode rotateMode = RotateMode.FastBeyond360;
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector3 beginAngle = new Vector3();
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector3 endAngle = new Vector3();

            [Serializable]
            public class Sequence
            {
                public List<Vector3> sequence = new List<Vector3>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence angleSeq = new Sequence();

            public TweenRotation()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(Vector3 angle)
            {
                this.originAngle = angle;
            }

            public void Init()
            {
                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        this.originAngle = this.endAngle;
                        break;
                    case PlayMode.Sequence:
                        this.originAngle = this.angleSeq.sequence[0];
                        break;
                    default:
                        this.originAngle = this.beginAngle;
                        break;
                }
            }

            private Quaternion _ConvertToEuler(Vector3 vec3)
            {
                return Quaternion.Euler(vec3.x, vec3.y, vec3.z);
            }

            public override void Reset()
            {
                base.Reset();
                this.transform.rotation = this._ConvertToEuler(this.originAngle);
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (!inverse)
                {
                    this.transform.rotation = this._ConvertToEuler(this.beginAngle);

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalRotate(this.endAngle, this.duration, this.rotateMode).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.rotation = this._ConvertToEuler(this.endAngle);

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalRotate(this.beginAngle, this.duration, this.rotateMode).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (!inverse)
                {
                    this.transform.rotation = this._ConvertToEuler(this.endAngle);

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalRotate(this.beginAngle, this.duration, this.rotateMode).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.rotation = this._ConvertToEuler(this.beginAngle);

                    this.seq = DOTween.Sequence();
                    this.seq.Append(this.transform.DOLocalRotate(this.endAngle, this.duration, this.rotateMode).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (!inverse)
                {
                    this.transform.rotation = this._ConvertToEuler(this.beginAngle);

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalRotateQuaternion(this._ConvertToEuler(this.endAngle), this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.transform.DOLocalRotateQuaternion(this._ConvertToEuler(this.beginAngle), this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.rotation = this._ConvertToEuler(this.endAngle);

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOLocalRotateQuaternion(this._ConvertToEuler(this.beginAngle), this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.transform.DOLocalRotateQuaternion(this._ConvertToEuler(this.endAngle), this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.angleSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    this.transform.rotation = this._ConvertToEuler(this.angleSeq.sequence[0]);

                    int count = this.angleSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.angleSeq.sequence.ForEach(angle =>
                    {
                        this.seq.Append(this.transform.DOLocalRotateQuaternion(this._ConvertToEuler(angle), this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.rotation = this._ConvertToEuler(this.angleSeq.sequence[this.angleSeq.sequence.Count - 1]);

                    int count = this.angleSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.angleSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        this.seq.Append(this.transform.DOLocalRotateQuaternion(this._ConvertToEuler(this.angleSeq.sequence[i]), this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [SerializeField, ConditionalField(nameof(tRotationOn))]
        private TweenRotation _tRotation = new TweenRotation();
        public TweenRotation tRotation => _tRotation;

        #region TweenScale
        [Serializable]
        public class TweenScale : TweenBase
        {
            [HideInInspector]
            public Transform transform = null;

            [HideInInspector]
            public Vector3 originScale = new Vector3(0, 0, 0);

            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector3 beginScale = new Vector3(0, 0, 0);
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector3 endScale = new Vector3(0, 0, 0);

            [Serializable]
            public class Sequence
            {
                public List<Vector3> sequence = new List<Vector3>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence scaleSeq = new Sequence();

            public TweenScale()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(Vector3 scale)
            {
                this.originScale = scale;
            }

            public void Init()
            {
                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        this.originScale = this.endScale;
                        break;
                    case PlayMode.Sequence:
                        this.originScale = this.scaleSeq.sequence[0];
                        break;
                    default:
                        this.originScale = this.beginScale;
                        break;
                }
            }

            public override void Reset()
            {
                base.Reset();
                this.transform.localScale = this.originScale;
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (!inverse)
                {
                    this.transform.localScale = this.beginScale;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOScale(this.endScale, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localScale = this.endScale;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOScale(this.beginScale, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (!inverse)
                {
                    this.transform.localScale = this.endScale;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOScale(this.beginScale, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localScale = this.beginScale;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOScale(this.endScale, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (!inverse)
                {
                    this.transform.localScale = this.beginScale;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOScale(this.endScale, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.transform.DOScale(this.beginScale, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localScale = this.endScale;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.transform.DOScale(this.beginScale, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.transform.DOScale(this.endScale, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.scaleSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    this.transform.localScale = this.scaleSeq.sequence[0];

                    int count = this.scaleSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.scaleSeq.sequence.ForEach(scale =>
                    {
                        this.seq.Append(this.transform.DOScale(scale, this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.transform.localScale = this.scaleSeq.sequence[this.scaleSeq.sequence.Count - 1];

                    int count = this.scaleSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.scaleSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        this.seq.Append(this.transform.DOScale(this.scaleSeq.sequence[i], this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.transform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [SerializeField, ConditionalField(nameof(tScaleOn))]
        private TweenScale _tScale = new TweenScale();
        public TweenScale tScale => _tScale;

        #region TweenSize (RectTransform)
        [Serializable]
        public class TweenSize : TweenBase
        {
            [HideInInspector]
            public RectTransform rectTransform = null;

            [HideInInspector]
            public Vector2 originSize = new Vector2(0, 0);

            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector2 beginSize = new Vector2(0, 0);
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Vector2 endSize = new Vector2(0, 0);

            [Serializable]
            public class Sequence
            {
                public List<Vector2> sequence = new List<Vector2>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence sizeSeq = new Sequence();

            public TweenSize()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(Vector2 size)
            {
                this.originSize = size;
            }

            public void Init()
            {
                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        this.originSize = this.endSize;
                        break;
                    case PlayMode.Sequence:
                        this.originSize = this.sizeSeq.sequence[0];
                        break;
                    default:
                        this.originSize = this.beginSize;
                        break;
                }
            }

            public override void Reset()
            {
                base.Reset();
                if (this.rectTransform != null) this.rectTransform.sizeDelta = this.originSize;
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.rectTransform == null) return;

                if (!inverse)
                {
                    this.rectTransform.sizeDelta = this.beginSize;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.endSize, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.rectTransform.sizeDelta = this.endSize;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.beginSize, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.rectTransform == null) return;

                if (!inverse)
                {
                    this.rectTransform.sizeDelta = this.endSize;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.beginSize, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.rectTransform.sizeDelta = this.beginSize;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.endSize, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.rectTransform == null) return;

                if (!inverse)
                {
                    this.rectTransform.sizeDelta = this.beginSize;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.endSize, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.beginSize, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.rectTransform.sizeDelta = this.endSize;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.beginSize, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.rectTransform.DOSizeDelta(this.endSize, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.rectTransform == null) return;

                if (this.sizeSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    this.rectTransform.sizeDelta = this.sizeSeq.sequence[0];

                    int count = this.sizeSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.sizeSeq.sequence.ForEach(size =>
                    {
                        this.seq.Append(this.rectTransform.DOSizeDelta(size, this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.rectTransform.sizeDelta = this.sizeSeq.sequence[this.sizeSeq.sequence.Count - 1];

                    int count = this.sizeSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.sizeSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        this.seq.Append(this.rectTransform.DOSizeDelta(this.sizeSeq.sequence[i], this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.rectTransform.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                if (this.rectTransform == null) return;

                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [SerializeField, ConditionalField(nameof(tSizeOn))]
        private TweenSize _tSize = new TweenSize();
        public TweenSize tSize => _tSize;

        #region TweenAlpha (CanvasGroup)
        [Serializable]
        public class TweenAlpha : TweenBase
        {
            [HideInInspector]
            public CanvasGroup cg = null;

            [HideInInspector]
            public float originAlpha = 0.0f;

            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence), Range(0, 1)]
            public float beginAlpha = 0.0f;
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence), Range(0, 1)]
            public float endAlpha = 0.0f;

            [Serializable]
            public class Sequence
            {
                [SerializeField, Range(0, 1)]
                public List<float> sequence = new List<float>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence alphaSeq = new Sequence();

            public TweenAlpha()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(float alpha)
            {
                this.originAlpha = alpha;
            }

            public void Init()
            {
                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        this.originAlpha = this.endAlpha;
                        break;
                    case PlayMode.Sequence:
                        this.originAlpha = this.alphaSeq.sequence[0];
                        break;
                    default:
                        this.originAlpha = this.beginAlpha;
                        break;
                }
            }

            public override void Reset()
            {
                base.Reset();
                if (this.cg != null) this.cg.alpha = this.originAlpha;
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.cg == null) return;

                if (!inverse)
                {
                    this.cg.alpha = this.beginAlpha;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.cg.DOFade(this.endAlpha, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.cg.alpha = this.endAlpha;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.cg.DOFade(this.beginAlpha, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.cg == null) return;

                if (!inverse)
                {
                    this.cg.alpha = this.endAlpha;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.cg.DOFade(this.beginAlpha, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.cg.alpha = this.beginAlpha;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.cg.DOFade(this.endAlpha, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.cg == null) return;

                if (!inverse)
                {
                    this.cg.alpha = this.beginAlpha;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.cg.DOFade(this.endAlpha, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.cg.DOFade(this.beginAlpha, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.cg.alpha = this.endAlpha;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.cg.DOFade(this.beginAlpha, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.cg.DOFade(this.endAlpha, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.cg == null) return;

                if (this.alphaSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    this.cg.alpha = this.alphaSeq.sequence[0];

                    int count = this.alphaSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.alphaSeq.sequence.ForEach(alpha =>
                    {
                        this.seq.Append(this.cg.DOFade(alpha, this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.cg.alpha = this.alphaSeq.sequence[this.alphaSeq.sequence.Count - 1];

                    int count = this.alphaSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.alphaSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        this.seq.Append(this.cg.DOFade(this.alphaSeq.sequence[i], this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.cg.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                if (this.cg == null) return;

                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [SerializeField, ConditionalField(nameof(tAlphaOn))]
        private TweenAlpha _tAlpha = new TweenAlpha();
        public TweenAlpha tAlpha => _tAlpha;

        #region TweenImageColor (Image)
        [Serializable]
        public class TweenImgColor : TweenBase
        {
            [HideInInspector]
            public Image img = null;

            [HideInInspector]
            public Color originColor = Color.white;

            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Color beginColor = Color.white;
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Color endColor = Color.white;

            [Serializable]
            public class Sequence
            {
                public List<Color> sequence = new List<Color>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence colorSeq = new Sequence();

            public TweenImgColor()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(Color color)
            {
                this.originColor = color;
            }

            public void Init()
            {
                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        this.originColor = this.endColor;
                        break;
                    case PlayMode.Sequence:
                        this.originColor = this.colorSeq.sequence[0];
                        break;
                    default:
                        this.originColor = this.beginColor;
                        break;
                }
            }

            public override void Reset()
            {
                base.Reset();
                if (this.img != null) this.img.color = this.originColor;
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.img == null) return;

                if (!inverse)
                {
                    this.img.color = this.beginColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.img.DOColor(this.endColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.img.color = this.endColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.img.DOColor(this.beginColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.img == null) return;

                if (!inverse)
                {
                    this.img.color = this.endColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.img.DOColor(this.beginColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.img.color = this.beginColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.img.DOColor(this.endColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.img == null) return;

                if (!inverse)
                {
                    this.img.color = this.beginColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.img.DOColor(this.endColor, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.img.DOColor(this.beginColor, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.img.color = this.endColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.img.DOColor(this.beginColor, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.img.DOColor(this.endColor, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.img == null) return;

                if (this.colorSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    this.img.color = this.colorSeq.sequence[0];

                    int count = this.colorSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.colorSeq.sequence.ForEach(color =>
                    {
                        this.seq.Append(this.img.DOColor(color, this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.img.color = this.colorSeq.sequence[this.colorSeq.sequence.Count - 1];

                    int count = this.colorSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.colorSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        this.seq.Append(this.img.DOColor(this.colorSeq.sequence[i], this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.img.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                if (this.img == null) return;

                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [SerializeField, ConditionalField(nameof(tImgColorOn))]
        private TweenImgColor _tImgColor = new TweenImgColor();
        public TweenImgColor tImgColor => _tImgColor;

        #region TweenSprColor (Sprite)
        [Serializable]
        public class TweenSprColor : TweenBase
        {
            [HideInInspector]
            public SpriteRenderer spr = null;

            [HideInInspector]
            public Color originColor = Color.white;

            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Color beginColor = Color.white;
            [SerializeField, ConditionalField(nameof(playMode), inverse: true, PlayMode.Sequence)]
            public Color endColor = Color.white;

            [Serializable]
            public class Sequence
            {
                public List<Color> sequence = new List<Color>();
            }
            [SerializeField, ConditionalField(nameof(playMode), false, PlayMode.Sequence)]
            public Sequence colorSeq = new Sequence();

            public TweenSprColor()
            {
                this.intervalTimer = new DeltaTimer();
            }

            public void Init(Color color)
            {
                this.originColor = color;
            }

            public void Init()
            {
                switch (this.playMode)
                {
                    case PlayMode.Reverse:
                        this.originColor = this.endColor;
                        break;
                    case PlayMode.Sequence:
                        this.originColor = this.colorSeq.sequence[0];
                        break;
                    default:
                        this.originColor = this.beginColor;
                        break;
                }
            }

            public override void Reset()
            {
                base.Reset();
                if (this.spr != null) this.spr.color = this.originColor;
            }

            public override void DoTweenNormal(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.spr == null) return;

                if (!inverse)
                {
                    this.spr.color = this.beginColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.spr.DOColor(this.endColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.spr.color = this.endColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.spr.DOColor(this.beginColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenReverse(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.spr == null) return;

                if (!inverse)
                {
                    this.spr.color = this.endColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.spr.DOColor(this.beginColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.spr.color = this.beginColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.spr.DOColor(this.endColor, this.duration).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenPingPong(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.spr == null) return;

                if (!inverse)
                {
                    this.spr.color = this.beginColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.spr.DOColor(this.endColor, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.spr.DOColor(this.beginColor, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.spr.color = this.endColor;

                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.seq.Append(this.spr.DOColor(this.beginColor, this.duration / 2).SetEase(this.easeMode));
                    this.seq.Append(this.spr.DOColor(this.endColor, this.duration / 2).SetEase(this.easeMode));
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void DoTweenSequence(int loopTimes = 0, bool inverse = false, bool trigger = false)
            {
                if (this.spr == null) return;

                if (this.colorSeq.sequence.Count == 0) return;

                if (!inverse)
                {
                    this.spr.color = this.colorSeq.sequence[0];

                    int count = this.colorSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    this.colorSeq.sequence.ForEach(color =>
                    {
                        this.seq.Append(this.spr.DOColor(color, this.duration / count).SetEase(this.easeMode));
                    });
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
                else
                {
                    this.spr.color = this.colorSeq.sequence[this.colorSeq.sequence.Count - 1];

                    int count = this.colorSeq.sequence.Count;
                    this.seq = DOTween.Sequence();
                    this.seq.SetUpdate(this.ignoreTimeScale);
                    for (int i = (this.colorSeq.sequence.Count - 1); i >= 0; i--)
                    {
                        this.seq.Append(this.spr.DOColor(this.colorSeq.sequence[i], this.duration / count).SetEase(this.easeMode));
                    }
                    if (this.endCallback != null) this.seq.AppendCallback(this.endCallback);
                    if (!trigger && this.autoActive) this.seq.AppendCallback(() => { this.spr.gameObject.SetActive(false); });
                    this.seq.SetEase(this.easeMode);
                    this.seq.SetLoops(loopTimes, this.loopType);
                }
            }

            public override void TickPlay()
            {
                if (this.spr == null) return;

                switch (this.playMode)
                {
                    case PlayMode.Normal:
                        this.DoTweenNormal();
                        break;
                    case PlayMode.Reverse:
                        this.DoTweenReverse();
                        break;
                    case PlayMode.PingPong:
                        this.DoTweenPingPong();
                        break;
                    case PlayMode.Sequence:
                        this.DoTweenSequence();
                        break;
                }
            }
        }
        #endregion
        [SerializeField, ConditionalField(nameof(tSprColorOn))]
        private TweenSprColor _tSprColor = new TweenSprColor();
        public TweenSprColor tSprColor => _tSprColor;

        #region Editor
#if UNITY_EDITOR
        public static bool isPreviewMode = false;
        [SerializeField, HideInInspector] private bool _isSyncBeginValue = false;

        private void OnValidate()
        {
            this._SyncBeginValues();
        }

        private void _SyncBeginValues()
        {
            if (this._isSyncBeginValue)
            {
                // Tween Postion
                if (this.tPositionOn)
                {
                    switch (this._tPosition.playMode)
                    {
                        case PlayMode.Sequence:
                            if (this._tPosition.posSeq.sequence.Count > 0)
                            {
                                this.transform.localPosition = this._tPosition.posSeq.sequence[0];
                            }
                            break;
                        default:
                            this.transform.localPosition = this._tPosition.from;
                            break;
                    }
                }

                // Tween Rotation
                if (this.tRotationOn)
                {
                    switch (this._tRotation.playMode)
                    {
                        case PlayMode.Sequence:
                            if (this._tRotation.angleSeq.sequence.Count > 0)
                            {
                                this.transform.eulerAngles = this._tRotation.angleSeq.sequence[0];
                            }
                            break;
                        default:
                            this.transform.eulerAngles = this._tRotation.beginAngle;
                            break;
                    }
                }

                // Tween Scale
                if (this.tScaleOn)
                {
                    switch (this._tScale.playMode)
                    {
                        case PlayMode.Sequence:
                            if (this._tScale.scaleSeq.sequence.Count > 0)
                            {
                                this.transform.localScale = this._tScale.scaleSeq.sequence[0];
                            }
                            break;
                        default:
                            this.transform.localScale = this._tScale.beginScale;
                            break;
                    }
                }

                // Tween Size (RectTransform)
                if (this.tSizeOn)
                {
                    var rectTransform = this.transform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        switch (this._tSize.playMode)
                        {
                            case PlayMode.Sequence:
                                if (this._tSize.sizeSeq.sequence.Count > 0)
                                {
                                    UnityEditor.EditorApplication.delayCall += () =>
                                    {
                                        if (rectTransform != null) rectTransform.sizeDelta = this._tSize.sizeSeq.sequence[0];
                                        UnityEditor.EditorApplication.delayCall = null;
                                    };
                                }
                                break;
                            default:
                                UnityEditor.EditorApplication.delayCall += () =>
                                {
                                    if (rectTransform != null) rectTransform.sizeDelta = this._tSize.beginSize;
                                    UnityEditor.EditorApplication.delayCall = null;
                                };
                                break;
                        }
                    }
                }

                // Tween Alpha (CanvasGroup)
                if (this.tAlphaOn)
                {
                    var cg = this.transform.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        switch (this._tAlpha.playMode)
                        {
                            case PlayMode.Sequence:
                                if (this._tAlpha.alphaSeq.sequence.Count > 0)
                                {
                                    UnityEditor.EditorApplication.delayCall += () =>
                                    {
                                        if (cg != null) cg.alpha = this._tAlpha.alphaSeq.sequence[0];
                                        UnityEditor.EditorApplication.delayCall = null;
                                    };
                                }
                                break;
                            default:
                                UnityEditor.EditorApplication.delayCall += () =>
                                {
                                    if (cg != null) cg.alpha = this._tAlpha.beginAlpha;
                                    UnityEditor.EditorApplication.delayCall = null;
                                };
                                break;
                        }
                    }
                }

                // Tween Image Color (Image)
                if (this.tImgColorOn)
                {
                    var img = this.transform.GetComponent<Image>();
                    if (img != null)
                    {
                        switch (this._tImgColor.playMode)
                        {
                            case PlayMode.Sequence:
                                if (this._tImgColor.colorSeq.sequence.Count > 0)
                                {
                                    img.color = this._tImgColor.colorSeq.sequence[0];
                                }
                                break;
                            default:
                                img.color = this._tImgColor.beginColor;
                                break;
                        }
                    }
                }

                // Tween Sprite Color (Sprite)
                if (this.tSprColorOn)
                {
                    var spr = this.transform.GetComponent<SpriteRenderer>();
                    if (spr != null)
                    {
                        switch (this._tSprColor.playMode)
                        {
                            case PlayMode.Sequence:
                                if (this._tSprColor.colorSeq.sequence.Count > 0)
                                {
                                    spr.color = this._tSprColor.colorSeq.sequence[0];
                                }
                                break;
                            default:
                                spr.color = this._tSprColor.beginColor;
                                break;
                        }
                    }
                }
            }
        }
#endif
        #endregion

        private void Awake()
        {
            // Init values
            this.InitTweens();

            // Init ignoreTimeScales
            this._tPosition.ignoreTimeScale = this._ignoreTimeScale;
            this._tRotation.ignoreTimeScale = this._ignoreTimeScale;
            this._tScale.ignoreTimeScale = this._ignoreTimeScale;
            this._tSize.ignoreTimeScale = this._ignoreTimeScale;
            this._tAlpha.ignoreTimeScale = this._ignoreTimeScale;
            this._tImgColor.ignoreTimeScale = this._ignoreTimeScale;
            this._tSprColor.ignoreTimeScale = this._ignoreTimeScale;

            // Reset first
            this.ResetTweens();

            // Finally, do active init by DriveMode
            switch (this.driveMode)
            {
                case DriveMode.Event:
                    if (this._autoActive) this.gameObject.SetActive(false);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (this.driveMode == DriveMode.Event) return;

            float dt = this._ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            #region tPostion TickPlay
            if (this.tPositionOn && this._tPosition.isInterval)
            {
                this._tPosition.intervalTimer.UpdateTimer(dt);
                if (this._tPosition.intervalTimer.IsTickTimeout())
                {
                    this._tPosition.TickPlay();
                }
            }
            #endregion

            #region tRoatation TickPlay
            if (this.tRotationOn && this._tRotation.isInterval)
            {
                this._tRotation.intervalTimer.UpdateTimer(dt);
                if (this._tRotation.intervalTimer.IsTickTimeout())
                {
                    this._tRotation.TickPlay();
                }
            }
            #endregion

            #region tScale TickPlay
            if (this.tScaleOn && this._tScale.isInterval)
            {
                this._tScale.intervalTimer.UpdateTimer(dt);
                if (this._tScale.intervalTimer.IsTickTimeout())
                {
                    this._tScale.TickPlay();
                }
            }
            #endregion

            #region tSize TickPlay (RectTransform)
            if (this.tSizeOn && this._tSize.isInterval)
            {
                this._tSize.intervalTimer.UpdateTimer(dt);
                if (this._tSize.intervalTimer.IsTickTimeout())
                {
                    this._tSize.TickPlay();
                }
            }
            #endregion

            #region tAlpah TickPlay (CanvasGroup)
            if (this.tAlphaOn && this._tAlpha.isInterval)
            {
                this._tAlpha.intervalTimer.UpdateTimer(dt);
                if (this._tAlpha.intervalTimer.IsTickTimeout())
                {
                    this._tAlpha.TickPlay();
                }
            }
            #endregion

            #region tImgColor TickPlay (Image)
            if (this.tImgColorOn && this._tImgColor.isInterval)
            {
                this._tImgColor.intervalTimer.UpdateTimer(dt);
                if (this._tImgColor.intervalTimer.IsTickTimeout())
                {
                    this._tImgColor.TickPlay();
                }
            }
            #endregion

            #region tSprColor TickPlay (Sprite)
            if (this.tSprColorOn && this._tSprColor.isInterval)
            {
                this._tSprColor.intervalTimer.UpdateTimer(dt);
                if (this._tSprColor.intervalTimer.IsTickTimeout())
                {
                    this._tSprColor.TickPlay();
                }
            }
            #endregion
        }

        public void InitTweens()
        {
            // Init Tween Postion
            if (this.tPositionOn)
            {
                this._tPosition.transform = this.transform;
                if (this._tPosition.relative) this._tPosition.Init(this.transform.localPosition);
                else this._tPosition.Init();
            }

            // Init Tween Rotation
            if (this.tRotationOn)
            {
                this._tRotation.transform = this.transform;
                //this._tRotation.Init(this.transform.localRotation.eulerAngles);
                this._tRotation.Init();
            }

            // Init Tween Scale
            if (this.tScaleOn)
            {
                this._tScale.transform = this.transform;
                //this._tScale.Init(this.transform.localScale);
                this._tScale.Init();
            }

            // Init Tween Size (RectTransform)
            if (this.tSizeOn)
            {
                var rectTransform = this.transform.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    this._tSize.rectTransform = rectTransform;
                    //this._tSize.Init(rectTransform.sizeDelta);
                    this._tSize.Init();
                }
            }

            // Init Tween Alpha (CanvasGroup)
            if (this.tAlphaOn)
            {
                var cg = this.transform.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    this._tAlpha.cg = cg;
                    //this._tAlpha.Init(cg.alpha);
                    this._tAlpha.Init();
                }
            }

            // Init Tween Image Color (Image)
            if (this.tImgColorOn)
            {
                var img = this.transform.GetComponent<Image>();
                if (img != null)
                {
                    this._tImgColor.img = img;
                    //this._tImgColor.Init(img.color);
                    this._tImgColor.Init();
                }
            }

            // Init Tween Sprite Color (Sprite)
            if (this.tSprColorOn)
            {
                var spr = this.transform.GetComponent<SpriteRenderer>();
                if (spr != null)
                {
                    this._tSprColor.spr = spr;
                    //this._tSprColor.Init(spr.color);
                    this._tSprColor.Init();
                }
            }
        }

        public void ResetTweens()
        {
            if (this.tPositionOn) this._tPosition.Reset();
            if (this.tRotationOn) this._tRotation.Reset();
            if (this.tScaleOn) this._tScale.Reset();
            if (this.tSizeOn) this._tSize.Reset();
            if (this.tAlphaOn) this._tAlpha.Reset();
            if (this.tImgColorOn) this._tImgColor.Reset();
            if (this.tSprColorOn) this._tSprColor.Reset();
        }

        #region Filter
        protected void SetMainEndCallbackByDuration(bool autoActive = false, TweenCallback endCallback = null)
        {
            TweenBase maxDurationTween = this.GetMaxDurationTween();
            if (maxDurationTween != null)
            {
                if (autoActive) maxDurationTween.autoActive = true;
                if (endCallback != null) maxDurationTween.endCallback = endCallback;
            }
        }

        protected List<TweenBase> GetTweens()
        {
            List<TweenBase> tweens = new List<TweenBase>();

            if (this.tPositionOn) tweens.Add(this._tPosition);
            if (this.tRotationOn) tweens.Add(this._tRotation);
            if (this.tScaleOn) tweens.Add(this._tScale);
            if (this.tSizeOn) tweens.Add(this._tSize);
            if (this.tAlphaOn) tweens.Add(this._tAlpha);
            if (this.tImgColorOn) tweens.Add(this._tImgColor);
            if (this.tSprColorOn) tweens.Add(this._tSprColor);

            return tweens;
        }

        /// <summary>
        /// Get max duration tween
        /// </summary>
        /// <returns></returns>
        public TweenBase GetMaxDurationTween()
        {
            List<TweenBase> tweens = this.GetTweens();
            if (tweens.Count == 0) return null;

            // Get max of duration time
            TweenBase maxDurationTween = tweens.Aggregate((e, n) => e.duration > n.duration ? e : n);
            return maxDurationTween;
        }
        #endregion

        #region Play
        /// <summary>
        /// Play by event
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="endCallback"></param>
        public void PlayTween(bool trigger, TweenCallback endCallback = null)
        {
            this.gameObject.SetActive(true);
            this.ResetTweens();
            bool autoActive = this._autoActive;
#if UNITY_EDITOR
            if (isPreviewMode) autoActive = false;
#endif
            this.SetMainEndCallbackByDuration(autoActive, endCallback);
            this.DoTweenAnimes(trigger);
        }

        protected void PlayTween()
        {
            this.gameObject.SetActive(true);
            this.DoTweenAnimes(false);
        }

        protected void DoTweenAnimes(bool trigger)
        {
            int loopTimes;

            #region Tween Postion On
            if (this.tPositionOn)
            {
                switch (this._tPosition.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tPosition.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tPosition.isInterval)
                        {
                            this._tPosition.DoTweenNormal();

                            this._tPosition.intervalTimer.Play();
                            this._tPosition.intervalTimer.SetTick(this._tPosition.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tPosition.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tPosition.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tPosition.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tPosition.isInterval)
                        {
                            this._tPosition.DoTweenReverse();

                            this._tPosition.intervalTimer.Play();
                            this._tPosition.intervalTimer.SetTick(this._tPosition.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tPosition.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tPosition.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tPosition.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tPosition.isInterval)
                        {
                            this._tPosition.DoTweenPingPong();

                            this._tPosition.intervalTimer.Play();
                            this._tPosition.intervalTimer.SetTick(this._tPosition.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tPosition.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tPosition.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tPosition.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tPosition.isInterval)
                        {
                            this._tPosition.DoTweenSequence();

                            this._tPosition.intervalTimer.Play();
                            this._tPosition.intervalTimer.SetTick(this._tPosition.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tPosition.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tPosition.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion

            #region Tween Rotation On
            if (this.tRotationOn)
            {
                switch (this._tRotation.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tRotation.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tRotation.isInterval)
                        {
                            this._tRotation.DoTweenNormal();

                            this._tRotation.intervalTimer.Play();
                            this._tRotation.intervalTimer.SetTick(this._tRotation.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tRotation.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tRotation.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tRotation.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tRotation.isInterval)
                        {
                            this._tRotation.DoTweenReverse();

                            this._tRotation.intervalTimer.Play();
                            this._tRotation.intervalTimer.SetTick(this._tRotation.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tRotation.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tRotation.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tRotation.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tRotation.isInterval)
                        {
                            this._tRotation.DoTweenPingPong();

                            this._tRotation.intervalTimer.Play();
                            this._tRotation.intervalTimer.SetTick(this._tRotation.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tRotation.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tRotation.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tRotation.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tRotation.isInterval)
                        {
                            this._tRotation.DoTweenSequence();

                            this._tRotation.intervalTimer.Play();
                            this._tRotation.intervalTimer.SetTick(this._tRotation.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tRotation.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tRotation.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion

            #region Tween Scale On
            if (this.tScaleOn)
            {
                switch (this._tScale.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tScale.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tScale.isInterval)
                        {
                            this._tScale.DoTweenNormal();

                            this._tScale.intervalTimer.Play();
                            this._tScale.intervalTimer.SetTick(this._tScale.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tScale.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tScale.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tScale.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tScale.isInterval)
                        {
                            this._tScale.DoTweenReverse();

                            this._tScale.intervalTimer.Play();
                            this._tScale.intervalTimer.SetTick(this._tScale.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tScale.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tScale.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tScale.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tScale.isInterval)
                        {
                            this._tScale.DoTweenPingPong();

                            this._tScale.intervalTimer.Play();
                            this._tScale.intervalTimer.SetTick(this._tScale.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tScale.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tScale.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tScale.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tScale.isInterval)
                        {
                            this._tScale.DoTweenSequence();

                            this._tScale.intervalTimer.Play();
                            this._tScale.intervalTimer.SetTick(this._tScale.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tScale.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tScale.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion

            #region Tween Size On (RectTransform)
            if (this.tSizeOn)
            {
                switch (this._tSize.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSize.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tSize.isInterval)
                        {
                            this._tSize.DoTweenNormal();

                            this._tSize.intervalTimer.Play();
                            this._tSize.intervalTimer.SetTick(this._tSize.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSize.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSize.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSize.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tSize.isInterval)
                        {
                            this._tSize.DoTweenReverse();

                            this._tSize.intervalTimer.Play();
                            this._tSize.intervalTimer.SetTick(this._tSize.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSize.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSize.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSize.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tSize.isInterval)
                        {
                            this._tSize.DoTweenPingPong();

                            this._tSize.intervalTimer.Play();
                            this._tSize.intervalTimer.SetTick(this._tSize.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSize.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSize.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSize.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tSize.isInterval)
                        {
                            this._tSize.DoTweenSequence();

                            this._tSize.intervalTimer.Play();
                            this._tSize.intervalTimer.SetTick(this._tSize.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSize.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSize.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion

            #region Tween Alpah On (CanvasGroup)
            if (this.tAlphaOn)
            {
                switch (this._tAlpha.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tAlpha.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tAlpha.isInterval)
                        {
                            this._tAlpha.DoTweenNormal();

                            this._tAlpha.intervalTimer.Play();
                            this._tAlpha.intervalTimer.SetTick(this._tAlpha.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tAlpha.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tAlpha.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tAlpha.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tAlpha.isInterval)
                        {
                            this._tAlpha.DoTweenReverse();

                            this._tAlpha.intervalTimer.Play();
                            this._tAlpha.intervalTimer.SetTick(this._tAlpha.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tAlpha.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tAlpha.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tAlpha.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tAlpha.isInterval)
                        {
                            this._tAlpha.DoTweenPingPong();

                            this._tAlpha.intervalTimer.Play();
                            this._tAlpha.intervalTimer.SetTick(this._tAlpha.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tAlpha.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tAlpha.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tAlpha.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tAlpha.isInterval)
                        {
                            this._tAlpha.DoTweenSequence();

                            this._tAlpha.intervalTimer.Play();
                            this._tAlpha.intervalTimer.SetTick(this._tAlpha.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tAlpha.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tAlpha.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion

            #region Tween Image Color On (Image)
            if (this.tImgColorOn)
            {
                switch (this._tImgColor.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tImgColor.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tImgColor.isInterval)
                        {
                            this._tImgColor.DoTweenNormal();

                            this._tImgColor.intervalTimer.Play();
                            this._tImgColor.intervalTimer.SetTick(this._tImgColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tImgColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tImgColor.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tImgColor.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tImgColor.isInterval)
                        {
                            this._tImgColor.DoTweenReverse();

                            this._tImgColor.intervalTimer.Play();
                            this._tImgColor.intervalTimer.SetTick(this._tImgColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tImgColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tImgColor.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tImgColor.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tImgColor.isInterval)
                        {
                            this._tImgColor.DoTweenPingPong();

                            this._tImgColor.intervalTimer.Play();
                            this._tImgColor.intervalTimer.SetTick(this._tImgColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tImgColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tImgColor.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tImgColor.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tImgColor.isInterval)
                        {
                            this._tImgColor.DoTweenSequence();

                            this._tImgColor.intervalTimer.Play();
                            this._tImgColor.intervalTimer.SetTick(this._tImgColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tImgColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tImgColor.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion

            #region Tween Sprite Color On (Sprite)
            if (this.tSprColorOn)
            {
                switch (this._tSprColor.playMode)
                {
                    case PlayMode.Normal:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSprColor.DoTweenNormal(0, !trigger, trigger);
                        }
                        else if (this._tSprColor.isInterval)
                        {
                            this._tSprColor.DoTweenNormal();

                            this._tSprColor.intervalTimer.Play();
                            this._tSprColor.intervalTimer.SetTick(this._tSprColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSprColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSprColor.DoTweenNormal(loopTimes);
                        }
                        break;
                    case PlayMode.Reverse:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSprColor.DoTweenReverse(0, !trigger, trigger);
                        }
                        else if (this._tSprColor.isInterval)
                        {
                            this._tSprColor.DoTweenReverse();

                            this._tSprColor.intervalTimer.Play();
                            this._tSprColor.intervalTimer.SetTick(this._tSprColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSprColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSprColor.DoTweenReverse(loopTimes);
                        }
                        break;
                    case PlayMode.PingPong:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSprColor.DoTweenPingPong(0, !trigger, trigger);
                        }
                        else if (this._tSprColor.isInterval)
                        {
                            this._tSprColor.DoTweenPingPong();

                            this._tSprColor.intervalTimer.Play();
                            this._tSprColor.intervalTimer.SetTick(this._tSprColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSprColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSprColor.DoTweenPingPong(loopTimes);
                        }
                        break;
                    case PlayMode.Sequence:
                        if (this.driveMode == DriveMode.Event)
                        {
                            this._tSprColor.DoTweenSequence(0, !trigger, trigger);
                        }
                        else if (this._tSprColor.isInterval)
                        {
                            this._tSprColor.DoTweenSequence();

                            this._tSprColor.intervalTimer.Play();
                            this._tSprColor.intervalTimer.SetTick(this._tSprColor.intervalTime);
                        }
                        else
                        {
                            loopTimes = this._tSprColor.loopTimes;
#if UNITY_EDITOR
                            if (isPreviewMode) loopTimes = 0;
#endif
                            this._tSprColor.DoTweenSequence(loopTimes);
                        }
                        break;
                }
            }
            #endregion
        }
        #endregion

        private void OnEnable()
        {
            switch (this.driveMode)
            {
                case DriveMode.Active:
                    this.ResetTweens();
                    this.PlayTween();
                    break;
            }
        }

        private void OnDisable()
        {
            switch (this.driveMode)
            {
                case DriveMode.Active:
                    this.ResetTweens();
                    break;
            }
        }

        private void OnDestroy()
        {
            // Kill Tweens
            this.ResetTweens();
        }
    }
}