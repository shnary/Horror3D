using System;
using System.Collections;
using System.Collections.Generic;
using Core.Systems;
using Core.UI.Elements;
using Core.Data;
using Core.Plains;
using UnityEngine;

namespace Core.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance { get; private set; }
        
        private readonly Dictionary<string, UIObject> _uiDictionary = new Dictionary<string, UIObject>();
        private readonly Dictionary<string, UIObject> _uiSceneDictionary = new Dictionary<string, UIObject>();

        private readonly List<string> openedUiScenes = new List<string>();

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            SceneLoader.OnLoad += () => {
                // Scene load operation removes appended scenes automatically.
                openedUiScenes.Clear();
                _uiDictionary.Clear();
                _uiSceneDictionary.Clear();
            };
        }

        public void RegisterUIObject(string uiKey, UIObject uiObject) {
            if (_uiDictionary.ContainsKey(uiKey)) {
                return;
            }

            _uiDictionary[uiKey] = uiObject;
        }

        public void RegisterScene(string uiKey, UIObject uiObject) {
            if (_uiSceneDictionary.ContainsKey(uiKey)) {
                return;
            }

            _uiSceneDictionary[uiKey] = uiObject;
        }

        public void DeRegisterUIObject(string uiKey) {
            if (_uiDictionary.ContainsKey(uiKey) == false) {
                return;
            }

            _uiDictionary.Remove(uiKey);
        }

        public void DeRegisterScene(string uiKey) {
            if (_uiSceneDictionary.ContainsKey(uiKey) == false) {
                return;
            }

            _uiSceneDictionary.Remove(uiKey);
        }

        public void OpenScene(string sceneName, bool closeOtherUis=true, Action<UIObject> onComplete=null) {

            if (SceneLoader.CanAppend(sceneName) == false) {
                return;
            }

            if (closeOtherUis) {
                CloseOpenedScenes();
            }

            SceneLoader.Append(sceneName, () => {
                var uiObject = GetUIObject(sceneName);
                if (uiObject == null) {
                    Debug.LogError($"Couldn't find {sceneName} UI object.");
                    return;
                }

                if (openedUiScenes.Contains(sceneName) == false){
                    openedUiScenes.Add(sceneName);
                }

                onComplete?.Invoke(uiObject);
            });
        }
        
        public void OpenSceneForceAppend(string sceneName, bool closeOtherUis=true, Action<UIObject> onComplete=null) {
            if (closeOtherUis) {
                CloseOpenedScenes();
            }
            
            SceneLoader.Append(sceneName, () => {
                var uiObject = GetUIObject(sceneName);
                if (uiObject == null) {
                    Debug.LogError($"Couldn't find {sceneName} UI object.");
                    return;
                }

                if (openedUiScenes.Contains(sceneName) == false){
                    openedUiScenes.Add(sceneName);
                }

                onComplete?.Invoke(uiObject);
            });
        }

        public void CloseScene(string sceneName) {
            if (openedUiScenes.Contains(sceneName)){
                openedUiScenes.Remove(sceneName);
            }
            SceneLoader.Remove(sceneName);
        }

        public void CloseOpenedScenes() {
            foreach (var openedUi in openedUiScenes) {
                SceneLoader.Remove(openedUi);
            }
            openedUiScenes.Clear();
        }

        public UIObject GetUIObject(string uiKey) {
            if (_uiSceneDictionary.ContainsKey(uiKey) == false) {
                return null;
            }

            var uiObject = _uiSceneDictionary[uiKey];

            return uiObject;
        }

        public UIObject ActivateUIObject(string uiKey) {
            if (_uiDictionary.ContainsKey(uiKey) == false) {
                return null;
            }

            var uiObject = _uiDictionary[uiKey];

            foreach (var key in _uiDictionary.Keys) {
                if (string.Equals(uiKey, key)) {
                    continue;
                }

                if (_uiDictionary[key].objectActivated == null) {
                    Debug.LogError($"Object to activate is not referenced for {key}.");
                    continue;
                }

                _uiDictionary[key].objectActivated.SetActive(false);
            }

            return uiObject;
        }
        
        public void DeactivateUIObject(string uiKey) {
            if (_uiDictionary.ContainsKey(uiKey) == false) {
                return;
            }

            if (_uiDictionary[uiKey].objectActivated == null) {
                Debug.LogError($"Object to deactivate is not referenced for {uiKey}.");
                return;
            }

            _uiDictionary[uiKey].objectActivated.SetActive(false);
        }
        
        public void ActivateAllUIObjects() {
            foreach (var key in _uiDictionary.Keys) {
                if (_uiDictionary[key].objectActivated == null) {
                    Debug.LogError($"Object to activate is not referenced for {key}.");
                    continue;
                }
                _uiDictionary[key].objectActivated.SetActive(true);
            }
        }

        public void DeactivateAllUIObjects() {
            foreach (var key in _uiDictionary.Keys) {
                if (_uiDictionary[key].objectActivated == null) {
                    Debug.LogError($"Object to deactivate is not referenced for {key}.");
                    continue;
                }
                _uiDictionary[key].objectActivated.SetActive(false);
            }
        }
    }
}
