using System;
using System.Collections.Generic;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索，带返回位置及索引号，内存版，保存快，
    /// 性能从小到大  WordsSearch &lt; WordsSearchEx &lt; WordsSearchEx2 &lt; WordsSearchEx3
    /// 最新版本的WordsSearchEx， 与3.1.0.0以前的版本不兼容。
    /// </summary>
    public class WordsSearchEx : BaseSearchEx
    {
        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<WordsSearchResult> FindAll(string text)
        {
            List<WordsSearchResult> result = new List<WordsSearchResult>();
            var p = 0;
            var txt = text.AsSpan();
            for (int i = 0; i < txt.Length; i++) {
                var t = _dict[txt[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    next = _first[t];
                }
                if (next != 0) {
                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        var index = _resultIndex[j];
                        var len = _keywordLengths[index];
                        var st = i + 1 - len;
                        var r = new WordsSearchResult(ref text, st, i, index);
                        result.Add(r);
                    }
                }
                p = next;
            }
            return result;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe WordsSearchResult FindFirst(string text)
        {
            var p = 0;
            var txt = text.AsSpan();
            fixed (int* first = &_first[0])
            fixed (int* end = &_end[0])
            fixed (ushort* dict = &_dict[0])
            fixed (int* resultIndex = &_resultIndex[0])
            fixed (int* keywordLengths = &_keywordLengths[0]) {
                for (int i = 0; i < txt.Length; i++) {
                    var t = dict[txt[i]];
                    if (t == 0) {
                        p = 0;
                        continue;
                    }
                    int next;
                    if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                        next = first[t];
                    }
                    if (next != 0) {
                        var start = end[next];
                        if (start < end[next + 1]) {
                            var index = resultIndex[start];
                            var len = keywordLengths[index];
                            var st = i + 1 - len;
                            return new WordsSearchResult(ref text, st, i, index);
                        }
                    }
                    p = next;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe bool ContainsAny(string text)
        {
            var p = 0;
            fixed (int* first = &_first[0])
            fixed (int* end = &_end[0])
            fixed (ushort* dict = &_dict[0]) {
                foreach (char t1 in text) {
                    var t = dict[t1];
                    if (t == 0) {
                        p = 0;
                        continue;
                    }
                    int next;
                    if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                        next = first[t];
                    }
                    if (next != 0) {
                        if (end[next] < end[next + 1]) {
                            return true;
                        }
                    }
                    p = next;
                }
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

            var p = 0;
            for (int i = 0; i < text.Length; i++) {
                var t = _dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    next = _first[t];
                }
                if (next != 0) {
                    var start = _end[next];
                    if (start < _end[next + 1]) {
                        var maxLength = _keywordLengths[_resultIndex[start]];
                        for (int j = i + 1 - maxLength; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                p = next;
            }
            return result.ToString();
        }

        #endregion



    }
}
