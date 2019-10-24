package main

import "fmt"
import . "./ToolGood/Words"
 
  
func main()  {
 
	test := "我是中国人"
	 list := []string{"中国","国人","zg人"}

	//   t2:= []rune(test)
	//   fmt.Println("s1=", t2)
	//   t2[1]='3'
	//   fmt.Println("s1=", t2)
	//   fmt.Println("s1=",string (t2))

 	
	search:= NewStringSearch()
	search.SetKeywords(list)


	t:=search.Replace(test,'*')

  
    fmt.Println("s1=", t)
    fmt.Println("s1=", test)
    fmt.Println("s1=", list)
}
