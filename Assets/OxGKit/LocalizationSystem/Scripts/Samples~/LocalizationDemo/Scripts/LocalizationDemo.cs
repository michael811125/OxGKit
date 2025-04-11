using OxGKit.LocalizationSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationDemo : MonoBehaviour
{
    #region Language Table Data Example
    public static readonly Dictionary<string, Dictionary<string, string>> langSheet = new Dictionary<string, Dictionary<string, string>>
    {
        {
            LangType.English.ToString(), new Dictionary<string, string>()
            {
                { "Str1", "Nice to meet you." },
                { "Str2", "Thank you." },
                { "Str3", "Have a nice day!" }
            }
        },
        {
            LangType.ChineseTraditional.ToString(), new Dictionary<string, string>()
            {
                { "Str1", "很高興認識你。" },
                { "Str2", "謝謝。" },
                { "Str3", "祝你有美好的一天！" }
            }
        },
        {
            LangType.ChineseSimplified.ToString(), new Dictionary<string, string>()
            {
                { "Str1", "很高兴认识你。" },
                { "Str2", "谢谢。" },
                { "Str3", "祝你有美好的一天！" }
            }
        },
        {
            LangType.Japanese.ToString(), new Dictionary<string, string>()
            {
                { "Str1", "はじめまして。" },
                { "Str2", "ありがとうございます。" },
                { "Str3", "良い一日をお過ごしください！" }
            }
        },
        {
            LangType.Korean.ToString(), new Dictionary<string, string>()
            {
                { "Str1", "만나서 반갑습니다." },
                { "Str2", "감사합니다." },
                { "Str3", "좋은 하루 보내세요!" }
            }
        },
    };
    #endregion

    public Text[] texts;
    public Dropdown langsDrd;

    private void Awake()
    {
        // Initialize localization config
        InitializeLocalization();

        // Initialize language setting to read last setting
        GameSettings.Language.Init();
    }

    private void Start()
    {
        // Init events
        this._InitEvents();

        // Draw UI View
        this._BasicDisplay();
    }

    #region Localization Config
    /// <summary>
    /// Initialize localization config
    /// </summary>
    public static void InitializeLocalization()
    {
        // Add supproted languages
        Localization.onAddSupportedLanguages = AddSupportedLanguages;

        // Parsing language table data
        Localization.onParsingLanguageData = ParsingLanguageData;
    }

    /// <summary>
    /// Handle by Localization.onAddSupportedLanguages
    /// </summary>
    /// <param name="supportedLanguages"></param>
    public static void AddSupportedLanguages(HashSet<LangType> supportedLanguages)
    {
        supportedLanguages.Add(LangType.English);
        supportedLanguages.Add(LangType.ChineseTraditional);
        supportedLanguages.Add(LangType.ChineseSimplified);
        supportedLanguages.Add(LangType.Japanese);
        supportedLanguages.Add(LangType.Korean);
    }

    /// <summary>
    /// Handle by Localization.onParsingLanguageData
    /// </summary>
    /// <param name="langType"></param>
    /// <param name="langData"></param>
    /// <returns></returns>
    public static bool ParsingLanguageData(LangType langType, Dictionary<string, string> langData)
    {
        // Your lang sheet (can load from json or server)
        if (langSheet.ContainsKey(langType.ToString()))
        {
            // The ref langData will be cached by Localization 
            foreach (var pair in langSheet[langType.ToString()])
                langData.TryAdd(pair.Key, pair.Value);
            return true;
        }
        return false;
    }
    #endregion

    #region UI View Logic
    private void _BasicDisplay()
    {
        this._DrawLanguageOptionsView();
        this._RefreshLastLanguageOptions();
    }

    /// <summary>
    /// Init events
    /// </summary>
    private void _InitEvents()
    {
        // Refresh lang text callback
        Localization.onChangeLanguage += (langType) => { this._RefreshLanguage(); };

        // Drd on value changed evnet
        this.langsDrd.onValueChanged.AddListener(idx =>
        {
            // Language selection save logic
            int selectedIndex = idx;
            string selectedOption = this.langsDrd.options[selectedIndex].text;
            // Convert language desc to language type
            Localization.GetSupportedLanguagesMappingByLangDesc().TryGetValue(selectedOption, out LangType selectedLangType);
            // Save selected language and change language
            GameSettings.Language.gameLanguage = selectedLangType;
        });
    }

    /// <summary>
    /// Set and draw language options view
    /// </summary>
    private void _DrawLanguageOptionsView()
    {
        if (this.langsDrd != null)
        {
            this.langsDrd.ClearOptions();
            // Add spported languages into dropdown options
            this.langsDrd.AddOptions(Localization.GetSupportedLanguagesMappingByLangDesc().Keys.ToList());
        }
    }

    /// <summary>
    /// Refresh last language to dropdown options
    /// </summary>
    private void _RefreshLastLanguageOptions()
    {
        // Load the last saved language setting (Convert language type to language desc)
        Localization.GetSupportedLanguagesMappingByLangType().TryGetValue(GameSettings.Language.gameLanguage, out string lastLanguageDesc);
        this.langsDrd.value = this.langsDrd.options.FindIndex(option => option.text == lastLanguageDesc);
    }

    /// <summary>
    /// Handle by Localization.onChangeLanguage
    /// </summary>
    private void _RefreshLanguage()
    {
        if (this.texts != null)
        {
            this.texts[0].text = Localization.GetStringByCode("Str1");
            this.texts[1].text = Localization.GetStringByCode("Str2");
            this.texts[2].text = Localization.GetStringByCode("Str3");
        }
    }
    #endregion
}