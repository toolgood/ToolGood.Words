using System;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.Words.internals
{
    class MiniSearchResult
    {
        public MiniSearchResult(string keyword, int start, int end)
        {
            Keyword = keyword;
            End = end;
            Start = start;
        }
        public int Start { get; private set; }
        public int End { get; private set; }
        public string Keyword { get; private set; }
    }
}
