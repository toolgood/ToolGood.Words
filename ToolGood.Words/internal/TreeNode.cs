using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.Words
{
    class TreeNode<T>
    {
        #region Constructor & Methods

        public TreeNode(TreeNode<T> parent, char c)
        {
            _char = c; _parent = parent;
            _results = new List<T>();

            _transitionsAr = new List<TreeNode<T>>();
            _transHash = new Dictionary<char, TreeNode<T>>();
        }

        public void AddResult(T result)
        {
            if (_results.Contains(result)) return;
            _results.Add(result);
        }

        public void AddTransition(TreeNode<T> node)
        {
            if (minflag > node.Char) { minflag = node.Char; }
            if (maxflag < node.Char) { maxflag = node.Char; }
            flag = flag | node.Char;
            _transHash.Add(node.Char, node);
            _transitionsAr.Add(node);
        }

        public TreeNode<T> GetTransition(char c)
        {
            if ((flag | c) == flag && minflag <= (uint)c && maxflag >= (uint)c) {
                TreeNode<T> tn;
                if (_transHash.TryGetValue(c, out tn)) { return tn; }
            }
            return null;
        }

        public bool ContainsTransition(char c)
        {
            return _transHash.ContainsKey(c);
        }
        #endregion

        #region Properties
        private int flag = 0;
        private uint minflag = uint.MaxValue;
        private uint maxflag = uint.MinValue;

        private char _char;
        private TreeNode<T> _parent;
        private TreeNode<T> _failure;
        private List<T> _results;
        private List<TreeNode<T>> _transitionsAr;
        private Dictionary<char, TreeNode<T>> _transHash;

        public char Char
        {
            get { return _char; }
        }

        public TreeNode<T> Parent
        {
            get { return _parent; }
        }


        /// <summary>
        /// Failure function - descendant node
        /// </summary>
        public TreeNode<T> Failure
        {
            get { return _failure; }
            set { _failure = value; }
        }


        /// <summary>
        /// Transition function - list of descendant nodes
        /// </summary>
        public List<TreeNode<T>> Transitions
        {
            get { return _transitionsAr; }
        }


        /// <summary>
        /// Returns list of patterns ending by this letter
        /// </summary>
        public List<T> Results
        {
            get { return _results; }
        }

        #endregion

        public override string ToString()
        {
            return _char.ToString() + "|" + _transHash.Count.ToString();
        }
    }
}
