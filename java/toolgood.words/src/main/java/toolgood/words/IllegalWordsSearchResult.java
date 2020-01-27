package toolgood.words;


public class IllegalWordsSearchResult
{
    public IllegalWordsSearchResult(String keyword, int start, int end, String srcText)
    {
        Keyword = keyword;
        End = end;
        Start = start;
        SrcString = srcText.substring(Start, end+1);
    }
    public IllegalWordsSearchResult(String keyword, int start, int end, String srcText, int type)
    {
        Keyword = keyword;
        End = end;
        Start = start;
        SrcString = srcText.substring(Start, end+1);
        BlacklistType = type;
    }
 
    /**开始位置 */
    public int Start;
    /**结束位置 */
    public int End ;
    /**原始文本 */
    public String SrcString ;
    /**关键字 */
    public String Keyword;
    /**黑名单类型 */
    public int BlacklistType ;
}