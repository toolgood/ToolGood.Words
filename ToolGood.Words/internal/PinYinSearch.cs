using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    class PinYinResult : TextSearchResult
    {
        public PinYinResult(string keyword, int end, int index) : base(keyword, end)
        {
            Index = index;
        }
        public int Index { get; set; }
    }

    class PinYinSearch
    {
        private TreeNode<Tuple<string,int>> _root;

        public PinYinSearch() { }
        public void BuildTree(string[] _keywords)
        {
            _root = new TreeNode<Tuple<string, int>>(null, ' ');

            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                // add pattern to tree
                TreeNode<Tuple<string, int>> nd = _root;
                foreach (char c in p) {
                    TreeNode<Tuple<string, int>> ndNew = null;
                    foreach (TreeNode<Tuple<string, int>> trans in nd.Transitions)
                        if (trans.Char == c) { ndNew = trans; break; }

                    if (ndNew == null) {
                        ndNew = new TreeNode<Tuple<string, int>>(nd, c);
                        nd.AddTransition(ndNew);
                    }
                    nd = ndNew;
                }
                nd.AddResult(Tuple.Create(p, i));
            }

            List<TreeNode<Tuple<string, int>>> nodes = new List<TreeNode<Tuple<string, int>>>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TreeNode<Tuple<string, int>> nd in _root.Transitions) {
                nd.Failure = _root;
                foreach (TreeNode<Tuple<string, int>> trans in nd.Transitions) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TreeNode<Tuple<string, int>>> newNodes = new List<TreeNode<Tuple<string, int>>>();

                //ArrayList newNodes = new ArrayList();
                foreach (TreeNode<Tuple<string, int>> nd in nodes) {
                    TreeNode<Tuple<string, int>> r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = _root;
                    else {
                        nd.Failure = r.GetTransition(c);
                        foreach (Tuple<string, int> result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode<Tuple<string, int>> child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            _root.Failure = _root;
        }

        public List<PinYinResult> FindAll(string text)
        {
            List<PinYinResult> ret = new List<PinYinResult>();
            TreeNode<Tuple<string, int>> ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode<Tuple<string, int>> trans = null;
                while (trans == null) {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (Tuple<string, int> found in ptr.Results)
                    ret.Add(new PinYinResult(found.Item1, index,found.Item2));
                index++;
            }
            return ret;
        }


    }
}
