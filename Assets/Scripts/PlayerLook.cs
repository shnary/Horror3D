using System;
using UnityEngine;

public class PlayerLook : MonoBehaviour {

    public static event Action<GameObject> OnSawObject;
    
    public Transform playerBody;
    public Camera playerCamera;
    
    public float rayDistance = 20f;
    public float lookSpeed = 2f;

    private float _xRotation = 0f;

    void Update() {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Rotate the camera vertically
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); // Clamp the vertical rotation

        // Apply the rotation to the camera
        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        
        // Rotate the player body horizontally
        playerBody.Rotate(Vector3.up * mouseX);
        
        bool found = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, rayDistance);
        // If nothing is hit, invoke with null
        // If the raycast hits an object, invoke the OnSawObject event
        OnSawObject?.Invoke(found ? hit.collider.gameObject : null);
    }
    
}