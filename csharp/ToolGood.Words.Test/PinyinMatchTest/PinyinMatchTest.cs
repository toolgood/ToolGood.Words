using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class PinyinMatchTest
    {
        [Test]
        public void test2()
        {
            string s = "北京|天津|河北|辽宁|吉林|黑龙江|山东|江苏|上海|浙江|安徽|福建|江西|广东|广西|海南|河南|湖南|湖北|山西|内蒙古|宁夏|青海|陕西|甘肃|新疆|四川|贵州|云南|重庆|西藏|香港|澳门|台湾";

            PinyinMatch match = new PinyinMatch();
            match.SetKeywords(s.Split('|').ToList());

            var all = match.Find("BJ");
            Assert.AreEqual("北京", all[0]);
            Assert.AreEqual(1, all.Count);

            all = match.Find("北J");
            Assert.AreEqual("北京", all[0]);
            Assert.AreEqual(1, all.Count);

            all = match.Find("北Ji");
            Assert.AreEqual("北京", all[0]);
            Assert.AreEqual(1, all.Count);

            all = match.Find("S");
            Assert.AreEqual("山东", all[0]);
            Assert.AreEqual("江苏", all[1]);

            all = match.Find("Su");
            Assert.AreEqual("江苏", all[0]);

            all = match.Find("Sdong");
            Assert.AreEqual("山东", all[0]);

            all = match.Find("Sdo");
            Assert.AreEqual("山东", all[0]);

            all = match.Find("S东");
            Assert.AreEqual("山东", all[0]);

            var all2 = match.FindIndex("BJ");
            Assert.AreEqual(0, all2[0]);
            Assert.AreEqual(1, all.Count);

            all = match.FindWithSpace("S 东");
            Assert.AreEqual("山东", all[0]);

            all = match.FindWithSpace("h 江");
            Assert.AreEqual("黑龙江", all[0]);


            all = match.FindWithSpace("京 北");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("黑龙 龙江");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("黑龙 江");
            Assert.AreEqual("黑龙江", all[0]);

            all = match.FindWithSpace("黑 龙 江");
            Assert.AreEqual("黑龙江", all[0]);



        }
        [Test]
        public void test3()
        {
            string s = "黑龙江|黑龙江123";

            PinyinMatch match = new PinyinMatch();
            match.SetKeywords(s.Split('|').ToList());

            var all = match.FindWithSpace("黑龙 龙江");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("江 黑");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("黑龙 江");
            Assert.AreEqual("黑龙江", all[0]);

            all = match.FindWithSpace("黑 龙 江");
            Assert.AreEqual("黑龙江", all[0]);

            var all2 = match.FindIndexWithSpace("黑龙 江");
            Assert.AreEqual(0, all2[0]);

        }
        [Test]
        public void test4()
        {
            string s = "北京|天津|河北|辽宁|吉林|黑龙江|山东|江苏|上海|浙江|安徽|福建|江西|广东|广西|海南|河南|湖南|湖北|山西|内蒙古|宁夏|青海|陕西|甘肃|新疆|四川|贵州|云南|重庆|西藏|香港|澳门|台湾";

            var match = new PinyinMatch<string>(s.Split('|'));
            match.SetKeywordsFunc(q => q);

            var all = match.Find("BJ");
            Assert.AreEqual("北京", all[0]);
            Assert.AreEqual(1, all.Count);

            all = match.Find("北J");
            Assert.AreEqual("北京", all[0]);
            Assert.AreEqual(1, all.Count);

            all = match.Find("北Ji");
            Assert.AreEqual("北京", all[0]);
            Assert.AreEqual(1, all.Count);

            all = match.Find("S");
            Assert.AreEqual("山东", all[0]);
            Assert.AreEqual("江苏", all[1]);

            all = match.Find("Su");
            Assert.AreEqual("江苏", all[0]);

            all = match.Find("Sdong");
            Assert.AreEqual("山东", all[0]);

            all = match.Find("S东");
            Assert.AreEqual("山东", all[0]);

            all = match.FindWithSpace("S 东");
            Assert.AreEqual("山东", all[0]);

            all = match.FindWithSpace("h 江");
            Assert.AreEqual("黑龙江", all[0]);


            all = match.FindWithSpace("京 北");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("黑龙 龙江");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("黑龙 江");
            Assert.AreEqual("黑龙江", all[0]);

            all = match.FindWithSpace("黑 龙 江");
            Assert.AreEqual("黑龙江", all[0]);

        }
        [Test]
        public void test5()
        {
            string s = "黑龙江|黑龙江123";

            var match = new PinyinMatch<string>(s.Split('|'));
            match.SetKeywordsFunc(q => q);

            var all = match.FindWithSpace("黑龙 龙江");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("江 黑");
            Assert.AreEqual(0, all.Count);

            all = match.FindWithSpace("黑龙 江");
            Assert.AreEqual("黑龙江", all[0]);

            all = match.FindWithSpace("黑 龙 江");
            Assert.AreEqual("黑龙江", all[0]);

        }

    }
}
