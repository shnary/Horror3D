using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Core.Systems {
    public static class GameLocalization {
        public static void SetLocale(int localeIndex) {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[localeIndex]);
        }
        
        public static string GetLocale(int localeIndex) {
            return LocalizationSettings.AvailableLocales.Locales[localeIndex].LocaleName;
        }
        
        public static string Get(string key, string table) {
            if (HasTable(table) == false) {
                return key;
            }
            string text = LocalizationSettings.StringDatabase.GetLocalizedString(
                table.ToLower(),
                key.ToLower());
            return text;
        }
        
        public static string Get(string key, string table, Dictionary<string, object> passedData) {
            if (HasTable(table) == false) {
                return key;
            }

            string text = "";

            try {
                text = LocalizationSettings.StringDatabase.GetLocalizedString(
                    table.ToLower(), 
                    key.ToLower(), new object[] { passedData });
            }
            catch (Exception e) {
                Debug.Log(e.ToString());
                text = "NULL";
            }
            return text;
        }

        public static string Get(string key, string table, object[] passedData) {
            if (HasTable(table) == false) {
                return key;
            }

            string text = "";

            try {
                text = LocalizationSettings.StringDatabase.GetLocalizedString(
                    table.ToLower(), 
                    key.ToLower(), passedData);
            }
            catch (Exception e) {
                Debug.Log(e.ToString());
                text = "NULL";
            }
            return text;
        }

        public static string Translate(this string s, string table) {
            if (string.IsNullOrEmpty(s)) {
                return "NULL";
            }

            var translatedVersion = LocalizationSettings.StringDatabase.GetLocalizedString(
                table, 
                s.ToLower());
            return translatedVersion;
        }
            
        public static string Translate<T>(this T enumerationValue, string table) where T : struct {
            var type = enumerationValue.GetType();
            if (!type.IsEnum) {
                return enumerationValue.ToString();
            }

            var translatedVersion = LocalizationSettings.StringDatabase.GetLocalizedString(
                table, 
                enumerationValue.ToString().ToLower());
            return translatedVersion;
        }

        private static void SetLocale(Locale locale) {
            LocalizationSettings.SelectedLocale = locale;
        }

        private static bool HasTable(string table) {
            if (string.IsNullOrEmpty(table) || string.IsNullOrWhiteSpace(table)) {
                return false;
            }

            if (LocalizationSettings.StringDatabase.GetTable(table) == null) {
                return false;
            }

            return true;
        }
    }
}