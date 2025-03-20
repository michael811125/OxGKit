using System.IO;
using UnityEditor;
using UnityEngine;

namespace OxGKit.LoggingSystem.Editor
{
    public static class LoggingHelper
    {
        [MenuItem(itemName: "Assets/Create/OxGKit/Logging System/Create loggersconfig.json (In StreamingAssets)", isValidateFunction: false, priority: 0)]
        private static void _ExportAndCreateLoggersConfig()
        {
            // Initialized loggers
            LoggingLauncher.InitLoggers();
            var loggersConfig = new LoggersConfig();
            LoggingLauncher.ReloadLoggersConfigAndSet(loggersConfig, (result) =>
            {
                WriteConfig(loggersConfig);
                LoggingLauncherEditor.SaveEditorLoggersConfigData(loggersConfig);
            });
        }

        /// <summary>
        /// 寫入配置文件
        /// </summary>
        /// <param name="loggersConfig"></param>
        public static void WriteConfig(LoggersConfig loggersConfig)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, LoggersConfig.LOGGERS_CONFIG_FILE_NAME);

            // 獲取文件夾路徑
            string directoryPath = Path.GetDirectoryName(filePath);

            // 檢查文件夾是否存在, 如果不存在則創建
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"<color=#00d6ff>Created directory: {directoryPath}</color>");
            }

            // 將配置轉換為 JSON 字符串
            string json = JsonUtility.ToJson(loggersConfig, true);

            // 寫入文件
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh();
            Debug.Log($"<color=#00d6ff>Saved Loggers Config JSON to: {filePath}</color>");
        }
    }
}