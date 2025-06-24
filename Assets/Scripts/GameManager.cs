using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

    public const int TotalPages = 5;
    
    public static GameManager Instance { get; private set; }

    public NetworkVariable<int> CollectedPages = new NetworkVariable<int>(0);
    private string _host = "192.168.1.108";
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } 
    }

    private void Start() {
#if !UNITY_EDITOR
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    private void Update() {
#if !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
#endif
    }

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
            
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
    
    public void LoadJumpscareScene() {
        // NetworkManager.Singleton.SceneManager.LoadScene("Jumpscare", LoadSceneMode.Single);
        SceneManager.LoadScene("Jumpscare", LoadSceneMode.Single);
    }
    
    public void CollectPage() {
        CollectedPages.Value++;
        if (CollectedPages.Value >= TotalPages) {
            var slender = FindAnyObjectByType<Slender>();
            if (slender != null) {
                slender.GetComponent<NetworkObject>().Despawn(false);
                slender.gameObject.SetActive(false);
            }
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
