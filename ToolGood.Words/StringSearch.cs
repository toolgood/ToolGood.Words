using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;


namespace ToolGood.Words
{
    public class StringSearch
    {
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

        public string Replace(string text,char replaceChar='*')
        {
            StringBuilder result = new StringBuilder(text);

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
                       var length= tn.Results.Max(q => q.Length);
                        var start = i + 1 - length;
                        for (int j = start; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                ptr = tn;
            }
            return result.ToString();
        }

    }
}
