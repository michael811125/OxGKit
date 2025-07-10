using System;
using System.Collections.Generic;

namespace OxGKit.SaverSystem
{
    public abstract class Saver : ISaver, IDisposable
    {
        /// <summary>
        /// DataMap dirty 標記處理
        /// </summary>
        private Dictionary<string, bool> _dataMapDirtyFlags = new Dictionary<string, bool>();

        /// <summary>
        /// DataMap 緩存
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> _dataMaps = new Dictionary<string, Dictionary<string, string>>();

        public abstract void SaveString(string key, string value);

        public abstract string GetString(string key, string defaultValue = "");

        public abstract void SaveInt(string key, int value);

        public abstract int GetInt(string key, int defaultValue = 0);

        public abstract void SaveFloat(string key, float value);

        public abstract float GetFloat(string key, float defaultValue = 0);

        public abstract bool HasKey(string key);

        public abstract void DeleteKey(string key);

        public abstract void DeleteAll();

        /// <summary>
        /// 透過文本形式儲存數據
        /// </summary>
        /// <param name="contentKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveData(string contentKey, string key, string value)
        {
            string content = this.GetString(contentKey);

            Dictionary<string, string> dataMap = ParsingDataMap(content);
            if (dataMap.ContainsKey(key))
            {
                content = content.Replace($"{key} {dataMap[key]}", $"{key} {value}");
            }
            else
            {
                content += $"{key} {value}\n";
            }

            this.SaveString(contentKey, content);

            // Dirty check
            if (!this._dataMapDirtyFlags.ContainsKey(contentKey))
                this._dataMapDirtyFlags.Add(contentKey, true);
            else
                this._dataMapDirtyFlags[contentKey] = true;
        }

        /// <summary>
        /// 獲取文本中的特定數據
        /// </summary>
        /// <param name="contentKey"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetData(string contentKey, string key, string defaultValue = null)
        {
            string content = this.GetString(contentKey);
            if (string.IsNullOrEmpty(content))
                return defaultValue;

            // Check dirty state
            this._dataMapDirtyFlags.TryGetValue(contentKey, out bool isDirty);
            Dictionary<string, string> dataMap;
            if (isDirty)
            {
                dataMap = ParsingDataMap(content);

                if (!this._dataMaps.ContainsKey(contentKey))
                    this._dataMaps.Add(contentKey, dataMap);
                else
                    this._dataMaps[contentKey] = dataMap;

                if (this._dataMapDirtyFlags.ContainsKey(contentKey))
                    this._dataMapDirtyFlags[contentKey] = false;
            }
            else
            {
                if (this._dataMaps.ContainsKey(contentKey))
                {
                    dataMap = this._dataMaps[contentKey];
                }
                else
                {
                    dataMap = ParsingDataMap(content);
                    this._dataMaps.Add(contentKey, dataMap);
                }
            }

            dataMap.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// 刪除文本中的特定數據
        /// </summary>
        /// <param name="contentKey"></param>
        /// <param name="key"></param>
        public void DeleteData(string contentKey, string key)
        {
            string content = this.GetString(contentKey);

            Dictionary<string, string> dataMap = ParsingDataMap(content);
            if (dataMap.ContainsKey(key))
            {
                content = content.Replace($"{key} {dataMap[key]}\n", string.Empty);
            }

            this.SaveString(contentKey, content);

            // Dirty check
            if (!this._dataMapDirtyFlags.ContainsKey(contentKey))
                this._dataMapDirtyFlags.Add(contentKey, true);
            else
                this._dataMapDirtyFlags[contentKey] = true;
        }

        /// <summary>
        /// 刪除全文本數據
        /// </summary>
        /// <param name="contextKey"></param>
        public void DeleteContext(string contextKey)
        {
            this.DeleteKey(contextKey);
            if (this._dataMapDirtyFlags.ContainsKey(contextKey))
                this._dataMapDirtyFlags.Remove(contextKey);
            if (this._dataMaps.ContainsKey(contextKey))
                this._dataMaps.Remove(contextKey);
        }

        /// <summary>
        /// 解析文本數據
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParsingDataMap(string content)
        {
            Dictionary<string, string> dataMap = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(content))
                return dataMap;

            var allWords = content.Split('\n');
            var lines = new List<string>(allWords);
            foreach (var readLine in lines)
            {
                if (readLine.IndexOf('#') != -1 && readLine[0] == '#')
                    continue;
                var args = readLine.Split(' ', 2);
                if (args.Length >= 2)
                {
                    if (!dataMap.ContainsKey(args[0]))
                        dataMap.Add(args[0], args[1].Replace("\n", "").Replace("\r", ""));
                }
            }

            return dataMap;
        }

        public virtual void Dispose()
        {
            this._dataMapDirtyFlags = null;
            this._dataMaps = null;
        }
    }
}
