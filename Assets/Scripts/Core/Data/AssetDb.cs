using System;
using UnityEngine;

namespace Core.Data {
    public interface IAssetDb {
        Type GenericType {get;}
    }
    public class AssetDb<T> : ScriptableObject, IAssetDb where T : class {
        [Serializable]
        public class Asset {
            public string key;
            public T asset;
        }
        
        public Asset[] assets;

        public Type GenericType => typeof(T);

        public T Get(string key) {
            Asset assetFound = Array.Find(assets, t => string.Equals(t.key, key));
            if (assetFound == null) {
                Debug.LogError($"{key} could not be found. Returning null.");
                return default(T);
            }
            return assetFound.asset as T;
        }
    }
}