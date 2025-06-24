using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField] private TMP_Text objectNameTMP;
    
    private void OnEnable() {
        PlayerLook.OnSawObject += PlayerLook_OnSawObject;   
    }

    private void OnDisable() {
        PlayerLook.OnSawObject -= PlayerLook_OnSawObject;
    }

    private void PlayerLook_OnSawObject(GameObject obj) {
        #if DEBUG
        objectNameTMP.text = obj != null ? obj.name : ""; 
        #endif
    }
}