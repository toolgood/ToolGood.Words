package toolgood.words;

import java.io.InputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import toolgood.words.internals.BaseSearchEx;
import toolgood.words.internals.Dict;

/**
 *
 */
public class IllegalWordsSearch extends BaseSearchEx {
    /**
     * 使用跳词过滤器
     */
    public boolean UseSkipWordFilter = false;
    private String _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;" +
            "':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
    private boolean[] _skipBitArray;
    /**
     * 使用重复词过滤器
     */
    public boolean UseDuplicateWordFilter = false;
    /**
     * 使用黑名单过滤器
     */
    public boolean UseBlacklistFilter = false;
    private int[] _blacklist;
    /**
     * 使用半角转化器
     */
    public boolean UseDBCcaseConverter = true;
    /**
     * 使用简体中文转化器
     */
    public boolean UseSimplifiedChineseConverter = true;
    /**
     * 使用忽略大小写
     */
    public boolean UseIgnoreCase = true;

    public IllegalWordsSearch() {
        _skipBitArray = new boolean[Character.MAX_VALUE + 1];
        for (int i = 0; i < _skipList.length(); i++) {
            _skipBitArray[_skipList.charAt(i)] = true;
        }
        _blacklist = new int[0];
    }

    /**
     * 在文本中查找所有的关键字
     *
     * @param text 文本
     * @return
     */
    public List<IllegalWordsSearchResult> FindAll(String text) {
        return FindAll(text, '*');
    }

    /**
     * 在文本中查找所有的关键字
     *
     * @param text 文本
     * @param flag 黑名单
     * @return
     */
    public List<IllegalWordsSearchResult> FindAll(String text, int flag) {
        List<IllegalWordsSearchResult> results = new ArrayList<IllegalWordsSearchResult>();
        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) {
                            results.add(r);
                        }
                    }
                }
            }

            Character c = text.charAt(i);
            if (UseSkipWordFilter && _skipBitArray[c]) {
                findIndex = 0;
                continue;
            }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) {
                p = 0;
                pChar = c;
                continue;
            }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false) {
                if (UseDuplicateWordFilter && pChar == c) {
                    continue;
                }
                if (p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }
            pChar = c;
        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) {
                    results.add(r);
                }
            }
        }
        return results;
    }

    /**
     * 在文本中查找第一个关键字
     *
     * @param text 文本
     * @return
     */
    public IllegalWordsSearchResult FindFirst(String text) {
        return FindFirst(text, Integer.MAX_VALUE);
    }

    /**
     * 在文本中查找第一个关键字
     *
     * @param text 文本
     * @param flag 黑名单
     * @return
     */
    public IllegalWordsSearchResult FindFirst(String text, int flag) {
        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) {
                            return r;
                        }
                    }
                }
            }

            Character c = text.charAt(i);
            if (UseSkipWordFilter && _skipBitArray[c]) {
                findIndex = 0;
                continue;
            }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) {
                p = 0;
                pChar = c;
                continue;
            }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false) {
                if (UseDuplicateWordFilter && pChar == c) {
                    continue;
                }
                if (p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }
            pChar = c;
        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) {
                    return r;
                }
            }
        }
        return null;
    }

    /**
     * 判断文本是否包含关键字
     *
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(String text) {
        return ContainsAny(text, Integer.MAX_VALUE);
    }

    /**
     * 判断文本是否包含关键字
     *
     * @param text 文本
     * @param flag 黑名单
     * @return
     */
    public boolean ContainsAny(String text, int flag) {
        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        char pChar = (char) 0;

        for (int i = 0; i < text.length(); i++) {
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) {
                            return true;
                        }
                    }
                }
            }

            Character c = text.charAt(i);
            if (UseSkipWordFilter && _skipBitArray[c]) {
                findIndex = 0;
                continue;
            }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) {
                p = 0;
                pChar = c;
                continue;
            }//不在字表中，跳过


            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false) {
                if (UseDuplicateWordFilter && pChar == c) {
                    continue;
                }
                if (p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }
            pChar = c;
        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) {
                    return true;
                }
            }
        }
        return false;
    }

    /**
     * 在文本中替换所有的关键字
     *
     * @param text 文本
     * @return
     */
    public String Replace(String text) {
        return Replace(text, '*', Integer.MAX_VALUE);
    }

    /**
     * 在文本中替换所有的关键字
     *
     * @param text        文本
     * @param replaceChar 替换符
     * @return
     */
    public String Replace(String text, char replaceChar) {
        return Replace(text, replaceChar, Integer.MAX_VALUE);
    }

    /**
     * 在文本中替换所有的关键字
     *
     * @param text        文本
     * @param replaceChar 替换符
     * @param flag        黑名单
     * @return
     */
    public String Replace(String text, char replaceChar, int flag) {
        StringBuilder result = new StringBuilder(text);

        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        char pChar = (char) 0;


        for (int i = 0; i < text.length(); i++) {
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) {
                            for (int j = r.Start; j < i; j++) {
                                result.setCharAt(j, replaceChar);
                            }
                            break;
                        }
                    }

                }
            }

            Character c = text.charAt(i);
            if (UseSkipWordFilter && _skipBitArray[c]) {
                findIndex = 0;
                continue;
            }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) {
                p = 0;
                pChar = c;
                continue;
            }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false) {
                if (UseDuplicateWordFilter && pChar == c) {
                    continue;
                }
                if (p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }
            pChar = c;
        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) {
                    for (int j = r.Start; j < text.length(); j++) {
                        result.setCharAt(j, replaceChar);
                    }
                    break;
                }
            }
        }
        return result.toString();
    }

    private int FindStart(String keyword, int end, String srcText, int p, int[] pIndex) {
        if (end + 1 < srcText.length()) {
            boolean en1 = IsEnglishOrNumber(srcText.charAt(end + 1));
            boolean en2 = IsEnglishOrNumber(srcText.charAt(end));
            if (en1 && en2) {
                return -1;
            }
        }
        int n = keyword.length();
        int start = end;
        int pp = p;
        while (n > 0) {
            int pi = pIndex[start--];
            if (pi != pp) {
                n--;
                pp = pi;
            }
            if (start == -1) return 0;
        }
        boolean sn1 = IsEnglishOrNumber(srcText.charAt(start++));
        boolean sn2 = IsEnglishOrNumber(srcText.charAt(start));
        if (sn1 && sn2) {
            return -1;
        }
        return start;
    }

    private IllegalWordsSearchResult GetIllegalResult(int index, int end, String srcText, int p, int[] pIndex, int
            flag) {
        if (UseBlacklistFilter) {
            int b = _blacklist[index];
            if ((b | flag) != b) {
                return null;
            }
        }
        String keyword = _keywords[index];
        if (keyword.length() == 1) {
            if (ToSenseWord(srcText.charAt(end)).equals(ToSenseWord(keyword.charAt(0))) == false) {
                return null;
            }
            return new IllegalWordsSearchResult(keyword, end, end, srcText);
        }
        int start = FindStart(keyword, end, srcText, p, pIndex);
        if (start == -1) {
            return null;
        }
        if (ToSenseWord(srcText.charAt(start)).equals(ToSenseWord(keyword.charAt(0))) == false) {
            return null;
        }
        if (UseBlacklistFilter) {
            return new IllegalWordsSearchResult(keyword, start, end, srcText, _blacklist[index]);
        }
        return new IllegalWordsSearchResult(keyword, start, end, srcText);
    }


    protected void Save(FileOutputStream bw) throws IOException {
        super.Save(bw);

        bw.write(UseSkipWordFilter ? 1 : 0);
        bw.write(NumHelper.serialize(_skipBitArray.length));
        for (boolean item : _skipBitArray) {
            bw.write(item ? 1 : 0);
        }

        bw.write(UseDuplicateWordFilter ? 1 : 0);
        bw.write(UseBlacklistFilter ? 1 : 0);
        bw.write(NumHelper.serialize(_blacklist.length));
        for (int item : _blacklist) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(UseDBCcaseConverter ? 1 : 0);
        bw.write(UseSimplifiedChineseConverter ? 1 : 0);
        bw.write(UseIgnoreCase ? 1 : 0);
    }

    public void Load(InputStream br) throws IOException {
        super.Load(br);

        UseSkipWordFilter = br.read() > 0;
        int length = NumHelper.read(br);
        _skipBitArray = new boolean[length];
        for (int i = 0; i < length; i++) {
            _skipBitArray[i] = br.read() > 0;
        }

        UseDuplicateWordFilter = br.read() > 0;
        UseBlacklistFilter = br.read() > 0;
        length = NumHelper.read(br);
        _blacklist = new int[length];
        for (int i = 0; i < length; i++) {
            _blacklist[i] = NumHelper.read(br);
        }

        UseDBCcaseConverter = br.read() > 0;
        UseSimplifiedChineseConverter = br.read() > 0;
        UseIgnoreCase = br.read() > 0;
    }

    /**
     * 设置跳词
     *
     * @param skipList
     */
    public void SetSkipWords(String skipList) {

        _skipBitArray = new boolean[Character.MAX_VALUE + 1];
        if (skipList == null) {
            for (int i = 0; i < _skipList.length(); i++) {
                _skipBitArray[_skipList.charAt(i)] = true;
            }
        }
    }

    /**
     * 设置黑名单
     *
     * @param blacklist
     * @throws IllegalArgumentException
     */
    public void SetBlacklist(int[] blacklist) throws IllegalArgumentException {
        if (_keywords == null) {
            throw new IllegalArgumentException("请先使用SetKeywords方法设置关键字！");
        }
        if (blacklist.length != _keywords.length) {
            throw new IllegalArgumentException("请关键字与黑名单列表的长度要一样长！");
        }
        _blacklist = blacklist;
    }

    /**
     * 设置关键字
     *
     * @param keywords
     */
    public void SetKeywords(List<String> keywords) {
        Set<String> kws = new HashSet<String>(keywords);
        List<String> list = new ArrayList<String>();
        for (String item : kws) {
            list.add(ToSenseWord(item));
        }
        super.SetKeywords(list);
    }

    private Boolean IsEnglishOrNumber(Character c) {
        if (c < 128) {
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                return true;
            }
        }
        return false;
    }

    private String ToSenseWord(String text) {
        StringBuilder stringBuilder = new StringBuilder(text.length());
        for (int i = 0; i < text.length(); i++) {
            stringBuilder.append(ToSenseWord(text.charAt(i)));
        }
        return stringBuilder.toString();
    }

    private Character ToSenseWord(Character c) {

        if (UseIgnoreCase) {
            if (c >= 'A' && c <= 'Z') return (char) (c | 0x20);
        }
        if (UseDBCcaseConverter) {
            if (c == 12288) return ' ';
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
        if (UseSimplifiedChineseConverter) {
            if (c >= 0x4e00 && c <= 0x9fa5) {
                return Dict.Simplified.charAt(c - 0x4e00);
            }
        }
        return c;
    }


}