using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

public static class Extensions {
    public static void DeactivateNextFrame(this GameObject g) {
        FunctionUpdater.Create(() => {
            g.SetActive(false);
            return true;
        });
        return;
    }

    public static void DestroyChildren(this Transform parent) {
        MainUtils.DestroyChildren(parent);
    }

    public static void ForEach(this Transform parent, Action<Transform> getChildAction) {
        foreach (Transform child in parent) {
            getChildAction?.Invoke(child);
        }
    }
    
    public static string KFormat(this float value) {
        return MainUtils.KFormatter(value);
    }
    
    public static string KFormat(this int value) {
        return MainUtils.KFormatter(value);
    }
    
    public static Vector3 SetX(this Vector3 vector, float x) {
        return new Vector3(x, vector.y);
    }
    
    public static Vector3 SetY(this Vector3 vector, float y) {
        return new Vector3(vector.x, y);
    }
    
    public static Vector3 SetZ(this Vector3 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }
    
    public static Vector3 SetXY(this Vector3 vector, float x, float y) {
        return new Vector3(x, y, vector.z);
    }
    
    public static Vector3 SetXZ(this Vector3 vector, float x, float z) {
        return new Vector3(x, vector.y, z);
    }
    
    public static Vector3 SetYZ(this Vector3 vector, float y, float z) {
        return new Vector3(vector.x, y, z);
    }
}