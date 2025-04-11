using OxGKit.LocalizationSystem;
using UnityEngine;

public static class GameSettings
{
    public class Language
    {
        /// <summary>
        /// Key used to store language type
        /// </summary>
        private const string _GAME_LANGUAGE_KEY = "_GAME_LANGUAGE_KEY";

        /// <summary>
        /// Load the last [Language] setting
        /// <para>Note: Use the getter to retrieve it, and the setter to apply it.</para>
        /// </summary>
        public static void Init()
        {
            gameLanguage = gameLanguage;
        }

        public static LangType gameLanguage
        {
            get
            {
                return (LangType)PlayerPrefs.GetInt(_GAME_LANGUAGE_KEY, (int)Localization.systemLanguage);
            }
            set
            {
                PlayerPrefs.SetInt(_GAME_LANGUAGE_KEY, (int)value);
                _SetGameLanguage(value);
            }
        }

        private static void _SetGameLanguage(LangType langType)
        {
            var supportedLangType = Localization.GetAndCheckIsSupportedLanguage(langType);
            if (supportedLangType != langType)
                gameLanguage = supportedLangType;
            Localization.ChangeLanguage(supportedLangType);
        }
    }
}
