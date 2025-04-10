using UnityEngine;

namespace OxGKit.SaverSystem
{
    public class PlayerPrefsSaver : Saver
    {
        public override void SaveString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public override string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public override void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public override int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public override void SaveFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        public override float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public override void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
