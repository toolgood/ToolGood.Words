using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    public class WordsMatchEx : BaseMatchEx
    {
        #region FindFirst
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public WordsSearchResult FindFirst(string text)
        {
            var p = 0;
            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];

                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        var r = FindFirst(text, i + 1, _wildcard[p]);
                        if (r != null) {
                            return r;
                        }
                    }
                    next = _firstIndex[t];
                }
                if (next != 0) {
                    var start = _end[next];
                    if (start < _end[next + 1]) {
                        var idx = _resultIndex[start];
                        var length = _keywordLength[idx];
                        var start2 = i - length + 1;
                        if (start2 >= 0) {
                            var kIndex = _keywordIndex[idx];
                            var matchKeyword = _matchKeywords[kIndex];
                            var keyword = text.Substring(start2, length);
                            return new WordsSearchResult(keyword, start2, i, kIndex, matchKeyword);
                        }
                    }
                }
                p = next;
            }
            return null;
        }

        private WordsSearchResult FindFirst(string text, int index, int p)
        {
            for (int i = index; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];
                if (t == 0) {
                    return null;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        return FindFirst(text, i + 1, _wildcard[p]);
                    }
                    return null;
                }
                var start = _end[next];
                if (start < _end[next + 1]) {
                    var idx = _resultIndex[start];
                    var length = _keywordLength[idx];
                    var start2 = i - length + 1;
                    if (start2 >= 0) {
                        var kIndex = _keywordIndex[idx];
                        var matchKeyword = _matchKeywords[kIndex];
                        var keyword = text.Substring(start2, length);
                        return new WordsSearchResult(keyword, start2, i, kIndex, matchKeyword);
                    }
                }
                p = next;
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
            List<WordsSearchResult> result = new List<WordsSearchResult>();
            var p = 0;

            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        FindAll(text, i + 1, _wildcard[p], result);
                    }
                    next = _firstIndex[t];
                }
                if (next != 0) {

                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        var idx = _resultIndex[j];
                        var length = _keywordLength[idx];
                        var start = i - length + 1;
                        if (start >= 0) {
                            var kIndex = _keywordIndex[idx];
                            var matchKeyword = _matchKeywords[kIndex];
                            var keyword = text.Substring(start, length);
                            var r = new WordsSearchResult(keyword, start, i, kIndex, matchKeyword);
                            result.Add(r);
                        }


                    }
                }
                p = next;
            }
            return result;
        }
        private void FindAll(string text, int index, int p, List<WordsSearchResult> result)
        {
            for (int i = index; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];
                if (t == 0) {
                    return;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        FindAll(text, i + 1, _wildcard[p], result);
                    }
                    return;
                }
                for (int j = _end[next]; j < _end[next + 1]; j++) {
                    var idx = _resultIndex[j];
                    var length = _keywordLength[idx];
                    var start = i - length + 1;
                    if (start >= 0) {
                        var kIndex = _keywordIndex[idx];
                        var matchKeyword = _matchKeywords[kIndex];
                        var keyword = text.Substring(start, length);
                        var r = new WordsSearchResult(keyword, start, i, kIndex, matchKeyword);
                        result.Add(r);
                    }
                }
                p = next;
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
            var p = 0;
            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];

                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        var r = ContainsAny(text, i + 1, _wildcard[p]);
                        if (r) {
                            return true;
                        }
                    }
                    next = _firstIndex[t];
                }
                if (next != 0) {
                    if (_end[next] < _end[next + 1]) {
                        return true;
                    }
                }
                p = next;
            }
            return false;
        }

        private bool ContainsAny(string text, int index, int p)
        {
            for (int i = index; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];
                if (t == 0) {
                    return false;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        return ContainsAny(text, i + 1, _wildcard[p]);
                    }
                    return false;
                }
                var start = _end[next];
                if (start < _end[next + 1]) {
                    var length = _keywordLength[_resultIndex[start]];
                    var s = i - length + 1;
                    if (s >= 0) {
                        return true;
                    }
                }
                p = next;
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

            var p = 0;

            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];

                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        Replace(text, i + 1, _wildcard[p], replaceChar, result);
                    }
                    next = _firstIndex[t];
                }
                if (next != 0) {
                    var start = _end[next];
                    if (start < _end[next + 1]) {
                        var maxLength = _keywordLength[_resultIndex[start]];
                        var start2 = i + 1 - maxLength;
                        if (start2 >= 0) {
                            for (int j = start2; j <= i; j++) {
                                result[j] = replaceChar;
                            }
                        }
                    }
                }
                p = next;
            }
            return result.ToString();
        }

        private void Replace(string text, int index, int p, char replaceChar, StringBuilder result)
        {
            for (int i = index; i < text.Length; i++) {
                var t1 = text[i];
                var t = _dict[t1];
                if (t == 0) {
                    return;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (_wildcard[p] > 0) {
                        Replace(text, i + 1, _wildcard[p], replaceChar, result);
                    }
                    return;
                }
                var start = _end[next];
                if (start < _end[next + 1]) {
                    var maxLength = _keywordLength[_resultIndex[start]];
                    var start2 = i + 1 - maxLength;
                    if (start2 >= 0) {
                        for (int j = start2; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                p = next;
            }
        }

        #endregion

    }
}
