using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ToolGood.Bedrock;
using ToolGood.Words.ReferenceHelper;

namespace ToolGood.PinYin.Build.Pinyin
{
    public class PinyinDictBuild
    {
        private Dictionary<string, ushort[]> _pyName;
        private string[] _pyShow;
        private ushort[] _pyIndex;
        private ushort[] _pyData;
        private int[] _wordPyIndex;
        private ushort[] _wordPy;
        private WordsSearchExBuild _search;

        public void InitPyFile(string pyfile, string pyName)
        {
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(new List<string>() { pyfile });
            pinyinBuild.RemoveChar();
            var pyShow = pinyinBuild.GetPinyins();
            var dict = pinyinBuild.BuildPinyinDict(pyShow);
            var outPy = pinyinBuild.BuildSingleWord(pyShow, dict);
            var words = pinyinBuild.BuildMiniWords(pyShow, dict);
            //_pyShow = pyShow.ToArray();

            var ls = new List<string>();
            foreach (var item in words) {
                var str = item;
                List<int> pys = dict[str];
                foreach (var py in pys) {
                    str += "," + py.ToString("X");
                }
                ls.Add(str);
            }
            ls = ls.OrderBy(q => q).ToList();
            var pyWords = string.Join("\n", ls);

            PinyinNameBuild pinyinNameBuild = new PinyinNameBuild();
            var mDict = pinyinNameBuild.LoadPinyinName(pyName);
            var ps = pinyinNameBuild.BuildPinyinDict(pyShow, mDict);


            List<string> ls2 = new List<string>();
            foreach (var item in ps) {
                List<string> idxs = new List<string>();
                foreach (var index in item.Value) {
                    idxs.Add(index.ToString("X"));
                }
                ls2.Add($"{item.Key},{string.Join(",", idxs)}");
            }
            var outPyName = string.Join("\n", ls2);

            InitPyIndex(outPy);
            InitPyName(outPyName);
            InitPyWords(pyWords);
        }

 

        public void WriteGzip(string file)
        {
            var bytes = WritePinyinDat();
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllBytes(file, CompressionUtil.GzipCompress(bytes));
        }

        public void WriteBr(string file)
        {
            var bytes = WritePinyinDat();
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllBytes(file, CompressionUtil.BrCompress(bytes));
        }


        private byte[] WritePinyinDat()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

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
            return ms.ToArray();
        }


        private byte[] IntArrToByteArr(Int32[] intArr)
        {
            Int32 intSize = sizeof(Int32) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        private byte[] IntArrToByteArr(ushort[] intArr)
        {
            Int32 intSize = sizeof(ushort) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }


        #region private
        private void InitPyIndex(string tStr)
        {
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
            _pyData = pyData.ToArray();
            _pyIndex = pyIndex.ToArray();
            pyIndex = null;
            pyData = null;
        }

        private void InitPyName(string tStr)
        {
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

        private void InitPyWords(string tStr)
        {
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
            var search = new WordsSearchExBuild();
            search.SetKeywords(keywords);
            _wordPyIndex = wordPyIndex.ToArray();
            _wordPy = wordPy.ToArray();
            _search = search;

            wordPy = null;
            keywords = null;
            wordPyIndex = null;
        }
        #endregion



    }
}
