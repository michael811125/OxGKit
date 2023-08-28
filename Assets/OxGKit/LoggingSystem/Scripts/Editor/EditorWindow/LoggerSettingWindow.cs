using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static OxGKit.LoggingSystem.LoggerSetting;

namespace OxGKit.LoggingSystem.Editor
{
    public class LoggerSettingWindow : EditorWindow
    {
        private static LoggerSettingWindow _instance = null;
        internal static LoggerSettingWindow GetInstance()
        {
            if (_instance == null) _instance = GetWindow<LoggerSettingWindow>();
            return _instance;
        }

        private LoggerSetting _setting;

        [SerializeField]
        public bool logMainActive;
        [SerializeField]
        public List<LoggerConfig> loggers;

        private Vector2 _scrollview;

        private SerializedObject _serObj;
        private SerializedProperty _loggersPty;

        private static Vector2 _windowSize = new Vector2(800f, 400f);

        [MenuItem("OxGKit/Logging System/" + "Logger Setting", false, 999)]
        public static void ShowWindow()
        {
            _instance = null;
            GetInstance().titleContent = new GUIContent("Logger Setting");
            GetInstance().Show();
            GetInstance().minSize = _windowSize;
        }

        private void OnEnable()
        {
            this._serObj = new SerializedObject(this);
            this._loggersPty = this._serObj.FindProperty("loggers");

            // Init loggers
            Logging.InitLoggers();

            // Load setting
            this._setting = EditorTool.LoadSettingData<LoggerSetting>();
            this._setting.CheckAnSetSettingData();

            // Load data from setting
            this._LoadSettingData();
        }

        private void OnGUI()
        {
            this._DrawLoggersView();
        }

        private void _LoadSettingData()
        {
            this.logMainActive = this._setting.logMainActive;
            this.loggers = this._setting.loggerConfigs;
        }

        private void _DrawLoggersView()
        {
            EditorGUILayout.Space(10f);

            // Main active trigger
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            this.logMainActive = GUILayout.Toggle(this.logMainActive, new GUIContent("Enabled All Loggers"));
            if (EditorGUI.EndChangeCheck())
            {
                this._setting.logMainActive = this.logMainActive;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10f);

            this._scrollview = EditorGUILayout.BeginScrollView(this._scrollview, true, true);
            // Loggers setting
            this._serObj.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this._loggersPty, true);
            if (EditorGUI.EndChangeCheck())
            {
                this._serObj.ApplyModifiedProperties();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10f);

            // Refresh and Load button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Color bc = GUI.backgroundColor;
            GUI.backgroundColor = new Color32(31, 255, 102, 255);
            if (GUILayout.Button("Refresh and Load Loggers", GUILayout.MaxWidth(250f)))
            {
                this._setting.RefreshAndLoadLoggers();
            }
            GUI.backgroundColor = bc;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10f);

            // Reset button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bc = GUI.backgroundColor;
            GUI.backgroundColor = new Color32(255, 151, 240, 255);
            if (GUILayout.Button("Reset Loggers", GUILayout.MaxWidth(250f)))
            {
                this._setting.ResetSettingData();
            }
            GUI.backgroundColor = bc;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10f);
        }
    }
}