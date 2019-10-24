package Words

import . "./internals"

 
type StringSearchEx struct {
	BaseSearchEx
}

func NewStringSearchEx() *StringSearchEx{
	return &StringSearchEx{
	}
}

func (this *StringSearchEx)FindAll(text string) []string  {
	root:= make([]string, 0)
	p:=0
	for _,c := range text {
		t:=this.I_dict[c]
		if	t==0{
			p = 0;
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
					root=append(root,this.I_keywords[item])
				}
			}
			p = next;
		}
	}
	return root;
}
 

func (this *StringSearchEx)FindFirst(text string) string  {
	p:=0
	for _,c := range text {
		t:=this.I_dict[c]
		if	t==0{
			p = 0;
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
				return this.I_keywords[this.I_guides[index][0]];
			}
			p = next;
		}
	}
	return "";
}
 

func (this *StringSearchEx)ContainsAny(text string) bool  {
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
 

func (this *StringSearchEx)Replace(text string, replaceChar rune) string  {
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
