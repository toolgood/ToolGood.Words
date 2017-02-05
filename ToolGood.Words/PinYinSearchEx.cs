using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    /// <summary>
    /// PinYinSearch增强版，占用内存更小，
    /// 经测试28W个关键字，总字数147W，内存使用36M，输出文件34.1M。
    /// 原PinYinSearch测试时，内存使用700M以上。
    /// 
    /// 使用SaveFile、LoadFile可快速设置关键字。
    /// 
    /// 
    /// </summary>
    public class PinYinSearchEx
    {
        const char wordsSpace = '\b';// (char)1;
        const char pinYinSpace = '\t';// (char)2;

        #region class
        class FirstPinYinNode : IDisposable
        {
            public Dictionary<ushort, PinYinNode> PinYinNodes = new Dictionary<ushort, PinYinNode>();

            public ChineseNode AddNode(ushort pinYin, ushort chinese, int index)
            {
                PinYinNode node;
                if (PinYinNodes.TryGetValue(pinYin, out node) == false) {
                    node = new PinYinNode();
                    PinYinNodes.Add(pinYin, node);
                }
                return node.AddNode(chinese, index);
            }

            public void Dispose()
            {
                if (PinYinNodes != null) {
                    PinYinNodes.Clear();
                    PinYinNodes = null;
                }
            }
        }
        class PinYinNode : IDisposable
        {
            public Dictionary<ushort, ChineseNode> ChineseNodes = new Dictionary<ushort, ChineseNode>();

            public ChineseNode AddNode(ushort chinese, int index)
            {
                ChineseNode node;
                if (ChineseNodes.TryGetValue(chinese, out node) == false) {
                    node = new ChineseNode();
                    ChineseNodes.Add(chinese, node);
                    node.RangeStart = index;
                }
                node.RangeEnd = index;
                return node;
            }

            public void Dispose()
            {
                if (ChineseNodes != null) {
                    ChineseNodes.Clear();
                    ChineseNodes = null;
                }
            }
        }
        class ChineseNode : IDisposable
        {
            public Dictionary<byte, FirstPinYinNode> FirstPinYinNodes = new Dictionary<byte, FirstPinYinNode>();
            public int RangeStart;
            public int RangeEnd;
            //public byte Right;
            public int Index;

            public bool IsEnd
            {
                get
                {
                    return FirstPinYinNodes.Count == 0;
                }
            }

            public ChineseNode AddNode(byte firstPinYin, ushort pinYin, ushort chinese, int index)
            {
                RangeEnd = index;
                FirstPinYinNode node;
                if (FirstPinYinNodes.TryGetValue(firstPinYin, out node) == false) {
                    node = new FirstPinYinNode();
                    FirstPinYinNodes.Add(firstPinYin, node);
                }
                return node.AddNode(pinYin, chinese, index);
            }

            public Dictionary<ushort, ChineseNode> GetNodes()
            {
                Dictionary<ushort, ChineseNode> nodes = new Dictionary<ushort, ChineseNode>();
                foreach (var fpyn in FirstPinYinNodes) {
                    foreach (var pyn in fpyn.Value.PinYinNodes) {
                        foreach (var n in pyn.Value.ChineseNodes) {
                            nodes.Add(n.Key, n.Value);
                        }
                    }
                }
                return nodes;
            }

            public void Dispose()
            {
                if (FirstPinYinNodes != null) {
                    FirstPinYinNodes.Clear();
                    FirstPinYinNodes = null;
                }
            }
        }
        class TextLine
        {
            public List<TextLine> Lines = new List<TextLine>();
            public byte FristPinYin;
            public ushort PinYin;
            public ushort Chinese;
            public TextLine TopLine;
            //public byte Right;
            public bool IsError;

            public TextLine() { TopLine = this; }
            public TextLine(byte fristPinYin, ushort pinYin, ushort chinese)
            {
                FristPinYin = fristPinYin;
                PinYin = pinYin;
                Chinese = chinese;
                TopLine = this;
            }
            public TextLine(byte fristPinYin, ushort pinYin, ushort chinese, TextLine line)
            {
                FristPinYin = fristPinYin;
                PinYin = pinYin;
                Chinese = chinese;
                TopLine = line;
            }

            public void Add(TextLine line)
            {
                Lines.Add(line);
            }

            public void FilterError()
            {
                if (TopLine.Lines.Count == 0) return;
                foreach (var item in TopLine.Lines) {
                    item.FilterError();
                }
                var count = 0;
                foreach (var item in TopLine.Lines) {
                    if (item.IsError) {
                        count++;
                    }
                }
                if (count == TopLine.Lines.Count) {
                    this.IsError = true;
                }
            }

            //public void AutoSetRight()
            //{
            //    if (TopLine.Lines.Count == 0) return;
            //    var min = byte.MaxValue;
            //    foreach (var item in TopLine.Lines) {
            //        if (item.Right == 0) item.AutoSetRight();
            //        if (item.Right < min) min = item.Right;
            //    }
            //    Right = (byte)(min + 1);
            //}

            public static TextLine Error
            {
                get
                {
                    var line = new TextLine();
                    line.IsError = true;
                    return line;
                }

            }
        }

        #endregion

        #region 私有字段
        //搜索类型
        PinYinSearchType _type;

        //关键字信息
        private int[] _ids;//输入的关键字ID
        private string[] _keywords;//输入的关键字 
        private int[] _indexs;//排序后关键字对应的索引，TrieRange类的RangeStart，RangeEnd指向这个索引

        // 单字映射
        private ushort[] _toWord1;//字母、单音文字、符号映射标记
        private Dictionary<string, ushort> _toWord2;//拼音、多音文字映射标记
        private byte[] _toFirstPinYin;//
        private ushort[] _wordToPinYin;//转拼音映射标记
        private byte[] _pinYinToFirstPinYin;//拼音转首字母拼音映射标记
        public ushort[] _pinYinStart;//拼音映射标记从哪里开始，都是字符串映秀标记
        public ushort[] _firstPinYinStart;//首字母拼音映射标记从哪里开始，都是拼音映射标记

        //节点信息
        private ushort[] _word;//节点文字
        private int[] _start;//节点对前的排序
        private int[] _end;//节点对前的排序
        private BitArray _isEnd;//
        private int[] _fpyIndexBase;//下一层首字母拼音的开始基数

        private byte[] _fpy;//首字母 为0 失败
        private int[] _pyIndexBase;//下一层拼音的开始基数

        private ushort[] _py;//拼音 为0 失败
        private int[] _wordIndexBase;//下一层文字的开始基数

        private int[] _wordCheck;//为0 失败 ，反回节点的索引

        #endregion

        #region 构造方法
        public PinYinSearchEx(PinYinSearchType type = PinYinSearchType.PinYin)
        {
            _type = type;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字</param>
        public void SetKeywords(List<string> keywords)
        {
            setKeywords(keywords);
            _ids = new int[0];
        }
        /// <summary>
        /// 设置关键字和IDs
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <param name="ids">IDs</param>
        public void SetKeywords(List<string> keywords, List<int> ids)
        {
            if (keywords.Count != ids.Count) throw new ArgumentException("keywords and ids inconsistent number.");
            setKeywords(keywords);
            _ids = ids.ToArray();
        }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <returns></returns>
        public List<string> SearchTexts(string text, bool keywordSort = false)
        {
            var word_indexs = searchIndexs(text);
            if (word_indexs.Count == 0) return new List<string>();
            List<int> indexs = new List<int>();
            foreach (var word_index in word_indexs) {
                for (int i = _start[word_index]; i <= _end[word_index]; i++) {
                    indexs.Add(_indexs[i]);
                }
            }
            indexs = indexs.Distinct().ToList();
            if (keywordSort == false) {
                indexs = indexs.OrderBy(q => q).ToList();
            }
            List<string> texts = new List<string>();
            foreach (var index in indexs) {
                texts.Add(_keywords[index]);
            }
            return texts;
        }
        /// <summary>
        /// 搜索IDs
        /// </summary>
        /// <param name="text"文本></param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <returns></returns>
        public List<int> SearchIds(string text, bool keywordSort = false)
        {
            var word_indexs = searchIndexs(text);
            if (word_indexs.Count == 0) return new List<int>();
            List<int> indexs = new List<int>();
            foreach (var word_index in word_indexs) {
                for (int i = _start[word_index]; i <= _end[word_index]; i++) {
                    indexs.Add(_indexs[i]);
                }
            }
            indexs = indexs.Distinct().ToList();
            if (keywordSort == false) {
                indexs = indexs.OrderBy(q => q).ToList();
            }
            List<int> ids = new List<int>();
            foreach (var index in indexs) {
                ids.Add(_indexs[index]);
            }
            return ids;
        }
        /// <summary>
        /// 搜索关键字和IDs
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <returns></returns>
        public List<PinYinSearchResult> SearchTextWithIds(string text, bool keywordSort = false)
        {
            var word_indexs = searchIndexs(text);
            if (word_indexs.Count == 0) return new List<PinYinSearchResult>();
            List<int> indexs = new List<int>();
            foreach (var word_index in word_indexs) {
                for (int i = _start[word_index]; i <= _end[word_index]; i++) {
                    indexs.Add(_indexs[i]);
                }
            }
            indexs = indexs.Distinct().ToList();
            if (keywordSort == false) {
                indexs = indexs.OrderBy(q => q).ToList();
            }
            List<PinYinSearchResult> results = new List<PinYinSearchResult>();
            foreach (var index in indexs) {
                results.Add(new PinYinSearchResult(_keywords[index], _ids[index]));
            }
            return results;
        }

        /// <summary>
        /// 挑选关键字，关键字在指定长度内，最前显示
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <param name="pickLength">挑选长度</param>
        /// <returns></returns>
        public List<string> PickTexts(string text, bool keywordSort = false, int pickLength = 2)
        {
            var word_indexs = searchIndexs(text);
            if (word_indexs.Count == 0) return new List<string>();
            List<int> indexs = new List<int>();
            foreach (var word_index in word_indexs) {
                for (int i = _start[word_index]; i <= _end[word_index]; i++) {
                    indexs.Add(_indexs[i]);
                }
            }
            indexs = indexs.Distinct().ToList();
            if (keywordSort == false) {
                indexs = indexs.OrderBy(q => q).ToList();
            }
            List<string> results = new List<string>();
            List<string> appends = new List<string>();
            List<string> appends2 = new List<string>();
            var textCount = text.Length;
            var count = textCount + pickLength;
            foreach (var index in indexs) {
                var key = _keywords[index];
                if (key.Length == textCount) {
                    results.Add(key);
                } else if (key.Length <= count) {
                    appends.Add(key);
                } else {
                    appends2.Add(key);
                }
            }
            results.AddRange(appends);
            results.AddRange(appends2);
            return results;
        }

        /// <summary>
        /// 挑选关键字和IDs，关键字在指定长度内，最前显示
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <param name="pickLength">挑选长度</param>
        /// <returns></returns>
        public List<PinYinSearchResult> PickTextWithIds(string text, bool keywordSort = false, int pickLength = 2)
        {
            var word_indexs = searchIndexs(text);
            if (word_indexs.Count == 0) return new List<PinYinSearchResult>();
            List<int> indexs = new List<int>();
            foreach (var word_index in word_indexs) {
                for (int i = _start[word_index]; i <= _end[word_index]; i++) {
                    indexs.Add(_indexs[i]);
                }
            }
            indexs = indexs.Distinct().ToList();
            if (keywordSort == false) {
                indexs = indexs.OrderBy(q => q).ToList();
            }
            List<PinYinSearchResult> results = new List<PinYinSearchResult>();
            List<PinYinSearchResult> appends = new List<PinYinSearchResult>();
            List<PinYinSearchResult> appends2 = new List<PinYinSearchResult>();
            var textCount = text.Length;
            var count = textCount + pickLength;
            foreach (var index in indexs) {
                var key = _keywords[index];
                if (key.Length == textCount) {
                    results.Add(new PinYinSearchResult(key, _ids[index]));
                } else if (key.Length <= count) {
                    appends.Add(new PinYinSearchResult(key, _ids[index]));
                } else {
                    appends2.Add(new PinYinSearchResult(key, _ids[index]));
                }
            }
            results.AddRange(appends);
            results.AddRange(appends2);
            return results;
        }


        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            filePath = Path.GetFullPath(filePath);
            var dir = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(dir);
            if (File.Exists(filePath) == false) {
                File.Create(filePath).Close();
            }
            saveFile(filePath);
        }
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            filePath = Path.GetFullPath(filePath);
            if (File.Exists(filePath) == false) {
                throw new ArgumentException(filePath + "文体不存在");
            }
            loadFile(filePath);
        }
        #endregion

        #region LoadFile
        private void loadFile(string filePath)
        {
            var fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);

            _type = (PinYinSearchType)br.ReadInt32();

            //关键字信息
            _ids = readIntArray(br);
            _keywords = readStringArray(br);
            _indexs = readIntArray(br);
            // 单字映射
            _toWord1 = readWordArray(br);
            _toWord2 = readDictionary(br);
            _toFirstPinYin = readByteArray(br);
            _wordToPinYin = readWordArray(br);
            _pinYinToFirstPinYin = readByteArray(br);
            _pinYinStart = readWordArray(br);
            _firstPinYinStart = readWordArray(br);
            //节点信息
            _word = readWordArray(br);
            _start = readIntArray(br);
            _end = readIntArray(br);
            _isEnd = readBitArray(br);
            _fpyIndexBase = readIntArray(br);
            _fpy = readByteArray(br);
            _pyIndexBase = readIntArray(br);
            _py = readWordArray(br);
            _wordIndexBase = readIntArray(br);
            _wordCheck = readIntArray(br);

            br.Close();
            fs.Close();
        }
        private int[] readIntArray(BinaryReader br)
        {
            var count = br.ReadInt32();
            int[] array = new int[count];
            for (int i = 0; i < count; i++) {
                array[i] = br.ReadInt32();
            }
            return array;
        }
        private string[] readStringArray(BinaryReader br)
        {
            var count = br.ReadInt32();
            string[] array = new string[count];
            for (int i = 0; i < count; i++) {
                var length = br.ReadInt32();
                byte[] bs = br.ReadBytes(length);
                array[i] = Encoding.UTF8.GetString(bs);
            }
            return array;
        }
        private ushort[] readWordArray(BinaryReader br)
        {
            var count = br.ReadInt32();
            ushort[] array = new ushort[count];
            for (int i = 0; i < count; i++) {
                array[i] = br.ReadUInt16();
            }
            return array;
        }
        private byte[] readByteArray(BinaryReader br)
        {
            var count = br.ReadInt32();
            byte[] array = br.ReadBytes(count);
            return array;
        }
        private Dictionary<string, ushort> readDictionary(BinaryReader br)
        {
            Dictionary<string, ushort> dict = new Dictionary<string, ushort>();
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var length = br.ReadInt32();
                byte[] bs = br.ReadBytes(length);
                var str = Encoding.UTF8.GetString(bs);
                var value = br.ReadUInt16();
                dict.Add(str, value);
            }
            return dict;
        }
        private BitArray readBitArray(BinaryReader br)
        {
            var count = br.ReadInt32();
            byte[] array = br.ReadBytes(count);
            return new BitArray(array);
        }



        #endregion

        #region SaveFile
        private void saveFile(string filePath)
        {
            var fs = File.OpenWrite(filePath);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write((int)_type);
            //关键字信息
            write(bw, _ids);
            write(bw, _keywords);
            write(bw, _indexs);
            // 单字映射
            write(bw, _toWord1);
            write(bw, _toWord2);
            write(bw, _toFirstPinYin);
            write(bw, _wordToPinYin);
            write(bw, _pinYinToFirstPinYin);
            write(bw, _pinYinStart);
            write(bw, _firstPinYinStart);

            //节点信息
            write(bw, _word);
            write(bw, _start);
            write(bw, _end);
            write(bw, _isEnd);
            write(bw, _fpyIndexBase);
            write(bw, _fpy);
            write(bw, _pyIndexBase);
            write(bw, _py);
            write(bw, _wordIndexBase);
            write(bw, _wordCheck);


            bw.Close();
            fs.Close();
        }
        private void write(BinaryWriter bw, int[] w)
        {
            if (w == null || w.Length == 0) {
                bw.Write(0);
                return;
            }
            bw.Write(w.Length);
            foreach (var item in w) {
                bw.Write(item);
            }
        }
        private void write(BinaryWriter bw, string[] texts)
        {
            if (texts == null || texts.Length == 0) {
                bw.Write(0);
                return;
            }
            bw.Write(texts.Length);
            foreach (var item in texts) {
                var bs = Encoding.UTF8.GetBytes(item);
                bw.Write(bs.Length);
                bw.Write(bs);
            }
        }
        private void write(BinaryWriter bw, ushort[] w)
        {
            if (w == null || w.Length == 0) {
                bw.Write(0);
                return;
            }
            bw.Write(w.Length);
            foreach (var item in w) {
                bw.Write(item);
            }
        }
        private void write(BinaryWriter bw, byte[] w)
        {
            if (w == null || w.Length == 0) {
                bw.Write(0);
                return;
            }
            bw.Write(w.Length);
            bw.Write(w);
        }
        private void write(BinaryWriter bw, Dictionary<string, ushort> w)
        {
            if (w == null || w.Count == 0) {
                bw.Write(0);
                return;
            }
            bw.Write(w.Count);
            foreach (var item in w) {
                var bs = Encoding.UTF8.GetBytes(item.Key);
                bw.Write(bs.Length);
                bw.Write(bs);
                bw.Write(item.Value);
            }
        }
        private void write(BinaryWriter bw, BitArray array)
        {
            if (array == null || array.Length == 0) {
                bw.Write(0);
                return;
            }
            byte[] a = new byte[(int)Math.Ceiling(array.Length / 8.0)];

            array.CopyTo(a, 0);
            bw.Write(a.Length);
            bw.Write(a);
        }
        #endregion

        #region Search

        private List<int> searchIndexs(string text)
        {
            TextLine rootLine;
            if (trySplitSearchText(text, out rootLine)) {
                List<int> results = new List<int>();
                List<int> baseList = new List<int>() { 0 };
                searchIndexs(rootLine, baseList, results);
                return results.Distinct().ToList();
            }
            return new List<int>();
        }

        private void searchIndexs(TextLine line, List<int> baseList, List<int> results)
        {
            var topLine = line.TopLine;
            if (topLine.Lines.Count == 0) {
                results.AddRange(baseList);
                return;
            }

            foreach (var nextLine in topLine.Lines) {
                if (nextLine.IsError) continue;

                List<int> nextBaseList = new List<int>();
                for (int i = 0; i < baseList.Count; i++) {
                    var index = baseList[i];
                    if (_isEnd[index] == false) {
                        matchFirstPinYin(nextLine, _fpyIndexBase[index], nextBaseList);
                    }
                }
                if (nextBaseList.Count > 0) searchIndexs(nextLine, nextBaseList, results);
            }
        }

        private void matchFirstPinYin(TextLine line, int baseIndex, List<int> nextBaseList)
        {
            var fpyIndex = baseIndex + line.FristPinYin;
            var fpy = _fpy[fpyIndex];
            if (fpy == line.FristPinYin) {
                var pyIndexBase = _pyIndexBase[fpyIndex];
                matchPinYin(line, pyIndexBase, nextBaseList);
            }
        }
        private void matchPinYin(TextLine line, int baseIndex, List<int> nextBaseList)
        {
            if (line.PinYin == (ushort)0) {
                for (int i = _firstPinYinStart[line.FristPinYin]; i < _firstPinYinStart[line.FristPinYin + 1]; i++) {
                    var pyIndex = baseIndex + i;
                    var py = _py[pyIndex];
                    if (py == (ushort)i) {
                        var chineseIndexBase = _wordIndexBase[pyIndex];
                        for (int j = _pinYinStart[i]; j < _pinYinStart[i + 1]; j++) {
                            var chineseIndex = chineseIndexBase + j;
                            var index = _wordCheck[chineseIndex];
                            if (index > 0 && _word[index] == j) {
                                nextBaseList.Add(index);
                            }
                        }
                    }
                }
            } else {
                var pyIndex = baseIndex + line.PinYin;
                var py = _py[pyIndex];
                if (py == line.PinYin) {
                    var chineseIndexBase = _wordIndexBase[pyIndex];
                    matchChinese(line, chineseIndexBase, nextBaseList);
                }
            }
        }
        private void matchChinese(TextLine line, int baseIndex, List<int> nextBaseList)
        {
            if (line.Chinese == (ushort)0) {
                for (int i = _pinYinStart[line.PinYin]; i < _pinYinStart[line.PinYin + 1]; i++) {
                    var chineseIndex = baseIndex + i;
                    var index = _wordCheck[chineseIndex];
                    if (index == 0) continue;
                    if (_word[index] == i) {
                        nextBaseList.Add(index);
                    }
                }
            } else {
                var chineseIndex = baseIndex + line.Chinese;
                var index = _wordCheck[chineseIndex];
                if (index == 0) return;
                if (_word[index] == line.Chinese) {
                    nextBaseList.Add(index);
                }
            }
        }


        #region 搜索字划分、拼音分词
        class MiniSearchResult
        {
            public MiniSearchResult(string keyword, int start, int end)
            {
                Keyword = keyword;
                End = end;
                Start = start;
            }
            public int Start { get; private set; }
            public int End { get; private set; }
            public string Keyword { get; private set; }
        }
        class MiniSearch : internals.BaseSearch
        {
            public List<MiniSearchResult> FindAll(string text)
            {
                internals.TrieNode ptr = null;
                List<MiniSearchResult> list = new List<MiniSearchResult>();

                for (int i = 0; i < text.Length; i++) {
                    internals.TrieNode tn;
                    if (ptr == null) {
                        tn = _first[text[i]];
                    } else {
                        if (ptr.TryGetValue(text[i], out tn) == false) {
                            tn = _first[text[i]];
                        }
                    }
                    if (tn != null) {
                        if (tn.End) {
                            foreach (var item in tn.Results) {
                                list.Add(new MiniSearchResult(item, i + 1 - item.Length, i));
                            }
                        }
                    }
                    ptr = tn;
                }
                return list;
            }

        }

        private static MiniSearch _pinyinSplit;
        private MiniSearch getPinYinSplit()
        {
            if (_pinyinSplit == null) {
                _pinyinSplit = new MiniSearch();
                List<string> pys = new List<string>();
                foreach (var item in PinYinDict.pyName) {
                    var t = item.ToUpper();
                    if (t.Length < 2) continue;
                    pys.Add(t);
                }
                _pinyinSplit.SetKeywords(pys);
            }
            return _pinyinSplit;
        }
        private bool trySplitSearchText(string text, out TextLine root)
        {
            text = text.ToUpper();
            var rs = getPinYinSplit().FindAll(text);

            root = new TextLine();
            List<TextLine> textLines = new List<TextLine>() { root };

            #region 初始化 TextLine

            for (int i = 0; i < text.Length; i++) {
                var c = text[i];
                TextLine tl;
                #region 判断 0-9 A-Z 空格
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c == ' ') {
                    var fpy = _toFirstPinYin[c];
                    if (fpy == 0) {
                        tl = TextLine.Error;
                    } else {
                        tl = new TextLine(fpy, 0, 0);
                    }
                    textLines[i].Add(tl);
                    textLines.Add(tl);
                    continue;
                }
                #endregion

                var w = _toWord1[c];
                if (w == 0) return false;

                #region 多音词
                if (w == ushort.MaxValue) {
                    var pys = PinYinDict.GetAllPinYin(c);
                    for (int j = 0; j < pys.Count; j++) {
                        w = _toWord2[pys[j].ToUpper() + pinYinSpace + c];
                        if (j == 0) {
                            tl = new TextLine(getFirstPinYin(w), getPinYin(w), w);
                            textLines[i].Add(tl);
                            textLines.Add(tl);
                        } else {
                            tl = new TextLine(getFirstPinYin(w), getPinYin(w), w, textLines[i + 1]);
                            textLines[i].Add(tl);
                        }
                    }
                    continue;
                }
                #endregion

                #region 单音词
                tl = new TextLine(getFirstPinYin(w), getPinYin(w), getWord(w));
                textLines[i].Add(tl);
                textLines.Add(tl);
                #endregion
            }
            #endregion

            #region 划分拼音
            foreach (var r in rs) {
                ushort w;
                if (_toWord2.TryGetValue(r.Keyword, out w)) {
                    TextLine tl = new TextLine(_pinYinToFirstPinYin[w], w, 0, textLines[r.End + 1]);
                    textLines[r.Start].Add(tl);
                }
            }
            #endregion

            //root.AutoSetRight();
            root.FilterError();
            return true;
        }
        #endregion

        #endregion

        #region SetKeywords

        private void setKeywords(List<string> keywords)
        {
            initDictionary(keywords);
            var keySort = SplitKeywords(keywords);
            var rootNode = buildNode(keySort, keywords);
            buildToArray(rootNode);
            rootNode.Dispose();
            rootNode = null;
            _keywords = keywords.ToArray();
        }

        #region Node 转成 数组
        private void buildToArray(ChineseNode root)
        {
            buildNodeInfo(root);
            buildCheckArray(root);
        }

        private void buildNodeInfo(ChineseNode root)
        {
            List<ushort> word = new List<ushort>() { 0 };
            List<int> start = new List<int>() { 0 };
            List<int> end = new List<int>() { _indexs.Length };
            List<bool> isEnd = new List<bool>() { false };
            //List<byte> right = new List<byte>() { byte.MaxValue };

            int index = 1;
            buildNodeInfo(root, word, start, end, isEnd, ref index);

            word.Add(0);
            start.Add(0);
            end.Add(0);
            isEnd.Add(true);

            _word = word.ToArray();
            _start = start.ToArray();
            _end = end.ToArray();
            _isEnd = new BitArray(isEnd.ToArray());//.ToArray();
        }
        private void buildNodeInfo(ChineseNode node, List<ushort> word, List<int> start, List<int> end, List<bool> isEnd, ref int index)
        {
            var nodes = node.GetNodes();
            foreach (var n in nodes) {
                word.Add(n.Key);
                isEnd.Add(n.Value.IsEnd);
                //right.Add(n.Value.Right);
                start.Add(n.Value.RangeStart);
                end.Add(n.Value.RangeEnd);
                n.Value.Index = index++;
            }
            foreach (var n in nodes) {
                buildNodeInfo(n.Value, word, start, end, isEnd, ref index);
            }
        }

        private void buildCheckArray(ChineseNode root)
        {
            _fpyIndexBase = new int[_word.Length];

            var fpy = new List<byte>() { 0 };
            var pyIndexBase = new List<int>() { 0 };
            var py = new List<ushort>() { 0 };
            var wordIndexBase = new List<int>() { 0 };
            var wordCheck = new List<int>() { 0 };

            buildCheckArray(root, fpy, pyIndexBase, py, wordIndexBase, wordCheck);

            for (int i = 0; i < _wordToPinYin.Length; i++) {
                fpy.Add(0);
                pyIndexBase.Add(0);
                py.Add(0);
                wordIndexBase.Add(0);
                wordCheck.Add(0);
            }

            _fpy = fpy.ToArray();
            _pyIndexBase = pyIndexBase.ToArray();
            _py = py.ToArray();
            _wordIndexBase = wordIndexBase.ToArray();
            _wordCheck = wordCheck.ToArray();
        }

        private void buildCheckArray(ChineseNode root, List<byte> fpy, List<int> pyIndexBase, List<ushort> py, List<int> wordIndexBase,
            List<int> wordCheck)
        {
            if (root.IsEnd) return;
            #region 初始化pyIndexBase
            #region 初始化 min max start_old start
            byte min = byte.MaxValue;
            byte max = byte.MinValue;
            foreach (var item in root.FirstPinYinNodes) {
                if (item.Key < min) min = item.Key;
                if (item.Key > max) max = item.Key;
            }
            var start_old = pyIndexBase.Count;
            var start = start_old - min;
            #endregion

            #region 检查start的位置，以免冲突
            bool b = true;
            while (b) {
                b = false;
                for (int i = 1; i < min; i++) {
                    if (start + i < 0) continue;
                    if (fpy.Count <= start + i) break;
                    if (fpy[start + i] == (byte)i) {
                        b = true;
                        start++;
                        break;
                    }
                }
            }
            #endregion

            for (int i = start_old; i <= start + max; i++) {
                fpy.Add(0);
                pyIndexBase.Add(0);
            }
            _fpyIndexBase[root.Index] = start;
            #endregion
            foreach (var item in root.FirstPinYinNodes) {
                fpy[start + item.Key] = item.Key;
            }

            var keys = root.FirstPinYinNodes.Keys.OrderByDescending(q => q).ToList();
            foreach (var Key in keys) {
                var Value = root.FirstPinYinNodes[Key];
                buildCheckArray(Value, fpy, pyIndexBase, py, wordIndexBase, wordCheck, start + Key);
            }


        }

        private void buildCheckArray(FirstPinYinNode root, List<byte> fpy, List<int> pyIndexBase, List<ushort> py, List<int> wordIndexBase,
            List<int> wordCheck, int baseIndex)
        {
            #region 初始化wordIndexBase
            #region 初始化 min max start_old start
            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;
            foreach (var item in root.PinYinNodes) {
                if (item.Key < min) min = item.Key;
                if (item.Key > max) max = item.Key;
            }
            var start_old = wordIndexBase.Count;
            var start = start_old - min;
            #endregion

            #region 检验start位置，避免冲突
            bool b = true;
            var left = _pinYinStart[_pinYinToFirstPinYin[min]];
            while (b) {
                b = false;
                for (int i = left; i < min; i++) {
                    if (start + i < 0) continue;
                    if (py.Count <= start + i) break;
                    if (py[start + i] == (ushort)i) {
                        b = true;
                        start++;
                        break;
                    }
                }
            }
            #endregion

            for (int i = start_old; i <= start + max; i++) {
                py.Add(0);
                wordIndexBase.Add(0);
            }
            pyIndexBase[baseIndex] = start;
            #endregion
            foreach (var item in root.PinYinNodes) {
                py[start + item.Key] = item.Key;
            }

            var keys = root.PinYinNodes.Keys.OrderByDescending(q => q).ToList();
            foreach (var Key in keys) {
                var Value = root.PinYinNodes[Key];
                buildCheckArray(Value, fpy, pyIndexBase, py, wordIndexBase, wordCheck, start + Key);
            }

        }

        private void buildCheckArray(PinYinNode root, List<byte> fpy, List<int> pyIndexBase, List<ushort> py, List<int> wordIndexBase,
            List<int> wordCheck, int baseIndex)
        {
            #region 初始化wordCheck
            #region 初始化 min max start_old start
            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;
            foreach (var item in root.ChineseNodes) {
                if (item.Key < min) min = item.Key;
                if (item.Key > max) max = item.Key;
            }
            var start_old = wordCheck.Count;
            var start = start_old - min;
            #endregion

            #region 检验start位置，避免冲突
            bool b = true;
            var left = _pinYinStart[_wordToPinYin[min]];
            while (b) {
                b = false;
                for (int i = left; i < min; i++) {
                    if (start + i < 0) continue;
                    if (wordCheck.Count <= start + i) break;
                    var index = wordCheck[start + i];
                    if (index == 0) continue;
                    if (_word[index] == (ushort)i) {
                        b = true;
                        start++;
                        break;
                    }
                }
            }
            #endregion

            for (int i = start_old; i <= start + max; i++) {
                wordCheck.Add(0);
            }
            wordIndexBase[baseIndex] = start;
            #endregion
            foreach (var item in root.ChineseNodes) {
                wordCheck[start + item.Key] = item.Value.Index;
            }

            var keys = root.ChineseNodes.Keys.OrderByDescending(q => q).ToList();
            foreach (var Key in keys) {
                var Value = root.ChineseNodes[Key];
                buildCheckArray(Value, fpy, pyIndexBase, py, wordIndexBase, wordCheck);
            }

        }



        #endregion

        #region 生成node
        private ChineseNode buildNode(List<string> keySort, List<string> keywords)
        {
            ChineseNode root = new ChineseNode();
            List<int> indexs = new List<int>();

            for (int i = 0; i < keySort.Count; i++) {
                var keys = keySort[i];
                var keySp = keys.Split(wordsSpace);
                //var str = "";

                var node = root;
                for (int j = 0; j < keySp.Length - 1; j++) {
                    //var max = keySp.Length - 1 - j - 1;
                    //if (node.Right < max) node.Right = (byte)(max);
                    var word = getWord(keySp[j]);
                    node = node.AddNode(getFirstPinYin(word), getPinYin(word), word, i);

                    //str += keySp[j][keySp[j].Length - 1];
                }
                indexs.Add(int.Parse(keySp[keySp.Length - 1]));
            }
            _indexs = indexs.ToArray();
            return root;
        }
        private ushort getWord(string text)
        {
            var sp = text.Split(pinYinSpace);
            var result = _toWord1[sp[2][0]];
            if (result == ushort.MaxValue) {
                var key = sp[1] + pinYinSpace + sp[2];
                return _toWord2[key];
            }
            return result;
        }
        List<ushort> _word_firstPinYin;
        private ushort getWord(ushort word)
        {
            if (_word_firstPinYin == null) {
                _word_firstPinYin = new List<ushort>();
                var c = 'A';
                while (c <= 'Z') _word_firstPinYin.Add(_toWord1[c++]);
            }
            if (_word_firstPinYin.Contains(word)) return (ushort)0;
            return word;
        }
        private ushort getPinYin(ushort word)
        {
            if (_word_firstPinYin == null) {
                _word_firstPinYin = new List<ushort>();
                var c = 'A';
                while (c <= 'Z') _word_firstPinYin.Add(_toWord1[c++]);
            }
            if (_word_firstPinYin.Contains(word)) return (ushort)0;
            return _wordToPinYin[((int)word)];
        }
        private byte getFirstPinYin(ushort word)
        {
            return _pinYinToFirstPinYin[_wordToPinYin[word]];
        }
        #endregion

        #region 生成映射字典
        class WordNode
        {
            public List<WordNode> Nodes = new List<WordNode>();
            Dictionary<string, WordNode> dict = new Dictionary<string, WordNode>();
            public string Text;
            public int Count;
            public int Index;
            public int fpyIndex;
            public int pyIndex;
            public int cIndex;

            public void Add(string fpy, string py, string chinese)
            {
                WordNode fpyNode;
                if (dict.TryGetValue(fpy, out fpyNode) == false) {
                    fpyNode = new WordNode();
                    dict.Add(fpy, fpyNode);
                    Nodes.Add(fpyNode);
                    fpyNode.Text = fpy;
                }
                fpyNode.Count++;

                WordNode pyNode;
                if (fpyNode.dict.TryGetValue(py, out pyNode) == false) {
                    pyNode = new WordNode();
                    fpyNode.dict.Add(py, pyNode);
                    fpyNode.Nodes.Add(pyNode);
                    pyNode.Text = py;
                }
                pyNode.Count++;

                WordNode cNode;
                if (pyNode.dict.TryGetValue(chinese, out cNode) == false) {
                    cNode = new WordNode();
                    pyNode.dict.Add(chinese, cNode);
                    pyNode.Nodes.Add(cNode);
                    cNode.Text = chinese;
                }
                cNode.Count++;
            }

            public void SetIndex()
            {
                var index = 1;
                foreach (var node in Nodes) {
                    node.Index = index++;
                }
                fpyIndex = index;

                index = 1;
                foreach (var node1 in Nodes) {
                    foreach (var node2 in node1.Nodes) {
                        node2.Index = index++;
                    }
                }
                pyIndex = index;

                index = 1;
                foreach (var node1 in Nodes) {
                    foreach (var node2 in node1.Nodes) {
                        foreach (var node3 in node2.Nodes) {
                            node3.Index = index++;
                        }
                    }
                }
                cIndex = index;
            }
            public void SetSort2()
            {
                if (Nodes.Count == 0) return;
                foreach (var node in Nodes) {
                    node.SetSort2();
                }
                Nodes = Nodes.OrderBy(q => q.Text).ToList();
            }

            // //以下代码会使体积减少，但会影响输出排列
            //public void SetSort()
            //{
            //    if (Nodes.Count == 0) return;
            //    foreach (var node in Nodes) {
            //        node.SetSort();
            //    }

            //    List<WordNode> nodes = new List<WordNode>();
            //    var ns = Nodes.OrderByDescending(q => q.Count).ToList();
            //    var append = true;
            //    foreach (var node in ns) {
            //        if (append) {
            //            append = false;
            //            nodes.Add(node);
            //        } else {
            //            append = true;
            //            nodes.Insert(0, node);
            //        }
            //    }
            //    Nodes = nodes;
            //}

        }
        private void initDictionary(List<string> keywords)
        {
            List<char> multitone = new List<char>();
            WordNode root = new WordNode();

            #region 分析关键字
            for (int i = 0; i < keywords.Count; i++) {
                var keyword = keywords[i].ToUpper();
                var pylist = PinYinDict.GetPinYinList(keyword);

                for (int j = 0; j < keyword.Length; j++) {
                    var c = keyword[j];
                    if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == ' ') {
                        root.Add(c.ToString(), c.ToString(), c.ToString());
                    } else if (c >= 0x4e00 && c <= 0x9fa5) {
                        var pys = PinYinDict.GetAllPinYin(c);
                        if (pys.Count > 1) multitone.Add(c);
                        foreach (var py in pys) {
                            root.Add(py[0].ToString(), py.ToUpper(), c.ToString());
                        }
                        root.Add(pylist[j][0].ToString(), pylist[j].ToUpper(), c.ToString());
                    } else {
                        root.Add(" ", " ", c.ToString());
                    }
                }
            }
            #endregion

            root.SetSort2();
            root.SetIndex();

            #region 初始化数组长度
            _toWord1 = new ushort[char.MaxValue + 1];
            _toWord2 = new Dictionary<string, ushort>();
            _toFirstPinYin = new byte[128];
            _wordToPinYin = new ushort[root.cIndex + 1];
            _pinYinToFirstPinYin = new byte[root.pyIndex + 1];

            _pinYinStart = new ushort[root.pyIndex + 1];
            _pinYinStart[1] = 1;
            _firstPinYinStart = new ushort[root.fpyIndex + 1];
            _firstPinYinStart[1] = 1;
            #endregion

            #region 初始化数组数据
            foreach (var node1 in root.Nodes) {
                _toFirstPinYin[node1.Text[0]] = (byte)node1.Index; //

                foreach (var node2 in node1.Nodes) {
                    _toWord2[node2.Text] = (ushort)node2.Index;
                    _pinYinToFirstPinYin[node2.Index] = (byte)node1.Index;

                    _firstPinYinStart[node1.Index + 1] = (ushort)(node2.Index + 1);

                    foreach (var node3 in node2.Nodes) {
                        _wordToPinYin[node3.Index] = (ushort)node2.Index;
                        var c = node3.Text[0];
                        if (multitone.Contains(c)) {
                            _toWord1[c] = ushort.MaxValue;
                            _toWord2[node2.Text + pinYinSpace + c] = (ushort)node3.Index;

                        } else {
                            _toWord1[c] = (ushort)node3.Index;
                        }

                        _pinYinStart[node2.Index + 1] = (ushort)(node3.Index + 1);
                    }
                }
            }
            #endregion
        }

        #endregion

        #region 关键字排列组合
        //返回 拼音首字母\t拼音\t原文本\b……
        private List<string> SplitKeywords(List<string> keywords)
        {
            List<string> results = new List<string>();
            for (int i = 0; i < keywords.Count; i++) {
                var keyword = keywords[i];
                splitKeywords(keyword.ToUpper(), i, results);
            }
            return results.OrderBy(q => q).ToList();
        }

        private void splitKeywords(string keyword, int baseIndex, List<string> results)
        {
            List<List<string>> ks = new List<List<string>>();
            if (_type == PinYinSearchType.PinYin) {
                var pys = PinYinDict.GetPinYinList(keyword);
                for (int i = 0; i < keyword.Length; i++) {
                    var list = new List<string>();
                    var t = keyword[i];
                    if ((t >= '0' && t <= '9') || (t >= 'A' && t <= 'Z')) {
                        list.Add(t.ToString());
                    } else if (t >= 0x4e00 && t <= 0x9fa5) {
                        var py = pys[i];
                        if (py == null) py = " ";
                        //if (py == t.ToString()) py = " ";
                        list.Add(py.ToUpper());
                    } else {
                        list.Add(" ");
                    }
                    ks.Add(list);
                }
            } else {
                for (int i = 0; i < keyword.Length; i++) {
                    var list = new List<string>();
                    var t = keyword[i];
                    if ((t >= '0' && t <= '9') || (t >= 'A' && t <= 'Z')) {
                        list.Add(t.ToString());
                    } else if (t >= 0x4e00 && t <= 0x9fa5) {
                        var pys = PinYinDict.GetAllPinYin(t);
                        if (pys.Count == 0) pys.Add(" ");
                        foreach (var item in pys) {
                            list.Add(item.ToUpper());
                        }
                    } else {
                        list.Add(" ");
                    }
                    ks.Add(list);
                }
            }
            foreach (var item in ks[0]) {
                var py = item[0].ToString() + pinYinSpace + item + pinYinSpace + keyword[0];
                splitKeywords(py, 1, ks, keyword, baseIndex, results);
            }
        }
        private void splitKeywords(string py, int index, List<List<string>> ks, string keyword, int baseIndex, List<string> results)
        {
            if (ks.Count == index) {
                results.Add(py + wordsSpace + baseIndex.ToString());
                return;
            }

            var k = ks[index];
            foreach (var item in k) {
                py += wordsSpace + item[0].ToString() + pinYinSpace + item + pinYinSpace + keyword[index].ToString();
                splitKeywords(py, index + 1, ks, keyword, baseIndex, results);
            }
        }
        #endregion

        #endregion
    }
}
