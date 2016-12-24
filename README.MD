ToolGood.Words
====


支持 繁体简体转化, 支持全角 半角转化, 获取拼音首字母, 获取 拼音字母 , 非法词(敏感词)查询

``` csharp
            string s = "中国|国人|zg人|fuck|all|as|19|http://|ToolGood";
            IllegalWordsSearch iwords = new IllegalWordsSearch(s.Split('|'), 2);

            string test = "我是中国人";
            var all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SearchString);
            Assert.AreEqual("国人", all[1].SearchString);

            test = "我是中国zg人";
            all = iwords.FindAll(test);
            Assert.AreEqual("中国", all[0].SearchString);
            Assert.AreEqual("国zg人", all[1].SearchString);
            Assert.AreEqual("国人", all[1].Keyword);
            Assert.AreEqual("zg人", all[2].SearchString);

            test = "中间国zg人";
            all = iwords.FindAll(test);
            Assert.AreEqual("国zg人", all[0].SearchString);
            Assert.AreEqual("zg人", all[1].SearchString);

            test = "fuck al.l";
            all = iwords.FindAll(test);
            Assert.AreEqual("fuck", all[0].SearchString);
            Assert.AreEqual("al.l", all[1].SearchString);
            Assert.AreEqual(2, all.Count);

            test = "ht@tp://toolgood.com";
            all = iwords.FindAll(test);
            Assert.AreEqual("ht@tp://", all[0].SearchString);
            Assert.AreEqual("http://", all[0].Keyword);
            Assert.AreEqual("toolgood", all[1].SearchString);
            Assert.AreNotEqual("ToolGood", all[1].Keyword);
            Assert.AreEqual("toolgood", all[1].Keyword);
            Assert.AreEqual(2, all.Count);


            test = "asssert all";
            all = iwords.FindAll(test);
            Assert.AreEqual("all", all[0].SearchString);
            Assert.AreEqual(1, all.Count);

            test = "19w 1919 all";
            all = iwords.FindAll(test);
            Assert.AreEqual("19", all[0].SearchString);
            Assert.AreEqual("all", all[1].SearchString);
```

字符搜索优化`StringSearch`

TextSearch   : 184ms
TrieFilter : 175ms
FastFilter : 106ms
StringSearch   : 32ms
----------------------- Find All -----------------------------------
TextSearch   : 1,572ms
TrieFilter : 1,360ms
FastFilter : 308ms
StringSearch   : 376ms


注:`FastFilter`来源: http://www.cnblogs.com/yeerh/archive/2011/10/20/2219035.html

注: 在 Find All测试中,`FastFilter`只能检测出7个:
    [0]: "主席"
    [1]: "赵洪祝"
    [2]: "中国"
    [3]: "铁道部"
    [4]: "党"
    [5]: "胡锦涛"
    [6]: "倒台"

`StringSearch`检测出14个:
    [0]: "党"
    [1]: "党委"
    [2]: "西藏"
    [3]: "党"
    [4]: "党委"
    [5]: "主席"
    [6]: "赵洪祝"
    [7]: "中国"
    [8]: "铁道部"
    [9]: "党"
    [10]: "胡锦涛"
    [11]: "锦涛"
    [12]: "倒台"
    [13]: "黑社会"

