using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseSearchEx2
    {

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
            bw.Close();
            fs.Close();
        }

        /// <summary>
        /// 保存到Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            Save(bw);
            bw.Close();
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
            br.Close();
            fs.Close();
        }
        /// <summary>
        /// 加载Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            Load(br);
            br.Close();
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
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(char));
            char[] intArr = new char[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }

        protected Int32[] ByteArrToIntArr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(Int32));

            Int32[] intArr = new Int32[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }

        protected bool[] ByteArrToBoolArr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(bool));
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
            SetKeywords();
        }

        private void SetKeywords()
        {
            var root = new TrieNode();
            Dictionary<int, List<TrieNode>> allNodeLayers = new Dictionary<int, List<TrieNode>>();
            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (int j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)p[j]);
                    if (nd.Layer == 0) {
                        nd.Layer = j + 1;
                        List<TrieNode> trieNodes;
                        if (allNodeLayers.TryGetValue(nd.Layer, out trieNodes) == false) {
                            trieNodes = new List<TrieNode>();
                            allNodeLayers[nd.Layer] = trieNodes;
                        }
                        trieNodes.Add(nd);
                    }
                }
                nd.SetResults(i);
            }

            List<TrieNode> allNode = new List<TrieNode>();
            allNode.Add(root);
            foreach (var trieNodes in allNodeLayers) {
                foreach (var nd in trieNodes.Value) {
                    allNode.Add(nd);
                }
            }
            allNodeLayers = null;

            for (int i = 1; i < allNode.Count; i++) {
                var nd = allNode[i];
                nd.Index = i;
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
            }
            root.Failure = root;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < allNode.Count; i++) {
                stringBuilder.Append(allNode[i].Char);
            }
            var length = CreateDict(stringBuilder.ToString());
            stringBuilder = null;

            var allNode2 = new List<TrieNodeEx>();
            for (int i = 0; i < allNode.Count; i++) {
                allNode2.Add(new TrieNodeEx() { Index = i });
            }
            for (int i = 0; i < allNode2.Count; i++) {
                var oldNode = allNode[i];
                var newNode = allNode2[i];
                newNode.Char = _dict[oldNode.Char];

                foreach (var item in oldNode.m_values) {
                    var key = _dict[item.Key];
                    var index = item.Value.Index;
                    newNode.Add(key, allNode2[index]);
                }
                foreach (var item in oldNode.Results) {
                    newNode.SetResults(item);
                }
                oldNode = oldNode.Failure;
                while (oldNode != root) {
                    foreach (var item in oldNode.m_values) {
                        var key = _dict[item.Key];
                        var index = item.Value.Index;
                        if (newNode.HasKey(key) == false) {
                            newNode.Add(key, allNode2[index]);
                        }
                    }
                    foreach (var item in oldNode.Results) {
                        newNode.SetResults(item);
                    }
                    oldNode = oldNode.Failure;
                }
            }
            allNode.Clear();
            allNode = null;
            root = null;


            build(allNode2, length);
        }






        private void build(List<TrieNodeEx> nodes, Int32 length)
        {
            int[] has = new int[0x00FFFFFF];
            bool[] seats = new bool[0x00FFFFFF];
            bool[] seats2 = new bool[0x00FFFFFF];
            Int32 start = 1;
            Int32 oneStart = 1;
            for (int i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                node.Rank(ref oneStart, ref start, seats, seats2, has);
            }
            Int32 maxCount = has.Length - 1;
            while (has[maxCount] == 0) { maxCount--; }
            length = maxCount + length + 1;

            _key = new Int32[length];
            _next = new Int32[length];
            _check = new Int32[length];
            List<Int32[]> guides = new List<Int32[]>();
            guides.Add(new Int32[] { 0 });
            for (Int32 i = 0; i < length; i++) {
                var item = nodes[has[i]];
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

        private int CreateDict(string keywords)
        {
            Dictionary<char, Int32> dictionary = new Dictionary<char, Int32>();
            foreach (var item in keywords) {
                if (dictionary.ContainsKey(item)) {
                    dictionary[item] += 1;
                } else {
                    dictionary[item] = 1;
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


        //private Int32 CreateDict()
        //{
        //    Dictionary<char, Int32> dictionary = new Dictionary<char, Int32>();

        //    foreach (var keyword in _keywords) {
        //        for (Int32 i = 0; i < keyword.Length; i++) {
        //            var item = keyword[i];
        //            if (dictionary.ContainsKey(item)) {
        //                if (i > 0)
        //                    dictionary[item] += 2;
        //            } else {
        //                dictionary[item] = i > 0 ? 2 : 1;
        //            }
        //        }
        //    }
        //    var list = dictionary.OrderByDescending(q => q.Value).Select(q => q.Key).ToList();
        //    var list2 = new List<char>();
        //    var sh = false;
        //    foreach (var item in list) {
        //        if (sh) {
        //            list2.Add(item);
        //        } else {
        //            list2.Insert(0, item);
        //        }
        //        sh = !sh;
        //    }
        //    _dict = new Int32[char.MaxValue + 1];
        //    for (Int32 i = 0; i < list2.Count; i++) {
        //        _dict[list2[i]] = i + 1;
        //    }
        //    return dictionary.Count;
        //}
        #endregion
    }
}
