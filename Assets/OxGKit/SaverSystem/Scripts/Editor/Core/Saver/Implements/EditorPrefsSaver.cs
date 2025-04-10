using UnityEditor;

namespace OxGKit.SaverSystem.Editor
{
    public class EditorPrefsSaver : Saver
    {
        public override void SaveString(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public override string GetString(string key, string defaultValue = "")
        {
            return EditorPrefs.GetString(key, defaultValue);
        }

        public override void SaveInt(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        public override int GetInt(string key, int defaultValue = 0)
        {
            return EditorPrefs.GetInt(key, defaultValue);
        }

        public override void SaveFloat(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        public override float GetFloat(string key, float defaultValue = 0f)
        {
            return EditorPrefs.GetFloat(key, defaultValue);
        }

        public override bool HasKey(string key)
        {
            return EditorPrefs.HasKey(key);
        }

        public override void DeleteKey(string key)
        {
            EditorPrefs.DeleteKey(key);
        }

        public override void DeleteAll()
        {
            EditorPrefs.DeleteAll();
        }
    }
}