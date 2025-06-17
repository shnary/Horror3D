using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Core.Editors {
    public static class ScreenshotTakerEditor {
        [MenuItem("Tools/Take Screenshot")]
        public static void CaptureScreenshot() {
            if (Directory.Exists(Application.dataPath + "/Screenshots") == false) {
                Directory.CreateDirectory(Application.dataPath + "/Screenshots");
            }
            int fileIndex = 0;
            while (File.Exists(Application.dataPath + "/Screenshots/" + $"screenshot_{fileIndex}.png")) {
                fileIndex += 1;
            }
            ScreenCapture.CaptureScreenshot(Application.dataPath + $"/Screenshots/" + $"screenshot_{fileIndex}.png");
        }
    }
}
