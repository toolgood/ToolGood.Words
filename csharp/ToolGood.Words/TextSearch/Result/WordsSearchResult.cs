namespace ToolGood.Words
{
    public class WordsSearchResult
    {
        public WordsSearchResult(string keyword, int start, int end, int index)
        {
            _keyword = keyword;
            End = end;
            Start = start;
            Index = index;
            _matchKeyword = keyword;
        }

        public WordsSearchResult(ref string text, int start, int end, int index)
        {
            _text = text;
            End = end;
            Start = start;
            Index = index;
        }


        public WordsSearchResult(string keyword, int start, int end, int index, string matchKeyword)
        {
            _keyword = keyword;
            End = end;
            Start = start;
            Index = index;
            _matchKeyword = matchKeyword;
        }
        private string _text;
        private string _keyword;
        private string _matchKeyword;

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
        public string Keyword {
            get
            {
                if (_keyword == null) {
                    _keyword = _text.Substring(Start, End + 1 - Start);
                }
                return _keyword;
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 匹配关键字
        /// </summary>
        public string MatchKeyword {
            get
            {
                if (_matchKeyword == null) {
                    if (_keyword == null) {
                        _matchKeyword = _text.Substring(Start, End + 1 - Start);
                    } else {
                        _matchKeyword = _keyword;
                    }
                }
                return _matchKeyword;
            }
        }


        public override string ToString()
        {
            if (MatchKeyword != Keyword) {
                return Start.ToString() + "|" + Keyword + "|" + MatchKeyword;
            }
            return Start.ToString() + "|" + Keyword;
        }
    }
}
