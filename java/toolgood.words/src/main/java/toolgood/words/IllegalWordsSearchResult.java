package toolgood.words;


public class IllegalWordsSearchResult
{
    public IllegalWordsSearchResult(final String keyword, final int start, final int end, final int index,
            final String matchKeyword, final int type)
    {
        MatchKeyword = matchKeyword;
        End = end;
        Start = start;
        Index = index;
        Keyword = keyword;
        BlacklistType = type;
    }

    /**开始位置 */
    public int Start;
    /**结束位置 */
    public int End ;
    /**原始文本 */
    public String Keyword ;
    /**关键字 */
    public String MatchKeyword;
    /**黑名单类型 */
    public int BlacklistType ;
    /**索引 */
    public int Index;

 
}