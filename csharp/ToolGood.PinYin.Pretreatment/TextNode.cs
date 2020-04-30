using System.Collections.Generic;

namespace ToolGood.Pinyin.Pretreatment
{
    partial class Program
    {
        public class TextNode
        {
            public bool IsEnd { get; set; }
            public List<TextLine> Children { get; set; }

            public TextNode()
            {
                Children = new List<TextLine>();
            }

            public List<string> GetFullTextLine()
            {
                List<string> textLines = new List<string>();
                GetFullTextLine(textLines, "");
                return textLines;
            }
            public void GetFullTextLine(List<string> textLines,string pre)
            {
                if (this.IsEnd) {
                    textLines.Add(pre);
                    return;
                }
                foreach (var child in Children) {
                    var p = pre + "," + child.Keyword;
                    child.Next.GetFullTextLine(textLines, p);
                }
            }




        }

    }
}
