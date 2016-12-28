using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;

namespace ToolGood.Words.Test
{
    [TestFixture]
    class WordsSearchTest
    {
        [Test]
        public void test()
        {
            string s = "中国|国人|zg人";
            string test = "我是中国人";

            WordsSearch iwords = new WordsSearch();
            iwords.SetKeywords(s.Split('|'));

            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = iwords.FindFirst(test);
            Assert.AreEqual("中国", f.Keyword);

            var alls = iwords.FindAll(test);
            Assert.AreEqual("中国", alls[0].Keyword);
            Assert.AreEqual(2, alls[0].Start);
            Assert.AreEqual(3, alls[0].End);
            Assert.AreEqual(0, alls[0].Index);//返回索引Index,默认从0开始
            Assert.AreEqual("国人", alls[1].Keyword);
            Assert.AreEqual(2, alls.Count);

        }

    }
}
