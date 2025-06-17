using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Core.Hooks {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalStringTextSetter : MonoBehaviour {
        [SerializeField] private LocalizedString localizedString;
        
        [Foldout("Formats")]
        [SerializeField] private string insertBefore;
        [Foldout("Formats")]
        [SerializeField] private string appendLast;

        private void Awake() {
            var textMesh = GetComponent<TextMeshProUGUI>();

            if (localizedString.IsEmpty) {
                Debug.Log(gameObject.name + " localization reference is empty!");
                return;
            }
            
            localizedString.StringChanged += value => {
                textMesh.text = insertBefore + value + appendLast;
            };
        }
    }
}