using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public bool allowSprint = true;

    private Rigidbody _rb;
    private Vector3 _inputDirection;
    private bool _isSprinting;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Get raw movement input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        // Convert input to world-space direction
        _inputDirection = (transform.right * inputX + transform.forward * inputZ).normalized;
        // _inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0 ,Input.GetAxis("Vertical")).normalized;


        // Sprint toggle
        _isSprinting = allowSprint && Input.GetKey(KeyCode.LeftShift);
    }

    private void FixedUpdate()
    {
        // Determine final movement speed
        float currentSpeed = _isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // Move using Rigidbody.MovePosition
        Vector3 targetPosition = _inputDirection * currentSpeed * Time.fixedDeltaTime;
        _rb.velocity = new Vector3(targetPosition.x, _rb.velocity.y, targetPosition.z);
    }
}