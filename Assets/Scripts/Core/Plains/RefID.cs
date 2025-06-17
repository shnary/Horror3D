using System;
using UnityEngine;
 
namespace Core.Plains {
    [Serializable]
    public struct RefID {
        
        [SerializeField] private int refId;
        [HideInInspector]
        [SerializeField] private int backupRefId;

        public int Id => refId;

        public static int GenerateId() {
            int digits = UnityEngine.Random.Range(7, 10);
            string id = "";
            for (int i = 0; i < digits; i++) {
                float randomNumber = UnityEngine.Random.Range(0, 10);
                id += randomNumber.ToString();
            }

            return int.Parse(id);
        }

        public bool Equals(RefID other) {
            return refId == other.refId;
        }

        public override int GetHashCode() {
            return refId;
        }

        public override string ToString() {
            return Id.ToString();
        }

        public static implicit operator int(RefID i) => i.refId;
    }
}