ToolGood.Words
==================
一款高性能非法词(敏感词)检测组件，附带繁体简体互换，支持全角半角互换等功能。

`C#`版的`ToolGood.Words` 是已知最快的非法词(敏感词)检测组件， 使用`StringSearchEx2.Replace`过滤，在48k敏感词库上的过滤速度超过3亿字符每秒。（cpu i7 8750h，2019年） 


目前java版本已移植了`StringSearch`、`StringSearchEx`、`WordsSearch`、`WordsSearchEx`、`IllegalWordsSearch`、`StringTypoSearch`、`NumberTypoSearch`及部分`WordsHelper`


## 非法词(敏感词)检测（字符串搜索）
非法词(敏感词)检测类：`StringSearch`、`StringSearchEx`、`WordsSearch`、`WordsSearchEx`、`IllegalWordsSearch`、`StringTypoSearch`、`NumberTypoSearch`;

* `StringSearch`、`StringSearchEx`: 搜索`FindFirst`方法返回结果为`string`类型。
* `WordsSearch`、`WordsSearchEx`: 搜索`FindFirst`方法返回结果为`WordsSearchResult`类型,
`WordsSearchResult`不仅仅有关键字，还有关键字的开始位置、结束位置，关键字序号等。
* `IllegalWordsSearch`: 过滤非法词（敏感词）专用类，可设置跳字长度，默认**繁体转简体，全角转半角，忽略大小写，跳词，重复词，黑名单**，
搜索`FindFirst`方法返回为`IllegalWordsSearchResult`,有关键字，对应原文，开始、位置，黑名单类型。
* `IllegalWordsSearch`、`StringSearchEx`、`WordsSearchEx` 使用`Save`、`Load`方法，可以加快初始化。
* 共同方法有：`SetKeywords`、`ContainsAny`、`FindFirst`、`FindAll`、`Replace`
* `IllegalWordsSearch`独有方法：`SetSkipWords`（设置跳词）、`SetBlacklist`（设置黑名单）。
* `IllegalWordsSearch`字段`UseIgnoreCase`：设置是忽略否大小写,必须在`SetKeywords`方法之前，注：使用`Load`方法则该字段无效。
* `StringTypoSearch` 错字搜索,英文数字搜索,不包含中文转数字,主要检测网址，推广账号。
* `NumberTypoSearch` 数字错字搜索，先使用静态方法ReplaceNumberSymbol对❿这些符号进行转化 ，然后使用本类


``` java
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
 