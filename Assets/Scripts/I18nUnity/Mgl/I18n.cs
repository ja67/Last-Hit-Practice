using Lib.SimpleJSON;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastHitPractice
{
    public sealed class I18n
    {
        private static JSONNode config = null;

        private static readonly I18n instance = new I18n();

        private static string[] locales = new string[] { "en-US", "zh-CN" };//, "es-ES" };

        private static int _currentLocaleIndex = 1;

        private static string _currentLocale;

        public static string CurrentLocale => _currentLocale;

        public void NextLanguage(bool next)
        {
            if (next)
            {
                _currentLocaleIndex = (_currentLocaleIndex + 1) % locales.Length;
            }
            else
            {
                _currentLocaleIndex = (_currentLocaleIndex - 1) % locales.Length;
            }
            SetLocale(locales[_currentLocaleIndex]);
        }

        public static string[] Locales => locales;

        private static string _localePath = "Locales/";

        private static bool _isLoggingMissing = true;

        static I18n()
        {

            TextAsset configText = Resources.Load("config") as TextAsset;
            _currentLocale = JSON.Parse(configText.text)["language"];
        }

        private I18n()
        {
        }

        public static I18n Instance => instance;

        static void InitConfig()
        {
            if (locales.Contains(_currentLocale))
            {
                string localConfigPath = _localePath + _currentLocale;
                // Read the file as one string.
                TextAsset configText = Resources.Load(localConfigPath) as TextAsset;
                config = JSON.Parse(configText.text);
                _currentLocaleIndex = Array.IndexOf(Locales, _currentLocale);
            }
            else if (_isLoggingMissing)
            {
                Debug.Log("Missing: locale [" + _currentLocale + "] not found in supported list");
            }
        }

        public static string GetLocale()
        {
            return _currentLocale;
        }

        public static void SetLocale(string newLocale = null)
        {
            if (newLocale != null)
            {
                _currentLocale = newLocale;
                InitConfig();
            }
        }

        public static void SetPath(string localePath = null)
        {
            if (localePath != null)
            {
                _localePath = localePath;
                InitConfig();
            }
        }

        public static void Configure(string localePath = null, string newLocale = null, bool logMissing = true)
        {
            _isLoggingMissing = logMissing;
            SetPath(localePath);
            SetLocale(newLocale);
            InitConfig();
        }

        public string __(string key, params object[] args)
        {
            if (config == null)
            {
                InitConfig();
            }
            string translation = key;
            if (config[key] != null)
            {
                // if this key is a direct string
                if (config[key].Count == 0)
                {
                    translation = config[key];
                }
                else
                {
                    translation = FindSingularOrPlural(key, args);
                }
                // check if we have embeddable data
                if (args.Length > 0)
                {
                    translation = string.Format(translation, args);
                }
            }
            else if (_isLoggingMissing)
            {
                Debug.Log("Missing translation for:" + key);
            }
            return translation;
        }

        string FindSingularOrPlural(string key, object[] args)
        {
            JSONClass translationOptions = config[key].AsObject;
            string translation = key;
            string singPlurKey;
            // find format to try to use
            switch (GetCountAmount(args))
            {
                case 0:
                    singPlurKey = "zero";
                    break;
                case 1:
                    singPlurKey = "one";
                    break;
                default:
                    singPlurKey = "other";
                    break;
            }
            // try to use this plural/singular key
            if (translationOptions[singPlurKey] != null)
            {
                translation = translationOptions[singPlurKey];
            }
            else if (_isLoggingMissing)
            {
                Debug.Log("Missing singPlurKey:" + singPlurKey + " for:" + key);
            }
            return translation;
        }

        int GetCountAmount(object[] args)
        {
            int argOne = 0;
            // if arguments passed, try to parse first one to use as count
            if (args.Length > 0 && IsNumeric(args[0]))
            {
                argOne = Math.Abs(Convert.ToInt32(args[0]));
                if (argOne == 1 && Math.Abs(Convert.ToDouble(args[0])) != 1)
                {
                    // check if arg actually equals one
                    argOne = 2;
                }
                else if (argOne == 0 && Math.Abs(Convert.ToDouble(args[0])) != 0)
                {
                    // check if arg actually equals one
                    argOne = 2;
                }
            }
            return argOne;
        }

        bool IsNumeric(object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is short || Expression is int || Expression is long || Expression is decimal || Expression is float || Expression is double || Expression is bool)
                return true;

            return false;
        }

    }
}
