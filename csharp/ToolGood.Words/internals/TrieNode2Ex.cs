using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    class TrieNode2Ex : TrieNode2
    {
        public int Index;
        public bool End;
        public List<int> Results;
        public Dictionary<char, TrieNode2Ex> m_values;
        public ushort minflag = ushort.MaxValue;
        public ushort maxflag = ushort.MinValue;

        public TrieNode2Ex()
        {
            Results = new List<int>();
            m_values = new Dictionary<char, TrieNode2Ex>();
        }

        public void Add(char c, TrieNode2Ex node3)
        {
            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }
            m_values.Add(c, node3);
        }

        public void SetResults(int index)
        {
            if (End == false) {
                End = true;
            }
            if (Results.Contains(index) == false) {
                Results.Add(index);
            }
        }

        public bool HasKey(char c)
        {
            return m_values.ContainsKey(c);
        }

        public bool TryGetValue(char c, out TrieNode2Ex node)
        {
            if (minflag <= (uint)c && maxflag >= (uint)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }
    }
}
