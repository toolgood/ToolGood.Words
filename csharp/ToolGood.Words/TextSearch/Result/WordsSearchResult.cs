namespace ToolGood.Words
{
    public class WordsSearchResult
    {
        internal WordsSearchResult(string keyword, int start, int end, int index)
        {
            Keyword = keyword;
            End = end;
            Start = start;
            Index = index;
            MatchKeyword = keyword;
        }


        internal WordsSearchResult(string keyword, int start, int end, int index, string matchKeyword)
        {
            Keyword = keyword;
            End = end;
            Start = start;
            Index = index;
            MatchKeyword = matchKeyword;
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
        /// 关键字
        /// </summary>
        public string Keyword { get; private set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 匹配关键字
        /// </summary>
        public string MatchKeyword { get; private set; }


        public override string ToString()
        {
            if (MatchKeyword != Keyword)
            {
                return Start.ToString() + "|" + Keyword + "|" + MatchKeyword;
            }
            return Start.ToString() + "|" + Keyword;
        }
    }
}
