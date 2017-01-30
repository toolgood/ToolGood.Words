//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using Word = System.UInt16;

//namespace ToolGood.Words
//{


//    public class PinYinSearchEx2
//    {
//        const char wordsSpace = '\b';// (char)1;
//        const char pinYinSpace = '\t';// (char)2;

//        #region 私有类
//        class Range : IDisposable
//        {
//            public List<int> Ranges = new List<int>();
//            public Range() { }
//            public Range(int index)
//            {
//                Ranges.Add(index);
//                Ranges.Add(index);
//            }
//            public Range(int start, int end)
//            {
//                Ranges.Add(start);
//                Ranges.Add(end);
//            }

//            public void Add(int index)
//            {
//                var last = Ranges[Ranges.Count - 1];
//                if (last == index) { return; }

//                if (last + 1 == index) {
//                    Ranges[Ranges.Count - 1] = index;
//                } else {
//                    Ranges.Add(index);
//                    Ranges.Add(index);
//                }
//            }

//            public Range Intersection(List<int> range)
//            {
//                List<int> results = new List<int>();
//                var length = range.Count / 2;
//                for (int i = 0; i < length; i++) {
//                    Intersection(range[i * 2], range[i * 2 + 1], results);
//                }
//                Range ra = new Range();
//                ra.Ranges = results;
//                return ra;
//            }

//            private void Intersection(int start, List<int> results)
//            {
//                var length = Ranges.Count / 2;
//                for (int i = 0; i < length; i++) {
//                    var s = Ranges[i * 2];
//                    var e = Ranges[i * 2 + 1];
//                    if (start > e) continue;
//                    results.Add(Math.Max(s, start));
//                    results.Add(e);
//                }
//            }

//            private void Intersection(int start, int end, List<int> results)
//            {
//                if (end < start) {
//                    Intersection(start, end, results);
//                    return;
//                }

//                var length = Ranges.Count / 2;
//                for (int i = 0; i < length; i++) {
//                    var s = Ranges[i * 2];
//                    var e = Ranges[i * 2 + 1];
//                    if (s > end) break;
//                    if (start > e) continue;
//                    results.Add(Math.Max(s, start));
//                    results.Add(Math.Min(e, end));
//                }
//            }

//            public override string ToString()
//            {
//                StringBuilder sb = new StringBuilder();
//                var length = Ranges.Count / 2;
//                for (int i = 0; i < length; i++) {
//                    if (i > 0) {
//                        sb.Append(',');
//                    }
//                    sb.AppendFormat("[{0}-{1}]", Ranges[i * 2], Ranges[i * 2 + 1]);
//                }
//                return sb.ToString();
//            }

//            public bool HasNone()
//            {
//                return Ranges.Count == 0;
//            }

//            public void AddTo(List<int> results)
//            {
//                var length = Ranges.Count / 2;
//                for (int i = 0; i < length; i++) {
//                    int s = Ranges[i * 2];
//                    int e = Ranges[i * 2 + 1];
//                    for (int j = s; j <= e; j++) {
//                        if (results.Contains(j) == false) {
//                            results.Add(j);
//                        }
//                    }
//                }
//            }

//            public void Dispose()
//            {
//                Ranges.Clear();
//                Ranges = null;
//            }
//        }
//        class TrieRange : IDisposable
//        {
//            public byte Right = 0;
//            public int RangeStart;
//            public int RangeEnd;

//            public Dictionary<Word, TrieRange> Childs = new Dictionary<Word, TrieRange>();
//            public Dictionary<Word, Range> ThinRange = new Dictionary<Word, Range>();

//            public TrieRange() { }

//            public TrieRange AddRange(Word firstPinYin, Word pinYin, Word keyword, int index, int right)
//            {
//                RangeEnd = index;
//                if (right > Right) Right = (byte)right;

//                #region firstPinYin
//                TrieRange range;
//                if (Childs.TryGetValue(firstPinYin, out range) == false) {
//#if DEBUG
//                    range = new TrieRange(firstPinYin, _pys);
//#else
//                    range = new TrieRange();
//#endif
//                    range.RangeStart = index;
//                    Childs.Add(firstPinYin, range);
//                }
//                range.RangeEnd = index;
//                #endregion

//                #region pinYin
//                if (pinYin != firstPinYin) {
//                    Range r;
//                    if (range.ThinRange.TryGetValue(pinYin, out r) == false) {
//                        r = new Range(index);
//                        range.ThinRange.Add(pinYin, r);
//                    }
//                    r.Add(index);
//                }
//                #endregion

//                #region keyword
//                if (keyword != pinYin) {
//                    Range r2;
//                    if (range.ThinRange.TryGetValue(keyword, out r2) == false) {
//                        r2 = new Range(index);
//                        range.ThinRange.Add(keyword, r2);
//                    }
//                    r2.Add(index);
//                }
//                #endregion

//                return range;
//            }

//            public void Dispose()
//            {
//                Childs.Clear();
//                Childs = null;
//                ThinRange.Clear();
//                ThinRange = null;
//            }

//#if DEBUG
//            private PinYinSearchEx2 _pys;
//            private Word _word;
//            public TrieRange(PinYinSearchEx2 pys) { _pys = pys; }
//            public TrieRange(Word word, PinYinSearchEx2 pys) { _word = word; _pys = pys; }


//            public override string ToString()
//            {
//                var str = "";
//                for (int i = 0; i < char.MaxValue; i++) {
//                    if (_pys._pinyinDict1[i] == _word) {
//                        str += (char)i;
//                        break;
//                    }
//                }
//                foreach (var item in _pys._pinyinDict2) {
//                    if (item.Value == _word) {
//                        str += item.Key;
//                        break;
//                    }

//                }
//                return str;
//            }
//#endif

//        }
//        #endregion

//        #region 私有字段
//        //搜索类型
//        PinYinSearchType _type;
//        bool _startMiddle;
//        //关键字信息
//        private int[] _ids;//输入的关键字ID
//        private string[] _keywords;//输入的关键字
//        private int[] _indexs;//排序后关键字对应的索引，TrieRange类的RangeStart，RangeEnd指向这个索引
//        //字典
//        private Word[] _pinyinDict1;//
//        private Dictionary<string, Word> _pinyinDict2;//
//        //节点状态信息
//        private Word[] _words;//当前字符   前一级基数+当前字符=当前位置
//        private int[] _baseIndex;//下一级基数
//        private int[] _nodeIndex;//节点索引
//        //节点信息
//        private byte[] _right;//当前节点后还有的长度
//        private int[] _rangeStart;//
//        //private int[] _rangeEnd;//
//        //子节点信息
//        private int[] _subNodeBase;//子节点基数
//        private byte[] _subNodeLength;//子节点长度
//        private Word[] _subNodeWords;//当前子节字符
//        private int[] _subNodeRangeStart;//
//        private int[] _subNodeRangeEnd;//
//        #endregion

//        #region 构造方法
//        public PinYinSearchEx2(PinYinSearchType type = PinYinSearchType.PinYin, bool startMiddle = false)
//        {
//            _type = type;
//            _startMiddle = startMiddle;
//        }


//        #endregion

//        #region 公共方法
//        public void SetKeywords(List<string> keywords)
//        {
//            _keywords = keywords.ToArray();
//            buildArrary(keywords);
//        }

//        public void SetKeywords(List<string> keywords, List<int> ids)
//        {
//            if (keywords.Count != ids.Count) {
//                throw new ArgumentException("keywords与ids的数量不一致。");
//            }
//            _keywords = keywords.ToArray();
//            _ids = ids.ToArray();
//            buildArrary(keywords);
//        }

//        public void SaveFile(string filePath)
//        {
//            filePath = Path.GetFullPath(filePath);
//            var dir = Path.GetDirectoryName(filePath);
//            Directory.CreateDirectory(dir);
//            if (File.Exists(filePath) == false) {
//                File.Create(filePath).Close();
//            }
//            saveFile(filePath);
//        }

//        public void LoadFile(string filePath)
//        {
//            filePath = Path.GetFullPath(filePath);
//            if (File.Exists(filePath) == false) {
//                throw new ArgumentException(filePath + "文体不存在");
//            }
//            loadFile(filePath);
//        }

//        public List<string> SearchText(string text)
//        {
//            TextLine line;
//            if (trySplitSearchText(text, out line)) {
//                var indexs = matching(line);
//                List<string> results = new List<string>();
//                foreach (var item in indexs) {
//                    results.Add(_keywords[_indexs[item]]);
//                }
//                return results;
//            }
//            return new List<string>();
//        }

//        public List<int> SearchId(string text)
//        {
//            TextLine line;
//            if (trySplitSearchText(text, out line)) {
//                var indexs = matching(line);
//                List<int> results = new List<int>();
//                foreach (var item in indexs) {
//                    results.Add(_ids[_indexs[item]]);
//                }
//                return results;
//            }
//            return new List<int>();
//        }

//        public List<PinYinSearchResult> SearchTextAndId(string text)
//        {
//            TextLine line;
//            if (trySplitSearchText(text, out line)) {
//                var indexs = matching(line);
//                List<PinYinSearchResult> results = new List<PinYinSearchResult>();
//                foreach (var item in indexs) {
//                    int id = 0;
//                    if (_ids.Length == 0) id = _ids[_indexs[item]];
//                    results.Add(new PinYinSearchResult(_keywords[_indexs[item]], id));
//                }
//                return results;
//            }
//            return new List<PinYinSearchResult>();
//        }

//        #endregion

//        #region 匹配 关键字
//        private List<int> matching(TextLine line)
//        {
//            List<int> results = new List<int>();
//            Range range = new Range(0, _keywords.Length);
//            matching(line, _baseIndex[0], range, results);
//            return results;
//        }
//        private void matching(TextLine line, int baseIndex, Range range, List<int> results)
//        {
//            var topLine = line.TopLine;
//            if (topLine.Lines.Count == 0) {
//                range.AddTo(results);
//                return;
//            }
//            foreach (var nextLine in topLine.Lines) {
//                var index = baseIndex + nextLine.FristPinYin;
//                var word = _words[index];
//                if (word != nextLine.FristPinYin) continue;
//                var nodeIndex = _nodeIndex[index];
//                if (_right[nodeIndex] < nextLine.Right) continue;
//                var nextRange = GetRange(nextLine, range, nodeIndex);
//                if (nextRange.HasNone()) continue;
//                matching(nextLine, _baseIndex[index], nextRange, results);
//            }
//        }
//        private Range GetRange(TextLine line, Range range, int nodeIndex)
//        {
//            List<int> rs = new List<int>();
//            rs.Add(_rangeStart[nodeIndex]);
//            rs.Add(_rangeStart[nodeIndex + 1] - 1);
//            range = range.Intersection(rs);
//            if (range.HasNone()) return range;
//            if (line.Text == line.FristPinYin) return range;

//            var length = _subNodeLength[nodeIndex];
//            if (length == 0) return range;

//            var start = _subNodeBase[nodeIndex];
//            rs.Clear();
//            for (int i = start; i < start + length; i++) {
//                var word = _subNodeWords[i] - line.Text;
//                if (word == 0) {
//                    rs.Add(_subNodeRangeStart[i]);
//                    rs.Add(_subNodeRangeEnd[i]);
//                } else if (word > 0) {
//                    break;
//                }
//            }
//            range = range.Intersection(rs);
//            return range;
//        }

//        #endregion

//        #region 保存文件 加载文件
//        private void saveFile(string filePath)
//        {
//            var fs = File.OpenWrite(filePath);
//            BinaryWriter bw = new BinaryWriter(fs);

//            bw.Write((int)_type);
//            bw.Write(_startMiddle);
//            write(bw, _ids);
//            write(bw, _keywords);
//            write(bw, _indexs);
//            write(bw, _pinyinDict1);
//            write(bw, _pinyinDict2);
//            write(bw, _words);
//            write(bw, _baseIndex);
//            write(bw, _nodeIndex);
//            write(bw, _right);
//            write(bw, _rangeStart);
//            //write(bw, _rangeEnd);
//            write(bw, _subNodeBase);
//            write(bw, _subNodeLength);
//            write(bw, _subNodeWords);
//            write(bw, _subNodeRangeStart);
//            write(bw, _subNodeRangeEnd);

//            bw.Close();
//            fs.Close();
//        }
//        private void write(BinaryWriter bw, int[] w)
//        {
//            if (w == null || w.Length == 0) {
//                bw.Write(0);
//                return;
//            }
//            bw.Write(w.Length);
//            foreach (var item in w) {
//                bw.Write(item);
//            }
//        }
//        private void write(BinaryWriter bw, string[] texts)
//        {
//            if (texts == null || texts.Length == 0) {
//                bw.Write(0);
//                return;
//            }
//            bw.Write(texts.Length);
//            foreach (var item in texts) {
//                var bs = Encoding.UTF8.GetBytes(item);
//                bw.Write(bs.Length);
//                bw.Write(bs);
//            }
//        }
//        private void write(BinaryWriter bw, Word[] w)
//        {
//            if (w == null || w.Length == 0) {
//                bw.Write(0);
//                return;
//            }
//            bw.Write(w.Length);
//            foreach (var item in w) {
//                bw.Write(item);
//            }
//        }
//        private void write(BinaryWriter bw, byte[] w)
//        {
//            if (w == null || w.Length == 0) {
//                bw.Write(0);
//                return;
//            }
//            bw.Write(w.Length);
//            bw.Write(w);
//        }
//        private void write(BinaryWriter bw, Dictionary<string, Word> w)
//        {
//            if (w == null || w.Count == 0) {
//                bw.Write(0);
//                return;
//            }
//            bw.Write(w.Count);
//            foreach (var item in w) {
//                var bs = Encoding.UTF8.GetBytes(item.Key);
//                bw.Write(bs.Length);
//                bw.Write(bs);
//                bw.Write(item.Value);
//            }
//        }

//        private void loadFile(string filePath)
//        {
//            var fs = File.OpenRead(filePath);
//            BinaryReader br = new BinaryReader(fs);

//            _type = (PinYinSearchType)br.ReadInt32();
//            _startMiddle = br.ReadBoolean();
//            _ids = readIntArray(br);
//            _keywords = readStringArray(br);
//            _indexs = readIntArray(br);
//            _pinyinDict1 = readWordArray(br);
//            _pinyinDict2 = readDictionary(br);
//            _words = readWordArray(br);
//            _baseIndex = readIntArray(br);
//            _nodeIndex = readIntArray(br);
//            _right = readByteArray(br);
//            _rangeStart = readIntArray(br);
//            //_rangeEnd = readIntArray(br);

//            _subNodeBase = readIntArray(br);
//            _subNodeLength = readByteArray(br);
//            _subNodeWords = readWordArray(br);
//            _subNodeRangeStart = readIntArray(br);
//            _subNodeRangeEnd = readIntArray(br);

//            br.Close();
//            fs.Close();
//        }
//        private int[] readIntArray(BinaryReader br)
//        {
//            var count = br.ReadInt32();
//            int[] array = new int[count];
//            for (int i = 0; i < count; i++) {
//                array[i] = br.ReadInt32();
//            }
//            return array;
//        }
//        private string[] readStringArray(BinaryReader br)
//        {
//            var count = br.ReadInt32();
//            string[] array = new string[count];
//            for (int i = 0; i < count; i++) {
//                var length = br.ReadInt32();
//                byte[] bs = br.ReadBytes(length);
//                array[i] = Encoding.UTF8.GetString(bs);
//            }
//            return array;
//        }
//        private Word[] readWordArray(BinaryReader br)
//        {
//            var count = br.ReadInt32();
//            Word[] array = new Word[count];
//            for (int i = 0; i < count; i++) {
//                array[i] = br.ReadUInt16();
//            }
//            return array;
//        }
//        private byte[] readByteArray(BinaryReader br)
//        {
//            var count = br.ReadInt32();
//            byte[] array = br.ReadBytes(count);
//            return array;
//        }
//        private Dictionary<string, Word> readDictionary(BinaryReader br)
//        {
//            Dictionary<string, Word> dict = new Dictionary<string, Word>();
//            var count = br.ReadInt32();
//            for (int i = 0; i < count; i++) {
//                var length = br.ReadInt32();
//                byte[] bs = br.ReadBytes(length);
//                var str = Encoding.UTF8.GetString(bs);
//                var value = br.ReadUInt16();
//                dict.Add(str, value);
//            }
//            return dict;
//        }

//        #endregion

//        #region 关键字建Trie Tree , Trie Tree 转数组
//        private void buildArrary(List<string> keywords)
//        {
//            initPinYinDict(keywords);
//            var rs = SplitKeywords(keywords);
//            var root = setKeywordsWithPinYin(rs, keywords);
//            conversionToArray(root);
//            conversionTrieRange(root);
//            conversionRange(root);
//        }

//        #region Trie Tree 转数组
//        //类似二数组trieTree
//        private void conversionToArray(TrieRange range)
//        {
//            List<Word> words = new List<Word>() { 0 };
//            List<int> baseIndex = new List<int>() { 0 };
//            List<int> nodeIndex = new List<int>() { -1 };
//            int index = 0;
//            conversionToArray(range, words, baseIndex, nodeIndex, 0, ref index);

//            _words = words.ToArray();
//            _baseIndex = baseIndex.ToArray();
//            _nodeIndex = nodeIndex.ToArray();
//        }
//        private void conversionToArray(TrieRange range, List<Word> words, List<int> baseIndex, List<int> nodeIndex, int pIndex, ref int index)
//        {
//            if (range.Childs.Count == 0) return;
//            Word min = Word.MaxValue;
//            Word max = Word.MinValue;
//            foreach (var item in range.Childs) {
//                if (min > item.Key) min = item.Key;
//                if (max < item.Key) max = item.Key;
//            }
//            var start = words.Count - min;
//            baseIndex[pIndex] = start;
//            for (int i = min; i <= max; i++) {
//                words.Add(0);
//                baseIndex.Add(0);
//                nodeIndex.Add(0);
//            }
//            foreach (var item in range.Childs) {
//                words[start + item.Key] = item.Key;
//                nodeIndex[start + item.Key] = index++;
//            }

//            foreach (var item in range.Childs) {
//                conversionToArray(item.Value, words, baseIndex, nodeIndex, start + item.Key, ref index);
//            }
//        }
//        // 转化TrieRange 
//        private void conversionTrieRange(TrieRange range)
//        {
//            List<byte> right = new List<byte>();
//            List<int> rangeStart = new List<int>();
//            //List<int> rangeEnd = new List<int>();
//            conversionTrieRange(range, right, rangeStart);

//            rangeStart.Add(0);

//            _right = right.ToArray();
//            _rangeStart = rangeStart.ToArray();
//            //_rangeEnd = rangeEnd.ToArray();
//        }
//        private void conversionTrieRange(TrieRange range, List<byte> right, List<int> rangeStart)
//        {
//            foreach (var item in range.Childs) {
//                right.Add(item.Value.Right);
//                rangeStart.Add(item.Value.RangeStart);
//            }
//            foreach (var item in range.Childs) {
//                conversionTrieRange(item.Value, right, rangeStart);
//            }
//        }
//        // 转化Range
//        private void conversionRange(TrieRange range)
//        {
//            List<int> subNodeBase = new List<int>();
//            List<byte> subNodeLength = new List<byte>();
//            List<Word> subNodeWords = new List<Word>();
//            List<int> subNodeRangeStart = new List<int>();
//            List<int> subNodeRangeEnd = new List<int>();

//            conversionRange(range, subNodeBase, subNodeLength, subNodeWords, subNodeRangeStart, subNodeRangeEnd);

//            _subNodeBase = subNodeBase.ToArray();
//            _subNodeLength = subNodeLength.ToArray();
//            _subNodeWords = subNodeWords.ToArray();
//            _subNodeRangeStart = subNodeRangeStart.ToArray();
//            _subNodeRangeEnd = subNodeRangeEnd.ToArray();
//        }
//        private void conversionRange(TrieRange range, List<int> subNodeBase, List<byte> subNodeLength, List<Word> subNodeWords,
//            List<int> subNodeRangeStart, List<int> subNodeRangeEnd)
//        {
//            foreach (var item in range.Childs) {
//                var index = subNodeRangeStart.Count;
//                subNodeBase.Add(index);
//                var ranges = item.Value.ThinRange.OrderBy(q => q.Key);
//                foreach (var r in ranges) {
//                    var length = r.Value.Ranges.Count / 2;
//                    for (int i = 0; i < length; i++) {
//                        subNodeWords.Add(r.Key);
//                        subNodeRangeStart.Add(r.Value.Ranges[i * 2]);
//                        subNodeRangeEnd.Add(r.Value.Ranges[i * 2 + 1]);
//                    }
//                }
//                subNodeLength.Add((byte)(subNodeRangeStart.Count - index));
//            }
//            foreach (var item in range.Childs) {
//                conversionRange(item.Value, subNodeBase, subNodeLength, subNodeWords, subNodeRangeStart, subNodeRangeEnd);
//            }
//        }
//        #endregion

//        #region 关键字建Trie Tree
//        private TrieRange setKeywordsWithPinYin(List<string> pys, List<string> keywords)
//        {
//#if DEBUG
//            var _root = new TrieRange(this);
//#else
//            var _root = new TrieRange();
//#endif
//            List<int> indexs = new List<int>();
//            for (int i = 0; i < pys.Count; i++) {
//                var sp = pys[i].Split(wordsSpace);
//                var str = "";
//                var range = _root;
//                for (int j = 0; j < sp.Length; j++) {
//                    var s = sp[j].Split(pinYinSpace);
//                    range = range.AddRange(getPinYinIndex(s[0]), getPinYinIndex(s[1]), getPinYinIndex(s[2]), i, sp.Length - j);
//                    str += s[2];
//                }
//                indexs.Add(keywords.IndexOf(str));
//            }
//            _indexs = indexs.ToArray();
//            return _root;
//        }
//        #endregion

//        #region 关键字排列组合
//        //返回 拼音首字母\t拼音\t原文本\b……
//        private List<string> SplitKeywords(List<string> keywords)
//        {
//            List<string> results = new List<string>();
//            foreach (var keyword in keywords) {
//                splitKeywords(keyword.ToUpper(), results);
//            }
//            return results.OrderBy(q => q).ToList();
//        }

//        private void splitKeywords(string keyword, List<string> results)
//        {
//            List<List<string>> ks = new List<List<string>>();
//            if (_type == PinYinSearchType.PinYin) {
//                var pys = PinYinDict.GetPinYinList(keyword);
//                for (int i = 0; i < keyword.Length; i++) {
//                    var list = new List<string>();
//                    var t = keyword[i];
//                    if ((t >= '0' && t <= '9') || (t >= 'A' && t <= 'Z')) {
//                        list.Add(t.ToString());
//                    } else if (t >= 0x4e00 && t <= 0x9fa5) {
//                        var py = pys[i];
//                        if (py == null) py = " ";
//                        //if (py == t.ToString()) py = " ";
//                        list.Add(py.ToUpper());
//                    } else {
//                        list.Add(" ");
//                    }
//                    ks.Add(list);
//                }
//            } else {
//                for (int i = 0; i < keyword.Length; i++) {
//                    var list = new List<string>();
//                    var t = keyword[i];
//                    if ((t >= '0' && t <= '9') || (t >= 'A' && t <= 'Z')) {
//                        list.Add(t.ToString());
//                    } else if (t >= 0x4e00 && t <= 0x9fa5) {
//                        var pys = PinYinDict.GetAllPinYin(t);
//                        if (pys.Count == 0) pys.Add(" ");
//                        foreach (var item in pys) {
//                            list.Add(item.ToUpper());
//                        }
//                    } else {
//                        list.Add(" ");
//                    }
//                    ks.Add(list);
//                }
//            }
//            foreach (var item in ks[0]) {
//                var py = item[0].ToString() + pinYinSpace + item + pinYinSpace + keyword[0];
//                splitKeywords(py, 1, ks, keyword, results);
//            }
//        }
//        private void splitKeywords(string py, int index, List<List<string>> ks, string keyword, List<string> results)
//        {
//            if (ks.Count == index) {
//                results.Add(py);
//                return;
//            }

//            var k = ks[index];
//            foreach (var item in k) {
//                py += wordsSpace + item[0].ToString() + pinYinSpace + item + pinYinSpace + keyword[index].ToString();
//                splitKeywords(py, index + 1, ks, keyword, results);
//            }
//        }
//        #endregion
//        #endregion

//        #region 搜索字划分、拼音分词
//        class TextLine
//        {

//            public List<TextLine> Lines = new List<TextLine>();
//            public Word FristPinYin;
//            public Word Text;
//            public TextLine TopLine;
//            public byte Right;


//            public TextLine()
//            {
//                TopLine = this;
//                //Right = right;
//            }
//            public TextLine(Word fpy, Word t)
//            {
//                //Right = right;
//                FristPinYin = fpy;
//                Text = t;
//                TopLine = this;
//            }
//            public TextLine(Word fpy, Word t, TextLine topLine)
//            {
//                //Right = right;
//                FristPinYin = fpy;
//                Text = t;
//                TopLine = topLine;
//#if DEBUG
//                _pys = topLine._pys;
//#endif
//            }
//            public void Add(TextLine line)
//            {
//                //line.Right = (byte)(this.Right - 1);
//#if DEBUG
//                line._pys = this._pys;
//#endif
//                Lines.Add(line);
//            }
//#if DEBUG
//            private PinYinSearchEx2 _pys;
//            public TextLine(PinYinSearchEx2 pys)
//            {
//                TopLine = this;
//                //Right = right;
//                _pys = pys;
//            }
//            public override string ToString()
//            {
//                if (_pys == null) return base.ToString();

//                var str = "";
//                for (int i = 0; i < char.MaxValue; i++) {
//                    if (_pys._pinyinDict1[i] == FristPinYin) {
//                        str += (char)i + ",";
//                        break;
//                    }
//                }
//                for (int i = 0; i < char.MaxValue; i++) {
//                    if (_pys._pinyinDict1[i] == Text) {
//                        str += (char)i;
//                        break;
//                    }
//                }
//                foreach (var item in _pys._pinyinDict2) {
//                    if (item.Value == Text) {
//                        str += item.Key;
//                        break;
//                    }

//                }


//                return str;
//            }
//#endif
//            public void AutoSetRight()
//            {
//                if (TopLine.Lines.Count == 0) return;
//                var min = byte.MaxValue;
//                foreach (var item in TopLine.Lines) {
//                    if (item.Right == 0) item.AutoSetRight();
//                    if (item.Right < min) min = item.Right;
//                }
//                Right = (byte)(min + 1);
//            }

//        }
//        private static WordsSearch _pinyinSplit;
//        private WordsSearch getPinYinSplit()
//        {
//            if (_pinyinSplit == null) {
//                _pinyinSplit = new WordsSearch();
//                List<string> pys = new List<string>();
//                foreach (var item in PinYinDict.pyName) {
//                    var t = item.ToUpper();
//                    if (t.Length < 2) continue;
//                    pys.Add(t);
//                }
//                _pinyinSplit.SetKeywords(pys);
//            }
//            return _pinyinSplit;
//        }
//        private bool trySplitSearchText(string text, out TextLine root)
//        {
//            text = text.ToUpper();
//            var rs = getPinYinSplit().FindAll(text);
//            #region 初始TextLine
//#if DEBUG
//            root = new TextLine(this);
//#else
//            root = new TextLine((byte)text.Length);
//#endif

//            List<TextLine> textLines = new List<TextLine>() { root };
//            for (int i = 0; i < text.Length; i++) {
//                var c = text[i];
//                var word = getPinYinIndex(c);
//                if (word == 0) return false;
//                if (_type == PinYinSearchType.PinYin && c >= 0x4e00 && c <= 0x9fa5) {
//                    var pys = PinYinDict.GetAllPinYin(c);
//                    if (pys.Count == 0) pys.Add(" ");
//                    var list = new List<char>();
//                    for (int j = 0; j < pys.Count; j++) {
//                        var fpy = pys[j][0];
//                        if (list.Contains(fpy)) continue;
//                        list.Add(fpy);
//                        TextLine tl;// = new TextLine();
//                        if (j == 0) {
//                            tl = new TextLine(getPinYinIndex(fpy), word);
//                            textLines.Add(tl);
//                        } else {
//                            tl = new TextLine(getPinYinIndex(fpy), word, textLines[i + 1]);
//                        }
//                        textLines[i].Add(tl);
//                    }
//                } else {
//                    var fpy = c;
//                    if (c >= 0x4e00 && c <= 0x9fa5) {
//                        fpy = PinYinDict.GetPinYinFast(c)[0];
//                    } else if (c >= '0' || c <= '9') {
//                        fpy = c;
//                    } else if (c >= 'A' || c <= 'Z') {
//                        fpy = c;
//                    } else {
//                        fpy = ' ';
//                    }
//                    TextLine tl = new TextLine(getPinYinIndex(fpy), word);
//                    textLines[i].Add(tl);
//                    textLines.Add(tl);
//                }

//            }
//            #endregion
//            #region 划分拼音
//            foreach (var r in rs) {
//                Word word;
//                if (tryGetPinYinIndex(r.Keyword, out word)) {
//                    var tl2 = new TextLine(getPinYinIndex(r.Keyword[0]), word, textLines[r.End + 1]);
//                    textLines[r.Start].Add(tl2);
//                }
//            }
//            #endregion
//            root.AutoSetRight();


//            return true;
//        }
//        #endregion

//        #region 字典库
//        private void initPinYinDict(List<string> keywords)
//        {
//            ushort index = 1;//不能为0，为0代表关键字不存在。搜索时返回为空。
//            Dictionary<string, Word> dict = new Dictionary<string, Word>();
//            Dictionary<char, int> chinese_sum = new Dictionary<char, int>();
//            Dictionary<string, int> py_sum = new Dictionary<string, int>();

//            #region 初始空格 0-9 A-Z
//            dict[" "] = index++;
//            var c = '0';
//            while (c <= '9') {
//                dict[c.ToString()] = index++;
//                c++;
//            }
//            c = 'A';
//            while (c <= 'Z') {
//                dict[c.ToString()] = index++;
//                c++;
//            }
//            #endregion

//            #region 统计字符
//            foreach (var item in keywords) {
//                var keyword = item.ToUpper();
//                for (int j = 0; j < keyword.Length; j++) {
//                    c = keyword[j];
//                    if ((c != ' ' || c < '0' || c > '9') && (c < 'A' || c > 'Z')) {
//                        int count = 0;
//                        chinese_sum.TryGetValue(c, out count);
//                        chinese_sum[c] = count + 1;
//                        if (c >= 0x4e00 && c <= 0x9fa5) {
//                            var pys = PinYinDict.GetAllPinYin(c);
//                            foreach (var py in pys) {
//                                var pyUp = py.ToUpper();
//                                count = 0;
//                                py_sum.TryGetValue(pyUp, out count);
//                                py_sum[pyUp] = count + 1;
//                            }
//                        }
//                    }
//                }
//            }
//            #endregion

//            #region 初始拼音
//            foreach (var item in py_sum.OrderBy(q => q.Value)) {
//                if (item.Key.Length < 2) continue;
//                var t = item.Key.ToUpper();
//                dict[t] = index++;
//            }
//            #endregion

//            #region 初始中文，符号
//            foreach (var item in chinese_sum.OrderByDescending(q => q.Value)) {
//                var t = item.Key.ToString();
//                dict[t] = index++;
//            }
//            #endregion

//            #region 保存
//            _pinyinDict1 = new Word[char.MaxValue + 1];
//            _pinyinDict2 = new Dictionary<string, Word>();
//            foreach (var item in dict) {
//                if (item.Key.Length == 1) {
//                    _pinyinDict1[item.Key[0]] = item.Value;
//                } else {
//                    _pinyinDict2[item.Key] = item.Value;
//                }
//            }
//            #endregion
//        }
//        private Word getPinYinIndex(string key)
//        {
//            if (key.Length == 1) {
//                return _pinyinDict1[key[0]];
//            }
//            Word index = 0;
//            if (_pinyinDict2.TryGetValue(key, out index)) {
//                return index;
//            }
//            return 0;
//        }
//        private Word getPinYinIndex(char key)
//        {
//            return _pinyinDict1[key];
//        }
//        private bool tryGetPinYinIndex(string key, out Word word)
//        {
//            return _pinyinDict2.TryGetValue(key, out word);
//        }


//        #endregion
//    }
//}
