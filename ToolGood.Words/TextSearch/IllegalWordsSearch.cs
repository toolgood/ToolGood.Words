using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    public class IllegalWordsSearch
    {
        #region Class
        class TrieNode
        {
            public TrieNode Parent;
            public TrieNode Failure;
            public char Char;
            internal bool End;
            internal List<int> Results;
            internal Dictionary<char, TrieNode> m_values;
            internal Dictionary<char, TrieNode> merge_values;
            private int minflag = int.MaxValue;
            private int maxflag = 0;
            internal int Next;
            private int Count;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                merge_values = new Dictionary<char, TrieNode>();
                Results = new List<int>();
            }

            public bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (int)c && maxflag >= (int)c) {
                    return m_values.TryGetValue(c, out node);
                }
                node = null;
                return false;
            }

            public TrieNode Add(char c)
            {
                TrieNode node;

                if (m_values.TryGetValue(c, out node)) {
                    return node;
                }

                if (minflag > c) { minflag = c; }
                if (maxflag < c) { maxflag = c; }

                node = new TrieNode();
                node.Parent = this;
                node.Char = c;
                m_values[c] = node;
                Count++;
                return node;
            }

            public void SetResults(int text)
            {
                if (End == false) {
                    End = true;
                }
                if (Results.Contains(text) == false) {
                    Results.Add(text);
                }
            }

            public void Merge(TrieNode node)
            {
                var nd = node;
                while (nd.Char != 0) {
                    foreach (var item in node.m_values) {
                        if (m_values.ContainsKey(item.Key) == false) {
                            if (merge_values.ContainsKey(item.Key) == false) {
                                if (minflag > item.Key) { minflag = item.Key; }
                                if (maxflag < item.Key) { maxflag = item.Key; }
                                merge_values[item.Key] = item.Value;
                                Count++;
                            }
                        }
                    }
                    nd = nd.Failure;
                }
            }

            public int Rank(TrieNode[] has)
            {
                bool[] seats = new bool[has.Length];
                int start = 1;

                has[0] = this;

                Rank(ref start, seats, has);
                int maxCount = has.Length - 1;
                while (has[maxCount] == null) { maxCount--; }
                return maxCount;
            }

            private void Rank(ref int start, bool[] seats, TrieNode[] has)
            {
                if (maxflag == 0) return;
                var keys = m_values.Select(q => (int)q.Key).ToList();
                keys.AddRange(merge_values.Select(q => (int)q.Key).ToList());

                while (has[start] != null) { start++; }
                var s = start < (int)minflag ? (int)minflag : start;

                for (int i = s; i < has.Length; i++) {
                    if (has[i] == null) {
                        var next = i - (int)minflag;
                        //if (next < 0) continue;
                        if (seats[next]) continue;

                        var isok = true;
                        foreach (var item in keys) {
                            if (has[next + item] != null) { isok = false; break; }
                        }
                        if (isok) {
                            SetSeats(next, seats, has);
                            break;
                        }
                    }
                }
                start += keys.Count / 2;

                var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);
                foreach (var key in keys2) {
                    key.Value.Rank(ref start, seats, has);
                }
            }


            private void SetSeats(int next, bool[] seats, TrieNode[] has)
            {
                Next = next;
                seats[next] = true;

                foreach (var item in merge_values) {
                    var position = next + item.Key;
                    has[position] = item.Value;
                }

                foreach (var item in m_values) {
                    var position = next + item.Key;
                    has[position] = item.Value;
                }

            }


        }
        #endregion

        #region 私有变量
        private string[] _keywords;
        private int[][] _guides;
        private int[] _key;
        private int[] _next;
        private int[] _check;
        private int[] _dict;

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
                        if (TestBlacklist(findIndex, flag)) {
                            foreach (var item in _guides[findIndex]) {
                                var r = GetIllegalResult(_keywords[item], i - 1, text, p, pIndex);
                                if (r != null) { results.Add(r); }
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
                if (TestBlacklist(findIndex, flag)) {
                    foreach (var item in _guides[findIndex]) {
                        var r = GetIllegalResult(_keywords[item], text.Length - 1, text, p, pIndex);
                        if (r != null) { results.Add(r); }
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

            for (int i = 0; i < text.Length; i++) {
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0) {
                        if (TestBlacklist(findIndex, flag)) {
                            foreach (var item in _guides[findIndex]) {
                                var r = GetIllegalResult(_keywords[item], i - 1, text, p, pIndex);
                                if (r != null) return r;
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
                if (TestBlacklist(findIndex, flag)) {
                    foreach (var item in _guides[findIndex]) {
                        var r = GetIllegalResult(_keywords[item], text.Length - 1, text, p, pIndex);
                        if (r != null) return r;
                    }
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
                        if (TestBlacklist(findIndex, flag)) {
                            var s = FindStart(_keywords[_guides[findIndex][0]], i - 1, text, p, pIndex);
                            if (s != -1) { return true; }
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
                if (TestBlacklist(findIndex, flag)) {
                    var s = FindStart(_keywords[_guides[findIndex][0]], text.Length - 1, text, p, pIndex);
                    if (s != -1) { return true; }
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
                        if (TestBlacklist(findIndex, flag)) {
                            var keyword = _keywords[_guides[findIndex][0]];
                            var start = FindStart(keyword, i - 1, text, p, pIndex);
                            if (start != -1) {
                                for (int j = start; j < i; j++) {
                                    result[j] = replaceChar;
                                }
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
                if (TestBlacklist(findIndex, flag)) {
                    var keyword = _keywords[_guides[findIndex][0]];
                    var start = FindStart(keyword, text.Length - 1, text, p, pIndex);
                    if (start != -1) {
                        for (int j = start; j < text.Length; j++) {
                            result[j] = replaceChar;
                        }
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

        private IllegalWordsSearchResult GetIllegalResult(string keyword, int end, string srcText, int p, int[] pIndex)
        {
            var start = FindStart(keyword, end, srcText, p, pIndex);
            if (start == -1) {
                return null;
            }
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
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var fs = File.Open(filePath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            Save(bw);
#if NETSTANDARD1_3
            bw.Dispose();
            fs.Dispose();
#else
            bw.Close();
            fs.Close();
#endif
        }

        /// <summary>
        /// 保存到Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            Save(bw);
#if NETSTANDARD1_3
            bw.Dispose();
#else
            bw.Close();
#endif
        }

        protected internal virtual void Save(BinaryWriter bw)
        {
            bw.Write(_keywords.Length);
            foreach (var item in _keywords) {
                bw.Write(item);
            }

            List<int> guideslist = new List<int>();
            guideslist.Add(_guides.Length);
            foreach (var guide in _guides) {
                guideslist.Add(guide.Length);
                foreach (var item in guide) {
                    guideslist.Add(item);
                }
            }
            var bs = IntArrToByteArr(guideslist.ToArray());
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_key);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_next);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_check);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_dict);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseSkipWordFilter);
            bs = BoolArrToByteArr(_skipBitArray);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseDuplicateWordFilter);
            bw.Write(UseBlacklistFilter);
            bs = IntArrToByteArr(_blacklist);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseDBCcaseConverter);
            bw.Write(UseSimplifiedChineseConverter);
        }

        private byte[] IntArrToByteArr(int[] intArr)
        {
            int intSize = sizeof(int) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        private byte[] BoolArrToByteArr(bool[] intArr)
        {
            int intSize = sizeof(bool) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }

        #endregion

        #region 加载文件
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Load(string filePath)
        {
            var fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            Load(br);
#if NETSTANDARD1_3
            br.Dispose();
            fs.Dispose();
#else
            br.Close();
            fs.Close();
#endif
        }
        /// <summary>
        /// 加载Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            Load(br);
#if NETSTANDARD1_3
            br.Dispose();
#else
            br.Close();
#endif
        }

        protected internal virtual void Load(BinaryReader br)
        {
            var length = br.ReadInt32();
            _keywords = new string[length];
            for (int i = 0; i < length; i++) {
                _keywords[i] = br.ReadString();
            }

            length = br.ReadInt32();
            var bs = br.ReadBytes(length);
            using (MemoryStream ms = new MemoryStream(bs)) {
                BinaryReader b = new BinaryReader(ms);
                var length2 = b.ReadInt32();
                _guides = new int[length2][];
                for (int i = 0; i < length2; i++) {
                    var length3 = b.ReadInt32();
                    _guides[i] = new int[length3];
                    for (int j = 0; j < length3; j++) {
                        _guides[i][j] = b.ReadInt32();
                    }
                }
            }

            length = br.ReadInt32();
            _key = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _next = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _check = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _dict = ByteArrToIntArr(br.ReadBytes(length));



            UseSkipWordFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _skipBitArray = ByteArrToBoolArr(br.ReadBytes(length));

            UseDuplicateWordFilter = br.ReadBoolean();
            UseBlacklistFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _blacklist = ByteArrToIntArr(br.ReadBytes(length));

            UseDBCcaseConverter = br.ReadBoolean();
            UseSimplifiedChineseConverter = br.ReadBoolean();
        }

        private int[] ByteArrToIntArr(byte[] btArr)
        {
            int intSize = btArr.Length / sizeof(int);
            int[] intArr = new int[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, intSize);
            return intArr;
        }

        private bool[] ByteArrToBoolArr(byte[] btArr)
        {
            int intSize = btArr.Length / sizeof(bool);
            bool[] intArr = new bool[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, intSize);
            return intArr;
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
            _blacklist = blacklist;
        }
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(ICollection<string> keywords)
        {
            HashSet<string> kws = new HashSet<string>();
            foreach (var item in keywords) {
                kws.Add(ToSenseWord(item));
            }
            setKeywords(kws);
        }

        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        private void setKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            var length = CreateDict(keywords);
            var root = new TrieNode();

            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (int j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)_dict[p[j]]);
                }
                nd.SetResults(i);
            }

            List<TrieNode> nodes = new List<TrieNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TrieNode nd in root.m_values.Values) {
                nd.Failure = root;
                foreach (TrieNode trans in nd.m_values.Values) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TrieNode> newNodes = new List<TrieNode>();

                //ArrayList newNodes = new ArrayList();
                foreach (TrieNode nd in nodes) {
                    TrieNode r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.m_values.ContainsKey(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = root;
                    else {
                        nd.Failure = r.m_values[c];
                        foreach (var result in nd.Failure.Results)
                            nd.SetResults(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TrieNode child in nd.m_values.Values)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            root.Failure = root;
            foreach (var item in root.m_values.Values) {
                TryLinks(item);
            }
            BuildArray(root, length);
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

        private void TryLinks(TrieNode node)
        {
            node.Merge(node.Failure);
            foreach (var item in node.m_values.Values) {
                TryLinks(item);
            }
        }

        private void BuildArray(TrieNode root, int length)
        {
            TrieNode[] has = new TrieNode[0x00FFFFFF];
            length = root.Rank(has) + length + 1;
            _key = new int[length];
            _next = new int[length];
            _check = new int[length];
            List<int[]> guides = new List<int[]>();
            guides.Add(new int[] { 0 });
            for (int i = 0; i < length; i++) {
                var item = has[i];
                if (item == null) continue;
                _key[i] = item.Char;
                _next[i] = item.Next;
                if (item.End) {
                    _check[i] = guides.Count;
                    guides.Add(item.Results.ToArray());
                }
            }
            _guides = guides.ToArray();
        }

        #endregion

        #region 生成映射字典

        private int CreateDict(ICollection<string> keywords)
        {
            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            foreach (var keyword in keywords) {
                for (int i = 0; i < keyword.Length; i++) {
                    var item = keyword[i];
                    if (dictionary.ContainsKey(item)) {
                        if (i > 0)
                            dictionary[item] += 2;
                    } else {
                        dictionary[item] = i > 0 ? 2 : 1;
                    }
                }
            }
            var list = dictionary.OrderByDescending(q => q.Value).Select(q => q.Key).ToList();
            var list2 = new List<char>();
            var sh = false;
            foreach (var item in list) {
                if (sh) {
                    list2.Add(item);
                } else {
                    list2.Insert(0, item);
                }
                sh = !sh;
            }
            _dict = new int[char.MaxValue + 1];
            for (int i = 0; i < list2.Count; i++) {
                _dict[list2[i]] = i + 1;
            }
            return dictionary.Count;
        }
        #endregion

    }
}
