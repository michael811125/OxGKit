using System;
using System.Collections.Generic;

namespace OxGKit.Utilities.Timer
{
    public static class IntervalSetter
    {
        private const int _MAX_COUNT = 1 << 15;
        private static Dictionary<object, IntervalTimer> _intervalTimers = new Dictionary<object, IntervalTimer>(_MAX_COUNT);

        #region SetInterval
        public static void SetInterval(int id, Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _MAX_COUNT)
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

        public static void SetIntervalOnThread(int id, Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _MAX_COUNT)
            {
                throw new InvalidOperationException("Interval timer capacity limit exceeded.");
            }

            if (!_intervalTimers.ContainsKey(id))
            {
                IntervalTimer intervalTimer = new IntervalTimer();
                _intervalTimers.Add(id, intervalTimer);
                intervalTimer.SetIntervalOnThread(action, milliseconds, ignoreTimeScale);
            }
        }

        public static void SetInterval(string id, Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _MAX_COUNT)
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

        public static void SetIntervalOnThread(string id, Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _MAX_COUNT)
            {
                throw new InvalidOperationException("Interval timer capacity limit exceeded.");
            }

            if (!_intervalTimers.ContainsKey(id))
            {
                IntervalTimer intervalTimer = new IntervalTimer();
                _intervalTimers.Add(id, intervalTimer);
                intervalTimer.SetIntervalOnThread(action, milliseconds, ignoreTimeScale);
            }
        }

        public static IntervalTimer SetInterval(Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _MAX_COUNT)
            {
                throw new InvalidOperationException("Interval timer capacity limit exceeded.");
            }

            var intervalTimer = new IntervalTimer();
            var id = intervalTimer.GetHashCode();
            if (!_intervalTimers.ContainsKey(id))
            {
                _intervalTimers.Add(id, intervalTimer);
                intervalTimer.SetInterval(action, milliseconds, ignoreTimeScale);
            }
            return intervalTimer;
        }

        public static IntervalTimer SetIntervalOnThread(Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            if (_intervalTimers.Count >= _MAX_COUNT)
            {
                throw new InvalidOperationException("Interval timer capacity limit exceeded.");
            }

            var intervalTimer = new IntervalTimer();
            var id = intervalTimer.GetHashCode();
            if (!_intervalTimers.ContainsKey(id))
            {
                _intervalTimers.Add(id, intervalTimer);
                intervalTimer.SetIntervalOnThread(action, milliseconds, ignoreTimeScale);
            }
            return intervalTimer;
        }

        public static bool CheckIsRunning(int id)
        {
            if (_intervalTimers.ContainsKey(id))
                return _intervalTimers[id].IsRunning();
            return false;
        }

        public static bool TryClearInterval(int id)
        {
            if (_intervalTimers.ContainsKey(id))
            {
                _intervalTimers[id].StopInterval();
                _intervalTimers[id] = null;
                _intervalTimers.Remove(id);
                return true;
            }
            return false;
        }

        public static bool TryClearInterval(string id)
        {
            if (_intervalTimers.ContainsKey(id))
            {
                _intervalTimers[id].StopInterval();
                _intervalTimers[id] = null;
                _intervalTimers.Remove(id);
                return true;
            }
            return false;
        }

        public static bool TryClearInterval(IntervalTimer intervalTimer)
        {
            if (intervalTimer != null)
            {
                var id = intervalTimer.GetHashCode();
                bool isCleared = TryClearInterval(id);
                if (!isCleared)
                {
                    intervalTimer.StopInterval();
                    isCleared = true;
                }
                return isCleared;
            }
            return false;
        }

        public static void ClearAllIntervalTimers()
        {
            foreach (var intervalTimer in _intervalTimers.Values)
            {
                intervalTimer.StopInterval();
            }
            _intervalTimers.Clear();
        }
        #endregion
    }
}
