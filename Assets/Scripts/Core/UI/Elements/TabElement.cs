using System;
using Core.UI.Groups;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core.UI.Elements {
    public class TabElement : MonoBehaviour {
        [SerializeField] private Button button;   
        [SerializeField] private bool selectFirst;

        [Header("Group")]
        [SerializeField] private TabGroup tabGroup;

        [Header("Page")]
        [SerializeField] private GameObject page;

        [Header("Events")]
        [SerializeField] private UnityEvent onTabSelect;
        [SerializeField] private UnityEvent onTabDeselect;

        public event EventHandler OnActivatedFirstTime;
        public event EventHandler OnActivated;

        private bool _activatedFirstTime;

        private void Awake() {
            tabGroup.Register(this);
            button.onClick.AddListener(() => {
                tabGroup.Select(this);
            });
        }

        private void Start() {
            if (selectFirst) {
                tabGroup.Select(this);
            }
        }

        public void Select() {
            page.SetActive(true);
            onTabSelect?.Invoke();
            if (_activatedFirstTime == false) {
                _activatedFirstTime = true;
                OnActivatedFirstTime?.Invoke(this, EventArgs.Empty);
            }
            OnActivated?.Invoke(this, EventArgs.Empty);
        }

        public void Deselect() {
            page.SetActive(false);
            onTabDeselect?.Invoke();
        }
    }
}