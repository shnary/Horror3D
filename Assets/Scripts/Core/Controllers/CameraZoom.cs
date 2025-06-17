using System;
using Cinemachine;
using Core.Systems;
using UnityEngine;

namespace Core.Controllers {
    public class CameraZoom : MonoBehaviour {
        
        [SerializeField] private CinemachineVirtualCamera targetVirtualCamera;

        [Header("Values")]
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;
        [SerializeField] private float zoomSpeedPc = 5f;
        [SerializeField] private float zoomSpeedMobile = 0.01f;
        
        public float MinZoom { get => minZoom; set => minZoom = value; }
        public float MaxZoom { get => maxZoom; set => maxZoom = value; }
        public float ZoomSpeedPc { get => zoomSpeedPc; set => zoomSpeedPc = value; }
        public float ZoomSpeedMobile { get => zoomSpeedMobile; set => zoomSpeedMobile = value; }
        
        private void Start() {
            InputManager.Instance.OnZoom += InputManager_OnZoom;
        }

        private void OnDestroy() {
            InputManager.Instance.OnZoom -= InputManager_OnZoom;
        }

        private void InputManager_OnZoom(object sender, InputManager.InputEventArgs e) {
            Zoom(e.zoomDeltaDistance, e.isTouch);
        }

        public void SetVirtualCamera(CinemachineVirtualCamera virtualCamera) {
            this.targetVirtualCamera = virtualCamera;
        }

        private void Zoom(float deltaMagnitudeDiff, bool isTouch) {
            float speed = isTouch ? zoomSpeedMobile : zoomSpeedPc;
            var ortSize = targetVirtualCamera.m_Lens.OrthographicSize;
            var ortSpeed = isTouch ? ortSize : 1;
            if (ortSize < maxZoom || ortSize > minZoom) {
                targetVirtualCamera.m_Lens.OrthographicSize += deltaMagnitudeDiff * speed * ortSpeed;
            }
            
            // set min and max value of Clamp function upon your requirement
            targetVirtualCamera.m_Lens.OrthographicSize =
                Mathf.Clamp(targetVirtualCamera.m_Lens.OrthographicSize, minZoom, maxZoom);
        }

    }
}