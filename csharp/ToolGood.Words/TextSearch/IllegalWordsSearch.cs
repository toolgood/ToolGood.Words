using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 过滤跳词
    /// </summary>
    /// <param name="c">当前字符</param>
    /// <param name="text">查询字符串</param>
    /// <param name="index">索引</param>
    /// <returns>True 跳过，false 不跳过</returns>
    public delegate bool SkipWordFilterHandler(char c, string text, int index);

    /// <summary>
    /// 字符转化，可以设置繁简转化、忽略大小写
    /// </summary>
    /// <param name="c">当前字符</param>
    /// <param name="text">查询字符串</param>
    /// <param name="index">索引</param>
    /// <returns>转化后的字符</returns>
    public delegate char CharTranslateHandler(char c, string text, int index);

    /// <summary>
    /// 自定义字符串匹配
    /// </summary>
    /// <param name="c">当前字符</param>
    /// <param name="text">查询字符串</param>
    /// <param name="start">开始索引</param>
    /// <param name="end">结束索引</param>
    /// <param name="keyword">查询到的关键字</param>
    /// <param name="keywordIndex">关键字索引</param>
    /// <param name="matchKeyword">匹配关键字</param>
    /// <param name="blacklistIndex">黑名单序号</param>
    /// <returns>返回true匹配成功；返回false匹配失败 </returns>
    public delegate bool StringMatchHandler(string text, int start, int end, string keyword, int keywordIndex, string matchKeyword, int blacklistIndex);

    /// <summary>
    /// 最新版本的IllegalWordsSearch， 与3.0.2.0以前的版本不兼容。
    /// IllegalWordsSearch类太费精力了，头发稀疏了。
    /// 我未来可能以敏感词过滤做为创业项目，所以这是最后的开源版本，不再免费补bug了。
    /// IllegalWordsSearch修复了2020-10-8日前所有bug。
    /// </summary>
    [Obsolete("IllegalWordsSearch类太费精力了，头发稀疏了。我未来可能以敏感词过滤做为创业项目，所以这是最后的开源版本，不再免费补bug了，修复了2020-10-8日前所有bug。")]
    public class IllegalWordsSearch : BaseSearchEx
    {
        private int[] _blacklist = new int[0];

        /// <summary>
        /// 使用跳词过滤器，默认使用
        /// </summary> 
        public bool UseSkipWordFilter = true; //使用跳词过滤器
        private const string _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
        private bool[] _skipBitArray;

        /// <summary>
        /// 过滤跳词
        /// </summary>
        public event SkipWordFilterHandler SkipWordFilter;

        /// <summary>
        /// 字符转化，可以设置繁简转化、忽略大小写，启用后UseIgnoreCase开启无效
        /// 若想使用CharTranslateHandler，请先添加事件CharTranslateHandler, 再用SetKeywords设置关键字
        /// </summary>
        public event CharTranslateHandler CharTranslate;

        /// <summary>
        /// 自定义字符串匹配
        /// </summary>
        public event StringMatchHandler StringMatch;

        /// <summary>
        /// 使用重复词过滤，默认使用
        /// 在 SetKeywords 前使用
        /// </summary>
        public bool UseDuplicateWordFilter = true;

        /// <summary>
        /// 使用半角转化器，默认使用，如果不使用关闭，请先UseDBCcaseConverter设置为false,再用SetKeywords设置关键字
        /// </summary>
        public bool UseDBCcaseConverter = true;

        /// <summary>
        /// 使用忽略大小写，默认使用，如果不使用关闭，请先UseIgnoreCase设置为false,再用SetKeywords设置关键字
        /// </summary>
        public bool UseIgnoreCase = true;

        /// <summary>
        /// 最新版本的IllegalWordsSearch， 与3.0.2.0以前的版本不兼容
        /// </summary>
        public IllegalWordsSearch()
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            for (int i = 0; i < _skipList.Length; i++) {
                _skipBitArray[_skipList[i]] = true;
            }
        }

        #region 设置跳词
        /// <summary>
        /// 设置跳词
        /// </summary>
        /// <param name="skipList"></param>
        public void SetSkipWords(string skipList)
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            if (skipList != null) {
                for (int i = 0; i < skipList.Length; i++) {
                    _skipBitArray[skipList[i]] = true;
                }
            }
        }
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
        /// <para></para>
        /// 如果想使用CharTranslateHandler，请先添加事件CharTranslateHandler, 再用SetKeywords设置关键字
        /// <para></para>
        /// 使用CharTranslateHandler后，UseIgnoreCase配置无效
        /// <para></para>
        /// 如果不使用忽略大小写，请先UseIgnoreCase设置为false,再用SetKeywords设置关键字
        /// </summary>
        /// <param name="keywords"></param>
        public override void SetKeywords(ICollection<string> keywords)
        {
            if (CharTranslate != null) {
                string[] keys = new string[keywords.Count];
                var index = 0;
                foreach (var item in keywords) {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < item.Length; i++) {
                        sb.Append(CharTranslate(item[i], item, i));
                    }
                    keys[index++] = sb.ToString();
                }
                base.SetKeywords(keys);
            } else if (UseDBCcaseConverter || UseIgnoreCase) {
                string[] keys = new string[keywords.Count];
                var index = 0;
                foreach (var item in keywords) {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < item.Length; i++) {
                        sb.Append(ToSenseWord(item[i]));
                    }
                    keys[index++] = sb.ToString();
                }
                base.SetKeywords(keys);
            } else {
                base.SetKeywords(keywords);
            }
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
            bs = IntArrToByteArr(_blacklist);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseIgnoreCase);
        }

        private byte[] BoolArrToByteArr(bool[] intArr)
        {
            Int32 intSize = sizeof(bool) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }

        #endregion

        #region 加载文件
        /// <summary>
        /// 加载文件
        /// </summary>
        protected internal override void Load(BinaryReader br)
        {
            base.Load(br);

            UseSkipWordFilter = br.ReadBoolean();
            var length = br.ReadInt32();
            _skipBitArray = ByteArrToBoolArr(br.ReadBytes(length));

            UseDuplicateWordFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _blacklist = ByteArrToIntArr(br.ReadBytes(length));

            UseIgnoreCase = br.ReadBoolean();
        }

        private bool[] ByteArrToBoolArr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(bool));
            bool[] intArr = new bool[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }

        #endregion


        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        [Obsolete("IllegalWordsSearch类太费精力了，头发稀疏了。我未来可能以敏感词过滤做为创业项目，所以这是最后的开源版本，不再免费补bug了，修复了2020-10-8日前所有bug。")]
        public List<IllegalWordsSearchResult> FindAll(string text)
        {
            List<IllegalWordsSearchResult> result = new List<IllegalWordsSearchResult>();
            int p = 0;
            ushort pChar = 0;

            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                if (UseSkipWordFilter) {
                    if (SkipWordFilter != null) {//跳词跳过
                        if (SkipWordFilter(t1, text, i)) {
                            continue;
                        }
                    } else if (_skipBitArray[t1]) {
                        continue;
                    }
                }

                if (CharTranslate != null) { // 字符串转换
                    t1 = CharTranslate(t1, text, i);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    t1 = ToSenseWord(t1);
                }
                var t = _dict[t1];
                if (t == 0) {
                    pChar = t1;
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (UseDuplicateWordFilter && pChar == t1) {
                        next = p;
                    } else {
                        next = _first[t];
                    }
                }

                if (next != 0) {
                    if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                        for (int j = _end[next]; j < _end[next + 1]; j++) {
                            var index = _resultIndex[j];
                            var r = GetGetIllegalResult(text, i, index);
                            if (r != null) { result.Add(r); }
                        }
                    }
                }
                p = next;
                pChar = t1;
            }
            return result;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        [Obsolete("IllegalWordsSearch类太费精力了，头发稀疏了。我未来可能以敏感词过滤做为创业项目，所以这是最后的开源版本，不再免费补bug了，修复了2020-10-8日前所有bug。")]
        public IllegalWordsSearchResult FindFirst(string text)
        {
            int p = 0;
            ushort pChar = 0;

            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                if (UseSkipWordFilter) {
                    if (SkipWordFilter != null) {//跳词跳过
                        if (SkipWordFilter(t1, text, i)) {
                            continue;
                        }
                    } else if (_skipBitArray[t1]) {
                        continue;
                    }
                }

                if (CharTranslate != null) { // 字符串转换
                    t1 = CharTranslate(t1, text, i);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    t1 = ToSenseWord(t1);
                }
                var t = _dict[t1];
                if (t == 0) {
                    pChar = t1;
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (UseDuplicateWordFilter && pChar == t1) {
                        next = p;
                    } else {
                        next = _first[t];
                    }
                }

                if (next != 0) {
                    if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                        for (int j = _end[next]; j < _end[next + 1]; j++) {
                            var index = _resultIndex[j];
                            var r = GetGetIllegalResult(text, i, index);
                            if (r != null) { return r; }
                        }
                    }
                }
                p = next;
                pChar = t1;
            }
            return null;
        }

        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        [Obsolete("IllegalWordsSearch类太费精力了，头发稀疏了。我未来可能以敏感词过滤做为创业项目，所以这是最后的开源版本，不再免费补bug了，修复了2020-10-8日前所有bug。")]
        public bool ContainsAny(string text)
        {
            int p = 0;
            ushort pChar = 0;

            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                if (UseSkipWordFilter) {
                    if (SkipWordFilter != null) {//跳词跳过
                        if (SkipWordFilter(t1, text, i)) {
                            continue;
                        }
                    } else if (_skipBitArray[t1]) {
                        continue;
                    }
                }

                if (CharTranslate != null) { // 字符串转换
                    t1 = CharTranslate(t1, text, i);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    t1 = ToSenseWord(t1);
                }
                var t = _dict[t1];
                if (t == 0) {
                    pChar = t1;
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (UseDuplicateWordFilter && pChar == t1) {
                        next = p;
                    } else {
                        next = _first[t];
                    }
                }

                if (next != 0) {
                    if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                        for (int j = _end[next]; j < _end[next + 1]; j++) {
                            var index = _resultIndex[j];
                            var r = GetGetIllegalResult(text, i, index);
                            if (r != null) { return true; }
                        }
                    }
                }
                p = next;
                pChar = t1;
            }
            return false;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        [Obsolete("IllegalWordsSearch类太费精力了，头发稀疏了。我未来可能以敏感词过滤做为创业项目，所以这是最后的开源版本，不再免费补bug了，修复了2020-10-8日前所有bug。")]
        public string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);

            int p = 0;
            ushort pChar = 0;

            for (int i = 0; i < text.Length; i++) {
                var t1 = text[i];
                if (UseSkipWordFilter) {
                    if (SkipWordFilter != null) {//跳词跳过
                        if (SkipWordFilter(t1, text, i)) {
                            continue;
                        }
                    } else if (_skipBitArray[t1]) {
                        continue;
                    }
                }

                if (CharTranslate != null) { // 字符串转换
                    t1 = CharTranslate(t1, text, i);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    t1 = ToSenseWord(t1);
                }
                var t = _dict[t1];
                if (t == 0) {
                    pChar = t1;
                    p = 0;
                    continue;
                }
                int next;
                if (p == 0 || _nextIndex[p].TryGetValue(t, out next) == false) {
                    if (UseDuplicateWordFilter && pChar == t1) {
                        next = p;
                    } else {
                        next = _first[t];
                    }
                }

                if (next != 0) {
                    if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                        for (int j = _end[next]; j < _end[next + 1]; j++) {
                            var index = _resultIndex[j];
                            var r = GetGetIllegalResult(text, i, index);
                            if (r != null) {
                                for (int k = r.Start; k <= r.End; k++) {
                                    result[k] = replaceChar;
                                }
                                break;
                            }
                        }
                    }
                }
                p = next;
                pChar = t1;
            }
            return result.ToString();
        }


        /// <summary>
        /// 当前字符为不是英文数字，则通过，返回true
        /// 审核下一个字符是否为英文数字，如果是,则返回 false
        /// </summary>
        /// <param name="text"></param>
        /// <param name="c"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool CheckNextChar(string text, char c, int end)
        {
            if (IsEnglishOrNumber(c) == false) {
                return true;
            }
            if (end + 1 < text.Length) {
                var e1 = text[end + 1];
                if (UseSkipWordFilter) {
                    if (SkipWordFilter != null) {//跳词跳过
                        if (SkipWordFilter(e1, text, end + 1)) {
                            return true;
                        }
                    } else if (_skipBitArray[e1]) {
                        return true;
                    }
                }
                if (CharTranslate != null) { // 字符串转换
                    e1 = CharTranslate(e1, text, end + 1);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    e1 = ToSenseWord(e1);
                }
                if (IsEnglishOrNumber(e1)) {
                    return false;
                }
            }
            return true;
        }

        private IllegalWordsSearchResult GetGetIllegalResult(string text, int end, int index)
        {
            var key = _keywords[index];

            int keyIndex = key.Length - 1;
            int start = end;
            for (int i = end; i >= 0; i--) {
                var s2 = text[i];
                if (UseSkipWordFilter) {
                    if (SkipWordFilter != null) {
                        if (SkipWordFilter(s2, text, i)) { continue; }
                    } else if (_skipBitArray[s2]) { continue; }
                }

                if (CharTranslate != null) { // 字符串转换
                    s2 = CharTranslate(s2, text, i);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    s2 = ToSenseWord(s2);
                }
                if (s2 == key[keyIndex]) {
                    keyIndex--;
                    if (keyIndex == -1) { start = i; break; }
                }
            }
            for (int i = start; i >= 0; i--) {
                var s2 = text[i];
                if (CharTranslate != null) { // 字符串转换
                    s2 = CharTranslate(s2, text, i);
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    s2 = ToSenseWord(s2);
                }
                if (s2 != key[0]) { break; }
                start = i;
            }
            return GetGetIllegalResult(text, key, start, end, index);
        }

        private IllegalWordsSearchResult GetGetIllegalResult(string text, string key, int start, int end, int index)
        {
            if (start > 0) {
                var s1 = text[start];
                if (CharTranslate != null) { s1 = CharTranslate(s1, text, start); }
                if (IsEnglishOrNumber(s1)) {
                    var s2 = text[start - 1];
                    if (CharTranslate != null) { // 字符串转换
                        s2 = CharTranslate(s2, text, start - 1);
                    } else if (UseDBCcaseConverter || UseIgnoreCase) {
                        s2 = ToSenseWord(s2);
                    }
                    if (IsEnglishOrNumber(s2)) { return null; }
                }
            }

            var keyword = text.Substring(start, end - start + 1);
            var bl = _blacklist.Length > index ? _blacklist[index] : 0;
            if (StringMatch != null) {
                if (StringMatch(text, start, end, keyword, index, key, _blacklist[index])) {
                    return new IllegalWordsSearchResult(keyword, start, end, index, key, bl);
                }
                return null;
            }
            return new IllegalWordsSearchResult(keyword, start, end, index, key, bl);
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
            return c;
        }
        #endregion

        #region 设置黑名单
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



    }
}
