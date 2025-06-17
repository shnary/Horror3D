using UnityEngine;

namespace Core.UI.Elements {
    public class UISceneElement : UIElement {
        private void Awake() {
            var uiObject = new UIObject { element = this };
            UIManager.Instance.RegisterScene(uiKey, uiObject);
        }

        private void OnValidate() {
            #if UNITY_EDITOR
            var sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            uiKey = sceneName;
            #endif
        }

        private void OnDestroy() {
            UIManager.Instance.DeRegisterScene(uiKey);
        }
    }
}