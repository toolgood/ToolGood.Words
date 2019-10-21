package ToolGood.Words;

import java.util.ArrayList;
import java.util.List;

import ToolGood.Words.internals.BaseSearch;
import ToolGood.Words.internals.TrieNode;

public class StringSearch extends BaseSearch {

    public String FindFirst(String text)
    {
        TrieNode ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.TryGetValue(t, tn) == false) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    return tn.Results.get(0);
                }
            }
            ptr = tn;
        }
        return null;
    }

    public List<String> FindAll(String text)
    {
        TrieNode ptr = null;
        List<String> list = new ArrayList<String>();

        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.TryGetValue(t, tn) == false) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    tn.Results.forEach(item->{
                        list.add(item);
                    });
                }
            }
            ptr = tn;
        }
        return list;
    }

    public boolean ContainsAny(String text)
    {
        TrieNode ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.TryGetValue(t, tn) == false) {
                    tn = _first[t];
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

    public String Replace(String text){
        return Replace(text,'*');
    }

    public String Replace(String text,Character replaceChar)
    {
        StringBuilder result = new StringBuilder(text);

        TrieNode ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.TryGetValue(t,   tn) == false) {
                    tn = _first[t];
                }
            }
            if (tn != null) {
                if (tn.End) {
                    int maxLength = tn.Results.get(0).length();
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
