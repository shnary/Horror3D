using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    
    public override void OnNetworkSpawn() {
        if (IsOwner) {
            SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);

            #if !DEBUG
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            #endif

            GameObject slenderPrefab = Resources.Load<GameObject>("Slender");
            if (slenderPrefab == null) {
                Debug.LogError("Slender prefab not found in Resources folder.");
                return;
            }
            Instantiate(slenderPrefab);
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
