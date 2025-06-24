using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    private string _host = "192.168.1.108";
    
    public override void OnNetworkSpawn() {
        if (IsOwner) {
            SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);

            #if !UNITY_EDITOR
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            #endif

            GameObject slenderPrefab = Resources.Load<GameObject>("Slender");
            if (slenderPrefab == null) {
                Debug.LogError("Slender prefab not found in Resources folder.");
                return;
            }
            
            var slender = Instantiate(slenderPrefab);
            NetworkObject aiNetworkObject = slender.GetComponent<NetworkObject>();
            aiNetworkObject.Spawn();
        } 
    }

    private void OnGUI() {
        bool hostButton = GUILayout.Button("Host");
        bool clientButton = GUILayout.Button("Client");
        bool disconnectButton = GUILayout.Button("Disconnect");

        _host = GUILayout.TextArea(_host);
        
        if (hostButton) {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport != null) {
                transport.SetConnectionData(_host, 7777);
            }
            
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
