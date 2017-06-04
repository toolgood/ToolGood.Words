using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using ToolGood.Words;

namespace ToolGood.Words.Test
{
    [TestFixture]
    class WordHelperTest
    {
        [Test]
        public void GetPinYin()
        {
            var a = WordsHelper.GetPinYinFast("阿");
            Assert.AreEqual("A", a);


            var b = WordsHelper.GetPinYin("摩擦棒");
            Assert.AreEqual("MoCaBang", b);

            b = WordsHelper.GetPinYin("秘鲁");
            Assert.AreEqual("BiLu", b);

            

            var py = WordsHelper.GetPinYinFast("我爱中国");
            Assert.AreEqual("WoAiZhongGuo", py);



            py = WordsHelper.GetPinYin("快乐，乐清");
            Assert.AreEqual("KuaiLe，YueQing", py);

            py = WordsHelper.GetPinYin("我爱中国");
            Assert.AreEqual("WoAiZhongGuo", py);

            py = WordsHelper.GetFirstPinYin("我爱中国");
            Assert.AreEqual("WAZG", py);

            var pys = WordsHelper.GetAllPinYin('传');
            Assert.AreEqual("Chuan", pys[0]);
            Assert.AreEqual("Zhuan", pys[1]);


        }


        [Test]
        public void HasChinese()
        {
            var b = WordsHelper.HasChinese("xhdsf");
            Assert.AreEqual(false, b);

            var c = WordsHelper.HasChinese("我爱中国");
            Assert.AreEqual(true, c);

            var d = WordsHelper.HasChinese("I爱中国");
            Assert.AreEqual(true, d);
        }
        [Test]
        public void ToChineseRMB()
        {
            var t = WordsHelper.ToChineseRMB(12345678901.12);
            Assert.AreEqual("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分", t);
        }
        [Test]
        public void ToNumber()
        {
            var t = WordsHelper.ToNumber("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");
            Assert.AreEqual((decimal)12345678901.12, t);
        }

        [Test]
        public void ToSimplifiedChinese()
        {
            var tw = WordsHelper.ToSimplifiedChinese("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");

            Assert.AreEqual("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分", tw);
        }
        [Test]
        public void ToTraditionalChinese()
        {
            var tw = WordsHelper.ToTraditionalChinese("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
            Assert.AreEqual("壹佰貳拾叁億肆仟伍佰陸拾柒萬捌仟玖佰零壹元壹角貳分", tw);
        }


        [Test]
        public void ToSBC_ToDBC()
        {
            var s = WordsHelper.ToSBC("abcABC123");
            var t = WordsHelper.ToDBC(s);
            Assert.AreEqual("ａｂｃＡＢＣ１２３", s);
            Assert.AreEqual("abcABC123", t);


        }

    }
}
