using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ToolGood.Words;
using ZIPStream = System.IO.Compression.BrotliStream;



namespace ToolGood.PinYin.Build.Pinyin
{
    public static class PinyinDictBuild
    {
        private static Dictionary<string, ushort[]> _pyName;
        private static string[] _pyShow;
        private static ushort[] _pyIndex;
        private static ushort[] _pyData;
        private static ushort[][] _pyIndex2;
        private static ushort[][] _pyData2;
        private static int[] _wordPyIndex;
        private static ushort[] _wordPy;
        private static WordsSearchExBuild _search;

        public static void InitPy()
        {
            InitPyIndex();
            InitPyName();
            InitPyWords();
        }

        public static void WritePinyinDat()
        {
            var fs = File.Open("Pinyin.dat", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(_pyName.Count);
            foreach (var item in _pyName) {
                bw.Write(item.Key);
                var bs2 = IntArrToByteArr(item.Value);
                bw.Write(bs2.Length);
                bw.Write(bs2);
            }
            bw.Write(_pyShow.Length);
            foreach (var item in _pyShow) {
                bw.Write(item);
            }

            var bt = IntArrToByteArr(_pyIndex);
            bw.Write(bt.Length);
            bw.Write(bt);

            bt = IntArrToByteArr(_pyData);
            bw.Write(bt.Length);
            bw.Write(bt);

            bt = IntArrToByteArr(_wordPyIndex);
            bw.Write(bt.Length);
            bw.Write(bt);

            bt = IntArrToByteArr(_wordPy);
            bw.Write(bt.Length);
            bw.Write(bt);

            _search.SaveFile(bw);

            bw.Close();
            fs.Close();
        }

        public static void WritePinyinBigDat()
        {
            var fs = File.Open("PinyinBig.dat", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);


            bw.Write(_pyName.Count);
            foreach (var item in _pyName) {
                bw.Write(item.Key);
                var bs2 = IntArrToByteArr(item.Value);
                bw.Write(bs2.Length);
                bw.Write(bs2);
            }
            bw.Write(_pyShow.Length);
            foreach (var item in _pyShow) {
                bw.Write(item);
            }

            var bt = IntArrToByteArr(_pyIndex);
            bw.Write(bt.Length);
            bw.Write(bt);

            bt = IntArrToByteArr(_pyData);
            bw.Write(bt.Length);
            bw.Write(bt);

            //-------------------------
            bw.Write(_pyIndex2.Length);
            for (int i = 0; i < _pyIndex2.Length; i++) {
                bt = IntArrToByteArr(_pyIndex2[i]);
                bw.Write(bt.Length);
                bw.Write(bt);
            }
            bw.Write(_pyData2.Length);
            for (int i = 0; i < _pyData2.Length; i++) {
                bt = IntArrToByteArr(_pyData2[i]);
                bw.Write(bt.Length);
                bw.Write(bt);
            }
            //-------------------------
            bt = IntArrToByteArr(_wordPyIndex);
            bw.Write(bt.Length);
            bw.Write(bt);

            bt = IntArrToByteArr(_wordPy);
            bw.Write(bt.Length);
            bw.Write(bt);

            _search.SaveFile(bw);

            bw.Close();
            fs.Close();
        }

        public static void WritePinyinWordDat()
        {
            //_search.SaveFile("PinyinWord.dat");
        }

        private static byte[] IntArrToByteArr(Int32[] intArr)
        {
            Int32 intSize = sizeof(Int32) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        private static byte[] IntArrToByteArr(ushort[] intArr)
        {
            Int32 intSize = sizeof(ushort) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }


        #region private
        private static object lockObj = new object();
        private static void InitPyIndex()
        {
            if (_pyIndex2 == null) {
                lock (lockObj) {
                    if (_pyIndex2 == null) {
                        {
                            byte[] bs = File.ReadAllBytes("dict\\pyIndex.txt.br");
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
                            byte[] bs = File.ReadAllBytes("dict\\pyIndex2.txt.br");
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
                        byte[] bs = File.ReadAllBytes("dict\\pyName.txt.br");
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
                        byte[] bs = File.ReadAllBytes("dict\\pyWords.txt.br");

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
                        var search = new WordsSearchExBuild();
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
