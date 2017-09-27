using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;


namespace ToolGood.Words
{
    /// <summary>
    /// 脏字搜索类
    /// </summary>
    public class IllegalWordsSearch: BaseSearch
    {
        #region class
        class NodeInfo
        {
            public int Index;
            public bool End;
            public NodeInfo Parent;
            public TrieNode Node;
            public char Type;
            public bool TryGetValue(char c, out TrieNode node)
            {
                return Node.TryGetValue(c, out node);
            }
            public bool CanJump(char c, int index, int jump)
            {
                if (Index >= index - jump) {
                    int t;
                    if (c < 127) {
                        t = Type + bitType[c];
                    } else if (c >= 0x4e00 && c <= 0x9fa5) {
                        t = Type + 'z';
                    } else {
                        return true;
                    }
                    if (t == 98 || t == 194 || t == 244) {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            public NodeInfo(int index, char c, TrieNode node, NodeInfo parent = null)
            {
                Index = index;
                Node = node;
                End = node.End;
                Parent = parent;

                if (c < 127) {
                    Type = bitType[c];
                } else if (c >= 0x4e00 && c <= 0x9fa5) {
                    Type = 'z';
                } else {
                    Type = '0';
                }

            }
        }
        class SearchHelper : IDisposable
        {
            NodeInfo mainNode;
            List<NodeInfo> nodes = new List<NodeInfo>();
            private TrieNode[] _first;
            private int _jumpLength;

            public SearchHelper(ref TrieNode[] first, int jumpLength)
            {
                _first = first;
                _jumpLength = jumpLength;
            }

            public void Dispose()
            {
                mainNode = null;
                nodes.Clear();
            }

            public bool FindChar(char c, int index)
            {
                bool hasEnd = false;
                List<NodeInfo> new_node = new List<NodeInfo>();
                #region mainNode
                if (mainNode == null) {
                    TrieNode tn = _first[c];
                    if (tn != null) {
                        mainNode = new NodeInfo(index, c, tn);
                        if (mainNode.End) {
                            hasEnd = true;
                        }
                    }
                } else {
                    if (mainNode.CanJump(c, index, _jumpLength)) {
                        new_node.Add(mainNode);
                    }
                    TrieNode tn;
                    if (mainNode.TryGetValue(c, out tn)) {
                        mainNode = new NodeInfo(index, c, tn, mainNode);
                        if (mainNode.End) {
                            hasEnd = true;
                        }
                    } else {
                        tn = _first[c];
                        if (tn != null) {
                            mainNode = new NodeInfo(index, c, tn);
                            if (mainNode.End) {
                                hasEnd = true;
                            }
                        } else {

                            mainNode = null;
                        }
                    }
                }
                #endregion

                foreach (var n in nodes) {
                    if (n.CanJump(c, index, _jumpLength)) {
                        new_node.Add(n);
                    }
                    TrieNode tn;
                    if (n.TryGetValue(c, out tn)) {
                        var n2 = new NodeInfo(index, c, tn, n);
                        new_node.Add(n2);

                        if (n2.End) {
                            hasEnd = true;
                        }
                    }
                }
                nodes = new_node;
                return hasEnd;
            }

            public List<Tuple<int, int, string>> GetKeywords()
            {
                List<Tuple<int, int, string>> list = new List<Tuple<int, int, string>>();

                if (mainNode != null) {
                    if (mainNode.End) {
                        foreach (var keywords in mainNode.Node.Results) {
                            var p = mainNode;
                            for (int i = 0; i < keywords.Length - 1; i++) {
                                p = p.Parent;
                            }
                            list.Add(Tuple.Create(p.Index, mainNode.Index, keywords));
                        }
                    }
                }

                foreach (var node in nodes) {
                    foreach (var keywords in node.Node.Results) {
                        var p = node;
                        for (int i = 0; i < keywords.Length - 1; i++) {
                            p = p.Parent;
                        }
                        list.Add(Tuple.Create(p.Index, node.Index, keywords));
                    }
                }
                return list;
            }
        }
        #endregion

        #region Local fields
        protected const string bitType = "00000000000000000000000000000000000000000000000011111111110000000aaaaaaaaaaaaaaaaaaaaaaaaaa000000aaaaaaaaaaaaaaaaaaaaaaaaaa00000";
        protected int _jumpLength;
        #endregion

        public IllegalWordsSearch(int jumpLength = 1)
        {
            _jumpLength = jumpLength;
        }


        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public virtual bool ContainsAny(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                char c;
                if (ToSenseWords(text[i], out c)) {
                    sb[i] = c;
                }
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[c];
                } else {
                    if (ptr.TryGetValue(c, out tn) == false) {
                        tn = _first[c];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var find in tn.Results) {
                            var r = GetIllegalResult(find, c, i + 1 - find.Length, i, text, sb);
                            if (r != null) return true;
                        }
                    }
                }
                ptr = tn;
            }

            SearchHelper sh = new SearchHelper(ref _first, _jumpLength);

            for (int i = 0; i < text.Length; i++) {
                char c = sb[i];

                if (sh.FindChar(c, i)) {
                    foreach (var keywordInfos in sh.GetKeywords()) {
                        var r = GetIllegalResult(keywordInfos.Item3, c, keywordInfos.Item1, keywordInfos.Item2, text, sb);
                        if (r != null) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public virtual IllegalWordsSearchResult FindFirst(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                char c;
                if (ToSenseWords(text[i], out c)) {
                    sb[i] = c;
                }
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[c];
                } else {
                    if (ptr.TryGetValue(c, out tn) == false) {
                        tn = _first[c];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var find in tn.Results) {
                            var r = GetIllegalResult(find, c, i + 1 - find.Length, i, text, sb);
                            if (r != null) return r;
                        }
                    }
                }
                ptr = tn;
            }

            SearchHelper sh = new SearchHelper(ref _first, _jumpLength);

            for (int i = 0; i < text.Length; i++) {
                char c;
                if (ToSenseWords(text[i], out c)) {
                    sb[i] = c;
                }
                if (sh.FindChar(c, i)) {
                    foreach (var keywordInfos in sh.GetKeywords()) {
                        var r = GetIllegalResult(keywordInfos.Item3, c, keywordInfos.Item1, keywordInfos.Item2, text, sb);
                        if (r != null) return r;
                    }
                }
            }
            return IllegalWordsSearchResult.Empty;
        }

        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public virtual List<IllegalWordsSearchResult> FindAll(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            SearchHelper sh = new SearchHelper(ref _first, _jumpLength);
            List<IllegalWordsSearchResult> result = new List<IllegalWordsSearchResult>();

            for (int i = 0; i < text.Length; i++) {
                char c;
                if (ToSenseWords(text[i], out c)) {
                    sb[i] = c;
                }
                if (sh.FindChar(c, i)) {
                    foreach (var keywordInfos in sh.GetKeywords()) {
                        var r = GetIllegalResult(keywordInfos.Item3, c, keywordInfos.Item1, keywordInfos.Item2, text, sb);
                        if (r != null) {
                            if (result.Any(q => q.Start == r.Start && q.End == r.End) == false) {
                                result.Add(r);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public virtual string Replace(string text, char replaceChar = '*')
        {
            var all = FindAll(text);
            StringBuilder result = new StringBuilder(text);
            all = all.OrderBy(q => q.Start).ThenBy(q => q.End).ToList();
            for (int i = all.Count - 1; i >= 0; i--) {
                var r = all[i];
                for (int j = r.Start; j <= r.End; j++) {
                    if (result[j] != replaceChar) {
                        result[j] = replaceChar;
                    }
                }
            }
            return result.ToString();
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
                kws.Add(WordsHelper.ToSenseIllegalWords(item));
            }
            base.SetKeywords(kws);
        } 
        #endregion


        #region private
        protected bool isInEnglishOrInNumber(string keyword, char ch, int end, string searchText)
        {
            if (end < searchText.Length - 1) {
                if (ch < 127) {
                    var c = searchText[end + 1];
                    if (c < 127) {
                        int d = bitType[c] + bitType[ch];
                        if (d == 98 || d == 194) {
                            return true;
                        }
                    }
                }
            }
            var start = end + 1 - keyword.Length;
            if (start > 0) {
                var c = searchText[start - 1];
                if (c < 127) {
                    var k = keyword[0];
                    if (k < 127) {
                        int d = bitType[c] + bitType[k];
                        if (d == 98 || d == 194) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private IllegalWordsSearchResult GetIllegalResult(string keyword, char ch, int start, int end, string srcText, StringBuilder searchText)
        {
            if (end < searchText.Length - 1) {
                if (ch < 127) {
                    char c;
                    ToSenseWords(searchText[end + 1], out c);
                    if (c < 127) {
                        int d = bitType[c] + bitType[ch];
                        if (d == 98 || d == 194) {
                            return null;
                        }

                    }
                }
            }
            if (start > 0) {
                var c = searchText[start - 1];
                if (c < 127) {
                    var k = keyword[0];
                    if (k < 127) {
                        int d = bitType[c] + bitType[k];
                        if (d == 98 || d == 194) {
                            return null;
                        }
                    }
                }
            }
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
        }

        private bool ToSenseWords(char c, out char w)
        {
            if (c < 'A') { } else if (c <= 'Z') {
                w = (char)(c | 0x20);
                return true;
            } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                var index = Dict.nums1.IndexOf(c);
                if (index > -1) {
                    w = Dict.nums2[index];
                    return true;
                }
            } else if (c == 12288) {
                w = ' ';
                return true;

            } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                var k = Dict.Simplified[c - 0x4e00];
                if (k != c) {
                    w = k;
                    return true;

                }
            } else if (c < 65280) { } else if (c < 65375) {
                var k = (c - 65248);
                if ('A' <= k && k <= 'Z') {
                    k = k | 0x20;
                }
                w = (char)k;
                return true;
            }
            w = c;
            return false;
        }

        #endregion



    }
}
