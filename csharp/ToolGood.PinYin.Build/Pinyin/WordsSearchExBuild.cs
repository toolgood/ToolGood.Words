using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.PinYin.Build.Pinyin
{
    public class WordsSearchExBuild : BaseSearchEx
    {

        public void SaveFile(BinaryWriter bw)
        {
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

            //List<int> Index = new List<int>() { 0 };
            //List<ushort> keysList = new List<ushort>();
            //List<int> valuesList = new List<int>();
            //foreach (var dict in _nextIndex) {
            //    var keys = dict.Keys;
            //    var values = dict.Values;
            //    keysList.AddRange(keys);
            //    valuesList.AddRange(values);
            //    Index.Add(keysList.Count);
            //}

            //bs = IntArrToByteArr(Index.ToArray());
            //bw.Write(bs.Length);
            //bw.Write(bs);

            //bs = IntArrToByteArr(keysList.ToArray());
            //bw.Write(bs.Length);
            //bw.Write(bs);


            //bs = IntArrToByteArr(valuesList.ToArray());
            //bw.Write(bs.Length);
            //bw.Write(bs);

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


    }
}
