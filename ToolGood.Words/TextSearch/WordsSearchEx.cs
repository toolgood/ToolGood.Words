using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ToolGood.Words
{
    /// <summary>
    /// 文本搜索（增强版，速度更快），带返回位置及索引号
    /// </summary>
    public class WordsSearchEx
    {
        #region Class
        class TrieNode
        {
            public char Char;
            internal bool End;
            internal List<int> Results;
            internal Dictionary<char, TrieNode> m_values;
            internal Dictionary<char, TrieNode> merge_values;
            private uint minflag = uint.MaxValue;
            private uint maxflag = uint.MinValue;
            internal int Next;
            private int Count;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                merge_values = new Dictionary<char, TrieNode>();
                Results = new List<int>();
            }

            public bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (uint)c && maxflag >= (uint)c) {
                    return m_values.TryGetValue(c, out node);
                }
                node = null;
                return false;
            }

            public TrieNode Add(char c)
            {
                TrieNode node;

                if (m_values.TryGetValue(c, out node)) {
                    return node;
                }

                if (minflag > c) { minflag = c; }
                if (maxflag < c) { maxflag = c; }

                node = new TrieNode();
                node.Char = c;
                m_values[c] = node;
                Count++;
                return node;
            }

            public void SetResults(int text)
            {
                if (End == false) {
                    End = true;
                }
                if (Results.Contains(text) == false) {
                    Results.Add(text);
                }
            }

            public void Merge(TrieNode node, Dictionary<TrieNode, TrieNode> links)
            {
                if (node.End) {
                    if (End == false) {
                        End = true;
                    }
                    foreach (var item in node.Results) {
                        if (Results.Contains(item) == false) {
                            Results.Add(item);
                        }
                    }
                }

                foreach (var item in node.m_values) {
                    if (m_values.ContainsKey(item.Key) == false) {
                        if (minflag > item.Key) { minflag = item.Key; }
                        if (maxflag < item.Key) { maxflag = item.Key; }
                        if (merge_values.ContainsKey(item.Key) == false) {
                            merge_values[item.Key] = item.Value;
                            Count++;
                        }
                    }
                }
                TrieNode node2;
                if (links.TryGetValue(node, out node2)) {
                    Merge(node2, links);
                }
            }

            public int GetMaxLength()
            {
                var count = m_values.Count + merge_values.Count;
                count = count * 5;
                foreach (var item in m_values) {
                    count += item.Value.GetMaxLength();
                }
                return count;
            }

            public int Rank(TrieNode[] has)
            {
                bool[] seats = new bool[has.Length];
                int maxCount = 1;
                int start = 1;

                has[0] = this;

                Rank(ref maxCount, ref start, seats, has);
                return maxCount;
            }

            private void Rank(ref int maxCount, ref int start, bool[] seats, TrieNode[] has)
            {
                if (maxflag == 0) return;
                var keys = m_values.Select(q => q.Key).ToList();
                keys.AddRange(merge_values.Select(q => q.Key).ToList());

                while (has[start] != null) { start++; }
                for (int i = start; i < has.Length; i++) {
                    if (has[i] == null) {
                        var next = i - (int)minflag;
                        if (next < 0) continue;
                        if (seats[next]) continue;

                        var isok = true;
                        foreach (var item in keys) {
                            if (has[i - minflag + item] != null) { isok = false; break; }
                        }
                        if (isok) {
                            SetSeats(next, ref maxCount, seats, has);
                            break;
                        }
                    }
                }
                var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);
                foreach (var key in keys2) {
                    key.Value.Rank(ref maxCount, ref start, seats, has);
                }
            }

            private void SetSeats(int next, ref int maxCount, bool[] seats, TrieNode[] has)
            {
                Next = next;
                seats[next] = true;

                foreach (var item in merge_values) {
                    var position = next + item.Key;
                    has[position] = item.Value;
                }

                foreach (var item in m_values) {
                    var position = next + item.Key;
                    has[position] = item.Value;
                }
                var position2 = next + (int)maxflag;
                if (maxCount <= position2) {
                    maxCount = position2;
                }
            }


        }
        #endregion

        #region 私有变量
        private string[] _keywords;
        private int[][] _guides;
        private int[] _key;
        private int[] _next;
        private int[] _check;
        private int[] _dict; 
        #endregion

        #region 查找 替换 查找第一个关键字 判断是否包含关键字
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public List<WordsSearchResult> FindAll(string text)
        {
            List<WordsSearchResult> root = new List<WordsSearchResult>();
            var p = 0;

            for (int i = 0; i < text.Length; i++) {
                var t = (char)_dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    var index = _check[next];
                    if (index > 0) {
                        foreach (var item in _guides[index]) {
                            var key = _keywords[item];
                            var r = new WordsSearchResult(key, i + 1 - key.Length, i, item);
                            root.Add(r);
                        }
                    }
                    p = next;
                }
            }
            return root;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public WordsSearchResult FindFirst(string text)
        {
            var p = 0;
            for (int i = 0; i < text.Length; i++) {
                var t = (char)_dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                if (_key[next] == t) {
                    var index = _check[next];
                    if (index > 0) {
                        var item = _keywords[_guides[index][0]];
                        return new WordsSearchResult(item, i + 1 - item.Length, i, _guides[index][0]);
                    }
                    p = next;
                } else {
                    p = 0;
                    next = _next[p] + t;
                    if (_key[next] == t) {
                        var index = _check[next];
                        if (index > 0) {
                            var item = _keywords[_guides[index][0]];
                            return new WordsSearchResult(item, i + 1 - item.Length, i, _guides[index][0]);
                        }
                        p = next;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool ContainsAny(string text)
        {
            var p = 0;
            foreach (char t1 in text) {
                var t = (char)_dict[t1];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                if (_key[next] == t) {
                    if (_check[next] > 0) { return true; }
                    p = next;
                } else {
                    p = 0;
                    next = _next[p] + t;
                    if (_key[next] == t) {
                        if (_check[next] > 0) { return true; }
                        p = next;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*')
        {
            StringBuilder result = new StringBuilder(text);

            var p = 0;

            for (int i = 0; i < text.Length; i++) {
                var t = (char)_dict[text[i]];
                if (t == 0) {
                    p = 0;
                    continue;
                }
                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[p] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    var index = _check[next];
                    if (index > 0) {
                        var maxLength = _keywords[_guides[index][0]].Length;
                        var start = i + 1 - maxLength;
                        for (int j = start; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                    p = next;
                }
            }
            return result.ToString();
        }
        #endregion

        #region 保存到文件
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            var fs = File.Open(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(_keywords.Length);
            foreach (var item in _keywords) {
                bw.Write(item);
            }

            List<int> guideslist = new List<int>();
            guideslist.Add(_guides.Length);
            foreach (var guide in _guides) {
                guideslist.Add(guide.Length);
                foreach (var item in guide) {
                    guideslist.Add(item);
                }
            }
            var bs = IntArrToByteArr(guideslist.ToArray());
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_key);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_next);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_check);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_dict);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Close();
            fs.Close();
        }

        private byte[] IntArrToByteArr(int[] intArr)
        {
            int intSize = sizeof(int) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            //申请一块非托管内存  
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            //复制int数组到该内存块  
            Marshal.Copy(intArr, 0, ptr, intArr.Length);
            //复制回byte数组  
            Marshal.Copy(ptr, bytArr, 0, bytArr.Length);
            //释放申请的非托管内存  
            Marshal.FreeHGlobal(ptr);
            return bytArr;
        }

        #endregion

        #region 加载文件
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Load(string filePath)
        {
            var fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);

            var length = br.ReadInt32();
            _keywords = new string[length];
            for (int i = 0; i < length; i++) {
                _keywords[i] = br.ReadString();
            }

            length = br.ReadInt32();
            var bs = br.ReadBytes(length);
            using (MemoryStream ms = new MemoryStream(bs)) {
                BinaryReader b = new BinaryReader(ms);
                var length2 = b.ReadInt32();
                _guides = new int[length2][];
                for (int i = 0; i < length2; i++) {
                    var length3 = b.ReadInt32();
                    _guides[i] = new int[length3];
                    for (int j = 0; j < length3; j++) {
                        _guides[i][j] = b.ReadInt32();
                    }
                }
            }

            length = br.ReadInt32();
            _key = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _next = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _check = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _dict = ByteArrToIntArr(br.ReadBytes(length));

            br.Close();
            fs.Close();
        }

        private int[] ByteArrToIntArr(byte[] btArr)
        {
            int intSize = btArr.Length / sizeof(int);
            int[] intArr = new int[intSize];
            IntPtr ptr = Marshal.AllocHGlobal(btArr.Length);
            Marshal.Copy(btArr, 0, ptr, btArr.Length);
            Marshal.Copy(ptr, intArr, 0, intArr.Length);
            Marshal.FreeHGlobal(ptr);
            return intArr;
        }
        #endregion

        #region 设置关键字
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            var length = CreateDict(keywords);
            var root = new TrieNode();

            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (int j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)_dict[p[j]]);
                }
                nd.SetResults(i);
            }

            Dictionary<TrieNode, TrieNode> links = new Dictionary<TrieNode, TrieNode>();
            foreach (var item in root.m_values) {
                TryLinks(item.Value, null, links, root);
            }
            foreach (var item in links) {
                item.Key.Merge(item.Value, links);
            }

            build(root, length);
            //_root = root;
        }

        private void build(TrieNode root, int length)
        {
            TrieNode[] has = new TrieNode[root.GetMaxLength()];
            length = root.Rank(has) + length + 1;
            _key = new int[length];
            _next = new int[length];
            _check = new int[length];
            List<int[]> guides = new List<int[]>();
            guides.Add(new int[] { 0 });
            for (int i = 0; i < length; i++) {
                var item = has[i];
                if (item == null) continue;
                _key[i] = item.Char;
                _next[i] = item.Next;
                if (item.End) {
                    _check[i] = guides.Count;
                    guides.Add(item.Results.ToArray());
                }
            }
            _guides = guides.ToArray();
        }


        private void TryLinks(TrieNode node, TrieNode node2, Dictionary<TrieNode, TrieNode> links, TrieNode root)
        {
            foreach (var item in node.m_values) {
                TrieNode tn = null;
                if (node2 == null) {
                    if (root.TryGetValue(item.Key, out tn)) {
                        links[item.Value] = tn;
                    }
                } else if (node2.TryGetValue(item.Key, out tn)) {
                    links[item.Value] = tn;
                }
                TryLinks(item.Value, tn, links, root);
            }
        }
        #endregion

        #region 生成映射字典

        private int CreateDict(ICollection<string> keywords)
        {
            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            foreach (var keyword in keywords) {
                for (int i = 0; i < keyword.Length; i++) {
                    var item = keyword[i];
                    if (dictionary.ContainsKey(item)) {
                        if (i > 0)
                            dictionary[item] += 2;
                    } else {
                        dictionary[item] = i > 0 ? 2 : 1;
                    }
                }
            }
            var list = dictionary.OrderByDescending(q => q.Value).Select(q => q.Key).ToList();
            var list2 = new List<char>();
            var sh = false;
            foreach (var item in list) {
                if (sh) {
                    list2.Add(item);
                } else {
                    list2.Insert(0, item);
                }
                sh = !sh;
            }
            _dict = new int[char.MaxValue + 1];
            for (int i = 0; i < list2.Count; i++) {
                _dict[list2[i]] = i + 1;
            }
            return dictionary.Count;
        }
        #endregion

    }
}
