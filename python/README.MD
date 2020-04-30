ToolGood.Words（Python版本）
==================
一款高性能非法词(敏感词)检测组件，繁体简体互换，获取拼音首字母，获取拼音字母等功能。


文件说明：
```
ToolGood.Words.Pinyin.py        拼音转化
ToolGood.Words.Translate.py     繁体简体转化
ToolGood.Words.StringSearch.py  字符串检索
ToolGood.Words.WordsSearch.py   字符串检索，包含位置信息、索引
```

### 拼音操作
``` python
if __name__ == "__main__":
    pinyin = Pinyin()
    # 获取全拼
    pinyin.GetPinyin("我爱中国") # WoAiZhongGuo
    pinyin.GetPinyin("我爱中国",1) # WǒÀiZhōngGuó
    # 获取首字母
    pinyin.GetFirstPinyin("我爱中国") # WAZG
    # 获取全部拼音
    pinyin.GetAllPinyin('传') # Chuan,Zhuan
    # 获取姓名
    pinyin.GetPinyinForName("单一一") # ShanYiYi
    pinyin.GetPinyinForName("单一一",1) # ShànYīYī
```

### 繁体简体互换
``` python
if __name__ == "__main__":
    translate = Translate()
    # 转成简体
    translate.ToSimplifiedChinese("我愛中國")
    translate.ToSimplifiedChinese("我愛中國",1) # 港澳繁体 转 简体
    translate.ToSimplifiedChinese("我愛中國",2) # 台湾正体 转 简体
    # 转成繁体
    translate.ToTraditionalChinese("我爱中国")
    translate.ToTraditionalChinese("我爱中国",1) # 简体 转 港澳繁体
    translate.ToTraditionalChinese("我爱中国",2) # 简体 转 台湾正体
```


### 字符串检测 StringSearch
``` python
if __name__ == "__main__":
    s = "中国|国人|zg人"
    test = "我是中国人"

    search = StringSearch()
    search.SetKeywords(s.split('|'))

    f = search.FindFirst(test)
    if f!="中国" :
        print("StringSearch FindFirst is error.............................")
 
    all = search.FindAll(test)
    if all[0]!="中国" :
        print("StringSearch FindAll is error.............................")
    if all[1]!="国人" :
        print("StringSearch FindAll is error.............................")
    if len(all)!=2 :
        print("StringSearch FindAll is error.............................")

    b = search.ContainsAny(test)
    if b==False :
        print("StringSearch ContainsAny is error.............................")

    txt = search.Replace (test)
    if (txt != "我是***"):
        print("StringSearch Replace  is error.............................")
```

### 字符串检测 WordsSearch
``` python
if __name__ == "__main__":
    s = "中国|国人|zg人"
    test = "我是中国人"

    search = WordsSearch()
    search.SetKeywords(s.split('|'))

    f = search.FindFirst(test)
    if f["Keyword"]!="中国" :
        print("WordsSearch FindFirst is error.............................")
 
    all = search.FindAll(test)
    if all[0]["Keyword"]!="中国" :
        print("WordsSearch FindAll is error.............................")
    if all[1]["Keyword"]!="国人" :
        print("WordsSearch FindAll is error.............................")
    if all[0]["Start"]!=2 :
        print("WordsSearch FindAll is error.............................")
    if all[0]["End"]!=3 :
        print("WordsSearch FindAll is error.............................")
    if len(all)!=2 :
        print("WordsSearch FindAll is error.............................")

    b = search.ContainsAny(test)
    if b==False :
        print("WordsSearch ContainsAny is error.............................")

    txt = search.Replace (test)
    if (txt != "我是***"):
        print("WordsSearch Replace  is error.............................")
```


