using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseSearchEx
    {
        protected ushort[] _dict;
        protected int[] _first;

        protected IntDictionary[] _nextIndex;
        protected int[] _end;
        protected int[] _resultIndex;
        protected string[] _keywords;


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
                while (r != null && (r.m_values == null || !r.m_values.ContainsKey(c))) r = r.Failure;
                if (r == null)
                    nd.Failure = root;
                else {
                    nd.Failure = r.m_values[c];
                    if (nd.Failure.Results != null) {
                        foreach (var result in nd.Failure.Results)
                            nd.SetResults(result);
                    }
                }
            }
            root.Failure = root;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < allNode.Count; i++) {
                stringBuilder.Append(allNode[i].Char);
            }
            var length = CreateDict(stringBuilder.ToString());
            stringBuilder = null;

            var first = new int[Char.MaxValue + 1];
            if (allNode[0].m_values != null) {
                foreach (var item in allNode[0].m_values) {
                    var key = (char)_dict[item.Key];
                    first[key] = item.Value.Index;
                }
            }
            _first= first;

            var resultIndex2 = new List<int>();
            var isEndStart = new List<bool>();
            var _nextIndex2 = new IntDictionary[allNode.Count];

            for (int i = allNode.Count - 1; i >= 0; i--) {
                var dict = new Dictionary<ushort, int>();
                var result = new List<int>();
                var oldNode = allNode[i];

                if (oldNode.m_values != null) {
                    foreach (var item in oldNode.m_values) {
                        var key = (char)_dict[item.Key];
                        var index = item.Value.Index;
                        dict[key] = index;
                    }
                }
                if (oldNode.Results != null) {
                    foreach (var item in oldNode.Results) {
                        if (result.Contains(item)==false) {
                            result.Add(item);
                        }
                    }
                }

                oldNode = oldNode.Failure;
                while (oldNode != root) {
                    if (oldNode.m_values != null) {
                        foreach (var item in oldNode.m_values) {
                            var key = (char)_dict[item.Key];
                            var index = item.Value.Index;
                            if (dict.ContainsKey(key) == false) {
                                dict[key] = index;
                            }
                        }
                    }
                    if (oldNode.Results != null) {
                        foreach (var item in oldNode.Results) {
                            if (result.Contains(item) == false) {
                                result.Add(item);
                            }
                        }
                    }
                    oldNode = oldNode.Failure;
                }
                _nextIndex2[i] = new IntDictionary(dict);

                if (result.Count > 0) {
                    for (int j = result.Count - 1; j >= 0; j--) {
                        resultIndex2.Add(result[j]);
                        isEndStart.Add(false);
                    }
                    isEndStart[isEndStart.Count - 1] = true;
                } else {
                    resultIndex2.Add(-1);
                    isEndStart.Add(true);
                }
                dict = null;
                result= null;
                allNode[i].Dispose();
                allNode.RemoveAt(i);
            }
            allNode.Clear();
            allNode = null;
            root = null;
            _nextIndex = _nextIndex2;

            var resultIndex = new List<int>();
            var end = new List<int>() { };
            for (int i = isEndStart.Count - 1; i >= 0; i--) {
                if (isEndStart[i]) {
                    end.Add(resultIndex.Count);
                }
                if (resultIndex2[i] > -1) {
                    resultIndex.Add(resultIndex2[i]);
                }
            }
            end.Add(resultIndex.Count);
            _resultIndex = resultIndex.ToArray();
            _end = end.ToArray();
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
            _dict = new ushort[char.MaxValue + 1];
            for (Int32 i = 0; i < list2.Count; i++) {
                _dict[list2[i]] = (ushort)(i + 1);
            }
            return dictionary.Count;
        }

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
            var bs = IntArrToByteArr(_dict);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_first);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_end);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_resultIndex);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(_nextIndex.Length);
            foreach (var dict in _nextIndex) {
                var keys = dict.Keys;
                var values = dict.Values;

                bs = IntArrToByteArr(keys);
                bw.Write(bs.Length);
                bw.Write(bs);

                bs = IntArrToByteArr(values);
                bw.Write(bs.Length);
                bw.Write(bs);
            }
        }

        protected byte[] IntArrToByteArr(Int32[] intArr)
        {
            Int32 intSize = sizeof(Int32) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        protected byte[] IntArrToByteArr(ushort[] intArr)
        {
            Int32 intSize = sizeof(ushort) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }

        #endregion

        #region 加载文件
        /// <summary>
        /// 加载文件，注意：不是加载原文件，是加载Save后文件
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
        /// 加载Stream，注意：不是加载原文件，是加载Save后文件
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
            _dict = ByteArrToUshortArr(bs);


            length = br.ReadInt32();
            bs = br.ReadBytes(length);
            _first = ByteArrToIntArr(bs);

            length = br.ReadInt32();
            bs = br.ReadBytes(length);
            _end = ByteArrToIntArr(bs);

            length = br.ReadInt32();
            bs = br.ReadBytes(length);
            _resultIndex = ByteArrToIntArr(bs);

            var dictLength = br.ReadInt32();
            _nextIndex = new IntDictionary[dictLength];


            for (int i = 0; i < dictLength; i++) {
                length = br.ReadInt32();
                bs = br.ReadBytes(length);
                var keys = ByteArrToUshortArr(bs);

                length = br.ReadInt32();
                bs = br.ReadBytes(length);
                var values = ByteArrToIntArr(bs);


                IntDictionary dictionary = new IntDictionary(keys, values);
                _nextIndex[i] = dictionary;
            }
        }

        protected Int32[] ByteArrToIntArr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(Int32));
            Int32[] intArr = new Int32[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }
        protected ushort[] ByteArrToUshortArr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(ushort));
            ushort[] intArr = new ushort[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }
        #endregion
    }
}



