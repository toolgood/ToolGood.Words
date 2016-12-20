using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.Words
{
    public class IllegalResult
    {
        private IllegalResult(string keyword, int end, string searchText, string srcText)
        {
            Keyword = keyword;
            Success = true;
            End = end;
            Start = end;
            List<bool> right = new List<bool>();
            for (int i = keyword.Length - 1; i >= 0; i--) {
                var c = keyword[i];
                var c2 = searchText[Start--];
                while (c != c2) {
                    right.Add(false);
                    c2 = searchText[Start--];
                }
                right.Add(true);
            }
            Start++;
            SearchString = srcText.Substring(Start, end - Start + 1);
            searchString = searchText.Substring(Start, end - Start + 1);
            rightArray = right.Reverse<bool>().ToArray();
        }

        private IllegalResult()
        {
            Success = false;
            Start = 0;
            End = 0;
            SearchString = null;
            Keyword = null;
        }

        public bool Success { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public string SearchString { get; private set; }
        public string Keyword { get; private set; }
        internal string searchString { get; private set; }
        internal bool[] rightArray { get; private set; }

        public static IllegalResult Empty { get { return new IllegalResult(); } }

        private static bool IsSameType(char k, char c)
        {
            if ((k >= 'a' && k <= 'z') || (k >= '0' && k <= '9')) {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) {
                    return true;
                }
            } else if (k >= 0x4e00 && k <= 0x9fa5) {
                if (c >= 0x4e00 && c <= 0x9fa5) {
                    return true;
                }
            }
            return false;
        }
        public static IllegalResult GetIllegalResult(string keyword, int end, string searchText, string srcText)
        {
            var result = new IllegalResult(keyword, end, searchText, srcText);
            if (result.SearchString.Length == keyword.Length) {
                //判断关键字是否在英文内
                if (result.Start > 0) {
                    var c = searchText[result.Start - 1];
                    var k = keyword[0];
                    if (k >= 'a' && k <= 'z' && c >= 'a' && c <= 'z') { return null; }
                    if (k >= '0' && k <= '9' && c >= '0' && c <= '9') { return null; }
                }
                if (result.End < searchText.Length - 1) {
                    var c = searchText[result.End + 1];
                    var k = keyword[keyword.Length - 1];
                    if (k >= 'a' && k <= 'z' && c >= 'a' && c <= 'z') { return null; }
                    if (k >= '0' && k <= '9' && c >= '0' && c <= '9') { return null; }
                }
                return result;
            }
            for (int i = 0; i < result.searchString.Length; i++) {
                if (result.rightArray[i] == false) {
                    var c = result.searchString[i];
                    if (result.rightArray[i - 1]) {
                        var k = result.searchString[i - 1];
                        if (IsSameType(k, c)) { return null; }
                    }
                    if (result.rightArray[i + 1]) {
                        var k = result.searchString[i + 1];
                        if (IsSameType(k, c)) { return null; }
                    }
                }
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            var i = Start << 5;
            i += End - Start;
            return i << 1 + (Success ? 1 : 0);
        }
        public override string ToString()
        {
            return Start.ToString() + "|" + SearchString;
        }
    }

    /// <summary>
    /// 非法词语查询
    /// </summary>
    public class IllegalWords
    {
        #region Local fields
        private TreeNode _root;
        private int _jumpLength;
        #endregion

        public IllegalWords(ICollection<string> keywords, int jumpLength = 1)
        {
            _jumpLength = jumpLength;
            BuildTree(keywords);
        }
        void BuildTree(ICollection<string> _keywords)
        {
            _root = new TreeNode(null, ' ');
            foreach (string p in _keywords) {
                var t = WordHelper.ToSenseWord(p);

                // add pattern to tree
                TreeNode nd = _root;
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


        public List<IllegalResult> FindAll(string text)
        {
            var searchText = WordHelper.ToSenseWord(text);

            HashSet<IllegalResult> ret = new HashSet<IllegalResult>();
            IllegalSearchHelper helper = new IllegalSearchHelper(_root, _jumpLength);
            int index = 0;
            while (index < text.Length) {
                var c = text[index];
                if (helper.FindChar(c, index)) {
                    foreach (var find in helper.FindResults(index)) {
                        var r = IllegalResult.GetIllegalResult(find, index, searchText, text);
                        if (r != null) ret.Add(r);
                    }
                }
                index++;
            }
            return ret.Distinct().ToList();
        }

        public IllegalResult FindFirst(string text)
        {
            var searchText = WordHelper.ToSenseWord(text);
            IllegalSearchHelper helper = new IllegalSearchHelper(_root, _jumpLength);
            TreeNode ptr = _root;
            int index = 0;

            while (index < text.Length) {
                var c = text[index];
                if (helper.FindChar(c, index)) {
                    foreach (var find in helper.FindResults(index)) {
                        var r = IllegalResult.GetIllegalResult(find, index, searchText, text);
                        if (r != null) return r;
                    }
                }
                index++;
            }
            return IllegalResult.Empty;
        }

        public bool ContainsAny(string text)
        {
            return FindFirst(text).Success;
        }
    }

    class IllegalSearchHelper
    {
        class NodeInfo
        {
            public int Start;
            public TreeNode Node;
        }
        TreeNode _root;
        List<NodeInfo> Nodes = new List<NodeInfo>();
        int _jumpLength;

        public IllegalSearchHelper(TreeNode root, int jumpLength)
        {
            _root = root;
            _jumpLength = jumpLength;
        }


        public bool FindChar(char c, int start = 0)
        {
            bool hasChange = false;
            if (Nodes.Count == 0) {
                var ptr = _root;
                TreeNode trans = null;

                while (trans == null) {
                    trans = ptr.GetTransition(c);
                    if (ptr == _root) { break; }
                }
                if (trans != null ) {
                    hasChange = true;
                    Nodes.Add(new NodeInfo() { Start = start, Node = trans });
                }
            } else {
                var length = Nodes.Count;
                var _rootSearch = false;
                for (int i = 0; i < length; i++) {
                    var ptr = Nodes[i].Node;

                    TreeNode trans = null;
                    while (trans == null) {
                        trans = ptr.GetTransition(c);
                        if (ptr == _root) { _rootSearch = true; break; }
                        if (trans == null) {
                            ptr = ptr.Failure;
                            if (_rootSearch && ptr == _root) break;
                        }
                    }
                    if (trans != null && trans != _root) {
                        hasChange = true;
                        Nodes.Add(new NodeInfo() { Start = start, Node = trans });
                    }
                }
                Nodes.RemoveAll(q => q.Start < start - _jumpLength);
            }




            //for (int i = Nodes.Count - 1; i >= 0; i--) {
            //    if (Nodes[i].Node == _root) Nodes.RemoveAt(i);
            //}
            return hasChange;
        }

        public List<string> FindResults(int start)
        {
            List<string> r = new List<string>();
            foreach (var item in Nodes) {
                if (item.Start == start) {
                    foreach (string found in item.Node.Results) r.Add(found);
                }
            }
            return r;
        }

    }
}
