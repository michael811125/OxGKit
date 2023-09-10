using System;
using System.Collections.Generic;

namespace OxGKit.Utilities.Timer
{
    public static class IntervalSetter
    {
        private const int _maxCount = 1 << 10;
        private static Dictionary<int, IntervalTimer> _intervalTimers = new Dictionary<int, IntervalTimer>(_maxCount);

        #region SetInterval
        public static void SetInterval(int id, Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _maxCount)
            {
                throw new InvalidOperationException("Interval timer capacity limit exceeded.");
            }

            if (!_intervalTimers.ContainsKey(id))
            {
                IntervalTimer intervalTimer = new IntervalTimer();
                _intervalTimers.Add(id, intervalTimer);
                intervalTimer.SetInterval(action, milliseconds, ignoreTimeScale);
            }
        }

        public static void TryClearInterval(int id)
        {
            if (_intervalTimers.ContainsKey(id))
            {
                _intervalTimers[id].StopInterval();
                _intervalTimers[id] = null;
                _intervalTimers.Remove(id);
            }
        }
        #endregion
    }
}
