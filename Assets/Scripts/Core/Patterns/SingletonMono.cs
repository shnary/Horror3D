using UnityEngine;

namespace Core.Patterns {
    public class SingletonMono<T> : MonoBehaviour where T : class {
        
        public static T Instance { get; private set; }
        
        /// <summary>
        /// After inheriting, don't forget to call base.Awake() first.
        /// </summary>
        protected virtual void Awake() {
            Instance = this as T;
        }   
    }
}