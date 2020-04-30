package toolgood.words;

import java.util.ArrayList;
import java.util.List;

import toolgood.words.internals.BaseSearch;
import toolgood.words.internals.TrieNode2;

public class WordsSearch extends BaseSearch {
    public String[] _others;

    /**
     * 在文本中查找第一个关键字
     * 
     * @param text 文本
     * @return
     */
    public WordsSearchResult FindFirst(String text) {
        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode2 tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    for (Integer index : tn.Results) {
                        String key = _keywords[index];
                        return new WordsSearchResult(key, i + 1 - key.length(), i, index);
                    }
                }
            }
            ptr = tn;
        }
        return null;
    }

    /**
     * 在文本中查找所有的关键字
     * 
     * @param text 文本
     * @return
     */
    public List<WordsSearchResult> FindAll(String text) {
        TrieNode2 ptr = null;
        List<WordsSearchResult> list = new ArrayList<WordsSearchResult>();

        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode2 tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    for (Integer index : tn.Results) {
                        String key = _keywords[index];
                        WordsSearchResult item = new WordsSearchResult(key, i + 1 - key.length(), i, index);
                        list.add(item);
                    }
                }
            }
            ptr = tn;
        }
        return list;
    }

    /**
     * 判断文本是否包含关键字
     * 
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(String text) {
        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode2 tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    return true;
                }
            }
            ptr = tn;
        }
        return false;
    }

    /**
     * 在文本中替换所有的关键字, 替换符默认为 *
     * 
     * @param text 文本
     * @return
     */
    public String Replace(String text) {
        return Replace(text, '*');
    }

    /**
     * 在文本中替换所有的关键字
     * 
     * @param text        文本
     * @param replaceChar 替换符
     * @return
     */
    public String Replace(String text, Character replaceChar) {
        StringBuilder result = new StringBuilder(text);

        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode2 tn = null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    int maxLength = _keywords[tn.Results.get(0)].length();
                    int start = i + 1 - maxLength;
                    for (int j = start; j <= i; j++) {
                        result.setCharAt(j, replaceChar);
                    }
                }
            }
            ptr = tn;
        }
        return result.toString();
    }

}