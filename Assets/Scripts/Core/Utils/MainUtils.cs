using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Core.Utils {
    public static class MainUtils {
        
        private static readonly Vector3 Vector3Zero = Vector3.zero;
        private static readonly Vector3 Vector3One = Vector3.one;
        private static readonly Vector3 Vector3YDown = new Vector3(0,-1);
        
        private static List<RaycastResult> _raycastResults = new List<RaycastResult>(1);

        private static Camera _worldCamera;
        
        public const int SortingOrderDefault = 5000;
        
        // Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = SortingOrderDefault) {
            return (int)(baseSortingOrder - position.y) + offset;
        }

        public static Vector3 GetCenterPointOfPositions(Vector3[] vectors) {
            float xTotal = 0;
            float yTotal = 0;
            float zTotal = 0;
            foreach (var vector in vectors) {
                xTotal += vector.x;
                yTotal += vector.y;
                zTotal += vector.z;
            }

            var length = vectors.Length;
            return new Vector3(xTotal / length, yTotal / length, zTotal / length);
        }

        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition() {
            if (_worldCamera == null) {
                _worldCamera = Camera.main;
            }
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, _worldCamera);
            vec.z = 0f;
            return vec;
        }
        
        public static Ray GetMouseRay(Vector3 mousePosition) {
            if (_worldCamera == null) {
                _worldCamera = Camera.main;
            }
            Ray ray = _worldCamera.ScreenPointToRay(mousePosition);
            return ray;
        }

        public static Vector3 GetMouseWorldPositionWithZ() {
            if (_worldCamera == null) {
                _worldCamera = Camera.main;
            }
            return GetMouseWorldPositionWithZ(Input.mousePosition, _worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static void SpawnRepetitiveInsideParent(Transform template, Transform parent, int spawnCount, Action<int, Transform> onSpawn, bool removeOlds=true, List<Transform> excludeFromDestroying=null) {
            template.gameObject.SetActive(false);

            if (removeOlds) {
                foreach (Transform child in parent) {
                    if (template == child || (excludeFromDestroying != null && excludeFromDestroying.Contains(child))) {
                        continue;
                    }
                    Object.Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < spawnCount; i++) {
                var spawnTransform = Object.Instantiate(template, parent);
                onSpawn?.Invoke(i, spawnTransform);
                spawnTransform.gameObject.SetActive(true);
            }
        }

        public static Vector3 GetDirToMouse(Vector3 fromPosition) {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }

        public static bool IsPointerOverUIObject() {
            if (EventSystem.current == null) {
                return false;
            }
            
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };

            EventSystem.current.RaycastAll(eventDataCurrentPosition, _raycastResults);
            int uiLayer = LayerMask.NameToLayer("UI");
            _raycastResults = _raycastResults.FindAll(t => t.gameObject.layer == uiLayer);
            return _raycastResults.Count > 0;
        }
        
        // Returns 00-FF, value 0->255
        public static string Dec_to_Hex(int value) {
            return value.ToString("X2");
        }

        // Returns 0-255
        public static int Hex_to_Dec(string hex) {
            return Convert.ToInt32(hex, 16);
        }
        
        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value) {
            return Dec_to_Hex((int)Mathf.Round(value*255f));
        }

        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex) {
            return Hex_to_Dec(hex)/255f;
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(Color color) {
            string red = Dec01_to_Hex(color.r);
            string green = Dec01_to_Hex(color.g);
            string blue = Dec01_to_Hex(color.b);
            return red+green+blue;
        }
        
        // Get Hex Color FF00FFAA
        public static string GetStringFromColorWithAlpha(Color color) {
            string alpha = Dec01_to_Hex(color.a);
            return GetStringFromColor(color)+alpha;
        }

        // Sets out values to Hex String 'FF'
        public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha) {
            red = Dec01_to_Hex(color.r);
            green = Dec01_to_Hex(color.g);
            blue = Dec01_to_Hex(color.b);
            alpha = Dec01_to_Hex(color.a);
        }
        
        // Get Hex Color FF00FF
        public static string GetStringFromColor(float r, float g, float b) {
            string red = Dec01_to_Hex(r);
            string green = Dec01_to_Hex(g);
            string blue = Dec01_to_Hex(b);
            return red+green+blue;
        }
        
        // Get Hex Color FF00FFAA
        public static string GetStringFromColor(float r, float g, float b, float a) {
            string alpha = Dec01_to_Hex(a);
            return GetStringFromColor(r,g,b)+alpha;
        }
        
        // Get Color from Hex string FF00FFAA
        public static Color GetColorFromString(string color) {
            float red = Hex_to_Dec01(color.Substring(0,2));
            float green = Hex_to_Dec01(color.Substring(2,2));
            float blue = Hex_to_Dec01(color.Substring(4,2));
            float alpha = 1f;
            if (color.Length >= 8) {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6,2));
            }
            return new Color(red, green, blue, alpha);
        }

        // Get UI Position from World Position
        public static Vector2 GetWorldToUIPosition(Vector3 worldPosition, Transform parent, Camera uiCamera, Camera worldCamera) {
            Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            Vector3 uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
            Vector3 localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
            return new Vector2(localPos.x, localPos.y);
        }

        public static Vector3 GetWorldPositionFromUIZeroZ() {
            Vector3 vec = GetWorldPositionFromUI(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        // Get World Position from UI Position
        public static Vector3 GetWorldPositionFromUI() {
            return GetWorldPositionFromUI(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetWorldPositionFromUI(Camera worldCamera) {
            return GetWorldPositionFromUI(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static Vector3 GetWorldPositionFromUI_Perspective() {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera) {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera) {
            Ray ray = worldCamera.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        // Return random element from array
        public static T GetRandomElement<T>(T[] array) {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        
        // Return random element from list
        public static T GetRandomElement<T>(List<T> list) {
            if (list.Count == 0) {
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }


        // Return a number with milli dots, 1000000 -> 1.000.000
        public static string GetMilliDots(float n) {
            return GetMilliDots((long)n);
        }

        public static string GetMilliDots(long n) {
            string ret = n.ToString();
            for (int i = 1; i <= Mathf.Floor(ret.Length / 4); i++) {
                ret = ret.Substring(0, ret.Length - i * 3 - (i - 1)) + "." + ret.Substring(ret.Length - i * 3 - (i - 1));
            }
            return ret;
        }


        // Return with milli dots and dollar sign
        public static string GetDollars(float n) {
            return GetDollars((long)n);
        }

        [System.Serializable]
        private class JsonDictionary {
            public List<string> keyList = new List<string>();
            public List<string> valueList = new List<string>();
        }

        // Take a Dictionary and return JSON string
        public static string SaveDictionaryJson<TKey, TValue>(Dictionary<TKey, TValue> dictionary) {
            JsonDictionary jsonDictionary = new JsonDictionary();
            foreach (TKey key in dictionary.Keys) {
                jsonDictionary.keyList.Add(JsonUtility.ToJson(key));
                jsonDictionary.valueList.Add(JsonUtility.ToJson(dictionary[key]));
            }
            string saveJson = JsonUtility.ToJson(jsonDictionary);
            return saveJson;
        }

        // Take a JSON string and return Dictionary<T1, T2>
        public static Dictionary<TKey, TValue> LoadDictionaryJson<TKey, TValue>(string saveJson) {
            JsonDictionary jsonDictionary = JsonUtility.FromJson<JsonDictionary>(saveJson);
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
            for (int i = 0; i < jsonDictionary.keyList.Count; i++) {
                TKey key = JsonUtility.FromJson<TKey>(jsonDictionary.keyList[i]);
                TValue value = JsonUtility.FromJson<TValue>(jsonDictionary.valueList[i]);
                ret[key] = value;
            }
            return ret;
        }

        // Destroy all children of this parent
        public static void DestroyChildren(Transform parent) {
            foreach (Transform transform in parent)
                GameObject.Destroy(transform.gameObject);
        }

        // Destroy all children and randomize their names, useful if you want to do a Find() after calling destroy, since they only really get destroyed at the end of the frame
        public static void DestroyChildrenRandomizeNames(Transform parent) {
            foreach (Transform transform in parent) {
                transform.name = "" + UnityEngine.Random.Range(10000, 99999);
                GameObject.Destroy(transform.gameObject);
            }
        }

        // Destroy all children except the ones with these names
        // Set all parent and all children to this layer
        public static void SetAllChildrenLayer(Transform parent, int layer) {
            parent.gameObject.layer = layer;
            foreach (Transform trans in parent) {
                SetAllChildrenLayer(trans, layer);
            }
        }
        
        public static void ShuffleArray<T>(T[] arr, int iterations) {
            for (int i = 0; i < iterations; i++) {
                int rnd = UnityEngine.Random.Range(0, arr.Length);
                (arr[rnd], arr[0]) = (arr[0], arr[rnd]);
            }
        }
        public static void ShuffleArray<T>(T[] arr, int iterations, System.Random random) {
            for (int i = 0; i < iterations; i++) {
                int rnd = random.Next(0, arr.Length);
                (arr[rnd], arr[0]) = (arr[0], arr[rnd]);
            }
        }

        public static void ShuffleList<T>(List<T> list, int iterations) {
            for (int i = 0; i < iterations; i++) {
                int rnd = UnityEngine.Random.Range(0, list.Count);
                (list[rnd], list[0]) = (list[0], list[rnd]);
            }
        }

        public static T[] RemoveDuplicates<T>(T[] arr) {
            List<T> list = new List<T>();
            foreach (T t in arr) {
                if (!list.Contains(t)) {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        public static List<T> RemoveDuplicates<T>(List<T> arr) {
            List<T> list = new List<T>();
            foreach (T t in arr) {
                if (!list.Contains(t)) {
                    list.Add(t);
                }
            }
            return list;
        }

        public static void TriggerActionPrefs(Action action, string key) {
            if (PlayerPrefs.GetInt(key) == 0) {
                action?.Invoke();
                PlayerPrefs.SetInt(key, 1);
            }
        }

        public static void TriggerActionPrefsNoSave(Action action, string key) {
            if (PlayerPrefs.GetInt(key) == 0) {
                action?.Invoke();
            }
        }

        public static void TriggerActionNextFrame(Action action) {
            FunctionUpdater.Create(() => {
                action?.Invoke();
                return true;
            }, "Trigger Next Frame");
        }
        
        public static string IntToRoman(int num) {
            string romanResult = string.Empty;
            string[] romanLetters = {
                "M",
                "CM",
                "D",
                "CD",
                "C",
                "XC",
                "L",
                "XL",
                "X",
                "IX",
                "V",
                "IV",
                "I"
            };
            int[] numbers = {
                1000,
                900,
                500,
                400,
                100,
                90,
                50,
                40,
                10,
                9,
                5,
                4,
                1
            };
            int i = 0;
            while (num != 0) {
                if (num >= numbers[i]) {
                    num -= numbers[i];
                    romanResult += romanLetters[i];
                } else {
                    i++;
                }
            }
            return romanResult;
        }
        
        public static string KFormatter(float num) {
            if (num >= 1000000000) {
                return (num / 1000000000D).ToString("0.#", CultureInfo.InvariantCulture).TrimStart(new char[] { '0' }) +
                    "b";
            }

            if (num >= 1000000) {
                return (num / 1000000D).ToString("0.#", CultureInfo.InvariantCulture).TrimStart(new char[] { '0' }) +
                    "m";
            }

            if (num >= 10000) {
                return (num / 1000D).ToString("0.#", CultureInfo.InvariantCulture).TrimStart(new char[] { '0' }) + "k";
            }

            if (num <= 0) {
                return num + "";
            }

            return num.ToString("0,#", CultureInfo.InvariantCulture).TrimStart(new char[] { '0' });
        }
    }
}