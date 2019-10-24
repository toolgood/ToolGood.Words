package main

import "fmt"
import . "./ToolGood/Words"
 
  
func main()  {
	test_StringSearch();
	test_WordsSearch();
	test_StringSearchEx();
	test_WordsSearchEx();
	text_Save_Load()
}
func test_StringSearch(){
	fmt.Println("test_StringSearch");

	test := "我是中国人"
	list := []string{"中国","国人","zg人"}

	search:= NewStringSearch()
	search.SetKeywords(list)

	b := search.ContainsAny(test);
	if b==false {
		fmt.Println("ContainsAny is Error.");
	}

	f := search.FindFirst(test);
	if(f!="中国"){
		fmt.Println("FindFirst is Error.");
	}

	all := search.FindAll(test);
	if(all[0]!="中国"){
		fmt.Println("FindAll is Error.");
	}
	if(all[1] !="国人"){
		fmt.Println("FindAll is Error.");
	}
	if(len(all) !=2){
		fmt.Println("FindAll is Error.");
	}
	str := search.Replace(test, '*');
	if(str != "我是***"  ){
		fmt.Println("Replace is Error.");
	}
}
func test_WordsSearch(){
	fmt.Println("test_WordsSearch");

	test := "我是中国人"
	list := []string{"中国","国人","zg人"}

	search:= NewWordsSearch()
	search.SetKeywords(list)

	b := search.ContainsAny(test);
	if b==false {
		fmt.Println("ContainsAny is Error.");
	}

	f := search.FindFirst(test);
	if(f.Keyword!="中国"){
		fmt.Println("FindFirst is Error.");
	}

	all := search.FindAll(test);
	if(all[0].Keyword!="中国"){
		fmt.Println("FindAll is Error.");
	}
	if(all[1].Keyword !="国人"){
		fmt.Println("FindAll is Error.");
	}
	if(len(all) !=2){
		fmt.Println("FindAll is Error.");
	}
	str := search.Replace(test, '*');
	if(str != "我是***"  ){
		fmt.Println("Replace is Error.");
	}
}
func test_StringSearchEx(){
	fmt.Println("test_StringSearchEx");

	test := "我是中国人"
	list := []string{"中国","国人","zg人"}

	search:= NewStringSearch()
	search.SetKeywords(list)

	b := search.ContainsAny(test);
	if b==false {
		fmt.Println("ContainsAny is Error.");
	}

	f := search.FindFirst(test);
	if(f!="中国"){
		fmt.Println("FindFirst is Error.");
	}

	all := search.FindAll(test);
	if(all[0]!="中国"){
		fmt.Println("FindAll is Error.");
	}
	if(all[1] !="国人"){
		fmt.Println("FindAll is Error.");
	}
	if(len(all) !=2){
		fmt.Println("FindAll is Error.");
	}
	str := search.Replace(test, '*');
	if(str != "我是***"  ){
		fmt.Println("Replace is Error.");
	}
}
func test_WordsSearchEx(){
	fmt.Println("test_WordsSearchEx");

	test := "我是中国人"
	list := []string{"中国","国人","zg人"}

	search:= NewWordsSearch()
	search.SetKeywords(list)

	b := search.ContainsAny(test);
	if b==false {
		fmt.Println("ContainsAny is Error.");
	}

	f := search.FindFirst(test);
	if(f.Keyword!="中国"){
		fmt.Println("FindFirst is Error.");
	}

	all := search.FindAll(test);
	if(all[0].Keyword!="中国"){
		fmt.Println("FindAll is Error.");
	}
	if(all[1].Keyword !="国人"){
		fmt.Println("FindAll is Error.");
	}
	if(len(all) !=2){
		fmt.Println("FindAll is Error.");
	}
	str := search.Replace(test, '*');
	if(str != "我是***"  ){
		fmt.Println("Replace is Error.");
	}
}
func text_Save_Load(){
	fmt.Println("text_Save_Load");

	test := "我是中国人"
	list := []string{"中国","国人","zg人"}

	search2:= NewStringSearchEx()
	search2.SetKeywords(list)
	search2.Save("1.dat")
 

	search:= NewStringSearchEx()
	search.Load("1.dat")
 

	b := search.ContainsAny(test);
	if b==false {
		fmt.Println("ContainsAny is Error.");
	}

	f := search.FindFirst(test);
	if(f!="中国"){
		fmt.Println("FindFirst is Error.");
	}

	all := search.FindAll(test);
	if(all[0]!="中国"){
		fmt.Println("FindAll is Error.");
	}
	if(all[1] !="国人"){
		fmt.Println("FindAll is Error.");
	}
	if(len(all) !=2){
		fmt.Println("FindAll is Error.");
	}
	str := search.Replace(test, '*');
	if(str != "我是***"  ){
		fmt.Println("Replace is Error.");
	}
}