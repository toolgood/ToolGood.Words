using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.Words.Pinyin.internals
{
    public class WordsSearchResult
    {
        internal WordsSearchResult(  int start, int end, int index,int length)
        {
            End = end;
            Start = start;
            Index = index;
            Length = length;
        }

        /// <summary>
        /// 开始位置
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// 结束位置
        /// </summary>
        public int End { get; private set; }

 

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; private set; }

        public int Length { get; private set; }



    }

}
