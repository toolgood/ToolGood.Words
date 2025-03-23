#pragma 
#include <map>
#include <vector>
using std::map;
using std::vector;
class TrieNode
{
public:
	int Index;
	int Layer;
	wchar_t Char;
	vector<int> Results;
	map<wchar_t, TrieNode> m_values;
	TrieNode* Failure;
	TrieNode* Parent;
	bool IsWildcard;
	int WildcardLayer;
	bool HasWildcard;

	TrieNode() {
		Failure = NULL;
		Parent = NULL;
	}

	TrieNode Add(wchar_t c)
	{
		auto find = m_values.find(c);
		if (find != m_values.end())
		{
			return	find->second;
		}
		TrieNode node;
		node.Parent = this;
		node.Char = c;
		m_values[c] = node;
		return node;
	}
	void SetResults(int index)
	{
		Results.push_back(index);
	}
};

#pragma once
