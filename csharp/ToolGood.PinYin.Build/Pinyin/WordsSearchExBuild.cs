using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.PinYin.Build.Pinyin
{
    public class WordsSearchExBuild : BaseSearchEx
    {


        public void SaveFile(string file)
        {
            var fs = File.Open(file, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            byte[] _keywordsLengths = new byte[_keywords.Length];
            for (int i = 0; i < _keywordsLengths.Length; i++) {
                _keywordsLengths[i] = (byte)_keywords[i].Length;
            }
            bw.Write(_keywordsLengths.Length);
            bw.Write(_keywordsLengths);


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
                bw.Write(bs);
            }

            bw.Close();
            fs.Close();
        }
    }
}
