using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public class TrieNode
    {
        //public byte Type { get; set; }
        public bool End { get; set; }
        public List<string> Results { get; set; }
        private Dictionary<char, TrieNode> m_values;
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

        public void Add(TreeNode t, TrieNode node)
        {
            var c = t.Char;
            if (m_values.ContainsKey(c) == false) {
                if (minflag > c) { minflag = c; }
                if (maxflag < c) { maxflag = c; }
                m_values.Add(c, node);
                foreach (var item in t.Results) {
                    node.End = true;
                    if (node.Results.Contains(item)==false) {
                        node.Results.Add(item);
                    }
                }
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
