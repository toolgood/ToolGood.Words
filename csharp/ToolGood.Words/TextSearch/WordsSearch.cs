using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索，带返回位置及索引号
    /// </summary>
    public class WordsSearch
    {
        #region 私有变量
        private TrieNode2[] _first = new TrieNode2[char.MaxValue + 1];
        internal string[] _keywords;
        internal string[] _others;
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            SetKeywords();
        }

        private void SetKeywords()
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
            allNodeLayers.Clear();
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

        #endregion

        #region 查找 替换 查找第一个关键字 判断是否包含关键字
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
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public WordsSearchResult FindFirst(string text)
        {
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
                        var item = tn.Results[0];
                        var keyword = _keywords[item];
                        return new WordsSearchResult(keyword, i + 1 - keyword.Length, i, item);
                    }
                }
                ptr = tn;
            }
            return WordsSearchResult.Empty;
        }
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<WordsSearchResult> FindAll(string text)
        {
            TrieNode2 ptr = null;
            List<WordsSearchResult> list = new List<WordsSearchResult>();

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
                        foreach (var item in tn.Results) {
                            var keyword = _keywords[item];
                            list.Add(new WordsSearchResult(keyword, i + 1 - keyword.Length, i, item));
                        }
                    }
                }
                ptr = tn;
            }
            return list;
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
        #endregion

    }
}
