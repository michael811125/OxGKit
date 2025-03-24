using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OxGKit.LoggingSystem.Editor
{
    public static class LoggingHelper
    {
        [MenuItem(itemName: "Assets/Create/OxGKit/Logging System/[JSON - Plaintext] Create loggersconfig.conf (In StreamingAssets)", isValidateFunction: false, priority: 0)]
        private static void _ExportAndCreateJsonLoggersConfig()
        {
            // Initialized loggers
            LoggingLauncher.InitLoggers();
            var loggersConfig = new LoggersConfig();
            LoggingLauncher.ReloadLoggersConfigAndSet(loggersConfig, (result) =>
            {
                WriteConfig(loggersConfig, ConfigFileType.Json);
                LoggingLauncherEditor.SaveEditorLoggersConfigData(loggersConfig);
            });
        }

        [MenuItem(itemName: "Assets/Create/OxGKit/Logging System/[BYTES - Cipher] Create loggersconfig.conf (In StreamingAssets)", isValidateFunction: false, priority: 1)]
        private static void _ExportAndCreateBytesLoggersConfig()
        {
            // Initialized loggers
            LoggingLauncher.InitLoggers();
            var loggersConfig = new LoggersConfig();
            LoggingLauncher.ReloadLoggersConfigAndSet(loggersConfig, (result) =>
            {
                WriteConfig(loggersConfig, ConfigFileType.Bytes);
                LoggingLauncherEditor.SaveEditorLoggersConfigData(loggersConfig);
            });
        }

        [MenuItem("Assets/OxGKit/Logging System/Convert loggersconfig.conf (BYTES [Cipher] <-> JSON [Plaintext])", false, -99)]
        private static void _ConvertConfigFile()
        {
            UnityEngine.Object selectedObject = Selection.activeObject;

            if (selectedObject != null)
            {
                // 獲取選中的資源的相對路徑
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);

                // 檢查選中的文件是否位於 StreamingAssets 資料夾內
                if (assetPath.StartsWith("Assets/StreamingAssets"))
                {
                    // Application.dataPath 返回的是 Assets 資料夾的完整路徑
                    string fullPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length), assetPath);

                    // 確保文件存在
                    if (File.Exists(fullPath))
                    {
                        // 讀取文件內容
                        byte[] data = File.ReadAllBytes(fullPath);
                        var info = BinaryHelper.DecryptToString(data);
                        LoggersConfig loggersConfig = null;
                        bool isJsonConvertible;

                        try
                        {
                            loggersConfig = JsonUtility.FromJson<LoggersConfig>(info.content);
                            isJsonConvertible = true;
                        }
                        catch (Exception ex)
                        {
                            isJsonConvertible = false;
                            Debug.LogException(new Exception("Convert failed: The content format is not valid JSON or it does not match the expected structure.", ex));
                        }

                        if (isJsonConvertible && loggersConfig != null)
                        {
                            // 根據文件類型進行轉換
                            switch (info.type)
                            {
                                case ConfigFileType.Json:
                                    // JSON to Bytes
                                    WriteConfig(loggersConfig, ConfigFileType.Bytes);
                                    break;
                                case ConfigFileType.Bytes:
                                    // Bytes to JSON
                                    WriteConfig(loggersConfig, ConfigFileType.Json);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"File does not exist at path: {fullPath}");
                    }
                }
                else
                {
                    Debug.LogWarning("Selected file is not in StreamingAssets folder.");
                }
            }
            else
            {
                Debug.LogWarning("No file selected.");
            }
        }

        /// <summary>
        /// 寫入配置文件
        /// </summary>
        /// <param name="loggersConfig"></param>
        /// <param name="configFileType"></param>
        public static void WriteConfig(LoggersConfig loggersConfig, ConfigFileType configFileType = ConfigFileType.Bytes)
        {
            string fileName = LoggersConfig.LOGGERS_CONFIG_FILE_NAME;
            string savePath = Path.Combine(Application.streamingAssetsPath, fileName);

            // 獲取文件夾路徑
            string directoryPath = Path.GetDirectoryName(savePath);

            // 檢查文件夾是否存在, 如果不存在則創建
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"<color=#00d6ff>Created directory: {directoryPath}</color>");
            }

            switch (configFileType)
            {
                // Json 類型
                case ConfigFileType.Json:
                    {
                        // 將配置轉換為 JSON 字符串
                        string json = JsonUtility.ToJson(loggersConfig, true);

                        // 寫入文件
                        File.WriteAllText(savePath, json);
                        AssetDatabase.Refresh();
                        Debug.Log($"<color=#00d6ff>Saved Loggers Config JSON to: {savePath}</color>");
                    }
                    break;

                // Bytes 類型
                case ConfigFileType.Bytes:
                    {
                        // 將配置轉換為 JSON 字符串
                        string json = JsonUtility.ToJson(loggersConfig, false);

                        // Binary
                        var writeBuffer = BinaryHelper.EncryptToBytes(json);

                        // 寫入配置文件
                        File.WriteAllBytes(savePath, writeBuffer);
                        AssetDatabase.Refresh();
                        Debug.Log($"<color=#00d6ff>Saved Loggers Config BYTES to: {savePath}</color>");
                    }
                    break;
            }
        }
    }
}