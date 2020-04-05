package toolgood.words.internals;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import toolgood.words.WordsSearch;
import toolgood.words.WordsSearchResult;

public class PinYinDict {
    private static Map<String, Integer[]> _pyName;
    private static String[] _pyShow;
    private static Integer[] _pyIndex;
    private static Integer[] _pyData;
    private static Integer[] _wordPy;
    private static WordsSearch _search;

    public static String[] getPyShow() throws NumberFormatException, IOException {
        InitPyIndex();
        return _pyShow;
    }

    public static String[] GetPinYinList(String text, int tone) throws NumberFormatException, IOException {
        InitPyIndex();
        InitPyWords();

        String[] list = new String[text.length()];
        List<WordsSearchResult> pos = _search.FindAll(text);
        Integer pindex = -1;

        for (WordsSearchResult p : pos) {
            if (p.Start > pindex) {
                for (int i = 0; i < p.Keyword.length(); i++) {
                    list[i + p.Start] = _pyShow[_wordPy[i + p.Index] + tone];
                }
                pindex = p.End;
            }
        } 

        for (int i = 0; i < text.length(); i++) {
            if (list[i] != null)
                continue;
            Character c = text.charAt(i);
            if (c >= 0x3400 && c <= 0x9fd5) {
                int index = c - 0x3400;
                int start = _pyIndex[index];
                int end = _pyIndex[index + 1];
                if (end > start) {
                    list[i] = _pyShow[_pyData[start] + tone];
                }
            }
        }
        return list;
    }

    public static String GetPinYin(String text, int tone) throws NumberFormatException, IOException {
        InitPyIndex();

        String[] list = GetPinYinList(text, tone);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < list.length; i++) {
            String s = list[i];
            if (s != null) {
                sb.append(list[i]);
            } else {
                sb.append(text.charAt(i));
            }
        }
        return sb.toString();
    }

    public static String GetFirstPinYin(String text, int tone) throws NumberFormatException, IOException {
        InitPyIndex();

        String[] list = GetPinYinList(text, tone);
        StringBuilder sb = new StringBuilder(text);
        for (int i = 0; i < list.length; i++) {
            String c = list[i];
            if (c != null) {
                sb.setCharAt(i, c.charAt(0));
            }
        }
        return sb.toString();
    }

    public static List<String> GetAllPinYin(Character c, int tone) throws NumberFormatException, IOException {
        InitPyIndex();
        if (c >= 0x3400 && c <= 0x9fd5) {
            int index = c - 0x3400;
            List<String> list = new ArrayList<String>();
            int start = _pyIndex[index];
            int end = _pyIndex[index + 1];
            if (end > start) {
                for (int i = start; i < end; i++) {
                    String py = _pyShow[_pyData[i] + tone];
                    if (list.contains(py) == false) {
                        list.add(py);
                    }
                }
            }
            return list;
        }
        return new ArrayList<String>();
    }

    public static String GetPinYinFast(Character c, int tone) throws NumberFormatException, IOException {
        InitPyIndex();

        if (c >= 0x3400 && c <= 0x9fd5) {
            int index = c - 0x3400;
            int start = _pyIndex[index];
            int end = _pyIndex[index + 1];
            if (end > start) {
                return _pyShow[_pyData[start] + tone];
            }
        }
        return c.toString();
    }

    public static List<String> GetPinYinForName(String name, int tone  ) throws NumberFormatException, IOException
    {
        InitPyName();
        InitPyIndex();

        List<String> list = new ArrayList<String>();
        String xing;
        String ming;
        Integer[] indexs;
        if (name.length() > 1) { // 检查复姓
            xing = name.substring(0, 2);
            if(_pyName.containsKey(xing)){
                indexs=_pyName.get(xing);
                for (Integer index : indexs) {
                    list.add(_pyShow[index + tone]);
                }
                if (name.length() > 2) {
                    ming = name.substring(2);
                    String[] pys=GetPinYinList(ming, tone);
                    for (String py : pys) {
                        list.add(py);
                    }
                } 
                return list;
            }
        }
        xing = name.substring(0, 1);
        if(_pyName.containsKey(xing)){
            indexs=_pyName.get(xing);
            for (Integer index : indexs) {
                list.add(_pyShow[index + tone]);
            }
            if (name.length() > 1) {
                ming = name.substring(1);
                String[] pys=GetPinYinList(ming, tone);
                for (String py : pys) {
                    list.add(py);
                }
            } 
            return list;
        }
        String[] pys=GetPinYinList(name, tone);
        for (String py : pys) {
            list.add(py);
        }
        return list;
    }

    private static void InitPyIndex() throws NumberFormatException, IOException {
        if (_pyIndex == null) {
            String resourceName = "pyIndex.txt";
            InputStream u1 = WordsSearch.class.getClassLoader().getResourceAsStream(resourceName);
            BufferedReader br = new BufferedReader(new InputStreamReader(u1));

            String tStr = "";
            List<Integer> pyIndex = new ArrayList<Integer>();
            pyIndex.add(0);
            List<Integer> pyData = new ArrayList<Integer>();

            while ((tStr = br.readLine()) != null) {
                if (_pyShow == null) {
                    String[] ss = tStr.split(",");
                    _pyShow = ss;
                } else {
                    if (tStr != "0") {
                        for (String idx : tStr.split(",")) {
                            int in = Integer.valueOf(idx, 16);
                            pyData.add(in);
                        }
                    }
                    pyIndex.add((int) pyData.size());
                }
            }
            br.close();

            Integer[] pd = new Integer[pyData.size()];
            _pyData = pyData.toArray(pd);
            Integer[] pi = new Integer[pyIndex.size()];
            _pyIndex = pyIndex.toArray(pi);
        }
    }

    private static void InitPyName() throws NumberFormatException, IOException
    {
        if (_pyName == null) {
            String resourceName = "pyName.txt";
            InputStream u1 = WordsSearch.class.getClassLoader().getResourceAsStream(resourceName);
            BufferedReader br = new BufferedReader(new InputStreamReader(u1));

            Map<String, Integer[]> pyName = new HashMap<String, Integer[]>();
            String tStr = "";
            while ((tStr = br.readLine()) != null) {
                String[] sp = tStr.split(",");
                List<Integer> index = new ArrayList<Integer>();
                for (int i = 1; i < sp.length; i++) {
                    String idx = sp[i];
                    int in = Integer.valueOf(idx, 16);
                    index.add(in);
                }
                Integer[] temp=new  Integer[index.size()];
                pyName.put(sp[0], index.toArray(temp));
            }
            br.close();
 
            _pyName = pyName;
        }
    }

    private static void InitPyWords() throws NumberFormatException, IOException
    {
        if (_search == null) {
            String resourceName = "pyWords.txt";
            InputStream u1 = WordsSearch.class.getClassLoader().getResourceAsStream(resourceName);
            BufferedReader br = new BufferedReader(new InputStreamReader(u1));

            Map<String, Integer> keywords = new HashMap<String, Integer>();
            List<Integer> wordPy = new ArrayList<Integer>();

            String tStr = "";
            while ((tStr = br.readLine()) != null) {
                String[] sp = tStr.split(",");
                keywords.put(sp[0], wordPy.size());
                for (int i = 1; i < sp.length; i++) {
                    String idx = sp[i];
                    int in = Integer.valueOf(idx, 16);
                    wordPy.add(in);
                }
            }
            br.close();
            WordsSearch search = new WordsSearch();
            search.SetKeywords(keywords);
            Integer[] wp=new Integer[wordPy.size()];
            _wordPy = wordPy.toArray(wp);
            _search = search;
        }

    }

}