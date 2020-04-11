using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;


namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索
    /// 性能从小到大  StringSearch &lt; StringSearchEx &lt; StringSearchEx2 &lt; StringSearchEx3
    /// </summary>
    public class StringSearch : BaseSearch
    {
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public string FindFirst(string text)
        {
            TrieNode2 ptr = null;
            foreach (char t in text) {
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        return _keywords[tn.Results[0]];
                    }
                }
                ptr = tn;
            }
            return null;
        }
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<string> FindAll(string text)
        {
            TrieNode2 ptr = null;
            List<string> list = new List<string>();

            foreach (char t in text) {
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.TryGetValue(t, out tn) == false) {
                        tn = _first[t];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            list.Add(_keywords[item]);
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool ContainsAny(string text)
        {
            TrieNode2 ptr = null;
            foreach (char t in text) {
                TrieNode2 tn;
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
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);

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
                        var maxLength = _keywords[tn.Results[0]].Length;
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

    }
}
