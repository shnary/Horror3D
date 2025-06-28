using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Slender : NetworkBehaviour {

    public enum State {
        Walking,
        Screaming
    }

    private Animator _animator;
    private State _currentState = State.Walking;
    private float _stateTimer;
    private Vector3 _initialPosition;

    private void Awake() {
        _stateTimer = 3f;
        _animator = GetComponent<Animator>();
        if (_animator == null) {
            Debug.LogError("Animator component not found on Slender object.");
        }
    }


    public override void OnNetworkSpawn() {
        if (IsOwner) {
            var spawnLoc = GameObject.Find("SlenderSpawnLocation");
            if (spawnLoc == null) {
                Debug.LogError("SlenderSpawnLocation not found in the scene.");
                return;
            }
            _initialPosition = spawnLoc.transform.position;
            transform.position = spawnLoc.transform.position;
        }
    }

    private void Update() {
        
        if (_currentState == State.Screaming) {
            // If Slender is screaming, do not move
            
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f) {
                _currentState = State.Walking;
                _stateTimer = 3f; // Reset timer for next walking state
            }
            
            return;
        }

        var players = FindObjectsByType<PlayerMover>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var player = players.OrderBy(player => player.transform.position.z).FirstOrDefault();
        
        if (player == null) {
            _animator.Play("Idle");
            return;
        }
        
        transform.position += (player.transform.position - transform.position).normalized * Time.deltaTime * 5f;
        transform.LookAt(player.transform.position);
        if (Vector3.Distance(player.transform.position, transform.position) < 1.5f) {
            _animator.Play("Scream");
            _currentState = State.Screaming;
            player.OpenJumpscareScene();
            transform.position = _initialPosition;
        }
        else {
            _animator.Play("Walk");
            
        }
    }
}