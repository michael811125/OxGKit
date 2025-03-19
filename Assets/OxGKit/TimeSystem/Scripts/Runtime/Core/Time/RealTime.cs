using OxGKit.LoggingSystem;
using System;

namespace OxGKit.TimeSystem
{
    public static class RealTime
    {
        private static bool _firstInitStartupTime = false;            // 是否首次初始啟動時間                            
        public static DateTime timeSinceStartup { get; private set; } // 啟動時間, 建議由主程序調用初始 (將會是遊戲啟動時間)

        /// <summary>
        /// Call by Main MonoBehaviour Awake
        /// </summary>
        /// <returns></returns>
        public static void InitStartupTime()
        {
            if (_firstInitStartupTime) return;

            timeSinceStartup = DateTime.Now;

            _firstInitStartupTime = true;
        }

        public static bool IsInitStartupTime()
        {
            if (!_firstInitStartupTime)
            {
                Logging.Print<Logger>("<color=#FF0000>Please call <color=#FFB800>RealTime.InitStartupTime()</color> in main program first</color>");
                return false;
            }

            return true;
        }
    }
}