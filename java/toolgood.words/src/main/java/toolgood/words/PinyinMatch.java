package toolgood.words;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import toolgood.words.internals.BasePinyinMatch;
import toolgood.words.internals.PinyinDict;
import toolgood.words.internals.TwoTuple;

public class PinyinMatch extends BasePinyinMatch {
    private String[] _keywords;
    private String[] _keywordsFirstPinyin;
    private String[][] _keywordsPinyin;
    private int[] _indexs;

    /**
     * 设置关键字，注：索引会被清空
     * 
     * @param keywords
     * @throws NumberFormatException
     * @throws IOException
     */
    public void SetKeywords(final List<String> keywords) throws NumberFormatException, IOException {
        _keywords = keywords.toArray(new String[0]);
        _keywordsFirstPinyin = new String[_keywords.length];
        _keywordsPinyin = new String[_keywords.length][];
        for (int i = 0; i < _keywords.length; i++) {
            final String text = _keywords[i];
            final String[] pys = PinyinDict.GetPinyinList(text, 0);
            String fpy = "";
            for (int j = 0; j < pys.length; j++) {
                pys[j] = pys[j].toUpperCase();
                fpy += pys[j].charAt(0);
            }
            _keywordsPinyin[i] = pys;
            _keywordsFirstPinyin[i] = fpy;
        }
        _indexs = null;
    }

    /**
     * 设置关键字，注：索引会被清空
     * 
     * @param keywords
     * @param pinyin
     */
    public void SetKeywords(final List<String> keywords, final List<String> pinyin) {
        SetKeywords(keywords, pinyin, ',');
    }

    /**
     * 设置关键字，注：索引会被清空
     * 
     * @param keywords
     * @param pinyin
     * @param splitChar
     */
    public void SetKeywords(final List<String> keywords, final List<String> pinyin, final char splitChar) {
        _keywords = keywords.toArray(new String[0]);
        _keywordsFirstPinyin = new String[_keywords.length];
        _keywordsPinyin = new String[_keywords.length][];
        for (int i = 0; i < _keywords.length; i++) {
            final String text = pinyin.get(i);
            final String[] pys = text.split(((Character) splitChar).toString());
            String fpy = "";
            for (int j = 0; j < pys.length; j++) {
                pys[j] = pys[j].toUpperCase();
                fpy += pys[j].charAt(0);
            }
            _keywordsPinyin[i] = pys;
            _keywordsFirstPinyin[i] = fpy;
        }
        _indexs = null;
    }

    /**
     * 设置索引
     * 
     * @param indexs
     * @throws Exception
     */
    public void SetIndexs(final List<Integer> indexs) throws Exception {
        if (_keywords == null) {
            throw new Exception("请先使用 SetKeywords 方法");
        }
        if (indexs.size() < _keywords.length) {
            throw new Exception("indexs 数组长度大于 keywords");
        }
        _indexs = new int[indexs.size()];
        for (int i = 0; i < indexs.size(); i++) {
            _indexs[i] = indexs.get(i);
        }
    }

    /**
     * 查询
     * 
     * @param key
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public List<String> Find(String key) throws NumberFormatException, IOException {
        key = key.toUpperCase().trim();
        if (key == null || key.equals("")) {
            return null;
        }

        final boolean hasPinyin = key.matches("^.*?[A-Z]+.*$");// Pattern.matches("[a-zA-Z]",key);
        if (hasPinyin == false) {
            final List<String> rs = new ArrayList<String>();
            for (int i = 0; i < _keywords.length; i++) {
                final String keyword = _keywords[i];
                if (keyword.contains(key)) {
                    rs.add(keyword);
                }
            }
            return rs;
        }

        final List<String> pykeys = SplitKeywords(key);
        int minLength = Integer.MAX_VALUE;
        final List<TwoTuple<String, String[]>> list = new ArrayList<TwoTuple<String, String[]>>();
        for (final String pykey : pykeys) {
            final String[] keys = pykey.split(((Character) (char) 0).toString());
            if (minLength > keys.length) {
                minLength = keys.length;
            }
            MergeKeywords(keys, 0, "", list);
        }

        final PinyinSearch search = new PinyinSearch();
        search.SetKeywords2(list);
        final List<String> result = new ArrayList<String>();
        for (int i = 0; i < _keywords.length; i++) {
            final String keywords = _keywords[i];
            if (keywords.length() < minLength) {
                continue;
            }
            final String fpy = _keywordsFirstPinyin[i];
            final String[] pylist = _keywordsPinyin[i];

            if (search.Find(fpy, keywords, pylist)) {
                result.add(keywords);
            }
        }
        return result;
    }

    /**
     * 查询索引
     * 
     * @param key
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public List<Integer> FindIndex(String key) throws NumberFormatException, IOException {
        key = key.toUpperCase().trim();
        if (key == null || key.equals("")) {
            return null;
        }
        final boolean hasPinyin = key.matches("^.*?[A-Z]+.*$");// Pattern.matches("[a-zA-Z]",key);
        if (hasPinyin == false) {
            final List<Integer> rs = new ArrayList<Integer>();
            for (int i = 0; i < _keywords.length; i++) {
                final String keyword = _keywords[i];
                if (keyword.contains(key)) {
                    if (_indexs == null) {
                        rs.add(i);
                    } else {
                        rs.add(_indexs[i]);
                    }
                }
            }
            return rs;
        }

        final List<String> pykeys = SplitKeywords(key);
        int minLength = Integer.MAX_VALUE;
        final List<TwoTuple<String, String[]>> list = new ArrayList<TwoTuple<String, String[]>>();
        for (final String pykey : pykeys) {
            final String[] keys = pykey.split(((Character) (char) 0).toString());
            if (minLength > keys.length) {
                minLength = keys.length;
            }
            MergeKeywords(keys, 0, "", list);
        }

        final PinyinSearch search = new PinyinSearch();
        search.SetKeywords2(list);
        final List<Integer> result = new ArrayList<Integer>();
        for (int i = 0; i < _keywords.length; i++) {
            final String keywords = _keywords[i];
            if (keywords.length() < minLength) {
                continue;
            }
            final String fpy = _keywordsFirstPinyin[i];
            final String[] pylist = _keywordsPinyin[i];
            if (search.Find(fpy, keywords, pylist)) {
                if (_indexs == null) {
                    result.add(i);
                } else {
                    result.add(_indexs[i]);
                }
            }
        }
        return result;
    }

    /**
     * 查询，空格为通配符
     * 
     * @param keywords
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public List<String> FindWithSpace(String keywords) throws NumberFormatException, IOException {
        keywords = keywords.toUpperCase().trim();
        if (keywords == null || keywords.equals("")) {
            return null;
        }
        if (keywords.contains(" ") == false) {
            return Find(keywords);
        }

        final List<TwoTuple<String, String[]>> list = new ArrayList<TwoTuple<String, String[]>>();
        final List<Integer> indexs = new ArrayList<Integer>();
        int minLength = 0;
        int keysCount;
        {
            final String[] keys = keywords.split(" ");
            keysCount = keys.length;
            for (int i = 0; i < keys.length; i++) {
                final String key = keys[i];
                final List<String> pykeys = SplitKeywords(key);
                int min = Integer.MAX_VALUE;
                for (final String pykey : pykeys) {
                    final String[] keys2 = pykey.split(((Character) (char) 0).toString());
                    if (min > keys2.length) {
                        min = keys2.length;
                    }
                    MergeKeywords(keys2, 0, "", list, i, indexs);
                }
                minLength += min;
            }
        }

        final PinyinSearch search = new PinyinSearch();
        search.SetKeywords2(list);
        search.SetIndexs(indexs);

        final List<String> result = new ArrayList<String>();
        for (int i = 0; i < _keywords.length; i++) {
            final String keywords2 = _keywords[i];
            if (keywords2.length() < minLength) {
                continue;
            }
            final String fpy = _keywordsFirstPinyin[i];
            final String[] pylist = _keywordsPinyin[i];

            if (search.Find2(fpy, keywords2, pylist, keysCount)) {
                result.add(keywords2);
            }
        }
        return result;
    }

    /**
     * 查询索引号，空格为通配符
     * 
     * @param keywords
     * @return
     * @throws NumberFormatException
     * @throws IOException
     */
    public List<Integer> FindIndexWithSpace(String keywords) throws NumberFormatException, IOException {
        keywords = keywords.toUpperCase().trim();
        if (keywords == null || keywords.equals("")) {
            return null;
        }
        if (keywords.contains(" ") == false) {
            return FindIndex(keywords);
        }

        final List<TwoTuple<String, String[]>> list = new ArrayList<TwoTuple<String, String[]>>();
        final List<Integer> indexs = new ArrayList<Integer>();
        int minLength = 0;
        int keysCount;
        {
            final String[] keys = keywords.split(" ");
            keysCount = keys.length;
            for (int i = 0; i < keys.length; i++) {
                final String key = keys[i];
                final List<String> pykeys = SplitKeywords(key);
                int min = Integer.MAX_VALUE;
                for (final String pykey : pykeys) {
                    final String[] keys2 = pykey.split(((Character) (char) 0).toString());
                    if (min > keys2.length) {
                        min = keys2.length;
                    }
                    MergeKeywords(keys2, 0, "", list, i, indexs);
                }
                minLength += min;
            }
        }

        final PinyinSearch search = new PinyinSearch();
        search.SetKeywords2(list);
        search.SetIndexs(indexs);

        final List<Integer> result = new ArrayList<Integer>();
        for (int i = 0; i < _keywords.length; i++) {
            final String keywords2 = _keywords[i];
            if (keywords2.length() < minLength) {
                continue;
            }
            final String fpy = _keywordsFirstPinyin[i];
            final String[] pylist = _keywordsPinyin[i];
            if (search.Find2(fpy, keywords2, pylist, keysCount)) {
                if (_indexs == null) {
                    result.add(i);
                } else {
                    result.add(_indexs[i]);
                }
            }
        }
        return result;
    }
}