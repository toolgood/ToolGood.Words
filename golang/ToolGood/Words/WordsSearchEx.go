package Words

import . "./internals"

 
type WordsSearchEx struct {
	BaseSearchEx
}

func NewWordsSearchEx() *WordsSearchEx{
	return &WordsSearchEx{
	}
}

func (this *WordsSearchEx)FindAll(text string) []*WordsSearchResult  {
	list:= make([]*WordsSearchResult, 0)
	p:=0
	var i int
	for _,c := range text {
		t:=this.I_dict[c]
		if	t==0{
			p = 0;
			i++
			continue;
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if  find == false && p != 0  {
			p = 0;
			next = this.I_next[0] + t;
			find = this.I_key[next] == t;
		}
		if  find  {
			index := this.I_check[next];
			if (index > 0) {
				for _,item:=range this.I_guides[index]{
					k:=this.I_keywords[item]
					maxLength:=len([]rune(k))
					r:= NewWordsSearchResult(k, i + 1 - maxLength, i, index);
					list=append(list,r)
				}
			}
			p = next;
		}
		i++
	}
	return list;
}
 

func (this *WordsSearchEx)FindFirst(text string) *WordsSearchResult  {
	p:=0
	var i int
	for _,c := range text {
		t:=this.I_dict[c]
		if	t==0{
			p = 0;
			i++
			continue;
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if  find == false && p != 0  {
			p = 0;
			next = this.I_next[0] + t;
			find = this.I_key[next] == t;
		}
		if  find  {
			index := this.I_check[next];
			if (index > 0) {
				k:=this.I_keywords[this.I_guides[index][0]]
				maxLength:=len([]rune(k))
				return NewWordsSearchResult(k, i + 1 - maxLength, i, index);
			}
			p = next;
		}
		i++
	}
	return nil;
}
 

func (this *WordsSearchEx)ContainsAny(text string) bool  {
	p:=0
	for _,c := range text {
		t:=this.I_dict[c]
		if	t==0{
			p = 0;
			continue;
		}
		next := this.I_next[p] + t;

		if this.I_key[next] == t {
			if  this.I_check[next] > 0  { 
				return true;
			}
			p = next;
		} else {
			p = 0;
			next = this.I_next[p] + t;
			if (this.I_key[next] == t) {
				if (this.I_check[next] > 0) {
					 return true;
				}
				p = next;
			}
		}
	}
	return false;
}
 

func (this *WordsSearchEx)Replace(text string, replaceChar rune) string  {
	result:= []rune (text)
	p:=0
	var i int
	for _,c := range text {
		t:=this.I_dict[c]
		if	t==0{
			p = 0;
			i++
			continue;
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if  find == false && p != 0  {
			p = 0;
			next = this.I_next[0] + t;
			find = this.I_key[next] == t;
		}
		if  find  {
			index := this.I_check[next];
			if (index > 0) {
				r:=this.I_keywords[this.I_guides[index][0]]
				maxLength:=len([]rune(r))
				start:= i + 1 - maxLength
				for j := start; j <= i; j++{
					result[j]=replaceChar
				} 
			}
			p = next;
		}
		i++
	}
	return string (result) 
}
