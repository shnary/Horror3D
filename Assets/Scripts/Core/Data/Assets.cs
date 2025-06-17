using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.Data {
    public static class Assets {

        private static Dictionary<Type, IAssetDb> Databases = new Dictionary<Type, IAssetDb>();

        public static void SetupDatabase(IEnumerable<IAssetDb> assetDatabases) {
            foreach (var database in assetDatabases) {
                Databases[database.GenericType] = database;
            }
        }

        public static AssetDb<T> FromDb<T>() where T : class {
            if (Databases.ContainsKey(typeof(T)) == false) {
                Debug.LogError($"{typeof(T)} database could not be found, returning null.");
                return null;
            }
            return Databases[typeof(T)] as AssetDb<T>;
        }

    }
}