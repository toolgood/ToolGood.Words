package internals

import "sort"
import (
	"os"
	"bytes"
    "encoding/binary"
)

type BaseSearchEx struct{
	I_keywords []string
	I_guides [][]int
	I_key []int
	I_next []int
	I_check []int
	I_dict []int
}

func (this *BaseSearchEx)checkFileIsExist(filename string) bool {
    var exist = true
    if _, err := os.Stat(filename); os.IsNotExist(err) {
        exist = false
    }
    return exist
}
func (this *BaseSearchEx)Save(filename string){
	f, _ := os.OpenFile(filename, os.O_WRONLY | os.O_CREATE , 0666)
	this.Save2(f)
	f.Close()
}
func (this *BaseSearchEx)Save2(f *os.File){
	f.Write(this.intToBytes(len(this.I_keywords)))
	for _,key:=range this.I_keywords {
		f.Write(this.intToBytes(len(key)))
		f.Write([]byte (key))
	}

	f.Write(this.intToBytes(len(this.I_guides)))
	for _,key:=range this.I_guides {
		f.Write(this.intToBytes(len(key)))
		for _,item:=range key{
			f.Write(this.intToBytes(item))
		}
	}

	f.Write(this.intToBytes(len(this.I_key)))
	for _,item:= range this.I_key{
		f.Write(this.intToBytes(item))
	}

	f.Write(this.intToBytes(len(this.I_next)))
	for _,item:= range this.I_next{
		f.Write(this.intToBytes(item))
	}

	f.Write(this.intToBytes(len(this.I_check)))
	for _,item:= range this.I_check{
		f.Write(this.intToBytes(item))
	}
 
	f.Write(this.intToBytes(len(this.I_dict)))
	for _,item:= range this.I_dict{
		f.Write(this.intToBytes(item))
	}
}

func (this *BaseSearchEx)Load(filename string){
	f, _ := os.OpenFile(filename, os.O_RDONLY , 0666)
	this.Load2(f)
	f.Close()
}
func (this *BaseSearchEx)Load2(f *os.File){
	intBs := make([] byte,4)

	f.Read(intBs)
	length:= this.bytesToInt(intBs);

	this.I_keywords= make([]string,length) 
	for i:=0;i<length;i++ {
		f.Read(intBs)
		l := this.bytesToInt(intBs);
		temp:=make([]byte,l)
		f.Read(temp)
		this.I_keywords[i]=string (temp)
	}

	f.Read(intBs)
	length = this.bytesToInt(intBs);
	this.I_guides=make([][]int,length)
	for i:=0;i<length;i++ {
		f.Read(intBs)
		l := this.bytesToInt(intBs);
		ls:=make([]int,l)
		for j := 0; j < l; j++ {
			f.Read(intBs)
			ls[j]=this.bytesToInt(intBs);
		}
		this.I_guides[i]=ls
	}

	f.Read(intBs)
	length = this.bytesToInt(intBs);
	this.I_key=make([]int,length)
	for i := 0; i < length; i++ {
		f.Read(intBs)
		this.I_key[i]=this.bytesToInt(intBs);
	}

	f.Read(intBs)
	length = this.bytesToInt(intBs);
	this.I_next=make([]int,length)
	for i := 0; i < length; i++ {
		f.Read(intBs)
		this.I_next[i]=this.bytesToInt(intBs);
	}

	
	f.Read(intBs)
	length = this.bytesToInt(intBs);
	this.I_check=make([]int,length)
	for i := 0; i < length; i++ {
		f.Read(intBs)
		this.I_check[i]=this.bytesToInt(intBs);
	}

	f.Read(intBs)
	length = this.bytesToInt(intBs);
	this.I_dict=make([]int,length)
	for i := 0; i < length; i++ {
		f.Read(intBs)
		this.I_dict[i]=this.bytesToInt(intBs);
	}
}


func (this *BaseSearchEx)intToBytes(i int) []byte{
    x := int32(i)
    bytesBuffer := bytes.NewBuffer([]byte{})
	binary.Write(bytesBuffer, binary.BigEndian, x)
    return bytesBuffer.Bytes()
}
func (this *BaseSearchEx)bytesToInt(bs []byte) int{
	bytesBuffer := bytes.NewBuffer(bs)
    var x int32
    binary.Read(bytesBuffer, binary.BigEndian, &x)
	return int(x)
}


func (this *BaseSearchEx) SetKeywords(keywords []string)  {
	this.I_keywords=keywords
	length := this.CreateDict(keywords);
	root := NewTrieNodeEx() 

	for i,keyword:= range keywords {
		nd:=root
		p:= []rune(keyword)
		for _,c := range p{
			nd = nd.Add(int32  (this.I_dict[c]));
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
			c := nd.Char;
			for {
				if r == nil {
					break
				}
				if _,s :=r.M_values[c]; s{
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
	this.I_key=make([]int,length)
	this.I_next=make([]int,length)
	this.I_check=make([]int,length)
	var guides [][]int
	first:=make([]int,1)
	first[0]=0
	guides=append(guides,first)

	for i := 0; i < length; i++  {
		item := has[i];
		if item==nil{
			continue
		}
		this.I_key[i] = int (item.Char) ;
		this.I_next[i] = item.Next;
		if  item.End==true  {
			this.I_check[i] = len(guides) 
			guides=append(guides,item.Results)
		}
	}
	this.I_guides=guides
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
	for i:=0;i<len(list);i++ {
		list2 = append(list2,list[ index1[i]])
	}

	this.I_dict = make([]int,0x10000)   
	for i,v:=range list2{
		this.I_dict[v] = i + 1;
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
 
 