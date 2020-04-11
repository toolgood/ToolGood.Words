using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索匹配, 支持 部分 正则 如 . ? [ ] \ ( | ) ,不支持( )内再嵌套( )
    /// </summary>
    public class WordsMatch : BaseMatch
    {
        private TrieNode3[] _first = new TrieNode3[char.MaxValue + 1];
        internal int[] _keywordLength;
        internal int[] _keywordIndex;
        internal string[] _matchKeywords;

        #region SetKeywords
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public virtual void SetKeywords(ICollection<string> keywords)
        {
            _matchKeywords = keywords.ToArray();
            List<string> newKeyword = new List<string>();
            List<int> newKeywordLength = new List<int>();
            List<int> newKeywordIndex = new List<int>();
            var index = 0;
            foreach (var keyword in keywords) {
                if (HasMatch(keyword) == false) {
                    newKeyword.Add(keyword);
                    newKeywordLength.Add(keyword.Length);
                    newKeywordIndex.Add(index);
                } else {
                    var list = MatchKeywordBuild(keyword);
                    foreach (var item in list) {
                        newKeyword.Add(item);
                        newKeywordLength.Add(item.Length);
                        newKeywordIndex.Add(index);
                    }
                }
                index++;
            }
            _keywordLength = newKeywordLength.ToArray();
            _keywordIndex = newKeywordIndex.ToArray();

            SetKeywords2(newKeyword);
        }

        private void SetKeywords2(List<string> keywords)
        {
            List<TrieNode> allNode = BuildFirstLayerTrieNode(keywords);
            TrieNode root = allNode[0];

            var allNode2 = new List<TrieNode3>();
            for (int i = 0; i < allNode.Count; i++) {
                allNode2.Add(new TrieNode3());
            }

            for (int i = 0; i < allNode2.Count; i++) {
                var oldNode = allNode[i];
                var newNode = allNode2[i];

                foreach (var item in oldNode.m_values) {
                    var key = item.Key;
                    var index = item.Value.Index;
                    if (key == 0) {
                        newNode.HasWildcard = true;
                        newNode.WildcardNode = allNode2[index];
                        continue;
                    }
                    newNode.Add(key, allNode2[index]);
                }
                foreach (var item in oldNode.Results) {
                    if (oldNode.IsWildcard) {
                        if (_keywordLength[item] > oldNode.WildcardLayer) {
                            newNode.SetResults(item);
                        }
                    } else {
                        newNode.SetResults(item);
                    }
                    //newNode.SetResults(item);
                }

                var failure = oldNode.Failure;
                while (failure != root) {
                    if (oldNode.IsWildcard && failure.Layer <= oldNode.WildcardLayer) {
                        break;
                    }
                    foreach (var item in failure.m_values) {
                        var key = item.Key;
                        var index = item.Value.Index;
                        if (key == 0) {
                            newNode.HasWildcard = true;
                            if (newNode.WildcardNode == null) {
                                newNode.WildcardNode = allNode2[index];
                            }
                            continue;
                        }
                        if (newNode.HasKey(key) == false) {
                            newNode.Add(key, allNode2[index]);
                        }
                    }
                    foreach (var item in failure.Results) {
                        if (oldNode.IsWildcard) {
                            if (_keywordLength[item] > oldNode.WildcardLayer) {
                                newNode.SetResults(item);
                            }
                        } else {
                            newNode.SetResults(item);
                        }
                    }
                    failure = failure.Failure;
                }
            }
            allNode.Clear();
            allNode = null;
            root = null;

            //var root2 = allNode2[0];
            TrieNode3[] first = new TrieNode3[char.MaxValue + 1];
            foreach (var item in allNode2[0].m_values) {
                first[item.Key] = item.Value;
            }
            _first = first;
        }



        #endregion

        #region FindFirst
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public WordsSearchResult FindFirst(string text)
        {
            TrieNode3 ptr = null;
            for (int i = 0; i < text.Length; i++) {
                var t = text[i];

                TrieNode3 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        if (ptr.HasWildcard) {
                            var result = FindFirst(text, i + 1, ptr.WildcardNode);
                            if (result != null) {
                                return result;
                            }
                        }
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        var r = tn.Results[0];
                        var length = _keywordLength[r];
                        var start = i - length + 1;
                        if (start >= 0) {
                            var kIndex = _keywordIndex[r];
                            var matchKeyword = _matchKeywords[kIndex];
                            var keyword = text.Substring(start, length);
                            return new WordsSearchResult(keyword, start, i, kIndex, matchKeyword);
                        }
                    }
                }
                ptr = tn;
            }
            return null;
        }
        private WordsSearchResult FindFirst(string text, int index, TrieNode3 ptr)
        {
            for (int i = index; i < text.Length; i++) {
                var t = text[i];
                TrieNode3 tn;
                if (ptr.TryGetValue(t, out tn) == false) {
                    if (ptr.HasWildcard) {
                        var result = FindFirst(text, i + 1, ptr.WildcardNode);
                        if (result != null) {
                            return result;
                        }
                    }
                    return null;
                }
                if (tn.End) {
                    var r = tn.Results[0];
                    var length = _keywordLength[r];
                    var start = i - length + 1;
                    if (start >= 0) {
                        var kIndex = _keywordIndex[r];
                        var matchKeyword = _matchKeywords[kIndex];
                        var keyword = text.Substring(start, length);
                        return new WordsSearchResult(keyword, start, i, kIndex, matchKeyword);
                    }
                }
                ptr = tn;
            }
            return null;
        }
        #endregion

        #region FindAll
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<WordsSearchResult> FindAll(string text)
        {
            TrieNode3 ptr = null;
            List<WordsSearchResult> list = new List<WordsSearchResult>();

            for (int i = 0; i < text.Length; i++) {
                var t = text[i];
                TrieNode3 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        if (ptr.HasWildcard) {
                            FindAll(text, i + 1, ptr.WildcardNode, list);
                        }
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var r in tn.Results) {
                            var length = _keywordLength[r];
                            var start = i - length + 1;
                            if (start >= 0) {
                                var kIndex = _keywordIndex[r];
                                var matchKeyword = _matchKeywords[kIndex];
                                var keyword = text.Substring(start, length);
                                var result = new WordsSearchResult(keyword, start, i, kIndex, matchKeyword);
                                list.Add(result);
                            }
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }
        private void FindAll(string text, int index, TrieNode3 ptr, List<WordsSearchResult> list)
        {
            for (int i = index; i < text.Length; i++) {
                var t = text[i];
                TrieNode3 tn;
                if (ptr.TryGetValue(t, out tn) == false) {
                    if (ptr.HasWildcard) {
                        FindAll(text, i + 1, ptr.WildcardNode, list);
                    }
                    return;
                }
                if (tn.End) {
                    foreach (var r in tn.Results) {
                        var length = _keywordLength[r];
                        var start = i - length + 1;
                        if (start >= 0) {
                            var kIndex = _keywordIndex[r];
                            var matchKeyword = _matchKeywords[kIndex];
                            var keyword = text.Substring(start, length);
                            var result = new WordsSearchResult(keyword, start, i, kIndex, matchKeyword);
                            list.Add(result);
                        }
                    }
                }
                ptr = tn;
            }
        }
        #endregion

        #region ContainsAny
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool ContainsAny(string text)
        {
            TrieNode3 ptr = null;
            for (int i = 0; i < text.Length; i++) {
                var t = text[i];
                TrieNode3 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        if (ptr.HasWildcard) {
                            var result = ContainsAny(text, i + 1, ptr.WildcardNode);
                            if (result) {
                                return true;
                            }
                        }
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        var length = _keywordLength[tn.Results[0]];
                        var s = i - length + 1;
                        if (s >= 0) {
                            return true;
                        }
                    }
                }
                ptr = tn;
            }
            return false;
        }
        private bool ContainsAny(string text, int index, TrieNode3 ptr)
        {
            for (int i = index; i < text.Length; i++) {
                var t = text[i];
                TrieNode3 tn;
                if (ptr.TryGetValue(t, out tn) == false) {
                    if (ptr.HasWildcard) {
                        return ContainsAny(text, i + 1, ptr.WildcardNode);
                    }
                    return false;
                }
                if (tn.End) {
                    var length = _keywordLength[tn.Results[0]];
                    var s = i - length + 1;
                    if (s >= 0) {
                        return true;
                    }
                }
                ptr = tn;
            }
            return false;
        }

        #endregion

        #region Replace
        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);

            TrieNode3 ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode3 tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        if (ptr.HasWildcard) {
                            Replace(text, i + 1, ptr.WildcardNode, replaceChar, result);
                        }
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        var maxLength = _keywordLength[tn.Results[0]];
                        var start = i + 1 - maxLength;
                        if (start >= 0) {
                            for (int j = start; j <= i; j++) {
                                result[j] = replaceChar;
                            }
                        }
                    }
                }
                ptr = tn;
            }
            return result.ToString();
        }

        private void Replace(string text, int index, TrieNode3 ptr, char replaceChar, StringBuilder result)
        {
            for (int i = index; i < text.Length; i++) {
                var t = text[i];
                TrieNode3 tn;
                if (ptr.TryGetValue(t, out tn) == false) {
                    if (ptr.HasWildcard) {
                        Replace(text, i + 1, ptr.WildcardNode, replaceChar, result);
                    }
                    return;
                }
                if (tn.End) {
                    var maxLength = _keywordLength[tn.Results[0]];
                    var start = i + 1 - maxLength;
                    if (start >= 0) {
                        for (int j = start; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                ptr = tn;
            }
        }

        #endregion


    }
}
