using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace OxGKit.Utilities.Timer
{
    public class RTUpdater
    {
        public delegate void RealTimeUpdate(float deltaTime);
        public delegate void RealTimeFixedUpdate(float fixedDeltaTime);

        public RealTimeUpdate onUpdate = null;
        public RealTimeFixedUpdate onFixedUpdate = null;

        private DateTime _createTime;
        public float timeSinceStartup { get; protected set; }     // 自啟動以來的時間
        public float timeAtLastFrame { get; protected set; }      // 記錄最後一幀的時間
        protected float _timeScale = 1f;                          // 時間尺度, 預設 = 1
        public float timeScale
        {
            get { return this._timeScale; }
            set
            {
                if (value >= MAX_TIMESCALE) this._timeScale = MAX_TIMESCALE;
                else if (value < 0f) this._timeScale = 0f;
                else this._timeScale = value;
            }
        }
        protected float _targetFrameRate = FIXED_FRAME;
        public float targetFrameRate
        {
            get { return this._targetFrameRate; }
            set
            {
                if (value < 0f) this._targetFrameRate = 0f;
                else this._targetFrameRate = value;
            }
        }
        public float deltaTime { get; protected set; }
        public float fixedDeltaTime { get; protected set; }
        protected CancellationTokenSource _cts = null;

        protected const float FIXED_FRAME = 60;                   // 固定幀數 (固定 1 秒刷新 60 次, 毫秒單位 => 1000 ms / 60 = 16 ms, 秒數單位 => 1 s / 60 = 0.016 s)
        protected const float MAX_TIMESCALE = 1 << 6;

        public RTUpdater()
        {
            this._createTime = DateTime.Now;
        }

        public void Start()
        {
            if (this._cts == null) this._cts = new CancellationTokenSource();
            SetInterval(this._cts).Forget();
        }

        public void Stop()
        {
            if (this._cts == null) return;
            this._cts.Cancel();
            this._cts.Dispose();
            this._cts = null;
        }

        protected async UniTask SetInterval(CancellationTokenSource cts)
        {
            if (this.targetFrameRate > 0 && this.timeScale > 0)
            {
                // 幀數率
                float frameRate = this.targetFrameRate * this.timeScale;
                // 計算 fixedDeltaTime
                this.fixedDeltaTime = 1f / frameRate;

                // 使用 PlayerLoopTiming.Update 確保刷新率
                await UniTask.Delay(TimeSpan.FromSeconds(this.fixedDeltaTime), true, PlayerLoopTiming.Update, (cts == null) ? default : cts.Token);

                this.onFixedUpdate?.Invoke(this.fixedDeltaTime);

                // 計算 deltaTime
                this.deltaTime = this.timeSinceStartup - this.timeAtLastFrame;
                this.timeAtLastFrame = this.timeSinceStartup;

                // 計算經過的時間, 當前時間 - 最一開始的時間 = 啟動到現在的經過時間 (秒)
                var timeSpan = DateTime.Now.Subtract(this._createTime);
                this.timeSinceStartup = (float)timeSpan.TotalSeconds;

                this.onUpdate?.Invoke(this.deltaTime);
            }
            else await UniTask.Yield(cts.Token);

            SetInterval(cts).Forget();
        }
    }
}