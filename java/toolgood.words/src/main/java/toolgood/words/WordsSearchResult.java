package toolgood.words;


public class WordsSearchResult{

    public WordsSearchResult(String keyword, int start, int end, int index)
    {
        Keyword = keyword;
        End = end;
        Start = start;
        Index = index;
    }
    /**开始位置 */
    public int Start;
    /**结束位置 */
    public int End ;
    /**关键字 */
    public String Keyword;
    /**索引 */
    public int Index;

 

        
   
}