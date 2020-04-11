namespace ToolGood.Words
{
    public class IllegalWordsSearchResult
    {
        internal IllegalWordsSearchResult(string keyword, int start, int end, string srcText)
        {
            MatchKeyword = keyword;
            End = end;
            Start = start;
            Keyword = srcText.Substring(Start, end - Start + 1);
            BlacklistType = BlacklistType.None;
        }
        internal IllegalWordsSearchResult(string keyword, int start, int end, string srcText, BlacklistType type)
        {
            MatchKeyword = keyword;
            End = end;
            Start = start;
            Keyword = srcText.Substring(Start, end - Start + 1);
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
        /// 关键字
        /// </summary>
        public string MatchKeyword { get; private set; }

        /// <summary>
        /// 黑名单类型
        /// </summary>
        public BlacklistType BlacklistType { get; private set; }


        public override string ToString()
        {
            if (Keyword != MatchKeyword) {
                return Start.ToString() + "|" + Keyword + "|" + MatchKeyword;
            }
            return Start.ToString() + "|" + Keyword;
        }
    }
}
