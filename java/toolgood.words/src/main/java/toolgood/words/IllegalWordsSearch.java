package toolgood.words;

import java.io.InputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.function.Function;

import toolgood.words.internals.BaseSearchEx;

/**
 * 最新版本的IllegalWordsSearch， 与2020.05.24以前的版本不兼容
 */
public class IllegalWordsSearch extends BaseSearchEx {
    public class SkipWordFilterHandler {
        public char c;
        public String text;
        public int index;

        public SkipWordFilterHandler(final char c, final String text, final int index) {
            this.c = c;
            this.text = text;
            this.index = index;
        }
    }

    public class CharTranslateHandler {
        public char c;
        public String text;
        public int index;

        public CharTranslateHandler(final char c, final String text, final int index) {
            this.c = c;
            this.text = text;
            this.index = index;
        }
    }

    public class StringMatchHandler {
        public String text;
        public int start;
        public int end;
        public String keyword;
        public int keywordIndex;
        public String matchKeyword;
        public int blacklistIndex;

        public StringMatchHandler(final String text, final int start, final int end, final String keyword,
                final int keywordIndex, final String matchKeyword, final int blacklistIndex) {
            this.text = text;
            this.start = start;
            this.end = end;
            this.keyword = keyword;
            this.keywordIndex = keywordIndex;
            this.matchKeyword = matchKeyword;
            this.blacklistIndex = blacklistIndex;
        }
    }

    /**
     * 使用跳词过滤器，默认使用
     */
    public boolean UseSkipWordFilter = true;
    private final String _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;"
            + "':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
    private boolean[] _skipBitArray;

    /** 过滤跳词 */
    public Function<SkipWordFilterHandler, Boolean> SkipWordFilter;
    /**
     * 字符转化，可以设置繁简转化、忽略大小写，启用后UseIgnoreCase开启无效
     * 若想使用CharTranslateHandler，请先添加事件CharTranslateHandler, 再用SetKeywords设置关键字
     */
    public Function<CharTranslateHandler, Character> CharTranslate;

    /**
     * 自定义字符串匹配
     */
    public Function<StringMatchHandler, Boolean> StringMatch;
    /**
     * 使用重复词过滤器
     */
    public boolean UseDuplicateWordFilter = true;
    /**
     * 使用黑名单过滤器
     */
    private int[] _blacklist = new int[0];
    /**
     * 使用半角转化器
     */
    public boolean UseDBCcaseConverter = true;
    /**
     * 使用忽略大小写
     */
    public boolean UseIgnoreCase = true;

    public IllegalWordsSearch() {
        _skipBitArray = new boolean[Character.MAX_VALUE + 1];
        for (int i = 0; i < _skipList.length(); i++) {
            _skipBitArray[_skipList.charAt(i)] = true;
        }
        SkipWordFilter = null;
        CharTranslate = null;
        StringMatch = null;
    }

    /**
     * 设置跳词
     *
     * @param skipList
     */
    public void SetSkipWords(final String skipList) {

        _skipBitArray = new boolean[Character.MAX_VALUE + 1];
        if (skipList != null) {
            for (int i = 0; i < skipList.length(); i++) {
                _skipBitArray[skipList.charAt(i)] = true;
            }
        }
    }

    /**
     * 设置关键字 如果想使用CharTranslateHandler，请先添加事件CharTranslateHandler,
     * 再用SetKeywords设置关键字 使用CharTranslateHandler后，UseIgnoreCase配置无效
     * 如果不使用忽略大小写，请先UseIgnoreCase设置为false,再用SetKeywords设置关键字
     * 
     * @param keywords
     */
    public void SetKeywords(final List<String> keywords) {
        if (CharTranslate != null) {
            final Set<String> kws = new HashSet<String>(keywords);
            final List<String> list = new ArrayList<String>();
            for (final String item : kws) {
                final StringBuilder sb = new StringBuilder();
                for (int i = 0; i < item.length(); i++) {
                    final char c = CharTranslate.apply(new CharTranslateHandler(item.charAt(i), item, i));
                    sb.append(c);
                }
                list.add(sb.toString());
            }
            super.SetKeywords(list);
        } else if (UseDBCcaseConverter || UseIgnoreCase) {
            final Set<String> kws = new HashSet<String>(keywords);
            final List<String> list = new ArrayList<String>();
            for (final String item : kws) {
                list.add(ToSenseWord(item));
            }
            super.SetKeywords(list);
        } else {
            super.SetKeywords(keywords);
        }
    }

    protected void Save(final FileOutputStream bw) throws IOException {
        super.Save(bw);

        bw.write(UseSkipWordFilter ? 1 : 0);
        bw.write(NumHelper.serialize(_skipBitArray.length));
        for (final boolean item : _skipBitArray) {
            bw.write(item ? 1 : 0);
        }

        bw.write(UseDuplicateWordFilter ? 1 : 0);
        bw.write(NumHelper.serialize(_blacklist.length));
        for (final int item : _blacklist) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(UseDBCcaseConverter ? 1 : 0);
        bw.write(UseIgnoreCase ? 1 : 0);
    }

    public void Load(final InputStream br) throws IOException {
        super.Load(br);

        UseSkipWordFilter = br.read() > 0;
        int length = NumHelper.read(br);
        _skipBitArray = new boolean[length];
        for (int i = 0; i < length; i++) {
            _skipBitArray[i] = br.read() > 0;
        }

        UseDuplicateWordFilter = br.read() > 0;
        length = NumHelper.read(br);
        _blacklist = new int[length];
        for (int i = 0; i < length; i++) {
            _blacklist[i] = NumHelper.read(br);
        }

        UseDBCcaseConverter = br.read() > 0;
        UseIgnoreCase = br.read() > 0;
    }

    /**
     * 在文本中查找所有的关键字
     *
     * @param text 文本
     * @return
     */
    public List<IllegalWordsSearchResult> FindAll(final String text) {
        final List<IllegalWordsSearchResult> results = new ArrayList<IllegalWordsSearchResult>();
        boolean hasSkipWordOrDuplicateWord = false;
        int p = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            char t1 = text.charAt(i);
            if (UseSkipWordFilter) {
                if (SkipWordFilter != null) {// 跳词跳过
                    if (SkipWordFilter.apply(new SkipWordFilterHandler(t1, text, i))) {
                        hasSkipWordOrDuplicateWord=true;
                        continue;
                    }
                } else if (_skipBitArray[t1]) {
                    hasSkipWordOrDuplicateWord=true;
                    continue;
                }
            }

            if (CharTranslate != null) { // 字符串转换
                t1 = CharTranslate.apply(new CharTranslateHandler(t1, text, i));
            } else if (UseDBCcaseConverter || UseIgnoreCase) {
                t1 = ToSenseWord(t1);
            }
            final int t = _dict[t1];
            if (t == 0) {
                pChar = t1;
                p = 0;
                hasSkipWordOrDuplicateWord=false;
                continue;
            }
            int next;
            if (p == 0 || t < _min[p] || t > _max[p]) {
                next = _first[t];
            } else {
                final int index = _nextIndex[p].IndexOf(t);
                if (index > -1) {
                    next = _nextIndex[p].GetValue(index);
                } else if (UseDuplicateWordFilter && pChar == t1) {
                    next = p;
                    hasSkipWordOrDuplicateWord=true;
                } else {
                    next = _first[t];
                    hasSkipWordOrDuplicateWord=false;
                }
            }

            if (next != 0) {
                if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        final int index = _resultIndex[j];
                        final IllegalWordsSearchResult r = hasSkipWordOrDuplicateWord ? GetGetIllegalResult(text, i, index) : GetIllegalResultByLength(text, i, index);
                        if (r != null) {
                            results.add(r);
                        }
                    }
                }
            }
            p = next;
            pChar = t1;
        }
        return results;
    }

    /**
     * 在文本中查找第一个关键字
     *
     * @param text 文本
     * @return
     */
    public IllegalWordsSearchResult FindFirst(final String text) {
        boolean hasSkipWordOrDuplicateWord = false;
        int p = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            char t1 = text.charAt(i);
            if (UseSkipWordFilter) {
                if (SkipWordFilter != null) {// 跳词跳过
                    if (SkipWordFilter.apply(new SkipWordFilterHandler(t1, text, i))) {
                        hasSkipWordOrDuplicateWord=true;
                        continue;
                    }
                } else if (_skipBitArray[t1]) {
                    hasSkipWordOrDuplicateWord=true;
                    continue;
                }
            }

            if (CharTranslate != null) { // 字符串转换
                t1 = CharTranslate.apply(new CharTranslateHandler(t1, text, i));
            } else if (UseDBCcaseConverter || UseIgnoreCase) {
                t1 = ToSenseWord(t1);
            }
            final int t = _dict[t1];
            if (t == 0) {
                pChar = t1;
                p = 0;
                hasSkipWordOrDuplicateWord=false;
                continue;
            }
            int next;
            if (p == 0 || t < _min[p] || t > _max[p]) {
                next = _first[t];
            } else {
                final int index = _nextIndex[p].IndexOf(t);
                if (index > -1) {
                    next = _nextIndex[p].GetValue(index);
                } else if (UseDuplicateWordFilter && pChar == t1) {
                    next = p;
                    hasSkipWordOrDuplicateWord=true;
                } else {
                    next = _first[t];
                    hasSkipWordOrDuplicateWord=false;
                }
            }

            if (next != 0) {
                if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        final int index = _resultIndex[j];
                        final IllegalWordsSearchResult r = hasSkipWordOrDuplicateWord ? GetGetIllegalResult(text, i, index) : GetIllegalResultByLength(text, i, index);
                        if (r != null) {
                            return r;
                        }
                    }
                }
            }
            p = next;
            pChar = t1;
        }
        return null;
    }

    /**
     * 判断文本是否包含关键字
     *
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(final String text) {
        boolean hasSkipWordOrDuplicateWord = false;
        int p = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            char t1 = text.charAt(i);
            if (UseSkipWordFilter) {
                if (SkipWordFilter != null) {// 跳词跳过
                    if (SkipWordFilter.apply(new SkipWordFilterHandler(t1, text, i))) {
                        hasSkipWordOrDuplicateWord=true;
                        continue;
                    }
                } else if (_skipBitArray[t1]) {
                    hasSkipWordOrDuplicateWord=true;
                    continue;
                }
            }

            if (CharTranslate != null) { // 字符串转换
                t1 = CharTranslate.apply(new CharTranslateHandler(t1, text, i));
            } else if (UseDBCcaseConverter || UseIgnoreCase) {
                t1 = ToSenseWord(t1);
            }
            final int t = _dict[t1];
            if (t == 0) {
                pChar = t1;
                p = 0;
                hasSkipWordOrDuplicateWord=false;
                continue;
            }
            int next;
            if (p == 0 || t < _min[p] || t > _max[p]) {
                next = _first[t];
            } else {
                final int index = _nextIndex[p].IndexOf(t);
                if (index > -1) {
                    next = _nextIndex[p].GetValue(index);
                } else if (UseDuplicateWordFilter && pChar == t1) {
                    next = p;
                    hasSkipWordOrDuplicateWord=true;
                } else {
                    next = _first[t];
                    hasSkipWordOrDuplicateWord=false;
                }
            }

            if (next != 0) {
                if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        final int index = _resultIndex[j];
                        final IllegalWordsSearchResult r = hasSkipWordOrDuplicateWord ? GetGetIllegalResult(text, i, index) : GetIllegalResultByLength(text, i, index);
                        if (r != null) {
                            return true;
                        }
                    }
                }
            }
            p = next;
            pChar = t1;
        }
        return false;
    }

    /**
     * 在文本中替换所有的关键字
     *
     * @param text 文本
     * @return
     */
    public String Replace(final String text) {
        return Replace(text, '*');
    }

    /**
     * 在文本中替换所有的关键字
     *
     * @param text        文本
     * @param replaceChar 文本
     * @return
     */
    public String Replace(final String text, final char replaceChar) {
        final StringBuilder result = new StringBuilder(text);

        boolean hasSkipWordOrDuplicateWord = false;
        int p = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            char t1 = text.charAt(i);
            if (UseSkipWordFilter) {
                if (SkipWordFilter != null) {// 跳词跳过
                    if (SkipWordFilter.apply(new SkipWordFilterHandler(t1, text, i))) {
                        hasSkipWordOrDuplicateWord=true;
                        continue;
                    }
                } else if (_skipBitArray[t1]) {
                    hasSkipWordOrDuplicateWord=true;
                    continue;
                }
            }

            if (CharTranslate != null) { // 字符串转换
                t1 = CharTranslate.apply(new CharTranslateHandler(t1, text, i));
            } else if (UseDBCcaseConverter || UseIgnoreCase) {
                t1 = ToSenseWord(t1);
            }
            final int t = _dict[t1];
            if (t == 0) {
                pChar = t1;
                p = 0;
                hasSkipWordOrDuplicateWord=false;
                continue;
            }
            int next;
            if (p == 0 || t < _min[p] || t > _max[p]) {
                next = _first[t];
            } else {
                final int index = _nextIndex[p].IndexOf(t);
                if (index > -1) {
                    next = _nextIndex[p].GetValue(index);
                } else if (UseDuplicateWordFilter && pChar == t1) {
                    next = p;
                    hasSkipWordOrDuplicateWord=true;
                } else {
                    next = _first[t];
                    hasSkipWordOrDuplicateWord=false;
                }
            }

            if (next != 0) {
                if (_end[next] < _end[next + 1] && CheckNextChar(text, t1, i)) {
                    for (int j = _end[next]; j < _end[next + 1]; j++) {
                        final int index = _resultIndex[j];
                        final IllegalWordsSearchResult r = hasSkipWordOrDuplicateWord ? GetGetIllegalResult(text, i, index) : GetIllegalResultByLength(text, i, index);
                        if (r != null) {
                            for (int k = r.Start; k <= r.End; k++) {
                                result.setCharAt(k, replaceChar);
                            }
                            break;
                        }
                    }
                }
            }
            p = next;
            pChar = t1;
        }
        return result.toString();
    }

    private boolean CheckNextChar(final String text, final char c, final int end) {
        if (IsEnglishOrNumber(c) == false) {
            return true;
        }
        if (end + 1 < text.length()) {
            char e1 = text.charAt(end + 1);
            if (UseSkipWordFilter) {
                if (SkipWordFilter != null) {// 跳词跳过
                    if (SkipWordFilter.apply(new SkipWordFilterHandler(e1, text, end + 1))) {
                        return true;
                    }
                } else if (_skipBitArray[e1]) {
                    return true;
                }
            }
            if (CharTranslate != null) { // 字符串转换
                e1 = CharTranslate.apply(new CharTranslateHandler(e1, text, end + 1));
            } else if (UseDBCcaseConverter || UseIgnoreCase) {
                e1 = ToSenseWord(e1);
            }
            if (IsEnglishOrNumber(e1)) {
                return false;
            }
        }
        return true;
    }
 
    private IllegalWordsSearchResult GetGetIllegalResult(String text, int end, int index)
    {
        String key = _keywords[index];

        int keyIndex = key.length() - 1;
        int start = end;
        for (int i = end; i >= 0; i--)
        {
            char s2 = text.charAt(i);
            if (UseSkipWordFilter)
            {
                if (SkipWordFilter != null)
                {
                    if (SkipWordFilter.apply(new SkipWordFilterHandler(s2, text, i))) { continue; }
                }
                else if (_skipBitArray[s2]) { continue; }
            }

            if (CharTranslate != null)
            { // 字符串转换
                s2 = CharTranslate.apply(new CharTranslateHandler(s2, text, i));
            }
            else if (UseDBCcaseConverter || UseIgnoreCase)
            {
                s2 = ToSenseWord(s2);
            }
            if (s2 == key.charAt(keyIndex))
            {
                keyIndex--;
                if (keyIndex == -1) { start = i; break; }
            }
        }
        for (int i = start; i >= 0; i--)
        {
            char s2 = text.charAt(i);
            if (CharTranslate != null)
            { // 字符串转换
                s2 = CharTranslate.apply(new CharTranslateHandler(s2, text, i));
            }
            else if (UseDBCcaseConverter || UseIgnoreCase)
            {
                s2 = ToSenseWord(s2);
            }
            if (s2 != key.charAt(0)) { break; }
            start = i;
        }
        return GetGetIllegalResult(text, key, start, end, index);
    }

    /// <summary>
    /// 没有跳词，没有重复词
    /// </summary>
    /// <param name="text"></param>
    /// <param name="end"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private IllegalWordsSearchResult GetIllegalResultByLength(String text, int end, int index)
    {
        String key = _keywords[index];
        int start = end - key.length() + 1;
        return GetGetIllegalResult(text, key, start, end, index);
    }

    private IllegalWordsSearchResult GetGetIllegalResult(String text, String key, int start, int end, int index)
    {
        if (start > 0) {
            char s1 = text.charAt(start);
            if (CharTranslate != null) { // 字符串转换
                s1 = CharTranslate.apply(new CharTranslateHandler(s1, text, start));
            }
            if (IsEnglishOrNumber(s1)) {
                char s2 = text.charAt(start - 1);
                if (CharTranslate != null) { // 字符串转换
                    s2 = CharTranslate.apply(new CharTranslateHandler(s2, text, start - 1));
                } else if (UseDBCcaseConverter || UseIgnoreCase) {
                    s2 = ToSenseWord(s2);
                }
                if (IsEnglishOrNumber(s2)) {
                    return null;
                }
            }
        }


        final String keyword = text.substring(start, end + 1);
        final int bl = _blacklist.length > index ? _blacklist[index] : 0;
        if (StringMatch != null) {
            if (StringMatch.apply(new StringMatchHandler(text, start, end, keyword, index, key, _blacklist[index]))) {
                return new IllegalWordsSearchResult(keyword, start, end, index, key, bl);
            }
            return null;
        }
        return new IllegalWordsSearchResult(keyword, start, end, index, key, bl);
    }

    /**
     * 设置黑名单
     *
     * @param blacklist
     * @throws IllegalArgumentException
     */
    public void SetBlacklist(final int[] blacklist) throws IllegalArgumentException {
        if (_keywords == null) {
            throw new IllegalArgumentException("请先使用SetKeywords方法设置关键字！");
        }
        if (blacklist.length != _keywords.length) {
            throw new IllegalArgumentException("请关键字与黑名单列表的长度要一样长！");
        }
        _blacklist = blacklist;
    }

    private Boolean IsEnglishOrNumber(final char c) {
        if (c < 128) {
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                return true;
            }
        }
        return false;
    }

    private String ToSenseWord(final String text) {
        final StringBuilder stringBuilder = new StringBuilder(text.length());
        for (int i = 0; i < text.length(); i++) {
            stringBuilder.append(ToSenseWord(text.charAt(i)));
        }
        return stringBuilder.toString();
    }

    private Character ToSenseWord(final Character c) {

        if (UseIgnoreCase) {
            if (c >= 'A' && c <= 'Z')
                return (char) (c | 0x20);
        }
        if (UseDBCcaseConverter) {
            if (c == 12288)
                return ' ';
            if (c >= 65280 && c < 65375) {
                Character k = (char) (c - 65248);
                if (UseIgnoreCase) {
                    if ('A' <= k && k <= 'Z') {
                        k = (char) (k | 0x20);
                    }
                }
                return (char) k;
            }
        }
        return c;
    }

}