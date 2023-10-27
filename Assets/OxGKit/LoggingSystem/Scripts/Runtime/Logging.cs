using System;
using System.Collections.Generic;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    public abstract class Logging : ILogging
    {
        internal static bool logMainActive = true;
        private static readonly Dictionary<string, Logging> _cacheLoggers = new Dictionary<string, Logging>();

        #region Internal Methods
        internal static string GetLoggerName(Type type)
        {
            string typeName = type.Name;
            string loggerName = typeName;

            var attr = AssemblyFinder.GetAttribute<LoggerNameAttribute>(type);
            if (attr != null) loggerName = attr.loggerName;

            return loggerName;
        }

        internal static string GetLoggerName<TLogging>() where TLogging : Logging
        {
            var type = typeof(TLogging);

            string typeName = type.Name;
            string loggerName = typeName;

            var attr = AssemblyFinder.GetAttribute<LoggerNameAttribute>(type);
            if (attr != null) loggerName = attr.loggerName;

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

        internal static bool CheckHasAnyLoggers()
        {
            if (_cacheLoggers.Count == 0) return false;
            return true;
        }
        #endregion

        #region Global Methods
        public static void ClearLoggers()
        {
            _cacheLoggers.Clear();
        }

        public static void InitLoggers()
        {
            _cacheLoggers.Clear();

#if UNITY_EDITOR || OXGKIT_LOGGER_ON
            var types = AssemblyFinder.GetAssignableTypes(typeof(Logging));
            foreach (var type in types)
            {
                string typeName = type.Name;
                string key = GetLoggerName(type);

                if (typeName == nameof(Logging)) continue;

                if (!_cacheLoggers.ContainsKey(key))
                {
                    try
                    {
                        var instance = Activator.CreateInstance(type, null) as Logging;
                        _cacheLoggers.Add(key, instance);
                    }
                    catch
                    {
                        Debug.LogWarning($"Create logger: {typeName} instance error!!!");
                    }
                }
            }
#else
            Debug.Log($"<color=#ff2763>Not enabled {nameof(LoggingSystem)} by symbol [OXGKIT_LOGGER_ON].</color>");
#endif
        }

        public static void CreateLogger<TLogging>() where TLogging : Logging, new()
        {
#if UNITY_EDITOR || OXGKIT_LOGGER_ON
            string typeName = typeof(TLogging).Name;
            string key = GetLoggerName(typeof(TLogging));

            if (typeName == nameof(Logging)) return;

            if (!_cacheLoggers.ContainsKey(key))
            {
                try
                {
                    var instance = new TLogging();
                    _cacheLoggers.Add(key, instance);
                }
                catch
                {
                    Debug.LogWarning($"Create logger: {typeName} instance error!!!");
                }
            }
#else
            Debug.Log($"<color=#ff2763>Not enabled {nameof(LoggingSystem)} by symbol [OXGKIT_LOGGER_ON].</color>");
#endif
        }

        public static void Print<TLogging>(object message) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers()) return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].Print(message);
            }
        }

        public static void PrintWarning<TLogging>(object message) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers()) return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].PrintWarning(message);
            }
        }

        public static void PrintError<TLogging>(object message) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers()) return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].PrintError(message);
            }
        }

        public static void PrintException<TLogging>(Exception exception) where TLogging : Logging
        {
            if (!CheckHasAnyLoggers()) return;

            string key = GetLoggerName<TLogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].PrintException(exception);
            }
        }
        #endregion

        internal bool logActive = true;

        public bool LogActive()
        {
            return logMainActive && this.logActive;
        }

        internal void Print(object message)
        {
            if (!this.LogActive()) return;
            this.Log(message);
        }

        internal void PrintWarning(object message)
        {
            if (!this.LogActive()) return;
            this.LogWarning(message);
        }

        internal void PrintError(object message)
        {
            if (!this.LogActive()) return;
            this.LogError(message);
        }

        internal void PrintException(Exception exception)
        {
            if (!this.LogActive()) return;
            this.LogException(exception);
        }

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
        #endregion
    }
}
