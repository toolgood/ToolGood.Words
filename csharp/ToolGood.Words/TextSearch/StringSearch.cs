using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;


namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索
    /// </summary>
    public class StringSearch
    {
        private TrieNode2[] _first = new TrieNode2[char.MaxValue + 1];
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




        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public string FindFirst(string text)
        {
            TrieNode2 ptr = null;
            foreach (char t in text) {
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        return _keywords[tn.Results[0]];
                    }
                }
                ptr = tn;
            }
            return null;
        }
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<string> FindAll(string text)
        {
            TrieNode2 ptr = null;
            List<string> list = new List<string>();

            foreach (char t in text) {
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            list.Add(_keywords[item]);
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool ContainsAny(string text)
        {
            TrieNode2 ptr = null;
            foreach (char t in text) {
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        return true;
                    }
                }
                ptr = tn;
            }
            return false;
        }
        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);

            TrieNode2 ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        var maxLength = _keywords[tn.Results[0]].Length;
                        var start = i + 1 - maxLength;
                        for (int j = start; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                ptr = tn;
            }
            return result.ToString();
        }

    }
}
