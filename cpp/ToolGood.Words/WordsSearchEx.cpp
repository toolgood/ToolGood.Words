#include "BaseSearchEx.cpp";
#include <vector>
#include <string>
#include "WordsSearchResult.cpp"
using std::vector;
using std::string;

class WordsSearchEx :BaseSearchEx
{
public:
	WordsSearchEx() = default;

	void SetKeywords(string _keywords[]) {
		this->SetKeywords2(_keywords);
	}


	vector<WordsSearchResult> FindAll(string text)
	{
		vector<WordsSearchResult> result;
		int p = 0;
		for (size_t i = 0; i < text.size(); i++)
		{
			char t = _dict[text[i]];
			if (t == 0) {
				p = 0;
				continue;
			}
			int next;
			if (p == 0 || _nextIndex[p].TryGetValue(t, next) == false)
			{
				next = _first[t];
			}
			if (next != 0) {
				for (int j = _end[next]; j < _end[next + 1]; j++) {
					int index = _resultIndex[j];
					int len = _keywordLengths[index];
					int st = i + 1 - len;
					WordsSearchResult r;
					r.Success = true;
					r.Start = st;
					r.End = i;
					r.Keyword = text.substr(st, len);
					result.push_back(r);
				}
			}
			p = next;
		}
		return result;
	}

	WordsSearchResult FindFirst(string text)
	{
		int p = 0;
		for (size_t i = 0; i < text.size(); i++)
		{
			char t = _dict[text[i]];
			if (t == 0) {
				p = 0;
				continue;
			}
			int next;
			if (p == 0 || _nextIndex[p].TryGetValue(t, next) == false)
			{
				next = _first[t];
			}

			if (next != 0) {
				for (int j = _end[next]; j < _end[next + 1]; j++) {
					int index = _resultIndex[j];
					int len = _keywordLengths[index];
					int st = i + 1 - len;
					WordsSearchResult r;
					r.Start = st;
					r.End = i;
					r.Keyword = text.substr(st, len);
					return r;
				}
			}
			p = next;
		}
		WordsSearchResult t;
		return t;
	}
	bool ContainsAny(string text)
	{
		int p = 0;
		for (size_t i = 0; i < text.size(); i++)
		{
			char t = _dict[text[i]];
			if (t == 0) {
				p = 0;
				continue;
			}
			int next;
			if (p == 0 || _nextIndex[p].TryGetValue(t, next) == false)
			{
				next = _first[t];
			}

			if (next != 0) {
				if (_end[next] < _end[next + 1]) {
					return true;
				}
			}
			p = next;
		}
		return false;
	}

};