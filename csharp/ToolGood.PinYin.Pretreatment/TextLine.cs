using System.Collections.Generic;

namespace ToolGood.Pinyin.Pretreatment
{
    partial class Program
    {
        public class TextLine
        {
            public int Start { get; set; }
            public int End { get; set; }
            public string Keyword { get; set; }

            public List<string> Pinyin { get; set; }

            public TextNode Next { get; set; }

        }

    }
}
