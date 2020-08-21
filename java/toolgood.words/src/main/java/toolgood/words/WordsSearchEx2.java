package toolgood.words;

import toolgood.words.internals.BaseSearchEx2;

import java.util.ArrayList;
import java.util.List;


public class WordsSearchEx2 extends BaseSearchEx2 {
    /**
     * 在文本中查找所有的关键字
     * @param text 文本
     * @return
     */
    public List<WordsSearchResult> FindAll(final String text) {
        final List<WordsSearchResult> root = new ArrayList<WordsSearchResult>();
        int p = 0;
        final int length = text.length();
        for (int i = 0; i < length; i++) {
            final int t = _dict[text.charAt(i)];
            if (t == 0) {
                p = 0;
                continue;
            }
            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false && p != 0) {
                p = 0;
                next = _next[0] + t;
                find = _key[next] == t;
            }
            if (find) {
                final int index = _check[next];
                if (index > 0) {
                    for (final int item : _guides[index]) {
                        final String key = _keywords[item];
                        final WordsSearchResult r = new WordsSearchResult(key, i + 1 - key.length(), i, item);
                        root.add(r);
                    }
                }
                p = next;
            }
        }
        return root;
    }

    /**
     * 在文本中查找第一个关键字
     * 
     * @param text 文本
     * @return
     */
    public WordsSearchResult FindFirst(final String text) {
        int p = 0;
        final int length = text.length();
        for (int i = 0; i < length; i++) {
            final int t = _dict[text.charAt(i)];
            if (t == 0) {
                p = 0;
                continue;
            }
            int next = _next[p] + t;
            if (_key[next] == t) {
                final int index = _check[next];
                if (index > 0) {
                    final String item = _keywords[_guides[index][0]];
                    return new WordsSearchResult(item, i + 1 - item.length(), i, _guides[index][0]);
                }
                p = next;
            } else {
                p = 0;
                next = _next[p] + t;
                if (_key[next] == t) {
                    final int index = _check[next];
                    if (index > 0) {
                        final String item = _keywords[_guides[index][0]];
                        return new WordsSearchResult(item, i + 1 - item.length(), i, _guides[index][0]);
                    }
                    p = next;
                }
            }
        }
        return null;
    }

    /**
     * 判断文本是否包含关键字
     * 
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(final String text) {
        int p = 0;
        for (int i = 0; i < text.length(); i++) {
            final int t = _dict[text.charAt(i)];
            if (t == 0) {
                p = 0;
                continue;
            }
            int next = _next[p] + t;
            if (_key[next] == t) {
                if (_check[next] > 0) {
                    return true;
                }
                p = next;
            } else {
                p = 0;
                next = _next[p] + t;
                if (_key[next] == t) {
                    if (_check[next] > 0) {
                        return true;
                    }
                    p = next;
                }
            }
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

        int p = 0;

        for (int i = 0; i < text.length(); i++) {
            final int t = _dict[text.charAt(i)];
            if (t == 0) {
                p = 0;
                continue;
            }
            int next = _next[p] + t;
            boolean find = _key[next] == t;
            if (find == false && p != 0) {
                p = 0;
                next = _next[p] + t;
                find = _key[next] == t;
            }
            if (find) {
                final int index = _check[next];
                if (index > 0) {
                    final int maxLength = _keywords[_guides[index][0]].length();
                    final int start = i + 1 - maxLength;
                    for (int j = start; j <= i; j++) {
                        result.setCharAt(j, replaceChar);
                     }
                 }
                 p = next;
            }
        }
        return result.toString();
    }

}