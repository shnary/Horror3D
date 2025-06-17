using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Hooks {
    public class PersistentObjects : MonoBehaviour {
        private static PersistentObjects instance;

        public static PersistentObjects Instance => instance;

        private void Awake() {
            if (instance == null) {
                instance = this;
            }       
            else {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void Add(GameObject obj) {
            obj.transform.SetParent(transform);
        }
        
        public void Add(Transform obj) {
            obj.SetParent(transform);
        }

        public void Remove(GameObject obj, Transform newParent=null) {
            var child = transform.Find(obj.name);
            if (child != null) {
                child.SetParent(newParent);
            }
        }
        
        public void Remove(Transform obj, Transform newParent=null) {
            var child = transform.Find(obj.name);
            if (child != null) {
                child.SetParent(newParent);
            }
        }

        public void Remove(string objName, Transform newParent=null) {
            var child = transform.Find(objName);
            if (child != null) {
                child.SetParent(newParent);
            }
        }
    }
}
