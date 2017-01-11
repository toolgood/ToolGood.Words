//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ToolGood.Words
//{
//    public class PinYinSearch2
//    {
//        class Position
//        {
//            public int Start { get; set; }
//            public int End { get; set; }
//        }
//        class OneWord
//        {
//            public int Position { get; set; }
//            public string Word { get; set; }
//            public string[] PinYins { get; set; }
//            public bool IsEnd { get; set; }
//            public Position SourcePosition { get; set; }
//            public List<Position> Positions { get; set; }
//        }
//        class Keyword
//        {
//            public string Text { get; set; }
//            public int Index { get; set; }
//            public List<OneWord> Words { get; set; }

//            public Keyword(string text, int index, string[] pys, PinYinSearchType type)
//            {

//            }

//            public void GetKeywords(Dictionary<string, Keyword> dict)
//            {
//                List<string> list = new List<string>() { "" };
//                for (int i = 0; i < Text.Length; i++) {
//                    list = GetKeywords(list, i);
//                }
//                dict[Text] = this;
//                for (int i = 0; i < list.Count; i++) {
//                    dict[list[i]] = this;
//                }
//            }
//            private List<string> GetKeywords(List<string> list, int index)
//            {
//                List<string> newList = new List<string>();
//                var word = Words[index];
//                if (word.PinYins.Length > 0) {
//                    for (int i = 0; i < list.Count; i++) {
//                        for (int j = 0; j < word.PinYins.Length; j++) {
//                            newList.Add(list[i] + word.PinYins[j] + word.Word);
//                        }
//                    }
//                } else {
//                    for (int i = 0; i < list.Count; i++) {
//                        newList.Add(list[i] + word.Word);
//                    }
//                }
//                return newList;
//            }

//        }
//        PinYinSearchType _searchType;
//        List<Keyword> Keywords;
//        Keyword[] _orderBy;//排序后的
//        Keyword[] _first;
//        Dictionary<char, List<OneWord>> dict;


//        public PinYinSearch2(PinYinSearchType type = PinYinSearchType.FirstPinYin)
//        {
//            _searchType = type;
//        }

//        #region MyRegion
//        public void SetKeywords(List<string> keywords)
//        {
//            Keywords = new List<Keyword>();
//            for (int i = 0; i < keywords.Count; i++) {
//                var words = keywords[i];
//                var pys = PinYinDict.GetPinYinList(words);
//                Keywords.Add(new Keyword(words, i, pys, _searchType));
//            }
//            Dictionary<string, Keyword> dict = new Dictionary<string, Keyword>();
//            _orderBy = dict.OrderBy(q => q.Key).Select(q => q.Value).ToArray();




//        } 
//        #endregion






//    }
//}
