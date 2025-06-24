using System;
using Unity.Netcode;
using UnityEngine;

public class Slender : NetworkBehaviour {

    private Animator _animator;

    private void Awake() {
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
            transform.position = spawnLoc.transform.position;
        }
    }

    private void Update() {

        var players = FindObjectsByType<PlayerMover>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        if (players.Length == 0) {
            _animator.Play("Idle");
            return;
        }
        foreach (var player in players) {

            transform.position += (player.transform.position - transform.position).normalized * Time.deltaTime * 6f;
            transform.LookAt(player.transform.position);
            if (Vector3.Distance(player.transform.position, transform.position) < 1.5f) {
                _animator.Play("Scream");
            }
            else {
                _animator.Play("Walk");
            }

        }
        

    }
}