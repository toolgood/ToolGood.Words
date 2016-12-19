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
        public void HasChinese()
        {
            var b = WordHelper.HasChinese("xhdsf");
            Assert.AreEqual(false, b);

            var c = WordHelper.HasChinese("我爱中国");
            Assert.AreEqual(true, c);

            var d = WordHelper.HasChinese("I爱中国");
            Assert.AreEqual(true, d);
        }
        [Test]
        public void ToChineseRMB()
        {
            var t = WordHelper.ToChineseRMB(12345678901.12);
            Assert.AreEqual("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分", t);
        }
        [Test]
        public void ToSimplifiedChinese()
        {
            var tw = WordHelper.ToSimplifiedChinese("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");

            Assert.AreEqual("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分", tw);
        }
        [Test]
        public void ToSBC_ToDBC()
        {
            var s = WordHelper.ToSBC("abcABC123");
            var t= WordHelper.ToDBC(s);
            Assert.AreEqual("ａｂｃＡＢＣ１２３", s);
            Assert.AreEqual("abcABC123", t);


        }

    }
}
