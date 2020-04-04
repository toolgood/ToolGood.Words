using System.Collections.Generic;

namespace ToolGood.PinYin.Pretreatment
{
    partial class Program
    {
        public class TextLine
        {
            public int Start { get; set; }
            public int End { get; set; }
            public string Keyword { get; set; }

            public List<string> PinYin { get; set; }

            public TextNode Next { get; set; }

        }

    }
}
