package internals

type TrieNodes struct {
	Items []*TrieNode
}

func NewTrieNodes() *TrieNodes {
	return &TrieNodes{
		Items: make([]*TrieNode, 0),
	}
}
