using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
    
    public override void OnNetworkSpawn() {
        if (IsOwner) {
            GetComponent<PlayerInteraction>().OnInteract += HandleInteraction;
        }
    }
    
    private void HandleInteraction(GameObject obj) {
        if (obj == null) {
            return; // No object to interact with
        }
        if (obj.transform.parent == null) {
            return;
        }
        if (obj.transform.parent.TryGetComponent<Page>(out var page)) {
            page.Collect();
        } 
    }
    
}