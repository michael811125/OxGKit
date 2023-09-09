using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace OxGKit.LoggingSystem.Editor
{
    [CustomEditor(typeof(LoggingLauncher))]
    public class LoggingLauncherEditor : UnityEditor.Editor
    {
        private UnityEditor.Editor _editor;
        private LoggingLauncher _target = null;
        private bool _isDirty = false;

        public override void OnInspectorGUI()
        {
            this._target = (LoggingLauncher)target;

            base.OnInspectorGUI();

            this.DrawLoggingSettingView();
        }

        protected void DrawLoggingSettingView()
        {
            serializedObject.Update();

            var setting = this._target.loggerSetting;

            if (setting != null)
            {
                EditorGUILayout.Space(20);
                this.DrawLine(new Color32(0, 255, 168, 255));

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // Reload button
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = new Color32(0, 255, 168, 255);
                EditorGUI.BeginDisabledGroup(!this._isDirty);
                if (GUILayout.Button("Runtime Reload Setting", GUILayout.MaxWidth(225f)))
                {
                    this._target.InitLoggers();
                    this._isDirty = false;
                }
                GUI.backgroundColor = bc;
                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                this.DrawLine(new Color32(0, 255, 222, 255));
                EditorGUILayout.Space(20);
            }

            EditorGUI.BeginChangeCheck();
            if (setting != null)
            {
                // Create setting view on inspector
                if (this._editor == null) this._editor = CreateEditor(setting);
                this._editor.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (Application.isPlaying) this._isDirty = true;
                serializedObject.ApplyModifiedProperties();
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
