package ToolGood_Words

type StringSearch struct {
	_first []*TrieNode
}
func NewStringSearch() *StringSearch  {
	return &StringSearch{
		_first : make([]*TrieNode,0),
	}
}

func (this *StringSearch)SetKeywords(keywords []string){
	first := make([]*TrieNode,0x10000)
	root := NewTrieNode()
	for	_,p := range keywords {
		length:=len(p)
		if length>0 {
			var nd *TrieNode
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
			nd.SetResults(p);
		}
	}
	this._first=first
	links:= make(map[*TrieNode]*TrieNode)
	for _,val := range root.m_values {
		this.TryLinks(val, nil, links);
	}
	for key,val := range links {
		key.Merge(val,links)
 	}
}

func (this *StringSearch) TryLinks(node *TrieNode,node2 *TrieNode,links map[*TrieNode]*TrieNode){
	for key,value:=range node.m_values {
		var tn *TrieNode
		if node2 == nil {
			tn = this._first[key]
			if tn != nil {
				links[value]=tn
			}
		} else {
			b,tn:=  node2.TryGetValue(key)
			if b==true{
				links[value]=tn
			}
		}
		this.TryLinks(value,tn,links)
	}
}

func (this *StringSearch) FindFirst(text string) string{
	var ptr *TrieNode
	for _,t := range text {
		var tn *TrieNode
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
				return tn.Results[0]
			}
		}
		ptr = tn;
	}
	return ""
}


func (this *StringSearch) FindAll(text string) []string{
	list := make([]string,0) 
	var ptr *TrieNode
	for _,t := range text {
		var tn *TrieNode
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
				for	_,item:=range tn.Results {
					list=append(list,item)
				}
			}
		}
		ptr = tn;
	}
	return list
}

func (this *StringSearch) ContainsAny(text string) bool{
	var ptr *TrieNode
	for _,t := range text {
		var tn *TrieNode
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

func (this *StringSearch) Replace(text string,replaceChar int32) string{
	result:= []rune(text)
	var ptr *TrieNode
	var i int
	for _,t := range text {
		var tn *TrieNode
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
				maxLength:=len([]rune(tn.Results[0]))
				start:= i + 1 - maxLength
				for j := start; j <= i; j++{
					result[j]=replaceChar
				} 
			}
		}
		ptr = tn;
		i++
	}
	return string (result) 
}
 