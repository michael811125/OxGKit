using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OxGKit.Utilities.CursorAnim
{
    [DisallowMultipleComponent]
    public class CursorManager : MonoBehaviour
    {
        [Serializable]
        public class CursorState
        {
            /// <summary>
            /// 定義靜態或動態游標類型
            /// </summary>
            public enum RenderType
            {
                Static,
                Dynamic
            }

            /// <summary>
            /// 定義動畫的播放模式
            /// </summary>
            public enum PlayMode
            {
                Normal,
                Reverse,
                PingPong,
                PingPongReverse
            }

            [Header("Cursor State")]
            public string stateName;

            [Header("Cursor Options")]
            public RenderType renderType = RenderType.Static;                     // 設定靜態或動態游標
            public CursorMode cursorMode = CursorMode.Auto;                       // Cursor 渲染模式
            [Tooltip("Cursor scale is only supported in ForceSoftware rendering mode.")]
            public bool scalingEnabled = false;
            [Tooltip("You need to enable scalingEnabled for the scale adjustment to take effect.")]
            public Vector2 scale = new Vector2(1f, 1f);                           // 設置縮放比例
            public Vector2 hotspot = Vector2.zero;                                // 設定游標的熱點位置

            [Header("Static Cursor")]
            public Texture2D staticCursorTexture;                                 // 靜態游標貼圖

            [Header("Dynamic Cursor")]
            public List<Texture2D> dynamicCursorTextures = new List<Texture2D>(); // 動態游標序列幀
            public bool isLoop = false;                                           // 是否循環
            public PlayMode playMode = PlayMode.Normal;                           // 播放模式
            public int frameRate = 0;                                             // 播放速率

            private float _dt = 0;
            private int _frameIndex = 0;

            private bool _pingPongStart = false;
            private int _pingPongCount = 0;

            private Texture2D _scaledTexture;                                     // 儲存已縮放的貼圖

            #region Public Methods
            public void DriveUpdate(float dt)
            {
                switch (this.renderType)
                {
                    case RenderType.Dynamic:
                        if (this.dynamicCursorTextures.Count > 0)
                            // 更新序列動畫
                            this._UpdateTextureAnimation(dt);
                        break;
                }
            }

            /// <summary>
            /// 取得狀態名稱
            /// </summary>
            /// <returns></returns>
            public string GetStateName()
            {
                return this.stateName;
            }

            /// <summary>
            /// 設定狀態名稱
            /// </summary>
            /// <param name="stateName"></param>
            public void SetStateName(string stateName)
            {
                this.stateName = stateName;
            }

            /// <summary>
            /// 設定 cursor mode
            /// </summary>
            /// <param name="cursorMode"></param>
            public void SetCursorMode(CursorMode cursorMode)
            {
                this.cursorMode = cursorMode;
            }

            /// <summary>
            /// 設定 cursor scale
            /// </summary>
            /// <param name="scale"></param>
            public void SetCursorScale(Vector2 scale)
            {
                this.scale = scale;
            }

            /// <summary>
            /// 設定 render type
            /// </summary>
            /// <param name="renderType"></param>
            public void SetRenderType(RenderType renderType)
            {
                this.renderType = renderType;
            }

            /// <summary>
            /// 設定靜態游標
            /// </summary>
            public void SetStaticCursor(Texture2D t2d = null)
            {
                if (t2d != null)
                    this.staticCursorTexture = t2d;

                if (this.staticCursorTexture != null)
                {
                    switch (this.cursorMode)
                    {
                        case CursorMode.Auto:
                            Cursor.SetCursor(this.staticCursorTexture, this.hotspot, this.cursorMode);
                            break;
                        case CursorMode.ForceSoftware:
                            if (this.scalingEnabled)
                                t2d = this._ScaleTexture(this.staticCursorTexture, this.scale);
                            else
                                t2d = this.staticCursorTexture;
                            Cursor.SetCursor(t2d, this.hotspot, this.cursorMode);
                            break;
                    }
                }
            }

            /// <summary>
            /// 設定動態游標
            /// </summary>
            /// <param name="t2d"></param>
            public void SetDynamicCursor(Texture2D[] t2ds)
            {
                if (t2ds != null)
                    this.dynamicCursorTextures = t2ds.ToList();

                this._ResetAnim();
            }

            /// <summary>
            /// 設定 cursor offset
            /// </summary>
            /// <param name="offset"></param>
            public void SetCursorOffset(Vector2 offset)
            {
                this.hotspot = offset;
            }

            /// <summary>
            /// 設定 frame rate
            /// </summary>
            /// <param name="frameRate"></param>
            public void SetFrameRate(int frameRate)
            {
                this.frameRate = frameRate;
            }

            /// <summary>
            /// 設定 loop 開關
            /// </summary>
            /// <param name="isLoop"></param>
            public void SetLoop(bool isLoop)
            {
                this.isLoop = isLoop;
            }

            /// <summary>
            /// 重置 cursor 渲染
            /// </summary>
            public void ResetRender()
            {
                switch (this.renderType)
                {
                    case RenderType.Static:
                        // 設定靜態游標
                        this.SetStaticCursor();
                        break;

                    case RenderType.Dynamic:
                        // 開始播放動態游標動畫
                        this._ResetAnim();
                        break;
                }
            }
            #endregion

            /// <summary>
            /// 根據 scale Vector2 進行縮放
            /// </summary>
            /// <param name="source"></param>
            /// <param name="scale"></param>
            /// <returns></returns>
            private Texture2D _ScaleTexture(Texture2D source, Vector2 scale)
            {
                int targetWidth = Mathf.Max(1, Mathf.RoundToInt(source.width * scale.x));
                int targetHeight = Mathf.Max(1, Mathf.RoundToInt(source.height * scale.y));

                // 檢查是否需要創建新的 Texture2D
                if (this._scaledTexture == null ||
                    this._scaledTexture.width != targetWidth ||
                    this._scaledTexture.height != targetHeight)
                {
                    // 銷毀之前的 Texture2D (如果存在)
                    if (this._scaledTexture != null)
                    {
                        Destroy(this._scaledTexture);
                    }

                    // 創建或重新分配 Texture2D
                    this._scaledTexture = new Texture2D(targetWidth, targetHeight, source.format, false);
                }

                // 創建 RenderTexture 來處理縮放
                RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
                RenderTexture.active = rt;

                // 渲染 source 到 rt 並縮放
                Graphics.Blit(source, rt);

                // 將縮放結果讀取到 scaledTexture
                this._scaledTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
                this._scaledTexture.Apply();

                // 清理 RenderTexture
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rt);

                return this._scaledTexture;
            }

            /// <summary>
            /// 重置動畫參數
            /// </summary>
            private void _ResetAnim()
            {
                this._dt = 0;
                this._pingPongCount = 0;
                if (this.playMode == PlayMode.PingPong) this._pingPongStart = true;
                else if (playMode == PlayMode.PingPongReverse) this._pingPongStart = false;
            }

            /// <summary>
            /// 設定動態游標
            /// </summary>
            /// <param name="t2d"></param>
            private void _SetDynamicCursor(Texture2D t2d)
            {
                if (t2d != null)
                {
                    switch (this.cursorMode)
                    {
                        case CursorMode.Auto:
                            Cursor.SetCursor(t2d, this.hotspot, this.cursorMode);
                            break;
                        case CursorMode.ForceSoftware:
                            if (this.scalingEnabled)
                                t2d = this._ScaleTexture(t2d, this.scale);
                            Cursor.SetCursor(t2d, this.hotspot, this.cursorMode);
                            break;
                    }
                }
            }

            private void _UpdateTextureAnimation(float dt)
            {
                float fps = this.frameRate;
                this._dt += dt;

                this._frameIndex = Mathf.FloorToInt(this._dt * fps);

                switch (this.playMode)
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

            private void _ModeNormal()
            {
                if (!this.isLoop && this._frameIndex >= this.dynamicCursorTextures.Count) return;

                this._frameIndex %= this.dynamicCursorTextures.Count;
                this._SetDynamicCursor(this.dynamicCursorTextures[this._frameIndex]);
            }

            private void _ModeReverse()
            {
                if (!this.isLoop && this._frameIndex >= this.dynamicCursorTextures.Count) return;

                this._frameIndex %= this.dynamicCursorTextures.Count;
                int lastFrame = this.dynamicCursorTextures.Count - 1;
                int reverseIndex = lastFrame - this._frameIndex;
                this._SetDynamicCursor(this.dynamicCursorTextures[reverseIndex]);
            }

            private void _ModePingPong()
            {
                if (this._pingPongStart)
                {
                    if (!this.isLoop && this._pingPongCount >= 2) return;

                    if (this._frameIndex >= (this.dynamicCursorTextures.Count - 1))
                    {
                        if (!this.isLoop) this._pingPongCount++;
                        this._pingPongStart = false;
                        this._dt = 0;
                    }

                    this._frameIndex %= this.dynamicCursorTextures.Count;
                    this._SetDynamicCursor(this.dynamicCursorTextures[this._frameIndex]);
                }
                else
                {
                    if (this._frameIndex >= (this.dynamicCursorTextures.Count - 1))
                    {
                        if (!this.isLoop) this._pingPongCount++;
                        this._pingPongStart = true;
                        this._dt = 0;
                    }

                    this._frameIndex %= this.dynamicCursorTextures.Count;
                    int lastFrame = this.dynamicCursorTextures.Count - 1;
                    int reverseIndex = lastFrame - this._frameIndex;
                    this._SetDynamicCursor(this.dynamicCursorTextures[reverseIndex]);
                }
            }

            private void _ModePingPongReverse()
            {
                if (this._pingPongStart)
                {
                    if (this._frameIndex >= (this.dynamicCursorTextures.Count - 1))
                    {
                        if (!this.isLoop) this._pingPongCount++;
                        this._pingPongStart = false;
                        this._dt = 0;
                    }

                    this._frameIndex %= this.dynamicCursorTextures.Count;
                    this._SetDynamicCursor(this.dynamicCursorTextures[this._frameIndex]);
                }
                else
                {
                    if (!this.isLoop && this._pingPongCount >= 2) return;

                    if (this._frameIndex >= (this.dynamicCursorTextures.Count - 1))
                    {
                        if (!this.isLoop) this._pingPongCount++;
                        this._pingPongStart = true;
                        this._dt = 0;
                    }

                    this._frameIndex %= this.dynamicCursorTextures.Count;
                    int lastFrame = this.dynamicCursorTextures.Count - 1;
                    int reverseIndex = lastFrame - this._frameIndex;
                    this._SetDynamicCursor(this.dynamicCursorTextures[reverseIndex]);
                }
            }
        }

        [SerializeField]
        private bool _ignoreTimeScale = true;

        [SerializeField]
        private List<CursorState> _listCursorStates = new List<CursorState>()
        {
            new CursorState()
            {
                stateName = "Default",
                scale = new Vector2(1f, 1f),
                frameRate = 30
            }
        };

        private CursorState _currentCursorState = null;

        private static readonly object _locker = new object();
        private static CursorManager _instance;

        internal static CursorManager GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    _instance = FindObjectOfType(typeof(CursorManager)) as CursorManager;
                    if (_instance == null) _instance = new GameObject(typeof(CursorManager).Name).AddComponent<CursorManager>();
                }
            }
            return _instance;
        }

        private void Awake()
        {
            string newName = $"[{nameof(CursorManager)}]";
            this.gameObject.name = newName;
            if (this.gameObject.transform.root.name == newName)
            {
                var container = GameObject.Find(nameof(OxGKit));
                if (container == null) container = new GameObject(nameof(OxGKit));
                this.gameObject.transform.SetParent(container.transform);
                DontDestroyOnLoad(container);
            }
            else DontDestroyOnLoad(this.gameObject.transform.root);

            // Init cursor set
            this._Initialize();
        }

        private void Start()
        {
            this.ResetRender();
        }

        private void Update()
        {
            this.GetCurrentCursorState()?.DriveUpdate(this._ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        private void OnDisable()
        {
            this.RemoveCursorRender();
        }

        private void OnEnable()
        {
            this.ResetCursorState();
        }

        private void _Initialize()
        {
            this._currentCursorState = this._listCursorStates.FirstOrDefault();
        }

        /// <summary>
        /// 設定 ignore scale
        /// </summary>
        /// <param name="ignore"></param>
        public void SetIgnoreScale(bool ignore)
        {
            this._ignoreTimeScale = ignore;
        }

        /// <summary>
        /// 取得 cursor lock state
        /// </summary>
        /// <returns></returns>
        public CursorLockMode GetCurrentCursorLockState()
        {
            return Cursor.lockState;
        }

        /// <summary>
        /// 設定 cursor lock state
        /// </summary>
        /// <param name="cursorLockMode"></param>
        public void SetCursorLockState(CursorLockMode cursorLockMode)
        {
            Cursor.lockState = cursorLockMode;
        }

        /// <summary>
        /// 檢查返回 cursor visible
        /// </summary>
        /// <returns></returns>
        public bool IsCursorVisible()
        {
            return Cursor.visible;
        }

        /// <summary>
        /// 設定 cursor visible
        /// </summary>
        /// <param name="visible"></param>
        public void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
        }

        /// <summary>
        /// 設定 scale 到所有的 cursor state, 並且重置渲染
        /// </summary>
        /// <param name="scale"></param>
        public void SetScaleToAllCursors(Vector2 scale)
        {
            foreach (var cursorState in this.GetAllCursorStates())
            {
                cursorState.SetCursorScale(scale);
                cursorState.ResetRender();
            }
        }

        /// <summary>
        /// 取得所有的 cursor states
        /// </summary>
        /// <returns></returns>
        public CursorState[] GetAllCursorStates()
        {
            return this._listCursorStates.ToArray();
        }

        /// <summary>
        /// 取得對應的 Cursor 狀態
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public CursorState GetCursorState(string stateName)
        {
            if (this._listCursorStates.Count > 0)
            {
                foreach (var cursorState in this._listCursorStates)
                {
                    if (cursorState.GetStateName() == stateName)
                        return cursorState;
                }
            }
            return null;
        }

        /// <summary>
        /// 取得當前 Cursor 狀態
        /// </summary>
        /// <returns></returns>
        public CursorState GetCurrentCursorState()
        {
            return this._currentCursorState;
        }

        /// <summary>
        /// 設定切換 Cursor 狀態
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool SetCursorState(string stateName)
        {
            var cursorState = this.GetCursorState(stateName);
            if (cursorState != null)
            {
                this._currentCursorState = cursorState;
                this._currentCursorState.ResetRender();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重置 Cursor 狀態為 Default (first-element)
        /// </summary>
        public void ResetCursorState()
        {
            this._Initialize();
            this.ResetRender();
        }

        /// <summary>
        /// 重置渲染
        /// </summary>
        public void ResetRender()
        {
            this.GetCurrentCursorState()?.ResetRender();
        }

        /// <summary>
        /// 移除 Cursor 渲染
        /// <para>If you want to restore visibility, you can use ResetCursorState method to initialize and reset the rendering.</para>
        /// </summary>
        public void RemoveCursorRender()
        {
            this._currentCursorState = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}