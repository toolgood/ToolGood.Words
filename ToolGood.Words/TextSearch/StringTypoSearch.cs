using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 错字搜索--英文数字搜索,不包含中文转数字
    /// 主要检测网址，推广账号
    /// </summary>
    public class StringTypoSearch : BaseSearchEx
    {
        #region 私有变量


        /// <summary>
        /// 使用跳词过滤器
        /// </summary> 
        public bool UseSkipWordFilter = false; //使用跳词过滤器
        protected const string _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
        protected bool[] _skipBitArray;

        /// <summary>
        /// 使用黑名单过滤器
        /// </summary>
        public bool UseBlacklistFilter = false;
        protected int[] _blacklist;


        private int _maxJump = 5;//设置一个最大大跳跃点
        private char[] toWord;

        /// <summary>
        /// 使用忽略大小写
        /// </summary>
        public bool UseIgnoreCase = true;
        #endregion

        #region 构造函数
        private const string baseTypo =
            "0 0\r1 1\r2 2\r3 3\r4 4\r5 5\r6 6\r7 7\r8 8\r9 9\r" +
            "０ 0\r１ 1\r２ 2\r３ 3\r４ 4\r５ 5\r６ 6\r７ 7\r８ 8\r９ 9\r" +
            "A a\rB b\rC c\rD d\rE e\rF f\rG g\rH h\rI i\rJ j\rK k\rL l\rM m\rN n\rO o\rP p\rQ q\rR r\rS s\rT t\rU u\rV v\rW w\rX x\rY y\rZ z\r" +
            "a a\rb b\rc c\rd d\re e\rf f\rg g\rh h\ri i\rj j\rk k\rl l\rm m\rn n\ro o\rp p\rq q\rr r\rs s\rt t\ru u\rv v\rw w\rx x\ry y\rz z\r" +
            "Ａ a\rＢ b\rＣ c\rＤ d\rＥ e\rＦ f\rＧ g\rＨ h\rＩ i\rＪ j\rＫ k\rＬ l\rＭ m\rＮ n\rＯ o\rＰ p\rＱ q\rＲ r\rＳ s\rＴ t\rＵ u\rＶ v\rＷ w\rＸ x\rＹ y\rＺ z\r" +
            "ａ a\rｂ b\rｃ c\rｄ d\rｅ e\rｆ f\rｇ g\rｈ h\rｉ i\rｊ j\rｋ k\rｌ l\rｍ m\rｎ n\rｏ o\rｐ p\rｑ q\rｒ r\rｓ s\rｔ t\rｕ u\rｖ v\rｗ w\rｘ x\rｙ y\rｚ z\r" +
            "# #\r$ $\r% %\r& &\r+ +\r- -\r. .\r/ /\r: :\r= =\r? ?\r@ @\r_ _\r\\ \\\r" +
            "＃ #\r＄ $\r％ %\r＆ &\r＋ +\r－ -\r． .\r／ /\r： :\r＝ =\r？ ?\r＠ @\r＿ _\r＼ \\\r";
        private const string defaultTypo = "⓪ 0\r零 0\rº 0\r₀ 0\r⓿ 0\r○ 0\r〇 0\r" +
                                          "⒜ a\r⒝ b\r⒞ c\r⒟ d\r⒠ e\r⒡ f\r⒢ g\r⒣ h\r⒤ i\r⒥ j\r⒦ k\r⒧ l\r⒨ m\r⒩ n\r⒪ o\r⒫ p\r⒬ q\r⒭ r\r⒮ s\r⒯ t\r⒰ u\r⒱ v\r⒲ w\r⒳ x\r⒴ y\r⒵ z\r" +
                                          "Ⓐ a\rⒷ b\rⒸ c\rⒹ d\rⒺ e\rⒻ f\rⒼ g\rⒽ h\rⒾ i\rⒿ j\rⓀ k\rⓁ l\rⓂ m\rⓃ n\rⓄ o\rⓅ p\rⓆ q\rⓇ r\rⓈ s\rⓉ t\rⓊ u\rⓋ v\rⓌ w\rⓍ x\rⓎ y\rⓏ z\r" +
                                          "ⓐ a\rⓑ b\rⓒ c\rⓓ d\rⓔ e\rⓕ f\rⓖ g\rⓗ h\rⓘ i\rⓙ j\rⓚ k\rⓛ l\rⓜ m\rⓝ n\rⓞ o\rⓟ p\rⓠ q\rⓡ r\rⓢ s\rⓣ t\rⓤ u\rⓥ v\rⓦ w\rⓧ x\rⓨ y\rⓩ z\r" +
                                         "一 1\r二 2\r三 3\r四 4\r五 5\r六 6\r七 7\r八 8\r九 9\r" +
                                         "壹 1\r贰 2\r叁 3\r肆 4\r伍 5\r陆 6\r柒 7\r捌 8\r玖 9\r" +
                                          "。 .\r点 .\r點 .\r— 1\r貮 2\r参 3\r陸 6\r" +
                                         "¹ 1\r² 2\r³ 3\r⁴ 4\r⁵ 5\r⁶ 6\r⁷ 7\r⁸ 8\r⁹ 9\r" +
                                         "₁ 1\r₂ 2\r₃ 3\r₄ 4\r₅ 5\r₆ 6\r₇ 7\r₈ 8\r₉ 9\r" +
                                         "① 1\r② 2\r③ 3\r④ 4\r⑤ 5\r⑥ 6\r⑦ 7\r⑧ 8\r⑨ 9\r" +
                                         "⑴ 1\r⑵ 2\r⑶ 3\r⑷ 4\r⑸ 5\r⑹ 6\r⑺ 7\r⑻ 8\r⑼ 9\r" +
                                         "⒈ 1\r⒉ 2\r⒊ 3\r⒋ 4\r⒌ 5\r⒍ 6\r⒎ 7\r⒏ 8\r⒐ 9\r" +
                                         "❶ 1\r❷ 2\r❸ 3\r❹ 4\r❺ 5\r❻ 6\r❼ 7\r❽ 8\r❾ 9\r" +
                                         "➀ 1\r➁ 2\r➂ 3\r➃ 4\r➄ 5\r➅ 6\r➆ 7\r➇ 8\r➈ 9\r" +
                                         "➊ 1\r➋ 2\r➌ 3\r➍ 4\r➎ 5\r➏ 6\r➐ 7\r➑ 8\r➒ 9\r" +
                                         "㈠ 1\r㈡ 2\r㈢ 3\r㈣ 4\r㈤ 5\r㈥ 6\r㈦ 7\r㈧ 8\r㈨ 9\r" +
                                         "⓵ 1\r⓶ 2\r⓷ 3\r⓸ 4\r⓹ 5\r⓺ 6\r⓻ 7\r⓼ 8\r⓽ 9\r" +
                                         "㊀ 1\r㊁ 2\r㊂ 3\r㊃ 4\r㊄ 5\r㊅ 6\r㊆ 7\r㊇ 8\r㊈ 9\r";




        public StringTypoSearch()
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            for (int i = 0; i < _skipList.Length; i++) {
                _skipBitArray[_skipList[i]] = true;
            }
            _blacklist = new int[0];

            toWord = new char[char.MaxValue + 1];

            buildWordConverter(GetTypos());
        }

        protected virtual string GetTypos()
        {
            return defaultTypo + baseTypo;
        }

        private void buildWordConverter(string typoText)
        {
            var ts = typoText.Replace("\n", "").Split('\r');
            var splits = " \t".ToCharArray();
            foreach (var t in ts) {
                var sp = t.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length < 2) continue;
                toWord[(int)sp[0][0]] = sp[1][0];
            }
        }


        #endregion

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
            int jump = 0;

            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) { results.Add(r); }
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }

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
            int jump = 0;

            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) return r;
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }

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
            int jump = 0;

            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) return true;
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }

            }
            if (findIndex != 0) {
                foreach (var item in _guides[findIndex]) {
                    var r = GetIllegalResult(item, text.Length - 1, text, p, pIndex, flag);
                    if (r != null) return true;
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
            int jump = 0;


            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        foreach (var item in _guides[findIndex]) {
                            var r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                            if (r != null) {
                                for (int j = r.Start; j < i; j++) { result[j] = replaceChar; }
                                break;
                            }
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }
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


        private bool IsEnglishOrNumber(char c)
        {
            if (c < 128) {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                    return true;
                }
            }
            return false;
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
            var start = FindStart(keyword, end, srcText, p, pIndex);
            if (start == -1) { return null; }
            if (UseBlacklistFilter) {
                return new IllegalWordsSearchResult(keyword, start, end, srcText, (BlacklistType)_blacklist[index]);
            }
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
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

            bw.Write(UseBlacklistFilter);
            bs = IntArrToByteArr(_blacklist);
            bw.Write(bs.Length);
            bw.Write(bs);
            bw.Write(_maxJump);
            bs = CharArrToByteArr(toWord);
            bw.Write(bs.Length);
            bw.Write(bs);

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

            UseBlacklistFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _blacklist = ByteArrToIntArr(br.ReadBytes(length));

            _maxJump = br.ReadInt32();
            length = br.ReadInt32();
            toWord = ByteArrToCharArr(br.ReadBytes(length));
            try {
                UseIgnoreCase = br.ReadBoolean();
            } catch (Exception) { }
        }
        #endregion

        #region 设置跳词
        /// <summary>
        /// 设置跳词，搜索匹配前，请设置UseSkipWordFilter=true
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
        /// 设置黑名单，搜索匹配前，请设置UseBlacklistFilter=true
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
        /// 设置黑名单，搜索匹配前，请设置UseBlacklistFilter=true
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
        private string ToSenseWord(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++) {
                stringBuilder.Append(ToSenseWord(text[i]));
                //stringBuilder[i] = ToSenseWord(text[i]);
            }
            return stringBuilder.ToString();
        }

        private char ToSenseWord(char c)
        {
            if (UseIgnoreCase) {
                if (c >= 'A' && c <= 'Z') return (char)(c | 0x20);
            }
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
            return c;
        }
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
