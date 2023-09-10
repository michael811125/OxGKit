using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace OxGKit.Utilities.Timer
{
    public class IntervalTimer
    {
        private bool _isRuning = false;
        private CancellationTokenSource _cts = null;

        private async UniTaskVoid _SetInterval(Action action, int milliseconds, bool ignoreTimeScale, CancellationTokenSource cts)
        {
            try
            {
                do
                {
                    await UniTask.Delay(milliseconds, ignoreTimeScale, PlayerLoopTiming.Update, (cts == null) ? default : cts.Token);
                    action?.Invoke();
                } while (this._isRuning);
            }
            catch { }
        }

        public void SetInterval(Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (this._isRuning) return;
            this._isRuning = true;
            if (this._cts == null) this._cts = new CancellationTokenSource();
            this._SetInterval(action, milliseconds, ignoreTimeScale, this._cts).Forget();
        }

        public void StopInterval()
        {
            this._isRuning = false;
            if (this._cts == null) return;
            this._cts.Cancel();
            this._cts.Dispose();
            this._cts = null;
        }

        ~IntervalTimer()
        {
            this._cts = null;
        }
    }
}