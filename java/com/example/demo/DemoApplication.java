package com.example.demo;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import ToolGood.Words.StringSearch;
import ToolGood.Words.StringSearchEx;
import ToolGood.Words.WordsSearch;
import ToolGood.Words.WordsSearchEx;
import ToolGood.Words.WordsSearchResult;

public class DemoApplication {

	public static void main(String[] args) {
		test_StringSearch();
		test_WordsSearch();

		test_StringSearchEx();
		test_WordsSearchEx();

		try {
			test_save_load();
		} catch (Exception e) {
			//TODO: handle exception
		}
		
	}

	private static void test_StringSearch(){
		String test = "我是中国人";
			List<String> list=new ArrayList<String>();
			list.add("中国");		 
			list.add("国人");
			list.add("zg人");
			System.out.println("StringSearch run Test.");

            StringSearch iwords = new StringSearch();
            iwords.SetKeywords(list);

			boolean b = iwords.ContainsAny(test);
			if(b==false){
				System.out.println("ContainsAny is Error.");
			}

			String f = iwords.FindFirst(test);
			if(f!="中国"){
				System.out.println("FindFirst is Error.");
			}

			List<String> all = iwords.FindAll(test);
			if(all.get(0)!="中国"){
				System.out.println("FindAll is Error.");
			}
			if(all.get(1)!="国人"){
				System.out.println("FindAll is Error.");
			}
			if(all.size()!=2){
				System.out.println("FindAll is Error.");
			}

    

			String str = iwords.Replace(test, '*');
			if(str.equals("我是***")==false ){
				System.out.println("Replace is Error.");
			}
	}

	private static void test_StringSearchEx(){
		String test = "我是中国人";
			List<String> list=new ArrayList<String>();
			list.add("中国");		 
			list.add("国人");
			list.add("zg人");
			System.out.println("StringSearchEx run Test.");

            StringSearchEx iwords = new StringSearchEx();
            iwords.SetKeywords(list);

			boolean b = iwords.ContainsAny(test);
			if(b==false){
				System.out.println("ContainsAny is Error.");
			}

			String f = iwords.FindFirst(test);
			if(f!="中国"){
				System.out.println("FindFirst is Error.");
			}

			List<String> all = iwords.FindAll(test);
			if(all.get(0)!="中国"){
				System.out.println("FindAll is Error.");
			}
			if(all.get(1)!="国人"){
				System.out.println("FindAll is Error.");
			}
			if(all.size()!=2){
				System.out.println("FindAll is Error.");
			}

    

			String str = iwords.Replace(test, '*');
			if(str.equals("我是***")==false ){
				System.out.println("Replace is Error.");
			}
	}

	private static void test_WordsSearch(){
		String test = "我是中国人";
			List<String> list=new ArrayList<String>();
			list.add("中国");		 
			list.add("国人");
			list.add("zg人");
			System.out.println("WordsSearch run Test.");

            WordsSearch iwords = new WordsSearch();
            iwords.SetKeywords(list);

			boolean b = iwords.ContainsAny(test);
			if(b==false){
				System.out.println("ContainsAny is Error.");
			}

			WordsSearchResult f = iwords.FindFirst(test);
			if(f.Keyword!="中国"){
				System.out.println("FindFirst is Error.");
			}

			List<WordsSearchResult> all = iwords.FindAll(test);
			if(all.get(0).Keyword!="中国"){
				System.out.println("FindAll is Error.");
			}
			if(all.get(1).Keyword!="国人"){
				System.out.println("FindAll is Error.");
			}
			if(all.size()!=2){
				System.out.println("FindAll is Error.");
			}

    

			String str = iwords.Replace(test, '*');
			if(str.equals("我是***")==false ){
				System.out.println("Replace is Error.");
			}
	}

	private static void test_WordsSearchEx(){
		String test = "我是中国人";
			List<String> list=new ArrayList<String>();
			list.add("中国");		 
			list.add("国人");
			list.add("zg人");
			System.out.println("WordsSearchEx run Test.");

            WordsSearchEx iwords = new WordsSearchEx();
            iwords.SetKeywords(list);

			boolean b = iwords.ContainsAny(test);
			if(b==false){
				System.out.println("ContainsAny is Error.");
			}

			WordsSearchResult f = iwords.FindFirst(test);
			if(f.Keyword!="中国"){
				System.out.println("FindFirst is Error.");
			}

			List<WordsSearchResult> all = iwords.FindAll(test);
			if(all.get(0).Keyword!="中国"){
				System.out.println("FindAll is Error.");
			}
			if(all.get(1).Keyword!="国人"){
				System.out.println("FindAll is Error.");
			}
			if(all.size()!=2){
				System.out.println("FindAll is Error.");
			}
    

			String str = iwords.Replace(test, '*');
			if(str.equals("我是***")==false ){
				System.out.println("Replace is Error.");
			}
	}

	private static void test_save_load() throws IOException {
		String test = "我是中国人";
			List<String> list=new ArrayList<String>();
			list.add("中国");		 
			list.add("国人");
			list.add("zg人");
			System.out.println("test_save_load run Test.");

			StringSearchEx search = new StringSearchEx();
			search.SetKeywords(list);
			search.Save("1.dat");


            StringSearchEx iwords = new StringSearchEx();
	 		iwords.Load("1.dat");
			
			boolean b = iwords.ContainsAny(test);
			if(b==false){
				System.out.println("ContainsAny is Error.");
			}

			String f = iwords.FindFirst(test);
			if(f!="中国"){
				System.out.println("FindFirst is Error.");
			}

			List<String> all = iwords.FindAll(test);
			if(all.get(0)!="中国"){
				System.out.println("FindAll is Error.");
			}
			if(all.get(1)!="国人"){
				System.out.println("FindAll is Error.");
			}
			if(all.size()!=2){
				System.out.println("FindAll is Error.");
			}

    

			String str = iwords.Replace(test, '*');
			if(str.equals("我是***")==false ){
				System.out.println("Replace is Error.");
			}
	}
}
