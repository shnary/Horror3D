using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{

    public static event Action<GameObject> OnSawObject;

    public Transform playerBody;
    public Camera playerCamera;

    public float rayDistance = 20f;
    public float lookSpeed = 2f;

    private float _xRotation = 0f;

    void Start()
    {
        if (!IsOwner) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX * Time.deltaTime * 300f);

        bool found = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, rayDistance);
        OnSawObject?.Invoke(found ? hit.collider.gameObject : null);
    }
}
