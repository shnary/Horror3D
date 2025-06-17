using System;
using Cinemachine;
using Core.Systems;
using UnityEngine;


namespace Core.Controllers {
    public class CameraSwipe : MonoBehaviour {

        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Collider2D confineArea;

        [Header("Values")]
        [SerializeField] private float swipeSpeedPc = 5f;
        [SerializeField] private float swipeSpeedMobile = 5f;

        private Vector2 _maxDistance;
        private Vector2 _minDistance;
        private Vector3 _dragOriginVector;
        
        public float SwipeSpeedPc { get => swipeSpeedPc; set => swipeSpeedPc = value; }
        public float SwipeSpeedMobile { get => swipeSpeedMobile; set => swipeSpeedMobile = value; }
        
        private void Start() {
            InputManager.Instance.OnSwipe += InputManager_OnSwipe;
            InputManager.Instance.OnZoom += InputManager_OnZoom;
        }

        private void OnDestroy() {
            InputManager.Instance.OnSwipe -= InputManager_OnSwipe;
            InputManager.Instance.OnZoom -= InputManager_OnZoom;
        }

        private void InputManager_OnSwipe(object sender, InputManager.InputEventArgs e) {

            Vector3 pos = virtualCamera.transform.position;
            float speed = e.isTouch ? swipeSpeedMobile : swipeSpeedPc;
            
            var minDistance = GetMin();
            var maxDistance = GetMax();

            if (pos.x > minDistance.x || pos.x < maxDistance.x || pos.y > minDistance.y || pos.y < maxDistance.y) {
                pos += e.swipeVector * (Time.unscaledDeltaTime * speed);// * virtualCamera.m_Lens.OrthographicSize);
                pos.z = -10;
            }
            
            virtualCamera.transform.position = ConfineMovement(pos, minDistance, maxDistance);
        }

        private void InputManager_OnZoom(object sender, InputManager.InputEventArgs e) {

            var pos = virtualCamera.transform.position;
            var minDistance = GetMin();
            var maxDistance = GetMax();
            
            virtualCamera.transform.position = ConfineMovement(pos, minDistance, maxDistance);
        }

        private Vector3 ConfineMovement(Vector3 pos, Vector3 minDistance, Vector3 maxDistance) {
            pos.x = Mathf.Clamp(pos.x, minDistance.x, maxDistance.x);
            pos.y = Mathf.Clamp(pos.y, minDistance.y, maxDistance.y);
            
            return pos;
        }

        public void SetVirtualCamera(CinemachineVirtualCamera virtualCamera) {
            this.virtualCamera = virtualCamera;
        }

        public Vector3 GetPosition() {
            return virtualCamera.transform.position;
        }

        public void FlyTo(Vector3 position) {
            virtualCamera.ForceCameraPosition(new Vector3(position.x, position.y, -10), Quaternion.identity);
        }

        public void FollowTarget(Transform targetTranform) {
            virtualCamera.Follow = targetTranform;
        }

        private Vector2 GetMin() {
            var bounds = confineArea.bounds;

            var verticalExtent = virtualCamera.m_Lens.OrthographicSize;
            var horizontalExtent = verticalExtent * Screen.width / Screen.height;

            _minDistance.x = horizontalExtent - bounds.max.x;
            _minDistance.y = verticalExtent - bounds.max.y;

            return _minDistance;
        }

        private Vector2 GetMax() {
            var bounds = confineArea.bounds;
            
            var verticalExtent = virtualCamera.m_Lens.OrthographicSize;
            var horizontalExtent = verticalExtent * Screen.width / Screen.height;
            
            _maxDistance.x = bounds.max.x - horizontalExtent;
            _maxDistance.y = bounds.max.y - verticalExtent;

            return _maxDistance;
        }
    }
}