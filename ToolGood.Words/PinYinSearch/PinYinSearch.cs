using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    public class PinYinSearch
    {
        const char wordsSpace = '\b';// (char)1;
        const char pinYinSpace = '\t';// (char)2;

        #region class
        class FirstPinYinNode
        {
            Dictionary<string, PinYinNode> PinYinNodes = new Dictionary<string, PinYinNode>();

            public ChineseNode AddNode(string pinYin, char chinese, int index)
            {
                PinYinNode node;
                if (PinYinNodes.TryGetValue(pinYin, out node) == false) {
                    node = new PinYinNode();
                    PinYinNodes.Add(pinYin, node);
                }
                return node.AddNode(chinese, index);
            }
            public bool MatchingNode(char firstPinYin, string pinYin, char chinese, List<ChineseNode> nodes)
            {
                if (string.IsNullOrEmpty(pinYin)) {
                    foreach (var item in PinYinNodes) {
                        item.Value.ToAddNodes(nodes);
                    }
                } else if (firstPinYin.ToString() == pinYin && chinese == (char)0) {
                    foreach (var item in PinYinNodes) {
                        item.Value.ToAddNodes(nodes);
                    }
                    return true;
                } else {
                    PinYinNode node;
                    if (PinYinNodes.TryGetValue(pinYin, out node) == false) return false;
                    node.MatchingNode(firstPinYin, pinYin, chinese, nodes);
                }
                return true;
            }

        }
        class PinYinNode
        {
            Dictionary<char, ChineseNode> ChineseNodes = new Dictionary<char, ChineseNode>();
            public void ToAddNodes(List<ChineseNode> nodes)
            {
                foreach (var item in ChineseNodes) {
                    nodes.Add(item.Value);
                }
            }

            public ChineseNode AddNode(char chinese, int index)
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
            public bool MatchingNode(char firstPinYin, string pinYin, char chinese, List<ChineseNode> nodes)
            {
                if (chinese == (char)0) {
                    foreach (var item in ChineseNodes) {
                        nodes.Add(item.Value);
                    }
                } else {
                    ChineseNode node;
                    if (ChineseNodes.TryGetValue(chinese, out node) == false) return false;
                    nodes.Add(node);
                }
                return true;
            }
        }
        class ChineseNode
        {
            Dictionary<char, FirstPinYinNode> FirstPinYinNodes = new Dictionary<char, FirstPinYinNode>();
            public int RangeStart;
            public int RangeEnd;

            public ChineseNode AddNode(char firstPinYin, string pinYin, char chinese, int index)
            {
                RangeEnd = index;
                FirstPinYinNode node;
                if (FirstPinYinNodes.TryGetValue(firstPinYin, out node) == false) {
                    node = new FirstPinYinNode();
                    FirstPinYinNodes.Add(firstPinYin, node);
                }
                return node.AddNode(pinYin, chinese, index);
            }

            public bool MatchingNode(char firstPinYin, string pinYin, char chinese, List<ChineseNode> nodes)
            {
                FirstPinYinNode node;
                if (FirstPinYinNodes.TryGetValue(firstPinYin, out node) == false) return false;
                node.MatchingNode(firstPinYin, pinYin, chinese, nodes);
                return true;
            }
        }
        class TextLine
        {
            public List<TextLine> Lines = new List<TextLine>();
            public char FristPinYin;
            public string PinYin;
            public char Chinese;
            public TextLine TopLine;

            public TextLine() { TopLine = this; }
            public TextLine(char fristPinYin, string pinYin, char chinese)
            {
                FristPinYin = fristPinYin;
                PinYin = pinYin;
                Chinese = chinese;
                TopLine = this;
            }
            public TextLine(char fristPinYin, string pinYin, char chinese, TextLine line)
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

        }
        #endregion

        #region 私有字段
        //搜索类型
        PinYinSearchType _type;
        //bool _startMiddle;
        //关键字信息
        private int[] _ids;//输入的关键字ID
        private string[] _keywords;//输入的关键字 
        private int[] _indexs;//排序后关键字对应的索引，TrieRange类的RangeStart，RangeEnd指向这个索引

        private ChineseNode _root;
        #endregion

        #region 构造方法
        public PinYinSearch(PinYinSearchType type = PinYinSearchType.PinYin)
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
            var keySorts = SplitKeywords(keywords);
            buildKeywords(keySorts, keywords);
        }
        /// <summary>
        /// 设置关键字和IDs
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <param name="ids">IDs</param>
        public void SetKeywords(List<string> keywords, List<int> ids)
        {
            if (keywords.Count != ids.Count) throw new ArgumentException("keywords and ids inconsistent number.");
            var keySorts = SplitKeywords(keywords);
            buildKeywords(keySorts, keywords);
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
            TextLine line;
            trySplitSearchText(text, out line);
            List<ChineseNode> nodes = matching(line);

            List<string> results = new List<string>();
            if (keywordSort) {
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        results.Add(_keywords[_indexs[i]]);
                    }
                }
            } else {
                List<int> indexs = new List<int>();
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        indexs.Add(_indexs[i]);
                    }
                }
                indexs = indexs.OrderBy(q => q).Distinct().ToList();
                foreach (var index in indexs) {
                    results.Add(_keywords[index]);
                }
            }
            return results;
        }
        /// <summary>
        /// 搜索IDs
        /// </summary>
        /// <param name="text"文本></param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <returns></returns>
        public List<int> SearchIds(string text, bool keywordSort = false)
        {
            TextLine line;
            trySplitSearchText(text, out line);
            List<ChineseNode> nodes = matching(line);

            List<int> results = new List<int>();
            if (keywordSort) {
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        results.Add(_ids[_indexs[i]]);
                    }
                }
            } else {
                List<int> indexs = new List<int>();
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        indexs.Add(_indexs[i]);
                    }
                }
                indexs = indexs.OrderBy(q => q).Distinct().ToList();
                foreach (var index in indexs) {
                    results.Add(_ids[index]);
                }
            }
            return results;
        }
        /// <summary>
        /// 搜索关键字和IDs
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="keywordSort">是否按拼音排序</param>
        /// <returns></returns>
        public List<PinYinSearchResult> SearchTextWithIds(string text, bool keywordSort = false)
        {
            TextLine line;
            trySplitSearchText(text, out line);
            List<ChineseNode> nodes = matching(line);

            List<PinYinSearchResult> results = new List<PinYinSearchResult>();
            if (keywordSort) {
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        var index = _indexs[i];
                        results.Add(new PinYinSearchResult(_keywords[index], _ids[index]));
                    }
                }
            } else {
                List<int> indexs = new List<int>();
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        indexs.Add(_indexs[i]);
                    }
                }
                indexs = indexs.OrderBy(q => q).Distinct().ToList();
                foreach (var index in indexs) {
                    results.Add(new PinYinSearchResult(_keywords[index], _ids[index]));
                }
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
            TextLine line;
            trySplitSearchText(text, out line);
            List<ChineseNode> nodes = matching(line);

            List<string> results = new List<string>();
            List<string> appends = new List<string>();
            var count = text.Length + pickLength;
            if (keywordSort) {
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        var key = _keywords[i];
                        if (key.Length <= count) {
                            results.Add(key);
                        } else {
                            appends.Add(key);
                        }
                    }
                }
            } else {
                List<int> indexs = new List<int>();
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        indexs.Add(_indexs[i]);
                    }
                }
                indexs = indexs.OrderBy(q => q).Distinct().ToList();
                foreach (var index in indexs) {
                    var key = _keywords[index];
                    if (key.Length <= count) {
                        results.Add(key);
                    } else {
                        appends.Add(key);
                    }
                }
            }
            results.AddRange(appends);
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
            TextLine line;
            trySplitSearchText(text, out line);
            List<ChineseNode> nodes = matching(line);

            List<PinYinSearchResult> results = new List<PinYinSearchResult>();
            List<PinYinSearchResult> appends = new List<PinYinSearchResult>();
            var count = text.Length + pickLength;
            if (keywordSort) {
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        var index = _indexs[i];
                        var key = _keywords[index];
                        if (key.Length <= count) {
                            results.Add(new PinYinSearchResult(key, _ids[index]));
                        } else {
                            appends.Add(new PinYinSearchResult(key, _ids[index]));
                        }
                    }
                }
            } else {
                List<int> indexs = new List<int>();
                foreach (var item in nodes) {
                    for (int i = item.RangeStart; i <= item.RangeEnd; i++) {
                        indexs.Add(_indexs[i]);
                    }
                }
                indexs = indexs.OrderBy(q => q).Distinct().ToList();
                foreach (var index in indexs) {
                    var key = _keywords[index];
                    if (key.Length <= count) {
                        results.Add(new PinYinSearchResult(key, _ids[index]));
                    } else {
                        appends.Add(new PinYinSearchResult(key, _ids[index]));
                    }
                }
            }
            results.AddRange(appends);
            return results;
        }


        #endregion

        #region Search
        private List<ChineseNode> matching(TextLine line)
        {
            List<ChineseNode> results = new List<ChineseNode>();
            matching(line, new List<ChineseNode>() { _root }, results);
            return results;
        }
        private void matching(TextLine line, List<ChineseNode> nodes, List<ChineseNode> results)
        {
            var topLine = line.TopLine;
            if (topLine.Lines.Count == 0) {
                foreach (var item in nodes) results.Add(item);
                return;
            }

            foreach (var nextLine in topLine.Lines) {
                List<ChineseNode> newNodes = new List<ChineseNode>();
                foreach (var node in nodes) {
                    node.MatchingNode(nextLine.FristPinYin, nextLine.PinYin, nextLine.Chinese, newNodes);
                }
                if (nodes.Count > 0) matching(nextLine, newNodes, results);
            }
        }
        #endregion

        #region 搜索字划分、拼音分词
        private static WordsSearch _pinyinSplit;
        private WordsSearch getPinYinSplit()
        {
            if (_pinyinSplit == null) {
                _pinyinSplit = new WordsSearch();
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
                if (_type == PinYinSearchType.PinYin && c >= 0x4e00 && c <= 0x9fa5) {
                    var pys = PinYinDict.GetAllPinYin(c);
                    if (pys.Count == 0) pys.Add(" ");
                    for (int j = 0; j < pys.Count; j++) {
                        var py = pys[j].ToUpper();
                        TextLine tl;// = new TextLine();
                        if (j == 0) {
                            tl = new TextLine(py[0], py, c);
                            textLines.Add(tl);
                        } else {
                            tl = new TextLine(py[0], py, c, textLines[i + 1]);
                        }
                        textLines[i].Add(tl);
                    }
                } else {
                    TextLine tl;
                    if (c >= 0x4e00 && c <= 0x9fa5) {
                        var py = PinYinDict.GetPinYinFast(c);
                        tl = new TextLine(py[0], py, c);
                    } else if (c >= '0' || c <= '9') {
                        tl = new TextLine(c, null, (char)0);
                    } else if (c >= 'A' || c <= 'Z') {
                        tl = new TextLine(c, null, (char)0);
                    } else {
                        tl = new TextLine(' ', " ", c);
                    }
                    textLines[i].Add(tl);
                    textLines.Add(tl);
                }
            }
            #endregion

            #region 划分拼音
            foreach (var r in rs) {
                var tl2 = new TextLine(r.Keyword[0], r.Keyword, (char)0, textLines[r.End + 1]);
                textLines[r.Start].Add(tl2);
            }
            #endregion

            return true;
        }
        #endregion

        #region 组合关键字
        private void buildKeywords(List<string> keySorts, List<string> keywords)
        {
            _root = new ChineseNode();
            List<int> indexs = new List<int>();

            for (int i = 0; i < keySorts.Count; i++) {
                var sp = keySorts[i].Split(wordsSpace);
                //var str = "";
                var range = _root;
                for (int j = 0; j < sp.Length-1; j++) {
                    var s = sp[j].Split(pinYinSpace);
                    range = range.AddNode(s[0][0], s[1], s[2][0], i);
                    //str += s[2];
                }
                indexs.Add(int.Parse(sp[sp.Length - 1]));
            }
            _keywords = keywords.ToArray();
            _indexs = indexs.ToArray();
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

    }
}
