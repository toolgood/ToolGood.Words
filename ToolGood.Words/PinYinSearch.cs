using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolGood.Words
{
    /// <summary>
    /// 未优化
    /// </summary>
    public class PinYinSearch
    {
        #region class
        class KeywordsInfo
        {
            public string Keywords;
            public int Length { get { return Keywords.Length; } }
            public long Index;

            public KeywordsInfo(string keywords, long index)
            {
                Keywords = keywords;
                Index = index;
            }
        }
        class TreeNode
        {
            private Dictionary<char, TreeNode> nodes = new Dictionary<char, TreeNode>();
            private bool End;//是否包含结果
            private HashSet<KeywordsInfo> keywords;//关键字
            private bool sm_End;//中间开始 是否包含结果
            private HashSet<KeywordsInfo> sm_keywords;//中间开始 关键字


            public TreeNode Add(char c)
            {
                TreeNode node = new TreeNode();
                nodes.Add(c, node);
                return node;
            }

            public void SetKeywords(ref KeywordsInfo keyword)
            {
                End = true;
                if (keywords == null) {
                    keywords = new HashSet<KeywordsInfo>();
                }
                keywords.Add(keyword);
            }
            public void SetKeywords_sm(ref KeywordsInfo keyword)
            {
                sm_End = true;
                if (sm_keywords == null) {
                    sm_keywords = new HashSet<KeywordsInfo>();
                }
                sm_keywords.Add(keyword);
            }

            public bool TryGetValue(char c, out TreeNode tn)
            {
                return nodes.TryGetValue(c, out tn);
            }

            public void GetResult(int minLength, int maxLength, int maxCount, HashSet<KeywordsInfo> set)
            {
                if (End) {
                    foreach (var item in keywords) {
                        if (maxCount > 0 && set.Count >= maxCount) {
                            return;
                        }
                        Append(minLength, maxLength, set, item);
                    }
                }
                foreach (var node in nodes) {
                    if (maxCount > 0 && set.Count >= maxCount) {
                        return;
                    }
                    node.Value.GetResult(minLength, maxLength, maxCount, set);
                }
            }
            private void Append(int minLength, int maxLength, HashSet<KeywordsInfo> set, KeywordsInfo keywords)
            {
                if (minLength > 0) {
                    if (keywords.Length < minLength) {
                        return;
                    }
                }
                if (maxLength > 0) {
                    if (keywords.Length >= maxLength) {
                        return;
                    }
                }
                set.Add(keywords);
            }

            public void GetResult_sm(int minLength, int maxLength, int maxCount, HashSet<KeywordsInfo> set)
            {
                if (sm_End) {
                    foreach (var item in sm_keywords) {
                        if (maxCount > 0 && set.Count >= maxCount) {
                            return;
                        }
                        Append(minLength, maxLength, set, item);
                    }
                }
                foreach (var node in nodes) {
                    if (maxCount > 0 && set.Count >= maxCount) {
                        return;
                    }
                    node.Value.GetResult_sm(minLength, maxLength, maxCount, set);
                }
            }

        }
        class PinYinKeywordsInfo
        {
            public List<HashSet<string>> list = new List<HashSet<string>>();
            public string Keywords;
            public KeywordsInfo KeywordsInfo;
            public PinYinKeywordsInfo(KeywordsInfo keywordsInfo)
            {
                KeywordsInfo = keywordsInfo;
                Keywords = keywordsInfo.Keywords.ToLower();
            }

        }

        #endregion

        #region private
        private readonly static Regex replaceSpace = new Regex(" +");
        private List<KeywordsInfo> _Keywords;
        private TreeNode _root = new TreeNode();
        private PinYinSearchType _searchType;
        #endregion

        public PinYinSearch(PinYinSearchType type = PinYinSearchType.FirstPinYin)
        {
            _searchType = type;
        }

        #region SetKeywords

        public void SetKeywords(Dictionary<string, long> keywords)
        {
            _Keywords = new List<KeywordsInfo>();
            foreach (var item in keywords) {
                _Keywords.Add(new KeywordsInfo(item.Key, item.Value));
            }
            SetKeywords(_Keywords);
        }
        public void SetKeywords(List<string> keywords)
        {
            _Keywords = new List<KeywordsInfo>();
            for (int i = 0; i < keywords.Count; i++) {
                _Keywords.Add(new KeywordsInfo(keywords[i], i));
            }
            SetKeywords(_Keywords);
        }
        private void SetKeywords(List<KeywordsInfo> keywords)
        {
            List<PinYinKeywordsInfo> infos = new List<PinYinKeywordsInfo>();
            for (int i = 0; i < keywords.Count; i++) {
                var keyword = keywords[i];
                PinYinKeywordsInfo info = new PinYinKeywordsInfo(keyword);
                var text = WordsHelper.GetPinYin(keyword.Keywords.ToUpper());
                var keyList = split(text);

                for (int j = 0; j < keyword.Length; j++) {
                    var c = char.ToUpper(keyword.Keywords[j]);
                    HashSet<string> set = new HashSet<string>();
                    set.Add(c.ToString());
                    if (c >= 0x4e00 && c <= 0x9fa5) {
                        if (_searchType.HasFlag(PinYinSearchType.FirstPinYin)) {
                            set.Add(keyList[j][0].ToString().ToLower());
                        }
                        //if (_searchType.HasFlag(PinYinSearchType.PinYin)) {
                        //    set.Add(keyList[j].ToLower());
                        //}
                        if (_searchType.HasFlag(PinYinSearchType.AllPinYin)) {
                            var pinyins = WordsHelper.GetAllPinYin(c);
                            if (_searchType.HasFlag(PinYinSearchType.FirstPinYin)) {
                                foreach (var py in pinyins) { set.Add(py[0].ToString().ToLower()); }
                            }
                            //foreach (var py in pinyins) { set.Add(py.ToLower()); }
                        }
                    } else if (!(c >= '0' && c <= '9') || !(c >= 'a' && c <= 'z')) {
                        if (_searchType.HasFlag(PinYinSearchType.SpaceReplaceSymbol)) {
                            set.Add(" ");
                        }
                    }
                    info.list.Add(set);
                }
                infos.Add(info);
            }
            setKeywords(infos);
            if (_searchType.HasFlag(PinYinSearchType.StartMiddle)) {
                setKeywords_sm(infos);
            }
        }
        private List<string> split(string text)
        {
            List<string> list = new List<string>();
            var ms = Regex.Matches(text, "[A-Z][a-z]*|.");
            foreach (Match m in ms) {
                list.Add(m.Value);
            }
            return list;
        }

        private void setKeywords(List<PinYinKeywordsInfo> list)
        {
            _root = new TreeNode();

            foreach (var item in list) {
                var t = item.KeywordsInfo.Keywords.ToLower();
                TreeNode tn = _root;
                for (int i = 0; i < t.Length; i++) {
                    TreeNode n;
                    if (tn.TryGetValue(t[i], out n)) {
                        tn = n;
                    } else {
                        tn = tn.Add(t[i]);
                    }
                }
                tn.SetKeywords(ref item.KeywordsInfo);
            }
            foreach (var item in list) {
                setKeywords(item, 0, _root);
            }
        }
        private void setKeywords(PinYinKeywordsInfo info, int index, TreeNode node)
        {
            var list = info.list[index];
            foreach (var item in list) {
                var t = item;
                TreeNode tn = node;
                for (int i = 0; i < t.Length; i++) {
                    TreeNode n;
                    if (tn.TryGetValue(t[i], out n)) {
                        tn = n;
                    } else {
                        tn = tn.Add(t[i]);
                    }
                }
                if (index < info.list.Count - 1) {
                    setKeywords(info, index + 1, tn);
                } else {
                    tn.SetKeywords(ref info.KeywordsInfo);
                }
            }
        }

        private void setKeywords_sm(List<PinYinKeywordsInfo> list)
        {
            foreach (var item in list) {
                for (int i = 1; i < item.Keywords.Length; i++) {
                    setKeywords_sm(item, i, _root);
                }
            }
        }
        private void setKeywords_sm(PinYinKeywordsInfo info, int index, TreeNode node)
        {
            var list = info.list[index];
            foreach (var item in list) {
                var t = item.ToLower();
                TreeNode tn = node;
                for (int i = 0; i < t.Length; i++) {
                    TreeNode n;
                    if (tn.TryGetValue(t[i], out n)) {
                        tn = n;
                    } else {
                        tn = tn.Add(t[i]);
                    }
                    tn.SetKeywords_sm(ref info.KeywordsInfo);
                }
                if (index < info.list.Count - 1) {
                    setKeywords_sm(info, index + 1, tn);
                }
            }
        }
        #endregion

        #region Search
        public List<string> SearchText(string text, int maxCount = 0)
        {
            var list = Search(text, maxCount);
            return list.Select(q => q.Keywords).ToList();
        }
        public List<long> SearchIndex(string text, int maxCount = 0)
        {
            var list = Search(text, maxCount);
            return list.Select(q => q.Index).ToList();
        }
        private HashSet<KeywordsInfo> Search(string text, int maxCount)
        {
            var t = replaceSpace.Replace(text.Trim().ToLower(), " ");
            var tn = _root;
            for (int i = 0; i < t.Length; i++) {
                if (tn.TryGetValue(t[i], out tn) == false) {
                    return new HashSet<KeywordsInfo>();
                }
            }
            HashSet<KeywordsInfo> set = new HashSet<KeywordsInfo>();
            tn.GetResult(0, text.Length + 2, maxCount, set);
            tn.GetResult(text.Length + 2, 0, maxCount, set);
            if (_searchType.HasFlag(PinYinSearchType.StartMiddle)) {
                tn.GetResult_sm(0, 0, maxCount, set);
            }
            return set;
        }
        #endregion

    }

    [Flags]
    public enum PinYinSearchType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 首字母拼音
        /// </summary>
        FirstPinYin = 1,
        /// <summary>
        /// 拼音，吃内存
        /// </summary>
        PinYin = 2,
        /// <summary>
        /// 全拼音
        /// </summary>
        AllPinYin = 4,
        /// <summary>
        /// 可从中间开始搜索
        /// </summary>
        StartMiddle = 8,
        /// <summary>
        /// 空格代替符号
        /// </summary>
        SpaceReplaceSymbol = 16

    }

}
