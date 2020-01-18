package toolgood.words;

import toolgood.words.internals.BaseSearchEx;

import java.util.ArrayList;
import java.util.List;


public class StringSearchEx extends BaseSearchEx {

    /**
     * 在文本中查找所有的关键字
     * @param text 文本
     * @return
     */
    public List<String> FindAll(String text)
    {
        List<String> root = new ArrayList<String>();
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
                next = _next[0] + t;
                find = _key[next] == t;
            }
            if (find) {
                int index = _check[next];
                if (index > 0) {
                    for (int item : _guides[index]) {
                        root.add(_keywords[item]);
                    }
                }
                p = next;
            }
        }
        return root;
    }
    /**
     * 在文本中查找第一个关键字
     * @param text 文本
     * @return
     */
    public String FindFirst(String text)
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
                int index = _check[next];
                if (index > 0) {
                    return _keywords[_guides[index][0]];
                }
                p = next;
            } else {
                p = 0;
                next = _next[p] + t;
                if (_key[next] == t) {
                    int index = _check[next];
                    if (index > 0) {
                        return _keywords[_guides[index][0]];
                    }
                    p = next;
                }
            }
        }
        return null;
    }

    /**
     * 判断文本是否包含关键字
     * @param text 文本
     */
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
    /**
     * 在文本中替换所有的关键字, 替换符默认为 *
     * @param text 文本
     * @return
     */
    public String Replace(String text){
        return Replace(text,'*');
    }
    /**
     * 在文本中替换所有的关键字
     * @param text 文本
     * @param replaceChar 替换符
     * @return
     */
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