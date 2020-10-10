using PetaTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    [TestFixture]
    public class IssuesTest
    {
        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/17
        /// </summary>
        [Test]
        public void IssuesTest_17()
        {
            var illegalWordsSearch = new IllegalWordsSearch();
            string s = "中国|zg人|abc";
            illegalWordsSearch.SetKeywords(s.Split('|'));
            var str = illegalWordsSearch.Replace("我是中美国人厉害中国完美ａｂｃddb好的", '*');

            //Assert.AreEqual("我是中美国人厉害**完美***ddb好的", str);
            //注，ａｂｃ先转abc,再判断abc左右是否为英文或数字,因为后面为d是英文，所以不能过滤
            Assert.AreEqual("我是中美国人厉害**完美ａｂｃddb好的", str);
        }


        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/20
        /// </summary>
        [Test]
        public void IssuesTest_20()
        {
            string text = "A10021003吃饭";
            var keywords = new string[] { "1", "A", "2", "0", "吃" };
            var iws = new IllegalWordsSearch();
            var ss = new StringSearch();
            var sse = new StringSearchEx2();

            iws.SetKeywords(keywords);
            iws.UseIgnoreCase = true;
            iws.UseDBCcaseConverter = true;
            var iwsFirst = iws.FindFirst(text);
            Assert.AreEqual("吃", iwsFirst.Keyword);
            var iwsAll = iws.FindAll(text);
            Assert.AreEqual(1, iwsAll.Count);// 因为1A20左右都是英文或数字，所以识别失败

            ss.SetKeywords(keywords);
            var ssFirst = ss.FindFirst(text);
            Assert.AreEqual("A", ssFirst);
            var ssAll = ss.FindAll(text);
            Assert.AreEqual(9, ssAll.Count);

            sse.SetKeywords(keywords);
            var sseFirst = sse.FindFirst(text);
            Assert.AreEqual("A", sseFirst);
            var sseAll = sse.FindAll(text);
            Assert.AreEqual(9, sseAll.Count);

        }

        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/36
        /// </summary>
        [Test]
        public void IssuesTest_36()
        {
            StringSearch iwords = new StringSearch();
            // 此处有4000个A
            var words = new List<string>() { "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" };
            iwords.SetKeywords(words);
        }

        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/56
        /// </summary>
        [Test]
        public void IssuesTest_56()
        {
            var keywords = new string[] { "我爱中国", "中国", };
            var txt = "新型财富密码就是大喊“我[爱中]国”么？伏拉夫，轻松拥有千万粉丝的新晋网红，快手粉丝465万，抖音粉丝704万。他是靠“爱中国”火起来的。伏拉夫在短视频平台上的简介是：爱中国！爱火锅！";

            var iws = new IllegalWordsSearch();
            iws.SetKeywords(keywords);
            iws.SetSkipWords("]");

            var ts = iws.FindAll(txt);
            Assert.AreEqual(3, ts.Count);
            Assert.AreEqual("中]国", ts[0].Keyword);

        }

        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/57
        /// </summary>
        [Test]
        public void IssuesTest_57()
        {
            String test = "一,二二,三三三,四四四四,五五五五五,六六六六六六";
            List<String> list = new List<String>();
            list.Add("一");
            list.Add("二二");
            list.Add("三三三");
            list.Add("四四四四");
            list.Add("五五五五五");
            list.Add("六六六六六六");

            IllegalWordsSearch iwords = new IllegalWordsSearch();
            iwords.SetKeywords(list);

            bool b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);


            IllegalWordsSearchResult f = iwords.FindFirst(test);
            Assert.AreEqual("一", f.Keyword);

            List<IllegalWordsSearchResult> all = iwords.FindAll(test);
            Assert.AreEqual("一", all[0].Keyword);
            Assert.AreEqual("二二", all[1].Keyword);
            Assert.AreEqual("三三三", all[2].Keyword);
            Assert.AreEqual("四四四四", all[3].Keyword);
            Assert.AreEqual("五五五五五", all[4].Keyword);
            Assert.AreEqual("六六六六六六", all[5].Keyword);

        }
        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/57
        /// </summary>
        [Test]
        public void IssuesTest_57_2()
        {
            String test = "jameson吃饭";
            List<String> list = new List<String>();
            list.Add("jameson吃饭");
            list.Add("吃饭jameson");

            IllegalWordsSearch iwords = new IllegalWordsSearch();
            iwords.SetKeywords(list);

            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);

            var f = iwords.FindFirst(test);
            Assert.AreEqual("jameson吃饭", f.Keyword);

        }
        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/57
        /// </summary>
        [Test]
        public void IssuesTest_57_3()
        {
            String test = "his is sha ash";
            List<String> list = new List<String>();
            list.Add("ash");
            list.Add("sha");
            list.Add("bcd");

            IllegalWordsSearch iwords = new IllegalWordsSearch();
            iwords.SetKeywords(list);

            var b = iwords.ContainsAny(test);
            Assert.AreEqual(true, b);

            var f = iwords.FindFirst(test);
            Assert.AreEqual("sha", f.Keyword);

            var all = iwords.FindAll(test);
            Assert.AreEqual(2, all.Count);
        }

        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/65
        /// </summary>
        [Test]
        public void IssuesTest_65()
        {
            var search = new IllegalWordsSearch();
            List<string> keywords = new List<string>();
            keywords.Add("fuck");
            keywords.Add("ffx");
            search.SetKeywords(keywords);
            var result = search.Replace("fFuck");
            Assert.AreEqual("*****", result);
        }


        /// <summary>
        /// https://github.com/toolgood/ToolGood.Words/issues/68
        /// </summary>
        [Test]
        public void IssuesTest_68_1()
        {
            var txts = File.ReadAllLines("_texts/dict.txt");
            var keywords = new List<String>();
            foreach (var item in txts)
            {
                keywords.Add(item.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            var search = new PinyinMatch();
            search.SetKeywords(keywords);
            var ts = search.Find("野huo");
            Assert.AreEqual(5, ts.Count);
        }
    }
}
