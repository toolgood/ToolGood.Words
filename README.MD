ToolGood.Words
==================
支持繁体简体互换，支持全角半角互换，获取拼音首字母，获取拼音字母，字符串搜索，非法词(敏感词)搜索。

文件夹说明：
```
ToolGood.PinYin.Build:      生成词的拼音
ToolGood.PinYin.WordsBuild: 生成多音词的拼音
ToolGood.Words.Contrast:    字符串搜索对比
ToolGood.Words.Test:        单元测试
ToolGood.Words:             本项目源代码
```


## 繁体简体互换、全角半角互换、数字转成中文大写

``` csharp
    // 转成简体
    WordsHelper.ToSimplifiedChinese("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");
    // 转成繁体
    WordsHelper.ToTraditionalChinese("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
    // 转成全角
    WordsHelper.ToSBC("abcABC123");
    // 转成半角
    WordsHelper.ToDBC("ａｂｃＡＢＣ１２３");
    // 数字转成中文大写
    WordsHelper.ToChineseRMB(12345678901.12);
``` 

## 拼音操作

``` csharp
    // 获取全拼
    WordsHelper.GetPinYin("我爱中国");//WoAiZhongGuo
    // 获取首字母
    WordsHelper.GetFirstPinYin("我爱中国");//WAZG
    // 获取全部拼音
    WordsHelper.GetAllPinYin('传');//Chuan,Zhuan
``` 

## 字符串搜索
字符串搜索类：`StringSearch`、`WordsSearch`、`IllegalWordsSearch`、`IllegalWordsQuickSearch`;

* `StringSearch`: 搜索`FindFirst`方法返回结果为`string`类型。
* `WordsSearch`: 搜索`FindFirst`方法返回结果为`WordsSearchResult`类型,
`WordsSearchResult`不仅仅有关键字，还有关键字的开始位置、结束位置，关键字序号等。
* `IllegalWordsSearch`: 过滤非法词（敏感词）专用类，可设置跳字长度，默认**繁体转简体，全角转半角，忽略大小写，转化特殊数字**，
搜索`FindFirst`方法返回为`IllegalWordsSearchResult`,有关键字，开始位置、结束位置。
* `IllegalWordsQuickSearch`: `IllegalWordsSearch`简化版本。
* 共同方法有：`SetKeywords`、`ContainsAny`、`FindFirst`、`FindAll`、`Replace`


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

    var str = iwords.Replace(test, '*');
    Assert.AreEqual("我是***", str);
``` 

##### 性能对比
设备：DELL，系统：windows 10，CPU：i5-5200U，内存：12G，每种方法执行 10万次，结果如下：
```
-------------------- FindFirst OR ContainsAny --------------------
TrieFilter : 147ms
FastFilter : 102ms
StringSearch（ContainsAny） : 31ms
StringSearch（FindFirst） : 44ms
WordsSearch（ContainsAny） : 30ms
WordsSearch（FindFirst） : 50ms
IllegalWordsQuickSearch（FindFirst） : 53ms
IllegalWordsQuickSearch（ContainsAny） : 45ms
IllegalWordsSearch（FindFirst） : 160ms
IllegalWordsSearch（ContainsAny） : 154ms
-------------------- Find All --------------------
TrieFilter（FindAll） : 1,340ms
FastFilter（FindAll） : 302ms
StringSearch（FindAll） : 377ms
WordsSearch（FindAll） : 425ms
IllegalWordsQuickSearch（FindAll） : 1,068ms
IllegalWordsSearch（FindAll） : 2,519ms
```
注:C#自带正则很慢，`IsMatch`效率不到`StringSearch.ContainsAny`的千分之一。

注:`TrieFilter`,`FastFilter`来源: http://www.cnblogs.com/yeerh/archive/2011/10/20/2219035.html

注: 在 Find All测试中,

`FastFilter`只能检测出7个:
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
