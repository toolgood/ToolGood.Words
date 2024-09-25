#pragma once

#include <map>
#include <vector>
#include <string>
#include <algorithm>
using std::map;
using std::vector;
using std::string;
using namespace std;


typedef pair<unsigned short, int> PAIR;
//int cmp2(const PAIR& x, const PAIR& y) {
//	return x.first < y.first;
//}

class IntDictionary
{
public:
	unsigned short* _keys;
	int* _values;
	int last;

public:
	
	IntDictionary()
	{
	
	}
	IntDictionary(unsigned short* keys, int* values, int len)
	{
		_keys = keys;
		_values = values;
		last = len - 1;
	}
	IntDictionary(map<unsigned short, int> dict)
	{
		int len = dict.size();
		_keys = new unsigned short[len];
		_values = new int[len];
		last = len - 1;

		vector<PAIR> vec(dict.begin(), dict.end());
		sort(vec.begin(), vec.end());

		for (size_t i = 0; i < vec.size(); i++)
		{
			_keys[i] = vec[i].first;
			_values[i] = vec[i].second;
		}		
		last = len - 1;
	}


	bool TryGetValue(unsigned short key, int& value)
	{
		if (last == -1) {
			value = 0;
			return false;
		}
		if (_keys[0] == key) {
			value = _values[0];
			return true;
		}
		else if (last == 0 || _keys[0] > key) {
			value = 0;
			return false;
		}

		if (_keys[last] == key) {
			value = _values[last];
			return true;
		}
		else if (_keys[last] < key) {
			value = 0;
			return false;
		}

		int left = 1;
		int right = last - 1;
		while (left <= right) {
			int mid = (left + right) >> 1;
			int d = _keys[mid] - key;

			if (d == 0) {
				value = _values[mid];
				return true;
			}
			else if (d > 0) {
				right = mid - 1;
			}
			else {
				left = mid + 1;
			}
		}
		value = 0;
		return false;
	}
};