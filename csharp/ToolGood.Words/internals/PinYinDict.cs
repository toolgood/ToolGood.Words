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
        private static int[] _wordPyIndex;
        private static ushort[] _wordPy;
        private static WordsSearch _search;
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

            string[] list = new string[text.Length];
            var pos = _search.FindAll(text);
            var pindex = -1;
            foreach (var p in pos) {
                if (p.Start > pindex) {
                    for (int i = 0; i < p.Keyword.Length; i++) {
                        list[i + p.Start] = _pyShow[_wordPy[_wordPyIndex[p.Index] + i] + tone];
                    }
                    pindex = p.End;
                }
            }
            for (int i = 0; i < text.Length; i++) {
                if (list[i] != null) continue;
                var c = text[i];
                if (c >= 0x3400 && c <= 0x9fd5) {
                    var index = c - 0x3400;
                    var start = _pyIndex[index];
                    var end = _pyIndex[index + 1];
                    if (end > start) {
                        list[i] = _pyShow[_pyData[start] + tone];
                    }
                } else {
                    list[i] = text[i].ToString();
                }
            }
            return list.ToArray();
        }

        public static string GetFirstPinyin(string text, int tone = 0)
        {
            InitPyIndex();

            string[] list = GetPinyinList(text, tone);
            StringBuilder sb = new StringBuilder(text);
            for (int i = 0; i < list.Length; i++) {
                var c = list[i];
                if (c != null) {
                    sb[i] = c[0];
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

        private static void InitPyIndex()
        {
            if (_pyIndex == null) {
                var ass = typeof(WordsHelper).Assembly;
#if NETSTANDARD2_1
                var resourceName = "ToolGood.Words.dict.pyIndex.txt.br";
#else
                var resourceName = "ToolGood.Words.dict.pyIndex.txt.z";
#endif
                Stream sm = ass.GetManifestResourceStream(resourceName);
                byte[] bs = new byte[sm.Length];
                sm.Read(bs, 0, (int)sm.Length);
                sm.Close();
                var bytes = Decompress(bs);
                var tStr = Encoding.UTF8.GetString(bytes);

                var sp = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

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
                //foreach (var idxs in sp) {
                //    if (idxs != "0") {
                //        foreach (var idx in idxs.Split(',')) {
                //            pyData.Add(ushort.Parse(idx, System.Globalization.NumberStyles.HexNumber));
                //        }
                //    }
                //    pyIndex.Add((ushort)pyData.Count);
                //}
                _pyData = pyData.ToArray();
                _pyIndex = pyIndex.ToArray();
            }
        }

        private static void InitPyName()
        {
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

        private static void InitPyWords()
        {
            if (_search == null) {
                var ass = typeof(WordsHelper).Assembly;
#if NETSTANDARD2_1
                var resourceName = "ToolGood.Words.dict.pyWords.txt.br";
#else
                var resourceName = "ToolGood.Words.dict.pyWords.txt.z";
#endif
                Stream sm = ass.GetManifestResourceStream(resourceName);
                byte[] bs = new byte[sm.Length];
                sm.Read(bs, 0, (int)sm.Length);
                sm.Close();
                var bytes = Decompress(bs);
                var tStr = Encoding.UTF8.GetString(bytes);

                var lines = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
                var search = new WordsSearch();
                search.SetKeywords(keywords);
                _wordPyIndex = wordPyIndex.ToArray();
                _wordPy = wordPy.ToArray();
                _search = search;
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
