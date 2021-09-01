ToolGood.Words
==================
一款高性能非法词(敏感词)检测组件，附带繁体简体互换，支持全角半角互换，获取拼音首字母，获取拼音字母，拼音模糊搜索等功能。

`C#`语言，使用`StringSearchEx2.Replace`过滤，在48k敏感词库上的过滤速度超过3亿字符每秒。（cpu i7 8750h） 


csharp 文件夹说明：
```
ToolGood.Pinyin.Build:          生成词的拼音
ToolGood.Pinyin.Pretreatment:   生成拼音预处理，核对拼音，词组最小化
ToolGood.Transformation.Build： 生成简体繁体转换文档，更新时文档放在同一目录下，词库参考 https://github.com/BYVoid/OpenCC
ToolGood.Words.Contrast:        字符串搜索对比
ToolGood.Words.Test:            单元测试
ToolGood.Words:                 本项目源代码
```

## 非法词(敏感词)检测（字符串搜索）
非法词(敏感词)检测类：`StringSearch`、`StringSearchEx`、`StringSearchEx2`、`WordsSearch`、`WordsSearchEx`、`WordsSearchEx2`、`IllegalWordsSearch`;

* `StringSearch`、`StringSearchEx`、`StringSearchEx2`、`StringSearchEx3`: 搜索`FindFirst`方法返回结果为`string`类型。
* `WordsSearch`、`WordsSearchEx`、`WordsSearchEx2`、`WordsSearchEx3`: 搜索`FindFirst`方法返回结果为`WordsSearchResult`类型,
`WordsSearchResult`不仅仅有关键字，还有关键字的开始位置、结束位置，关键字序号等。
* `IllegalWordsSearch`: 过滤非法词（敏感词）专用类，可设置跳字长度，默认**全角转半角，忽略大小写，跳词，重复词，黑名单**，
搜索`FindFirst`方法返回为`IllegalWordsSearchResult`,有关键字，对应原文，开始、位置，黑名单类型。
* `IllegalWordsSearch`、`StringSearchEx`、`StringSearchEx2`、`WordsSearchEx`、`WordsSearchEx2` 使用`Save`、`Load`方法，可以加快初始化。
* 共同方法有：`SetKeywords`、`ContainsAny`、`FindFirst`、`FindAll`、`Replace`
* `IllegalWordsSearch`独有方法：`SetSkipWords`（设置跳词）、`SetBlacklist`（设置黑名单）。
* `IllegalWordsSearch`字段`UseIgnoreCase`：设置是忽略否大小写,必须在`SetKeywords`方法之前，注：使用`Load`方法则该字段无效。
* `StringSearchEx3`、`WordsSearchEx3`为指针版优化版，实测时发现性能浮动比较大。

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
## 非法词(敏感词)检测（字符串搜索）（支持通配符）
非法词(敏感词)检测类：`StringMatch`、`StringMatchEx`、`WordsMatch`、`WordsMatchEx`。

支持部分正则表达式类型：`.`（点）`?`(问号) `[]`(方括号) `(|)`(括号与竖线)

``` csharp
    string s = ".[中美]国|国人|zg人";
    string test = "我是中国人";

    WordsMatch wordsSearch = new WordsMatch();
    wordsSearch.SetKeywords(s.Split('|'));

    var b = wordsSearch.ContainsAny(test);
    Assert.AreEqual(true, b);

    var f = wordsSearch.FindFirst(test);
    Assert.AreEqual("是中国", f.Keyword);

    var alls = wordsSearch.FindAll(test);
    Assert.AreEqual("是中国", alls[0].Keyword);
    Assert.AreEqual(".[中美]国", alls[0].MatchKeyword);
    Assert.AreEqual(1, alls[0].Start);
    Assert.AreEqual(3, alls[0].End);
    Assert.AreEqual(0, alls[0].Index);//返回索引Index,默认从0开始
    Assert.AreEqual("国人", alls[1].Keyword);
    Assert.AreEqual(2, alls.Count);

    var t = wordsSearch.Replace(test, '*');
    Assert.AreEqual("我****", t);
```

## 繁体简体互换、全角半角互换、数字转成中文大写、拼音操作

``` csharp
    // 转成简体
    WordsHelper.ToSimplifiedChinese("我愛中國");
    WordsHelper.ToSimplifiedChinese("我愛中國",1);// 港澳繁体 转 简体
    WordsHelper.ToSimplifiedChinese("我愛中國",2);// 台湾正体 转 简体
    // 转成繁体
    WordsHelper.ToTraditionalChinese("我爱中国");
    WordsHelper.ToTraditionalChinese("我爱中国",1);// 简体 转 港澳繁体
    WordsHelper.ToTraditionalChinese("我爱中国",2);// 简体 转 台湾正体
    // 转成全角
    WordsHelper.ToSBC("abcABC123");
    // 转成半角
    WordsHelper.ToDBC("ａｂｃＡＢＣ１２３");
    // 数字转成中文大写
    WordsHelper.ToChineseRMB(12345678901.12);
    // 中文转成数字
    WordsHelper.ToNumber("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
    // 获取全拼
    WordsHelper.GetPinyin("我爱中国");//WoAiZhongGuo   
    WordsHelper.GetPinyin("我爱中国",",");//Wo,Ai,Zhong,Guo   
    WordsHelper.GetPinyin("我爱中国",true);//WǒÀiZhōngGuó

    // 获取首字母
    WordsHelper.GetFirstPinyin("我爱中国");//WAZG
    // 获取全部拼音
    WordsHelper.GetAllPinyin('传');//Chuan,Zhuan
    // 获取姓名
    WordsHelper.GetPinyinForName("单一一")//ShanYiYi
    WordsHelper.GetPinyinForName("单一一",",")//Shan,Yi,Yi
    WordsHelper.GetPinyinForName("单一一",true)//ShànYīYī
``` 
##### 拼音分支
[ToolGood.Words.Pinyin](https://github.com/toolgood/ToolGood.Words.Pinyin)
追求更快的加载速度（目前只有C#代码）。

##  拼音匹配
`PinyinMatch`：方法有`SetKeywords`、`SetIndexs`、`Find`、`FindIndex`。

`PinyinMatch<T>`：方法有`SetKeywordsFunc`、`SetPinyinFunc`、`SetPinyinSplitChar`、`Find`。
``` csharp
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

    var all2 = match.FindIndex("BJ");
    Assert.AreEqual(0, all2[0]);
    Assert.AreEqual(1, all.Count);
``` 




## 性能对比
执行10万次性能对比，结果如下：

![10W次性能对比](https://toolgood.github.io/ToolGood.Words/10W次性能对比_20200901.png)

注:C#自带正则很慢，`StringSearchEx2.ContainsAny`是`Regex.IsMatch`效率的8.8万倍多，跟关键字数量有关。


`Regex.Matches`的运行方式跟`IQueryable`的类似，只返回`MatchCollection`,还没有计算。

 在 Find All测试中,

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


插曲：在细查`Regex.Matches`神奇3ms，我发现`Regex.Matches`有一个小问题，

`Regex.Matches`只能检测出11个:
    [0]: "党"
    [1]: "西藏"
    [2]: "党"
    [3]: "主席"
    [4]: "赵洪祝"
    [5]: "中国"
    [6]: "铁道部"
    [7]: "党"
    [8]: "胡锦涛"
    [9]: "倒台"
    [10]: "黑社会"

## 探讨敏感信息过滤：

我建一个Q群，用于探讨敏感信息过滤算法、敏感词汇及图片过滤。

群文件分享了一些收集的资料：通用敏感词、购物app、聊天app、游戏app敏感词。

敏感信息过滤研究会，Q群：128994346

## 广告一下

《ToolGood 内容审核系统》正式发布，Windows、Linux双平台，内存占用小于100M。

官网：https://toolgood.com/

内存少了，功能却不少，可以下文字去官网测试。拼接后的脏字有26303904组（约2630W）。

```
c  C  ⒞  ｃ  ⓒ  C  Ｃ  ℂ  Ⓒ  с  С  𝐜  𝑐  𝚌  𝒸  𝒄  𝓬  𝕔  𝖼  𝗰  𝘤  𝙘  𝐂  𝐶  𝙲  𝒞  𝑪  𝓒  𝖢  𝗖  𝘊  𝘾  ©
a  A  ⒜  ā  á  ǎ  à  ａ  ⓐ  A  Ａ  Ⓐ  а  А  𝐚  𝑎  𝚊  𝒶  𝒂  𝓪  𝕒  𝖺  𝗮  𝘢  𝙖  𝐀  𝐴  𝙰  𝒜  𝑨  𝓐  𝔸  𝖠  𝗔  𝘈  𝘼  Α  α
o  O  ⒪  ō  ó  ǒ  ò  ｏ  ⓞ  O  Ｏ  Ⓞ  о  О  𝐨  𝑜  𝚘  𝒐  𝓸  𝕠  𝗈  𝗼  𝘰  𝙤  𝐎  𝑂  𝙾  𝒪  𝑶  𝓞  𝕆  𝖮  𝗢  𝘖  𝙊  Ο  ο  σ
㚷  伱  你  倪  呢  妮  妳  尼  您  拟  昵  汝  沵  泥  祢  迩  铌  鉨  your  you  亻尓  女尓  ni
㐷  吗  嘛  妈  娘  嫲  嬷  杩  溤  犘  犸  玛  痲  码  祃  蔴  蚂  马  骂  鰢  鷌  麻  嚒  ma
```

## 敏感词相关文章
1、[敏感词过滤方案那些事](https://www.cnblogs.com/toolgood/p/15208734.html)

2、[普通公司敏感词审核制度](https://www.cnblogs.com/toolgood/p/15213549.html)

3、新人小白过滤敏感词方案

4、网络常用敏感词过滤方法

5、ToolGood.Words算法过滤敏感词优化原理

6、敏感词过滤中常遇的几个问题
