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
        public void test3()
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

            all = match.Find("S东");
            Assert.AreEqual("山东", all[0]);

            var all2 = match.FindIndex("BJ");
            Assert.AreEqual(0, all2[0]);
            Assert.AreEqual(1, all.Count);


            all = match.Find("m");
 
        }
    }
}
