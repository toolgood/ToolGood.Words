using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public sealed class TrieNode //:IDisposable
    {
        public int Index;
        public int Layer;
        public bool End { get { return Results != null; } }
        public char Char;
        public List<int> Results;
        public Dictionary<char, TrieNode> m_values;
        public TrieNode Failure;
        public TrieNode Parent;
        public bool IsWildcard;
        public int WildcardLayer;
        public bool HasWildcard;


        public TrieNode Add(char c)
        {
            TrieNode node;
            if (m_values == null) {
                m_values = new Dictionary<char, TrieNode>();
            } else if (m_values.TryGetValue(c, out node)) {
                return node;
            }
            node = new TrieNode();
            node.Parent = this;
            node.Char = c;
            m_values[c] = node;
            return node;
        }

        public void SetResults(int index)
        {

            if (Results == null) {
                Results = new List<int>();
            }
            Results.Add(index);
        }
        /// <summary>
        /// 伪释放
        /// </summary>
        public void Dispose()
        {
            if (Results!=null) {
                Results.Clear();
                Results = null;
            }
            if (m_values!=null) {
                m_values.Clear();
                m_values = null;
            }
            Failure =null;
            Parent = null;
        }
    }
}
