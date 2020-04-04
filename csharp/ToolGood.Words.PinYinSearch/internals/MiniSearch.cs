using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words.internals
{
    class MiniSearch
    {
        TrieNode2[] _first = new TrieNode2[char.MaxValue + 1];
        internal string[] _keywords;

        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public virtual void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            SetKeywords();
        }

        private void SetKeywords()
        {
            var root = new TrieNode();

            List<TrieNode> allNode = new List<TrieNode>();
            allNode.Add(root);

            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (int j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)p[j]);
                    if (nd.Layer == 0) {
                        nd.Layer = j + 1;
                        allNode.Add(nd);
                    }
                }
                nd.SetResults(i);
            }


            List<TrieNode> nodes = new List<TrieNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TrieNode nd in root.m_values.Values) {
                nd.Failure = root;
                foreach (TrieNode trans in nd.m_values.Values) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TrieNode> newNodes = new List<TrieNode>();
                foreach (TrieNode nd in nodes) {
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
                    // add child nodes to BFS list 
                    foreach (TrieNode child in nd.m_values.Values)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            root.Failure = root;

            allNode = allNode.OrderBy(q => q.Layer).ToList();
            for (int i = 0; i < allNode.Count; i++) { allNode[i].Index = i; }

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
                if (oldNode.Failure != root) {
                    foreach (var item in oldNode.Failure.m_values) {
                        var key = item.Key;
                        var index = item.Value.Index;
                        if (newNode.HasKey(key) == false) {
                            newNode.Add(key, allNode2[index]);
                        }
                    }
                    foreach (var item in oldNode.Failure.Results) {
                        newNode.SetResults(item);
                    }
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


        public List<MiniSearchResult> FindAll(string text)
        {
            internals.TrieNode2 ptr = null;
            List<MiniSearchResult> list = new List<MiniSearchResult>();

            for (int i = 0; i < text.Length; i++) {
                internals.TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            list.Add(new MiniSearchResult(_keywords[item], i + 1 - _keywords[item].Length, i));
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }

    }
}
