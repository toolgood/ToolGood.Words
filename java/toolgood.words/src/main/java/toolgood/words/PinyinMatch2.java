package toolgood.words;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

import toolgood.words.internals.BasePinyinMatch;
import toolgood.words.internals.PinyinDict;
import toolgood.words.internals.TwoTuple;

public class PinyinMatch2<T> extends BasePinyinMatch {
    private List<T> _list;
    private Function<T, String> _keywordsFunc;
    private Function<T, String> _pinyinFunc;
    private char _splitChar = ',';

    /**
     * 拼音匹配, 不支持[0x20000-0x2B81D]
     * @param list
     */
    public PinyinMatch2(List<T> list) {
        _list = list;
        _keywordsFunc = null;
        _pinyinFunc =null;
    }
/**
 * 设置获取关键字的方法
 * @param keywordsFunc
 */
    public void SetKeywordsFunc(Function<T, String> keywordsFunc) {
        _keywordsFunc = keywordsFunc;
    }
/**
 * 设置获取拼音的方法
 * @param pinyinFunc
 */
    public void SetPinyinFunc(Function<T, String> pinyinFunc) {
        _pinyinFunc = pinyinFunc;
    }
/**
 * 设置拼音分隔符
 * @param splitChar
 */
    public void SetPinyinSplitChar(char splitChar) {
        _splitChar = splitChar;
    }

    /**
     * 查询
     * @param keywords
     * @return
     * @throws Exception
     */
    public List<T> Find(String keywords) throws Exception {
        if (_keywordsFunc == null) {
            throw new Exception("请先使用SetKeywordsFunc方法。");
        }
        keywords = keywords.toUpperCase().trim();
        if (keywords == null || keywords.equals("")) {
            return null;
        }
        List<T> result = new ArrayList<T>();
        boolean hasPinyin =keywords.matches("^.*?[A-Z]+.*$");// Pattern.matches("[a-zA-Z]",key);
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

    /**
     * 查询，空格为通配符
     * @param keywords
     * @return
     * @throws Exception
     */
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