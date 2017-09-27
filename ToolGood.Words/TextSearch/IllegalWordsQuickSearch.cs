using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 脏字搜索类，极速版
    /// </summary>
    public class IllegalWordsQuickSearch : IllegalWordsSearch
    {
        public IllegalWordsQuickSearch(int jumpLength = 1) : base(jumpLength) { }

        #region ContainsAny
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public override bool ContainsAny(string text)
        {
            bool r = false;
            search(text, (keyword, ch, end) => {
                r = !isInEnglishOrInNumber(keyword, ch, end, text);
                return r;
            });
            if (r) return true;
            var searchText = WordsHelper.ToSenseIllegalWords(text);
            search(searchText, (keyword, ch, end) => {
                r = !isInEnglishOrInNumber(keyword, ch, end, searchText);
                return r;
            });
            if (r) return true;
            searchText = WordsHelper.RemoveNontext(searchText);
            search(searchText, (keyword, ch, end) => {
                r = !isInEnglishOrInNumber(keyword, ch, end, searchText);
                return r;
            });
            return r;
        }
        #endregion

        #region FindFirst
        /// <summary>
        /// 从文本中查找到第一个关键字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override IllegalWordsSearchResult FindFirst(string text)
        {
            IllegalWordsSearchResult result = null;
            search(text, (keyword, ch, end) => {
                var start = end + 1 - keyword.Length;
                result = GetIllegalResult(keyword, ch, start, end, text, text);
                return result != null;
            });
            if (result != null) return result;
            var searchText = WordsHelper.ToSenseIllegalWords(text);
            search(searchText, (keyword, ch, end) => {
                var start = end + 1 - keyword.Length;
                result = GetIllegalResult(keyword, ch, start, end, text, searchText);
                return result != null;
            });
            if (result != null) return result;
            searchText = WordsHelper.RemoveNontext(searchText);
            search(searchText, (keyword, ch, end) => {
                var start = end;
                for (int i = 0; i < keyword.Length; i++) {
                    var n = searchText[start--];
                    while (n == 1) { n = searchText[start--]; }
                }
                start++;
                result = GetIllegalResult(keyword, ch, start, end, text, searchText);
                return result != null;
            });
            if (result != null) return result;
            return IllegalWordsSearchResult.Empty;
        }

        #endregion

        #region FindAll
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public override List<IllegalWordsSearchResult> FindAll(string text)
        {
            List<IllegalWordsSearchResult> newlist = new List<IllegalWordsSearchResult>();
            string searchText = WordsHelper.ToSenseIllegalWords(text);
            searchAll(searchText, (keyword, ch, end) => {
                var start = end + 1 - keyword.Length;
                var r = GetIllegalResult(keyword, ch, start, end, text, searchText);
                if (r != null) newlist.Add(r);
            });
            searchText = removeChecks(searchText, newlist);
            //list.AddRange(newlist);
            //newlist.Clear();
            searchText = WordsHelper.RemoveNontext(searchText);
            searchAll(searchText, (keyword, ch, end) => {
                var start = end;
                for (int i = 0; i < keyword.Length; i++) {
                    var n = searchText[start--];
                    while (n == 1) { n = searchText[start--]; }
                }
                start++;
                var r = GetIllegalResult(keyword, ch, start, end, text, searchText);
                if (r != null) newlist.Add(r);
            });
            return newlist;
        }
        #endregion

        #region isInEnglishOrInNumber  search searchAll GetIllegalResult

        private IllegalWordsSearchResult GetIllegalResult(string keyword, char ch, int start, int end, string srcText, string searchText)
        {
            if (end < searchText.Length - 1) {
                if (ch < 127) {
                    var c = searchText[end + 1];
                    if (c < 127) {
                        int d = bitType[c] + bitType[ch];
                        if (d == 98 || d == 194) {
                            return null;
                        }

                    }
                }
            }
            if (start > 0) {
                var c = searchText[start - 1];
                if (c < 127) {
                    var k = keyword[0];
                    if (k < 127) {
                        int d = bitType[c] + bitType[k];
                        if (d == 98 || d == 194) {
                            return null;
                        }
                    }
                }
            }
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
        }

        // func :关键字，结尾字符，结尾索引，是否退出搜索
        private void search(string text, Func<string, char, int, bool> func)
        {
            TrieNode ptr = null;
            int jumpCount = 0;
            for (int i = 0; i < text.Length; i++) {
                var ch = text[i];
                if (ch == 1) {
                    jumpCount++;
                    if (jumpCount > _jumpLength) { jumpCount = 0; ptr = null; }
                    continue;
                }
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[ch];
                } else {
                    if (ptr.TryGetValue(ch, out tn) == false) {
                        tn = _first[ch];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            var r = func(item, ch, i);
                            if (r) return;
                        }
                    }
                }
                ptr = tn;
            }
        }
        private void searchAll(string text, Action<string, char, int> action)
        {
            TrieNode ptr = null;
            int jumpCount = 0;
            for (int i = 0; i < text.Length; i++) {
                var ch = text[i];
                if (ch == 1) {
                    jumpCount++;
                    if (jumpCount > _jumpLength) { jumpCount = 0; ptr = null; }
                    continue;
                }
                jumpCount = 0;
                if (ch == 0) { ptr = null; continue; }


                TrieNode tn;
                if (ptr == null) {
                    tn = _first[ch];
                } else {
                    if (ptr.TryGetValue(ch, out tn) == false) {
                        tn = _first[ch];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            action(item, ch, i);
                        }
                    }
                }
                ptr = tn;
            }
        }

        // 删除 已符合数据
        private string removeChecks(string text, List<IllegalWordsSearchResult> results)
        {
            StringBuilder sb = new StringBuilder(text);
            foreach (var r in results) {
                for (int i = r.Start; i <= r.End; i++) {
                    sb[i] = (char)0;
                }
            }
            return sb.ToString();
        }

        #endregion


    }
}
