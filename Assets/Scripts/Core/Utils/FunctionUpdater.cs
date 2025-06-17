using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.Utils {
    public class FunctionUpdater {

        /*
        * Class to hook Actions into MonoBehaviour
        * */
        private class MonoBehaviourHook : MonoBehaviour {

            public Action OnUpdate;

            private void Update() {
                if (OnUpdate != null) OnUpdate();
            }

        }

        private static List<FunctionUpdater> updaterList; // Holds a reference to all active updaters
        private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded() {
            if (initGameObject == null) {
                initGameObject = new GameObject("FunctionUpdater_Global");
                updaterList = new List<FunctionUpdater>();
            }
        }



        
        public static FunctionUpdater Create(Action updateFunc) {
            return Create(() => { updateFunc(); return false; }, "", true, false);
        }

        public static FunctionUpdater Create(Action updateFunc, string functionName) {
            return Create(() => { updateFunc(); return false; }, functionName, true, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc) {
            return Create(updateFunc, "", true, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc, string functionName) {
            return Create(updateFunc, functionName, true, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active) {
            return Create(updateFunc, functionName, active, false);
        }

        public static FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active, bool stopAllWithSameName) {
            InitIfNeeded();

            if (stopAllWithSameName) {
                StopAllUpdatersWithName(functionName);
            }

            GameObject gameObject = new GameObject("FunctionUpdater Object " + functionName, typeof(MonoBehaviourHook));
            FunctionUpdater functionUpdater = new FunctionUpdater(gameObject, updateFunc, functionName, active);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionUpdater.Update;

            updaterList.Add(functionUpdater);
            return functionUpdater;
        }

        private static void RemoveUpdater(FunctionUpdater funcUpdater) {
            InitIfNeeded();
            updaterList.Remove(funcUpdater);
        }

        public static void DestroyUpdater(FunctionUpdater funcUpdater) {
            InitIfNeeded();
            if (funcUpdater != null) {
                funcUpdater.DestroySelf();
            }
        }

        public static void StopUpdaterWithName(string functionName) {
            InitIfNeeded();
            for (int i = 0; i < updaterList.Count; i++) {
                if (updaterList[i].m_functionName == functionName) {
                    updaterList[i].DestroySelf();
                    return;
                }
            }
        }

        public static void StopAllUpdatersWithName(string functionName) {
            InitIfNeeded();
            for (int i = 0; i < updaterList.Count; i++) {
                if (updaterList[i].m_functionName == functionName) {
                    updaterList[i].DestroySelf();
                    i--;
                }
            }
        }





        private GameObject m_gameObject;
        private string m_functionName;
        private bool m_active;
        private Func<bool> m_updateFunc; // Destroy Updater if return true;

        public FunctionUpdater(GameObject gameObject, Func<bool> updateFunc, string functionName, bool active) {
            this.m_gameObject = gameObject;
            this.m_updateFunc = updateFunc;
            this.m_functionName = functionName;
            this.m_active = active;
        }

        public void Pause() {
            m_active = false;
        }

        public void Resume() {
            m_active = true;
        }

        private void Update() {
            if (!m_active) return;
            if (m_updateFunc()) {
                DestroySelf();
            }
        }

        public void DestroySelf() {
            RemoveUpdater(this);
            if (m_gameObject != null) {
                UnityEngine.Object.Destroy(m_gameObject);
            }
        }

    }
}