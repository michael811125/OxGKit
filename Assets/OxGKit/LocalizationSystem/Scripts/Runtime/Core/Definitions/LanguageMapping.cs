using System.Collections.Generic;

namespace OxGKit.LocalizationSystem
{
    public static class LanguageMapping
    {
        /// <summary>
        /// 定義語系類型與語系文字對照表
        /// </summary>
        private static readonly Dictionary<LangType, string> _langDescs = new Dictionary<LangType, string>()
        {
            { LangType.Arabic, "العربية" },                                // 阿拉伯語 ar_IL
            { LangType.ChineseSimplified, "简体中文" },                     // 中文簡體 zh_CN
            { LangType.ChineseTraditional, "繁體中文" },                    // 中文繁體 zh_TW
            { LangType.Dutch, "Nederlands" },                              // 荷蘭語 nl_NL
            { LangType.English, "English" },                               // 英語 en_US
            { LangType.French, "Français" },                               // 法語 fr_FR
            { LangType.German, "Deutsch" },                                // 德文 de_DE
            { LangType.Italian, "Italiano" },                              // 義大利語 it_IT
            { LangType.Portuguese, "Protuguês" },                          // 葡萄牙語 pt_BR
            { LangType.Spanish, "Español" },                               // 西班牙語 es_ES
            { LangType.Bengali, "বাংলা" },                                  // 孟加拉語 bn_BD
            { LangType.Croatian, "hrvatski" },                             // 克羅埃西亞語 hr_HR
            { LangType.Czech, "čeština" },                                 // 捷克語 cs_CZ
            { LangType.Danish, "Dansk" },                                  // 丹麥語 da_DK
            { LangType.Greek, "ελληνικά" },                                // 希臘文 el_GR
            { LangType.Hebrew, "עברית" },                                  // 希伯來文 he_IL
            { LangType.Hindi, "हिंदी" },                                     // 印度語 hi_IN
            { LangType.Hungarian, "Magyar" },                              // 匈牙利語 hu_HU
            { LangType.Indonesian, "Bahasa Indonesia" },                   // 印尼語 in_ID
            { LangType.Japanese, "日本語" },                                // 日語 ja_JP
            { LangType.Korean, "한국의" },                                  // 韓語 ko_KR
            { LangType.Malay, "Bahasa Melayu" },                           // 馬來語 ms_MY
            { LangType.Perisan, "فارسی" },                                 // 波斯語 fa_IR
            { LangType.Polish, "Polski" },                                 // 波蘭語 pl_PL
            { LangType.Romanian, "româna" },                               // 羅馬尼亞語 ro_RO
            { LangType.Russian, "Русский" },                               // 俄語 ru_RU
            { LangType.Serbian, "српски" },                                // 塞爾維亞語 sr_RS
            { LangType.Swedish, "Svenska" },                               // 瑞典語 sv_SE
            { LangType.Thai, "ไทย" },                                      // 泰語 th_TH
            { LangType.Turkish, "Türkçe" },                                // 土耳其語 tr_TR
            { LangType.Urdu, "اردو" },                                     // 烏爾都語 ur_PK
            { LangType.Vietnamese, "tiếng việt" },                         // 越南語 vi_VN
            { LangType.Catalan, "catalá" },                                // 加泰隆語 (西班牙) ca_ES
            { LangType.Latvian, "Latviski" },                              // 拉脫維亞語 lv_LV
            { LangType.Lithuanian, "Lietuvių" },                           // 立陶宛語 lt_LT
            { LangType.Norwegian, "Norsk bokmal" },                        // 挪威語 nb_NO
            { LangType.Slovak, "Slovenčina" },                             // 斯洛伐克語 sk_SK
            { LangType.Slovenian, "Slovenščina" },                         // 斯洛維尼亞語 sl_SI
            { LangType.Bulgarian, "български" },                           // 保加利亞語 bg_BG
            { LangType.Ukrainian, "українська" },                          // 烏克蘭語 uk_UA
            { LangType.Filipino, "Tagalog" },                              // 菲律賓語 tl_PH
            { LangType.Finnish, "Suomi" },                                 // 芬蘭語 fi_FI
            { LangType.Afrikaans, "Afrikaans" },                           // 南非荷蘭語 af_ZA
            { LangType.Romansh, "Rumantsch" },                             // 羅曼什語 (瑞士) rm_CH
            { LangType.Burmese, "ဗမာ" },                                   // 緬甸語 (官方) my_MM
            { LangType.Khmer, "ខ្មែរ" },                                     // 柬埔寨語 km_KH
            { LangType.Amharic, "አማርኛ" },                                 // 阿姆哈拉語 (衣索比亞) am_ET
            { LangType.Belarusian, "беларуская" },                         // 白俄羅斯語 be_BY
            { LangType.Estonian, "eesti" },                                // 愛沙尼亞語 et_EE
            { LangType.Swahili, "Kiswahili" },                             // 斯瓦希里語 (坦尚尼亞) sw_TZ
            { LangType.Zulu, "isiZulu" },                                  // 祖魯語 (南非) zu_ZA
            { LangType.Azerbaijani, "azərbaycanca" },                      // 亞塞拜然語 az_AZ
            { LangType.Armenian, "Հայերէն" },                              // 亞美尼亞語 (亞美尼亞) hy_AM
            { LangType.Georgian, "ქართული" },                             // 格魯吉亞語 (格魯吉亞) ka_GE
            { LangType.Laotian, "ລາວ" },                                   // 寮語 (寮國) lo_LA
            { LangType.Mongolian, "Монгол" },                              // 蒙古語 mn_MN
            { LangType.Nepali, "नेपाली" },                                   // 尼泊爾語 ne_NP
            { LangType.Kazakh, "қазақ тілі" },                             // 哈薩克語 kk_KZ
            { LangType.Galician, "Galego" },                               // 加利西亞語 gl-rES
            { LangType.Icelandic, "íslenska" },                            // 冰島語 is-rIS
            { LangType.Kannada, "ಕನ್ನಡ" },                                 // 坎納達語 kn-rIN
            { LangType.Kyrgyz, "кыргыз тили; قىرعىز تىلى" },              // 吉爾吉斯語 ky-rKG
            { LangType.Malayalam, "മലയാളം" },                           // 馬拉亞拉姆語 ml-rIN
            { LangType.Marathi, "मराठी" },                                  // 馬拉提語/馬拉地語 mr-rIN
            { LangType.Tamil, "தமிழ்" },                                   // 泰米爾語 ta-rIN
            { LangType.Macedonian, "македонски јазик" },                   // 馬其頓語 mk-rMK
            { LangType.Telugu, "తెలుగు" },                                 // 泰盧固語 te-rIN
            { LangType.Uzbek, "Ўзбек тили" },                              // 烏茲別克語 uz-rUZ
            { LangType.Basque, "Euskara" },                                // 巴斯克語 eu-rES
            { LangType.Sinhala, "සිංහල" },                                // 僧加羅語 (斯里蘭卡) si_LK
            { LangType.Faroese, "Føroyskt" }                               // 法羅語 fo-FO
        };

        /// <summary>
        /// 獲取對應語系文字說明
        /// </summary>
        /// <param name="langType"></param>
        /// <returns></returns>
        public static string GetLanguageDesc(LangType langType)
        {
            if (_langDescs.TryGetValue(langType, out string result))
                return result;
            return "Unknown Language";
        }
    }
}
