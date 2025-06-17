using System;
using Core.Systems;
using Core.Data;
using Core.Plains;
using Core.Utils;
using UnityEngine;

namespace Core.Utils {
    public class VideoTV : MonoBehaviour {
        [SerializeField] private TVCamera tvCamera;

        private string scene;

        public static void Load(string sceneName, int width, int height, Action<TVCamera> tv) {
            if (SceneLoader.CanAppend(sceneName)) {
                SceneLoader.Append(sceneName, () => {
                    var obj = FindObjectOfType<VideoTV>();
                    obj.scene = sceneName;
                    obj.Setup(width, height);
                    tv?.Invoke(obj.tvCamera);
                });
            }
        }
        
        public void Setup(int width, int height) {
            tvCamera.SetupRenderTexture(width, height);
        }

        public void Remove() {
            if (string.IsNullOrEmpty(scene) == false) {
                SceneLoader.Remove(scene);
            }
        }
    }
}