using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.Words
{
    public class SearchResult
    {
        public SearchResult(string keyword, int end)
        {
            Success = true;
            Keyword = keyword;
            SearchString = keyword;
            End = end;
            Start = End - Keyword.Length + 1;
        }

        private SearchResult()
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
        public static SearchResult Empty { get { return new SearchResult(); } }

    }
    /// <summary>
    /// 完整字符搜索
    /// </summary>
    public class SearchText
    {
        #region Local fields

        /// <summary>
        /// Root of keyword tree
        /// </summary>
        private TreeNode _root;

        /// <summary>
        /// Keywords to search for
        /// </summary>
        private string[] _keywords;

        #endregion

        public SearchText(List<string> keywords)
        {
            Keywords = keywords.ToArray();
        }

        public SearchText(string[] keywords)
        {
            Keywords = keywords;
        }


        public SearchText() { }

        void BuildTree()
        {
            _root = new TreeNode(null, ' ');
            foreach (string p in _keywords) {
                // add pattern to tree
                TreeNode nd = _root;
                foreach (char c in p) {
                    TreeNode ndNew = null;
                    foreach (TreeNode trans in nd.Transitions)
                        if (trans.Char == c) { ndNew = trans; break; }

                    if (ndNew == null) {
                        ndNew = new TreeNode(nd, c);
                        nd.AddTransition(ndNew);
                    }
                    nd = ndNew;
                }
                nd.AddResult(p);
            }

            List<TreeNode> nodes = new List<TreeNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TreeNode nd in _root.Transitions) {
                nd.Failure = _root;
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
                        nd.Failure = _root;
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

        public List<SearchResult> FindAll(string text)
        {
            List<SearchResult> ret = new List<SearchResult>();
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode trans = null;
                while (trans == null) {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    ret.Add(new SearchResult(found, index));
                index++;
            }
            return ret;
        }

        public SearchResult FindFirst(string text)
        {
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode trans = null;
                while (trans == null) {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root) break;
                    if (trans == null) ptr = ptr.Failure;
                }
                if (trans != null) ptr = trans;

                foreach (string found in ptr.Results)
                    return new SearchResult(found, index);
                index++;
            }
            return SearchResult.Empty;
        }


        public bool ContainsAny(string text)
        {
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length) {
                TreeNode trans = null;
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
