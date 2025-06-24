using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : NetworkBehaviour {
    
    [SerializeField] private Camera cam;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public bool allowSprint = true;

    private Rigidbody _rb;
    private Vector3 _inputDirection;
    private bool _isSprinting;

    private void Awake() {
        if (_rb != null) {
            Debug.LogError("Rigidbody already assigned.");
            return;
        }
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            // Disable the camera for non-owners
            if (cam != null) {
                cam.gameObject.SetActive(false);
            }
        } 
    }

    private void Update()
    {
        if (!IsOwner)
        {
            // If this player is not the owner, skip input processing
            return;
        }
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
        if (!IsOwner)
        {
            // If this player is not the owner, skip movement
            return;
        }
        // Determine final movement speed
        float currentSpeed = _isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // Move using Rigidbody.MovePosition
        Vector3 targetPosition = _inputDirection * currentSpeed * Time.fixedDeltaTime;
        _rb.velocity = new Vector3(targetPosition.x, _rb.velocity.y, targetPosition.z);
    }

    public void OpenJumpscareScene() {
        FindAnyObjectByType<GameManager>().LoadJumpscareScene();
    }
}