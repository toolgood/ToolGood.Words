using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BasePinyinMatch
    {
        #region Hash
        const ulong lab_a = 0b1;
        const ulong lab_b = 0b10;
        const ulong lab_c = 0b100;
        const ulong lab_d = 0b1000;
        const ulong lab_e = 0b10000;
        const ulong lab_f = 0b100000;
        const ulong lab_g = 0b1000000;
        const ulong lab_h = 0b10000000;
        const ulong lab_i = 0b100000000;
        const ulong lab_j = 0b1000000000;
        const ulong lab_k = 0b10000000000;
        const ulong lab_l = 0b100000000000;
        const ulong lab_m = 0b1000000000000;
        const ulong lab_n = 0b10000000000000;
        const ulong lab_o = 0b100000000000000;
        const ulong lab_p = 0b1000000000000000;
        const ulong lab_q = 0b10000000000000000;
        const ulong lab_r = 0b100000000000000000;
        const ulong lab_s = 0b1000000000000000000;
        const ulong lab_t = 0b10000000000000000000;
        const ulong lab_u = 0b100000000000000000000;
        const ulong lab_v = 0b1000000000000000000000;
        const ulong lab_w = 0b10000000000000000000000;
        const ulong lab_x = 0b100000000000000000000000;
        const ulong lab_y = 0b1000000000000000000000000;
        const ulong lab_z = 0b10000000000000000000000000;
        const ulong lab_0 = 0b100000000000000000000000000;
        const ulong lab_1 = 0b1000000000000000000000000000;
        const ulong lab_2 = 0b10000000000000000000000000000;
        const ulong lab_3 = 0b100000000000000000000000000000;
        const ulong lab_4 = 0b1000000000000000000000000000000;
        const ulong lab_5 = 0b10000000000000000000000000000000;
        const ulong lab_6 = 0b100000000000000000000000000000000;
        const ulong lab_7 = 0b1000000000000000000000000000000000;
        const ulong lab_8 = 0b10000000000000000000000000000000000;
        const ulong lab_9 = 0b100000000000000000000000000000000000;
        const ulong lab_other = 0b1000000000000000000000000000000000000;

        const ulong len_1 = 0b10000000000000000000000000000000000000;
        const ulong len_2 = 0b110000000000000000000000000000000000000;
        const ulong len_3 = 0b1110000000000000000000000000000000000000;
        const ulong len_4 = 0b11110000000000000000000000000000000000000;
        const ulong len_5 = 0b111110000000000000000000000000000000000000;
        const ulong len_6 = 0b1111110000000000000000000000000000000000000;
        const ulong len_7 = 0b11111110000000000000000000000000000000000000;
        const ulong len_8 = 0b111111110000000000000000000000000000000000000;
        const ulong len_9 = 0b1111111110000000000000000000000000000000000000;
        const ulong len_10 = 0b11111111110000000000000000000000000000000000000;
        const ulong len_11 = 0b111111111110000000000000000000000000000000000000;
        const ulong len_12 = 0b1111111111110000000000000000000000000000000000000;
        const ulong len_13 = 0b11111111111110000000000000000000000000000000000000;
        const ulong len_14 = 0b111111111111110000000000000000000000000000000000000;
        const ulong len_15 = 0b1111111111111110000000000000000000000000000000000000;
        const ulong len_16 = 0b11111111111111110000000000000000000000000000000000000;
        const ulong len_17 = 0b111111111111111110000000000000000000000000000000000000;
        const ulong len_18 = 0b1111111111111111110000000000000000000000000000000000000;
        const ulong len_19 = 0b11111111111111111110000000000000000000000000000000000000;
        const ulong len_20 = 0b111111111111111111110000000000000000000000000000000000000;
        const ulong len_21 = 0b1111111111111111111110000000000000000000000000000000000000;
        const ulong len_22 = 0b11111111111111111111110000000000000000000000000000000000000;
        const ulong len_23 = 0b111111111111111111111110000000000000000000000000000000000000;
        const ulong len_24 = 0b1111111111111111111111110000000000000000000000000000000000000;
        const ulong len_25 = 0b11111111111111111111111110000000000000000000000000000000000000;
        const ulong len_26 = 0b111111111111111111111111110000000000000000000000000000000000000;
        const ulong len_27 = 0b1111111111111111111111111110000000000000000000000000000000000000;


        protected ulong BuildHashByChar(ulong hash, char c)
        {
            c = char.ToLower(c);
            if (c == 'a') hash = hash | lab_a;
            else if (c == 'b') hash = hash | lab_b;
            else if (c == 'c') hash = hash | lab_c;
            else if (c == 'd') hash = hash | lab_d;
            else if (c == 'e') hash = hash | lab_e;
            else if (c == 'f') hash = hash | lab_f;
            else if (c == 'g') hash = hash | lab_g;
            else if (c == 'h') hash = hash | lab_h;
            else if (c == 'i') hash = hash | lab_i;
            else if (c == 'j') hash = hash | lab_j;
            else if (c == 'k') hash = hash | lab_k;
            else if (c == 'l') hash = hash | lab_l;
            else if (c == 'm') hash = hash | lab_m;
            else if (c == 'n') hash = hash | lab_n;
            else if (c == 'o') hash = hash | lab_o;
            else if (c == 'p') hash = hash | lab_p;
            else if (c == 'q') hash = hash | lab_q;
            else if (c == 'r') hash = hash | lab_r;
            else if (c == 's') hash = hash | lab_s;
            else if (c == 't') hash = hash | lab_t;
            else if (c == 'u') hash = hash | lab_u;
            else if (c == 'v') hash = hash | lab_v;
            else if (c == 'w') hash = hash | lab_w;
            else if (c == 'x') hash = hash | lab_x;
            else if (c == 'y') hash = hash | lab_y;
            else if (c == 'z') hash = hash | lab_z;
            else if (c == '0') hash = hash | lab_0;
            else if (c == '1') hash = hash | lab_1;
            else if (c == '2') hash = hash | lab_2;
            else if (c == '3') hash = hash | lab_3;
            else if (c == '4') hash = hash | lab_4;
            else if (c == '5') hash = hash | lab_5;
            else if (c == '6') hash = hash | lab_6;
            else if (c == '7') hash = hash | lab_7;
            else if (c == '8') hash = hash | lab_8;
            else if (c == '9') hash = hash | lab_9;
            else hash = hash | lab_other;
            return hash;
        }

        protected ulong BuildHashByLength(ulong hash,int len)
        {
            if (len == 1) hash = hash | len_1;
            else if (len == 2) hash = hash | len_2;
            else if (len == 3) hash = hash | len_3;
            else if (len == 4) hash = hash | len_4;
            else if (len == 5) hash = hash | len_5;
            else if (len == 6) hash = hash | len_6;
            else if (len == 7) hash = hash | len_7;
            else if (len == 8) hash = hash | len_8;
            else if (len == 9) hash = hash | len_9;
            else if (len == 10) hash = hash | len_10;
            else if (len == 11) hash = hash | len_11;
            else if (len == 12) hash = hash | len_12;
            else if (len == 13) hash = hash | len_13;
            else if (len == 14) hash = hash | len_14;
            else if (len == 15) hash = hash | len_15;
            else if (len == 16) hash = hash | len_16;
            else if (len == 17) hash = hash | len_17;
            else if (len == 18) hash = hash | len_18;
            else if (len == 19) hash = hash | len_19;
            else if (len == 20) hash = hash | len_20;
            else if (len == 21) hash = hash | len_21;
            else if (len == 22) hash = hash | len_22;
            else if (len == 23) hash = hash | len_23;
            else if (len == 24) hash = hash | len_24;
            else if (len == 25) hash = hash | len_25;
            else if (len == 26) hash = hash | len_26;
            else hash = hash | len_27;
            return hash;
        }

        #endregion
 
        #region pinyinSearch
        public class PinyinSearch : BaseSearch
        {
            string[][] _keywordPinyins;
            int[] _indexs;

            public void SetKeywords(List<Tuple<string, string[]>> keywords)
            {
                _keywords = new string[keywords.Count];
                _keywordPinyins = new string[keywords.Count][];
                for (int i = 0; i < keywords.Count; i++)
                {
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
                for (int i = 0; i < text.Length; i++)
                {
                    TrieNode2 tn;
                    if (ptr == null)
                    {
                        tn = _first[text[i]];
                    }
                    else
                    {
                        if (ptr.TryGetValue(text[i], out tn) == false)
                        {
                            tn = _first[text[i]];
                        }
                    }
                    if (tn != null)
                    {
                        if (tn.End)
                        {
                            foreach (var result in tn.Results)
                            {
                                var keyword = _keywords[result];
                                var start = i + 1 - keyword.Length;
                                var end = i;
                                bool isok = true;
                                var keywordPinyins = _keywordPinyins[result];


                                for (int j = 0; j < keyword.Length; j++)
                                {
                                    var idx = start + j;
                                    var py = keywordPinyins[j];
                                    if (py.Length == 1 && py[0] >= 0x3400 && py[0] <= 0x9fd5)
                                    {
                                        if (hz[idx] != py[0])
                                        {
                                            isok = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pinyins[idx].StartsWith(py) == false)
                                        {
                                            isok = false;
                                            break;
                                        }
                                    }
                                }
                                if (isok)
                                {
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
                for (int i = 0; i < text.Length; i++)
                {
                    TrieNode2 tn;
                    if (ptr == null)
                    {
                        tn = _first[text[i]];
                    }
                    else
                    {
                        if (ptr.TryGetValue(text[i], out tn) == false)
                        {
                            tn = _first[text[i]];
                        }
                    }
                    if (tn != null)
                    {
                        if (tn.End)
                        {
                            foreach (var result in tn.Results)
                            {
                                var index = _indexs[result];
                                if (index != findCount) { continue; }

                                var keyword = _keywords[result];
                                var start = i + 1 - keyword.Length;
                                if (lastWordsIndex >= start) { continue; }

                                var end = i;
                                bool isok = true;
                                var keywordPinyins = _keywordPinyins[result];

                                for (int j = 0; j < keyword.Length; j++)
                                {
                                    var idx = start + j;
                                    var py = keywordPinyins[j];
                                    if (py.Length == 1 && py[0] >= 0x3400 && py[0] <= 0x9fd5)
                                    {
                                        if (hz[idx] != py[0])
                                        {
                                            isok = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pinyins[idx].StartsWith(py) == false)
                                        {
                                            isok = false;
                                            break;
                                        }
                                    }
                                }
                                if (isok)
                                {
                                    findCount++;
                                    lastWordsIndex = i;
                                    if (findCount == keysCount)
                                    {
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
            if (id >= keys.Length)
            {
                list.Add(Tuple.Create(keyword, keys));
                return;
            }
            var key = keys[id];
            if (key[0] >= 0x3400 && key[0] <= 0x9fd5)
            {
                var all = PinyinDict.GetAllPinyin(key[0]);
                var fpy = new HashSet<char>();
                foreach (var item in all)
                {
                    fpy.Add(item[0]);
                }
                foreach (var item in fpy)
                {
                    MergeKeywords(keys, id + 1, keyword + item, list);
                }
            }
            else
            {
                MergeKeywords(keys, id + 1, keyword + key[0], list);
            }
        }
        protected void MergeKeywords(string[] keys, int id, string keyword, List<Tuple<string, string[]>> list, int index, List<int> indexs)
        {
            if (id >= keys.Length)
            {
                list.Add(Tuple.Create(keyword, keys));
                indexs.Add(index);
                return;
            }
            var key = keys[id];
            if (key[0] >= 0x3400 && key[0] <= 0x9fd5)
            {
                var all = PinyinDict.GetAllPinyin(key[0]);
                var fpy = new HashSet<char>();
                foreach (var item in all)
                {
                    fpy.Add(item[0]);
                }
                foreach (var item in fpy)
                {
                    MergeKeywords(keys, id + 1, keyword + item, list, index, indexs);
                }
            }
            else
            {
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
            for (int i = 0; i < key.Length; i++)
            {
                TextLine line = new TextLine();
                line.Next = textNodes[i + 1];
                line.Words = key[i].ToString();
                textNodes[i].Children.Add(line);
            }

            var all = _wordsSearch.FindAll(key);
            foreach (var searchResult in all)
            {
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
            if (textNode.End)
            {
                list.Add(keywords.Substring(1));
                return;
            }
            foreach (var item in textNode.Children)
            {
                BuildKsywords(item.Next, id + 1, keywords + (char) 0 + item.Words, list);
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
            if (_wordsSearch == null)
            {
                HashSet<string> allPinyins = new HashSet<string>();
                var pys = PinyinDict.PyShow;
                for (int i = 1; i < pys.Length; i += 2)
                {
                    var py = pys[i].ToUpper();
                    for (int j = 1; j <= py.Length; j++)
                    {
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
