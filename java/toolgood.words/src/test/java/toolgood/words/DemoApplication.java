package toolgood.words;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.function.Function;
import java.util.stream.Stream;

import org.springframework.core.io.ClassPathResource;
import org.springframework.util.StopWatch;

public class DemoApplication {

	public static void main(String[] args) throws Exception {

		test_StringSearch();
		test_WordsSearch();

		test_StringSearchEx();
		test_WordsSearchEx();

		test_StringSearchEx2();
		test_WordsSearchEx2();
		test_IllegalWordsSearch();

		test_StringMatch();
		test_WordsMatch();

		test_StringMatchEx();
		test_WordsMatchEx();

		test_PinyinMatch();
		test_PinyinMatch2();

		test_Pinyin();
		test_words();

		// try {
		// test_save_load();
		// test_IllegalWordsSearch_loadWordsFormBinaryFile();
		// } catch (Exception e) {
		// e.printStackTrace();
		// }
		// test_times();

		test_issues_54();
		test_issues_57();
		test_issues_57_2();
		test_issues_57_3();
		test_issues_65();
		test_issues_74();
		test_issues_90();
	}

	private static void test_StringSearch() {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("StringSearch run Test.");

		StringSearch iwords = new StringSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.FindFirst(test);
		if (f != "中国") {
			System.out.println("FindFirst is Error.");
		}

		List<String> all = iwords.FindAll(test);
		if (all.get(0) != "中国") {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1) != "国人") {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_StringSearchEx() {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("StringSearchEx run Test.");

		StringSearchEx iwords = new StringSearchEx();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.FindFirst(test);
		if (f != "中国") {
			System.out.println("FindFirst is Error.");
		}

		List<String> all = iwords.FindAll(test);
		if (all.get(0) != "中国") {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1) != "国人") {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_StringSearchEx2() {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("StringSearchEx2 run Test.");

		StringSearchEx2 iwords = new StringSearchEx2();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.FindFirst(test);
		if (f != "中国") {
			System.out.println("FindFirst is Error.");
		}

		List<String> all = iwords.FindAll(test);
		if (all.get(0) != "中国") {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1) != "国人") {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_WordsSearch() {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("WordsSearch run Test.");

		WordsSearch iwords = new WordsSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		WordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword != "中国") {
			System.out.println("FindFirst is Error.");
		}

		List<WordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword != "中国") {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword != "国人") {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_WordsSearchEx() throws IOException {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("WordsSearchEx run Test.");

		WordsSearchEx iwords2 = new WordsSearchEx();
		iwords2.SetKeywords(list);
		iwords2.Save("WordsSearchEx.dat");

		WordsSearchEx iwords = new WordsSearchEx();
		iwords.Load("WordsSearchEx.dat");

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		WordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword.equals("中国") == false) {
			System.out.println("FindFirst is Error.");
		}

		List<WordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword.equals("中国") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword.equals("国人") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_WordsSearchEx2() {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("WordsSearchEx2 run Test.");

		WordsSearchEx2 iwords = new WordsSearchEx2();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		WordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword != "中国") {
			System.out.println("FindFirst is Error.");
		}

		List<WordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword != "中国") {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword != "国人") {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_IllegalWordsSearch() {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("IllegalWordsSearch run Test.");

		IllegalWordsSearch iwords = new IllegalWordsSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		IllegalWordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword.equals("中国") == false) {
			System.out.println("FindFirst is Error.");
		}

		List<IllegalWordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword.equals("中国") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword.equals("国人") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_StringMatch() throws Exception {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("[中美]国");
		list.add("国人");
		list.add("zg人");
		System.out.println("StringMatch run Test.");

		StringMatch iwords = new StringMatch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.FindFirst(test);
		if (!f.equals("中国")) {
			System.out.println("FindFirst is Error.");
		}

		List<String> all = iwords.FindAll(test);
		if (!all.get(0).equals("中国")) {
			System.out.println("FindAll is Error.");
		}
		if (!all.get(1).equals("国人")) {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_StringMatchEx() throws Exception {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("[中美]国");
		list.add("国人");
		list.add("zg人");
		System.out.println("StringMatchEx run Test.");

		StringMatchEx iwords = new StringMatchEx();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.FindFirst(test);
		if (!f.equals("中国")) {
			System.out.println("FindFirst is Error.");
		}

		List<String> all = iwords.FindAll(test);
		if (!all.get(0).equals("中国")) {
			System.out.println("FindAll is Error.");
		}
		if (!all.get(1).equals("国人")) {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_WordsMatch() throws Exception {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("[中美]国");
		list.add("国人");
		list.add("zg人");
		System.out.println("WordsMatch run Test.");

		WordsMatch iwords = new WordsMatch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		WordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword.equals("中国") == false) {
			System.out.println("FindFirst is Error.");
		}

		List<WordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword.equals("中国") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword.equals("国人") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_WordsMatchEx() throws Exception {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("[中美]国");
		list.add("国人");
		list.add("zg人");
		System.out.println("WordsMatchEx run Test.");

		WordsMatchEx iwords = new WordsMatchEx();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		WordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword.equals("中国") == false) {
			System.out.println("FindFirst is Error.");
		}

		List<WordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword.equals("中国") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword.equals("国人") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_PinyinMatch() throws NumberFormatException, IOException {
		String s = "北京|天津|河北|辽宁|吉林|黑龙江|山东|江苏|上海|浙江|安徽|福建|江西|广东|广西|海南|河南|湖南|湖北|山西|内蒙古|宁夏|青海|陕西|甘肃|新疆|四川|贵州|云南|重庆|西藏|香港|澳门|台湾";
		List<String> list = new ArrayList<String>();
		String[] ss = s.split("\\|");
		for (String st : ss) {
			list.add(st);
		}
		PinyinMatch match = new PinyinMatch();
		match.SetKeywords(list);
		System.out.println("PinyinMatch run Test.");

		List<String> all = match.Find("BJ");
		if (all.get(0).equals("北京") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}

		all = match.Find("北J");
		if (all.get(0).equals("北京") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}

		all = match.Find("北Ji");
		if (all.get(0).equals("北京") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}
		all = match.Find("Su");
		if (all.get(0).equals("江苏") == false) {
			System.out.println("Find is Error.");
		}

		all = match.Find("Sdon");
		if (all.get(0).equals("山东") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}
		all = match.Find("S东");
		if (all.get(0).equals("山东") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}

		List<Integer> all2 = match.FindIndex("BJ");
		if (all2.get(0) != 0) {
			System.out.println("FindIndex is Error.");
		}
		if (all2.size() != 1) {
			System.out.println("FindIndex is Error.");
		}

		all = match.FindWithSpace("S 东");
		if (all.get(0).equals("山东") == false) {
			System.out.println("FindWithSpace is Error.");
		}
		if (all.size() != 1) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("h 江");
		if (all.get(0).equals("黑龙江") == false) {
			System.out.println("FindWithSpace is Error.");
		}

		all2 = match.FindIndexWithSpace("B J");
		if (all2.get(0) != 0) {
			System.out.println("FindIndexWithSpace is Error.");
		}
		if (all2.size() != 1) {
			System.out.println("FindIndexWithSpace is Error.");
		}

		all = match.FindWithSpace("京 北");
		if (all.size() != 0) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("黑龙 龙江");
		if (all.size() != 0) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("黑龙 江");
		if (all.get(0).equals("黑龙江") == false) {
			System.out.println("FindWithSpace is Error.");
		}
		all = match.FindWithSpace("黑 龙 江");
		if (all.get(0).equals("黑龙江") == false) {
			System.out.println("FindWithSpace is Error.");
		}
	}

	private static void test_PinyinMatch2() throws Exception {
		String s = "北京|天津|河北|辽宁|吉林|黑龙江|山东|江苏|上海|浙江|安徽|福建|江西|广东|广西|海南|河南|湖南|湖北|山西|内蒙古|宁夏|青海|陕西|甘肃|新疆|四川|贵州|云南|重庆|西藏|香港|澳门|台湾";
		List<String> list = new ArrayList<String>();
		String[] ss = s.split("\\|");
		for (String st : ss) {
			list.add(st);
		}
		PinyinMatch2<String> match = new PinyinMatch2<String>(list);
		match.SetKeywordsFunc(new Function<String, String>() {
			@Override
			public String apply(String t) {
				return t;
			}
		});

		System.out.println("PinyinMatch2 run Test.");

		List<String> all = match.Find("BJ");
		if (all.get(0).equals("北京") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}

		all = match.Find("北J");
		if (all.get(0).equals("北京") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}

		all = match.Find("北Ji");
		if (all.get(0).equals("北京") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}
		all = match.Find("Su");
		if (all.get(0).equals("江苏") == false) {
			System.out.println("Find is Error.");
		}

		all = match.Find("Sdon");
		if (all.get(0).equals("山东") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}
		all = match.Find("S东");
		if (all.get(0).equals("山东") == false) {
			System.out.println("Find is Error.");
		}
		if (all.size() != 1) {
			System.out.println("Find is Error.");
		}

		all = match.FindWithSpace("S 东");
		if (all.get(0).equals("山东") == false) {
			System.out.println("FindWithSpace is Error.");
		}
		if (all.size() != 1) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("h 江");
		if (all.get(0).equals("黑龙江") == false) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("京 北");
		if (all.size() != 0) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("黑龙 龙江");
		if (all.size() != 0) {
			System.out.println("FindWithSpace is Error.");
		}

		all = match.FindWithSpace("黑龙 江");
		if (all.get(0).equals("黑龙江") == false) {
			System.out.println("FindWithSpace is Error.");
		}
		all = match.FindWithSpace("黑 龙 江");
		if (all.get(0).equals("黑龙江") == false) {
			System.out.println("FindWithSpace is Error.");
		}
	}

	private static void test_save_load() throws IOException {
		String test = "我是中国人";
		List<String> list = new ArrayList<String>();
		list.add("中国");
		list.add("国人");
		list.add("zg人");
		System.out.println("test_save_load run Test.");

		StringSearchEx2 search = new StringSearchEx2();
		search.SetKeywords(list);
		search.Save("1.dat");

		StringSearchEx2 iwords = new StringSearchEx2();
		iwords.Load("1.dat");

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.FindFirst(test);
		if (f != "中国") {
			System.out.println("FindFirst is Error.");
		}

		List<String> all = iwords.FindAll(test);
		if (all.get(0) != "中国") {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1) != "国人") {
			System.out.println("FindAll is Error.");
		}
		if (all.size() != 2) {
			System.out.println("FindAll is Error.");
		}

		String str = iwords.Replace(test, '*');
		if (str.equals("我是***") == false) {
			System.out.println("Replace is Error.");
		}
	}

	private static void test_times() {
		String ts = readLineByLineJava8("BadWord.txt");
		String[] sp = ts.split("[\r\n]");
		List<String> list = new ArrayList<String>();
		for (String item : sp) {
			list.add(item);
		}
		String words = readLineByLineJava8("Talk.txt");

		StringSearchEx2 iwords = new StringSearchEx2();
		iwords.SetKeywords(list);

		StopWatch sw = new StopWatch();
		sw.start("校验耗时");
		for (int i = 0; i < 100000; i++) {
			// iwords.ContainsAny(words);
			iwords.FindAll(words);
			// System.out.println(list2.size());
		}
		sw.stop();
		System.out.println(sw.getTotalTimeMillis() + "ms");

	}

	private static String readLineByLineJava8(String filePath) {
		StringBuilder contentBuilder = new StringBuilder();
		try (Stream<String> stream = Files.lines(Paths.get(filePath), StandardCharsets.UTF_8)) {
			stream.forEach(s -> contentBuilder.append(s).append("\n"));
		} catch (IOException e) {
			e.printStackTrace();
		}
		return contentBuilder.toString();
	}

	private static void test_IllegalWordsSearch_loadWordsFormBinaryFile() throws IOException {

		long l1 = System.currentTimeMillis();

		IllegalWordsSearch search = new IllegalWordsSearch();
		long l2 = System.currentTimeMillis();
		System.out.println("IllegalWordsSearch init time:" + (l2 - l1));

		search.Load(new ClassPathResource("IllegalWordsSearch.dat").getFile().getAbsolutePath());
		long l3 = System.currentTimeMillis();
		System.out.println("load Load time:" + (l3 - l2));

		String test = "卖毒品哈哈哈哈毛澤東porn哈哈哈哈胡锦涛pornasds哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛胡锦涛撒旦撒旦ｐｏｒｎporn哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn"
				+ "哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn哈哈哈哈胡錦濤porn哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn"
				+ "哈哈哈哈胡锦涛porn哈哈哈哈胡锦涛porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn"
				+ "哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn"
				+ "哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn"
				+ "哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn"
				+ "哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn哈哈哈哈或porn";

		boolean b = search.ContainsAny(test);
		if (!b) {
			System.out.println("ContainsAny is Error.");
		}
		long l4 = System.currentTimeMillis();
		System.out.println("ContainsAny time:" + (l4 - l3));

		String str = search.Replace(test, '*');
		long l5 = System.currentTimeMillis();
		System.out.println("Replace Result:" + str);
		System.out.println("Replace time:" + (l5 - l4));
	}

	private static void test_IllegalWordsSearch_saveToBinaryFile() throws IOException {
		List<String> list = new ArrayList<>();
		try (BufferedReader bufferedReader = new BufferedReader(
				new InputStreamReader(new ClassPathResource("sensi_words.txt").getInputStream()))) {
			for (String line = bufferedReader.readLine(); line != null; line = bufferedReader.readLine()) {
				list.add(line);
			}
		}
		IllegalWordsSearch search = new IllegalWordsSearch();
		search.SetKeywords(list);
		search.Save("IllegalWordsSearch.dat");
	}

	private static void test_Pinyin() throws NumberFormatException, IOException {
		System.out.println("text_Pinyin run Test.");
		List<String> t = WordsHelper.GetAllPinyin('芃');
		if (t.get(0).equals("Peng") == false) {
			System.out.println("GetAllPinyin is Error.");
		}

		String a = WordsHelper.GetPinyinFast("阿");
		if (a.equals("A") == false) {
			System.out.println("GetPinyinFast is Error.");
		}

		String b = WordsHelper.GetPinyin("摩擦棒");
		if (b.equals("MoCaBang") == false) {
			System.out.println("GetPinyin is Error.");
		}
		b = WordsHelper.GetPinyin("秘鲁");
		if (b.equals("BiLu") == false) {
			System.out.println("GetPinyin is Error.");
		}

		String py = WordsHelper.GetPinyinFast("我爱中国");
		if (py.equals("WoAiZhongGuo") == false) {
			System.out.println("GetPinyinFast is Error.");
		}

		py = WordsHelper.GetPinyin("快乐，乐清");
		if (py.equals("KuaiLe，YueQing") == false) {
			System.out.println("GetPinyin is Error.");
		}

		py = WordsHelper.GetPinyin("快乐清理");
		if (py.equals("KuaiLeQingLi") == false) {
			System.out.println("GetPinyin is Error.");
		}

		py = WordsHelper.GetPinyin("我爱中国", true);
		if (py.equals("WǒÀiZhōngGuó") == false) {
			System.out.println("GetPinyin is Error.");
		}

		py = WordsHelper.GetFirstPinyin("我爱中国");
		if (py.equals("WAZG") == false) {
			System.out.println("GetPinyin is Error.");
		}

		List<String> pys = WordsHelper.GetAllPinyin('传');
		if (pys.get(0).equals("Chuan") == false) {
			System.out.println("GetAllPinyin is Error.");
		}
		if (pys.get(1).equals("Zhuan") == false) {
			System.out.println("GetAllPinyin is Error.");
		}

		py = WordsHelper.GetPinyinForName("单一一");
		if (py.equals("ShanYiYi") == false) {
			System.out.println("GetPinyinForName is Error.");
		}

		py = WordsHelper.GetPinyinForName("单一一", true);
		if (py.equals("ShànYīYī") == false) {
			System.out.println("GetPinyinForName is Error.");
		}

		List<String> all = WordsHelper.GetAllPinyin('石');
		if (all.size() == 0) {
			System.out.println("GetAllPinyin is Error.");
		}

	}

	private static void test_words() throws Exception {
		System.out.println("test_words run Test.");
		String s = WordsHelper.ToSimplifiedChinese("壹佰贰拾叁億肆仟伍佰陆拾柒萬捌仟玖佰零壹元壹角贰分");
		if (s.equals("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分") == false) {
			System.out.println("ToSimplifiedChinese is Error.");
		}

		String tw = WordsHelper.ToTraditionalChinese("壹佰贰拾叁亿肆仟伍佰陆拾柒万捌仟玖佰零壹元壹角贰分");
		if (tw.equals("壹佰貳拾叄億肆仟伍佰陸拾柒萬捌仟玖佰零壹元壹角貳分") == false) {
			System.out.println("ToTraditionalChinese is Error.");
		}

		String tw2 = WordsHelper.ToTraditionalChinese("原代码11", 2);
		if (tw2.equals("原始碼11") == false) {
			System.out.println("ToTraditionalChinese is Error.");
		}

		String tw3 = WordsHelper.ToTraditionalChinese("反反复复", 2);
		if (tw3.equals("反反覆覆") == false) {
			System.out.println("ToTraditionalChinese is Error.");
		}

		String tw4 = WordsHelper.ToTraditionalChinese("这人考虑事情总是反反复复的", 2);
		if (tw4.equals("這人考慮事情總是反反覆覆的") == false) {
			System.out.println("ToTraditionalChinese is Error.");
		}

	}

	public static void test_issues_54() {
		IllegalWordsSearch search = new IllegalWordsSearch();
		search.SetKeywords(Arrays.asList("test", "world", "this", "hello", "monster"));
		String result = search.Replace("test, hahaha, this is a hello world", '*');
		if (result.equals("****, hahaha, **** is a ***** *****") == false) {
			System.out.println("IllegalWordsSearch Replace is Error.");
		}
	}

	public static void test_issues_57() {
		String test = "一,二二,三三三,四四四四,五五五五五,六六六六六六";
		List<String> list = new ArrayList<String>();
		list.add("一");
		list.add("二二");
		list.add("三三三");
		list.add("四四四四");
		list.add("五五五五五");
		list.add("六六六六六六");
		System.out.println("test_issues_57 run Test.");

		IllegalWordsSearch iwords = new IllegalWordsSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		IllegalWordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword.equals("一") == false) {
			System.out.println("FindFirst is Error.");
		}

		List<IllegalWordsSearchResult> all = iwords.FindAll(test);
		if (all.get(0).Keyword.equals("一") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(1).Keyword.equals("二二") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(2).Keyword.equals("三三三") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(3).Keyword.equals("四四四四") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(4).Keyword.equals("五五五五五") == false) {
			System.out.println("FindAll is Error.");
		}
		if (all.get(5).Keyword.equals("六六六六六六") == false) {
			System.out.println("FindAll is Error.");
		}
	}

	public static void test_issues_57_2() {
		String test = "jameson吃饭";
		List<String> list = new ArrayList<String>();
		list.add("jameson吃饭");
		list.add("吃饭jameson");
		System.out.println("test_issues_57_2 run Test.");

		IllegalWordsSearch iwords = new IllegalWordsSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		IllegalWordsSearchResult f = iwords.FindFirst(test);
		if (f.Keyword.equals("jameson吃饭") == false) {
			System.out.println("FindFirst is Error.");
		}
	}

	public static void test_issues_57_3() {
		String test = "his is sha ash";
		List<String> list = new ArrayList<String>();
		list.add("ash");
		list.add("sha");
		list.add("bcd");
		System.out.println("test_issues_57_3 run Test.");

		IllegalWordsSearch iwords = new IllegalWordsSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		IllegalWordsSearchResult f = iwords.FindFirst(test);
		if (f == null || f.Keyword.equals("sha") == false) {
			System.out.println("FindFirst is Error.");
		}
	}

	public static void test_issues_65() {
		String test = "fFuck";
		List<String> list = new ArrayList<String>();
		list.add("fuck");
		list.add("ffx");
		list.add("bcd");
		System.out.println("test_issues_65 run Test.");

		IllegalWordsSearch iwords = new IllegalWordsSearch();
		iwords.SetKeywords(list);

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}

		String f = iwords.Replace(test);
		if (f == null || f.equals("*****") == false) {
			System.out.println("Replace is Error.");
		}
	}

	public static void test_issues_74() {
		List<String> list = loadKeywords(new File("sensi_words.txt"));
		System.out.println("test_issues_74 run Test.");

		IllegalWordsSearch iwords = new IllegalWordsSearch();
		iwords.SetKeywords(list);
		String test = "机机歪歪";

		boolean b = iwords.ContainsAny(test);
		if (b == false) {
			System.out.println("ContainsAny is Error.");
		}
	}

	public static List<String> loadKeywords(File file) {
		List<String> keyArray = new ArrayList<String>();
		try {
			BufferedReader br = new BufferedReader(new FileReader(file));// 构造一个BufferedReader类来读取文件
			String s = null;
			while ((s = br.readLine()) != null) {// 使用readLine方法，一次读一行
				keyArray.add(s);
			}
			br.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return keyArray;
	}

	public static void test_issues_90() {
		System.out.println("test_issues_90 run Test.");
		String word = "钢笔狗，芭芭拉";
		String[] arrays = { "爱液", "按摩棒", "爆草", "包二奶", "暴干", "暴奸", "暴乳", "爆乳", "暴淫", "屄", "被操", "被插", "被干", "逼奸", "仓井空",
				"插暴", "操逼", "操黑", "操烂", "肏你", "肏死", "操死", "操我", "厕奴", "插比", "插b", "插逼", "插你", "插我", "插阴", "潮吹", "潮喷",
				"成人dv", "成人电影", "成人论坛", "成人小说", "成人电", "成人电影", "成人卡通", "成人聊", "成人片", "成人视", "成人图", "成人文", "成人小", "成人电影",
				"成人论坛", "成人色情", "成人网站", "成人文学", "成人小说", "艳情小说", "成人游戏", "吃精", "赤裸", "抽插", "扌由插", "抽一插", "春药", "大波",
				"大力抽送", "大乳", "荡妇", "荡女", "盗撮", "多人轮", "发浪", "放尿", "肥逼", "粉穴", "风月大陆", "干死你", "干穴", "肛交", "肛门", "龟头",
				"国产av", "豪乳", "黑逼", "后庭", "后穴", "虎骑", "花花公子", "换妻俱乐部", "黄片", "几吧", "鸡吧", "鸡巴", "鸡奸", "寂寞男", "寂寞女", "妓女",
				"激情", "集体淫", "奸情", "叫床", "脚交", "金鳞岂是池中物", "金麟岂是池中物", "精液", "就去日", "巨屌", "菊花洞", "菊门", "巨奶", "巨乳", "菊穴",
				"开苞", "口爆", "口活", "口交", "口射", "口淫", "狂操", "狂插", "浪逼", "浪妇", "浪叫", "浪女", "狼友", "聊性", "流淫", "凌辱", "漏乳",
				"露b", "乱交", "乱伦", "轮暴", "轮操", "轮奸", "裸陪", "买春", "美逼", "美少妇", "美乳", "美腿", "美穴", "美幼", "秘唇", "迷奸", "密穴",
				"蜜穴", "蜜液", "摸奶", "摸胸", "母奸", "奶子", "男奴", "内射", "嫩逼", "嫩女", "嫩穴", "女优", "炮友", "砲友", "喷精", "屁眼", "前凸后翘",
				"强jian", "强暴", "强奸处女", "情色", "拳交", "全裸", "群交", "人妻", "人兽", "日逼", "日烂", "肉棒", "肉逼", "肉唇", "肉洞", "肉缝",
				"肉棍", "肉茎", "肉具", "揉乳", "肉穴", "肉欲", "乳爆", "乳房", "乳沟", "乳交", "乳头", "三级片", "骚逼", "骚比", "骚女", "骚水", "骚穴",
				"色逼", "色界", "色猫", "色盟", "色情网站", "色区", "色诱", "色欲", "色b", "射爽", "射颜", "食精", "释欲", "兽奸", "兽交", "手淫", "兽欲",
				"熟妇", "熟母", "熟女", "爽片", "双臀", "死逼", "丝袜", "丝诱", "酥痒", "套弄", "体奸", "体位", "舔脚", "舔阴", "调教", "偷欢", "偷拍",
				"推油", "我就色", "无码", "吸精", "相奸", "小逼", "校鸡", "小穴", "小xue", "写真", "性感妖娆", "性感诱惑", "性虎", "性饥渴", "性技巧", "性交",
				"性奴", "性虐", "性息", "性欲", "胸推", "穴口", "学生妹", "穴图", "亚情", "颜射", "阳具", "要射了", "夜勤病栋", "一本道", "一夜欢", "一夜情",
				"一ye情", "阴部", "淫虫", "阴唇", "淫荡", "阴道", "淫电影", "阴阜", "淫妇", "淫河", "阴核", "阴户", "淫贱", "淫叫", "淫教师", "阴茎",
				"阴精", "淫浪", "淫媚", "淫糜", "淫魔", "淫母", "淫女", "淫虐", "淫妻", "淫情", "淫色", "淫声浪语", "淫兽学园", "淫书", "淫术炼金士", "淫水",
				"淫娃", "淫威", "淫亵", "淫样", "淫液", "淫照", "阴b", "应召", "幼交", "幼男", "幼女", "欲火", "欲女", "玉女心经", "玉蒲团", "玉乳",
				"欲仙欲死", "玉穴", "援交", "原味内衣", "援助交际", "招鸡", "招妓", "中年美妇", "抓胸", "自拍", "自慰", "作爱", "18禁", "adult",
				"amateur", "a片", "fuck", "gay片", "g点", "g片", "hardcore", "h动画", "h动漫", "porn", "sexinsex", "sm女王",
				"xing伴侣", "tokyohot", "yin荡", "贱人", "装b", "大sb", "傻逼", "傻b", "煞逼", "煞笔", "刹笔", "傻比", "沙比", "欠干", "婊子养的",
				"我日你", "我操", "我草", "卧艹", "卧槽", "爆你菊", "艹你", "cao你", "你他妈", "真他妈", "别他吗", "草你吗", "草你丫", "操你妈", "擦你妈",
				"操你娘", "操他妈", "日你妈", "干你妈", "干你娘", "娘西皮", "狗操", "狗草", "狗杂种", "狗日的", "操你祖宗", "操你全家", "操你大爷", "妈逼", "你麻痹",
				"麻痹的", "妈了个逼", "马勒", "狗娘养", "贱比", "贱b", "下贱", "死全家", "全家死光", "全家不得好死", "全家死绝", "白痴", "无耻", "sb", "杀b",
				"你吗b", "你妈的", "婊子", "贱货", "人渣", "混蛋", "性伴侣", "男公关", "火辣", "精子", "射精", "诱奸", "强奸", "做爱", "性爱", "发生关系",
				"快感", "处男", "猛男", "少妇", "屌", "下体", "a片", "咪咪", "发情", "兽性", "风骚", "呻吟", "sm", "阉割", "高潮", "裸露", "一丝不挂",
				"脱光", "干你", "干死", "我干", "裙中性运动", "乱奸", "乱伦", "乱伦类", "乱伦小", "伦理大", "伦理电影", "伦理毛", "伦理片", "裸聊", "裸聊网",
				"裸体写真", "裸舞视", "裸照", "美女裸体", "美女写真", "美女上门", "美艳少妇", "妹按摩", "妹上门", "迷幻药", "迷幻藥", "迷昏口", "迷昏药", "迷昏藥",
				"迷魂香", "迷魂药", "迷魂藥", "迷奸粉", "迷奸药", "迷情粉", "迷情水", "迷情药", "迷药", "迷藥", "谜奸药", "骚妇", "骚货", "骚浪", "骚女", "骚嘴",
				"色电影", "色妹妹", "色情表演", "色情电影", "色情服务", "色情图片", "色情小说", "色情影片", "色情表演", "色情电影", "色情服务", "色情片", "色视频",
				"色小说", "性伴侣", "性服务", "性福情", "性感少", "性伙伴", "性交", "性交视频", "性交图片", "性奴", "性奴集中营", "性虐", "阴唇", "阴道", "阴蒂",
				"阴户", "阴间来电", "阴茎", "阴茎增大", "阴茎助勃", "阴毛", "陰唇", "陰道", "陰戶", "淫荡", "淫荡美女", "淫荡视频", "淫荡照片", "淫乱", "淫靡",
				"淫魔", "淫魔舞", "淫女", "淫情女", "淫肉", "淫騷妹", "淫兽", "淫兽学", "淫水", "淫穴", "morphine", "摇头丸", "迷药", "乖乖粉", "麻醉药",
				"福音会", "中国教徒", "统一教", "李洪志", "志洪李", "李宏志", "轮功", "法轮", "轮法功", "三去车仑", "氵去车仑", "发论工", "法x功", "法o功",
				"法0功", "法一轮一功", "轮子功", "车仑工力", "法lun", "fa轮", "法lg", "flg", "fl功", "falungong", "大法弟子", "大纪元",
				"dajiyuan", "明慧网", "明慧周报", "正见网", "新唐人", "伪火", "退党", "tuidang", "退dang", "超越红墙", "自fen", "真善忍", "九评",
				"9评", "9ping", "九ping", "jiuping", "藏字石", "集体自杀", "自sha", "zi杀", "suicide", "titor", "逢8必灾", "逢八必灾",
				"逢9必乱", "逢九必乱", "朱瑟里诺", "根达亚文明", "诺查丹玛斯", "人类灭亡进程表", "按照马雅历法", "济世灵文", "诸世纪", "电狗", "电话定位器", "电话拦截器",
				"电话窃听", "电话监", "电话追杀系统", "电击枪", "电鸡", "电警棒", "枪出售", "枪的制", "枪货到", "枪决女犯", "枪模", "枪手", "枪销售", "枪械制",
				"枪子弹", "售步枪", "售纯度", "售单管", "售弹簧刀", "售防身", "售狗子", "售虎头", "售火药", "售假币", "售健卫", "售军用", "售猎枪", "售氯胺",
				"售麻醉", "售枪支", "售热武", "售三棱", "售手枪", "售五四", "售一元硬", "售子弹", "售左轮", "亚砷（酸）酐", "亚砷酸钾", "亚砷酸钠", "亚硒酸",
				"亚硒酸二钠", "亚硒酸镁", "亚硒酸钠", "亚硒酸氢钠", "亚硝酸乙酯", "亚硝酰乙氧", "氧化二丁基锡", "氧化汞", "氧化铊", "氧化亚铊", "氧氯化磷", "原装弹",
				"原子弹方法", "原子弹清单", "安眠酮", "代血浆", "普萘洛尔", "呋塞米", "西布曲明", "testosterone", "胰岛素样生长因子", "促红细胞生成素", "地西泮",
				"尼可刹米", "甲睾酮", "adrenaline", "erythropoietin", "地奈德", "莫达非尼", "氯噻嗪", "苯巴比妥", "促性腺激素", "泼尼松", "麻黄草",
				"雄烯二醇", "地塞米松", "tamoxifen", "strychnine", "androst", "新型毒品", "杜冷丁", "兴奋剂", "mdma", "海洛因", "海luo因",
				"heroin", "diamorphine", "diacetylmorphine", "鸦片", "阿芙蓉", "咖啡因", "cocain", "三唑仑", "美沙酮", "麻古", "k粉",
				"凯他敏", "ketamine", "冰毒", "苯丙胺", "cannabis", "大麻", "爱他死", "氯胺酮", "benzodiazepines", "甲基安非他明", "安非他命",
				"吗啡", "腐败中国", "三个呆婊", "社会主义灭亡", "打倒中国", "打倒共产党", "打倒共产主义", "打倒胡锦涛", "打倒江泽民", "打倒江主席", "打倒李鹏", "打倒罗干",
				"打倒温家宝", "打倒中共", "打倒朱镕", "抵制共产党", "抵制共产主义", "抵制胡锦涛", "抵制江泽民", "抵制江主席", "抵制李鹏", "抵制罗干", "抵制温家宝", "抵制中共",
				"抵制朱镕基", "灭亡中国", "亡党亡国", "粉碎四人帮", "激流中国", "zf大楼", "殃视", "贪污腐败", "强制拆除", "形式主义", "政治风波", "太子党", "上海帮",
				"北京帮", "清华帮", "红色贵族", "权贵集团", "河蟹社会", "喝血社会", "九风", "9风", "十七大", "十7大", "17da", "九学", "9学", "四风", "4风",
				"双规", "最淫官员", "警匪", "官匪", "独夫民贼", "官商勾结", "城管暴力执法", "强制捐款", "毒豺", "一党执政", "一党专制", "一党专政", "专制政权",
				"宪法法院", "胡平", "苏晓康", "贺卫方", "谭作人", "焦国标", "万润南", "张志新", "辛灝年", "高勤荣", "王炳章", "高智晟", "司马璐", "刘晓竹", "刘宾雁",
				"魏京生", "林昭", "别梦成灰", "谁是新中国", "讨伐中宣部", "异议人士", "民运人士", "启蒙派", "选国家主席", "民一主", "min主", "民竹", "民珠", "民猪",
				"chinesedemocracy", "大赦国际", "国际特赦", "da选", "投公", "公头", "宪政", "平反", "党章", "维权", "昝爱宗", "宪章", "08宪",
				"08xz", "抿主", "敏主", "人拳", "人木又", "人quan", "renquan", "中国人权", "中国新民党", "群体事件", "群体性事件", "上中央", "去中央",
				"讨说法", "请愿", "请命", "公开信", "联名上书", "万人大签名", "万人骚动", "截访", "上访", "shangfang", "信访", "访民", "集会", "静坐",
				"静zuo", "jing坐", "示威", "示wei", "游行", "you行", "油行", "游xing", "youxing", "官逼民反", "反party", "反共", "抗议",
				"亢议", "抵制", "低制", "底制", "di制", "抵zhi", "dizhi", "boycott", "血书", "焚烧中国国旗", "baoluan", "流血冲突", "出现暴动",
				"发生暴动", "引起暴动", "baodong", "灭共", "杀毙", "罢工", "霸工", "罢考", "罢餐", "霸餐", "罢参", "罢饭", "罢吃", "罢食", "罢课",
				"罢ke", "霸课", "ba课", "罢教", "罢学", "罢运", "网特", "网评员", "网络评论员", "五毛党", "五毛们", "5毛党", "戒严", "jieyan", "jie严",
				"戒yan", "8的平方事件", "知道64", "八九年", "贰拾年", "2o年", "20和谐年", "贰拾周年", "六四", "六河蟹四", "六百度四", "六和谐四", "陆四",
				"陆肆", "5月35", "89年春夏之交", "64惨案", "64时期", "64运动", "4事件", "四事件", "北京风波", "学潮", "学chao", "xuechao", "学百度潮",
				"门安天", "天按门", "坦克压大学生", "民主女神", "历史的伤口", "高自联", "北高联", "血洗京城", "四二六社论", "王丹", "柴玲", "沈彤", "封从德", "王超华",
				"王维林", "吾尔开希", "吾尔开西", "侯德健", "阎明复", "方励之", "蒋捷连", "丁子霖", "辛灏年", "蒋彦永", "严家其", "陈一咨", "中华局域网", "党的喉舌",
				"互联网审查", "当局严密封锁", "新闻封锁", "封锁消息", "爱国者同盟", "关闭所有论坛", "网络封锁", "金盾工程", "gfw", "无界浏览", "无界网络", "自由门",
				"何清涟", "中国的陷阱", "汪兆钧", "记者无疆界", "境外媒体", "维基百科", "纽约时报", "bbc中文网", "华盛顿邮报", "世界日报", "东森新闻网", "东森电视",
				"星岛日报", "wikipedia", "youtube", "googleblogger", "美国广播公司", "英国金融时报", "自由亚洲", "自由时报", "中国时报", "反分裂",
				"威胁论", "左翼联盟", "钓鱼岛", "保钓组织", "主权", "弓单", "火乍", "木仓", "石肖", "核蛋", "步qiang", "bao炸", "爆zha", "baozha",
				"zha药", "zha弹", "炸dan", "炸yao", "zhadan", "zhayao", "hmtd", "三硝基甲苯", "六氟化铀", "炸药配方", "弹药配方", "炸弹配方",
				"皮箱炸弹", "火药配方", "人体炸弹", "人肉炸弹", "解放军", "兵力部署", "军转", "军事社", "8341部队", "第21集团军", "七大军区", "7大军区", "北京军区",
				"沈阳军区", "济南军区", "成都军区", "广州军区", "南京军区", "兰州军区", "颜色革命", "规模冲突", "塔利班", "基地组织", "恐怖分子", "恐怖份子", "三股势力",
				"印尼屠华", "印尼事件", "蒋公纪念歌", "马英九", "mayingjiu", "李天羽", "苏贞昌", "林文漪", "陈水扁", "陈s扁", "陈随便", "阿扁", "a扁",
				"告全国同胞书", "台百度湾", "台完", "台wan", "taiwan", "台弯", "湾台", "台湾国", "台湾共和国", "台军", "台独", "台毒", "台du", "taidu",
				"twdl", "一中一台", "打台湾", "两岸战争", "攻占台湾", "支持台湾", "进攻台湾", "占领台湾", "统一台湾", "收复台湾", "登陆台湾", "解放台湾", "解放tw",
				"解决台湾", "光复民国", "台湾独立", "台湾问题", "台海问题", "台海危机", "台海统一", "台海大战", "台海战争", "台海局势", "入联", "入耳关", "中华联邦",
				"国民党", "x民党", "民进党", "青天白日", "闹独立", "duli", "fenlie", "日本万岁", "小泽一郎", "劣等民族", "汉人", "汉维", "维汉", "维吾",
				"吾尔", "热比娅", "伊力哈木", "疆独", "东突厥斯坦解放组织", "东突解放组织", "蒙古分裂分子", "列确", "阿旺晋美", "藏人", "臧人", "zang人", "藏民",
				"藏m", "达赖", "赖达", "dalai", "哒赖", "dl喇嘛", "丹增嘉措", "打砸抢", "西独", "藏独", "葬独", "臧独", "藏毒", "藏du", "zangdu",
				"支持zd", "藏暴乱", "藏青会", "雪山狮子旗", "拉萨", "啦萨", "啦沙", "啦撒", "拉sa", "lasa", "la萨", "西藏", "藏西", "藏春阁", "藏獨",
				"藏独", "藏独立", "藏妇会", "藏青会", "藏字石", "xizang", "xi藏", "x藏", "西z", "tibet", "希葬", "希藏", "硒藏", "稀藏", "西脏",
				"西奘", "西葬", "西臧", "援藏", "bjork", "王千源", "安拉", "回教", "回族", "回回", "回民", "穆斯林", "穆罕穆德", "穆罕默德", "默罕默德",
				"伊斯兰", "圣战组织", "清真", "清zhen", "qingzhen", "真主", "阿拉伯", "韩国狗", "满洲第三帝国", "满狗", "鞑子", "江丑闻", "江嫡系", "江毒",
				"江独裁", "江蛤蟆", "江核心", "江黑心", "江胡内斗", "江祸心", "江家帮", "江绵恒", "江派和胡派", "江派人马", "江泉集团", "江人马", "江三条腿", "江氏集团",
				"江氏家族", "江氏政治局", "江氏政治委员", "江梳头", "江太上", "江戏子", "江系人", "江系人马", "江宰民", "江贼", "江贼民", "江主席", "麻果丸", "麻将透",
				"麻醉弹", "麻醉狗", "麻醉枪", "麻醉槍", "麻醉藥", "台独", "台湾版假币", "台湾独立", "台湾国", "台湾应该独立", "台湾有权独立", "天灭中共", "中共帮凶",
				"中共保命", "中共裁", "中共党文化", "中共腐败", "中共的血旗", "中共的罪恶", "中共帝国", "中共独裁", "中共封锁", "中共封网", "中共腐败", "中共黑", "中共黑帮",
				"中共解体", "中共近期权力斗争", "中共恐惧", "中共权力斗争", "中共任用", "中共退党", "中共洗脑", "中共邪教", "中共邪毒素", "中共政治游戏", "共c党", "共x党",
				"共铲", "供产", "共惨", "供铲党", "供铲谠", "供铲裆", "共残党", "共残主义", "共产主义的幽灵", "拱铲", "老共", "中珙", "中gong", "gc党", "贡挡",
				"gong党", "g产", "狗产蛋", "共残裆", "恶党", "邪党", "共产专制", "共产王朝", "裆中央", "土共", "土g", "共狗", "g匪", "共匪", "仇共",
				"共产党腐败", "共产党专制", "共产党的报应", "共产党的末日", "共产党专制", "communistparty", "症腐", "政腐", "政付", "正府", "政俯", "政f",
				"zhengfu", "政zhi", "挡中央", "档中央", "中国zf", "中央zf", "国wu院", "中华帝国", "gong和", "大陆官方", "北京政权", "刘志军", "张曙",
				"气狗", "钢笔狗", "长秃" };
		List resultList2 = Arrays.asList(arrays);
		WordsSearchEx2 sensitiveWords = new WordsSearchEx2();
		sensitiveWords.SetKeywords(resultList2);

		boolean b = sensitiveWords.ContainsAny(word);
		if (b == false) {
			System.out.println("test_issues_90 is Error.");
		}
	}

}
