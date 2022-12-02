using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public sealed class TrieNode3
    {
        public bool End;
        public bool HasWildcard;
        public List<int> Results;
        public Dictionary<char, TrieNode3> m_values;
        private uint minflag = uint.MaxValue;
        private uint maxflag = uint.MinValue;
        public TrieNode3 WildcardNode;


        public void Add(char c, TrieNode3 node3)
        {
            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }
            if (m_values == null) {
                m_values = new Dictionary<char, TrieNode3>();
            }
            m_values.Add(c, node3);
        }

        public void SetResults(int index)
        {
            if (End == false) {
                End = true;
            }
            if (Results == null) {
                Results = new List<int>();
            }
            if (Results.Contains(index) == false) {
                Results.Add(index);
            }
        }

        public bool HasKey(char c)
        {
            if (m_values==null) {
                return false;
            }
            return m_values.ContainsKey(c);
        }

        public bool TryGetValue(char c, out TrieNode3 node)
        {
            if (minflag <= (uint)c && maxflag >= (uint)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }
    }
}
