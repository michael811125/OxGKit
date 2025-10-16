using System.IO;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    public class LoggingSettings : ScriptableObject
    {
        /// <summary>
        /// 配置文件頭標
        /// </summary>
        public const short CIPHER_HEADER = 0x584F;

        /// <summary>
        /// 配置文件金鑰
        /// </summary>
        public byte cipher = 0x42;

        /// <summary>
        /// 配置文件擴展名
        /// </summary>
        public const string LOGGERS_CFG_EXTENSION = ".conf";

        /// <summary>
        /// 配置文件名稱
        /// </summary>
        public string loggersCfgName = "loggersconfig";

        private static LoggingSettings _settings = null;
        public static LoggingSettings settings
        {
            get
            {
                if (_settings == null)
                    _LoadSettingsData();
                return _settings;
            }
        }

        /// <summary>
        /// 加載配置文件
        /// </summary>
        private static void _LoadSettingsData()
        {
            _settings = Resources.Load<LoggingSettings>(nameof(LoggingSettings));
            if (_settings == null)
            {
                Debug.Log("[OxGKit.LoggingSystem] use default settings.");
                _settings = ScriptableObject.CreateInstance<LoggingSettings>();
            }
            else
            {
                Debug.Log("[OxGKit.LoggingSystem] use user settings.");
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/OxGKit/Create Settings/Create Logging Settings in Resources", priority = 1000)]
        private static void _CreateSettingsData()
        {
            string selectedPath = _GetSelectedPathOrFallback();

            string folderPath = Path.Combine(selectedPath, "Resources");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                UnityEditor.AssetDatabase.Refresh();
            }

            string assetPath = Path.Combine(folderPath, $"{nameof(LoggingSettings)}.asset");

            // 檢查是否已經存在
            LoggingSettings existingAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<LoggingSettings>(assetPath);
            if (existingAsset != null)
            {
                UnityEditor.EditorUtility.DisplayDialog("Already Exists", $"{nameof(LoggingSettings)}.asset already exists.", "OK");
                UnityEditor.Selection.activeObject = existingAsset;
                return;
            }

            // 建立新的 Settings
            var asset = ScriptableObject.CreateInstance<LoggingSettings>();
            UnityEditor.AssetDatabase.CreateAsset(asset, assetPath.Replace("\\", "/"));
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }

        private static string _GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in UnityEditor.Selection.GetFiltered(typeof(UnityEditor.DefaultAsset), UnityEditor.SelectionMode.Assets))
            {
                path = UnityEditor.AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }
            return path;
        }
#endif
    }
}
