ToolGood.Words(Java版)
==================
一款高性能非法词(敏感词)检测组件，附带繁体简体互换，支持全角半角互换等功能。

`C#`版的`ToolGood.Words` 是已知最快的非法词(敏感词)检测组件， 使用`StringSearchEx2.Replace`过滤，在48k敏感词库上的过滤速度超过3亿字符每秒。（cpu i7 8750h，2019年） 


目前java版本已移植了`StringSearch`、`StringSearchEx`、`StringSearchEx2`、`WordsSearch`、`WordsSearchEx`、`WordsSearchEx2`、`IllegalWordsSearch`及部分`WordsHelper`


## 繁体简体互换、全角半角互换、拼音操作
``` java
    import toolgood.words.WordsHelper;
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
    // 获取全拼
    WordsHelper.GetPinYin("我爱中国");//WoAiZhongGuo
    WordsHelper.GetPinYin("我爱中国",true);//WǒÀiZhōngGuó
    // 获取首字母
    WordsHelper.GetFirstPinYin("我爱中国");//WAZG
    // 获取全部拼音
    WordsHelper.GetAllPinYin('传');//Chuan,Zhuan
    // 获取姓名
    WordsHelper.GetPinYinForName("单一一")//ShanYiYi
    WordsHelper.GetPinYinForName("单一一",true)//ShànYīYī
```

## 非法词(敏感词)检测（字符串搜索）
非法词(敏感词)检测类：`StringSearch`、`StringSearchEx`、`StringSearchEx2`、`WordsSearch`、`WordsSearchEx`、`WordsSearchEx2`、`IllegalWordsSearch`;

* `StringSearch`、`StringSearchEx`、`StringSearchEx2`: 搜索`FindFirst`方法返回结果为`string`类型。
* `WordsSearch`、`WordsSearchEx`、`WordsSearchEx2`: 搜索`FindFirst`方法返回结果为`WordsSearchResult`类型,
`WordsSearchResult`不仅仅有关键字，还有关键字的开始位置、结束位置，关键字序号等。
* `IllegalWordsSearch`: 过滤非法词（敏感词）专用类，可设置跳字长度，默认**全角转半角，忽略大小写，跳词，重复词，黑名单**，
搜索`FindFirst`方法返回为`IllegalWordsSearchResult`,有关键字，对应原文，开始、位置，黑名单类型。
* `IllegalWordsSearch`、`StringSearchEx2`、`WordsSearchEx2` 使用`Save`、`Load`方法，可以加快初始化。
* 共同方法有：`SetKeywords`、`ContainsAny`、`FindFirst`、`FindAll`、`Replace`
* `IllegalWordsSearch`独有方法：`SetSkipWords`（设置跳词）、`SetBlacklist`（设置黑名单）。
* `IllegalWordsSearch`字段`UseIgnoreCase`：设置是忽略否大小写,必须在`SetKeywords`方法之前，注：使用`Load`方法则该字段无效。


``` java
	import toolgood.words.IllegalWordsSearch;
	import toolgood.words.IllegalWordsSearchResult;
	import toolgood.words.StringSearch;
	import toolgood.words.StringSearchEx;
	import toolgood.words.StringSearchEx2;
	import toolgood.words.WordsSearch;
	import toolgood.words.WordsSearchEx;
	import toolgood.words.WordsSearchEx2;
	import toolgood.words.WordsSearchResult;

	String test = "我是中国人";
	List<String> list=new ArrayList<String>();
	list.add("中国");		 
	list.add("国人");
	list.add("zg人");
	System.out.println("StringSearch run Test.");

	StringSearch iwords = new StringSearch();
	iwords.SetKeywords(list);

	boolean b = iwords.ContainsAny(test);
	if(b==false){
		System.out.println("ContainsAny is Error.");
	}

	String f = iwords.FindFirst(test);
	if(f!="中国"){
		System.out.println("FindFirst is Error.");
	}

	List<String> all = iwords.FindAll(test);
	if(all.get(0)!="中国"){
		System.out.println("FindAll is Error.");
	}
	if(all.get(1)!="国人"){
		System.out.println("FindAll is Error.");
	}
	if(all.size()!=2){
		System.out.println("FindAll is Error.");
	}

	String str = iwords.Replace(test, '*');
	if(str.equals("我是***")==false ){
		System.out.println("Replace is Error.");
	}
``` 
 
## 非法词(敏感词)检测（字符串搜索）（支持通配符）
非法词(敏感词)检测类：`StringMatch`、`WordsMatch`。

支持部分正则表达式类型：`.`（点）`?`(问号) `[]`(方括号) `(|)`(括号与竖线)
 
``` java
	import toolgood.words.StringMatch;
	import toolgood.words.WordsMatch;
 
	String test = "我是中国人";
	List<String> list = new ArrayList<String>();
	list.add("[中美]国");
	list.add("国人");
	list.add("zg人");
	System.out.println("StringMatch run Test.");

	WordsMatch iwords = new WordsMatch();
	iwords.SetKeywords(list);

	boolean b = iwords.ContainsAny(test);
	if (b == false) {
		System.out.println("ContainsAny is Error.");
	}

	WordsSearchResult f = iwords.FindFirst(test);
	if (f.Keyword.equals("中国") == false) {
		System.out.println("FindFirst is Error.");
	}

	List<WordsSearchResult> all = iwords.FindAll(test);
	if (all.get(0).Keyword.equals("中国") == false) {
		System.out.println("FindAll is Error.");
	}
	if (all.get(1).Keyword.equals("国人") == false) {
		System.out.println("FindAll is Error.");
	}
	if (all.size() != 2) {
		System.out.println("FindAll is Error.");
	}

	String str = iwords.Replace(test, '*');
	if (str.equals("我是***") == false) {
		System.out.println("Replace is Error.");
	}
``` 
 