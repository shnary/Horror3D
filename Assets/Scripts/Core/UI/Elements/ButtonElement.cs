using Core.UI.Groups;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core.UI.Elements {
    public class ButtonElement : MonoBehaviour {
        [SerializeField] private Button button;
        [SerializeField] private bool selectFirst;

        [Header("Group")]
        [SerializeField] private ButtonGroup buttonGroup;       

        [Header("Events")]
        [SerializeField] private UnityEvent onSelect;
        [SerializeField] private UnityEvent onDeselect;

        private void Awake() {
            buttonGroup.Register(this);
            button.onClick.AddListener(() => {
                buttonGroup.Select(this);
            });
        }

        private void Start() {
            if (selectFirst) {
                buttonGroup.Select(this);
            }
        }

        public void Select() {
            onSelect?.Invoke();
        }

        public void Deselect() {
            onDeselect?.Invoke();
        }
    }
}