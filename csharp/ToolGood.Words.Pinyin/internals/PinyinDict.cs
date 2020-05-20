using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Pinyin.internals
{
    public static class PinyinDict
    {
        private static Dictionary<string, ushort[]> _pyName;
        private static string[] _pyShow;
        private static ushort[] _pyIndex;
        private static ushort[] _pyData;
        private static ushort[][] _pyIndex2;
        private static ushort[][] _pyData2;
        private static int[] _wordPyIndex;
        private static ushort[] _wordPy;
        private static WordsSearchEx _search;

        public static string[] GetPinyinList(string text, int tone = 0)
        {
            InitPy();

            List<string> list = new List<string>();
            for (int j = 0; j < text.Length; j++) { list.Add(null); }

            var pos = _search.FindAll(text);
            var pindex = -1;
            foreach (var p in pos) {
                if (p.Start > pindex) {
                    for (int j = 0; j < p.Length; j++) {
                        list[j + p.Start] = _pyShow[_wordPy[_wordPyIndex[p.Index] + j] + tone];
                    }
                    pindex = p.End;
                }
            }
            var i = 0;
            while (i < text.Length) {
                if (list[i] == null) {
                    var c = text[i];
                    if (c >= 0x3400 && c <= 0x9fd5) {
                        var index = c - 0x3400;
                        var start = _pyIndex[index];
                        var end = _pyIndex[index + 1];
                        if (end > start) {
                            list[i] = _pyShow[_pyData[start] + tone];
                        }
                    } else if (c >= 0xd840 && c <= 0xd86e && i + 1 < text.Length) {
                        var ct = text[i + 1];
                        if (ct >= 0xdc00 && ct <= 0xdfff) {
                            var index = _pyIndex2[c - 0xd840][ct - 0xdc00];
                            var index2 = _pyIndex2[c - 0xd840][ct - 0xdc00 + 1];
                            if (index < index2) {
                                i++;
                                list[i] = _pyShow[_pyData2[c - 0xd840][index] + tone];
                            } else {
                                list[i] = text[i].ToString();
                            }
                        } else {
                            list[i] = text[i].ToString();
                        }
                    } else {
                        list[i] = text[i].ToString();
                    }
                }
                i++;
            }
            list.RemoveAll(q => q == null);
            return list.ToArray();
        }

        public static string GetFirstPinyin(string text, int tone = 0)
        {
            InitPy();

            string[] list = GetPinyinList(text, tone);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Length; i++) {
                var c = list[i];
                if (c[0] <= 128) {
                    sb.Append(c[0]);
                } else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static List<string> GetAllPinyin(char c, int tone = 0)
        {
            InitPy();
            if (c >= 0x3400 && c <= 0x9fd5) {
                var index = c - 0x3400;
                List<string> list = new List<string>();
                var start = _pyIndex[index];
                var end = _pyIndex[index + 1];
                if (end > start) {
                    for (int i = start; i < end; i++) {
                        list.Add(_pyShow[_pyData[i] + tone]);
                    }
                }
                return list.Distinct().ToList();
            }
            return new List<string>();
        }

        public static List<string> GetPinyinForName(string name, int tone = 0)
        {
            InitPy();

            List<string> list = new List<string>();
            string xing;
            string ming;
            ushort[] indexs;
            if (name.Length > 1) { // 检查复姓
                xing = name.Substring(0, 2);
                if (_pyName.TryGetValue(xing, out indexs)) {
                    foreach (var index in indexs) {
                        list.Add(_pyShow[index + tone]);
                    }
                    if (name.Length > 2) {
                        ming = name.Substring(2);
                        list.AddRange(GetPinyinList(ming, tone));
                    }
                    return list;
                }
            }
            xing = name.Substring(0, 1);
            if (_pyName.TryGetValue(xing, out indexs)) {
                foreach (var index in indexs) {
                    list.Add(_pyShow[index + tone]);
                }
                if (name.Length > 1) {
                    ming = name.Substring(1);
                    list.AddRange(GetPinyinList(ming, tone));
                }
                return list;
            }

            return GetPinyinList(name, tone).ToList();
        }



        #region private
        private static object lockObj = new object();

        private static void InitPy()
        {
            if (_search == null) {
                lock (lockObj) {
                    if (_search == null) {
                        var ass = typeof(WordsHelper).Assembly;
                        Stream sm = ass.GetManifestResourceStream("ToolGood.Words.Pinyin.dict.PinyinDict.dat");
                        BinaryReader reader = new BinaryReader(sm);
                        _pyName = new Dictionary<string, ushort[]>();
                        var length = reader.ReadInt32();
                        for (int i = 0; i < length; i++) {
                            var key = reader.ReadString();
                            var count = reader.ReadInt32();
                            var ubs = reader.ReadBytes(count);
                            _pyName.Add(key, ByteArrToUint16Arr(ubs));
                        }
                        length = reader.ReadInt32();
                        _pyShow = new string[length];
                        for (int i = 0; i < length; i++) {
                            _pyShow[i] = reader.ReadString();
                        }

                        length = reader.ReadInt32();
                        var bs = reader.ReadBytes(length);
                        _pyIndex = ByteArrToUint16Arr(bs);


                        length = reader.ReadInt32();
                        bs = reader.ReadBytes(length);
                        _pyData = ByteArrToUint16Arr(bs);

                        length = reader.ReadInt32();
                        _pyIndex2 = new ushort[length][];
                        for (int i = 0; i < length; i++) {
                            var count = reader.ReadInt32();
                            bs = reader.ReadBytes(count);
                            _pyIndex2[i] = ByteArrToUint16Arr(bs);
                        }

                        length = reader.ReadInt32();
                        _pyData2 = new ushort[length][];
                        for (int i = 0; i < length; i++) {
                            var count = reader.ReadInt32();
                            bs = reader.ReadBytes(count);
                            _pyData2[i] = ByteArrToUint16Arr(bs);
                        }

                        length = reader.ReadInt32();
                        bs = reader.ReadBytes(length);
                        _wordPyIndex = ByteArrToIntArr(bs);

                        length = reader.ReadInt32();
                        bs = reader.ReadBytes(length);
                        _wordPy = ByteArrToUint16Arr(bs);

                        reader.Close();
                        sm.Close();

                        Stream sm2 = ass.GetManifestResourceStream("ToolGood.Words.Pinyin.dict.SearchEx.dat");
                        var search = new WordsSearchEx();
                        search.Load(sm2);
                        sm2.Close();
                        _search = search;
                    }
                }
            }
        }

        private static Int32[] ByteArrToIntArr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(Int32));
            Int32[] intArr = new Int32[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }

        private static ushort[] ByteArrToUint16Arr(byte[] btArr)
        {
            Int32 intSize = (int)Math.Ceiling(btArr.Length / (double)sizeof(UInt16));
            ushort[] intArr = new ushort[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, btArr.Length);
            return intArr;
        }
        #endregion

    }
}
