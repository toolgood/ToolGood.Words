using System.Collections.Generic;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索（增强版，速度更快），如果关键字太多(5W以上)，建议使用 StringSearchEx
    /// 性能从小到大  StringSearch &lt; StringSearchEx &lt; StringSearchEx2 &lt; StringSearchEx3
    /// </summary>
    public class StringSearchEx2 : BaseSearchEx2
    {
        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<string> FindAll(string text)
        {
            List<string> root = new List<string>();
            var p = 0;

            foreach (char t1 in text) {
                var t = (char)_dict[t1];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    var index = _check[next];
                    if (index > 0) {
                        foreach (var item in _guides[index]) {
                            root.Add(_keywords[item]);
                        }
                    }
                    p = next;
                }
            }
            return root;
        }
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public string FindFirst(string text)
        {
            var p = 0;
            foreach (char t1 in text) {
                var t = (char)_dict[t1];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                if (_key[next] == t) {
                    var index = _check[next];
                    if (index > 0) {
                        return _keywords[_guides[index][0]];
                    }
                    p = next;
                } else {
                    p = 0;
                    next = _next[p] + t;
                    if (_key[next] == t) {
                        var index = _check[next];
                        if (index > 0) {
                            return _keywords[_guides[index][0]];
                        }
                        p = next;
                    }
                }
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
                var t = (char)_dict[t1];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                if (_key[next] == t) {
                    if (_check[next] > 0) { return true; }
                    p = next;
                } else {
                    p = 0;
                    next = _next[p] + t;
                    if (_key[next] == t) {
                        if (_check[next] > 0) { return true; }
                        p = next;
                    }
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
                var t = (char)_dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[p] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    var index = _check[next];
                    if (index > 0) {
                        var maxLength = _keywords[_guides[index][0]].Length;
                        var start = i + 1 - maxLength;
                        for (int j = start; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                    p = next;
                }
            }
            return result.ToString();
        }
        #endregion



    }
}
