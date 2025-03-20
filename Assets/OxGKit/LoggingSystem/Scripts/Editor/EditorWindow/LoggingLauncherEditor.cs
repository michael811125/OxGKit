using DG.DemiEditor;
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

            #region Section 1
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();

            this.serializedObject.Update();
            this.DrawSeparator("Options", "#282828", 1);
            Rect infoBoxRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.HelpBox(infoBoxRect, "HybridCLR is not supported and must be initialized manually.", MessageType.Warning);
            this._target.initLoggersOnAwake = EditorGUILayout.Toggle(new GUIContent("Initialized On Awake", "If enabled, all loggers in the application domain will be automatically detected and loaded. (HybridCLR is not supported and must be initialized manually.)"), this._target.initLoggersOnAwake);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this._target);
                this.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region Section 2
            this.DrawRuntimeReloadButtonView(loggersConfig);
            #endregion

            #region Section 3
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
            #endregion

            #region Section 4
            this.DrawControlButtonsView(loggersConfig);
            #endregion
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

        /// <summary>
        /// 繪製線條
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        protected void DrawLine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        /// 在 Inspector 中繪製分隔線
        /// </summary>
        /// <param name="title"></param>
        /// <param name="colorHex"></param>
        /// <param name="thickness"></param>
        protected void DrawSeparator(string title, string colorHex, float thickness)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color color);
            Texture2D t2d = MakeT2d(1, 1, color);

            // 根據是否有標題決定分隔線高度
            float separatorHeight = title.IsNullOrEmpty() ? thickness + 10 : thickness + 20;
            Rect separatorRect = EditorGUILayout.GetControlRect(false, separatorHeight);

            if (title.IsNullOrEmpty())
            {
                // 只有線條
                separatorRect.height = thickness;
                separatorRect.y += (separatorHeight - thickness) / 2; // 讓線條置中
                GUI.DrawTexture(separatorRect, t2d);
            }
            else
            {
                // 計算標題大小
                Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(title));
                float separatorWidth = (separatorRect.width - textSize.x) / 2 - 5;

                // 讓線條與標題垂直置中
                float centerY = separatorRect.y + (separatorHeight - thickness) / 2;
                float labelY = separatorRect.y + (separatorHeight - textSize.y) / 2;

                GUI.DrawTexture(new Rect(separatorRect.xMin, centerY, separatorWidth, thickness), t2d);
                GUI.Label(new Rect(separatorRect.xMin + separatorWidth + 5, labelY, textSize.x, textSize.y), title);
                GUI.DrawTexture(new Rect(separatorRect.xMin + separatorWidth + 10 + textSize.x, centerY, separatorWidth, thickness), t2d);
            }
        }

        /// <summary>
        /// 生成單色紋理
        /// </summary>
        protected Texture2D MakeT2d(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = color;
            }
            Texture2D t2d = new Texture2D(width, height);
            t2d.SetPixels(pixels);
            t2d.Apply();
            return t2d;
        }
    }
}
