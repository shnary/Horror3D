using System;
using Core.Systems;
using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Popup {
    public enum BoxButtonType {
        Apply=0,
        Cancel=1,
        Exit=2,
        Special=3,
    }
    public abstract class PopupBox : PopupElement {
        [Serializable]
        public class PopupButton {
            public BoxButtonType type;
            public Button button;
        }

        public Action opened;
        public Action closed;
        
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI descriptionText;
        [SerializeField] private PopupButton[] buttons;

        private bool _customClicks;
        
        public bool IsActive { get; private set; }

        public PopupBox WithTitle(string localString) {
            titleText.text = localString;
            return this;
        }

        public PopupBox WithDesc(string localString) {
            descriptionText.text = localString;
            return this;
        }

        public PopupBox WithClick(Action<PopupBox, BoxButtonType> onClick) {
            _customClicks = true;
            foreach (var button in buttons) {
                button.button.onClick.RemoveAllListeners();
                button.button.onClick.AddListener(() => {
                    AudioManager.Play("", "click1");
                    onClick?.Invoke(this, button.type);
                });
            }

            return this;
        }

        public override void Show() {
            if (_customClicks == false) {
                foreach (var button in buttons) {
                    button.button.onClick.RemoveAllListeners();
                    button.button.onClick.AddListener(() => {
                        AudioManager.Play("", "click1");
                        Hide();
                    });
                }
            }

            IsActive = true;
            transform.SetSiblingIndex(0);
            Popups.Add(this);
            opened?.Invoke();
            // TODO: check tween on or off
        }

        public override void Hide() {
            IsActive = false;
            MainUtils.TriggerActionNextFrame(() => {
                closed?.Invoke();
            });
            Popups.Remove(this);
            // TODO: check tween on or off
        }
    }
}