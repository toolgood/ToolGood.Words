package Words

import . "./internals"
import "errors"
import "os"
import (
	"bytes"
    "encoding/binary"
)

type IllegalWordsSearch struct {
	BaseSearchEx
	UseSkipWordFilter bool
 	_skipBitArray []bool
	UseDuplicateWordFilter bool
	UseBlacklistFilter bool
	_blacklist []int
	UseDBCcaseConverter bool
	UseSimplifiedChineseConverter bool
	UseIgnoreCase bool
}

func NewIllegalWordsSearch() *IllegalWordsSearch  {
	_skipList := " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
	_skipBitArray := make([]bool, 0x10000)
	for _,val:=range _skipList{
		_skipBitArray [int (val)] = true
	}
 
	return &IllegalWordsSearch{
		UseSkipWordFilter:true,
		_skipBitArray:_skipBitArray,
		UseDuplicateWordFilter:false,
		UseBlacklistFilter:false,
		_blacklist:make([]int,0),
		UseDBCcaseConverter:true,
		UseSimplifiedChineseConverter:true,
		UseIgnoreCase:true,
	}
}

//在文本中查找所有的关键字
func (this *IllegalWordsSearch)FindAll(text string) []*IllegalWordsSearchResult{
	return this.FindAll2(text,0xffffffff);
}

func (this *IllegalWordsSearch)FindAll2(text string,flag int) []*IllegalWordsSearchResult{
	results:=make([]*IllegalWordsSearchResult,0)
	pIndex:=make([]int,len(text))
	p := 0;
	findIndex := 0;
	var pChar int32 = 0;

	var i int
	for _,c := range text {
		if	p!=0 {
			pIndex[i]=p
			if findIndex!=0 {
				for _,item:= range this.I_guides[findIndex] {
					r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
					if	r!=nil {
						results=append(results,r)
					}
				}
			}
		}
		if this.UseSkipWordFilter && this._skipBitArray[c] {//使用跳词
			findIndex=0
			i++
			continue
		}
		t:=this.I_dict[c]
		if t==0  { //不在字表中，跳过
			p=0
			pChar = c
			i++
			continue
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if	find==false {
			if this.UseDuplicateWordFilter && pChar==c {
				i++
				continue
			}
			if p!=0 {
				p=0
				next = this.I_next[0] + t;
				find = this.I_key[next] == t;
			}
		}
		if find ==true {
			findIndex = this.I_check[next];
			p = next;
		}
		pChar = c;
		i++
	}
	if (findIndex != 0) {
		for _,item := range this.I_guides[findIndex] {
			r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
			if	r!=nil {
				results=append(results,r)
			}
		}
	}
	return results;
}

func (this *IllegalWordsSearch)FindFirst(text string) *IllegalWordsSearchResult{
	return this.FindFirst2(text,0xffffffff);
}

func (this *IllegalWordsSearch)FindFirst2(text string,flag int) *IllegalWordsSearchResult{
	pIndex:=make([]int,len(text))
	p := 0;
	findIndex := 0;
	var pChar int32 = 0;

	var i int
	for _,c := range text {
		if	p!=0 {
			pIndex[i]=p
			if findIndex!=0 {
				for _,item:= range this.I_guides[findIndex] {
					r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
					if	r!=nil {
						return r;
					}
				}
			}
		}
		if this.UseSkipWordFilter && this._skipBitArray[c] {//使用跳词
			findIndex=0
			i++
			continue
		}
		t:=this.I_dict[c]
		if t==0  { //不在字表中，跳过
			p=0
			pChar = c
			i++
			continue
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if	find==false {
			if this.UseDuplicateWordFilter && pChar==c {
				i++
				continue
			}
			if p!=0 {
				p=0
				next = this.I_next[0] + t;
				find = this.I_key[next] == t;
			}
		}
		if find ==true {
			findIndex = this.I_check[next];
			p = next;
		}
		pChar = c;
		i++
	}
	if (findIndex != 0) {
		for _,item := range this.I_guides[findIndex] {
			r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
			if	r!=nil {
				return r;
			}
		}
	}
	return nil;
}


func (this *IllegalWordsSearch)ContainsAny(text string) bool{
	return this.ContainsAny2(text,0xffffffff);
}

func (this *IllegalWordsSearch)ContainsAny2(text string,flag int) bool{
	pIndex:=make([]int,len(text))
	p := 0;
	findIndex := 0;
	var pChar int32 = 0;

	var i int=0
	for _,c := range text {
		if	p!=0 {
			pIndex[i]=p
			if findIndex!=0 {
				for _,item:= range this.I_guides[findIndex] {
					r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
					if	r!=nil {
						return true;
					}
				}
			}
		}
		if this.UseSkipWordFilter && this._skipBitArray[c] {//使用跳词
			findIndex=0
			i++
			continue
		}
		t:=this.I_dict[c]
		if t==0  { //不在字表中，跳过
			p=0
			pChar = c
			i++
			continue
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if	find==false {
			if this.UseDuplicateWordFilter && pChar==c {
				i++
				continue
			}
			if p!=0 {
				p=0
				next = this.I_next[0] + t;
				find = this.I_key[next] == t;
			}
		}
		if find ==true {
			findIndex = this.I_check[next];
			p = next;
		}
		pChar = c;
		i++
	}
	if (findIndex != 0) {
		for _,item := range this.I_guides[findIndex] {
			r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
			if	r!=nil {
				return true;
			}
		}
	}
	return false;
}

func (this *IllegalWordsSearch)Replace(text string, replaceChar rune) string{
	return this.Replace2(text,replaceChar,0xffffffff);
}

func (this *IllegalWordsSearch)Replace2(text string, replaceChar rune,flag int) string{
	result:= []rune (text)

	pIndex:=make([]int,len(text))
	p := 0;
	findIndex := 0;
	var pChar int32 = 0;

	var i int
	for _,c := range text {
		if	p!=0 {
			pIndex[i]=p
			if findIndex!=0 {
				for _,item:= range this.I_guides[findIndex] {
					r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
					if	r!=nil {
						for j:=r.Start;j<i; j++{
							result[j]=replaceChar;
						}
						break;
					}
				}
			}
		}
		if this.UseSkipWordFilter && this._skipBitArray[c] {//使用跳词
			findIndex=0
			i++
			continue
		}
		t:=this.I_dict[c]
		if t==0  { //不在字表中，跳过
			p=0
			pChar = c
			i++
			continue
		}
		next := this.I_next[p] + t;
		find := this.I_key[next] == t;
		if	find==false {
			if this.UseDuplicateWordFilter && pChar==c {
				i++
				continue
			}
			if p!=0 {
				p=0
				next = this.I_next[0] + t;
				find = this.I_key[next] == t;
			}
		}
		if find ==true {
			findIndex = this.I_check[next];
			p = next;
		}
		pChar = c;
		i++
	}
	if (findIndex != 0) {
		for _,item := range this.I_guides[findIndex] {
			r := this.getIllegalResult(item, i - 1, text, p, pIndex, flag);
			if	r!=nil {
				for j:=r.Start;j<i; j++{
					result[j]=replaceChar;
				}
				break;			
			}
		}
	}
	return string (result)
}

func (this *IllegalWordsSearch)findStart( keyword string, end int, srcText string,  p int, pIndex []int) int  {
	_srcText := []rune (srcText)
	if end + 1 < len(_srcText) {
		en1 := isEnglishOrNumber(_srcText[end+1] );
		en2 := isEnglishOrNumber(_srcText[end]);
		if (en1 && en2) { 
			return -1;
		}
	}
	n := len( []rune(keyword)) 
	start := end;
	pp := p;
	for n > 0 {
		pi := pIndex[start];
		start--;
		if (pi != pp) {
			 n--; 
			 pp = pi; 
		}
		if (start == -1){
			return 0
		}
	}
 
	sn1 := isEnglishOrNumber(_srcText[start]);
	start++;
	sn2 := isEnglishOrNumber(_srcText[start]);
	if (sn1 && sn2) {
		return -1;
	}
	return start;
}

func isEnglishOrNumber(c int32) bool {
	if (c < 128) {
		if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
			return true;
		}
	}
	return false;
}


func (this *IllegalWordsSearch)getIllegalResult( index int, end int, srcText string,p int, pIndex []int, flag int) *IllegalWordsSearchResult{
	if (this.UseBlacklistFilter) {
		b := this._blacklist[index];
		if ((b | flag) != b) { return nil; }
	}
	keyword := this.I_keywords[index];
	_keyword :=[]rune (keyword);
	_srcText:=[]rune (srcText);
	if (len(_keyword)==1) {
		if (this.toSenseWord(_srcText[end]) == this.toSenseWord(_keyword[0]))==false { 
			return nil;
		 }
		return NewIllegalWordsSearchResult(keyword, end, end, srcText,0xffffffff);
	}
	start := this.findStart(keyword, end, srcText, p, pIndex);
	if (start == -1) { 
		return nil; 
	}
	if (this.toSenseWord(_srcText[start]) == this.toSenseWord(_keyword[0]))==false { 
		return nil;
	 }
	if (this.UseBlacklistFilter) {
		return NewIllegalWordsSearchResult(keyword, start, end, srcText, this._blacklist[index]);
	}
	return NewIllegalWordsSearchResult(keyword, start, end, srcText,0xffffffff);
}
func (this *IllegalWordsSearch)toSenseWord( c int32) int32{
	if (this.UseIgnoreCase) {
		if (c >= 'A' && c <= 'Z') {
			return int32 (c | 0x20);
		} 
	}
	if (this.UseDBCcaseConverter) {
		if (c == 12288){
			return int32(' ');
		}
		if (c >= 65280 && c < 65375) {
			k := int32 (c - 65248);
			if (this.UseIgnoreCase) {
				if ('A' <= k && k <= 'Z') {
					k =  int32(k | 0x20);
				}
			}
			return k;
		}
	}
	if (this.UseSimplifiedChineseConverter) {
		if (c >= 0x4e00 && c <= 0x9fa5) {
			return GetSimplified( int (c - 0x4e00));
		}
	}
	return c;
}
func (this *IllegalWordsSearch)toSenseWord2(text string) string{
	_text:=[]rune(text)
	stringBuilder:=make([]int32,len(_text))
	for i,c :=range _text{
		stringBuilder[i]=this.toSenseWord(c)
	}
	return string (stringBuilder)
}

func (this *IllegalWordsSearch)SetSkipWords(skipList string){
	this._skipBitArray = make([]bool,0x10000)
	for _,c := range skipList {
		this._skipBitArray[c]=true
	} 
}

func (this *IllegalWordsSearch)SetBlacklist(blacklist []int) error {
	if	len(this.I_keywords) != len(blacklist) {
		return errors.New("请关键字与黑名单列表的长度要一样长！")
	}
	this._blacklist = blacklist;
	return nil
}

func (this *IllegalWordsSearch)SetKeywords(keywords []string)   {
	list:=make([]string,0)
	for _,val:=range keywords{
		list=append(list,this.toSenseWord2(val))
	}
	this.BaseSearchEx.SetKeywords(list)
}

func (this *IllegalWordsSearch)Save2(f *os.File){
	this.BaseSearchEx.Save2(f)

	f.Write(this.intToBytes(this.boolToInt(this.UseSkipWordFilter)))
	f.Write(this.intToBytes(len(this._skipBitArray)))
	for _,key:=range this._skipBitArray {
		f.Write(this.intToBytes(this.boolToInt(key)))
	}
	f.Write(this.intToBytes(this.boolToInt(this.UseDuplicateWordFilter)))
	f.Write(this.intToBytes(this.boolToInt(this.UseBlacklistFilter)))

	f.Write(this.intToBytes(len(this._blacklist)))
	for _,key:=range this._blacklist {
		f.Write(this.intToBytes(key))
	}
	f.Write(this.intToBytes(this.boolToInt(this.UseDBCcaseConverter)))
	f.Write(this.intToBytes(this.boolToInt(this.UseSimplifiedChineseConverter)))
	f.Write(this.intToBytes(this.boolToInt(this.UseIgnoreCase)))
}

func (this *IllegalWordsSearch)Load2(f *os.File){
	this.BaseSearchEx.Load2(f)
	intBs := make([] byte,4)

	f.Read(intBs)
	this.UseSkipWordFilter=this.intToBool(this.bytesToInt(intBs))

	f.Read(intBs)
	length:= this.bytesToInt(intBs);
	this._skipBitArray=make([]bool,length)
	for i := 0; i < length; i++ {
		f.Read(intBs)
		this._skipBitArray[i]= this.intToBool(this.bytesToInt(intBs));
	}

	f.Read(intBs)
	this.UseDuplicateWordFilter=this.intToBool(this.bytesToInt(intBs))
	f.Read(intBs)
	this.UseBlacklistFilter=this.intToBool(this.bytesToInt(intBs))

	f.Read(intBs)
	length = this.bytesToInt(intBs);
	this._blacklist=make([]int,length)
	for i := 0; i < length; i++ {
		f.Read(intBs)
		this._blacklist[i]=  this.bytesToInt(intBs);
	}
	f.Read(intBs)
	this.UseDBCcaseConverter=this.intToBool(this.bytesToInt(intBs))

	f.Read(intBs)
	this.UseSimplifiedChineseConverter=this.intToBool(this.bytesToInt(intBs))
 
	f.Read(intBs)
	this.UseIgnoreCase=this.intToBool(this.bytesToInt(intBs))
}

 

func (this *IllegalWordsSearch)intToBytes(i int) []byte{
    x := int32(i)
    bytesBuffer := bytes.NewBuffer([]byte{})
	binary.Write(bytesBuffer, binary.BigEndian, x)
    return bytesBuffer.Bytes()
}
func (this *IllegalWordsSearch)bytesToInt(bs []byte) int{
	bytesBuffer := bytes.NewBuffer(bs)
    var x int32
    binary.Read(bytesBuffer, binary.BigEndian, &x)
	return int(x)
}
func (this *IllegalWordsSearch)boolToInt( b bool) int{
	if	b==true{
		return 1
	}
	return 0
}
func (this *IllegalWordsSearch)intToBool(i int) bool{
	if	i==0{
		return false
	}
	return true
}
  