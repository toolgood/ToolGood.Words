package ToolGood.Words;


public class IllegalWordsSearchResult
{
    public IllegalWordsSearchResult(String keyword, int start, int end, String srcText)
    {
        Keyword = keyword;
        Success = true;
        End = end;
        Start = start;
        SrcString = srcText.substring(Start, end+1);
    }
    public IllegalWordsSearchResult(String keyword, int start, int end, String srcText, int type)
    {
        Keyword = keyword;
        Success = true;
        End = end;
        Start = start;
        SrcString = srcText.substring(Start, end+1);
        BlacklistType = type;
    }

    public IllegalWordsSearchResult()
    {
        Success = false;
        Start = 0;
        End = 0;
        SrcString = null;
        Keyword = null;
    }
    public Boolean Success ;
    public int Start;
    public int End ;
    public String SrcString ;
    public String Keyword;
    public int BlacklistType ;
}