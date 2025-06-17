using System;
using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;

namespace Core.Popup {
    public class Popups : MonoBehaviour {

        private const int StartingPoolTexts = 3;
        private const int StartingPoolPopup = 1;
        
        private static Popups instance;

        [SerializeField] private Transform popupCanvasTransform;
        [SerializeField] private Transform topTextCanvasTransform;

        [SerializeField] private TopText topText;

        private List<PopupBox> _activePopupBoxes;
        private List<TopText> _activeTopTexts;

        private Dictionary<Type, GenericObjectPool<PopupBox>> _boxesPoolDictionary;
        private GenericObjectPool<TopText> _textPool;

        private void Awake() {
            instance = this;
            _activePopupBoxes = new List<PopupBox>();
            _activeTopTexts = new List<TopText>();
            _boxesPoolDictionary = new Dictionary<Type, GenericObjectPool<PopupBox>>();
            instance.popupCanvasTransform.gameObject.SetActive(false);
            instance.topTextCanvasTransform.gameObject.SetActive(false);
        }

        public static void Setup(PopupBox[] boxes) {
            instance.SetupPool(boxes);
        }

        public void SetupPool(PopupBox[] boxes) {
            _textPool = new GenericObjectPool<TopText>(StartingPoolTexts, () => Instantiate(topText, topTextCanvasTransform));
            foreach (var box in boxes) {
                if (_boxesPoolDictionary.ContainsKey(box.GetType())) continue;
                _boxesPoolDictionary[box.GetType()] =
                    new GenericObjectPool<PopupBox>(
                        StartingPoolPopup,
                        () => Instantiate(box.gameObject, popupCanvasTransform).GetComponent<PopupBox>());
            }
        }

        public static T Get<T>() where T : PopupBox {
            return instance._boxesPoolDictionary[typeof(T)].Get() as T;
        }
        
        public static TopText GetText() {
            return instance._textPool.Get();
        }

        public static PopupSequence Sequence() {
            var sequence = new PopupSequence();
            return sequence;
        }

        public static void ExecuteSequence(PopupSequence sequence) {
            instance.StartCoroutine(sequence.ShowBox());
        }

        public static void Add(PopupBox popup) {
            instance._activePopupBoxes.Add(popup);
            instance.popupCanvasTransform.gameObject.SetActive(true);
            popup.gameObject.SetActive(true);
        }

        public static void Add(TopText topText) {
            instance._activeTopTexts.Add(topText);
            instance.topTextCanvasTransform.gameObject.SetActive(true);
            topText.gameObject.SetActive(true);
        }

        public static void Remove(PopupBox popup) {
            instance._boxesPoolDictionary[popup.GetType()].Release(popup);
            popup.gameObject.DeactivateNextFrame();
            if (instance._activePopupBoxes.Contains(popup)) {
                instance._activePopupBoxes.Remove(popup);
            }
            if (instance._activePopupBoxes.Count == 0) {
                instance.popupCanvasTransform.gameObject.DeactivateNextFrame();
            }
        }

        public static void Remove(TopText topText) {
            instance._textPool.Release(topText);
            topText.gameObject.SetActive(false);
            if (instance._activeTopTexts.Contains(topText)) {
                instance._activeTopTexts.Remove(topText);
            }
            if (instance._activeTopTexts.Count == 0 ) {
                instance.topTextCanvasTransform.gameObject.SetActive(false);
            }
        }
    }
}
