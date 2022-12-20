using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words.internals;

namespace ToolGood.Words.Benchmark.SearchExs
{
    public sealed class WordsSearchEx : BaseSearchEx
    {
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
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<WordsSearchResult> FindAll2(string text)
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
        public WordsSearchResult FindFirst(string text)
        {
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
                    var start = _end[next];
                    if (start < _end[next + 1]) {
                        var index = _resultIndex[start];
                        var len = _keywordLengths[index];
                        var st = i + 1 - len;
                        return new WordsSearchResult(ref text, st, i, index);
                    }
                }
                p = next;
            }
            return null;
        }
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public WordsSearchResult FindFirst2(string text)
        {
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
                        var index = _resultIndex[start];
                        var len = _keywordLengths[index];
                        var st = i + 1 - len;
                        return new WordsSearchResult(ref text, st, i, index);
                    }
                }
                p = next;
            }
            return null;
        }

    }
    
}
