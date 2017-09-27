using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    /// <summary>
    /// 脏字搜索类，支持重复词、跳词
    /// </summary>
    public class IllegalWordsSearchEx
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
            public TreeNode GetTransition(string text, int index)
            {
                if (index == -1) { return this; }

                var c = text[index];
                TreeNode tn;
                if (_transHash.TryGetValue(c, out tn)) {
                    return tn.GetTransition(text, index - 1);
                }
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
            public bool End { get; set; }
            public List<string> Results { get; set; }
            private Dictionary<char, TrieNode> m_values;
            private uint minflag = uint.MaxValue;
            private uint maxflag = uint.MinValue;

            /// <summary>
            /// 是否为重复节点
            /// </summary>
            public bool IsRepeat { get; set; }
            /// <summary>
            /// 上一级
            /// </summary>
            public TrieNode Parent { get; set; }
            /// <summary>
            /// 最后匹配位置
            /// </summary>
            public int LastMatchLocation { get; set; }

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                Results = new List<string>();
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
                        if (node.Results.Contains(item) == false) {
                            node.Results.Add(item);
                        }
                    }
                }
            }

            public TrieNode GetParent(int pNum)
            {
                var tn = this;
                for (int i = 0; i < pNum; i++) {
                    tn = tn.Parent;
                }
                return tn;
            }

            public TrieNode[] ToArray()
            {
                TrieNode[] first = new TrieNode[char.MaxValue + 1];
                foreach (var item in m_values) {
                    first[item.Key] = item.Value;
                }
                return first;
            }
        }
        #endregion

        #region 私有变量
        private TrieNode _root = new TrieNode();
        private TrieNode[] _first = new TrieNode[char.MaxValue + 1];
        private const string _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
        private BitArray _skipBitArray;
        #endregion

        #region 构造函数
        public IllegalWordsSearchEx(string skipList = null)
        {
            _skipBitArray = new BitArray(char.MaxValue + 1);
            if (skipList == null) {
                for (int i = 0; i < _skipList.Length; i++) {
                    _skipBitArray[_skipList[i]] = true;
                }
            } else {
                for (int i = 0; i < skipList.Length; i++) {
                    _skipBitArray[skipList[i]] = true;
                }
            }
        }

        #endregion

        #region 设置跳词
        /// <summary>
        /// 设置跳词
        /// </summary>
        /// <param name="skipList"></param>
        public void SetSkipWords(string skipList)
        {
            _skipBitArray = new BitArray(char.MaxValue + 1);
            if (skipList == null) {
                for (int i = 0; i < _skipList.Length; i++) {
                    _skipBitArray[_skipList[i]] = true;
                }
            }
        } 
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(ICollection<string> keywords)
        {
            var tn = BuildTreeWithBFS(keywords);
            SimplifyTree(tn);
        }
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        TreeNode BuildTreeWithBFS(ICollection<string> _keywords)
        {
            var root = new TreeNode(null, ' ');
            foreach (string p in _keywords) {
                string t = p;

                // add pattern to tree
                TreeNode nd = root;
                foreach (char cc in t) {
                    char c = ToSenseWord(cc);

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
                    dict[item] = new TrieNode();
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
                string str = "";
                List<char> rootChar = new List<char>();
                var node = treeNode;
                if (node.Char == '锦') {

                }
                while (node.Parent != root) {
                    str += node.Char;
                    var topNode = root.GetTransition(str, str.Length - 1);
                    if (topNode != null) {
                        foreach (var item in topNode.Transitions) {
                            tridNode.Add(item, dict[item]);
                        }
                    }
                    node = node.Parent;
                }
            }
        }
        #endregion

        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public virtual bool ContainsAny(string text)
        {
            TrieNode ptr = null;
            char pChar = (char)0;
            for (int i = 0; i < text.Length; i++) {
                char c = ToSenseWord(text[i]);
                if (_skipBitArray[c]) continue;
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[c];
                } else if (ptr.TryGetValue(c, out tn) == false) {
                    tn = (c != pChar) ? _first[c] : ptr;
                }

                if (tn != null && tn.End) {
                    return true;
                }
                ptr = tn;
                pChar = c;
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
            TrieNode ptr = null;
            char pChar = (char)0;
            for (int i = 0; i < text.Length; i++) {
                char c = ToSenseWord(text[i]);
                if (_skipBitArray[c]) continue;
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[c];
                } else if (ptr.TryGetValue(c, out tn) == false) {
                    tn = (c != pChar) ? _first[c] : ptr;
                }
                if (tn != null) {
                    if (ptr != tn) {
                        tn.LastMatchLocation = i;
                        tn.Parent = ptr;
                    }
                    if (tn.End) {
                        foreach (var find in tn.Results) {
                            var p = tn.GetParent(find.Length - 1);
                            return new IllegalWordsSearchResult(find, p.LastMatchLocation, i, text);
                        }
                    }
                }
                ptr = tn;
                pChar = c;
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
            List<IllegalWordsSearchResult> result = new List<IllegalWordsSearchResult>();

            TrieNode ptr = null;
            char pChar = (char)0;

            for (int i = 0; i < text.Length; i++) {
                char c = ToSenseWord(text[i]);
                if (_skipBitArray[c]) continue;
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[c];
                } else if (ptr.TryGetValue(c, out tn) == false) {
                    tn = (c != pChar) ? _first[c] : ptr;
                }
                if (tn != null) {
                    if (ptr != tn) {
                        tn.LastMatchLocation = i;
                        tn.Parent = ptr;
                    }
                    if (tn.End) {
                        foreach (var find in tn.Results) {
                            var p = tn.GetParent(find.Length - 1);
                            var r = new IllegalWordsSearchResult(find, p.LastMatchLocation, i, text);
                            result.Add(r);
                        }
                    }
                }
                ptr = tn;
                pChar = c;
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
            StringBuilder sb = new StringBuilder(text);
            List<IllegalWordsSearchResult> result = new List<IllegalWordsSearchResult>();

            TrieNode ptr = null;
            char pChar = (char)0;

            for (int i = 0; i < text.Length; i++) {
                char c = ToSenseWord(text[i]);

                if (_skipBitArray[c]) continue;
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[c];
                } else if (ptr.TryGetValue(c, out tn) == false) {
                    tn = (c != pChar) ? _first[c] : ptr;
                }
                if (tn != null) {
                    if (ptr != tn) {
                        tn.LastMatchLocation = i;
                        tn.Parent = ptr;
                    }
                    if (tn.End) {
                        foreach (var find in tn.Results) {
                            var p = tn.GetParent(find.Length - 1);
                            for (int j = p.LastMatchLocation; j <= i; j++) {
                                sb[j] = replaceChar;
                            }
                        }
                    }
                }
                ptr = tn;
                pChar = c;
            }
            return sb.ToString();
        }

        #endregion


        #region 私有方法

        private char ToSenseWord(char c)
        {
            if (c < 'A') { } else if (c <= 'Z') {
                return (char)(c | 0x20);
            } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                var index = Dict.nums1.IndexOf(c);
                if (index > -1) {
                    return Dict.nums2[index];
                }
            } else if (c == 12288) {
                return ' ';

            } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                var k = Dict.Simplified[c - 0x4e00];
                if (k != c) {
                    return k;
                }
            } else if (c < 65280) { } else if (c < 65375) {
                var k = (c - 65248);
                if ('A' <= k && k <= 'Z') {
                    k = k | 0x20;
                }
                return (char)k;
            }
            return c;
        }

        #endregion
    }
}
