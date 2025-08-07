using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OxGKit.VirtualJoystick
{
    public enum StickVectorMode
    {
        Normalized,
        Delta
    }

    public enum StickType
    {
        Fixed = 0,
        Floating = 1
    }

    public enum AxisConstraint
    {
        Both = 0,
        Horizontal = 1,
        Vertical = 2
    }

    [AddComponentMenu("OxGKit/VirtualJoystick/VirtualJoystick")]
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        /// <summary>
        /// 當搖桿輸出向量變化時的回調
        /// </summary>
        public Action<Vector2> onStickInput;

        /// <summary>
        /// 搖桿向量輸出模式
        /// <para> Normalized: 範圍限制在 -1.00 ~ 1.00 </para>
        /// <para> Delta: 模仿 Mouse Delta 可大於 1 </para>
        /// </summary>
        [SerializeField]
        private StickVectorMode _stickVectorMode = StickVectorMode.Normalized;

        /// <summary>
        /// 搖桿顯示類型 (固定位置、浮動位置、動態顯示)
        /// </summary>
        [SerializeField]
        private StickType _stickType;

        /// <summary>
        /// 限制搖桿輸出方向 (水平、垂直、雙軸)
        /// </summary>
        [SerializeField]
        private AxisConstraint _axisConstraint = AxisConstraint.Both;

        /// <summary>
        /// 搖桿把手控制範圍
        /// </summary>
        [SerializeField]
        private float _handleMovementRange = 100f;

        /// <summary>
        /// 死區範圍, 低於此值的搖桿輸入將視為無效 (避免誤觸)
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        private float _deadZone = 0f;

        /// <summary>
        /// 是否僅在按下時顯示搖桿背景 (例如浮動搖桿)
        /// </summary>
        [SerializeField]
        private bool _showOnlyWhenPressed;

        /// <summary>
        /// 搖桿背景 UI 元件 (搖桿底圖)
        /// </summary>
        [SerializeField]
        private RectTransform _background;

        /// <summary>
        /// 搖桿把手 UI 元件 (可拖曳的控制點)
        /// </summary>
        [SerializeField]
        private RectTransform _handle;

        /// <summary>
        /// 搖桿所在畫布 (用於解析比例與位置換算)
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// 按下時記錄的起始位置, 用於計算拖曳向量
        /// </summary>
        private Vector2 _pressedPosition;

        public StickVectorMode stickVectorMode
        {
            get => this._stickVectorMode;
            set => this._stickVectorMode = value;
        }

        public StickType stickType
        {
            get => this._stickType;
            set => this._stickType = value;
        }

        public AxisConstraint axisConstraint
        {
            get => this._axisConstraint;
            set => this._axisConstraint = value;
        }

        public float handleMovementRange
        {
            get => this._handleMovementRange;
            set => this._handleMovementRange = value;
        }

        public float deadZone
        {
            get => this._deadZone;
            set => this._deadZone = value;
        }

        private void Awake()
        {
            this._canvas = this.GetComponentInParent<Canvas>();

            if (this._canvas == null)
            {
                Debug.LogError("[VirtualJoystick] Missing Canvas in parent hierarchy!", this);
                this.enabled = false;
                return;
            }

            if (this._showOnlyWhenPressed)
                this._background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!this._background.gameObject.activeSelf)
                this._background.gameObject.SetActive(true);

            var pressedPosition = eventData.position;

            if (this._stickType != StickType.Fixed)
            {
                this._background.localPosition = this._ScreenToAnchoredPosition(pressedPosition);
            }

            this._pressedPosition = pressedPosition;

            this.OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this._handle.anchoredPosition = Vector2.zero;

            if (this._showOnlyWhenPressed)
                this._background.gameObject.SetActive(false);

            // 離開時歸零
            this.onStickInput?.Invoke(Vector2.zero);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - this._pressedPosition;
            Vector2 input = delta / (this._handleMovementRange * this._canvas.scaleFactor);
            input *= this._GetAxisConstraintVector();

            float magnitude = input.magnitude;

            if (magnitude < this._deadZone)
            {
                input = Vector2.zero;
            }
            else
            {
                if (this._stickVectorMode == StickVectorMode.Normalized)
                {
                    if (magnitude > 1f)
                        input = input.normalized;
                }
            }

            // Update UI Handle (仍使用 clamped 範圍避免超出視覺)
            this._handle.anchoredPosition = Vector2.ClampMagnitude(input, 1f) * this._handleMovementRange;

            this.onStickInput?.Invoke(input);
        }

        private Vector2 _ScreenToAnchoredPosition(Vector2 screenPosition)
        {
            // 點擊區域
            RectTransform parent = this._background.parent as RectTransform;

            if (parent == null)
                return Vector2.zero;

            var camera = this._canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : this._canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, camera, out var localPoint))
            {
                return localPoint;
            }

            return Vector2.zero;
        }

        private Vector2 _GetAxisConstraintVector()
        {
            if (this._axisConstraint == AxisConstraint.Horizontal)
                return Vector2.right;
            else if (this._axisConstraint == AxisConstraint.Vertical)
                return Vector2.up;
            return Vector2.one;
        }
    }
}