using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
#if NETSTANDARD2_1
using ZIPStream = System.IO.Compression.BrotliStream;
#else
using ZIPStream = System.IO.Compression.GZipStream;
#endif

namespace ToolGood.Words.internals
{
    public class PinyinDict
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
        public static string[] PyShow {
            get {
                InitPyIndex();
                return _pyShow;
            }
        }

        public static string[] GetPinyinList(string text, int tone = 0)
        {
            InitPyIndex();
            InitPyWords();

            List<string> list = new List<string>();
            for (int j = 0; j < text.Length; j++) { list.Add(null); }

            var pos = _search.FindAll(text);
            var pindex = -1;
            foreach (var p in pos) {
                if (p.Start > pindex) {
                    for (int j = 0; j < p.Keyword.Length; j++) {
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
            InitPyIndex();

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
            InitPyIndex();
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
        public static List<string> GetAllPinyin(char c, char ct, int tone = 0)
        {
            InitPyIndex();
            if (c >= 0xd840 && c <= 0xd86e) {
                if (ct >= 0xdc00 && ct <= 0xdfff) {
                    List<string> list = new List<string>();

                    var start = _pyIndex2[c - 0xd840][ct - 0xdc00];
                    var end = _pyIndex2[c - 0xd840][ct - 0xdc00 + 1];
                    if (start < end) {
                        for (int i = start; i < end; i++) {
                            list.Add(_pyShow[_pyData2[c - 0xd840][i] + tone]);
                        }
                    }
                }
            }
            return new List<string>();
        }


        public static string GetPinyinFast(char c, int tone = 0)
        {
            InitPyIndex();

            if (c >= 0x3400 && c <= 0x9fd5) {
                var index = c - 0x3400;
                var start = _pyIndex[index];
                var end = _pyIndex[index + 1];
                if (end > start) {
                    return _pyShow[_pyData[start] + tone];
                }
            }
            return c.ToString();
        }

        public static List<string> GetPinyinForName(string name, int tone = 0)
        {
            InitPyName();
            InitPyIndex();

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
        private static void InitPyIndex()
        {
            if (_pyIndex2 == null) {
                lock (lockObj) {
                    if (_pyIndex2 == null) {
                        var ass = typeof(WordsHelper).Assembly;
                        {
#if NETSTANDARD2_1
                    const string resourceName = "ToolGood.Words.dict.pyIndex.txt.br";
#else
                            const string resourceName = "ToolGood.Words.dict.pyIndex.txt.z";
#endif
                            Stream sm = ass.GetManifestResourceStream(resourceName);
                            byte[] bs = new byte[sm.Length];
                            sm.Read(bs, 0, (int)sm.Length);
                            sm.Close();
                            var bytes = Decompress(bs);
                            var tStr = Encoding.UTF8.GetString(bytes);
                            bytes = null;

                            var sp = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            tStr = null;

                            List<ushort> pyIndex = new List<ushort>() { 0 };
                            List<ushort> pyData = new List<ushort>();
                            for (int i = 0; i < sp.Length; i++) {
                                var idxs = sp[i];
                                if (i == 0) {
                                    _pyShow = idxs.Split(',');
                                } else {
                                    if (idxs != "0") {
                                        foreach (var idx in idxs.Split(',')) {
                                            pyData.Add(ushort.Parse(idx, System.Globalization.NumberStyles.HexNumber));
                                        }
                                    }
                                    pyIndex.Add((ushort)pyData.Count);
                                }
                            }
                            _pyData = pyData.ToArray();
                            _pyIndex = pyIndex.ToArray();
                            pyIndex = null;
                            pyData = null;
                        }
                        {
#if NETSTANDARD2_1
                    const string resourceName = "ToolGood.Words.dict.pyIndex2.txt.br";
#else
                            const string resourceName = "ToolGood.Words.dict.pyIndex2.txt.z";
#endif
                            Stream sm = ass.GetManifestResourceStream(resourceName);
                            byte[] bs = new byte[sm.Length];
                            sm.Read(bs, 0, (int)sm.Length);
                            sm.Close();
                            var bytes = Decompress(bs);
                            var tStr = Encoding.UTF8.GetString(bytes);
                            bytes = null;

                            List<ushort[]> pyIndex2 = new List<ushort[]>();
                            List<ushort[]> pyData2 = new List<ushort[]>();

                            var ts = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            tStr = null;

                            foreach (var t in ts) {
                                var sp = t.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                List<ushort> pyIndex = new List<ushort>() { 0 };
                                List<ushort> pyData = new List<ushort>();
                                for (int i = 0; i < sp.Length; i++) {
                                    var idxs = sp[i];
                                    if (idxs != "0") {
                                        foreach (var idx in idxs.Split(',')) {
                                            pyData.Add(ushort.Parse(idx, System.Globalization.NumberStyles.HexNumber));
                                        }
                                    }
                                    pyIndex.Add((ushort)pyData.Count);
                                }
                                pyIndex2.Add(pyIndex.ToArray());
                                pyData2.Add(pyData.ToArray());
                            }
                            _pyIndex2 = pyIndex2.ToArray();
                            _pyData2 = pyData2.ToArray();

                            pyIndex2 = null;
                            pyData2 = null;
                        }
                    }
                }
            }
        }

        private static void InitPyName()
        {
            if (_pyName == null) {
                lock (lockObj) {
                    if (_pyName == null) {
                        var ass = typeof(WordsHelper).Assembly;
#if NETSTANDARD2_1
                var resourceName = "ToolGood.Words.dict.pyName.txt.br";
#else
                        var resourceName = "ToolGood.Words.dict.pyName.txt.z";
#endif
                        Stream sm = ass.GetManifestResourceStream(resourceName);
                        byte[] bs = new byte[sm.Length];
                        sm.Read(bs, 0, (int)sm.Length);
                        sm.Close();
                        var bytes = Decompress(bs);
                        var tStr = Encoding.UTF8.GetString(bytes);

                        var lines = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        Dictionary<string, ushort[]> pyName = new Dictionary<string, ushort[]>();
                        foreach (var line in lines) {
                            var sp = line.Split(',');
                            List<ushort> index = new List<ushort>();
                            for (int i = 1; i < sp.Length; i++) {
                                var idx = sp[i];
                                index.Add(ushort.Parse(idx, System.Globalization.NumberStyles.HexNumber));
                            }
                            pyName[sp[0]] = index.ToArray();
                        }
                        _pyName = pyName;
                    }
                }
            }
        }

        private static void InitPyWords()
        {
            if (_search == null) {
                lock (lockObj) {
                    if (_search == null) {
                        var ass = typeof(WordsHelper).Assembly;
#if NETSTANDARD2_1
                const string resourceName = "ToolGood.Words.dict.pyWords.txt.br";
#else
                        const string resourceName = "ToolGood.Words.dict.pyWords.txt.z";
#endif
                        Stream sm = ass.GetManifestResourceStream(resourceName);
                        byte[] bs = new byte[sm.Length];
                        sm.Read(bs, 0, (int)sm.Length);
                        sm.Close();
                        var bytes = Decompress(bs);
                        var tStr = Encoding.UTF8.GetString(bytes);
                        bytes = null;

                        var lines = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        tStr = null;

                        var wordPy = new List<ushort>();
                        List<string> keywords = new List<string>();
                        List<int> wordPyIndex = new List<int>();
                        wordPyIndex.Add(0);

                        foreach (var line in lines) {
                            var sp = line.Split(',');
                            keywords.Add(sp[0]);
                            for (int i = 1; i < sp.Length; i++) {
                                var idx = sp[i];
                                wordPy.Add(ushort.Parse(idx, System.Globalization.NumberStyles.HexNumber));
                            }
                            wordPyIndex.Add(wordPy.Count);
                        }
                        var search = new WordsSearchEx();
                        search.SetKeywords(keywords);
                        _wordPyIndex = wordPyIndex.ToArray();
                        _wordPy = wordPy.ToArray();
                        _search = search;

                        wordPy = null;
                        keywords = null;
                        wordPyIndex = null;
                    }
                    GC.Collect();
                }
            }
        }


        private static byte[] Decompress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream(data)) {
                    using (var zStream = new ZIPStream(stream, CompressionMode.Decompress)) {
                        using (var resultStream = new MemoryStream()) {
                            zStream.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    }
                }
            } catch {
                return data;
            }
        }
        #endregion
    }
}
