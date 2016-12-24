using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;

namespace ToolGood.Words.Test
{
    [TestFixture]
    class IllegalWordsTest
    {
        [Test]
        public void Test()
        {
            string s = "中国|国人|zg人|fuck|all|as|19|http://|ToolGood";
            IllegalWordsSearch iwords = new IllegalWordsSearch(s.Split('|'), 2);

            string test = "我是中国人";
            var all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SearchString);
            Assert.AreEqual("国人", all[1].SearchString);

            test = "我是中国zg人";
            all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SearchString);
            Assert.AreEqual("国zg人", all[1].SearchString);
            Assert.AreEqual("国人", all[1].Keyword);
            Assert.AreEqual("zg人", all[2].SearchString);

            test = "中间国zg人";
            all = iwords.FindAll(test);
            Assert.AreEqual("国zg人", all[0].SearchString);
            Assert.AreEqual("zg人", all[1].SearchString);

            test = "fuck al.l";
            all = iwords.FindAll(test);
            Assert.AreEqual("fuck", all[0].SearchString);
            Assert.AreEqual("al.l", all[1].SearchString);
            Assert.AreEqual(2, all.Count);

            test = "ht@tp://toolgood.com";
            all = iwords.FindAll(test);
            Assert.AreEqual("ht@tp://", all[0].SearchString);
            Assert.AreEqual("http://", all[0].Keyword);
            Assert.AreEqual("toolgood", all[1].SearchString);
            Assert.AreNotEqual("ToolGood", all[1].Keyword);
            Assert.AreEqual("toolgood", all[1].Keyword);
            Assert.AreEqual(2, all.Count);


            test = "asssert all";
            all = iwords.FindAll(test);
            Assert.AreEqual("all", all[0].SearchString);
            Assert.AreEqual(1, all.Count);

            test = "19w 1919 all";
            all = iwords.FindAll(test);
            Assert.AreEqual("19", all[0].SearchString);
            Assert.AreEqual("all", all[1].SearchString);

        }


    }
}
