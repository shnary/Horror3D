using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Testing : NetworkBehaviour {

    [SerializeField] private TMP_Text text;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            text.text = "Server";
        }
        else if (IsClient) {
            text.text = "Client";
        }
        else {
            text.text = "Unknown";
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            SetTextToRandomIntServerRPC();
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTextToRandomIntServerRPC() {
        int randomInt = UnityEngine.Random.Range(0, 100);
        text.text = "Random Int: " + randomInt;
        SetTextToRandomIntClientRPC(randomInt);
    }
    
    [ClientRpc]
    public void SetTextToRandomIntClientRPC(int randomInt) {
        text.text = "Random Int: " + randomInt;
    }
}