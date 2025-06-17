using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Systems {
    public class LoadingScreenLegacy : MonoBehaviour {
        private static LoadingScreenLegacy Instance {get; set;}

        [SerializeField] private GameObject progressCanvasGameObject;
        [SerializeField] private RectTransform loadingIndicatorTransform;

        private readonly List<AsyncOperation> _asyncOperations = new List<AsyncOperation>();
        private readonly List<Task> _taskOperations = new List<Task>();

        private bool _isActive;

        [HideInInspector]
        public UnityEvent onLoadingCompleted;

        private void Awake() {
            Instance = this;
            Disable();
        }

        public static void AddOperation(AsyncOperation operation) {
            Instance._asyncOperations.Add(operation);
        }

        public static void AddOperation(Task task) {
            Instance._taskOperations.Add(task);
        }
        
        public static void AddOperation(List<AsyncOperation> operations) {
            Instance._asyncOperations.AddRange(operations);
        }

        public static void AddOperation(List<Task> tasks) {
            Instance._taskOperations.AddRange(tasks);
        }

        public static void SetOnComplete(Action onComplete) {
            Instance.onLoadingCompleted.AddListener(() => onComplete?.Invoke());
        }

        public static void Activate(bool hideCanvasOnComplete=true) {
            if (Instance._isActive) {
                return;
            }
            Instance._isActive = true;
            Instance.progressCanvasGameObject.SetActive(true);
            Instance.StartCoroutine(Instance.HandleProgress(hideCanvasOnComplete));
            Instance.StartCoroutine(Instance.AnimateLoadingIndicator());
        }

        private IEnumerator AnimateLoadingIndicator() {
            while (_isActive) {
                loadingIndicatorTransform.Rotate(new Vector3(0, 0, -100 * Time.deltaTime));
                yield return null;
            }
        }

        private IEnumerator HandleProgress(bool hideCanvasOnComplete=true) {
            for (int i = 0; i < _asyncOperations.Count; i++) {
                while (!_asyncOperations[i].isDone) {
                    
                    yield return null;
                }
            }

            for (int i = 0; i < _taskOperations.Count; i++) {
                while (!_taskOperations[i].IsCompleted) {
                    yield return null;
                }
            }

            onLoadingCompleted?.Invoke();
            
            if (hideCanvasOnComplete) {
                Disable();
            }
        }

        public static void Refresh() {
            Instance.onLoadingCompleted.RemoveAllListeners();
            Instance._asyncOperations.Clear();
            Instance._taskOperations.Clear();
            Instance._isActive = false;
        }

        public static void HideCanvas() {
            Instance.progressCanvasGameObject.SetActive(false);
        }

        private static void Disable() {
            Refresh();
            HideCanvas();
        }
    }
}
