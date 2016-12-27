using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    public class IllegalWordsSearchResult
    {
        public IllegalWordsSearchResult(string keyword, int start, int end, string srcText)
        {
            Keyword = keyword;
            Success = true;
            End = end;
            Start = start;
            SrcString = srcText.Substring(Start, end - Start + 1);
        }

        private IllegalWordsSearchResult()
        {
            Success = false;
            Start = 0;
            End = 0;
            SrcString = null;
            Keyword = null;
        }

        public bool Success { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public string SrcString { get; private set; }
        public string Keyword { get; private set; }

        public static IllegalWordsSearchResult Empty { get { return new IllegalWordsSearchResult(); } }

        private int _hash = -1;
        public override int GetHashCode()
        {
            if (_hash == -1) {
                var i = Start << 5;
                i += End - Start;
                _hash = i << 1 + (Success ? 1 : 0);
            }
            return _hash;
        }
        public override string ToString()
        {
            return Start.ToString() + "|" + SrcString;
        }
    }

    public class IllegalWordsSearch
    {
        #region class
        class TreeNode
        {
            #region Constructor & Methods

            public TreeNode(TreeNode parent, char c)
            {
                _char = c; _parent = parent;
                _results = new List<string>();

                _transitionsAr = new List<TreeNode>();
                _transHash = new Dictionary<char, TreeNode>();
            }

            public void AddResult(string result)
            {
                if (_results.Contains(result)) return;
                _results.Add(result);
            }

            public void AddTransition(TreeNode node)
            {
                _transHash.Add(node.Char, node);
                _transitionsAr.Add(node);
            }

            public TreeNode GetTransition(char c)
            {
                TreeNode tn;
                if (_transHash.TryGetValue(c, out tn)) { return tn; }
                return null;
            }

            public bool ContainsTransition(char c)
            {
                return _transHash.ContainsKey(c);
            }
            #endregion

            #region Properties
            private char _char;
            private TreeNode _parent;
            private TreeNode _failure;
            private List<string> _results;
            private List<TreeNode> _transitionsAr;
            private Dictionary<char, TreeNode> _transHash;

            public char Char
            {
                get { return _char; }
            }

            public TreeNode Parent
            {
                get { return _parent; }
            }


            /// <summary>
            /// Failure function - descendant node
            /// </summary>
            public TreeNode Failure
            {
                get { return _failure; }
                set { _failure = value; }
            }


            /// <summary>
            /// Transition function - list of descendant nodes
            /// </summary>
            public List<TreeNode> Transitions
            {
                get { return _transitionsAr; }
            }


            /// <summary>
            /// Returns list of patterns ending by this letter
            /// </summary>
            public List<string> Results
            {
                get { return _results; }
            }

            #endregion
        }
        class TrieNode
        {
            //public byte Type { get; set; }
            public bool End { get; set; }
            public HashSet<string> Results { get; set; }
            private Dictionary<char, TrieNode> m_values;
            private uint minflag = uint.MaxValue;
            private uint maxflag = uint.MinValue;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                Results = new HashSet<string>();
            }

            public bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (uint)c && maxflag >= (uint)c) {
                    return m_values.TryGetValue(c, out node);
                }
                node = null;
                return false;
            }

            public void Add(TreeNode t, TrieNode node)
            {
                var c = t.Char;
                if (m_values.ContainsKey(c) == false) {
                    if (minflag > c) { minflag = c; }
                    if (maxflag < c) { maxflag = c; }
                    m_values.Add(c, node);
                    foreach (var item in t.Results) {
                        node.End = true;
                        //node.Type = GetType(c);
                        node.Results.Add(item);
                    }
                }
            }

            public TrieNode[] ToArray()
            {
                TrieNode[] first = new TrieNode[char.MaxValue + 1];
                foreach (var item in m_values) {
                    first[item.Key] = item.Value;
                }
                return first;
            }
            //public static byte GetType(char c)
            //{
            //    if (c < '0') { } else if (c <= '9') {
            //        return 1;
            //    } else if (c < 'A') { } else if (c <= 'Z') {
            //        return 2;
            //    } else if (c < 'a') { } else if (c <= 'z') {
            //        return 2;
            //    } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
            //        return 3;
            //    }
            //    return 0;
            //}
        }
        #endregion

        #region Local fields
        private TrieNode _root = new TrieNode();
        private TrieNode[] _first = new TrieNode[char.MaxValue + 1];
        private int _jumpLength;
        #endregion

        public IllegalWordsSearch(int jumpLength = 1)
        {
            _jumpLength = jumpLength;
        }

        #region SetKeywords
        public void SetKeywords(ICollection<string> _keywords)
        {
            HashSet<string> list = new HashSet<string>();
            foreach (var item in _keywords) {
                list.Add(WordsHelper.ToSenseWords(item));
                var c = WordsHelper.ToSenseWords(item);
            }
            list.Remove("");
            var tn = BuildTreeWithBFS(list);
            SimplifyTree(tn);
        }
        TreeNode BuildTreeWithBFS(ICollection<string> _keywords)
        {
            var root = new TreeNode(null, ' ');
            foreach (string p in _keywords) {
                string t = p;
                //if (_quick) {
                //    t = p.ToLower();
                //} else {
                //    t = WordHelper.ToSenseWord(p);
                //}

                // add pattern to tree
                TreeNode nd = root;
                foreach (char c in t) {
                    TreeNode ndNew = null;
                    foreach (TreeNode trans in nd.Transitions)
                        if (trans.Char == c) { ndNew = trans; break; }

                    if (ndNew == null) {
                        ndNew = new TreeNode(nd, c);
                        nd.AddTransition(ndNew);
                    }
                    nd = ndNew;
                }
                nd.AddResult(t);
            }

            List<TreeNode> nodes = new List<TreeNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TreeNode nd in root.Transitions) {
                nd.Failure = root;
                foreach (TreeNode trans in nd.Transitions) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TreeNode> newNodes = new List<TreeNode>();

                //ArrayList newNodes = new ArrayList();
                foreach (TreeNode nd in nodes) {
                    TreeNode r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = root;
                    else {
                        nd.Failure = r.GetTransition(c);
                        foreach (string result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            root.Failure = root;
            return root;
        }
        void SimplifyTree(TreeNode tn)
        {
            _root = new TrieNode();
            Dictionary<TreeNode, TrieNode> dict = new Dictionary<TreeNode, TrieNode>();

            List<TreeNode> list = new List<TreeNode>();
            foreach (var item in tn.Transitions) list.Add(item);

            while (list.Count > 0) {
                foreach (var item in list) {
                    simplifyNode(item, tn, dict);
                }
                List<TreeNode> newNodes = new List<TreeNode>();
                foreach (var item in list) {
                    foreach (var node in item.Transitions) {
                        newNodes.Add(node);
                    }
                }
                list = newNodes;
            }
            addNode(tn, tn, _root, dict);
            _first = _root.ToArray();
        }
        void addNode(TreeNode treeNode, TreeNode root, TrieNode tridNode, Dictionary<TreeNode, TrieNode> dict)
        {
            foreach (var item in treeNode.Transitions) {
                var node = dict[item];
                tridNode.Add(item, node);
                addNode(item, root, node, dict);
            }
            if (treeNode != root) {
                var topNode = root.GetTransition(treeNode.Char);
                if (topNode != null) {
                    foreach (var item in topNode.Transitions) {
                        var node = dict[item];
                        tridNode.Add(item, node);
                    }
                }
            }
        }

        void simplifyNode(TreeNode treeNode, TreeNode root, Dictionary<TreeNode, TrieNode> dict)
        {
            List<TreeNode> list = new List<TreeNode>();
            var tn = treeNode;
            while (tn != root) {
                list.Add(tn);
                tn = tn.Failure;
            }

            TrieNode node = new TrieNode();

            foreach (var item in list) {
                if (dict.ContainsKey(item) == false) {
                    if (item.Results.Count > 0) {
                        dict[item] = new TrieNode();
                    } else {
                        dict[item] = node;
                    }
                }
            }
        }
        #endregion

        #region ContainsAny
        public bool ContainsAny(string text)
        {
            bool r = false;
            search(text, (keyword, ch, end) => {
                r = !isInEnglishOrInNumber(keyword, ch, end, text);
                return r;
            });
            if (r) return true;
            var searchText = WordsHelper.ToSenseWords(text);
            search(searchText, (keyword, ch, end) => {
                r = !isInEnglishOrInNumber(keyword, ch, end, searchText);
                return r;
            });
            if (r) return true;
            searchText = WordsHelper.RemoveNontext(searchText);
            search(searchText, (keyword, ch, end) => {
                r = !isInEnglishOrInNumber(keyword, ch, end, searchText);
                return r;
            });
            return r;
        }
        #endregion

        #region FindFirst
        public IllegalWordsSearchResult FindFirst(string text)
        {
            IllegalWordsSearchResult result = null;
            search(text, (keyword, ch, end) => {
                var start = end + 1 - keyword.Length;
                result = GetIllegalResult(keyword, ch, start, end, text, text);
                return result != null;
            });
            if (result != null) return result;
            var searchText = WordsHelper.ToSenseWords(text);
            search(searchText, (keyword, ch, end) => {
                var start = end + 1 - keyword.Length;
                result = GetIllegalResult(keyword, ch, start, end, text, searchText);
                return result != null;
            });
            if (result != null) return result;
            searchText = WordsHelper.RemoveNontext(searchText);
            search(searchText, (keyword, ch, end) => {
                var start = end;
                for (int i = 0; i < keyword.Length; i++) {
                    var n = searchText[start--];
                    while (n == 1) { n = searchText[start--]; }
                }
                start++;
                result = GetIllegalResult(keyword, ch, start, end, text, searchText);
                return result != null;
            });
            if (result != null) return result;
            return IllegalWordsSearchResult.Empty;
        }

        #endregion

        #region FindAll
        public List<IllegalWordsSearchResult> FindAll(string text)
        {
            List<IllegalWordsSearchResult> newlist = new List<IllegalWordsSearchResult>();
            string searchText = WordsHelper.ToSenseWords(text);
            searchAll(searchText, (keyword, ch, end) => {
                var start = end + 1 - keyword.Length;
                var r = GetIllegalResult(keyword, ch, start, end, text, searchText);
                if (r != null) newlist.Add(r);
            });
            searchText = removeChecks(searchText, newlist);
            //list.AddRange(newlist);
            //newlist.Clear();
            searchText = WordsHelper.RemoveNontext(searchText);
            searchAll(searchText, (keyword, ch, end) => {
                var start = end;
                for (int i = 0; i < keyword.Length ; i++) {
                    var n = searchText[start--];
                    while (n == 1) { n = searchText[start--]; }
                }
                start++;
                var r = GetIllegalResult(keyword, ch, start, end, text, searchText);
                if (r != null) newlist.Add(r);
            });
            return newlist;
        }
        #endregion


        #region isInEnglishOrInNumber  search searchAll GetIllegalResult
        const string bitType = "00000000000000000000000000000000000000000000000011111111110000000zzzzzzzzzzzzzzzzzzzzzzzzzz000000zzzzzzzzzzzzzzzzzzzzzzzzzz00000";
        private bool isInEnglishOrInNumber(string keyword, char ch, int end, string searchText)
        {
            if (end < searchText.Length - 1) {
                if (ch < 127) {
                    var c = searchText[end + 1];
                    if (c < 127) {
                        int d = bitType[c] + bitType[ch];
                        if (d == 98 || d == 244) {
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
                        if (d == 98 || d == 244) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private IllegalWordsSearchResult GetIllegalResult(string keyword, char ch, int start, int end, string srcText, string searchText)
        {
            if (end < searchText.Length - 1) {
                if (ch < 127) {
                    var c = searchText[end + 1];
                    if (c < 127) {
                        int d = bitType[c] + bitType[ch];
                        if (d == 98 || d == 244) {
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
                        if (d == 98 || d == 244) {
                            return null;
                        }
                    }
                }
            }
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
        }

        // func :关键字，结尾字符，结尾索引，是否退出搜索
        private void search(string text, Func<string, char, int, bool> func)
        {
            TrieNode ptr = null;
            int jumpCount = 0;
            for (int i = 0; i < text.Length; i++) {
                var ch = text[i];
                if (ch == 1) {
                    jumpCount++;
                    if (jumpCount > _jumpLength) { jumpCount = 0; ptr = null; }
                    continue;
                }
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[ch];
                } else {
                    if (ptr.TryGetValue(ch, out tn) == false) {
                        tn = _first[ch];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            var r = func(item, ch, i);
                            if (r) return;
                        }
                    }
                }
                ptr = tn;
            }
        }
        private void searchAll(string text, Action<string, char, int> action)
        {
            TrieNode ptr = null;
            int jumpCount = 0;
            for (int i = 0; i < text.Length; i++) {
                var ch = text[i];
                if (ch == 1) {
                    jumpCount++;
                    if (jumpCount > _jumpLength) { jumpCount = 0; ptr = null; }
                    continue;
                }
                jumpCount = 0;
                if (ch == 0) { ptr = null; continue; }


                TrieNode tn;
                if (ptr == null) {
                    tn = _first[ch];
                } else {
                    if (ptr.TryGetValue(ch, out tn) == false) {
                        tn = _first[ch];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            action(item, ch, i);
                        }
                    }
                }
                ptr = tn;
            }
        }

        // 删除 已符合数据
        private string removeChecks(string text, List<IllegalWordsSearchResult> results)
        {
            StringBuilder sb = new StringBuilder(text);
            foreach (var r in results) {
                for (int i = r.Start; i <= r.End; i++) {
                    sb[i] = (char)0;
                }
            }
            return sb.ToString();
        }

        #endregion


    }
}
