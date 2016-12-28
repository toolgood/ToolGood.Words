//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ToolGood.Words
//{
//    public class PinYinSearch
//    {
//        class TreeNode
//        {
//            private Dictionary<char, TreeNode> _dict = new Dictionary<char, TreeNode>();
//            public bool End { get; private set; }
//            private List<int> _results;

//            public HashSet<int> GetAllResults()
//            {
//                HashSet<int> list = new HashSet<int>();
//                getAllResults(list);
//                return list;
//            }
//            private void getAllResults(HashSet<int> list)
//            {
//                if (End) {
//                    foreach (var item in _results) {
//                        list.Add(item);
//                    }
//                }
//                foreach (var item in _dict) {
//                    item.Value.getAllResults(list);
//                }
//            }

//            public HashSet<int> GetAllResults(int count)
//            {
//                HashSet<int> list = new HashSet<int>();
//                getAllResults(list, count);
//                return list;
//            }
//            private void getAllResults(HashSet<int> list, int count)
//            {
//                if (End) {
//                    foreach (var item in _results) {
//                        list.Add(item);
//                        if (list.Count >= count) {
//                            break;
//                        }
//                    }
//                }
//                foreach (var item in _dict) {
//                    item.Value.getAllResults(list);
//                    if (list.Count >= count) {
//                        break;
//                    }
//                }
//            }


//        }
//        private string[] _keywords;
//        private long[] indexs;
//        private TreeNode _root;
//        private TreeNode[] _first;

//        public void SetKeywords(IDictionary<string, long> keywords, PinYinSearchType type)
//        {
            


//        }

//        public void SetKeywords(ICollection<string> keywords, PinYinSearchType type)
//        {

//        }


//        public List<string> SearchText(string text)
//        {

//            return null;
//        }

//        public List<long> SearchIndex(string text)
//        {

//            return null;
//        }

//    }

//    [Flags]
//    public enum PinYinSearchType
//    {
//        /// <summary>
//        /// 无
//        /// </summary>
//        None,
//        /// <summary>
//        /// 拼音
//        /// </summary>
//        PinYin,
//        /// <summary>
//        /// 首字母拼音
//        /// </summary>
//        FirstPinYin,
//        /// <summary>
//        /// 全拼音
//        /// </summary>
//        AllPinYin,
//        /// <summary>
//        /// 可从中间开始搜索
//        /// </summary>
//        StartMiddle,
//        /// <summary>
//        /// 空格代替符号
//        /// </summary>
//        SpaceReplaceSymbol

//    }

//}
