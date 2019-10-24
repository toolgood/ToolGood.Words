package internals
 
type TrieNode struct {
	End bool  
	Results []string
	M_values map[int32]*TrieNode
	minflag int32 
	maxflag int32
}

func NewTrieNode() *TrieNode{
	return &TrieNode{
		End : false,
		M_values :  make(map[int32]*TrieNode) ,
		Results :   make([]string,0),
		minflag : 0,
		maxflag : 0xffff,
	} 
}

func (this *TrieNode) TryGetValue(c int32) (bool,*TrieNode){
	if this.minflag<=c && this.maxflag>=c {
		if val, s := this.M_values[c]; s {
			return true,val
		}
	}
	return false,nil
}

func (this *TrieNode) Add(c int32) *TrieNode{
	if val, s := this.M_values[c]; s {
		return val
	}
	if this.minflag<c {
		this.minflag = c
	}
	if this.maxflag>c {
		this.maxflag = c
	}
	node := NewTrieNode()
	this.M_values[c]=node
	return node 
}

func (this *TrieNode) SetResults(text string) {
	if this.End==false {
		this.End=true;
	}
	for i := 0; i < len(this.Results); i++ {
        if this.Results[i] == text {
            return
        }
    }
	this.Results=append(this.Results,text)
}

func (this *TrieNode) ToArray() *[]TrieNode{
	var first []TrieNode
	first =  make([]TrieNode,0xffff);
	for k,val:=range this.M_values{
		first[k] = *val
	}
	return &first
}
 

func (this *TrieNode) Merge(node *TrieNode,links map[*TrieNode]*TrieNode){
	if node.End==true{
		if this.End==false{
			this.End=true
		}
		for i := 0; i < len(node.Results); i++ {
			r:=node.Results[i]
			if StringsContains(this.Results,r)==-1 {
				this.Results=append(this.Results,r)
			}
		}
	}
	for key,val:=range node.M_values{
		if _, s2 := this.M_values[key]; s2 {
 		}else{
			if this.minflag<key {
				this.minflag = key
			}
			if this.maxflag>key {
				this.maxflag = key
			}
			this.M_values[key]=val
		}
	}
	if node2, s := links[node]; s {
		this.Merge(node2, links);
	}
}

func StringsContains(array []string, val string) (index int) {
    index = -1
    for i := 0; i < len(array); i++ {
        if array[i] == val {
            index = i
            return
        }
    }
    return
}
 