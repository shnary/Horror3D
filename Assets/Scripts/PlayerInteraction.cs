using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour {
    
    public static event Action OnInteract;

    private void Update() {
        if (!IsOwner) {
            return; // Only the owner of the player can interact
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
    }
    
    private void Interact() {
        OnInteract?.Invoke();
        Debug.Log("Interacted with object");
    }
}