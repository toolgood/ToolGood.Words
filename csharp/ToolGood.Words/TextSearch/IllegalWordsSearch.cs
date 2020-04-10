using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    public class IllegalWordsSearch : BaseSearchEx
    {
        #region 私有变量

        /// <summary>
        /// 使用跳词过滤器
        /// </summary> 
        public bool UseSkipWordFilter = false; //使用跳词过滤器
        private const string _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
        private bool[] _skipBitArray;
        /// <summary>
        /// 使用重复词过滤器
        /// </summary>
        public bool UseDuplicateWordFilter = false;
        /// <summary>
        /// 使用黑名单过滤器
        /// </summary>
        public bool UseBlacklistFilter = false;
        private int[] _blacklist;
        /// <summary>
        /// 使用半角转化器
        /// </summary>
        public bool UseDBCcaseConverter = true;
        /// <summary>
        /// 使用简体中文转化器
        /// </summary>
        public bool UseSimplifiedChineseConverter = true;

        /// <summary>
        /// 使用忽略大小写
        /// </summary>
        public bool UseIgnoreCase = true;
        #endregion

        public IllegalWordsSearch()
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            for (int i = 0; i < _skipList.Length; i++) {
                _skipBitArray[_skipList[i]] = true;
            }
            _blacklist = new int[0];
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

            for (int i = 0; i < text.Length; i++) {
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) { results.Add(r); }
                        }
                    }
                }

                var c = text[i];
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; pChar = c; continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false) {
                    if (UseDuplicateWordFilter && pChar == c) { continue; }
                    if (p != 0) {
                        p = 0;
                        next = _next[0] + t;
                        find = _key[next] == t;
                    }
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }
                pChar = c;
            }
            if (findIndex != 0) {
                foreach (var item in _guides[findIndex]) {
                    var r = GetIllegalResult(item, text.Length - 1, text, p, pIndex, flag);
                    if (r != null) { results.Add(r); }
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

            for (int i = 0; i < text.Length; i++) {
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) return r;
                        }
                    }
                }

                var c = text[i];
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; pChar = c; continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false) {
                    if (UseDuplicateWordFilter && pChar == c) { continue; }
                    if (p != 0) {
                        p = 0;
                        next = _next[0] + t;
                        find = _key[next] == t;
                    }
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }
                pChar = c;
            }
            if (findIndex != 0) {
                foreach (var item in _guides[findIndex]) {
                    var r = GetIllegalResult(item, text.Length - 1, text, p, pIndex, flag);
                    if (r != null) return r;
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
            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            char pChar = (char)0;

            for (int i = 0; i < text.Length; i++) {
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) { return true; }
                        }
                    }
                }

                var c = text[i];
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; pChar = c; continue; }//不在字表中，跳过


                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false) {
                    if (UseDuplicateWordFilter && pChar == c) { continue; }
                    if (p != 0) {
                        p = 0;
                        next = _next[0] + t;
                        find = _key[next] == t;
                    }
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }
                pChar = c;
            }
            if (findIndex != 0) {
                foreach (var item in _guides[findIndex]) {
                    var r = GetIllegalResult(item, text.Length - 1, text, p, pIndex, flag);
                    if (r != null) { return true; }
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
            var p = 0;
            int findIndex = 0;
            char pChar = (char)0;


            for (int i = 0; i < text.Length; i++) {
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) {
                                for (int j = r.Start; j < i; j++) { result[j] = replaceChar; }
                                break;
                            }
                        }
                    }
                }

                var c = text[i];
                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { p = 0; pChar = c; continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false) {
                    if (UseDuplicateWordFilter && pChar == c) { continue; }
                    if (p != 0) {
                        p = 0;
                        next = _next[0] + t;
                        find = _key[next] == t;
                    }
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }
                pChar = c;
            }
            if (findIndex != 0) {
                foreach (var item in _guides[findIndex]) {
                    var r = GetIllegalResult(item, text.Length - 1, text, p, pIndex, flag);
                    if (r != null) {
                        for (int j = r.Start; j < text.Length; j++) { result[j] = replaceChar; }
                        break;
                    }
                }
            }
            return result.ToString();
        }

        private int FindStart(string keyword, int end, string srcText, int p, int[] pIndex)
        {
            if (end + 1 < srcText.Length) {
                var en1 = IsEnglishOrNumber(srcText[end + 1]);
                var en2 = IsEnglishOrNumber(srcText[end]);
                if (en1 && en2) { return -1; }
            }
            var n = keyword.Length;
            var start = end;
            int pp = p;
            while (n > 0) {
                var pi = pIndex[start--];
                if (pi != pp) { n--; pp = pi; }
                if (start == -1) return 0;
            }
            var sn1 = IsEnglishOrNumber(srcText[start++]);
            var sn2 = IsEnglishOrNumber(srcText[start]);
            if (sn1 && sn2) {
                return -1;
            }
            return start;
        }

        private IllegalWordsSearchResult GetIllegalResult(int index, int end, string srcText, int p, int[] pIndex, int flag)
        {
            if (UseBlacklistFilter) {
                var b = _blacklist[index];
                if ((b | flag) != b) { return null; }
            }
            var keyword = _keywords[index];
            if (keyword.Length==1) {
                if (ToSenseWord(srcText[end]) != ToSenseWord(keyword[0])) { return null; }
                return new IllegalWordsSearchResult(keyword, end, end, srcText);
            }
            var start = FindStart(keyword, end, srcText, p, pIndex);
            if (start == -1) { return null; }
            if (ToSenseWord(srcText[start]) != ToSenseWord(keyword[0])) { return null; }
            if (UseBlacklistFilter) {
                return new IllegalWordsSearchResult(keyword, start, end, srcText, (BlacklistType)_blacklist[index]);
            }
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
        }
 
        private char ToSenseWord(char c)
        {
            if (UseIgnoreCase) {
                if (c >= 'A' && c <= 'Z') return (char)(c | 0x20);
            }
            if (UseDBCcaseConverter) {
                if (c == 12288) return ' ';
                if (c >= 65280 && c < 65375) {
                    var k = (c - 65248);
                    if (UseIgnoreCase) {
                        if ('A' <= k && k <= 'Z') {
                            k = k | 0x20;
                        }
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

            bw.Write(UseSkipWordFilter);
            var bs = BoolArrToByteArr(_skipBitArray);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseDuplicateWordFilter);
            bw.Write(UseBlacklistFilter);
            bs = IntArrToByteArr(_blacklist);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseDBCcaseConverter);
            bw.Write(UseSimplifiedChineseConverter);
            bw.Write(UseIgnoreCase);
        }

        #endregion

        #region 加载文件

        protected internal override void Load(BinaryReader br)
        {
            base.Load(br);

            UseSkipWordFilter = br.ReadBoolean();
            var length = br.ReadInt32();
            _skipBitArray = ByteArrToBoolArr(br.ReadBytes(length));

            UseDuplicateWordFilter = br.ReadBoolean();
            UseBlacklistFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _blacklist = ByteArrToIntArr(br.ReadBytes(length));

            UseDBCcaseConverter = br.ReadBoolean();
            UseSimplifiedChineseConverter = br.ReadBoolean();
            try {
                UseIgnoreCase = br.ReadBoolean();
            } catch (Exception) { }
        }
        #endregion

        #region 设置跳词
        /// <summary>
        /// 设置跳词
        /// </summary>
        /// <param name="skipList"></param>
        public void SetSkipWords(string skipList)
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            if (skipList == null) {
                for (int i = 0; i < _skipList.Length; i++) {
                    _skipBitArray[_skipList[i]] = true;
                }
            }
        }
        #endregion

        #region 设置黑名单
        /// <summary>
        /// 设置黑名单
        /// </summary>
        /// <param name="blacklist"></param>
        public void SetBlacklist(BlacklistType[] blacklist)
        {
            if (_keywords == null) {
                throw new NullReferenceException("请先使用SetKeywords方法设置关键字！");
            }
            if (blacklist.Length != _keywords.Length) {
                throw new ArgumentException("请关键字与黑名单列表的长度要一样长！");
            }

            var list = new int[blacklist.Length];
            for (int i = 0; i < blacklist.Length; i++) {
                list[i] = (int)blacklist[i];
            }
            _blacklist = list;
        }

        /// <summary>
        /// 设置黑名单
        /// </summary>
        /// <param name="blacklist"></param>
        public void SetBlacklist(int[] blacklist)
        {
            if (_keywords == null) {
                throw new NullReferenceException("请先使用SetKeywords方法设置关键字！");
            }
            if (blacklist.Length != _keywords.Length) {
                throw new ArgumentException("请关键字与黑名单列表的长度要一样长！");
            }

            _blacklist = blacklist;
        }
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
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

        private bool IsEnglishOrNumber(char c)
        {
            if (c < 128) {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                    return true;
                }
            }
            return false;
        }

        #endregion


    }
}
