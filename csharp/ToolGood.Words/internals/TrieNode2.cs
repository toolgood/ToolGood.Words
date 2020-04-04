using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public class TrieNode2
    {
        public bool End;
        public List<int> Results;
        public Dictionary<char, TrieNode2> m_values;
        private uint minflag = uint.MaxValue;
        private uint maxflag = uint.MinValue;

        public TrieNode2()
        {
            Results = new List<int>();
            m_values = new Dictionary<char, TrieNode2>();
        }

        public void Add(char c, TrieNode2 node3)
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

        public bool TryGetValue(char c, out TrieNode2 node)
        {
            if (minflag <= (uint)c && maxflag >= (uint)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }

    }
}
