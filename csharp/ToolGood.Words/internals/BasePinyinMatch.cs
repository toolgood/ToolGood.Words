using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BasePinyinMatch
    {


        #region pinyinSearch
        public class PinyinSearch : BaseSearch
        {
            string[][] _keywordPinyins;
            int[] _indexs;

            public void SetKeywords(List<Tuple<string, string[]>> keywords)
            {
                _keywords = new string[keywords.Count];
                _keywordPinyins = new string[keywords.Count][];
                for (int i = 0; i < keywords.Count; i++) {
                    _keywords[i] = keywords[i].Item1;
                    _keywordPinyins[i] = keywords[i].Item2;
                }
                SetKeywords();
            }

            public void SetIndexs(int[] indexs)
            {
                _indexs = indexs;
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

            public bool Find2(string text, string hz, string[] pinyins, int keysCount)
            {
                int findCount = 0;
                int lastWordsIndex = -1;
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
                                var index = _indexs[result];
                                if (index != findCount) { continue; }

                                var keyword = _keywords[result];
                                var start = i + 1 - keyword.Length;
                                if (lastWordsIndex >= start) { continue; }

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
                                    findCount++;
                                    lastWordsIndex = i;
                                    if (findCount == keysCount) {
                                        return true;
                                    }
                                    break;
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

        protected void MergeKeywords(string[] keys, int id, string keyword, List<Tuple<string, string[]>> list)
        {
            if (id >= keys.Length) {
                list.Add(Tuple.Create(keyword, keys));
                return;
            }
            var key = keys[id];
            if (key[0] >= 0x3400 && key[0] <= 0x9fd5) {
                var all = PinyinDict.GetAllPinyin(key[0]);
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
        protected void MergeKeywords(string[] keys, int id, string keyword, List<Tuple<string, string[]>> list, int index, List<int> indexs)
        {
            if (id >= keys.Length) {
                list.Add(Tuple.Create(keyword, keys));
                indexs.Add(index);
                return;
            }
            var key = keys[id];
            if (key[0] >= 0x3400 && key[0] <= 0x9fd5) {
                var all = PinyinDict.GetAllPinyin(key[0]);
                var fpy = new HashSet<char>();
                foreach (var item in all) {
                    fpy.Add(item[0]);
                }
                foreach (var item in fpy) {
                    MergeKeywords(keys, id + 1, keyword + item, list, index, indexs);
                }
            } else {
                MergeKeywords(keys, id + 1, keyword + key[0], list, index, indexs);
            }
        }

        #endregion

        #region SplitKeywords
        /// <summary>
        /// 初步分割
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected List<string> SplitKeywords(string key)
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
            list = list.Distinct().ToList();
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
                HashSet<string> allPinyins = new HashSet<string>();
                var pys = PinyinDict.PyShow;
                for (int i = 1; i < pys.Length; i += 2) {
                    var py = pys[i].ToUpper();
                    for (int j = 1; j <= py.Length; j++) {
                        var key = py.Substring(0, j);
                        allPinyins.Add(key);
                    }
                }
                var wordsSearch = new WordsSearch();
                wordsSearch.SetKeywords(allPinyins.ToList());
                _wordsSearch = wordsSearch;
            }
        }
        #endregion
    }
}
