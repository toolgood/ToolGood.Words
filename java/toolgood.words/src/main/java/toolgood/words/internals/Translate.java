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

public class Translate {
    private static WordsSearch s2tSearch;
    private static WordsSearch t2sSearch;
    private static WordsSearch t2twSearch;
    private static WordsSearch tw2tSearch;
    private static WordsSearch t2hkSearch;
    private static WordsSearch hk2tSearch;

    /**
     * 转繁体中文
     * 
     * @param text
     * @param type 0、繁体中文，1、港澳繁体，2、台湾正体
     * @return
     * @throws Exception
     */
    public static String ToTraditionalChinese(String text, final int type) throws Exception {
        if (type > 2 || type < 0) {
            throw new Exception("type 不支持该类型");
        }

        final WordsSearch s2t = GetWordsSearch(true, 0);
        text = TransformationReplace(text, s2t);
        if (type > 0) {
            final WordsSearch t2 = GetWordsSearch(true, type);
            text = TransformationReplace(text, t2);
        }
        return text;
    }

    /**
     * 转简体中文
     * 
     * @param text
     * @param srcType 0、繁体中文，1、港澳繁体，2、台湾正体
     * @return
     * @throws Exception
     */
    public static String ToSimplifiedChinese(String text, final int srcType) throws Exception {
        if (srcType > 2 || srcType < 0) {
            throw new Exception("srcType 不支持该类型");
        }
        if (srcType > 0) {
            final WordsSearch t2 = GetWordsSearch(false, srcType);
            text = TransformationReplace(text, t2);
        }
        final WordsSearch s2t = GetWordsSearch(false, 0);
        text = TransformationReplace(text, s2t);
        return text;
    }

    /**
     * 清理 简繁转换 缓存
     */
    public static void ClearTranslate() {
        s2tSearch = null;
        t2sSearch = null;
        t2twSearch = null;
        tw2tSearch = null;
        t2hkSearch = null;
        hk2tSearch = null;
    }

    /**
     * 
     * @param text
     * @param wordsSearch
     * @return
     */
    private static String TransformationReplace(String text, WordsSearch wordsSearch) {
        List<WordsSearchResult> ts = wordsSearch.FindAll(text);
        StringBuilder sb = new StringBuilder();
        int index = 0;
        while (index < text.length()) {
            WordsSearchResult t = null;
            int end = -1;
            for (WordsSearchResult wordsSearchResult : ts) {
                if (wordsSearchResult.Start == index) {
                    if (end < wordsSearchResult.End) {
                        end = wordsSearchResult.End;
                        t = wordsSearchResult;
                    }
                }
            }
            if (t == null) {
                sb.append(text.charAt(index));
                index++;
            } else {
                sb.append(wordsSearch._others[t.Index]);
                index = t.End + 1;
            }
        }
        return sb.toString();
    }

    private static WordsSearch GetWordsSearch(Boolean s2t, int srcType) throws IOException {
        if (s2t) {
            if (srcType == 0) {
                if (s2tSearch == null) {
                    s2tSearch = BuildWordsSearch("s2t.dat", false);
                }
                return s2tSearch;
            } else if (srcType == 1) {
                if (t2hkSearch == null) {
                    t2hkSearch = BuildWordsSearch("t2hk.dat", false);
                }
                return t2hkSearch;
            } else if (srcType == 2) {
                if (t2twSearch == null) {
                    t2twSearch = BuildWordsSearch("t2tw.dat", false);
                }
                return t2twSearch;
            }
        } else {
            if (srcType == 0) {
                if (t2sSearch == null) {
                    t2sSearch = BuildWordsSearch("t2s.dat", false);
                }
                return t2sSearch;
            } else if (srcType == 1) {
                if (hk2tSearch == null) {
                    hk2tSearch = BuildWordsSearch("t2hk.dat", true);
                }
                return hk2tSearch;
            } else if (srcType == 2) {
                if (tw2tSearch == null) {
                    tw2tSearch = BuildWordsSearch("t2tw.dat", true);
                }
                return tw2tSearch;
            }
        }
        return null;
    }

    private static WordsSearch BuildWordsSearch(String fileName, Boolean reverse) throws IOException {
        Map<String, String> dict = GetTransformationDict(fileName);
        List<String> Keys = new ArrayList<String>();
        List<String> Values = new ArrayList<String>();
        dict.forEach((k, v) -> {
            Keys.add(k);
            Values.add(v);
        });
        WordsSearch wordsSearch = new WordsSearch();
        if (reverse) {
            wordsSearch.SetKeywords(Values);
            String[] temp = new String[Keys.size()];
            wordsSearch._others = Keys.toArray(temp);
        } else {
            wordsSearch.SetKeywords(Keys);
            String[] temp = new String[Keys.size()];
            wordsSearch._others = Values.toArray(temp);
        }
        return wordsSearch;
    }

    static Map<String, String> GetTransformationDict(String fileName) throws IOException {
        String resourceName = fileName;
        InputStream u1 = WordsSearch.class.getClassLoader().getResourceAsStream(resourceName);
        BufferedReader br = new BufferedReader(new InputStreamReader(u1));

        String tStr = "";
        Map<String, String> dict = new HashMap<String, String>();
        while ((tStr = br.readLine()) != null) {
            String[] ss = tStr.split("\t");
            if (ss.length < 2) {
                continue;
            }
            dict.put(ss[0], ss[1]);
        }
        br.close();
        return dict;
    }
}