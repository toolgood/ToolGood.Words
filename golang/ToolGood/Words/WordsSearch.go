package Words

import . "./internals"


type WordsSearch struct {
	_first []*TrieNode2
}

func NewWordsSearch() *WordsSearch  {
	return &WordsSearch{
		_first : make([]*TrieNode2,0),
	}
}

func (this *WordsSearch)SetKeywords(keywords []string){
	first := make([]*TrieNode2,0x10000)
	root := NewTrieNode2()
	for	i,p := range keywords {
		length:=len(p)
		if length>0 {
			var nd *TrieNode2
			for	i,ch := range p{
				if i==0 {
					nd = first[ch]
					if nd==nil {
						nd=root.Add(ch)
						first[ch] = nd;
					}
				}else{
					nd = nd.Add(ch);
				}
			}
			nd.SetResults(p,i);
		}
	}
	this._first=first
	links:= make(map[*TrieNode2]*TrieNode2)
	for _,val := range root.M_values {
		this.tryLinks(val, nil, links);
	}
	for key,val := range links {
		key.Merge(val,links)
 	}
}


func (this *WordsSearch) tryLinks(node *TrieNode2,node2 *TrieNode2,links map[*TrieNode2]*TrieNode2){
	for key,value:=range node.M_values {
		var tn *TrieNode2
		if node2 == nil {
			tn = this._first[key]
			if tn != nil {
				links[value]=tn
			}
		} else {
			b,tn:= node2.TryGetValue(key)
			if b==true{
				links[value]=tn
			}
		}
		this.tryLinks(value,tn,links)
	}
}


func (this *WordsSearch) FindFirst(text string) *WordsSearchResult{
	var ptr *TrieNode2
	var i int
	for _,t := range text {
		var tn *TrieNode2
		if ptr==nil{
			tn = this._first[t];
		}else{
			var b bool
			b,tn =ptr.TryGetValue(t)
			if b==false{
				tn = this._first[t];
			}
		}
		if tn!=nil {
			if tn.End==true {
				for k,v:= range tn.Results{
					maxLength:=len([]rune(k))
					return NewWordsSearchResult(k, i + 1 - maxLength, i, v);
				}
			}
		}
		ptr = tn
		i++
	}
	return nil
}



func (this *WordsSearch) FindAll(text string) []*WordsSearchResult{
	list := make([]*WordsSearchResult,0) 
	var ptr *TrieNode2
	var i int
	for _,t := range text {
		var tn *TrieNode2
		if ptr==nil{
			tn = this._first[t];
		}else{
			var b bool
			b,tn =ptr.TryGetValue(t)
			if b==false{
				tn = this._first[t];
			}
		}
		if tn!=nil {
			if tn.End==true {
				for k,v:= range tn.Results{
					maxLength:=len([]rune(k))
					r:= NewWordsSearchResult(k, i + 1 - maxLength, i, v);
					list=append(list,r)
				}
			}
		}
		ptr = tn;
		i++
	}
	return list
}


func (this *WordsSearch) ContainsAny(text string) bool{
	var ptr *TrieNode2
	for _,t := range text {
		var tn *TrieNode2
		if ptr==nil{
			tn = this._first[t];
		}else{
			var b bool
			b,tn =ptr.TryGetValue(t)
			if b==false{
				tn = this._first[t];
			}
		}
		if tn!=nil {
			if tn.End==true {
				return true
			}
		}
		ptr = tn;
	}
	return false
}


func (this *WordsSearch) Replace(text string,replaceChar int32) string{
	result:= []rune(text)
	var ptr *TrieNode2
	var i int
	for _,t := range text {
		var tn *TrieNode2
		if ptr==nil{
			tn = this._first[t];
		}else{
			var b bool
			b,tn =ptr.TryGetValue(t)
			if b==false{
				tn = this._first[t];
			}
		}
		if tn!=nil {
			if tn.End==true {
				for k,_:= range tn.Results{
					maxLength:=len([]rune(k))
					start:= i + 1 - maxLength
					for j := start; j <= i; j++{
						result[j]=replaceChar
					} 
					break
				}
			}
		}
		ptr = tn;
		i++
	}
	return string (result) 
}
