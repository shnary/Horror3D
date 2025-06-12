using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {
    
    public static event Action OnInteract;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
    }
    
    private void Interact() {
        OnInteract?.Invoke();
        Debug.Log("Interacted with object");
    }
}