using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    
    public override void OnNetworkSpawn() {
        if (IsOwner) {
            SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
        } 
    }

    private void OnGUI() {
        bool hostButton = GUILayout.Button("Host");
        bool clientButton = GUILayout.Button("Client");
        bool disconnectButton = GUILayout.Button("Disconnect");
        
        if (hostButton) {
            NetworkManager.Singleton.StartHost();
        }
        if (clientButton) {
            NetworkManager.Singleton.StartClient();
        }
        if (disconnectButton) {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
