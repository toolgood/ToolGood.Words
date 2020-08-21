package toolgood.words.internals;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class IntDictionary {
    private int[] _keys;
    private int[] _values;
    private int last;

    public IntDictionary() {
        last = -1;
    }

    public int[] getKeys() {
        return _keys;
    }

    public int[] getValues() {
        return _values;
    }

    public void SetDictionary(Map<Integer, Integer> dict) {

        List<Integer> keys = new ArrayList<Integer>();
        dict.forEach((k, v) -> {
            keys.add((int) k);
        });

        _keys = new int[dict.size()];
        _values = new int[dict.size()];
        for (int i = 0; i < keys.size(); i++) {
            _keys[i] = keys.get(i);
            _values[i] = dict.get(_keys[i]);
        }
        last = _keys.length - 1;
    }

    public void SetDictionary(int[] keys, int[] values) {
        _keys = keys;
        _values = values;
        last = _keys.length - 1;
    }

    public int IndexOf(int key) {
        if (last == -1) {
            return -1;
        }
        if (_keys[0] == key) {
            return 0;
        }
        if (_keys[last] == key) {
            return last;
        }

        int left = 0;
        int right = last;
        while (left + 1 < right) {
            int mid = (left + right) >> 1;
            int d = _keys[mid] - key;

            if (d == 0) {
                return mid;
            } else if (d > 0) {
                right = mid;
            } else {
                left = mid;
            }
        }
        return -1;
    }

    public int GetValue(int index){
        return _values[index];
    }
 
}