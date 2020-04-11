using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class StringSearchExTest
    {
        [Test]
        public void test()
        {
            string s = "中国|国人|zg人";
            string test = "我是中国人";

            StringSearchEx iwords2 = new StringSearchEx();
            iwords2.SetKeywords(s.Split('|'));
            iwords2.Save("BigStringSearchEx.dat");

            StringSearchEx iwords = new StringSearchEx();
            iwords.Load("BigStringSearchEx.dat");

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

            StringSearchEx iwords = new StringSearchEx();
            iwords.SetKeywords(s.Split('|'));



            var all = iwords.FindAll(test);

            Assert.AreEqual(6, all.Count);

            var str = iwords.Replace(test, '*');
            Assert.AreEqual("*****", str);


        }

    }
}
