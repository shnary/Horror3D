using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour {
    
    public event Action<GameObject> OnInteract;

    private void Update() {
        if (!IsOwner) {
            return; // Only the owner of the player can interact
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
    }
    
    private void Interact() {
        var cam = GetComponent<PlayerLook>().playerCamera;
        bool found = Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 5);
        if (!found) {
            return; // No object to interact with
        }
        print(hit.collider.gameObject.name);
        OnInteract?.Invoke(hit.collider.gameObject);
    }
}