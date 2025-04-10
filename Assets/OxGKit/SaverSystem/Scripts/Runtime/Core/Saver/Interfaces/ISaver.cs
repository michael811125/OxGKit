namespace OxGKit.SaverSystem
{
    public interface ISaver
    {
        void SaveString(string key, string value);

        string GetString(string key, string defaultValue = "");

        void SaveInt(string key, int value);

        int GetInt(string key, int defaultValue = 0);

        void SaveFloat(string key, float value);

        float GetFloat(string key, float defaultValue = 0f);

        bool HasKey(string key);

        void DeleteKey(string key);

        void DeleteAll();
    }
}
