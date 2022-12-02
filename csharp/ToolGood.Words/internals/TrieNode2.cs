using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public sealed class TrieNode2
    {
        public bool End { get { return Results != null; } }
        public List<int> Results;
        public Dictionary<char, TrieNode2> m_values;
        private uint minflag = uint.MaxValue;
        private uint maxflag = uint.MinValue;


        public void Add(char c, TrieNode2 node3)
        {
            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }
            if (m_values==null) {
                m_values = new Dictionary<char, TrieNode2>();
            }
            m_values.Add(c, node3);
        }

        public void SetResults(int index)
        {
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
