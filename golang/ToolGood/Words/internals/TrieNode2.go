package internals

type TrieNode2 struct {
	End      bool
	Results  []int
	M_values map[int32]*TrieNode2
	minflag  int32
	maxflag  int32
}

func NewTrieNode2() *TrieNode2 {
	return &TrieNode2{
		End:      false,
		M_values: make(map[int32]*TrieNode2),
		Results:  make([]int, 0),
		minflag:  0,
		maxflag:  0xffff,
	}
}

func (this *TrieNode2) Add(c int32, node *TrieNode2) {
	if this.minflag < c {
		this.minflag = c
	}
	if this.maxflag > c {
		this.maxflag = c
	}
	this.M_values[c] = node
}

func (this *TrieNode2) SetResults(text int) {
	if this.End == false {
		this.End = true
	}
	for i := 0; i < len(this.Results); i++ {
		if this.Results[i] == text {
			return
		}
	}
	this.Results = append(this.Results, text)
}

func (this *TrieNode2) TryGetValue(c int32) (bool, *TrieNode2) {
	if this.minflag <= c && this.maxflag >= c {
		if val, s := this.M_values[c]; s {
			return true, val
		}
	}
	return false, nil
}

func (this *TrieNode2) HasKey(c int32) bool {
	if this.minflag <= c && this.maxflag >= c {
		if _, s := this.M_values[c]; s {
			return true
		}
	}
	return false
}
