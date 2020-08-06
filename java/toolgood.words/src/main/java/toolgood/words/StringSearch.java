package toolgood.words;

import java.util.ArrayList;
import java.util.List;

import toolgood.words.internals.TrieNode2;
import toolgood.words.internals.BaseSearch;

public class StringSearch extends BaseSearch {
    /**
     * 在文本中查找第一个关键字
     * 
     * @param text 文本
     * @return
     */
    public String FindFirst(final String text) {
        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            final char t = text.charAt(i);
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
                    return _keywords[tn.Results.get(0)];
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
    public List<String> FindAll(final String text) {
        TrieNode2 ptr = null;
        final List<String> list = new ArrayList<String>();

        for (int i = 0; i < text.length(); i++) {
            final char t = text.charAt(i);
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
                    tn.Results.forEach(item -> {
                        list.add(_keywords[item]);
                    });
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
    public boolean ContainsAny(final String text) {
        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            final char t = text.charAt(i);
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
    public String Replace(final String text) {
        return Replace(text, '*');
    }

    /**
     * 在文本中替换所有的关键字
     * 
     * @param text        文本
     * @param replaceChar 替换符
     * @return
     */
    public String Replace(final String text, final char replaceChar) {
        final StringBuilder result = new StringBuilder(text);

        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            final char t = text.charAt(i);
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
                    final int maxLength = _keywords[tn.Results.get(0)].length();
                    final int start = i + 1 - maxLength;
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
