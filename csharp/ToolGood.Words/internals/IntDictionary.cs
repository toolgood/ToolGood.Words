using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public struct IntDictionary
    {
        private ushort[] _keys;
        private int[] _values;
        private int last;
        public IntDictionary(ushort[] keys, int[] values)
        {
            _keys = keys;
            _values = values;
            last = keys.Length - 1;
        }
        public IntDictionary(Dictionary<ushort, int> dict)
        {
            var keys = dict.Select(q => q.Key).OrderBy(q => q).ToArray();
            var values = new int[keys.Length];
            for (int i = 0; i < keys.Length; i++) {
                values[i] = dict[keys[i]];
            }
            _keys = keys;
            _values = values;
            last = keys.Length - 1;
        }


        public ushort[] Keys {
            get {
                return _keys;
            }
        }

        public int[] Values {
            get {
                return _values;
            }
        }

        public bool TryGetValue(ushort key, out int value)
        {
            if (last == -1) {
                value = 0;
                return false;
            }
            if (_keys[0] == key) {
                value = _values[0];
                return true;
            } else if (last == 0 || _keys[0] > key) {
                value = 0;
                return false;
            }

            if (_keys[last] == key) {
                value = _values[last];
                return true;
            } else if (_keys[last] < key) {
                value = 0;
                return false;
            }

            var left = 1;
            var right = last - 1;
            while (left <= right) {
                int mid = (left + right) >> 1;
                int d = _keys[mid] - key;

                if (d == 0) {
                    value = _values[mid];
                    return true;
                } else if (d > 0) {
                    right = mid - 1;
                } else {
                    left = mid + 1;
                }
            }
            value = 0;
            return false;
        }



    }
}
