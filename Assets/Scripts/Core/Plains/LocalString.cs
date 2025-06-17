using System.Collections.Generic;
using Core.Systems;

namespace Core.Plains {
    [System.Serializable]
    public struct LocalString {
        public string key;
        public string table;

        private object[] _parameters;

        private string Value => CalculateValue();

        public LocalString(string key, string table="default") {
            this.key = key;
            this.table = table;
            _parameters = null;
        }
        
        // Implicit operator makes the opportunity to return string from a class constructor.
        public static implicit operator string(LocalString l) {
            return l.Value;
        }

        public LocalString WithParams(Dictionary<string, object> paramsDictionary) {
            _parameters = new object[]{ paramsDictionary};
            return this;
        }

        public override string ToString() {
            return Value;
        }

        private string CalculateValue() {
            if (_parameters == null) {
                return GameLocalization.Get(key, table);
            }
            else {
                return GameLocalization.Get(key, table, _parameters);
            }
        }
    }
}