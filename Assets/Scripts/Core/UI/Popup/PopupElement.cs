using System;
using UnityEngine;

namespace Core.Popup {
    public abstract class PopupElement : MonoBehaviour {
        private void Awake() {
            gameObject.SetActive(false);
        }

        public abstract void Show();
        public abstract void Hide();
    }
}