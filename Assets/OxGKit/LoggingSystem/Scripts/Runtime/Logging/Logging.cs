using System;
using System.Collections.Generic;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    /// <summary>
    /// 日誌器實作
    /// </summary>
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
        /// 全域顏色模式
        /// </summary>
        internal static LogColor logMainColor = LogColor.EditorOnly;

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

        internal static void DispatchLog<TLogging>(LogLevel logLevel, object message, UnityEngine.Object context) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers())
                return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                switch (logLevel)
                {
                    case LogLevel.Log:
                        {
                            if (context == null)
                                _cacheLoggers[key].Print(message);
                            else
                                _cacheLoggers[key].Print(message, context);
                        }
                        break;
                    case LogLevel.LogInfo:
                        {
                            if (context == null)
                                _cacheLoggers[key].PrintInfo(message);
                            else
                                _cacheLoggers[key].PrintInfo(message, context);
                        }
                        break;
                    case LogLevel.LogWarning:
                        {
                            if (context == null)
                                _cacheLoggers[key].PrintWarning(message);
                            else
                                _cacheLoggers[key].PrintWarning(message, context);
                        }
                        break;
                    case LogLevel.LogError:
                        {
                            if (context == null)
                                _cacheLoggers[key].PrintError(message);
                            else
                                _cacheLoggers[key].PrintError(message, context);
                        }
                        break;
                    case LogLevel.LogException:
                        {
                            if (context == null)
                                _cacheLoggers[key].PrintException((Exception)message);
                            else
                                _cacheLoggers[key].PrintException((Exception)message, context);
                        }
                        break;
                }
            }
        }

        #region Global Methods
        public static void Print<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            DispatchLog<TLogging>(LogLevel.Log, message, context);
        }

        public static void PrintInfo<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            DispatchLog<TLogging>(LogLevel.LogInfo, message, context);
        }

        public static void PrintWarning<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            DispatchLog<TLogging>(LogLevel.LogWarning, message, context);
        }

        public static void PrintError<TLogging>(object message, UnityEngine.Object context = null) where TLogging : Logging
        {
            DispatchLog<TLogging>(LogLevel.LogError, message, context);
        }

        public static void PrintException<TLogging>(Exception exception, UnityEngine.Object context = null) where TLogging : Logging
        {
            DispatchLog<TLogging>(LogLevel.LogException, exception, context);
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
        /// 日誌器顏色模式
        /// </summary>
        internal LogColor logColor = LogColor.EditorOnly;

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
                    case LogLevel.LogInfo:
                        return this.logActive && this.logLevel.HasFlag(LogLevel.LogInfo);
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

        /// <summary>
        /// 檢查是否啟用顏色
        /// </summary>
        /// <returns></returns>
        public bool ShouldColorize()
        {
            LogColor logColor;

            // 檢查全域 + 個別
            if (logMainColor == LogColor.Disabled || this.logColor == LogColor.Disabled)
                logColor = LogColor.Disabled;
            else if (logMainColor == LogColor.EditorOnly || this.logColor == LogColor.EditorOnly)
                logColor = LogColor.EditorOnly;
            else
                logColor = LogColor.Enabled;

#if UNITY_EDITOR
            // Editor: Enabled/EditorOnly 皆會包含 RichText 上色
            return logColor != LogColor.Disabled;
#else
            // Player (發布): 只有 Enabled 會包含 RichText 上色
            return logColor == LogColor.Enabled;
#endif
        }

        /// <summary>
        /// 顏色級別格式
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object ColorizeLogMessage(LogLevel logLevel, object message)
        {
            // 依照型別進行上色處理
            if (message is string msg)
            {
                if (!this.ShouldColorize())
                    return msg;

                switch (logLevel)
                {
                    case LogLevel.Log:
                        return $"<color=#00ffd8><b>[DEBUG] ► </b></color><color=#72ffe9>{msg}</color>";
                    case LogLevel.LogInfo:
                        return $"<color=#00ff73><b>[INFO] ► </b></color><color=#72ffb2>{msg}</color>";
                    case LogLevel.LogWarning:
                        return $"<color=#ffc400><b>[WARNING] ► </b></color><color=#ffdf71>{msg}</color>";
                    case LogLevel.LogError:
                        return $"<color=#ff003c><b>[ERROR] ► </b></color><color=#ff7293>{msg}</color>";
                }
            }
            else if (message is Exception ex)
            {
                if (!this.ShouldColorize())
                    return ex;

                switch (logLevel)
                {
                    case LogLevel.LogException:
                        return new Exception($"<color=#ff003c><b>[EXCEPTION] ► </b></color><color=#ff7293>{ex}</color>");
                }
            }

            return message;
        }

        #region Internal Print Methods
        internal void Print(object message)
        {
            if (!this.CheckLogActive(LogLevel.Log))
                return;

            message = this.ColorizeLogMessage(LogLevel.Log, message);

            this.Log(message);
        }

        internal void PrintInfo(object message)
        {
            if (!this.CheckLogActive(LogLevel.LogInfo))
                return;

            message = this.ColorizeLogMessage(LogLevel.LogInfo, message);

            this.LogInfo(message);
        }

        internal void PrintWarning(object message)
        {
            if (!this.CheckLogActive(LogLevel.LogWarning))
                return;

            message = this.ColorizeLogMessage(LogLevel.LogWarning, message);

            this.LogWarning(message);
        }

        internal void PrintError(object message)
        {
            if (!this.CheckLogActive(LogLevel.LogError))
                return;

            message = this.ColorizeLogMessage(LogLevel.LogError, message);

            this.LogError(message);
        }

        internal void PrintException(Exception exception)
        {
            if (!this.CheckLogActive(LogLevel.LogException))
                return;

            exception = (Exception)this.ColorizeLogMessage(LogLevel.LogException, exception);

            this.LogException(exception);
        }

        internal void Print(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.Log))
                return;

            message = this.ColorizeLogMessage(LogLevel.Log, message);

            this.Log(message, context);
        }

        internal void PrintInfo(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogInfo))
                return;

            message = this.ColorizeLogMessage(LogLevel.LogInfo, message);

            this.LogInfo(message, context);
        }

        internal void PrintWarning(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogWarning))
                return;

            message = this.ColorizeLogMessage(LogLevel.LogWarning, message);

            this.LogWarning(message, context);
        }

        internal void PrintError(object message, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogError))
                return;

            message = this.ColorizeLogMessage(LogLevel.LogError, message);

            this.LogError(message, context);
        }

        internal void PrintException(Exception exception, UnityEngine.Object context)
        {
            if (!this.CheckLogActive(LogLevel.LogException))
                return;

            exception = (Exception)this.ColorizeLogMessage(LogLevel.LogException, exception);

            this.LogException(exception, context);
        }
        #endregion

        #region Interface
        /// <summary>
        /// LOG/DEBUG (Level: Log)
        /// </summary>
        /// <param name="message"></param>
        public virtual void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// INFO (Level: LogInfo)
        /// </summary>
        /// <param name="message"></param>
        public virtual void LogInfo(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// WARNING (Level: LogWarning)
        /// </summary>
        /// <param name="message"></param>
        public virtual void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        /// <summary>
        /// ERROR (Level: LogError)
        /// </summary>
        /// <param name="message"></param>
        public virtual void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// EXCEPTION (Level: LogException)
        /// </summary>
        /// <param name="exception"></param>
        public virtual void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        /// <summary>
        /// LOG/DEBUG with Context (Level: Log)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public virtual void Log(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        /// <summary>
        /// INFO with Context (Level: LogInfo)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public virtual void LogInfo(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        /// <summary>
        /// WARNING with Context (Level: LogWarning)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public virtual void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        /// <summary>
        /// ERROR with Context (Level: LogError)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public virtual void LogError(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        /// <summary>
        /// EXCEPTION with Context (Level: LogException)
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public virtual void LogException(Exception exception, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(exception, context);
        }
        #endregion
    }
}
