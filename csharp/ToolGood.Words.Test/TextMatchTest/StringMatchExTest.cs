using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class StringMatchExTest
    {
        [Test]
        public void test()
        {
            string s = "aaaaa|BBBB|CCCC|中国|国人|zg人";
            string test = "我是中国人";

            StringMatchEx iwords = new StringMatchEx();
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

            StringMatchEx iwords = new StringMatchEx();
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

            StringMatchEx iwords = new StringMatchEx();
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
            string s = "...中国|国人|zg人";
            string test = "我是中国人";

            StringMatchEx iwords = new StringMatchEx();
            iwords.SetKeywords(s.Split('|'));

            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);

            var f = iwords.FindFirst(test);
            Assert.AreEqual("国人", f);

            var all = iwords.FindAll(test);
            Assert.AreEqual("国人", all[0]);
            Assert.AreEqual(1, all.Count);

            var str = iwords.Replace(test, '*');
            Assert.AreEqual("我是中**", str);

            test = "sky是中国人";
            f = iwords.FindFirst(test);
            Assert.AreEqual("ky是中国", f);
        }



    }
}
