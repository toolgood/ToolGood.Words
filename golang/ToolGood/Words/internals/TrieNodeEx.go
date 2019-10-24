package internals

type TrieNodeEx struct {
	Parent *TrieNodeEx  
	Failure *TrieNodeEx  
	Char int32  
	End bool   
	Results []int 
	M_values map[int32]*TrieNodeEx  
	Merge_values map[int32]*TrieNodeEx  
	minflag int32 
	maxflag int32
	Next int  
	Count int 
}

func NewTrieNodeEx() *TrieNodeEx  {
	return &TrieNodeEx{
		M_values: make(map[int32]*TrieNodeEx, 0),
		Merge_values: make(map[int32]*TrieNodeEx, 0),
		Results: make([]int, 0),
		minflag: 0xffff,
		maxflag:0,
		Next:0,
		Count:0,
	}
}

func (this *TrieNodeEx) TryGetValue(c int32) (bool,*TrieNodeEx){
	if this.minflag<=c && this.maxflag>=c {
		if val, s := this.M_values[c]; s {
			return true,val
		}
	}
	return false,nil
}


func (this *TrieNodeEx) Add(c int32) *TrieNodeEx{
	if val, s := this.M_values[c]; s {
		return val
	}
	if this.minflag>c {
		this.minflag = c
	}
	if this.maxflag<c {
		this.maxflag = c
	}
	node := NewTrieNodeEx()
	node.Parent = this;
	node.Char = c;
	this.M_values[c]=node
	this.Count++;
	return node 
}

func (this *TrieNodeEx) SetResults(text int) {
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

func (this *TrieNodeEx)Merge(node *TrieNodeEx) {
	nd:=node
	for	nd.Char != 0 {
		for key,value := range node.M_values{
			if _,s := this.M_values[key]; s{
				continue;
			} 
			if _,s := this.Merge_values[key]; s{
				continue;
			} 
			if this.minflag>key {
				this.minflag = key
			}
			if this.maxflag<key {
				this.maxflag = key
			}
			this.Merge_values[key]=value
			this.Count++;
		}
		nd = nd.Failure;
	}
}

func (this *TrieNodeEx)Rank(has []*TrieNodeEx) int {
    var seats []bool =make([]bool,len(has))
	var start int = 1
	has[0] = this;
	this.Rank2(start, seats, has)
	maxCount := len(has) - 1;
	for	has[maxCount] == nil {
		maxCount--;
	}
	return maxCount;
}
 
func (this *TrieNodeEx)Rank2(start int,seats []bool,has []*TrieNodeEx) int{
	if (this.maxflag == 0) {
		return start
	}
	keys := make([]int32,0)
	for k,_:=range this.M_values {
		keys=append(keys,k)
	}
	for k,_:=range this.Merge_values {
		keys=append(keys,k)
	}	

	for	has[start] != nil{
		start++
	}
	s := start
	if start < int (this.minflag){
		s= int (this.minflag)
	}

	for i := s; i < len(has); i++{
		if has[i]==nil {
			next := i - int (this.minflag);
			if seats[next]{
				continue
			}
			isok := true;

			for	_,item :=range keys{
				if has[next+ int (item)] != nil{
					isok = false
					break
				}
			}
			if (isok) {
				this.SetSeats(next, seats, has);
				break;
			}
		}
	}
	start += len(keys) / 2;
	
	for _,value:= range this.M_values{
		start=value.Rank2(start, seats, has);
	}
	return start
}

func (this *TrieNodeEx)SetSeats(next int,seats []bool,has []*TrieNodeEx){
	this.Next = next;
	seats[next] = true;
	for key,value := range this.Merge_values{
		position := next + int (key);
		has[position] = value;
	}
	for key,value := range this.M_values{
		position := next + int (key) ;
		has[position] = value;
	}
}

 