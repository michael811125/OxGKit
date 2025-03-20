using System;
using UnityEditor;
using UnityEngine;

namespace OxGKit.LoggingSystem.Editor
{
    [CustomEditor(typeof(LoggingLauncher))]
    public class LoggingLauncherEditor : UnityEditor.Editor
    {
        private LoggingLauncher _target = null;
        private LoggersConfig _currentLoggersConfig
        {
            set => SaveEditorLoggersConfigData(value);
        }
        private LogLevel _selectedLogLevel;
        private bool _isDirty = false;
        private bool _isSaved = true;

        internal static string projectPath;
        internal static string keySaver;

        private void OnEnable()
        {
            this._target = (LoggingLauncher)this.target;

            projectPath = Application.dataPath;
            keySaver = $"{projectPath}_{nameof(LoggingLauncherEditor)}";

            string json = EditorStorage.GetData(keySaver, "_currentLoggersConfig", null);
            if (!string.IsNullOrEmpty(json))
                this._target.loggersConfig = JsonUtility.FromJson<LoggersConfig>(json);

            if (!Application.isPlaying)
            {
                this._isDirty = false;
                EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
            }

            this._isDirty = Convert.ToBoolean(EditorStorage.GetData(keySaver, "_isDirty", "false"));

            this._selectedLogLevel = (LogLevel)Convert.ToInt32(EditorStorage.GetData(keySaver, "_selectedLogLevel", "-1"));

            this._isSaved = Convert.ToBoolean(EditorStorage.GetData(keySaver, "_isSaved", "true"));
        }

        internal static void SaveEditorLoggersConfigData(LoggersConfig loggersConfig)
        {
            string json = JsonUtility.ToJson(loggersConfig);
            EditorStorage.SaveData(keySaver, "_currentLoggersConfig", json);
        }

        public override void OnInspectorGUI()
        {
            var loggersConfig = this._target.loggersConfig;

            this.DrawRuntimeReloadButtonView(loggersConfig);

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying)
                {
                    this._isDirty = true;
                    EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                }

                LoggingHelper.WriteConfig(loggersConfig);
                this._currentLoggersConfig = loggersConfig;
            }

            this.DrawControlButtonsView(loggersConfig);
        }

        protected void DrawRuntimeReloadButtonView(LoggersConfig loggersConfig)
        {
            // Draw runtime changes button
            if (loggersConfig != null)
            {
                EditorGUILayout.Space(20);
                this.DrawLine(new Color32(0, 255, 168, 255));

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // Reload button
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(102, 255, 153, 255);
                EditorGUI.BeginDisabledGroup(!this._isDirty);
                if (GUILayout.Button(new GUIContent("Apply Runtime Changes", $"When the logger settings are modified at runtime, they need to be reapplied."), GUILayout.MaxWidth(225f)))
                {
                    this._isDirty = false;
                    EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    LoggingLauncher.TryLoadLoggers();
                }
                GUI.backgroundColor = bc;
                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                this.DrawLine(new Color32(0, 255, 222, 255));
                EditorGUILayout.Space(20);
            }
        }

        protected void DrawControlButtonsView(LoggersConfig loggersConfig)
        {
            if (loggersConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // Reload from config button
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(24, 233, 255, 255);
                if (GUILayout.Button(new GUIContent("Reload From Config", $"Load configuration only from {LoggersConfig.LOGGERS_CONFIG_FILE_NAME}."), GUILayout.MaxWidth(250f)))
                {
                    if (Application.isPlaying)
                    {
                        this._isDirty = true;
                        EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    }

                    this._target.ReloadFromLoggersConfig((loggersConfig) =>
                    {
                        this._currentLoggersConfig = loggersConfig;
                    });
                }
                GUI.backgroundColor = bc;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (loggersConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // Reload button
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(0, 255, 209, 255);
                if (GUILayout.Button(new GUIContent("Reload", $"When the {LoggersConfig.LOGGERS_CONFIG_FILE_NAME} file is modified or a new logger is added, you can use Reload."), GUILayout.MaxWidth(250f)))
                {
                    if (Application.isPlaying)
                    {
                        this._isDirty = true;
                        EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    }

                    this._target.ReloadLoggersConfig((loggersConfig) =>
                    {
                        LoggingHelper.WriteConfig(loggersConfig);
                        this._currentLoggersConfig = loggersConfig;
                    });
                }
                GUI.backgroundColor = bc;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (loggersConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // Reset button
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(255, 87, 134, 255);
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(250f)))
                {
                    if (Application.isPlaying)
                    {
                        this._isDirty = true;
                        EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    }

                    this._target.ResetLoggersConfig((loggersConfig) =>
                    {
                        LoggingHelper.WriteConfig(loggersConfig);
                        this._currentLoggersConfig = loggersConfig;
                    });

                }
                GUI.backgroundColor = bc;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            // Draw select all and deselect all buttons
            if (loggersConfig != null)
            {
                EditorGUILayout.Space(2.5f);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical();
                // Select all button
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(164, 227, 255, 255);
                if (GUILayout.Button("Select All", GUILayout.MaxWidth(150f)))
                {
                    if (Application.isPlaying)
                    {
                        this._isDirty = true;
                        EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    }

                    foreach (var loggerSetting in loggersConfig.loggerSettings)
                        loggerSetting.logActive = true;

                    LoggingHelper.WriteConfig(loggersConfig);
                    this._currentLoggersConfig = loggersConfig;
                }
                GUI.backgroundColor = bc;
                // Log level enum dropdown
                EditorGUI.BeginChangeCheck();
                this._selectedLogLevel = (LogLevel)EditorGUILayout.EnumPopup(this._selectedLogLevel);
                if (EditorGUI.EndChangeCheck())
                {
                    this._isSaved = false;
                    EditorStorage.SaveData(keySaver, "_isSaved", this._isSaved.ToString());
                    EditorStorage.SaveData(keySaver, "_selectedLogLevel", ((int)this._selectedLogLevel).ToString());
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                // Deselect all button
                bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(164, 227, 255, 255);
                if (GUILayout.Button("Deselect All", GUILayout.MaxWidth(150f)))
                {
                    if (Application.isPlaying)
                    {
                        this._isDirty = true;
                        EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    }

                    foreach (var loggerSetting in loggersConfig.loggerSettings)
                        loggerSetting.logActive = false;

                    LoggingHelper.WriteConfig(loggersConfig);
                    this._currentLoggersConfig = loggersConfig;
                }
                GUI.backgroundColor = bc;
                // Set to All button
                EditorGUI.BeginDisabledGroup(this._isSaved);
                bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(164, 227, 255, 255);
                if (GUILayout.Button(new GUIContent("Set to All", "Set log level through dropdown options."), GUILayout.MaxWidth(150f)))
                {
                    this._isSaved = true;
                    EditorStorage.SaveData(keySaver, "_isSaved", this._isSaved.ToString());

                    if (Application.isPlaying)
                    {
                        this._isDirty = true;
                        EditorStorage.SaveData(keySaver, "_isDirty", this._isDirty.ToString());
                    }

                    foreach (var loggerSetting in loggersConfig.loggerSettings)
                        loggerSetting.logLevel = this._selectedLogLevel;

                    LoggingHelper.WriteConfig(loggersConfig);
                    this._currentLoggersConfig = loggersConfig;
                }
                GUI.backgroundColor = bc;
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        protected void DrawLine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
