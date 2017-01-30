//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Word = System.UInt16;

//namespace ToolGood.Words
//{
//    public class PinYinSearchEx3
//    {
//        const char wordsSpace = '\b';// (char)1;
//        const char pinYinSpace = '\t';// (char)2;

//        #region 私有字段
//        //搜索类型
//        PinYinSearchType _type;
//        bool _startMiddle;
//        //关键字信息
//        private int[] _ids;//输入的关键字ID
//        private string[] _keywords;//输入的关键字
//        private int[] _indexs;//排序后关键字对应的索引，TrieRange类的RangeStart，RangeEnd指向这个索引
//        private Word[][] _toWords;//关键字转word
//        //字典
//        private byte[] _pinyinDict1;// 小于128，
//        private Word[] _pinyinDict2;// 英文，字符，单音汉字字典，
//        private Dictionary<string, Word> _pinyinDict3;//拼音，多音汉字字典。
//        private byte[] _pinyinDict4;//汉字拼音转拼音

//        //节点状态信息
//        private int[] _nodeIndex;//节点索引
//        //节点信息
//        private byte[] _words;//当前字符   前一级基数+当前字符=当前位置 并且 当前字符 - 前一字符 ！= 1 
//        private int[] _baseIndex;//下一级基数
//        private byte[] _left;//当前节点位置
//        private byte[] _right;//当前节点后还有的长度
//        private int[] _rangeStart;//
//        #endregion

//        #region 构造方法
//        public PinYinSearchEx3(PinYinSearchType type = PinYinSearchType.PinYin, bool startMiddle = false)
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


//        #endregion


//        #region 搜索字划分、拼音分词
//        class TextLine
//        {
//            public List<TextLine> Lines = new List<TextLine>();
//            public byte FristPinYin;
//            public Word Text;
//            public TextLine TopLine;
//            public byte Right;

//            public TextLine()
//            {
//                TopLine = this;
//                //Right = right;
//            }
//            public TextLine(byte fpy, Word t)
//            {
//                //Right = right;
//                FristPinYin = fpy;
//                Text = t;
//                TopLine = this;
//            }
//            public TextLine(byte fpy, Word t, TextLine topLine)
//            {
//                //Right = right;
//                FristPinYin = fpy;
//                Text = t;
//                TopLine = topLine;
//            }
//            public void Add(TextLine line)
//            {
//                Lines.Add(line);
//            }
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
//            root = new TextLine();

//            List<TextLine> textLines = new List<TextLine>() { root };
//            for (int i = 0; i < text.Length; i++) {
//                var c = text[i];
//                var word = _pinyinDict2[c];
//                if (word == 0) return false;
//                if (word==Word.MaxValue ) {
//                    if (_type == PinYinSearchType.PinYin) {
//                        var pys = PinYinDict.GetAllPinYin(c);
//                        if (pys.Count == 0) pys.Add(" ");

//                        for (int j = 0; j < pys.Count; j++) {
//                            word = _pinyinDict3[pys[j] + wordsSpace + c];
//                            var fpy = _pinyinDict4[word];
//                            TextLine tl;// = new TextLine();
//                            if (j == 0) {
//                                tl = new TextLine(fpy, word);
//                                textLines.Add(tl);
//                            } else {
//                                tl = new TextLine(fpy, word, textLines[i + 1]);
//                            }
//                            textLines[i].Add(tl);
//                        }
//                    } else {
//                        TextLine tl = new TextLine(_pinyinDict4[word], word);
//                        textLines[i].Add(tl);
//                        textLines.Add(tl);
//                    }
//                } else {
//                    TextLine tl = new TextLine(_pinyinDict4[word], word);
//                    textLines[i].Add(tl);
//                    textLines.Add(tl);
//                }


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
//                if (_pinyinDict3.TryGetValue(r.Keyword, out word)) {
//                    var fpy = _pinyinDict4[word];
//                    var tl2 = new TextLine(fpy, word, textLines[r.End + 1]);
//                    textLines[r.Start].Add(tl2);
//                }
//            }
//            #endregion
//            root.AutoSetRight();
//            return true;
//        }
//        #endregion

//        #region 关键字建Trie Tree , Trie Tree 转数组
//        private void buildArrary(List<string> keywords)
//        {
//            initPinYinDict(keywords);
//            var rs = SplitKeywords(keywords);
//            setToWords(rs);
//            var root = setKeywordsWithPinYin(rs);
//            conversionToArray(root);
//        }

//        private void conversionToArray(TrieRange range)
//        {
//            List<int> nodeIndex = new List<int>() { -1 };
//            List<byte> words = new List<byte>() { 0 };
//            List<int> baseIndex = new List<int>() { 0 };
//            List<byte> left = new List<byte>() { 0 };
//            List<byte> right = new List<byte>() { 0 };
//            List<int> rangeStart = new List<int>() { 0 };

//            conversionRange(range, words, baseIndex, left, right, rangeStart);

//            int index = 1;
//            conversionToArray(range, nodeIndex, baseIndex, 0, ref index);
//            for (int i = 0; i < 37; i++) nodeIndex.Add(0);

//            _nodeIndex = nodeIndex.ToArray();
//            _words = words.ToArray();
//            _baseIndex = baseIndex.ToArray();
//            _left = left.ToArray();
//            _right = _left.ToArray();
//            _rangeStart = rangeStart.ToArray();
//        }

//        private void conversionToArray(TrieRange range, List<int> nodeIndex, List<int> baseIndex, int index1, ref int index2)
//        {
//            if (range.Childs.Count == 0) return;
//            Word min = Word.MaxValue;
//            Word max = Word.MinValue;
//            foreach (var item in range.Childs) {
//                if (min > item.Key) min = item.Key;
//                if (max < item.Key) max = item.Key;
//            }
//            var start = nodeIndex.Count - min;
//            while (nodeIndex.Contains(start)) start++;
//            baseIndex[index1] = start;

//            for (int i = nodeIndex.Count; i <= start + max; i++) {
//                nodeIndex.Add(0);
//            }

//            var oldIndex = index2;
//            var keys = range.Childs.Keys.ToList();
//            for (int i = 0; i < keys.Count; i++) {
//                nodeIndex[start + keys[i]] = index2++;
//            }

//            for (int i = 0; i < keys.Count; i++) {
//                var key = keys[i];
//                var value = range.Childs[key];
//                conversionToArray(value, nodeIndex, baseIndex, oldIndex + i + 1, ref index2);
//            }
//        }

//        private void conversionRange(TrieRange range, List<byte> words, List<int> baseIndex, List<byte> left, List<byte> right, List<int> rangeStart)
//        {
//            var keys = range.Childs.Keys.ToList();
//            for (int i = 0; i < keys.Count; i++) {
//                var key = keys[i];
//                var value = range.Childs[key];
//                words.Add(key);
//                baseIndex.Add(0);
//                left.Add(value.Left);
//                right.Add(value.Right);
//                rangeStart.Add(value.RangeStart);
//            }
//            foreach (var item in range.Childs) {
//                conversionRange(item.Value, words, baseIndex, left, right, rangeStart);
//            }
//        }

//        private TrieRange setKeywordsWithPinYin(List<string> pys)
//        {
//            var _root = new TrieRange(-1, 0, 0);

//            List<int> indexs = new List<int>();
//            for (int i = 0; i < pys.Count; i++) {
//                var sp = pys[i].Split(wordsSpace);
//                indexs.Add(int.Parse(sp[3]));
//                var range = _root;
//                for (int j = 0; j < sp[0].Length; j++) {
//                    var s = sp[0][j];
//                    range = range.AddTrieRange(_pinyinDict1[s], i, (byte)j, (byte)(sp.Length - j));
//                }
//            }
//            _indexs = indexs.ToArray();
//            return _root;
//        }

//        private void setToWords(List<string> pys)
//        {
//            _toWords = new Word[pys.Count][];
//            for (int i = 0; i < pys.Count; i++) {
//                var py = pys[i];
//                var sp = py.Split(wordsSpace);

//                var words = new Word[sp[2].Length];
//                for (int j = 0; j < sp[2].Length; j++) {
//                    var c = sp[2][j];
//                    var w = _pinyinDict2[c];
//                    if (w == Word.MaxValue) {
//                        var key = sp[1].Split(pinYinSpace)[j] + wordsSpace + c;
//                        if (_pinyinDict3.TryGetValue(key, out w)) {
//                            words[j] = _pinyinDict3[key];
//                        } else {

//                        }



//                    } else {
//                        words[j] = w;
//                    }
//                }
//                _toWords[i] = words;
//            }
//        }


//        #region 关键字排列组合
//        //返回 拼音首字母\t拼音\t原文本\b……
//        private List<string> SplitKeywords(List<string> keywords)
//        {
//            List<string> results = new List<string>();
//            foreach (var keyword in keywords) {
//                splitKeywords(keyword.ToUpper(), keywords.IndexOf(keyword), results);
//            }
//            return results.OrderBy(q => q).ToList();
//        }

//        private void splitKeywords(string keyword, int keywordIndex, List<string> results)
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

//                var py = item.ToString();
//                var fpy = py[0].ToString();
//                splitKeywords(fpy, py, 1, ks, keyword, keywordIndex, results);
//            }
//        }
//        private void splitKeywords(string fpy, string py, int index, List<List<string>> ks, string keyword, int keywordIndex, List<string> results)
//        {
//            if (ks.Count == index) {
//                results.Add(fpy + wordsSpace + py + wordsSpace + keyword + wordsSpace + keywordIndex);
//                return;
//            }
//            var k = ks[index];
//            foreach (var item in k) {
//                splitKeywords(fpy + item[0], py + pinYinSpace + item,
//                    index + 1, ks, keyword, keywordIndex, results);
//            }
//        }
//        #endregion

//        #endregion

//        #region 字典库
//        private void initPinYinDict(List<string> keywords)
//        {
//            Dictionary<char, int> fpySum = new Dictionary<char, int>();//拼音首字母，数字，空格统计
//            HashSet<string> pyList = new HashSet<string>();//拼音列表
//            HashSet<string> ChinesePinYinList = new HashSet<string>();//拼音列表
//            //Dictionary<char, string> toPyList = new Dictionary<char, string>();//单音中文转拼音
//            //HashSet<char> charList = new HashSet<char>();//中文符号列表
//            HashSet<char> pysList = new HashSet<char>();//多拼音列表

//            #region 分析关键字
//            for (int i = 0; i < keywords.Count; i++) {
//                var keyword = keywords[i].ToUpper();
//                for (int j = 0; j < keyword.Length; j++) {
//                    var c = keyword[j];
//                    if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')) {
//                        var count = 0;
//                        fpySum.TryGetValue(c, out count);
//                        fpySum[c] = count + 1;
//                    } else if (c >= 0x4e00 && c <= 0x9fa5) {
//                        var pys = PinYinDict.GetAllPinYin(c);
//                        if (pys.Count > 1) {
//                            pysList.Add(c);
//                            foreach (var py in pys) ChinesePinYinList.Add(py.ToUpper() + wordsSpace + c);
//                        }
//                        foreach (var py in pys) {
//                            pyList.Add(py.ToUpper());
//                            pyList.Add(py.ToUpper() + wordsSpace + c);
//                            var count = 0;
//                            fpySum.TryGetValue(py[0], out count);
//                            fpySum[py[0]] = count + 1;
//                        }
//                    } else {
//                        pyList.Add(" " + wordsSpace + c);
//                        var count = 0;
//                        fpySum.TryGetValue(' ', out count);
//                        fpySum[' '] = count + 1;
//                    }
//                }
//            }
//            #endregion

//            #region 设置
//            bool append = true;
//            List<char> fpys = new List<char>();
//            foreach (var item in fpySum.OrderByDescending(q => q.Value)) {
//                if (append) {
//                    append = false;
//                    fpys.Add(item.Key);
//                } else {
//                    append = true;
//                    fpys.Insert(0, item.Key);
//                }
//                pyList.Add(item.Key.ToString());
//            }
//            #endregion

//            var index = 1;
//            _pinyinDict1 = new byte[128];
//            _pinyinDict2 = new Word[char.MaxValue + 1];
//            _pinyinDict3 = new Dictionary<string, Word>();
//            #region 设置A-Z 0-9 空格
//            foreach (var item in fpys) {
//                _pinyinDict1[item] = (byte)index++;
//            }
//            #endregion

//            #region 设置字典
//            var pySort = pyList.OrderBy(q => q).ToList();
//            //_pinyinDict4 = new Word[pySort.Count + 1];
//            for (int i = 0; i < pySort.Count; i++) {
//                var py = pySort[i];
//                if (py.Contains(wordsSpace) == false) {
//                    if (py.Length == 1) {
//                        _pinyinDict2[py[0]] = (Word)(i + 1);
//                    } else {
//                        _pinyinDict3[py] = (Word)(i + 1);
//                    }
//                } else {
//                    var sp = py.Split(wordsSpace);
//                    if (pysList.Contains(sp[1][0])) {
//                        _pinyinDict2[sp[1][0]] = Word.MaxValue;
//                        _pinyinDict3[py] = (Word)(i + 1);

//                    } else {
//                        _pinyinDict2[sp[1][0]] = (Word)(i + 1);
//                    }
//                    //_pinyinDict4[(Word)(i + 1)] = (Word)(pySort.IndexOf(sp[0]) + 1);
//                }
//            }
//            #endregion
//        }
//        #endregion

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
//        class TrieRange
//        {
//            public byte Left = 0;
//            public byte Right = 0;
//            public int RangeStart;
//            public int RangeEnd;
//            public Dictionary<byte, TrieRange> Childs = new Dictionary<byte, TrieRange>();

//            public TrieRange(int index, byte left, byte right)
//            {
//                Left = left;
//                Right = right;
//                RangeStart = index;
//                RangeEnd = index;
//            }

//            public TrieRange AddTrieRange(byte firstPinYin, int index, byte left, byte right)
//            {
//                if (RangeEnd < index) {
//                    RangeEnd = index;
//                }
//                TrieRange range;
//                if (Childs.TryGetValue(firstPinYin, out range) == false) {
//                    range = new TrieRange(index, left, right);
//                    Childs.Add(firstPinYin, range);
//                }
//                range.RangeEnd = right;
//                return range;
//            }
//        }
//        #endregion
//    }
//}
