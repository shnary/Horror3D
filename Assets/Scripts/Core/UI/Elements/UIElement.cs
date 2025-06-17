using System;
using UnityEngine;

namespace Core.UI.Elements {
    public class UIElement : MonoBehaviour {
        [SerializeField] protected string uiKey;
        [SerializeField] private GameObject objectActivate;
        
        private void Start() {
            var uiObject = new UIObject { element = this, objectActivated = objectActivate };
            UIManager.Instance.RegisterUIObject(uiKey, uiObject);
        }

        private void OnDestroy() {
            UIManager.Instance.DeRegisterUIObject(uiKey);
        }
    }
}