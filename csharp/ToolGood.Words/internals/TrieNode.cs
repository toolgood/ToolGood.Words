using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public class TrieNode
    {
        public bool End { get; set; }
        public List<string> Results { get; private set; }
        internal Dictionary<char, TrieNode> m_values;
        private uint minflag = uint.MaxValue;
        private uint maxflag = uint.MinValue;


        public TrieNode()
        {
            m_values = new Dictionary<char, TrieNode>();
            Results = new List<string>();
        }

        public bool TryGetValue(char c, out TrieNode node)
        {
            if (minflag <= (uint)c && maxflag >= (uint)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }
        public ICollection<TrieNode> Transitions()
        {
            return m_values.Values;
        }

        public TrieNode Add(char c)
        {
            TrieNode node;

            if (m_values.TryGetValue(c, out node)) {
                return node;
            }

            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }

            node = new TrieNode();
            m_values[c] = node;
            return node;
        }

        public void SetResults(string text)
        {
            if (End == false) {
                End = true;
            }
            if (Results.Contains(text)==false) {
                Results.Add(text);
            }
        }

        public void Merge(TrieNode node, Dictionary<TrieNode, TrieNode> links)
        {
            if (node.End) {
                if (End == false) {
                    End = true;
                }
                foreach (var item in node.Results) {
                    if (Results.Contains(item) == false) {
                        Results.Add(item);
                    }
                }
            }

            foreach (var item in node.m_values) {
    
                if ( m_values.ContainsKey(item.Key) == false) {
                    if (minflag > item.Key) { minflag = item.Key; }
                    if (maxflag < item.Key) { maxflag = item.Key; }
                    m_values[item.Key] = item.Value;
                }
            }
            TrieNode node2;
            if (links.TryGetValue(node, out node2)) {
                Merge(node2, links);
            }
        }

        public TrieNode[] ToArray()
        {
            TrieNode[] first = new TrieNode[char.MaxValue + 1];
            foreach (var item in m_values) {
                first[item.Key] = item.Value;
            }
            return first;
        }

    }

}
