using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class StringSearchEx3Test
    {
        [Test]
        public void test5()
        {
            string s = "中国|国人|zg人";
            string test = "我是中国人";

            StringSearchEx3 iwords2 = new StringSearchEx3();
            iwords2.SetKeywords(s.Split('|'));
            iwords2.Save("StringSearchEx2Test.dat");

            StringSearchEx3 iwords = new StringSearchEx3();
            iwords.Load("StringSearchEx2Test.dat");


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


        }
    }
}
