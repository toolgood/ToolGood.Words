ToolGood.Words（Js版本）
==================
一款高性能非法词(敏感词)检测组件，繁体简体互换，获取拼音首字母，获取拼音字母等功能。

文件说明：
```
ToolGood.Words.Pinyin.js        拼音转化
ToolGood.Words.Translate.js     繁体简体转化
ToolGood.Words.StringSearch.js  字符串检索
ToolGood.Words.WordsSearch.js   字符串检索，包含位置信息、索引
```

### 拼音操作
``` javascript
    var pinyin = new Pinyin();
    // 获取全拼
    pinyin.GetPinyin("我爱中国");//WoAiZhongGuo
    pinyin.GetPinyin("我爱中国",true);//WǒÀiZhōngGuó
    // 获取首字母
    pinyin.GetFirstPinyin("我爱中国");//WAZG
    // 获取全部拼音
    pinyin.GetAllPinyin('传');//Chuan,Zhuan
    // 获取姓名
    pinyin.GetPinyinForName("单一一")//ShanYiYi
    pinyin.GetPinyinForName("单一一",true)//ShànYīYī
```

### 繁体简体互换
``` javascript
    var translate = new Translate();
    // 转成简体
    translate.ToSimplifiedChinese("我愛中國");
    translate.ToSimplifiedChinese("我愛中國",1);// 港澳繁体 转 简体
    translate.ToSimplifiedChinese("我愛中國",2);// 台湾正体 转 简体
    // 转成繁体
    translate.ToTraditionalChinese("我爱中国");
    translate.ToTraditionalChinese("我爱中国",1);// 简体 转 港澳繁体
    translate.ToTraditionalChinese("我爱中国",2);// 简体 转 台湾正体
```

### 字符串检测
``` javascript
    string s = "中国|国人|zg人";
    string test = "我是中国人";
    var search = new WordsSearch();// 或  var search = new StringSearch();
    wordsSearch.SetKeywords(s.split('|'));

    // 查找第一个匹配
    var f = search.FindFirst(test);
    // 查找所有匹配
    var all = search.FindAll(test);
    // 检查是否有匹配
    var b = search.ContainsAny(test);
    // 替换所有匹配
    var str = search.Replace(test, '*');
```
