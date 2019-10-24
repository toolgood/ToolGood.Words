package internals

import "sort"

type BaseSearchEx struct{
	_keywords []string
	_guides [][]int
	_key []int
	_next []int
	_check []int
	_dict []int
}

func (this *BaseSearchEx) SetKeywords(keywords []string)  {
	this._keywords=keywords
	length := this.CreateDict(keywords);
	root := NewTrieNodeEx() 

	for i,keyword:= range keywords {
		nd:=root
		p:= []rune(keyword)
		for _,c := range p{
			nd = nd.Add(c);
		}
		nd.SetResults(i);
	}
	nodes:=make([]*TrieNodeEx,0)
	for _,value := range root.M_values{
		value.Failure=root
		for _,trans := range value.M_values{
			nodes=append(nodes,trans)
		}
	}
	for len(nodes)>0{
		newNodes:=make([]*TrieNodeEx,0)
		for _,nd :=range nodes {
			r := nd.Parent.Failure;
			c :=  nd.Char;
			for {
				if	r == nil {
					break
				}
				if _,s :=r.M_values[c]; !s{
					break
				}
				r = r.Failure;
			}
			if r==nil{
				nd.Failure = root;
			}else{
				nd.Failure = r.M_values[c];
				for _,result:=range nd.Failure.Results {
					nd.SetResults(result);
				}
			}

			for _,child:= range nd.M_values{
				newNodes=append(newNodes,child)
			}
		}
		nodes = newNodes;
	}
	root.Failure = root;
	for _,item :=range root.M_values{
		this.tryLinks(item)
	}
	this.build(root, length);
}

func (this *BaseSearchEx) tryLinks(node *TrieNodeEx){
	node.Merge(node.Failure);
	for _,item:=range node.M_values{
		this.tryLinks(item)
	}
}

func (this *BaseSearchEx)build(root *TrieNodeEx,length int){
	has:= make([]*TrieNodeEx,0x00FFFFFF)
	length = root.Rank(has) + length + 1;
	this._key=make([]int,length)
	this._next=make([]int,length)
	this._check=make([]int,length)
	var guides [][]int
	first:=make([]int,1)
	first[0]=0
	guides=append(guides,first)

	for i := 0; i < length; i++  {
		item := has[i];
		if item==nil{
			continue
		}
		this._key[i] = int (item.Char) ;
		this._next[i] = item.Next;
		if  item.End==true  {
			this._check[i] = len(guides) 
			guides=append(guides,item.Results)
		}
	}
	this._guides=guides
}
 


func (this *BaseSearchEx)CreateDict(keywords []string) int {
	dictionary:= make(map[int32]int, 0)

	for	_,keyword := range keywords{
		for _,item:=range keyword{
			if v,s:= dictionary[item];s{
				if v>0 {
					dictionary[item]=dictionary[item]+2
				}
			}else{
				if v>0 {
					dictionary[item]=2
				} else {
					dictionary[item]=1
				}
			}
		}
	}
	list:=this.sortMap(dictionary)

	index1:=make([]int,0)
	for i:=0;i<len(list);i=i+2 {
		index1 = append(index1, i)
	}
	length:= len(index1)
	for i := 0 ; i < length/2 ; i++ {
		index1[i], index1[length -i - 1] = index1[length - i -1 ], index1[i]
	}
	for i:=1;i<len(list);i=i+2 {
		index1 = append(index1, i)
	}

	list2:=make([]int32,0)
	for i:=0;i<len(list);i=i+2 {
		list2 = append(list2,list[ index1[i]])
	}

	this._dict = make([]int,0x10000)   
	for i,v:=range list2{
		this._dict[v] = i + 1;
	}
	return len(dictionary)  
}
func (s *BaseSearchEx)sortMap(mp map[int32]int) []int32 {
	var newMp = make([]int, 0)
	var newMpKey = make([]int32, 0)
	for oldk, v := range mp {
	   newMp = append(newMp, v)
	   newMpKey = append(newMpKey, oldk)
	}
	sort.Ints(newMp)

	list:=make([]int32, 0)
	for k, _ := range newMp {
		list = append(list, newMpKey[k])
	}
	return list
}
 
 