package internals

type TrieNode struct {
	Index    int
	Layer    int
	End      bool
	Char     int
	Results  []int
	M_values map[int]*TrieNode
	Failure  *TrieNode
	Parent   *TrieNode
}

func NewTrieNode() *TrieNode {
	return &TrieNode{
		End:      false,
		M_values: make(map[int]*TrieNode),
		Results:  make([]int, 0),
	}
}

func (this *TrieNode) Add(c int) *TrieNode {
	if val, s := this.M_values[c]; s {
		return val
	}
	node := NewTrieNode()
	node.Parent = this
	node.Char = c
	this.M_values[c] = node
	return node
}

func (this *TrieNode) SetResults(text int) {
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
