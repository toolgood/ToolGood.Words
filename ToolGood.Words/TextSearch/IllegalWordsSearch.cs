using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    public class IllegalWordsSearch: IllegalWordsSearchBase
    {
        #region 私有变量
        /// <summary>
        /// 使用重复词过滤器
        /// </summary>
        public bool UseDuplicateWordFilter = false;
 
        /// <summary>
        /// 使用半角转化器，默认为true
        /// </summary>
        public bool UseDBCcaseConverter = true;
        /// <summary>
        /// 使用简体中文转化器，默认为true
        /// </summary>
        public bool UseSimplifiedChineseConverter = true;

        #endregion

        public IllegalWordsSearch():base()
        {
  
        }

        #region 查找 替换 查找第一个关键字 判断是否包含关键字

        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public List<IllegalWordsSearchResult> FindAll(string text, BlacklistType blacklist = BlacklistType.All)
        {
            return FindAll(text, (int)blacklist);
        }
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public List<IllegalWordsSearchResult> FindAll(string text, int flag)
        {
            List<IllegalWordsSearchResult> results = new List<IllegalWordsSearchResult>();
            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            char pChar = (char)0;
            bool pCharEN = false;

            for (int i = 0; i < text.Length; i++) {
                pIndex[i] = p;
                var c = text[i];
                var cEN = IsEnglishOrNumber(c);
                if (findIndex != 0 && (!pCharEN || !cEN)) {//查找数据 返回
                    if (TestBlacklist(findIndex, flag)) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(_keywords[item], i - 1, text, p, pIndex);
                            results.Add(r);
                        }
                    }
                }
                if (p == 0 && pCharEN && cEN) { findIndex = 0; pChar = c; continue; }//避免 bc 匹配到 abc
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; pChar = c; pCharEN = cEN; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; findIndex = 0; pChar = c; pCharEN = cEN; continue; }//不在字表中，跳过



                var next = _next[p] + t;
                if (_key[next] == t) {
                    p = next;
                    findIndex = _check[next];
                } else if (UseDuplicateWordFilter && c == pChar) {
                    continue;
                } else if (pCharEN == false) {
                    next = _next[0] + t;
                    if (_key[next] == t) {
                        p = next;
                        findIndex = _check[next];
                    } else {
                        p = 0;
                        findIndex = 0;
                    }
                } else {
                    p = 0;
                    findIndex = 0;
                }
                pChar = c;
                pCharEN = cEN;
            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    foreach (var item in _guides[findIndex]) {
                        var r = GetIllegalResult(_keywords[item], text.Length - 1, text, p, pIndex);
                        results.Add(r);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public IllegalWordsSearchResult FindFirst(string text, BlacklistType blacklist = BlacklistType.All)
        {
            return FindFirst(text, (int)blacklist);
        }
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public IllegalWordsSearchResult FindFirst(string text, int flag)
        {
            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            char pChar = (char)0;
            bool pCharEN = false;

            for (int i = 0; i < text.Length; i++) {
                pIndex[i] = p;
                var c = text[i];
                var cEN = IsEnglishOrNumber(c);
                if (findIndex != 0 && (!pCharEN || !cEN)) {//查找数据 返回
                    if (TestBlacklist(findIndex, flag)) {
                        return GetIllegalResult(_keywords[_guides[findIndex][0]], i - 1, text, p, pIndex);
                    }
                }
                if (p == 0 && pCharEN && cEN) { findIndex = 0; pChar = c; continue; }//避免 bc 匹配到 abc
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; pChar = c; pCharEN = cEN; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; findIndex = 0; pChar = c; pCharEN = cEN; continue; }//不在字表中，跳过


                var next = _next[p] + t;
                if (_key[next] == t) {
                    p = next;
                    findIndex = _check[next];
                } else if (UseDuplicateWordFilter && c == pChar) {
                    continue;
                } else if (pCharEN == false) {
                    next = _next[0] + t;
                    if (_key[next] == t) {
                        p = next;
                        findIndex = _check[next];
                    } else {
                        p = 0;
                        findIndex = 0;
                    }
                } else {
                    p = 0;
                    findIndex = 0;
                }
                pChar = c;
                pCharEN = cEN;
            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    return GetIllegalResult(_keywords[_guides[p][0]], text.Length - 1, text, p, pIndex);
                }
            }
            return null;
        }

        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public bool ContainsAny(string text, BlacklistType blacklist = BlacklistType.All)
        {
            return ContainsAny(text, (int)blacklist);
        }
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public bool ContainsAny(string text, int flag)
        {
            var p = 0;
            int findIndex = 0;
            char pChar = (char)0;
            bool pCharEN = false;

            for (int i = 0; i < text.Length; i++) {
                var c = text[i];
                var cEN = IsEnglishOrNumber(c);
                if (findIndex != 0 && (!pCharEN || !cEN)) {//查找数据 返回
                    if (TestBlacklist(findIndex, flag)) {
                        return true;
                    }
                }
                if (p == 0 && pCharEN && cEN) { findIndex = 0; pChar = c; continue; }//避免 bc 匹配到 abc
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; pChar = c; pCharEN = cEN; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; findIndex = 0; pChar = c; pCharEN = cEN; continue; }//不在字表中，跳过


                var next = _next[p] + t;
                if (_key[next] == t) {
                    p = next;
                    findIndex = _check[next];
                } else if (UseDuplicateWordFilter && c == pChar) {
                    continue;
                } else if (pCharEN == false) {
                    next = _next[0] + t;
                    if (_key[next] == t) {
                        p = next;
                        findIndex = _check[next];
                    } else {
                        p = 0;
                        findIndex = 0;
                    }
                } else {
                    p = 0;
                    findIndex = 0;
                }
                pChar = c;
                pCharEN = cEN;
            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*', BlacklistType blacklist = BlacklistType.All)
        {
            return Replace(text, replaceChar, (int)blacklist);
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar, int flag)
        {
            StringBuilder result = new StringBuilder(text);

            int[] pIndex = new int[text.Length];
            int p = 0;
            int findIndex = 0;
            char pChar = (char)0;
            bool pCharEN = false;

            for (int i = 0; i < text.Length; i++) {
                pIndex[i] = p;
                var c = text[i];
                var cEN = IsEnglishOrNumber(c);
                if (findIndex != 0 && (!pCharEN || !cEN)) {//查找数据 返回
                    if (TestBlacklist(findIndex, flag)) {
                        var keyword = _keywords[_guides[findIndex][0]];
                        var start = FindStart(keyword, i - 1, p, pIndex);
                        for (int j = start; j < i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                if (p == 0 && pCharEN && cEN) { findIndex = 0; pChar = c; continue; }//避免 bc 匹配到 abc
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; pChar = c; pCharEN = cEN; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; findIndex = 0; pChar = c; pCharEN = cEN; continue; }//不在字表中，跳过


                var next = _next[p] + t;
                if (_key[next] == t) {
                    p = next;
                    findIndex = _check[next];
                } else if (UseDuplicateWordFilter && c == pChar) {
                    continue;
                } else if (pCharEN == false) {
                    next = _next[0] + t;
                    if (_key[next] == t) {
                        p = next;
                        findIndex = _check[next];
                    } else {
                        p = 0;
                        findIndex = 0;
                    }
                } else {
                    p = 0;
                    findIndex = 0;
                }
                pChar = c;
                pCharEN = cEN;
            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    var keyword = _keywords[_guides[findIndex][0]];
                    var start = FindStart(keyword, text.Length - 1, p, pIndex);
                    for (int j = start; j < text.Length; j++) {
                        result[j] = replaceChar;
                    }
                }
            }
            return result.ToString();
        }

        private int FindStart(string keyword, int end, int p, int[] pIndex)
        {
            var n = keyword.Length;
            var start = end;
            int pp = p;
            while (n > 0) {
                var pi = pIndex[start--];
                if (pi != pp) { n--; pp = pi; }
                if (start == -1) break;
            }
            start++;
            return start;
        }

        private IllegalWordsSearchResult GetIllegalResult(string keyword, int end, string srcText, int p, int[] pIndex)
        {
            var start = FindStart(keyword, end, p, pIndex);
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
        }

        private bool TestBlacklist(int index, int flag)
        {
            if (UseBlacklistFilter) {
                var b = _blacklist[index];
                return (b | flag) == b;
            }
            return true;
        }

        private char ToSenseWord(char c)
        {
            if (c >= 'A' && c <= 'Z') return (char)(c | 0x20);
            if (UseDBCcaseConverter) {
                if (c == 12288) return ' ';
                if (c >= 65280 && c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') {
                        k = k | 0x20;
                    }
                    return (char)k;
                }
            }
            if (UseSimplifiedChineseConverter) {
                if (c >= 0x4e00 && c <= 0x9fa5) {
                    return Dict.Simplified[c - 0x4e00];
                }
            }
            return c;
        }

        private string ToSenseWord(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++) {
                stringBuilder.Append(ToSenseWord(text[i]));
                //stringBuilder[i] = ToSenseWord(text[i]);
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region 保存到文件
 
        protected internal override void Save(BinaryWriter bw)
        {
            base.Save(bw);
            bw.Write(UseDuplicateWordFilter);
            bw.Write(UseDBCcaseConverter);
            bw.Write(UseSimplifiedChineseConverter);
        }

        #endregion

        #region 加载文件
 
        protected internal override void Load(BinaryReader br)
        {
            base.Load(br);
            UseDuplicateWordFilter = br.ReadBoolean();
            UseDBCcaseConverter = br.ReadBoolean();
            UseSimplifiedChineseConverter = br.ReadBoolean();
        }

    
        #endregion

 

        #region 设置关键字
        /// <summary>
        /// 设置关键字，设置前请核对UseDBCcaseConverter、UseSimplifiedChineseConverter的值，两值可对关键字有影响
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public override void SetKeywords(ICollection<string> keywords)
        {
            HashSet<string> kws = new HashSet<string>();
            foreach (var item in keywords) {
                kws.Add(ToSenseWord(item));
            }
           base.SetKeywords(kws);
        }
        #endregion

    }
}
