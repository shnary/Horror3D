using System.IO;
using UnityEngine;

namespace Core.Utils {
    [RequireComponent(typeof(Camera))]
    public class ScreenshotHandler : MonoBehaviour {
        public static ScreenshotHandler Instance {get; private set;}

        private Camera _camera;

        private Sprite _sprite;
        private Texture2D _texture;

        private bool _takeScreenshotNextFrame;
        private bool _saveFile;

        private string _savePath;


        private void Awake() {
            Instance = this;
            _camera = GetComponent<Camera>();
        }

        private void OnPostRender() {
            if (_takeScreenshotNextFrame) {
                _takeScreenshotNextFrame = false;
                RenderTexture renderTexture = _camera.targetTexture;

                Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
                renderResult.ReadPixels(rect, 0, 0);
                renderResult.Apply();

                _texture = renderResult;
                _sprite = Sprite.Create(renderResult, rect, new Vector2(0.5f, 0.5f), 100);

                if (_saveFile) {
                    byte[] byteArray = renderResult.EncodeToPNG();
                    File.WriteAllBytes(_savePath, byteArray);
                }

                RenderTexture.ReleaseTemporary(renderTexture);
                _camera.targetTexture = null;
            }
        }

        public Texture2D GetTexture() {
            return _texture;
        }

        public Sprite GetSprite() {
            return _sprite;
        }

        public void TakeScreenshot(int width, int height) {
            _takeScreenshotNextFrame = true;
            _saveFile = false;
            _camera.targetTexture = RenderTexture.GetTemporary(width, height, 24);
        }

        public void TakeScreenshot(int width, int height, string savePath) {
            _takeScreenshotNextFrame = true;
            _saveFile = true;
            _savePath = savePath;
            _camera.targetTexture = RenderTexture.GetTemporary(width, height, 24);
        }
    }
}