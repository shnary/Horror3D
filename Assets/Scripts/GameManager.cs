using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public const int TotalPages = 5;
    
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    public NetworkVariable<int> CollectedPages = new NetworkVariable<int>(0);
    
    public AudioClip pageCollectedSound;
    public AudioClip flashlightSound;
    
    private string _host = "127.0.0.1"; // "192.168.1.108";
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } 
        
        hostButton.onClick.AddListener(() => {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport != null) {
                try {
                    transport.SetConnectionData(ipInputField.text, 7777);
                }
                catch (Exception e) {
                    Debug.LogError($"Failed to set connection data: {e.Message}");
                    return;
                }
            }

            try {
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception e) {
                Debug.LogError($"Failed to start host: {e.Message}");
            }
        });
        
        clientButton.onClick.AddListener(() => {
            try {
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception e) {
                Debug.LogError($"Failed to start client: {e.Message}");
            }
        });
        
        // NetworkManager.Singleton.Shutdown();
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
            mainMenuPanel.SetActive(false);
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
        SceneManager.LoadScene("Jumpscare", LoadSceneMode.Additive);
        Invoke(nameof(RemoveJumpscareScene), 3f);
    }

    private void RemoveJumpscareScene() {
        SceneManager.UnloadSceneAsync("Jumpscare");
    }
    
    public void CollectPage() {
        CollectedPages.Value++;
        PlayPageCollectedSoundServerRpc();
        if (CollectedPages.Value >= TotalPages) {
            var slender = FindAnyObjectByType<Slender>();
            if (slender != null) {
                slender.GetComponent<NetworkObject>().Despawn(false);
                slender.gameObject.SetActive(false);
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PlayPageCollectedSoundServerRpc() {
        if (pageCollectedSound != null) {
            AudioSource.PlayClipAtPoint(pageCollectedSound, Camera.main.transform.position);
        }
    }

}
