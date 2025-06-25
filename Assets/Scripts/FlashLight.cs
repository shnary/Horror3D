using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    private Light flashlightLight;

    void Start()
    {
        flashlightLight = GetComponentInChildren<Light>();
        if (flashlightLight == null)
        {
            Debug.LogError("Flashlight Toggle: Light component bulunamadý!");
        }
        else
        {
            flashlightLight.enabled = false;  // Baþlangýçta kapalý olsun
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlightLight != null)
            {
                flashlightLight.enabled = !flashlightLight.enabled;
            }
        }
    }
}
