using System;

namespace OxGKit.Utilities.Timer
{
    public class RealTimer
    {
        private DateTime _createTime;
        private bool _playing;
        private float _intervalTime;
        private float _pauseTime;

        private float _timerTime;
        private float _triggerTime;

        private float _tickTime;
        private float _lastTickTime;

        private float _mark;
        private float _timeSpeed;

        public RealTimer()
        {
            this._createTime = DateTime.Now;
            this.Reset();
        }

        public RealTimer(bool autoPlay) : this()
        {
            if (autoPlay) this.Play();
        }

        ~RealTimer()
        {
        }

        public void Reset()
        {
            this._playing = false;
            this._intervalTime = 0.0f;
            this._pauseTime = 0.0f;
            this._timerTime = 0.0f;
            this._triggerTime = 0.0f;
            this._tickTime = 0.0f;
            this._lastTickTime = 0.0f;
            this._mark = 0.0f;
            this._timeSpeed = 1.0f;
        }

        public float GetRealTime()
        {
            var timeSpan = DateTime.Now.Subtract(this._createTime);
            return (float)timeSpan.TotalSeconds;
        }

        public float GetTime()
        {
            if (!this._playing) return this._pauseTime - this._intervalTime;
            return (this.GetRealTime() - this._intervalTime) * this._timeSpeed;
        }

        public void Pause()
        {
            if (!this._playing) return;
            this._pauseTime = this.GetRealTime();
            this._playing = false;
        }

        public void Stop()
        {
            this._playing = false;
            this._intervalTime = 0.0f;
            this._pauseTime = 0.0f;
            this._timerTime = 0.0f;
            this._triggerTime = 0.0f;
            this._tickTime = 0.0f;
            this._lastTickTime = 0.0f;
            this._mark = 0.0f;
        }

        public void Play()
        {
            if (this._playing) return;
            this._intervalTime += this.GetRealTime() - this._pauseTime;
            this._playing = true;
        }

        public bool IsPause()
        {
            return !this._playing;
        }

        public bool IsPlaying()
        {
            return this._playing;
        }

        #region Timer, 依照設置的時間下去計時
        /// <summary>
        /// 設置要計時的秒數
        /// </summary>
        /// <param name="timeSeconds"></param>
        public void SetTimer(float timeSeconds)
        {
            this._timerTime = timeSeconds;
            this._triggerTime = this.GetTime() + this._timerTime;
        }

        /// <summary>
        /// 計算觸發時間倒數計時, 如果超過設置的觸發時間將直接返回 0
        /// </summary>
        /// <returns></returns>
        public float TimerCountdown()
        {
            float time = this.GetTime();
            if (time >= this._triggerTime) return 0.0f;
            return this._triggerTime - time;
        }

        /// <summary>
        /// 返回計時時間是否已經到了
        /// </summary>
        /// <returns></returns>
        public bool IsTimerTimeout()
        {
            if (this.GetTime() > this._triggerTime) return true;
            return false;
        }

        /// <summary>
        /// 取得 Timer 倒數計時的時間比率 1 遞減至 0, 0 = 時間到
        /// </summary>
        /// <returns></returns>
        public float GetTimerCountdownRatio()
        {
            float countdown = this.TimerCountdown();
            if (countdown <= 0.0f) return 0.0f;
            float ratio = countdown / this._timerTime;
            return ratio;
        }
        #endregion

        #region Tick, 持續依照 Set 的時間 Tick
        /// <summary>
        /// 設置 Tick 時間, 當 TickTimeout 時還會持續循環 Tick
        /// </summary>
        /// <param name="tickSeconds"></param>
        public void SetTick(float tickSeconds)
        {
            this._tickTime = tickSeconds;
            this._lastTickTime = this.GetTime() + this._tickTime;
        }

        /// <summary>
        /// 取得設置的 Tick 的時間
        /// </summary>
        /// <returns></returns>
        public float GetTick()
        {
            return this._tickTime;
        }

        /// <summary>
        /// Tick 觸發時間倒數, 如果超過設置的觸發時間將直接返回 0
        /// </summary>
        /// <returns></returns>
        public float TickCountdown()
        {
            float time = this.GetTime();
            if (time >= this._lastTickTime) return 0.0f;
            return this._lastTickTime - time;
        }

        /// <summary>
        /// 返回 Tick 時間是否已經到了
        /// </summary>
        /// <returns></returns>
        public bool IsTickTimeout()
        {
            float time = this.GetTime();
            if (time > this._lastTickTime)
            {
                this._lastTickTime = time + this._tickTime;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 取得 Tick 倒數計時的時間比率 1 遞減至 0, 0 = 時間到
        /// </summary>
        /// <returns></returns>
        public float GetTickCountdownRatio()
        {
            float countdown = this.TickCountdown();
            if (countdown <= 0.0f) return 0.0f;
            float ratio = countdown / this._tickTime;
            return ratio;
        }
        #endregion

        #region Mark, 標記時間
        /// <summary>
        /// 設置標記時間
        /// </summary>
        public void SetMark()
        {
            this._mark = this.GetTime();
        }

        /// <summary>
        /// 取得標記時間
        /// </summary>
        /// <returns></returns>
        public float GetMark()
        {
            return this._mark;
        }

        /// <summary>
        /// 取得上次標記時的經過時間
        /// </summary>
        /// <returns></returns>
        public float GetElapsedMarkTime()
        {
            float time = this.GetTime();
            if (time == this._mark || time < this._mark) return 0.0f;
            return time - this._mark;
        }
        #endregion

        /// <summary>
        /// 設置時間運轉速度
        /// </summary>
        /// <param name="timeSpeed"></param>
        public void SetTimeSpeed(float timeSpeed)
        {
            this._timeSpeed = timeSpeed;
        }

        public float GetTimeSpeed()
        {
            return this._timeSpeed;
        }
    }
}