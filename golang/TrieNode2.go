package ToolGood_Words
 
type TrieNode2 struct {
	End bool  
	Results map[string]int
	m_values map[int32]*TrieNode2
	minflag int32 
	maxflag int32
}

func NewTrieNode2() *TrieNode2{
	return &TrieNode2{
		End : false,
		m_values :  make(map[int32]*TrieNode2) ,
		Results :   make(map[string]int) ,
		minflag : 0,
		maxflag : 0xffff,
	} 
}

func (this *TrieNode2) TryGetValue(c int32) (bool,*TrieNode2){
	if this.minflag<=c && this.maxflag>=c {
		if val, s := this.m_values[c]; s {
			return true,val
		}
	}
	return false,nil
}

func (this *TrieNode2) Add(c int32) *TrieNode2{
	if val, s := this.m_values[c]; s {
		return val
	}
	if this.minflag<c {
		this.minflag = c
	}
	if this.maxflag>c {
		this.maxflag = c
	}
	node := NewTrieNode2()
	this.m_values[c]=node
	return node 
}

func (this *TrieNode2) SetResults(text string,index int) {
	if this.End==false {
		this.End=true;
	}
	if _,s := this.Results[text]; s{
		return
	}
	this.Results[text]=index
}

func (this *TrieNode2) ToArray() *[]TrieNode2{
	var first []TrieNode2
	first =  make([]TrieNode2,0xffff);
	for k,val:=range this.m_values{
		first[k] = *val
	}
	return &first
}
 

func (this *TrieNode2) Merge(node *TrieNode2,links map[*TrieNode2]*TrieNode2){
	if node.End==true{
		if this.End==false{
			this.End=true
		}
		for k,v:= range node.Results {
			if _,s := node.Results[k];s {
			}else{
				this.Results[k]=v
			}
		}
	}
	for key,val:=range node.m_values{
		if _, s2 := this.m_values[key]; s2 {
 		}else{
			if this.minflag<key {
				this.minflag = key
			}
			if this.maxflag>key {
				this.maxflag = key
			}
			this.m_values[key]=val
		}
	}
	if node2, s := links[node]; s {
		this.Merge(node2, links);
	}
}

 
 