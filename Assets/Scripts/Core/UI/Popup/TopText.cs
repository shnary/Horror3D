using Core.Systems;
using Core.Plains;
using Core.Utils;
using TMPro;
using UnityEngine;

namespace Core.Popup {
    public class TopText : PopupElement {
        private const float TextSpeed = 5f;
        
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 anchoredPosition;

        private bool _withError;
        
        public void Show(string localString) {
            textMesh.text = localString;
            Show();
        }

        public override void Show() {
            if (textMesh == null) {
                textMesh = GetComponent<TextMeshProUGUI>();
            }

            if (_withError) {
                AudioManager.Play("", "error");
            }
            else {
                textMesh.color = Color.white;
            }
            
            Popups.Add(this);       
            Invoke(nameof(Hide), 3f);
        }

        public TopText WithError() {
            if (textMesh == null) {
                textMesh = GetComponent<TextMeshProUGUI>();
            }
            textMesh.color = Color.red;
            _withError = true;
            return this;
        }

        public override void Hide() {
            Popups.Remove(this);
            _withError = false;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        private void Update() {
            transform.position += Vector3.up * Time.deltaTime * TextSpeed;       
        }
    }
}