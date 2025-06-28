using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LookAt2D : NetworkBehaviour {
    
    private Camera _mainCamera;
    
    void LateUpdate() {
        // Again, only on client
        if (!IsClient) return;
        if (_mainCamera == null) {
            _mainCamera = Camera.main; // Cache the main camera
            if (_mainCamera == null) {
                Debug.Log("Main camera not found!");
                return;
            }
        }

        // Make the sprite look at the camera
        transform.forward = _mainCamera.transform.forward;
    }
}

