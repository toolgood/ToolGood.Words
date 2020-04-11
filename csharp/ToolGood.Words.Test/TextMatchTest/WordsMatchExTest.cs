using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class WordsMatchExTest
    {
        [Test]
        public void test()
        {
            string s = "aaaaa|BBBB|CCCC|中国|国人|zg人";
            string test = "我是中国人";

            WordsMatchEx wordsSearch2 = new WordsMatchEx();
            wordsSearch2.SetKeywords(s.Split('|'));
            wordsSearch2.Save("WordsMatchEx.dat");

            WordsMatchEx wordsSearch = new WordsMatchEx();
            wordsSearch.Load("WordsMatchEx.dat");

            var b = wordsSearch.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = wordsSearch.FindFirst(test);
            Assert.AreEqual("中国", f.Keyword);

            var alls = wordsSearch.FindAll(test);
            Assert.AreEqual("中国", alls[0].Keyword);
            Assert.AreEqual(2, alls[0].Start);
            Assert.AreEqual(3, alls[0].End);
            Assert.AreEqual(3, alls[0].Index);//返回索引Index,默认从0开始
            Assert.AreEqual("国人", alls[1].Keyword);
            Assert.AreEqual(2, alls.Count);

            var t = wordsSearch.Replace(test, '*');
            Assert.AreEqual("我是***", t);


        }


        [Test]
        public void test2()
        {
            string s = "中国|国人|zg人";
            string test = "我是中国人";

            WordsMatchEx wordsSearch = new WordsMatchEx();
            wordsSearch.SetKeywords(s.Split('|').ToList());

            var b = wordsSearch.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = wordsSearch.FindFirst(test);
            Assert.AreEqual("中国", f.Keyword);

            var alls = wordsSearch.FindAll(test);
            Assert.AreEqual("中国", alls[0].Keyword);
            Assert.AreEqual(2, alls[0].Start);
            Assert.AreEqual(3, alls[0].End);
            Assert.AreEqual(0, alls[0].Index);//返回索引Index,默认从0开始
            Assert.AreEqual("国人", alls[1].Keyword);
            Assert.AreEqual(2, alls.Count);

            var t = wordsSearch.Replace(test, '*');
            Assert.AreEqual("我是***", t);


        }
        [Test]
        public void test3()
        {
            string s = ".[中美]国|国人|zg人";
            string test = "我是中国人";

            WordsMatchEx wordsSearch = new WordsMatchEx();
            wordsSearch.SetKeywords(s.Split('|'));

            var b = wordsSearch.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = wordsSearch.FindFirst(test);
            Assert.AreEqual("是中国", f.Keyword);

            var alls = wordsSearch.FindAll(test);
            Assert.AreEqual("是中国", alls[0].Keyword);
            Assert.AreEqual(".[中美]国", alls[0].MatchKeyword);
            Assert.AreEqual(1, alls[0].Start);
            Assert.AreEqual(3, alls[0].End);
            Assert.AreEqual(0, alls[0].Index);//返回索引Index,默认从0开始
            Assert.AreEqual("国人", alls[1].Keyword);
            Assert.AreEqual(2, alls.Count);

            var t = wordsSearch.Replace(test, '*');
            Assert.AreEqual("我****", t);


            test = "我是美国人";

            b = wordsSearch.ContainsAny(test);
            Assert.AreEqual(true, b);

            f = wordsSearch.FindFirst(test);
            Assert.AreEqual("是美国", f.Keyword);

            alls = wordsSearch.FindAll(test);
            Assert.AreEqual("是美国", alls[0].Keyword);
            Assert.AreEqual(".[中美]国", alls[0].MatchKeyword);
            Assert.AreEqual(1, alls[0].Start);
            Assert.AreEqual(3, alls[0].End);
            Assert.AreEqual(0, alls[0].Index);//返回索引Index,默认从0开始
            Assert.AreEqual("国人", alls[1].Keyword);
            Assert.AreEqual(2, alls.Count);

            t = wordsSearch.Replace(test, '*');
            Assert.AreEqual("我****", t);

        }


    }
}
