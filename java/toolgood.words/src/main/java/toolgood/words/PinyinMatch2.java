package toolgood.words;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;
import java.util.regex.Pattern;

import toolgood.words.internals.BasePinyinMatch;
import toolgood.words.internals.PinyinDict;
import toolgood.words.internals.TwoTuple;

public class PinyinMatch2<T> extends BasePinyinMatch {
    private List<T> _list;
    private Function<T, String> _keywordsFunc;
    private Function<T, String> _pinyinFunc;
    private char _splitChar = ',';

    public PinyinMatch2(List<T> list) {
        _list = list;
        _keywordsFunc = new Function<T, String>() {
            @Override
            public String apply(T in) {
                return (String) "";
            }
        };
        _pinyinFunc = new Function<T, String>() {
            @Override
            public String apply(T in) {
                return (String) "";
            }
        };
    }

    public void SetKeywordsFunc(Function<T, String> keywordsFunc) {
        _keywordsFunc = keywordsFunc;
    }

    public void SetPinyinFunc(Function<T, String> pinyinFunc) {
        _pinyinFunc = pinyinFunc;
    }

    public void SetPinyinSplitChar(char splitChar) {
        _splitChar = splitChar;
    }

    public List<T> Find(String keywords) throws Exception {
        if (_keywordsFunc == null) {
            throw new Exception("请先使用SetKeywordsFunc方法。");
        }
        keywords = keywords.toUpperCase().trim();
        if (keywords == null || keywords.equals("")) {
            return null;
        }
        List<T> result = new ArrayList<T>();
        boolean hasPinyin = Pattern.matches(keywords, "[a-zA-Z]");
        if (hasPinyin == false) {
            for (T item : _list) {
                String keyword = _keywordsFunc.apply(item);
                if (keyword.contains(keywords)) {
                    result.add(item);
                }
            }
            return result;
        }

        List<String> pykeys = SplitKeywords(keywords);
        int minLength = Integer.MAX_VALUE;
        List<TwoTuple<String, String[]>> list = new ArrayList<TwoTuple<String, String[]>>();
        for (String pykey : pykeys) {
            String[] keys = pykey.split(((Character) (char) 0).toString());
            if (minLength > keys.length) {
                minLength = keys.length;
            }
            MergeKeywords(keys, 0, "", list);
        }

        PinyinSearch search = new PinyinSearch();
        search.SetKeywords2(list);
        for (T item : _list) {
            String keyword = _keywordsFunc.apply(item);
            if (keyword.length() < minLength) {
                continue;
            }
            String fpy = "";
            String[] pylist;
            if (_pinyinFunc == null) {
                pylist = PinyinDict.GetPinyinList(keyword, 0);
            } else {
                pylist = _pinyinFunc.apply(item).split( ((Character)_splitChar).toString());
            }
            for (int j = 0; j < pylist.length; j++) {
                pylist[j] = pylist[j].toUpperCase();
                fpy += pylist[j].charAt(0);
            }
            if (search.Find(fpy, keyword, pylist)) {
                result.add(item);
            }
        }
        return result;
    }

    public List<T> FindWithSpace(String keywords) throws Exception {
        if (_keywordsFunc == null) {
            throw new Exception("请先使用SetKeywordsFunc方法。");
        }
        keywords = keywords.toUpperCase().trim();
        if (keywords == null || keywords.equals("")) {
            return null;
        }
        if (keywords.contains(" ") == false) {
            return Find(keywords);
        }

        List<TwoTuple<String, String[]>> list = new ArrayList<TwoTuple<String, String[]>>();
        List<Integer> indexs = new ArrayList<Integer>();
        int minLength = 0;
        int keysCount;
        {

            String[] keys = keywords.split(" ");
            keysCount = keys.length;
            for (int i = 0; i < keys.length; i++) {
                String key = keys[i];
                List<String> pykeys = SplitKeywords(key);
                int min = Integer.MAX_VALUE;
                for (String pykey : pykeys) {
                    String[] keys2 = pykey.split(((Character) (char) 0).toString());
                    if (min > keys2.length) {
                        min = keys2.length;
                    }
                    MergeKeywords(keys2, 0, "", list, i, indexs);
                }
                minLength += min;
            }
        }

        PinyinSearch search = new PinyinSearch();
        search.SetKeywords2(list);
        search.SetIndexs(indexs);

        List<T> result = new ArrayList<T>();
        for (T item : _list) {
            String keyword = _keywordsFunc.apply(item);
            if (keyword.length() < minLength) {
                continue;
            }
            String fpy = "";
            String[] pylist;
            if (_pinyinFunc == null) {
                pylist = PinyinDict.GetPinyinList(keyword, 0);
            } else {
                pylist = _pinyinFunc.apply(item).split(((Character)_splitChar).toString());
            }
            for (int j = 0; j < pylist.length; j++) {
                pylist[j] = pylist[j].toUpperCase();
                fpy += pylist[j].charAt(0);
            }
            if (search.Find2(fpy, keyword, pylist, keysCount)) {
                result.add(item);
            }
        }
        return result;
    }

}