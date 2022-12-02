using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public sealed class TrieNode3Ex
    {
        public int Index;
        public bool End { get { return Results != null; } }
        public List<int> Results;
        public Dictionary<char, TrieNode3Ex> m_values;
        public ushort minflag = ushort.MaxValue;
        public ushort maxflag = ushort.MinValue;
        public bool HasWildcard;
        public TrieNode3Ex WildcardNode;

        public void Add(char c, TrieNode3Ex node3)
        {
            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }
            if (m_values == null) {
                m_values = new Dictionary<char, TrieNode3Ex>();
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
            if (m_values == null) {
                return false;
            }
            return m_values.ContainsKey(c);
        }


    }
}
