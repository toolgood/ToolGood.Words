using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
//using ToolGood.Words.TextSearch;

namespace ToolGood.Words.Test
{
    [TestFixture]
    class IllegalWordsTest
    {

        [Test]
        public void IllegalWordsSearchTest()
        {
            string s = "中国|国人|zg人|fuck|all|as|19|http://|ToolGood|assert|zgasser";
            int[] bl = new int[] {7, 4, 7, 7, 7, 7, 7, 7, 7, 7, 7};
            string test = "我是中国人";


            var iwords = new IllegalWordsSearch();
            iwords.SetKeywords(s.Split('|'));


            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);


            var f = iwords.FindFirst(test);
            Assert.AreEqual(true, f.Success);
            Assert.AreEqual("中国", f.Keyword);
            Assert.AreEqual(2, f.Start);
            Assert.AreEqual(3, f.End);



            var all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SrcString);
            Assert.AreEqual("国人", all[1].SrcString);

            test = "我是中国zg人";
            all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SrcString);
            Assert.AreEqual("zg人", all[1].SrcString);

            test = "中间国zg人";
            all = iwords.FindAll(test);
            Assert.AreEqual("zg人", all[0].SrcString);

            test = "fuck al[]l";//未启用跳词
            all = iwords.FindAll(test);
            Assert.AreEqual("fuck", all[0].SrcString);
            Assert.AreEqual(1, all.Count);


            test = "fuck al[]l";
            iwords.UseSkipWordFilter = true;//启用跳词
            all = iwords.FindAll(test);
            Assert.AreEqual("fuck", all[0].SrcString);
            Assert.AreEqual("al[]l", all[1].SrcString);
            Assert.AreEqual(2, all.Count);

            test = "http://ToolGood.com";
            all = iwords.FindAll(test);
            Assert.AreEqual("toolgood", all[0].Keyword);//关键字ToolGood默认转小写
            Assert.AreEqual("ToolGood", all[0].SrcString);
            Assert.AreEqual(1, all.Count);

            test = "asssert all";
            all = iwords.FindAll(test);//未启用重复词
            Assert.AreEqual("all", all[0].SrcString);
            Assert.AreEqual(1, all.Count);

            test = "asssert all";
            iwords.UseDuplicateWordFilter = true;//启用重复词
            all = iwords.FindAll(test);
            Assert.AreEqual("asssert", all[0].SrcString);
            Assert.AreEqual("assert", all[0].Keyword);
            Assert.AreEqual("all", all[1].SrcString);
            Assert.AreEqual(2, all.Count);

            test = "asssert allll";//重复词匹配到末尾
            all = iwords.FindAll(test);
            Assert.AreEqual("asssert", all[0].SrcString);
            Assert.AreEqual("assert", all[0].Keyword);
            Assert.AreEqual("allll", all[1].SrcString);
            Assert.AreEqual(2, all.Count);

            test = "zgasssert aallll";//不会匹配zgasser 或 assert
            all = iwords.FindAll(test);
            Assert.AreEqual("aallll", all[0].SrcString);
            Assert.AreEqual("all", all[0].Keyword);
            Assert.AreEqual(1, all.Count);

            test = "我是【中]国【人";
            all = iwords.FindAll(test);
            Assert.AreEqual("中]国", all[0].SrcString);
            Assert.AreEqual("国【人", all[1].SrcString);

            test = "我是【中国【人";
            all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SrcString);
            Assert.AreEqual("国【人", all[1].SrcString);
            Assert.AreEqual(2, all.Count);


            var ss = iwords.Replace(test, '*');
            Assert.AreEqual("我是【****", ss);

            test = "我是中国人"; //使用黑名单
            iwords.SetBlacklist(bl);
            iwords.UseBlacklistFilter = true;
            all = iwords.FindAll(test,1);
            Assert.AreEqual("中国", all[0].SrcString);
            Assert.AreEqual(1, all.Count);

        }

        [Test]
        public void NumberTypoSearchTest()
        {
            string s = "123456|778899|11";
            string test = "123456789";

            NumberTypoSearch search = new NumberTypoSearch();
            search.SetKeywords(s.Split('|'));

            var all = search.FindAll("依依");
            Assert.AreEqual("11", all[0].Keyword);
            Assert.AreEqual("依依", all[0].SrcString);
            Assert.AreEqual(1, all.Count);

        }

        [Test]
        public void StringTypoSearchTest()
        {
            string s = "http://fanyi.baidu.com/|778899|11";

            StringTypoSearch search = new StringTypoSearch();
            search.SetKeywords(s.Split('|'));

            var all = search.FindAll(" http://fanyi删.bai除du.com/");
            Assert.AreEqual("http://fanyi.baidu.com/", all[0].Keyword);
            Assert.AreEqual(1, all.Count);


        }
    }
}
