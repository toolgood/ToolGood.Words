using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public class IntDictionary
    {
        private ushort[] _keys;
        private int[] _values;
        private int last;
        public IntDictionary()
        {
            last = -1;
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

        public void SetDictionary(Dictionary<ushort, int> dict)
        {
            _keys = dict.Select(q => q.Key).OrderBy(q => q).ToArray();
            _values = new int[_keys.Length];
            for (int i = 0; i < _keys.Length; i++) {
                _values[i] = dict[_keys[i]];
            }
            last = _keys.Length - 1;
        }
        public void SetDictionary(ushort[] keys, int[] values)
        {
            _keys = keys;
            _values = values;
            last = _keys.Length - 1;
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
            }
            if (_keys[last] == key) {
                value = _values[last];
                return true;
            }

            var left = 0;
            var right = last;
            while (left + 1 < right) {
                int mid = (left + right) / 2;
                int d = _keys[mid] - key;

                if (d == 0) {
                    value = _values[mid];
                    return true;
                } else if (d > 0) {
                    right = mid;
                } else {
                    left = mid;
                }
            }
            value = 0;
            return false;
        }



    }
}
