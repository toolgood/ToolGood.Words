package toolgood.words;

public class WordsSearchResult {

    public WordsSearchResult(final String keyword, final int start, final int end, final int index) {
        Keyword = keyword;
        End = end;
        Start = start;
        Index = index;
        MatchKeyword = keyword;
    }

    public WordsSearchResult(final String keyword, final int start, final int end, final int index,
            final String matchKeyword) {
        Keyword = keyword;
        End = end;
        Start = start;
        Index = index;
        MatchKeyword = matchKeyword;
    }

    /** 开始位置 */
    public int Start;
    /** 结束位置 */
    public int End;
    /** 关键字 */
    public String Keyword;
    /** 索引 */
    public int Index;
    /** 匹配关键字 */
    public String MatchKeyword;

}