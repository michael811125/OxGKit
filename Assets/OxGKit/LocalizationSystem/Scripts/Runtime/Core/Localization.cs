using System;
using System.Collections.Generic;
using System.Linq;

namespace OxGKit.LocalizationSystem
{
    /// <summary>
    /// 預設本地化輔助
    /// </summary>
    public class Localization
    {
        /// <summary>
        /// 解析文字表數據回調 (ChangeLanguage 時調用解析)
        /// </summary>
        public static Func<LangType, Dictionary<string, string>, bool> onParsingLanguageData = null;

        /// <summary>
        /// 新增支持語系回調
        /// </summary>
        public static Action<HashSet<LangType>> onAddSupportedLanguages = null;

        /// <summary>
        /// 切換語言回調
        /// </summary>
        public static Action<LangType> onChangeLanguage = null;

        /// <summary>
        /// 當前語言
        /// </summary>
        public static LangType currentLanguage { get; private set; } = systemLanguage;

        /// <summary>
        /// 當前文字表數據
        /// </summary>
        private static Dictionary<string, string> _currentLanguageData = null;

        /// <summary>
        /// 獲取系統語言
        /// </summary>
        public static LangType systemLanguage
        {
            get
            {
                // 獲取當前對應的系統語系定義
                LangType systemLanguage = GetSystemLanguageToLangType();
                return GetAndCheckIsSupportedLanguage(systemLanguage);
            }
        }

        /// <summary>
        /// 支持語系種類
        /// </summary>
        private static HashSet<LangType> _supportedLangs = null;

        /// <summary>
        /// 轉譯表 LangType 為 key
        /// </summary>
        private static Dictionary<LangType, string> _supportedLanguagesMappingWithLangType = null;

        /// <summary>
        /// 轉譯表 LangDesc 為 key
        /// </summary>
        private static Dictionary<string, LangType> _supportedLanguagesMappingWithLangDesc = null;

        /// <summary>
        /// 獲取系統語系回傳對應的定義
        /// </summary>
        /// <returns></returns>
        public static LangType GetSystemLanguageToLangType()
        {
            UnityEngine.SystemLanguage detectedLanguage = UnityEngine.Application.systemLanguage;
            switch (detectedLanguage)
            {
                case UnityEngine.SystemLanguage.Arabic: return LangType.Arabic;
                case UnityEngine.SystemLanguage.Chinese: return LangType.ChineseSimplified;
                case UnityEngine.SystemLanguage.ChineseSimplified: return LangType.ChineseSimplified;
                case UnityEngine.SystemLanguage.ChineseTraditional: return LangType.ChineseTraditional;
                case UnityEngine.SystemLanguage.Dutch: return LangType.Dutch;
                case UnityEngine.SystemLanguage.English: return LangType.English;
                case UnityEngine.SystemLanguage.French: return LangType.French;
                case UnityEngine.SystemLanguage.German: return LangType.German;
                case UnityEngine.SystemLanguage.Italian: return LangType.Italian;
                case UnityEngine.SystemLanguage.Portuguese: return LangType.Portuguese;
                case UnityEngine.SystemLanguage.Spanish: return LangType.Spanish;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Bengali: return LangType.Bengali;
                //case UnityEngine.SystemLanguage.Croatian: return LangType.Croatian;

                case UnityEngine.SystemLanguage.Czech: return LangType.Czech;
                case UnityEngine.SystemLanguage.Danish: return LangType.Danish;
                case UnityEngine.SystemLanguage.Greek: return LangType.Greek;
                case UnityEngine.SystemLanguage.Hebrew: return LangType.Hebrew;
#if UNITY_2022_3 || UNITY_6000_0_OR_NEWER
                case UnityEngine.SystemLanguage.Hindi: return LangType.Hindi;
#endif
                case UnityEngine.SystemLanguage.Hungarian: return LangType.Hungarian;
                case UnityEngine.SystemLanguage.Indonesian: return LangType.Indonesian;
                case UnityEngine.SystemLanguage.Japanese: return LangType.Japanese;
                case UnityEngine.SystemLanguage.Korean: return LangType.Korean;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Malay: return LangType.Malay;
                //case UnityEngine.SystemLanguage.Persian: return LangType.Perisan;

                case UnityEngine.SystemLanguage.Polish: return LangType.Polish;
                case UnityEngine.SystemLanguage.Romanian: return LangType.Romanian;
                case UnityEngine.SystemLanguage.Russian: return LangType.Russian;
                case UnityEngine.SystemLanguage.SerboCroatian: return LangType.Serbian;
                case UnityEngine.SystemLanguage.Swedish: return LangType.Swedish;
                case UnityEngine.SystemLanguage.Thai: return LangType.Thai;
                case UnityEngine.SystemLanguage.Turkish: return LangType.Turkish;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Urdu: return LangType.Urdu;

                case UnityEngine.SystemLanguage.Vietnamese: return LangType.Vietnamese;
                case UnityEngine.SystemLanguage.Catalan: return LangType.Catalan;
                case UnityEngine.SystemLanguage.Latvian: return LangType.Latvian;
                case UnityEngine.SystemLanguage.Lithuanian: return LangType.Lithuanian;
                case UnityEngine.SystemLanguage.Norwegian: return LangType.Norwegian;
                case UnityEngine.SystemLanguage.Slovak: return LangType.Slovak;
                case UnityEngine.SystemLanguage.Slovenian: return LangType.Slovenian;
                case UnityEngine.SystemLanguage.Bulgarian: return LangType.Bulgarian;
                case UnityEngine.SystemLanguage.Ukrainian: return LangType.Ukrainian;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Filipino: return LangType.Filipino;

                case UnityEngine.SystemLanguage.Finnish: return LangType.Finnish;
                case UnityEngine.SystemLanguage.Afrikaans: return LangType.Afrikaans;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Romansh: return LangType.Romansh;
                //case UnityEngine.SystemLanguage.Burmese: return LangType.Burmese;
                //case UnityEngine.SystemLanguage.Khmer: return LangType.Khmer;
                //case UnityEngine.SystemLanguage.Amharic: return LangType.Amharic;

                case UnityEngine.SystemLanguage.Belarusian: return LangType.Belarusian;
                case UnityEngine.SystemLanguage.Estonian: return LangType.Estonian;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Swahili: return LangType.Swahili;
                //case UnityEngine.SystemLanguage.Zulu: return LangType.Zulu;
                //case UnityEngine.SystemLanguage.Azerbaijani: return LangType.Azerbaijani;
                //case UnityEngine.SystemLanguage.Armenian: return LangType.Armenian;
                //case UnityEngine.SystemLanguage.Georgian: return LangType.Georgian;
                //case UnityEngine.SystemLanguage.Lao: return LangType.Laotian;
                //case UnityEngine.SystemLanguage.Mongolian: return LangType.Mongolian;
                //case UnityEngine.SystemLanguage.Nepali: return LangType.Nepali;
                //case UnityEngine.SystemLanguage.Kazakh: return LangType.Kazakh;
                //case UnityEngine.SystemLanguage.Galician: return LangType.Galician;

                case UnityEngine.SystemLanguage.Icelandic: return LangType.Icelandic;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Kannada: return LangType.Kannada;
                //case UnityEngine.SystemLanguage.Kyrgyz: return LangType.Kyrgyz;
                //case UnityEngine.SystemLanguage.Malayalam: return LangType.Malayalam;
                //case UnityEngine.SystemLanguage.Marathi: return LangType.Marathi;
                //case UnityEngine.SystemLanguage.Tamil: return LangType.Tamil;
                //case UnityEngine.SystemLanguage.Macedonian: return LangType.Macedonian;
                //case UnityEngine.SystemLanguage.Telugu: return LangType.Telugu;
                //case UnityEngine.SystemLanguage.Uzbek: return LangType.Uzbek;

                case UnityEngine.SystemLanguage.Basque: return LangType.Basque;

                // Unity 未內建的語言
                //case UnityEngine.SystemLanguage.Sinhala: return LangType.Sinhala;

                case UnityEngine.SystemLanguage.Unknown: return LangType.Unspecified;
                default: return LangType.Unspecified;
            }
        }

        /// <summary>
        /// 獲取當前支持語系
        /// </summary>
        /// <returns></returns>
        public static HashSet<LangType> GetSupportedLanguages()
        {
            if (_supportedLangs == null)
            {
                HashSet<LangType> supportedLangs = new HashSet<LangType>();
                onAddSupportedLanguages?.Invoke(supportedLangs);
                _supportedLangs = supportedLangs;
            }
            return _supportedLangs;
        }

        /// <summary>
        /// 獲取與檢測是否屬於支持語系類型
        /// </summary>
        /// <param name="langType"></param>
        /// <returns></returns>
        public static LangType GetAndCheckIsSupportedLanguage(LangType langType)
        {
            // 檢查是否當前系統語言是否屬於支持語系
            if (IsSupportedLanguage(langType))
                return langType;
            // 不支持的語系, 一律返回 English
            else
                return LangType.English;
        }

        /// <summary>
        /// 是否屬於支持語系
        /// </summary>
        /// <param name="langType"></param>
        /// <returns></returns>
        public static bool IsSupportedLanguage(LangType langType)
        {
            var supportedLangs = GetSupportedLanguages();
            return supportedLangs.Contains(langType);
        }

        /// <summary>
        /// Key = LangType, 獲取語系文字對照表 (ex: LangType.Spanish -> "Español")
        /// </summary>
        /// <returns></returns>
        public static Dictionary<LangType, string> GetSupportedLanguagesMappingByLangType()
        {
            if (_supportedLanguagesMappingWithLangType == null)
            {
                // 進行轉換
                _supportedLanguagesMappingWithLangType = new Dictionary<LangType, string>();
                foreach (var langType in GetSupportedLanguages().ToArray())
                {
                    _supportedLanguagesMappingWithLangType.Add(langType, LanguageMapping.GetLanguageDesc(langType));
                }
            }
            return _supportedLanguagesMappingWithLangType;
        }

        /// <summary>
        /// Key = LangDesc, 獲取語系文字對照表 (ex: "Español" -> LangType.Spanish)
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, LangType> GetSupportedLanguagesMappingByLangDesc()
        {
            if (_supportedLanguagesMappingWithLangDesc == null)
            {
                // 進行轉換
                _supportedLanguagesMappingWithLangDesc = new Dictionary<string, LangType>();
                foreach (var langType in GetSupportedLanguages().ToArray())
                {
                    _supportedLanguagesMappingWithLangDesc.Add(LanguageMapping.GetLanguageDesc(langType), langType);
                }
            }
            return _supportedLanguagesMappingWithLangDesc;
        }

        /// <summary>
        /// 切換語系與驗證語系合法性
        /// </summary>
        /// <param name="langType"></param>
        /// <exception cref="Exception"></exception>
        public static void ChangeLanguage(LangType langType)
        {
            Dictionary<string, string> langData = new Dictionary<string, string>();
            if (onParsingLanguageData != null && onParsingLanguageData(langType, langData))
            {
                if (IsSupportedLanguage(langType))
                {
                    _currentLanguageData = langData;
                    currentLanguage = langType;
                    onChangeLanguage?.Invoke(currentLanguage);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"The language type is not supported: {langType}.");
                }
            }
            else
                throw new Exception($"Failed to parse the language: {langType} table data! Make sure to implement the 'onParsingLanguageData' method to handle language data parsing.");
        }

        /// <summary>
        /// 根據代碼取得語言表中的對應文字
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetStringByCode(string code)
        {
            if (_currentLanguageData == null)
                throw new Exception($"Language table data not found for current language: {currentLanguage}.");

            if (_currentLanguageData.TryGetValue(code, out string result))
                return result;
            else
                result = "Unknown Text";

            return result;
        }
    }
}