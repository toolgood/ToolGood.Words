package toolgood.words;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import toolgood.words.internals.BaseSearchEx;

/***
 * 错字搜索--英文数字搜索,不包含中文转数字
 * 主要检测网址，推广账号
 */
public class StringTypoSearch extends BaseSearchEx {
    /**使用跳词过滤器 */
    public boolean UseSkipWordFilter = false; //使用跳词过滤器
    protected String _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
    protected boolean[] _skipBitArray;
    /**使用黑名单 */
    public boolean UseBlacklistFilter = false;
    protected int[] _blacklist;
    private int _maxJump = 5;//设置一个最大大跳跃点
    private char[] toWord;
    /**使用忽略大小写 */
    public boolean UseIgnoreCase = true;


    private String baseTypo =
    "0 0\r1 1\r2 2\r3 3\r4 4\r5 5\r6 6\r7 7\r8 8\r9 9\r" +
    "０ 0\r１ 1\r２ 2\r３ 3\r４ 4\r５ 5\r６ 6\r７ 7\r８ 8\r９ 9\r" +
    "A a\rB b\rC c\rD d\rE e\rF f\rG g\rH h\rI i\rJ j\rK k\rL l\rM m\rN n\rO o\rP p\rQ q\rR r\rS s\rT t\rU u\rV v\rW w\rX x\rY y\rZ z\r" +
    "a a\rb b\rc c\rd d\re e\rf f\rg g\rh h\ri i\rj j\rk k\rl l\rm m\rn n\ro o\rp p\rq q\rr r\rs s\rt t\ru u\rv v\rw w\rx x\ry y\rz z\r" +
    "Ａ a\rＢ b\rＣ c\rＤ d\rＥ e\rＦ f\rＧ g\rＨ h\rＩ i\rＪ j\rＫ k\rＬ l\rＭ m\rＮ n\rＯ o\rＰ p\rＱ q\rＲ r\rＳ s\rＴ t\rＵ u\rＶ v\rＷ w\rＸ x\rＹ y\rＺ z\r" +
    "ａ a\rｂ b\rｃ c\rｄ d\rｅ e\rｆ f\rｇ g\rｈ h\rｉ i\rｊ j\rｋ k\rｌ l\rｍ m\rｎ n\rｏ o\rｐ p\rｑ q\rｒ r\rｓ s\rｔ t\rｕ u\rｖ v\rｗ w\rｘ x\rｙ y\rｚ z\r" +
    "# #\r$ $\r% %\r& &\r+ +\r- -\r. .\r/ /\r: :\r= =\r? ?\r@ @\r_ _\r\\ \\\r" +
    "＃ #\r＄ $\r％ %\r＆ &\r＋ +\r－ -\r． .\r／ /\r： :\r＝ =\r？ ?\r＠ @\r＿ _\r＼ \\\r";
    private String defaultTypo = "⓪ 0\r零 0\rº 0\r₀ 0\r⓿ 0\r○ 0\r〇 0\r" +
                                  "⒜ a\r⒝ b\r⒞ c\r⒟ d\r⒠ e\r⒡ f\r⒢ g\r⒣ h\r⒤ i\r⒥ j\r⒦ k\r⒧ l\r⒨ m\r⒩ n\r⒪ o\r⒫ p\r⒬ q\r⒭ r\r⒮ s\r⒯ t\r⒰ u\r⒱ v\r⒲ w\r⒳ x\r⒴ y\r⒵ z\r" +
                                  "Ⓐ a\rⒷ b\rⒸ c\rⒹ d\rⒺ e\rⒻ f\rⒼ g\rⒽ h\rⒾ i\rⒿ j\rⓀ k\rⓁ l\rⓂ m\rⓃ n\rⓄ o\rⓅ p\rⓆ q\rⓇ r\rⓈ s\rⓉ t\rⓊ u\rⓋ v\rⓌ w\rⓍ x\rⓎ y\rⓏ z\r" +
                                  "ⓐ a\rⓑ b\rⓒ c\rⓓ d\rⓔ e\rⓕ f\rⓖ g\rⓗ h\rⓘ i\rⓙ j\rⓚ k\rⓛ l\rⓜ m\rⓝ n\rⓞ o\rⓟ p\rⓠ q\rⓡ r\rⓢ s\rⓣ t\rⓤ u\rⓥ v\rⓦ w\rⓧ x\rⓨ y\rⓩ z\r" +
                                 "一 1\r二 2\r三 3\r四 4\r五 5\r六 6\r七 7\r八 8\r九 9\r" +
                                 "壹 1\r贰 2\r叁 3\r肆 4\r伍 5\r陆 6\r柒 7\r捌 8\r玖 9\r" +
                                  "。 .\r点 .\r點 .\r— 1\r貮 2\r参 3\r陸 6\r" +
                                 "¹ 1\r² 2\r³ 3\r⁴ 4\r⁵ 5\r⁶ 6\r⁷ 7\r⁸ 8\r⁹ 9\r" +
                                 "₁ 1\r₂ 2\r₃ 3\r₄ 4\r₅ 5\r₆ 6\r₇ 7\r₈ 8\r₉ 9\r" +
                                 "① 1\r② 2\r③ 3\r④ 4\r⑤ 5\r⑥ 6\r⑦ 7\r⑧ 8\r⑨ 9\r" +
                                 "⑴ 1\r⑵ 2\r⑶ 3\r⑷ 4\r⑸ 5\r⑹ 6\r⑺ 7\r⑻ 8\r⑼ 9\r" +
                                 "⒈ 1\r⒉ 2\r⒊ 3\r⒋ 4\r⒌ 5\r⒍ 6\r⒎ 7\r⒏ 8\r⒐ 9\r" +
                                 "❶ 1\r❷ 2\r❸ 3\r❹ 4\r❺ 5\r❻ 6\r❼ 7\r❽ 8\r❾ 9\r" +
                                 "➀ 1\r➁ 2\r➂ 3\r➃ 4\r➄ 5\r➅ 6\r➆ 7\r➇ 8\r➈ 9\r" +
                                 "➊ 1\r➋ 2\r➌ 3\r➍ 4\r➎ 5\r➏ 6\r➐ 7\r➑ 8\r➒ 9\r" +
                                 "㈠ 1\r㈡ 2\r㈢ 3\r㈣ 4\r㈤ 5\r㈥ 6\r㈦ 7\r㈧ 8\r㈨ 9\r" +
                                 "⓵ 1\r⓶ 2\r⓷ 3\r⓸ 4\r⓹ 5\r⓺ 6\r⓻ 7\r⓼ 8\r⓽ 9\r" +
                                 "㊀ 1\r㊁ 2\r㊂ 3\r㊃ 4\r㊄ 5\r㊅ 6\r㊆ 7\r㊇ 8\r㊈ 9\r";



    public StringTypoSearch()
    {
        _skipBitArray = new boolean[Character.MAX_VALUE + 1];
        for (int i = 0; i < _skipList.length(); i++) {
            _skipBitArray[_skipList.charAt(i)] = true;
        }
        _blacklist = new int[0];

        toWord = new char[Character.MAX_VALUE + 1];
        buildWordConverter(GetTypos());
    }
    protected String GetTypos()
    {
        return defaultTypo + baseTypo;
    }


    private void buildWordConverter(String typoText)
    {
        String[] ts = typoText.replace("\n", "").split("\r");
        for (String t : ts) {
            String[] sp=t.split("[ \t]");
            if (sp.length<2) { continue; }
            toWord[(int)sp[0].charAt(0)] = sp[1].charAt(0);
        }
    }

    /**
     * 在文本中查找所有的关键字
     * @param text 文本
     * @return
     */
    public List<IllegalWordsSearchResult> FindAll(String text){
        return FindAll(text,'*');
    }
    /**
     * 在文本中查找所有的关键字
     * @param text 文本
     * @param flag 黑名单
     * @return
     */
    public List<IllegalWordsSearchResult> FindAll(String text, int flag)
    {
        List<IllegalWordsSearchResult> results = new ArrayList<IllegalWordsSearchResult>();
        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        int jump = 0;

        for (int i = 0; i < text.length(); i++) {
            char c = toWord[text.charAt(i)];
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0 && c == 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) { results.add(r); }
                    }
                }
            }

            if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false && p != 0) {
                p = 0;
                next = _next[0] + t;
                find = _key[next] == t;
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }

        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) {results.add(r);}
            }
        }
        return results;
    }
    /**
     * 在文本中查找第一个关键字
     * @param text 文本
     * @return
     */
    public IllegalWordsSearchResult FindFirst(String text){
        return FindFirst(text,Integer.MAX_VALUE);
    }
    /**
     * 在文本中查找第一个关键字
     * @param text 文本
     * @param flag 黑名单
     * @return
     */
    public IllegalWordsSearchResult FindFirst(String text, int flag)
    {
        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        int jump = 0;

        for (int i = 0; i < text.length();i++) {
            char c = toWord[text.charAt(i)];
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0 && c == 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) { return r; }
                    }
                }
            }

            if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false && p != 0) {
                p = 0;
                next = _next[0] + t;
                find = _key[next] == t;
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }

        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) { return r; }
            }
        }
        return null;
    }
    /**
     * 判断文本是否包含关键字
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(String text)
    {
        return ContainsAny(text,Integer.MAX_VALUE);
    }
    /**
     * 判断文本是否包含关键字
     * @param text 文本
     * @param flag黑名单
     * @return
     */
    public boolean ContainsAny(String text, int flag)
    {
        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        int jump = 0;

        for (int i = 0; i < text.length(); i++) {
            char c = toWord[text.charAt(i)];
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0 && c == 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if (r != null) { return true; }
                    }
                }
            }

            if (UseSkipWordFilter && _skipBitArray[(int)c]) { findIndex = 0; continue; }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false && p != 0) {
                p = 0;
                next = _next[0] + t;
                find = _key[next] == t;
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }
        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if (r != null) { return true; }
            }
        }
        return false;
    }

    /***
     * 在文本中替换所有的关键字, 替换符默认为 *
     * @param text 文本
     * @return
     */
    public String Replace(String text)
    {
        return Replace(text, '*', Integer.MAX_VALUE);
    }
    /**
     * 在文本中替换所有的关键字
     * @param text 文本
     * @param replaceChar 替换符
     * @return
     */
    public String Replace(String text, char replaceChar)
    {
        return Replace(text, replaceChar, Integer.MAX_VALUE);
    }
    /**
     * 在文本中替换所有的关键字
     * @param text 文本
     * @param replaceChar 替换符
     * @param flag 黑名单
     * @return
     */
    public String Replace(String text, char replaceChar, int flag)
    {
        StringBuilder result = new StringBuilder(text);

        int[] pIndex = new int[text.length()];
        int p = 0;
        int findIndex = 0;
        int jump = 0;

        for (int i = 0; i < text.length(); i++) {
            char c = toWord[text.charAt(i)];
            if (p != 0) {
                pIndex[i] = p;
                if (findIndex != 0 && c == 0) {
                    for (int item : _guides[findIndex]) {
                        IllegalWordsSearchResult r = GetIllegalResult(item, i - 1, text, p, pIndex, flag);
                        if(r!=null){
                            for (int j = r.Start; j < i; j++) { result.setCharAt(j, replaceChar);  }
                            break;
                        }
                    }
                }
            }

            if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
            int t = _dict[ToSenseWord(c)];
            if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false && p != 0) {
                p = 0;
                next = _next[0] + t;
                find = _key[next] == t;
            }
            if (find) {
                findIndex = _check[next];
                p = next;
            }
        }
        if (findIndex != 0) {
            for (int item : _guides[findIndex]) {
                IllegalWordsSearchResult r = GetIllegalResult(item, text.length() - 1, text, p, pIndex, flag);
                if(r!=null){
                    for (int j = r.Start; j < text.length(); j++) { result.setCharAt(j, replaceChar);  }
                    break;
                }
            }
        }
        return result.toString();
    }

    private int FindStart(String keyword, int end, String srcText, int p, int[] pIndex)
    {
        if (end + 1 < srcText.length()) {
            boolean en1 = IsEnglishOrNumber(srcText.charAt(end+1));
            boolean en2 = IsEnglishOrNumber(srcText.charAt(end));
            if (en1 && en2) { return -1; }
        }
        int n = keyword.length();
        int start = end;
        int pp = p;
        while (n > 0) {
            int pi = pIndex[start--];
            if (pi != pp) { n--; pp = pi; }
            if (start == -1) return 0;
        }
        boolean sn1 = IsEnglishOrNumber(srcText.charAt(start++));
        boolean sn2 = IsEnglishOrNumber(srcText.charAt(start));
        if (sn1 && sn2) {                return -1;            }
        return start;
    }

    private IllegalWordsSearchResult GetIllegalResult(int index, int end, String srcText, int p, int[] pIndex, int flag)
    {
        if (UseBlacklistFilter) {
            int b = _blacklist[index];
            if ((b | flag) != b) { return null; }
        }
        String keyword = _keywords[index];
        int start = FindStart(keyword, end, srcText, p, pIndex);
        if (start == -1) { return null; }
        if (UseBlacklistFilter) {
            return new IllegalWordsSearchResult(keyword, start, end, srcText,  _blacklist[index]);
        }
        return new IllegalWordsSearchResult(keyword, start, end, srcText);
    }

    protected void Save(FileOutputStream bw) throws IOException
    {
        super.Save(bw);

        bw.write(UseSkipWordFilter?1:0);
        bw.write(_skipBitArray.length);
        for (boolean item : _skipBitArray) {
            bw.write(item?1:0);
        }

        bw.write(UseBlacklistFilter?1:0);
        bw.write(_blacklist.length);
        for (int item : _blacklist) {
            bw.write(item);
        }

        bw.write(_maxJump);
        bw.write(toWord.length);
        for (char item : toWord) {
            bw.write((int)item);
        }
        bw.write(UseIgnoreCase?1:0);
    }

    protected void Load(FileInputStream  br) throws IOException
    {
        super.Load(br);

        UseSkipWordFilter=br.read()>0?true:false;
        int length =br.read();
        _skipBitArray=new boolean[length];
        for (int i = 0; i < length; i++) {
            _skipBitArray[i]=br.read()>0?true:false;
        }

        UseBlacklistFilter=br.read()>0?true:false;
        length =br.read();
        _blacklist=new int[length];
        for (int i = 0; i < length; i++) {
            _blacklist[i]=br.read();
        }
 
        _maxJump=br.read();
        length =br.read();
        toWord=new char[length];
        for (int i = 0; i < length; i++) {
            toWord[i]=(char)br.read();
        }
        UseIgnoreCase=br.read()>0?true:false;
    }

    public void SetSkipWords(String skipList)
    {
        _skipBitArray = new boolean[Character.MAX_VALUE + 1];
        if (skipList == null) {
            for (int i = 0; i < _skipList.length(); i++) {
                _skipBitArray[_skipList.charAt(i)] = true;
            }
        }
    }

    public void SetBlacklist(int[] blacklist) throws IllegalArgumentException
    {
        if (_keywords == null) {
            throw new IllegalArgumentException("请先使用SetKeywords方法设置关键字！");
        }
        if (blacklist.length != _keywords.length) {
            throw new IllegalArgumentException("请关键字与黑名单列表的长度要一样长！");
        }
        _blacklist = blacklist;
    }

    private String ToSenseWord(String text)
    {
        StringBuilder stringBuilder = new StringBuilder(text.length());
        for (int i = 0; i < text.length(); i++) {
            stringBuilder.append(ToSenseWord(text.charAt(i)));
        }
        return stringBuilder.toString();
    }

    private char ToSenseWord(char c)
    {
        if (UseIgnoreCase) {
            if (c >= 'A' && c <= 'Z') return (char)(c | 0x20);
        }
        if (c == 12288) return ' ';
        if (c >= 65280 && c < 65375) {
            char k = (char)(c - 65248);
            if (UseIgnoreCase) {
                if ('A' <= k && k <= 'Z') {
                    k = (char)(k | 0x20);
                }
            }
            return (char)k;
        }
        return c;
    }
    public void SetKeywords(List<String> keywords)
    {
        Set<String> kws = new HashSet<String>(keywords);
        List<String> list=new ArrayList<String>();
        for (String item : kws) {
            list.add(ToSenseWord(item));
        }
        super.SetKeywords(list);
    }
    private Boolean IsEnglishOrNumber(Character c)
    {
        if (c < 128) {
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                return true;
            }
        }
        return false;
    }

}
