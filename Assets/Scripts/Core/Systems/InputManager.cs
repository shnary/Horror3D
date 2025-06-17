using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Core.Systems {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }

        public class InputEventArgs : EventArgs {
            public Vector3 swipeVector;
            public Vector3 clickPosition;
            public Vector3 clickPositionWorld;
            public float zoomDeltaDistance;
            public bool isTouch;
        }

        public event EventHandler<InputEventArgs> OnClickDown;
        public event EventHandler<InputEventArgs> OnClickUp;
        public event EventHandler<InputEventArgs> OnDoubleClick;
        public event EventHandler<InputEventArgs> OnSwipe;
        public event EventHandler<InputEventArgs> OnZoom;

        [SerializeField] private float doubleClickTime = 0.2f;
        [SerializeField] private float swipeActivateMinDistance = 0.1f;
        [SerializeField] private float zoomActivateMinDistance = 1f;
        
        private Vector3 _dragOriginVector;
        private InputEventArgs _inputArgs;
        private float _lastClickTime;

        public float DoubleCLickTime { get => doubleClickTime; set => doubleClickTime = value; }
        public float SwipeActivateMinDistance { get => swipeActivateMinDistance; set => swipeActivateMinDistance = value; }
        public float ZoomActivateActivateMinDistance { get => zoomActivateMinDistance; set => zoomActivateMinDistance = value; }
        
        public bool InputsLocked { get; set; }
        public bool SwipeLocked { get; set; }
        public bool ZoomLocked { get; set; }

        private void Awake() {
            Instance = this;
            _inputArgs = new InputEventArgs();
            _inputArgs.isTouch = Input.touchSupported;
        }

        private void Update() {
            if (InputsLocked) {
                return;
            }
            if (Input.touchSupported == false || Input.touchCount == 1) {
                HandleSingleFinger();
            }

            if (!ZoomLocked) {
                HandleZoom();
            }
        }

        private void HandleSingleFinger() {
            if (Input.GetMouseButtonDown(0)) {
                if (MainUtils.IsPointerOverUIObject()) {
                    _dragOriginVector = Vector3.zero;
                    return;
                }

                var mouseWorldPosition = MainUtils.GetMouseWorldPosition();
                _dragOriginVector = mouseWorldPosition;
                _inputArgs.clickPosition = Input.mousePosition;
                _inputArgs.clickPositionWorld = mouseWorldPosition;
                OnClickDown?.Invoke(this, _inputArgs);
            }

            if (!SwipeLocked && Input.GetMouseButton(0)) {
                
                if (_dragOriginVector != Vector3.zero) {
                    HandleSwipe();
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                _dragOriginVector = Vector3.zero;
                _inputArgs.clickPosition = Input.mousePosition;
                _inputArgs.clickPositionWorld = MainUtils.GetMouseWorldPosition();
                
                float timeSinceLastClick = Time.time - _lastClickTime;

                if (timeSinceLastClick <= doubleClickTime) {
                    OnDoubleClick?.Invoke(this, _inputArgs);   
                }
                _lastClickTime = Time.time;
                
                OnClickUp?.Invoke(this, _inputArgs);
            }
        }
        
        private void HandleZoom() {
            if (Input.touchSupported) {
                // Pinch to zoom
                if (Input.touchCount == 2) {
                    // get current touch positions
                    Touch tZero = Input.GetTouch(0);
                    Touch tOne = Input.GetTouch(1);
                    // get touch position from the previous frame
                    Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                    Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                    float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                    float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                    // get offset value
                    float deltaDistance = oldTouchDistance - currentTouchDistance;
                    if (Mathf.Abs(deltaDistance) > zoomActivateMinDistance) {
                        _inputArgs.zoomDeltaDistance = deltaDistance;
                        OnZoom?.Invoke(this, _inputArgs);
                    }
                }
            }
            else {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0) {
                    _inputArgs.zoomDeltaDistance = scroll;
                    OnZoom?.Invoke(this, _inputArgs);
                }
            }
        }
        
        private void HandleSwipe() {

            Vector3 mouseWorldPos = MainUtils.GetMouseWorldPosition();
            Vector3 swipeVector = _dragOriginVector - mouseWorldPos;
            Vector3 swipeVectorScreen = Camera.main.WorldToScreenPoint(_dragOriginVector) - Input.mousePosition;
            
            if (swipeVectorScreen.magnitude > swipeActivateMinDistance) {
                _inputArgs.clickPositionWorld = mouseWorldPos;
                _inputArgs.swipeVector = swipeVector;
                OnSwipe?.Invoke(this, _inputArgs);
            }
        }
    }
}
