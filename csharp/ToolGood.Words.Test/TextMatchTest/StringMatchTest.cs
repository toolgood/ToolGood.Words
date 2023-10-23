using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class StringMatchTest
    {
        [Test]
        public void test()
        {
            string s = "aaaaa|BBBB|CCCC|中国|国人|zg人";
            string test = "我是中国人";

            StringMatch iwords = new StringMatch();
            iwords.SetKeywords(s.Split('|'));

            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = iwords.FindFirst(test);
            Assert.AreEqual("中国", f);



            var all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0]);
            Assert.AreEqual("国人", all[1]);
            Assert.AreEqual(2, all.Count);

            var str = iwords.Replace(test, '*');
            Assert.AreEqual("我是***", str);


            string s2 = "ABCDEFG|BCDEF|BCDEG|CDEK|CDE";
            StringSearch stringSearch = new StringSearch();
            stringSearch.SetKeywords(s2.Split('|'));
            all = stringSearch.FindAll("AAAAAABCDEKDE");
            Assert.AreEqual("CDE", all[0]);
            Assert.AreEqual("CDEK", all[1]);
            Assert.AreEqual(2, all.Count);

        }
        [Test]
        public void test2()
        {
            string s = "中国人|中国|国人|zg人|我是中国人|我是中国|是中国人";
            string test = "我是中国人";

            StringMatch iwords = new StringMatch();
            iwords.SetKeywords(s.Split('|'));


            var all = iwords.FindAll(test);

            Assert.AreEqual(6, all.Count);

            var str = iwords.Replace(test, '*');
            Assert.AreEqual("*****", str);
        }
        [Test]
        public void test3()
        {
            string s = ".中国|国人|zg人";
            string test = "我是中国人";

            StringMatch iwords = new StringMatch();
            iwords.SetKeywords(s.Split('|'));

            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = iwords.FindFirst(test);
            Assert.AreEqual("是中国", f);



            var all = iwords.FindAll(test);
            Assert.AreEqual("是中国", all[0]);
            Assert.AreEqual("国人", all[1]);
            Assert.AreEqual(2, all.Count);

            var str = iwords.Replace(test, '*');
            Assert.AreEqual("我****", str);
        }


        [Test]
        public void test4()
        {
            string s = "[ab][cd]";
            string test = "kac";

            StringMatch iwords = new StringMatch();
            iwords.SetKeywords(s.Split('|'));



            var f = iwords.FindFirst(test);
            Assert.AreEqual("ac", f);

            f = iwords.FindFirst("ad");
            Assert.AreEqual("ad", f);

            f = iwords.FindFirst("bc");
            Assert.AreEqual("bc", f);

            f = iwords.FindFirst("bd");
            Assert.AreEqual("bd", f);

        }


    }
}
