package Words

type IllegalWordsSearchResult struct {
	Start int
	End int
	Keyword string
	BlacklistType int
	SrcString string
}
 
func NewIllegalWordsSearchResult(keyword string,start int,end int,srcText string,_type int) *IllegalWordsSearchResult  {
	return &IllegalWordsSearchResult{
		Start:start,
		End:end,
		Keyword:keyword,
		SrcString:string ( []rune(srcText)[start:end]),
		BlacklistType:_type,
	}
}
 