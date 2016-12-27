ToolGood.Words
==================
支持 繁体简体转化, 支持全角 半角转化, 获取拼音首字母, 获取 拼音字母 , 非法词(敏感词)查询


#### 繁体简体互换

``` csharp
    //转成简体
    WordsHelper.ToSimplifiedChinese("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");
    //转成繁体
    WordsHelper.ToTraditionalChinese("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
``` 


#### 全角半角互换

``` csharp
    //转成全角
    WordsHelper.ToSBC("abcABC123");
    //转成半角
    WordsHelper.ToDBC("ａｂｃＡＢＣ１２３");
``` 

#### 拼音操作

``` csharp
    //获取全拼
    var py = WordsHelper.GetPinYin("我爱中国");
    Assert.AreEqual("WoAiZhongGuo", py);
    //获取首字母
    py = WordsHelper.GetFirstPinYin("我爱中国");
    Assert.AreEqual("WAZG", py);
    //获取全部拼音
    var pys = WordsHelper.GetAllPinYin('传');
    Assert.AreEqual("Chuan", pys[0]);
    Assert.AreEqual("Zhuan", pys[1]);
``` 

#### 字符串搜索

``` csharp
    string s = "中国|国人|zg人";
    string test = "我是中国人";

    StringSearch iwords = new StringSearch();
    iwords.SetKeywords(s.Split('|'));
    
    var b = iwords.ContainsAny(test);
    Assert.AreEqual(true, b);

    var f = iwords.FindFirst(test);
    Assert.AreEqual("中国", f);

    var all = iwords.FindAll(test);
    Assert.AreEqual("中国", all[0]);
    Assert.AreEqual("国人", all[1]);
    Assert.AreEqual(2, all.Count);
``` 

#### 查找非法词(敏感词)

``` csharp
    string s = "中国|国人|zg人|fuck|all|as|19|http://|ToolGood";
    string test = "我是中国人";

    IllegalWordsSearch iwords = new IllegalWordsSearch( 2);
    iwords.SetKeywords(s.Split('|'));

    var b = iwords.ContainsAny(test);
    Assert.AreEqual(true, b);

    var f = iwords.FindFirst(test);
    Assert.AreEqual(true, f.Success);
    Assert.AreEqual("中国", f.Keyword);
    Assert.AreEqual(2, f.Start);
    Assert.AreEqual(3, f.End);

    var all = iwords.FindAll(test);
    Assert.AreEqual("中国", all[0].SearchString);
    Assert.AreEqual("国人", all[1].SearchString);

    test = "我是中国zg人";
    all = iwords.FindAll(test);
    Assert.AreEqual("中国", all[0].SearchString);
    Assert.AreEqual("zg人", all[1].SearchString);

    test = "中间国zg人";
    all = iwords.FindAll(test);
    Assert.AreEqual("zg人", all[0].SearchString);

    test = "fuck al.l";
    all = iwords.FindAll(test);
    Assert.AreEqual("fuck", all[0].SearchString);
    Assert.AreEqual("al.l", all[1].SearchString);
    Assert.AreEqual(2, all.Count);

    test = "ht@tp://ToolGood.com";
    all = iwords.FindAll(test);
    Assert.AreEqual("toolgood", all[0].Keyword);
    Assert.AreEqual("ToolGood", all[0].SearchString);
    Assert.AreEqual(1, all.Count);


    test = "asssert all";
    all = iwords.FindAll(test);
    Assert.AreEqual("all", all[0].SearchString);
    Assert.AreEqual(1, all.Count);

    test = "19w 1919 all";
    all = iwords.FindAll(test);
    Assert.AreEqual("19", all[0].SearchString);
    Assert.AreEqual("all", all[1].SearchString);

    test = "我是【中]国【人";
    all = iwords.FindAll(test);
    Assert.AreEqual("中]国", all[0].SearchString);
    Assert.AreEqual("国【人", all[1].SearchString);

    test = "我是【中国【人";
    all = iwords.FindAll(test);
    Assert.AreEqual("中国", all[0].SearchString);
    Assert.AreEqual(1, all.Count);
```

##### 性能对比

TrieFilter : 153ms

FastFilter : 109ms

StringSearch   : 32ms

StringSearch（ToSenseWord） : 199ms

IllegalWordsSearch（FindFirst） : 54ms

IllegalWordsSearch（ContainsAny） : 52ms

TrieFilter（Find All） : 1,453ms

FastFilter （Find All）: 395ms

StringSearch（Find All）: 405ms

IllegalWordsSearch（Find All）: 1,201ms



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

`IllegalWordsSearch`检出15个：
    [0]: {63|党}
    [1]: {63|党委}
    [2]: {81|西藏}
    [3]: {83|党}
    [4]: {83|党委}
    [5]: {143|主席}
    [6]: {185|赵洪祝}
    [7]: {204|中国}
    [8]: {221|铁道部}
    [9]: {235|党}
    [10]: {244|胡锦涛}
    [11]: {245|锦涛}
    [12]: {323|倒台}
    [13]: {324|台}
    [14]: {364|黑社会}

注：`IllegalWordsSearch`比`StringSearch`多一个`台`，原因：关键字繁体转简体
