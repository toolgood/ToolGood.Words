package Words

type WordsSearchResult struct {
	Start int
	End int
	Keyword string
	Index int
}

func NewWordsSearchResult(keyword string,start int,end int,index int) *WordsSearchResult  {
	return &WordsSearchResult{
		Start:start,
		End:end,
		Keyword:keyword,
		Index:index,
	}
}