using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Core.Hooks {
    public class LocalStringsHook : MonoBehaviour {
        [System.Serializable]
        private struct Map {
            public string key;
            public LocalizedString value;
        }

        [SerializeField] private Map[] dataMap;

        public string GetLocalizedString(string key) {
            var map = GetMap(key);
            if (map.value != null) {
                return map.value.GetLocalizedString();
            }

            Debug.LogError($"Couldn't find {key} or it's value is missing!");
            return "NULL";
        }
        
        public LocalizedString GetValue(string key) {
            var map = GetMap(key);
            if (map.value != null) {
                return map.value;
            }

            Debug.LogError($"Couldn't find {key} or it's value is missing!");
            return null;
        }

        private Map GetMap(string key) {
            var result = Array.Find(dataMap, map => string.Equals(map.key, key));
            return result;
        }
    }
}