namespace ToolGood.Words
{
    public class IllegalWordsSearchResult
    {

        internal IllegalWordsSearchResult(string keyword, int start, int end, int index, string matchKeyword, int type)
        {
            MatchKeyword = matchKeyword;
            End = end;
            Start = start;
            Index = index;
            Keyword = keyword;
            BlacklistType = type;
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
        /// 原始文本
        /// </summary>
        public string Keyword { get; private set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string MatchKeyword { get; private set; }

        /// <summary>
        /// 黑名单类型
        /// </summary>
        public int BlacklistType { get; private set; }


        public override string ToString()
        {
            if (Keyword != MatchKeyword) {
                return Start.ToString() + "|" + Keyword + "|" + MatchKeyword;
            }
            return Start.ToString() + "|" + Keyword;
        }
    }
}
