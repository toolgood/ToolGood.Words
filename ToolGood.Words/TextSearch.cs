using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.Words
{
    public class TextSearchResult
    {
        public TextSearchResult(string keyword, int end)
        {
            Success = true;
            Keyword = keyword;
            SearchString = keyword;
            End = end;
            Start = End - Keyword.Length + 1;
        }

        protected TextSearchResult()
        {
            Success = false;
            Start = -1;
            End = -1;
            SearchString = null;
            Keyword = null;
        }

        public bool Success { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public string SearchString { get; private set; }
        public string Keyword { get; private set; }
        public static TextSearchResult Empty { get { return new TextSearchResult(); } }

        public override string ToString()
        {
            return Keyword;
        }

    }
    /// <summary>
    /// 完整字符搜索
    /// </summary>
    public class TextSearch
    {
        #region Local fields

        /// <summary>
        /// Root of keyword tree
        /// </summary>
        private TreeNode<string> _root;

        /// <summary>
        /// Keywords to search for
        /// </summary>
        private string[] _keywords;

        #endregion

        public TextSearch(List<string> keywords)
        {
            Keywords = keywords.ToArray();
        }

        public TextSearch(string[] keywords)
        {
            Keywords = keywords;
        }


        public TextSearch() { }

        void BuildTree()
        {
            _root = new TreeNode<string>(null, ' ');
            foreach (string p in _keywords) {
                // add pattern to tree
                TreeNode<string> nd = _root;
                foreach (char c in p) {
                    TreeNode<string> ndNew = null;
                    foreach (TreeNode<string> trans in nd.Transitions)
                        if (trans.Char == c) { ndNew = trans; break; }

                    if (ndNew == null) {
                        ndNew = new TreeNode<string>(nd, c);
                        nd.AddTransition(ndNew);
                    }
                    nd = ndNew;
                }
                nd.AddResult(p);
            }

            List<TreeNode<string>> nodes = new List<TreeNode<string>>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TreeNode<string> nd in _root.Transitions) {
                nd.Failure = _root;
                foreach (TreeNode<string> trans in nd.Transitions) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TreeNode<string>> newNodes = new List<TreeNode<string>>();

                //ArrayList newNodes = new ArrayList();
                foreach (TreeNode<string> nd in nodes) {
                    TreeNode<string> r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = _root;
                    else {
                        nd.Failure = r.GetTransition(c);
                        foreach (string result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode<string> child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            _root.Failure = _root;
        }


        public string[] Keywords
        {
            get { return _keywords; }
            set
            {
                _keywords = value;
                BuildTree();
            }
        }

        public List<TextSearchResult> FindAll(string text)
        {
            List<TextSearchResult> ret = new List<TextSearchResult>();
            TreeNode<string> ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode<string> trans = null;
                while (trans == null) {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    ret.Add(new TextSearchResult(found, index));
                index++;
            }
            return ret;
        }

        public TextSearchResult FindFirst(string text)
        {
            TreeNode<string> ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode<string> trans = null;
                while (trans == null) {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    return new TextSearchResult(found, index);
                index++;
            }
            return TextSearchResult.Empty;
        }


        public bool ContainsAny(string text)
        {
            TreeNode<string> ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode<string> trans = null;
                while (trans == null) {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                if (ptr.Results.Count > 0) return true;
                index++;
            }
            return false;
        }

    }
}
