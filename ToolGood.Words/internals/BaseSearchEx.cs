using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseSearchEx
    {
        #region Class
        class TrieNode
        {
            public TrieNode Parent;
            public TrieNode Failure;
            public char Char;
            internal bool End;
            internal List<Int32> Results;
            internal Dictionary<char, TrieNode> m_values;
            internal Dictionary<char, TrieNode> merge_values;
            private Int32 minflag = Int32.MaxValue;
            private Int32 maxflag = 0;
            internal Int32 Next;
            private Int32 Count;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                merge_values = new Dictionary<char, TrieNode>();
                Results = new List<Int32>();
            }

            public bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (Int32)c && maxflag >= (Int32)c) {
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
                node.Parent = this;
                node.Char = c;
                m_values[c] = node;
                Count++;
                return node;
            }

            public void SetResults(Int32 text)
            {
                if (End == false) {
                    End = true;
                }
                if (Results.Contains(text) == false) {
                    Results.Add(text);
                }
            }

            public void Merge(TrieNode node)
            {
                var nd = node;
                while (nd.Char != 0) {
                    foreach (var item in node.m_values) {
                        if (m_values.ContainsKey(item.Key) == false) {
                            if (merge_values.ContainsKey(item.Key) == false) {
                                if (minflag > item.Key) { minflag = item.Key; }
                                if (maxflag < item.Key) { maxflag = item.Key; }
                                merge_values[item.Key] = item.Value;
                                Count++;
                            }
                        }
                    }
                    nd = nd.Failure;
                }
            }

            public Int32 Rank(TrieNode[] has)
            {
                bool[] seats = new bool[has.Length];
                Int32 start = 1;

                has[0] = this;

                Rank(ref start, seats, has);
                Int32 maxCount = has.Length - 1;
                while (has[maxCount] == null) { maxCount--; }
                return maxCount;
            }

            private void Rank(ref Int32 start, bool[] seats, TrieNode[] has)
            {
                if (maxflag == 0) return;
                var keys = m_values.Select(q => (Int32)q.Key).ToList();
                keys.AddRange(merge_values.Select(q => (Int32)q.Key).ToList());

                while (has[start] != null) { start++; }
                var s = start < (Int32)minflag ? (Int32)minflag : start;

                for (Int32 i = s; i < has.Length; i++) {
                    if (has[i] == null) {
                        var next = i - (Int32)minflag;
                        //if (next < 0) continue;
                        if (seats[next]) continue;

                        var isok = true;
                        foreach (var item in keys) {
                            if (has[next + item] != null) { isok = false; break; }
                        }
                        if (isok) {
                            SetSeats(next, seats, has);
                            break;
                        }
                    }
                }
                start += keys.Count / 2;

                var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);
                foreach (var key in keys2) {
                    key.Value.Rank(ref start, seats, has);
                }
            }


            private void SetSeats(Int32 next, bool[] seats, TrieNode[] has)
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

            }


        }
        #endregion

        #region 私有变量
        protected string[] _keywords;
        protected Int32[][] _guides;
        protected Int32[] _key;
        protected Int32[] _next;
        protected Int32[] _check;
        protected Int32[] _dict;
        #endregion



        #region 保存到文件
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var fs = File.Open(filePath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            Save(bw);
#if NETSTANDARD1_3
            bw.Dispose();
            fs.Dispose();
#else
            bw.Close();
            fs.Close();
#endif
        }

        /// <summary>
        /// 保存到Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            Save(bw);
#if NETSTANDARD1_3
            bw.Dispose();
#else
            bw.Close();
#endif
        }

        protected internal virtual void Save(BinaryWriter bw)
        {
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
        }

        protected byte[] CharArrToByteArr(char[] intArr)
        {
            Int32 intSize = sizeof(char) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }

        protected byte[] IntArrToByteArr(Int32[] intArr)
        {
            Int32 intSize = sizeof(Int32) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        protected byte[] BoolArrToByteArr(bool[] intArr)
        {
            Int32 intSize = sizeof(bool) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
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
            Load(br);
#if NETSTANDARD1_3
            br.Dispose();
            fs.Dispose();
#else
            br.Close();
            fs.Close();
#endif
        }
        /// <summary>
        /// 加载Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            Load(br);
#if NETSTANDARD1_3
            br.Dispose();
#else
            br.Close();
#endif
        }

        protected internal virtual void Load(BinaryReader br)
        {
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
            bs = br.ReadBytes(length);
            _key = ByteArrToIntArr(bs);

            length = br.ReadInt32();
            bs = br.ReadBytes(length);
            _next = ByteArrToIntArr(bs);

            length = br.ReadInt32();
            bs = br.ReadBytes(length);
            _check = ByteArrToIntArr(bs);

            length = br.ReadInt32();
            bs = br.ReadBytes(length);
            _dict = ByteArrToIntArr(bs);
        }

        protected char[] ByteArrToCharArr(byte[] btArr)
        {
            Int32 intSize = btArr.Length / sizeof(char);
            char[] intArr = new char[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }

        protected Int32[] ByteArrToIntArr(byte[] btArr)
        {
            Int32 intSize = btArr.Length / sizeof(Int32);
            Int32[] intArr = new Int32[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }

        protected bool[] ByteArrToBoolArr(byte[] btArr)
        {
            Int32 intSize = btArr.Length / sizeof(bool);
            bool[] intArr = new bool[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }
#endregion

#region 设置关键字
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public virtual void SetKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            var length = CreateDict(keywords);
            var root = new TrieNode();

            for (Int32 i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (Int32 j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)_dict[p[j]]);
                }
                nd.SetResults(i);
            }

            List<TrieNode> nodes = new List<TrieNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TrieNode nd in root.m_values.Values) {
                nd.Failure = root;
                foreach (TrieNode trans in nd.m_values.Values) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TrieNode> newNodes = new List<TrieNode>();

                //ArrayList newNodes = new ArrayList();
                foreach (TrieNode nd in nodes) {
                    TrieNode r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.m_values.ContainsKey(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = root;
                    else {
                        nd.Failure = r.m_values[c];
                        foreach (var result in nd.Failure.Results)
                            nd.SetResults(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TrieNode child in nd.m_values.Values)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            root.Failure = root;
            foreach (var item in root.m_values.Values) {
                TryLinks(item);
            }
            build(root, length);
        }
        private void TryLinks(TrieNode node)
        {
            node.Merge(node.Failure);
            foreach (var item in node.m_values.Values) {
                TryLinks(item);
            }
        }

        private void build(TrieNode root, Int32 length)
        {
            TrieNode[] has = new TrieNode[0x00FFFFFF];
            length = root.Rank(has) + length + 1;
            _key = new Int32[length];
            _next = new Int32[length];
            _check = new Int32[length];
            List<Int32[]> guides = new List<Int32[]>();
            guides.Add(new Int32[] { 0 });
            for (Int32 i = 0; i < length; i++) {
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

#endregion

#region 生成映射字典

        private Int32 CreateDict(ICollection<string> keywords)
        {
            Dictionary<char, Int32> dictionary = new Dictionary<char, Int32>();

            foreach (var keyword in keywords) {
                for (Int32 i = 0; i < keyword.Length; i++) {
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
            _dict = new Int32[char.MaxValue + 1];
            for (Int32 i = 0; i < list2.Count; i++) {
                _dict[list2[i]] = i + 1;
            }
            return dictionary.Count;
        }
#endregion
    }
}
