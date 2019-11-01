using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    class TrieNode2
    {
        public bool End { get; set; }
        public List<Tuple<string, int>> Results { get; set; }
        internal Dictionary<char, TrieNode2> m_values;
        private uint minflag = uint.MaxValue;
        private uint maxflag = uint.MinValue;

        public TrieNode2()
        {
            m_values = new Dictionary<char, TrieNode2>();
            Results = new List<Tuple<string, int>>();
        }

        public bool TryGetValue(char c, out TrieNode2 node)
        {
            if (minflag <= (uint)c && maxflag >= (uint)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }

        public TrieNode2 Add(char c)
        {
            TrieNode2 node;
            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }
            if (m_values.TryGetValue(c, out node)) {
                return node;
            }
            node = new TrieNode2();
            m_values[c] = node;
            return node;
        }

        public void SetResults(string text, int index)
        {
            if (End == false) {
                End = true;
            }
            Results.Add(Tuple.Create(text, index));
        }

        public void Merge(TrieNode2 node)
        {
            if (node.End) {
                if (End == false) {
                    End = true;
                }
                foreach (var item in node.Results) {
                    Results.Add(item);
                }
            }

            foreach (var item in node.m_values) {
                if (m_values.ContainsKey(item.Key) == false) {
                    if (minflag > item.Key) { minflag = item.Key; }
                    if (maxflag < item.Key) { maxflag = item.Key; }
                    m_values[item.Key] = item.Value;
                }
            }
        }

    }
}
