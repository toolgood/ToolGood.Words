package ToolGood.Words;

import java.util.ArrayList;
import java.util.List;

import ToolGood.Words.internals.BaseSearchEx;

public class WordsSearchEx extends BaseSearchEx{

    public List<WordsSearchResult> FindAll(String text)
    {
        List<WordsSearchResult> root = new ArrayList<WordsSearchResult>();
        int p = 0;
        int length = text.length();
        for (int i = 0; i < length; i++) {
            int t =  _dict[text.charAt(i)];
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
                int index = _check[next];
                if (index > 0) {
                    for (int item : _guides[index]) {
                        String key = _keywords[item];
                        WordsSearchResult r = new WordsSearchResult(key, i + 1 - key.length(), i, item);
                        root.add(r);
                    }
                }
                p = next;
            }
        }
        return root;
    }

 
    public WordsSearchResult FindFirst(String text)
    {
        int p = 0;
        int length = text.length();
        for (int i = 0; i < length; i++) {
            int t =  _dict[text.charAt(i)];
            if (t == 0) {
                p = 0;
                continue;
            }
            int next = _next[p] + t;
            if (_key[next] == t) {
                int index = _check[next];
                if (index > 0) {
                    String item = _keywords[_guides[index][0]];
                    return new WordsSearchResult(item, i + 1 - item.length(), i, _guides[index][0]);
                }
                p = next;
            } else {
                p = 0;
                next = _next[p] + t;
                if (_key[next] == t) {
                    int index = _check[next];
                    if (index > 0) {
                        String item = _keywords[_guides[index][0]];
                        return new WordsSearchResult(item, i + 1 - item.length(),i, _guides[index][0]);
                    }
                    p = next;
                }
            }
        }
        return null;
    }

    public boolean ContainsAny(String text)
    {
        int p = 0;
        for (int i = 0; i < text.length(); i++) {
            int t =  _dict[text.charAt(i)];
            if (t == 0) {
                p = 0;
                continue;
            }
            int next = _next[p] + t;
            if (_key[next] == t) {
                if (_check[next] > 0) { return true; }
                p = next;
            } else {
                p = 0;
                next = _next[p] + t;
                if (_key[next] == t) {
                    if (_check[next] > 0) { return true; }
                    p = next;
                }
            }
        }
        return false;
    }

    public String Replace(String text){
        return Replace(text,'*');
    }

    public String Replace(String text, Character replaceChar)
    {
        StringBuilder result = new StringBuilder(text);

        int p = 0;

        for (int i = 0; i < text.length(); i++) {
            int t =  _dict[text.charAt(i)];
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
                int index = _check[next];
                if (index > 0) {
                    int maxLength = _keywords[_guides[index][0]].length();
                    int start = i + 1 - maxLength;
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