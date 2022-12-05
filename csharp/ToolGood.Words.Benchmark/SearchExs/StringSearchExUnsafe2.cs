using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words.internals;

namespace ToolGood.Words.Benchmark.SearchExs
{
    public sealed class StringSearchExUnsafe2 : BaseSearchEx
    {
        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe List<string> FindAll(string text)
        {
            List<string> result = new List<string>();
            var txt = text.AsSpan();
            var p = 0;
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
                        for (int j = end[next]; j < end[next + 1]; j++) {
                            var index = resultIndex[j];
                            var len = keywordLengths[index];
                            var key = txt.Slice(i + 1 - len, len).ToString();
                            result.Add(key);
                        }
                    }
                    p = next;
                }
            }
            return result;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe string FindFirst(string text)
        {
            var txt = text.AsSpan();
            fixed (int* first = &_first[0])
            fixed (int* end = &_end[0])
            fixed (ushort* dict = &_dict[0])
            fixed (int* resultIndex = &_resultIndex[0])
            fixed (int* keywordLengths = &_keywordLengths[0]) {
                var p = 0;
                for (int i = 0; i < txt.Length; i++) {
                    var t = _dict[txt[i]];
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
                            return txt.Slice(i + 1 - len, len).ToString();
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
        public unsafe string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);
            var p = 0;
            fixed (int* first = &_first[0])
            fixed (int* end = &_end[0])
            fixed (ushort* dict = &_dict[0])
            fixed (int* resultIndex = &_resultIndex[0])
            fixed (int* keywordLengths = &_keywordLengths[0]) {
                for (int i = 0; i < text.Length; i++) {
                    var t = dict[text[i]];
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
                            var maxLength = keywordLengths[resultIndex[start]];
                            for (int j = i + 1 - maxLength; j <= i; j++) {
                                result[j] = replaceChar;
                            }
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
