using ToolGood.Words.internals;

namespace ToolGood.Words.Benchmark.SearchExs
{
    public sealed class WordsSearchExUnsafe2 : BaseSearchEx
    {
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public unsafe List<WordsSearchResult> FindAll(string text)
        {
            List<WordsSearchResult> result = new List<WordsSearchResult>();
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
                        for (int j = end[next]; j < end[next + 1]; j++) {
                            var index = resultIndex[j];
                            var len = keywordLengths[index];
                            var st = i + 1 - len;
                            var r = new WordsSearchResult(ref text, st, i, index);
                            result.Add(r);
                        }
                    }
                    p = next;
                }
            }
            return result;
        }

        public unsafe List<WordsSearchResult> FindAll2(string text)
        {
            List<WordsSearchResult> result = new List<WordsSearchResult>();
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
                        for (int j = end[next]; j < end[next + 1]; j++) {
                            var index = resultIndex[j];
                            var len = keywordLengths[index];
                            var st = i + 1 - len;
                            var r = new WordsSearchResult(ref text, st, i, index);
                            result.Add(r);
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

        public unsafe WordsSearchResult FindFirst2(string text)
        {
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

    }
    
}
