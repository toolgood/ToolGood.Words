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
        public void GetPinyin()
        {
            var t = WordsHelper.GetAllPinyin('芃');
            Assert.AreEqual("Peng", t[0]);

            var a = WordsHelper.GetPinyinFast("阿");
            Assert.AreEqual("A", a);


            var b = WordsHelper.GetPinyin("摩擦棒");
            Assert.AreEqual("MoCaBang", b);

            b = WordsHelper.GetPinyin("秘鲁");
            Assert.AreEqual("BiLu", b);



            var py = WordsHelper.GetPinyinFast("我爱中国");
            Assert.AreEqual("WoAiZhongGuo", py);



            py = WordsHelper.GetPinyin("快乐，乐清");
            Assert.AreEqual("KuaiLe，YueQing", py);

            py = WordsHelper.GetPinyin("快乐清理");
            Assert.AreEqual("KuaiLeQingLi", py);


            py = WordsHelper.GetPinyin("我爱中国");
            Assert.AreEqual("WoAiZhongGuo", py);

            py = WordsHelper.GetPinyin("我爱中国",",");
            Assert.AreEqual("Wo,Ai,Zhong,Guo", py);

            py = WordsHelper.GetPinyin("我爱中国",true);
            Assert.AreEqual("WǒÀiZhōngGuó", py);

            py = WordsHelper.GetFirstPinyin("我爱中国");
            Assert.AreEqual("WAZG", py);

            var pys = WordsHelper.GetAllPinyin('传');
            Assert.AreEqual("Chuan", pys[0]);
            Assert.AreEqual("Zhuan", pys[1]);

            py = WordsHelper.GetPinyinForName("单一一");
            Assert.AreEqual("ShanYiYi", py);

            py = WordsHelper.GetPinyinForName("单一一",",");
            Assert.AreEqual("Shan,Yi,Yi", py);

            py = WordsHelper.GetPinyinForName("单一一",true);
            Assert.AreEqual("ShànYīYī", py);

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
            Assert.AreEqual("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分", t);
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
            Assert.AreEqual("壹佰貳拾叄億肆仟伍佰陸拾柒萬捌仟玖佰零壹元壹角貳分", tw);

            var tw2 = WordsHelper.ToTraditionalChinese("原代码11",2);
            Assert.AreEqual("原始碼11", tw2);

            var tw3 = WordsHelper.ToTraditionalChinese("反反复复", 2);
            Assert.AreEqual("反反覆覆", tw3);

            var tw4 = WordsHelper.ToTraditionalChinese("这人考虑事情总是反反复复的", 2);
            Assert.AreEqual("這人考慮事情總是反反覆覆的", tw4);

            var tw5 = WordsHelper.ToTraditionalChinese("计算发现", 2);

            

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
