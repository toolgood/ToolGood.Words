using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseSearch
    {
        protected internal TrieNode2[] _first = new TrieNode2[char.MaxValue + 1];
        protected internal string[] _keywords;

        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public virtual void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            SetKeywords();
        }

        protected void SetKeywords()
        {
            var root = new TrieNode();
            Dictionary<int, List<TrieNode>> allNodeLayers = new Dictionary<int, List<TrieNode>>();
            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (int j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)p[j]);
                    if (nd.Layer == 0) {
                        nd.Layer = j + 1;
                        List<TrieNode> trieNodes;
                        if (allNodeLayers.TryGetValue(nd.Layer, out trieNodes) == false) {
                            trieNodes = new List<TrieNode>();
                            allNodeLayers[nd.Layer] = trieNodes;
                        }
                        trieNodes.Add(nd);
                    }
                }
                nd.SetResults(i);
            }

            List<TrieNode> allNode = new List<TrieNode>();
            allNode.Add(root);
            foreach (var trieNodes in allNodeLayers) {
                foreach (var nd in trieNodes.Value) {
                    allNode.Add(nd);
                }
            }
            allNodeLayers = null;


            for (int i = 1; i < allNode.Count; i++) {
                var nd = allNode[i];
                nd.Index = i;
                TrieNode r = nd.Parent.Failure;
                char c = nd.Char;
                while (r != null && !r.m_values.ContainsKey(c)) r = r.Failure;
                if (r == null)
                    nd.Failure = root;
                else {
                    nd.Failure = r.m_values[c];
                    foreach (var result in nd.Failure.Results)
                        nd.SetResults(result);
                }
            }
            root.Failure = root;


            var allNode2 = new List<TrieNode2>();
            for (int i = 0; i < allNode.Count; i++) {
                allNode2.Add(new TrieNode2());
            }
            for (int i = 0; i < allNode2.Count; i++) {
                var oldNode = allNode[i];
                var newNode = allNode2[i];

                foreach (var item in oldNode.m_values) {
                    var key = item.Key;
                    var index = item.Value.Index;
                    newNode.Add(key, allNode2[index]);
                }
                foreach (var item in oldNode.Results) {
                    newNode.SetResults(item);
                }
                oldNode = oldNode.Failure;
                while (oldNode != root) {
                    foreach (var item in oldNode.m_values) {
                        var key = item.Key;
                        var index = item.Value.Index;
                        if (newNode.HasKey(key) == false) {
                            newNode.Add(key, allNode2[index]);
                        }
                    }
                    foreach (var item in oldNode.Results) {
                        newNode.SetResults(item);
                    }
                    oldNode = oldNode.Failure;
                }
            }
            allNode.Clear();
            allNode = null;
            root = null;

            TrieNode2[] first = new TrieNode2[char.MaxValue + 1];
            foreach (var item in allNode2[0].m_values) {
                first[item.Key] = item.Value;
            }
            _first = first;
        }

    }
}
