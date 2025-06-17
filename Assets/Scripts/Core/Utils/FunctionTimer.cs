using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.Utils {
    public class FunctionTimer {

        /*
        * Class to hook Actions into MonoBehaviour
        * */
        private class MonoBehaviourHook : MonoBehaviour {

            public Action OnUpdate;

            private void Update() {
                if (OnUpdate != null) OnUpdate();
            }

        }


        private static List<FunctionTimer> timerList; // Holds a reference to all active timers
        private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded() {
            if (initGameObject == null) {
                initGameObject = new GameObject("FunctionTimer_Global");
                timerList = new List<FunctionTimer>();
            }
        }




        public static FunctionTimer Create(Action action, float timer) {
            return Create(action, timer, "", false, false);
        }

        public static FunctionTimer Create(Action action, float timer, string functionName) {
            return Create(action, timer, functionName, false, false);
        }

        public static FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime) {
            return Create(action, timer, functionName, useUnscaledDeltaTime, false);
        }

        public static FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime, bool stopAllWithSameName) {
            InitIfNeeded();

            if (stopAllWithSameName) {
                StopAllTimersWithName(functionName);
            }

            GameObject obj = new GameObject("FunctionTimer Object "+functionName, typeof(MonoBehaviourHook));
            FunctionTimer funcTimer = new FunctionTimer(obj, action, timer, functionName, useUnscaledDeltaTime);
            obj.GetComponent<MonoBehaviourHook>().OnUpdate = funcTimer.Update;

            timerList.Add(funcTimer);

            return funcTimer;
        }

        public static void RemoveTimer(FunctionTimer funcTimer) {
            InitIfNeeded();
            timerList.Remove(funcTimer);
        }

        public static void StopAllTimersWithName(string functionName) {
            InitIfNeeded();
            for (int i = 0; i < timerList.Count; i++) {
                if (timerList[i].m_functionName == functionName) {
                    timerList[i].DestroySelf();
                    i--;
                }
            }
        }

        public static void StopFirstTimerWithName(string functionName) {
            InitIfNeeded();
            for (int i = 0; i < timerList.Count; i++) {
                if (timerList[i].m_functionName == functionName) {
                    timerList[i].DestroySelf();
                    return;
                }
            }
        }





        private GameObject m_gameObject;
        private float m_timer;
        private string m_functionName;
        private bool m_active;
        private bool m_useUnscaledDeltaTime;
        private Action m_action;



        public FunctionTimer(GameObject gameObject, Action action, float timer, string functionName, bool useUnscaledDeltaTime) {
            this.m_gameObject = gameObject;
            this.m_action = action;
            this.m_timer = timer;
            this.m_functionName = functionName;
            this.m_useUnscaledDeltaTime = useUnscaledDeltaTime;
        }

        private void Update() {
            if (m_useUnscaledDeltaTime) {
                m_timer -= Time.unscaledDeltaTime;
            } else {
                m_timer -= Time.deltaTime;
            }
            if (m_timer <= 0) {
                // Timer complete, trigger Action
                m_action();
                DestroySelf();
            }
        }

        private void DestroySelf() {
            RemoveTimer(this);
            if (m_gameObject != null) {
                UnityEngine.Object.Destroy(m_gameObject);
            }
        }




        /*
        * Class to trigger Actions manually without creating a GameObject
        * */
        public class FunctionTimerObject {

            private float m_timer;
            private Action m_callback;

            public FunctionTimerObject(Action callback, float timer) {
                this.m_callback = callback;
                this.m_timer = timer;
            }

            public bool Update() {
                return Update(Time.deltaTime);
            }

            public bool Update(float deltaTime) {
                m_timer -= deltaTime;
                if (m_timer <= 0) {
                    m_callback();
                    return true;
                } else {
                    return false;
                }
            }
        }

        // Create a Object that must be manually updated through Update();
        public static FunctionTimerObject CreateObject(Action callback, float timer) {
            return new FunctionTimerObject(callback, timer);
        }

    }
}