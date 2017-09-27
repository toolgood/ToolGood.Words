using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索，带返回位置及索引号
    /// </summary>
    public class WordsSearch
    {
        #region class
        class TrieNode
        {
            public bool End { get; set; }
            public List<Tuple<string, int>> Results { get; set; }
            internal Dictionary<char, TrieNode> m_values;
            private uint minflag = uint.MaxValue;
            private uint maxflag = uint.MinValue;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                Results = new List<Tuple<string, int>>();
            }

            public bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (uint)c && maxflag >= (uint)c) {
                    return m_values.TryGetValue(c, out node);
                }
                node = null;
                return false;
            }

            public TrieNode Add(char c)
            {
                TrieNode node;
                if (minflag > c) { minflag = c; }
                if (maxflag < c) { maxflag = c; }
                if (m_values.TryGetValue(c, out node)) {
                    return node;
                }
                node = new TrieNode();
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

            public void Merge(TrieNode node)
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
        #endregion

        #region 私有变量
        private TrieNode _root = new TrieNode();
        private TrieNode[] _first = new TrieNode[char.MaxValue + 1];
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(ICollection<string> keywords)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int index = 0;
            foreach (var item in keywords) {
                dict[item] = index++;
            }
            SetKeywords(dict);
        }
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        /// <param name="indexs">关键字索引列表</param>
        public void SetKeywords(ICollection<string> keywords, ICollection<int> indexs)
        {
            if (keywords.Count != indexs.Count) { throw new Exception("数量不一样"); }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            long index = 0;
            var ind = indexs.ToArray();
            foreach (var item in keywords) {
                dict[item] = ind[index++];
            }
            SetKeywords(dict);

        }
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(IDictionary<string, int> keywords)
        {
            var first = new TrieNode[char.MaxValue + 1];
            var root = new TrieNode();
            foreach (var key in keywords) {
                var p = key.Key;
                if (string.IsNullOrEmpty(p)) continue;

                var nd = first[p[0]];
                if (nd == null) {
                    nd = root.Add(p[0]);
                    first[p[0]] = nd;
                }
                for (int i = 1; i < p.Length; i++) {
                    nd = nd.Add(p[i]);
                }
                nd.SetResults(p, key.Value);
            }
            this._first = first;

            Dictionary<TrieNode, TrieNode> links = new Dictionary<TrieNode, TrieNode>();
            foreach (var item in root.m_values) {
                TryLinks(item.Value, null, links);
            }

            foreach (var item in links) {
                item.Key.Merge(item.Value);
            }

            _root = root;
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
                } else {
                    if (node2.TryGetValue(item.Key, out tn)) {
                        links[item.Value] = tn;
                    }
                }
                TryLinks(item.Value, tn, links);
            }
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
            TrieNode ptr = null;
            foreach (char t in text) {
                TrieNode tn;
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
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
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
                        return new WordsSearchResult(item.Item1, i + 1 - item.Item1.Length, i, item.Item2);
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
            TrieNode ptr = null;
            List<WordsSearchResult> list = new List<WordsSearchResult>();

            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
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
                            list.Add(new WordsSearchResult(item.Item1, i + 1 - item.Item1.Length, i, item.Item2));
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

            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        var maxLength = tn.Results[0].Item1.Length;

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
