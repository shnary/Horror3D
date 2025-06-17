using System;
using System.Collections.Generic;
using Core.Data;
using Core.Plains;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Systems {
    public static class SceneLoader {
        public static Scene ActiveScene => SceneManager.GetActiveScene();

        public static event Action OnLoad;

        private static readonly Dictionary<string, AsyncOperation> _loadingDictionary =
            new Dictionary<string, AsyncOperation>();

        public static AsyncOperation Load(string scene, Action onComplete = null) {

            if (IsValid(scene) == false) {
                Debug.LogError($"{scene} is not valid. Couldn't load the scene.");
                return null;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            _loadingDictionary.Clear();
            _loadingDictionary[scene] = operation;
            OnLoad?.Invoke();
            operation.completed += handle => {
                onComplete?.Invoke();
            };
            return operation;
        }

        public static AsyncOperation Append(string scene, Action onComplete = null) {

            if (IsValid(scene) == false) {
                Debug.LogError($"{scene} is not valid. Couldn't append the scene.");
                return null;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            _loadingDictionary[scene] = operation;
            operation.completed += handle => {
                onComplete?.Invoke();
            };
            return operation;
        }
        
        public static bool CanAppend(string scene) {

            if (_loadingDictionary.ContainsKey(scene)) {
                return false;
            }
            if (IsLoaded(scene)) {
                return false;
            }

            return true;
        }
        
        public static bool Remove(string scene) {

            if (IsValid(scene) == false) {
                Debug.LogError($"{scene} is not valid. Couldn't remove the scene.");
                return false;
            }

            if (IsLoaded(scene)) {
                SceneManager.UnloadSceneAsync(scene);
                if (_loadingDictionary.ContainsKey(scene)) {
                    _loadingDictionary.Remove(scene);
                }

                return true;
            }
            
            if (_loadingDictionary.ContainsKey(scene) == false) {
                return false;
            }

            if (_loadingDictionary[scene].isDone) {
                SceneManager.UnloadSceneAsync(scene);
                _loadingDictionary.Remove(scene);
                return true;
            }
            
            _loadingDictionary[scene].completed += handle => {
                SceneManager.UnloadSceneAsync(scene);
                _loadingDictionary.Remove(scene);
            };

            return false;
        }

        private static Scene GetScene(string sceneName) {
            return SceneManager.GetSceneByName(sceneName);
        }

        private static bool IsValid(string sceneName) {
            return SceneManager.GetSceneByName(sceneName).buildIndex != SceneManager.sceneCountInBuildSettings;
        }

        private static bool IsLoaded(string sceneName) {
            if (GetScene(sceneName).isLoaded == false) {
                return false;
            }

            return true;
        }
    }
}