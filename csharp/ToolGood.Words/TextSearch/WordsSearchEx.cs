using System.Collections.Generic;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索，带返回位置及索引号，内存版，保存快，
    /// 性能从小到大  WordsSearch &lt; WordsSearchEx &lt; WordsSearchEx2 &lt; WordsSearchEx3
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

            for (int i = 0; i < text.Length; i++) {
                var t = _dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    next = _first[t];
                }
                if (next != 0) {
                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        var index = _resultIndex[j];
                        var key = _keywords[index];
                        var r = new WordsSearchResult(key, i + 1 - key.Length, i, index);
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
        public WordsSearchResult FindFirst(string text)
        {
            var p = 0;
            var length = text.Length;
            for (int i = 0; i < length; i++) {
                var t = _dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    next = _first[t];
                }
                if (next != 0) {
                    var start = _end[next];
                    if (start < _end[next + 1]) {
                        var index = _resultIndex[start];
                        var key = _keywords[index];
                        return new WordsSearchResult(key, i + 1 - key.Length, i, index);
                    }
                }
                p = next;
            }
            return null;
        }

        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool ContainsAny(string text)
        {
            var p = 0;
            foreach (char t1 in text) {
                var t = _dict[t1];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    next = _first[t];
                }
                if (next > 0) {
                    if (_end[next] < _end[next + 1]) {
                        return true;
                    }
                }
                p = next;
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
                if (p == 0 ||  _nextIndex[p].TryGetValue(t, out next) == false) {
                    next = _first[t];
                }
                if (next != 0) {
                    var start = _end[next];
                    if (start < _end[next + 1]) {
                        var maxLength = _keywords[_resultIndex[start]].Length;
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
