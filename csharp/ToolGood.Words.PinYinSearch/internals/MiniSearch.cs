using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words.internals
{
    class MiniSearch
    {
        TrieNode[] _first = new TrieNode[char.MaxValue + 1];

        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="_keywords">关键字列表</param>
        public virtual void SetKeywords(ICollection<string> _keywords)
        {
            var first = new TrieNode[char.MaxValue + 1];
            var root = new TrieNode();

            foreach (var p in _keywords) {
                if (string.IsNullOrEmpty(p)) continue;

                var nd = _first[p[0]];
                if (nd == null) {
                    nd = root.Add(p[0]);
                    first[p[0]] = nd;
                }
                for (int i = 1; i < p.Length; i++) {
                    nd = nd.Add(p[i]);
                }
                nd.SetResults(p);
            }
            this._first = first;// root.ToArray();

            Dictionary<TrieNode, TrieNode> links = new Dictionary<TrieNode, TrieNode>();
            foreach (var item in root.m_values) {
                TryLinks(item.Value, null, links);
            }

            foreach (var item in links) {
                item.Key.Merge(item.Value, links);
            }

            //_root = root;
        }

        private void TryLinks(TrieNode node, TrieNode node2, Dictionary<TrieNode, TrieNode> links)
        {
            foreach (var item in node.m_values) {
                TrieNode tn = null;
                if (node2 == null) {
                    tn = _first[item.Key];
                    if (tn != null) {
                        links[item.Value] = tn;
                    }
                } else if (node2.TryGetValue(item.Key, out tn)) {
                    links[item.Value] = tn;
                }
                TryLinks(item.Value, tn, links);
            }
        }


        public List<MiniSearchResult> FindAll(string text)
        {
            internals.TrieNode ptr = null;
            List<MiniSearchResult> list = new List<MiniSearchResult>();

            for (int i = 0; i < text.Length; i++) {
                internals.TrieNode tn;
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
                            list.Add(new MiniSearchResult(item, i + 1 - item.Length, i));
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }

    }
}
