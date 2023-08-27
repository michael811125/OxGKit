using System;
using System.Collections.Generic;

namespace OxGKit.LoggingSystem
{
    public abstract class Logging : ILogging
    {
        internal static bool isLauncherInitialized = false;
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

        internal static string GetLoggerName<TILogging>() where TILogging : ILogging
        {
            var type = typeof(TILogging);

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

        internal static bool CheckIsInitialized()
        {
            if (!isLauncherInitialized || _cacheLoggers.Count == 0) return false;
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

            var types = AssemblyFinder.GetAssignableTypes(typeof(Logging));
            foreach (var type in types)
            {
                string typeName = type.Name;
                string key = GetLoggerName(type);

                if (typeName == nameof(Logging)) continue;

                if (!_cacheLoggers.ContainsKey(key))
                {
                    var instance = (Logging)Activator.CreateInstance(type);
                    _cacheLoggers.Add(key, instance);
                }
            }
        }

        public static void Print<TILogging>(string message) where TILogging : ILogging
        {
            if (!CheckIsInitialized()) return;

            string key = GetLoggerName<TILogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].Log(message);
            }
        }

        public static void PrintWarning<TILogging>(string message) where TILogging : ILogging
        {
            if (!CheckIsInitialized()) return;

            string key = GetLoggerName<TILogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].LogWarning(message);
            }
        }

        public static void PrintError<TILogging>(string message) where TILogging : ILogging
        {
            if (!CheckIsInitialized()) return;

            string key = GetLoggerName<TILogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].LogError(message);
            }
        }

        public static void PrintException<TILogging>(Exception exception) where TILogging : ILogging
        {
            if (!CheckIsInitialized()) return;

            string key = GetLoggerName<TILogging>();

            if (_cacheLoggers.ContainsKey(key))
            {
                _cacheLoggers[key].LogException(exception);
            }
        }
        #endregion

        internal bool logActive = true;

        public bool LogActive()
        {
            return logMainActive && this.logActive;
        }

        public virtual void Log(string message)
        {
            if (!this.LogActive()) return;
            UnityEngine.Debug.Log(message);
        }

        public virtual void LogWarning(string message)
        {
            if (!this.LogActive()) return;
            UnityEngine.Debug.LogWarning(message);
        }

        public virtual void LogError(string message)
        {
            if (!this.LogActive()) return;
            UnityEngine.Debug.LogError(message);
        }

        public virtual void LogException(Exception exception)
        {
            if (!this.LogActive()) return;
            UnityEngine.Debug.LogException(exception);
        }
    }
}
