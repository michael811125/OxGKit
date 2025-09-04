using System;
using System.Collections.Generic;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    public abstract class Logging : ILogging
    {
        /// <summary>
        /// 全域開關
        /// </summary>
        internal static bool logMainActive = true;

        /// <summary>
        /// 全域級別
        /// </summary>
        internal static LogLevel logMainLevel = LogLevel.All;

        /// <summary>
        /// 日誌器緩存
        /// </summary>
        private static readonly Dictionary<string, Logging> _cacheLoggers = new Dictionary<string, Logging>();

        #region Internal Methods
        internal static string GetLoggerName<TLogging>() where TLogging : Logging
        {
            var type = typeof(TLogging);

            string typeName = type.Name;
            string loggerName = typeName;

            var attr = AssemblyFinder.GetAttribute<LoggerNameAttribute>(type);
            if (attr != null)
                loggerName = attr.loggerName;

            return loggerName;
        }

        internal static bool HasLogger(string loggerName)
        {
            return _cacheLoggers.ContainsKey(loggerName);
        }

        internal static Dictionary<string, Logging> GetLoggers()
        {
            return _cacheLoggers;
        }

        internal static Logging GetLogger(string loggerName)
        {
            _cacheLoggers.TryGetValue(loggerName, out var logger);
            return logger;
        }

        internal static void ResetLoggers()
        {
            foreach (var logger in _cacheLoggers.Values)
            {
                logger.logActive = false;
                logger.logLevel = LogLevel.All;
            }
        }

        internal static bool CheckHasAnyLoggers()
        {
            if (_cacheLoggers.Count == 0)
                return false;
            return true;
        }

        internal static void ClearLoggers()
        {
            _cacheLoggers.Clear();
        }

        internal static void InitLoggers()
        {
            _cacheLoggers.Clear();

#if UNITY_EDITOR || OXGKIT_LOGGER_ON
            var types = AssemblyFinder.GetAssignableTypes(typeof(Logging));
            foreach (var type in types)
            {
                string typeName = type.Name;
                string key = typeName;
                bool isOverride = false;
                var attr = AssemblyFinder.GetAttribute<LoggerNameAttribute>(type);
                if (attr != null)
                {
                    key = attr.loggerName;
                    isOverride = attr.isOverride;
                }

                if (typeName == nameof(Logging))
                    continue;

                if (!_cacheLoggers.ContainsKey(key))
                {
                    try
                    {
                        var instance = Activator.CreateInstance(type, null) as Logging;
                        _cacheLoggers.Add(key, instance);
                    }
                    catch
                    {
                        Debug.LogWarning($"Create a logger: {typeName} instance error!!!");
                    }
                }
                else
                {
                    try
                    {
                        if (isOverride)
                        {
                            var instance = Activator.CreateInstance(type, null) as Logging;
                            _cacheLoggers[key] = instance;
                        }
                    }
                    catch
                    {
                        Debug.LogWarning($"Create a logger: {typeName} instance error!!!");
                    }
                }
            }
#else
            Debug.LogWarning($"Not enabled {nameof(LoggingSystem)} by symbol [OXGKIT_LOGGER_ON].");
#endif
        }

        internal static void CreateLogger<TLogging>() where TLogging : Logging, new()
        {
#if UNITY_EDITOR || OXGKIT_LOGGER_ON
            var type = typeof(TLogging);
            string typeName = type.Name;
            string key = typeName;
            bool isOverride = false;
            var attr = AssemblyFinder.GetAttribute<LoggerNameAttribute>(type);
            if (attr != null)
            {
                key = attr.loggerName;
                isOverride = attr.isOverride;
            }

            if (typeName == nameof(Logging))
                return;

            if (!_cacheLoggers.ContainsKey(key))
            {
                try
                {
                    var instance = new TLogging();
                    _cacheLoggers.Add(key, instance);
                }
                catch
                {
                    Debug.LogWarning($"Create a logger: {typeName} instance error!!!");
                }
            }
            else
            {
                try
                {
                    if (isOverride)
                    {
                        var instance = new TLogging();
                        _cacheLoggers[key] = instance;
                    }
                }
                catch
                {
                    Debug.LogWarning($"Create a logger: {typeName} instance error!!!");
                }
            }
#else
            Debug.LogWarning($"Not enabled {nameof(LoggingSystem)} by symbol [OXGKIT_LOGGER_ON].");
#endif
        }
        #endregion

        #region Global Methods
        public static void Print<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers())
                return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                if (context == null)
                    _cacheLoggers[key].Print(message);
                else
                    _cacheLoggers[key].Print(message, context);
            }
        }

        public static void PrintWarning<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers())
                return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                if (context == null)
                    _cacheLoggers[key].PrintWarning(message);
                else
                    _cacheLoggers[key].PrintWarning(message, context);
            }
        }

        public static void PrintError<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers())
                return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                if (context == null)
                    _cacheLoggers[key].PrintError(message);
                else
                    _cacheLoggers[key].PrintError(message, context);
            }
        }

        public static void PrintException<TLogging>(Exception exception, UnityEngine.Object context = null) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers())
                return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                if (context == null)
                    _cacheLoggers[key].PrintException(exception);
                else
                    _cacheLoggers[key].PrintException(exception, context);
            }
        }
        #endregion

        /// <summary>
        /// 日誌器激活標記
        /// </summary>
        internal bool logActive = false;

        /// <summary>
        /// 日誌器等級狀態
        /// </summary>
        internal LogLevel logLevel = LogLevel.All;

        /// <summary>
        /// 檢查日誌器激活狀態
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool CheckLogActive(LogLevel logLevel)
        {
            // 如果全域級別關閉, 則禁止所有日誌輸出
            if (!logMainActive ||
                logMainLevel == LogLevel.Off)
                return false;

            // 如果全域級別允許該級別, 則允許記錄
            // 但是需要考慮全域級別的限制, 不能打印高於全域級別的日誌
            if (logMainLevel.HasFlag(logLevel))
            {
                switch (logMainLevel)
                {
                    // 全域優先
                    case LogLevel.Log:
                        return this.logActive && this.logLevel.HasFlag(LogLevel.Log);
                    case LogLevel.LogWarning:
                        return this.logActive && this.logLevel.HasFlag(LogLevel.LogWarning);
                    case LogLevel.LogError:
                        return this.logActive && this.logLevel.HasFlag(LogLevel.LogError);
                    case LogLevel.LogException:
                        return this.logActive && this.logLevel.HasFlag(LogLevel.LogException);

                    // 檢查個別級別
                    default:
                        return this.logActive && this.logLevel.HasFlag(logLevel);
                }
            }

            return false;
        }

        #region Internal Print Methods
        internal void Print(object message)
        {
            if (!this.CheckLogActive(LogLevel.Log))
                return;

            this.Log(message);
        }

        internal void PrintWarning(object message)
        {
            if (!this.CheckLogActive(LogLevel.LogWarning))
                return;

            this.LogWarning(message);
        }

        internal void PrintError(object message)
        {
            if (!this.CheckLogActive(LogLevel.LogError))
                return;

            this.LogError(message);
        }

        internal void PrintException(Exception exception)
        {
            if (!this.CheckLogActive(LogLevel.LogException))
                return;

            this.LogException(exception);
        }

        internal void Print(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.Log))
                return;

            this.Log(message, context);
        }

        internal void PrintWarning(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogWarning))
                return;

            this.LogWarning(message, context);
        }

        internal void PrintError(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogError))
                return;

            this.LogError(message, context);
        }

        internal void PrintException(Exception exception, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogException))
                return;

            this.LogException(exception, context);
        }
        #endregion

        #region Interface
        public virtual void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public virtual void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public virtual void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public virtual void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        public virtual void Log(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        public virtual void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        public virtual void LogError(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        public virtual void LogException(Exception exception, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(exception, context);
        }
        #endregion
    }
}
