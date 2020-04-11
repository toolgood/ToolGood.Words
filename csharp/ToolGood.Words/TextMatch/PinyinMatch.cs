using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 拼音匹配
    /// </summary>
    public class PinyinMatch
    {
        private string[] _keywords;
        private string[] _keywordsFirstPinyin;
        private string[][] _keywordsPinyin;
        private int[] _indexs;

        #region SetKeywords
        /// <summary>
        /// 设置关键字，注：索引会被清空
        /// </summary>
        /// <param name="keywords"></param>
        public void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            _keywordsFirstPinyin = new string[_keywords.Length];
            _keywordsPinyin = new string[_keywords.Length][];
            for (int i = 0; i < _keywords.Length; i++) {
                var text = _keywords[i];
                var pys = PinYinDict.GetPinYinList(text);
                string fpy = "";
                for (int j = 0; j < pys.Length; j++) {
                    pys[j] = pys[j].ToUpper();
                    fpy += pys[j][0];
                }
                _keywordsPinyin[i] = pys;
                _keywordsFirstPinyin[i] = fpy;
            }
            _indexs = null;
        }

        /// <summary>
        /// 设置关键字，注：索引会被清空
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="pinyin"></param>
        /// <param name="splitChar"></param>
        public void SetKeywords(IList<string> keywords, IList<string> pinyin, char splitChar = ',')
        {
            _keywords = keywords.ToArray();
            _keywordsFirstPinyin = new string[_keywords.Length];
            _keywordsPinyin = new string[_keywords.Length][];
            for (int i = 0; i < _keywords.Length; i++) {
                var text = pinyin[i];
                var pys = text.Split(splitChar);
                string fpy = "";
                for (int j = 0; j < pys.Length; j++) {
                    pys[j] = pys[j].ToUpper();
                    fpy += pys[j][0];
                }
                _keywordsPinyin[i] = pys;
                _keywordsFirstPinyin[i] = fpy;
            }
            _indexs = null;
        }

        #endregion

        #region SetIndexs
        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="indexs"></param>
        public void SetIndexs(ICollection<int> indexs)
        {
            if (_keywords == null) {
                throw new Exception("请先使用 SetKeywords 方法");
            }
            if (indexs.Count < _keywords.Length) {
                throw new Exception("indexs 数组长度大于 keywords");
            }
            _indexs = indexs.ToArray();
        }
        #endregion


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> Find(string key)
        {
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            key = key.ToUpper();
            var hasPinyin = Regex.IsMatch(key, "[a-zA-Z]");
            if (hasPinyin == false) {
                List<string> rs = new List<string>();
                for (int i = 0; i < _keywords.Length; i++) {
                    var keyword = _keywords[i];
                    if (keyword.Contains(key)) {
                        rs.Add(keyword);
                    }
                }
                return rs;
            }

            var pykeys = SplitKeywords(key);
            var minLength = int.MaxValue;
            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            foreach (var pykey in pykeys) {
                var keys = pykey.Split((char)0);
                if (minLength > keys.Length) {
                    minLength = keys.Length;
                }
                MergeKeywords(keys, 0, "", list);
            }

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            List<String> result = new List<string>();
            for (int i = 0; i < _keywords.Length; i++) {
                var keywords = _keywords[i];
                if (keywords.Length < minLength) {
                    continue;
                }
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];


                if (search.Find(fpy, keywords, pylist)) {
                    result.Add(keywords);
                }
            }
            return result;
        }

        /// <summary>
        /// 查询索引号
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<int> FindIndex(string key)
        {
            if (string.IsNullOrEmpty(key)) {
                return null;
            }
            key = key.ToUpper();
            var hasPinyin = Regex.IsMatch(key, "[a-zA-Z]");
            if (hasPinyin == false) {
                List<int> rs = new List<int>();
                for (int i = 0; i < _keywords.Length; i++) {
                    var keyword = _keywords[i];
                    if (keyword.Contains(key)) {
                        if (_indexs == null) {
                            rs.Add(i);
                        } else {
                            rs.Add(_indexs[i]);
                        }
                    }
                }
                return rs;
            }

            var pykeys = SplitKeywords(key);
            var minLength = int.MaxValue;
            List<Tuple<string, string[]>> list = new List<Tuple<string, string[]>>();
            foreach (var pykey in pykeys) {
                var keys = pykey.Split((char)0);
                if (minLength > keys.Length) {
                    minLength = keys.Length;
                }
                MergeKeywords(keys, 0, "", list);
            }

            PinyinSearch search = new PinyinSearch();
            search.SetKeywords(list);
            List<int> result = new List<int>();
            for (int i = 0; i < _keywords.Length; i++) {
                var keywords = _keywords[i];
                if (keywords.Length < minLength) {
                    continue;
                }
                var fpy = _keywordsFirstPinyin[i];
                var pylist = _keywordsPinyin[i];
                if (search.Find(fpy, keywords, pylist)) {
                    if (_indexs == null) {
                        result.Add(i);
                    } else {
                        result.Add(_indexs[i]);
                    }
                }
            }
            return result;
        }

        #region pinyinSearch
        class PinyinSearch
        {
            TrieNode2[] _first;
            string[] _keywords;
            string[][] _keywordPinyins;

            public virtual void SetKeywords(List<Tuple<string, string[]>> keywords)
            {
                _keywords = new string[keywords.Count];
                _keywordPinyins = new string[keywords.Count][];
                for (int i = 0; i < keywords.Count; i++) {
                    _keywords[i] = keywords[i].Item1;
                    _keywordPinyins[i] = keywords[i].Item2;
                }
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

            public bool Find(string text, string hz, string[] pinyins)
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
                            foreach (var result in tn.Results) {
                                var keyword = _keywords[result];
                                var start = i + 1 - keyword.Length;
                                var end = i;
                                bool isok = true;
                                var keywordPinyins = _keywordPinyins[result];


                                for (int j = 0; j < keyword.Length; j++) {
                                    var idx = start + j;
                                    var py = keywordPinyins[j];
                                    if (py.Length == 1 && py[0] >= 0x3400 && py[0] <= 0x9fd5) {
                                        if (hz[idx] != py[0]) {
                                            isok = false;
                                            break;
                                        }
                                    } else {
                                        if (pinyins[idx].StartsWith(py) == false) {
                                            isok = false;
                                            break;
                                        }
                                    }
                                }
                                if (isok) {
                                    return true;
                                }
                            }
                        }
                    }
                    ptr = tn;
                }
                return false;
            }
        }



        #endregion


        #region 合并关键字

        private void MergeKeywords(string[] keys, int id, string keyword, List<Tuple<string, string[]>> list)
        {
            if (id >= keys.Length) {
                list.Add(Tuple.Create(keyword, keys));
                //list[keyword.Substring(1)] = keys;
                //list.Add(keyword.Substring(1));
                return;
            }
            var key = keys[id];
            if (key[0] >= 0x3400 && key[0] <= 0x9fd5) {
                var all = PinYinDict.GetAllPinYin(key[0]);
                var fpy = new HashSet<char>();
                foreach (var item in all) {
                    fpy.Add(item[0]);
                }
                foreach (var item in fpy) {
                    MergeKeywords(keys, id + 1, keyword + item, list);
                }
            } else {
                MergeKeywords(keys, id + 1, keyword + key[0], list);
            }
        }

        #endregion

        #region SplitKeywords
        /// <summary>
        /// 初步分割
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<string> SplitKeywords(string key)
        {
            InitPinyinSearch();
            List<TextNode> textNodes = new List<TextNode>();
            for (int i = 0; i <= key.Length; i++) { textNodes.Add(new TextNode()); }
            textNodes.Last().End = true;
            for (int i = 0; i < key.Length; i++) {
                TextLine line = new TextLine();
                line.Next = textNodes[i + 1];
                line.Words = key[i].ToString();
                textNodes[i].Children.Add(line);
            }

            var all = _wordsSearch.FindAll(key);
            foreach (var searchResult in all) {
                TextLine line = new TextLine();
                line.Next = textNodes[searchResult.End + 1];
                line.Words = searchResult.Keyword;
                textNodes[searchResult.Start].Children.Add(line);
            }

            List<string> list = new List<string>();
            BuildKsywords(textNodes[0], 0, "", list);
            return list;
        }
        private void BuildKsywords(TextNode textNode, int id, string keywords, List<string> list)
        {
            if (textNode.End) {
                list.Add(keywords.Substring(1));
                return;
            }
            foreach (var item in textNode.Children) {
                BuildKsywords(item.Next, id + 1, keywords + (char)0 + item.Words, list);
            }
        }

        class TextNode
        {
            public bool End;
            public List<TextLine> Children = new List<TextLine>();
        }
        class TextLine
        {
            public string Words;
            public TextNode Next;
        }

        #endregion

        #region InitPinyinSearch
        private static WordsSearch _wordsSearch;
        private void InitPinyinSearch()
        {
            if (_wordsSearch == null) {
                HashSet<string> allPinYins = new HashSet<string>();
                var pys = PinYinDict.PyShow;
                for (int i = 1; i < pys.Length; i++) {
                    allPinYins.Add(pys[i].ToUpper());
                }
                var wordsSearch = new WordsSearch();
                wordsSearch.SetKeywords(allPinYins.ToList());
                _wordsSearch = wordsSearch;
            }
        }
        #endregion
    }
}
