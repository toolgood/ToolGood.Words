using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public class IntDictionary2
    {
        private int[] _keys;
        private int[] _values;
        private int last;
        private int[] _keys2;
        private int[] _values2;
        private int last2;


        public IntDictionary2()
        {
            last = -1;
            last2 = -1;
        }

        public int[] Keys { get { return _keys; } }
        public int[] Values { get { return _values; } }
        public int[] Keys2 { get { return _keys2; } }
        public int[] Values2 { get { return _values2; } }



        public void SetDictionary(Dictionary<int, int> dict, Dictionary<int, int> dict2)
        {
            _keys = dict.Select(q => q.Key).OrderBy(q => q).ToArray();
            _values = new int[_keys.Length];
            for (int i = 0; i < _keys.Length; i++) {
                _values[i] = dict[_keys[i]];
            }
            last = _keys.Length - 1;


            _keys2 = dict2.Select(q => q.Key).OrderBy(q => q).ToArray();
            _values2 = new int[_keys2.Length];
            for (int i = 0; i < _keys2.Length; i++) {
                _values2[i] = dict2[_keys2[i]];
            }
            last2 = _keys2.Length - 1;
        }


        public bool TryGetValue(int key, out int value)
        {
            if (last == -1) {
                value = 0;
                return false;
            }
            if (_keys[0] == key) {
                value = _values[0];
                return true;
            } else if (_keys[0] > key) {
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

        public bool TryGetValue2(int key, out int value)
        {
            if (last2 == -1) {
                value = 0;
                return false;
            }
            if (_keys2[0] == key) {
                value = _values2[0];
                return true;
            } else if (_keys2[0] > key) {
                value = 0;
                return false;
            }
            if (_keys2[last2] == key) {
                value = _values2[last2];
                return true;
            } else if (_keys2[last2] < key) {
                value = 0;
                return false;
            }

            var left = 0;
            var right = last2;
            while (left + 1 < right) {
                int mid = (left + right) / 2;
                int d = _keys2[mid] - key;

                if (d == 0) {
                    value = _values2[mid];
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
