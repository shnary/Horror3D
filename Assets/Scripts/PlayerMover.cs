using System;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    
    //3D movement
    public float speed = 5f;

    private Rigidbody _rb;
    private Vector3 _movement;
    private float _initialSpeed;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _initialSpeed = speed;
    }

    private void Update() {
        // Get input from the user
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)) {
            speed = _initialSpeed * 2; // Double the speed when holding left shift
        }
        else {
            speed = _initialSpeed; // Reset to initial speed when not holding left shift
        }

        // Normalize the vector to ensure consistent speed in all directions
        _movement = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;        // Move the player
    }

    void FixedUpdate() {
        
        _rb.MovePosition(transform.position + _movement * speed * Time.deltaTime);
    }
}