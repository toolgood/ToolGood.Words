using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    public class StringSearch
    {
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
        }

        private TrieNode _root = new TrieNode();
        private TrieNode[] _first = new TrieNode[char.MaxValue + 1];

        #region SetKeywords
        public void SetKeywords(ICollection<string> _keywords)
        {
            var tn = BuildTreeWithBFS(_keywords);
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
            if (treeNode!= root) {
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

        public string FindFirst(string text)
        {
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
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
                            return item;
                        }
                    }
                }
                ptr = tn;
            }
            return null;
        }

        public List<string> FindAll(string text)
        {
            TrieNode ptr = null;
            List<string> list = new List<string>();

            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
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
                            list.Add(item);
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }

        public bool ContainsAny(string text)
        {
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        return true;
                    }
                }
                ptr = tn;
            }
            return false;
        }

    }
}
