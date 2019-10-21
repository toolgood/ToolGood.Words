package ToolGood.Words;


public class WordsSearchResult{

    public WordsSearchResult(String keyword, int start, int end, int index)
    {
        Keyword = keyword;
        Success = true;
        End = end;
        Start = start;
        Index = index;
    }

    private WordsSearchResult()
    {
        Success = false;
        Start = 0;
        End = 0;
        Index = -1;
        Keyword = null;
    }
    public Boolean Success;
    public int Start;
    public int End;
    public String Keyword;
    public int Index;


    public static WordsSearchResult getEmpty() {
        return new WordsSearchResult(); 
    }

        
   
}