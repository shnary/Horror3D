using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameUI : NetworkBehaviour {

    [SerializeField] private TMP_Text fpsTMP;
    [SerializeField] private TMP_Text logTMP;

    public override void OnNetworkSpawn() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        GameManager.Instance.CollectedPages.OnValueChanged += GameManager_CollectedPagesChanged;
    }

    private void OnDisable() {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        GameManager.Instance.CollectedPages.OnValueChanged -= GameManager_CollectedPagesChanged;
    }

    private void OnClientConnected(ulong clientId) {
        Add($"Client <color=red>{clientId}</color> connected.");
    }
    
    private void OnClientDisconnected(ulong clientId) {
        Add($"Client <color=red>{clientId}</color> disconnected.");
    }
    
    private void GameManager_CollectedPagesChanged(int previousValue, int newValue) {
        Add($"Collected pages: {newValue}");
        if (newValue >= GameManager.TotalPages) {
            Add("All pages collected! You have won!");
        }
    }

    public void Add(string msg) {
        logTMP.text += msg + "\n";
    }
    
    public void Clear() {
        logTMP.text = "";
    }
    
    private void Update() {
        if (fpsTMP != null) {
            fpsTMP.text = $"FPS: {Mathf.Round(1f / Time.deltaTime)}";
        }
    }
    
}