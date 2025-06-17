using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils {
    public class FunctionPeriodic {

        /*
        * Class to hook Actions into MonoBehaviour
        * */
        private class MonoBehaviourHook : MonoBehaviour {

            public Action OnUpdate;

            private void Update() {
                if (OnUpdate != null) OnUpdate();
            }

        }


        private static List<FunctionPeriodic> funcList; // Holds a reference to all active timers
        private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded() {
            if (initGameObject == null) {
                initGameObject = new GameObject("FunctionPeriodic_Global");
                funcList = new List<FunctionPeriodic>();
            }
        }



        // Persist through scene loads
        public static FunctionPeriodic Create_Global(Action action, Func<bool> testDestroy, float timer) {
            FunctionPeriodic functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
            MonoBehaviour.DontDestroyOnLoad(functionPeriodic.m_gameObject);
            return functionPeriodic;
        }


        // Trigger [action] every [timer], execute [testDestroy] after triggering action, destroy if returns true
        public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer) {
            return Create(action, testDestroy, timer, "", false);
        }

        public static FunctionPeriodic Create(Action action, float timer) {
            return Create(action, null, timer, "", false, false, false);
        }

        public static FunctionPeriodic Create(Action action, float timer, string functionName) {
            return Create(action, null, timer, functionName, false, false, false);
        }

        public static FunctionPeriodic Create(Action callback, Func<bool> testDestroy, float timer, string functionName, bool stopAllWithSameName) {
            return Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);
        }

        public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer, string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName) {
            InitIfNeeded();

            if (stopAllWithSameName) {
                StopAllFunc(functionName);
            }

            GameObject gameObject = new GameObject("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
            FunctionPeriodic functionPeriodic = new FunctionPeriodic(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

            funcList.Add(functionPeriodic);

            if (triggerImmediately) action();

            return functionPeriodic;
        }




        public static void RemoveTimer(FunctionPeriodic funcTimer) {
            InitIfNeeded();
            funcList.Remove(funcTimer);
        }
        public static void StopTimer(string name) {
            InitIfNeeded();
            for (int i = 0; i < funcList.Count; i++) {
                if (funcList[i].m_functionName == name) {
                    funcList[i].DestroySelf();
                    return;
                }
            }
        }
        public static void StopAllFunc(string name) {
            InitIfNeeded();
            for (int i = 0; i < funcList.Count; i++) {
                if (funcList[i].m_functionName == name) {
                    funcList[i].DestroySelf();
                    i--;
                }
            }
        }
        public static bool IsFuncActive(string name) {
            InitIfNeeded();
            for (int i = 0; i < funcList.Count; i++) {
                if (funcList[i].m_functionName == name) {
                    return true;
                }
            }
            return false;
        }




        private GameObject m_gameObject;
        private float m_timer;
        private float m_baseTimer;
        private bool m_useUnscaledDeltaTime;
        private string m_functionName;
        public Action action;
        public Func<bool> testDestroy;


        private FunctionPeriodic(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime) {
            this.m_gameObject = gameObject;
            this.action = action;
            this.m_timer = timer;
            this.testDestroy = testDestroy;
            this.m_functionName = functionName;
            this.m_useUnscaledDeltaTime = useUnscaledDeltaTime;
            m_baseTimer = timer;
        }

        public void SkipTimerTo(float timer) {
            this.m_timer = timer;
        }

        public void SetBaseTimer(float baseTimer) {
            this.m_baseTimer = baseTimer;
        }

        public float GetBaseTimer() {
            return m_baseTimer;
        }

        private void Update() {
            if (m_useUnscaledDeltaTime) {
                m_timer -= Time.unscaledDeltaTime;
            } else {
                m_timer -= Time.deltaTime;
            }
            if (m_timer <= 0) {
                action();
                if (testDestroy != null && testDestroy()) {
                    //Destroy
                    DestroySelf();
                } else {
                    //Repeat
                    m_timer += m_baseTimer;
                }
            }
        }

        public void DestroySelf() {
            RemoveTimer(this);
            if (m_gameObject != null) {
                UnityEngine.Object.Destroy(m_gameObject);
            }
        }
    }
}