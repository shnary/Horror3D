using UnityEngine;

namespace Core.Utils {
    [RequireComponent(typeof(Camera))]
    public class TVCamera : MonoBehaviour {

        private Camera _camera;
        private RenderTexture _renderTexture;

        public RenderTexture Result => _renderTexture;

        private void Awake() {
            _camera = GetComponent<Camera>();
        }

        public Camera GetCamera() {
            return _camera;
        }

        public void SetupRenderTexture(int width, int height, int depthBuffer=24) {
            // RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, depthBuffer);
            RenderTexture renderTexture = new RenderTexture(width, height, depthBuffer);
            _renderTexture = renderTexture;
            _camera.targetTexture = renderTexture;
        }

        private void OnDestroy() {
            if (_renderTexture != null) {
                _camera.targetTexture = null;
                _renderTexture.Release();
                // RenderTexture.ReleaseTemporary(_renderTexture);
            }
        }
    }
}