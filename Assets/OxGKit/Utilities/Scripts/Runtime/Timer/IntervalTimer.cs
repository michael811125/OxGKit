using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace OxGKit.Utilities.Timer
{
    public class IntervalTimer
    {
        private bool _isRuning = false;
        private CancellationTokenSource _cts = null;

        public IntervalTimer()
        {
        }

        ~IntervalTimer()
        {
            this.StopInterval();
        }

        private async UniTaskVoid _SetInterval(bool switchToThread, Action action, int milliseconds, bool ignoreTimeScale, CancellationTokenSource cts)
        {
            try
            {
#if !UNITY_WEBGL
                if (switchToThread) await UniTask.SwitchToThreadPool();
#endif
                do
                {
                    await UniTask.Delay(milliseconds, ignoreTimeScale, PlayerLoopTiming.Update, (cts == null) ? default : cts.Token);
                    action?.Invoke();
                } while (this._isRuning);
            }
            catch (Exception ex)
            {
                this.StopInterval();
                throw ex;
            }
        }

        public void SetInterval(Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (this._isRuning) return;
            this._isRuning = true;
            if (this._cts == null) this._cts = new CancellationTokenSource();
            this._SetInterval(false, action, milliseconds, ignoreTimeScale, this._cts).Forget();
        }

        public void SetIntervalOnThread(Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (this._isRuning) return;
            this._isRuning = true;
            if (this._cts == null) this._cts = new CancellationTokenSource();
            this._SetInterval(true, action, milliseconds, ignoreTimeScale, this._cts).Forget();
        }

        public void StopInterval()
        {
            this._isRuning = false;
            if (this._cts == null) return;
            this._cts.Cancel();
            this._cts.Dispose();
            this._cts = null;
        }

        public bool IsRunning()
        {
            return this._isRuning;
        }
    }
}