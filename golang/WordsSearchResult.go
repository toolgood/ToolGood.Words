package ToolGood_Words

type WordsSearchResult struct {
	Success bool
	Start int
	End int
	Keyword string
	Index int
}

func NewWordsSearchResult(keyword string,start int,end int,index int) *WordsSearchResult  {
	return &WordsSearchResult{
		Success:true,
		Start:start,
		End:end,
		Keyword:keyword,
		Index:index,
	}
}