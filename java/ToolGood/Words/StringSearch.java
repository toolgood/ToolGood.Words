package ToolGood.Words;

import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;

import org.springframework.util.StringUtils;

import ToolGood.Words.internals.TrieNode;

public class StringSearch {
    protected TrieNode[] _first = new TrieNode[Character.MAX_VALUE + 1];


    public void SetKeywords(List<String> _keywords)
    {
        TrieNode[] first =  new TrieNode[Character.MAX_VALUE + 1];
        TrieNode root = new TrieNode();

        _keywords.forEach(p->{
            if(StringUtils.isEmpty(p)==false){ 
                TrieNode nd = _first[ p.charAt(0)];
                if (nd == null) {
                    nd = root.Add(p.charAt(0));
                    first[p.charAt(0)] = nd;
                }
                for (int i = 1; i < p.length(); i++) {
                    nd = nd.Add(p.charAt(i));
                }
                nd.SetResults(p);
            }
        });
 
        this._first = first;// root.ToArray();

        Map<TrieNode, TrieNode> links = new Hashtable<TrieNode, TrieNode>();
        root.m_values.forEach((k,v)->{
            TryLinks(v, null, links);
        });
        links.forEach((k,v)->{
            k.Merge(v, links);
        });
    }

    private void TryLinks(TrieNode node, TrieNode node2, Map<TrieNode, TrieNode> links)
    {
        node.m_values.forEach((Key,Value)->{
            TrieNode tn = null;
            if (node2 == null) {
                tn = _first[Key];
                if (tn != null) {
                    links.put(Value, tn);
                }
            } else if (node2.TryGetValue(Key, tn)) {
                links.put(Value, tn);
            }
            TryLinks(Value, tn, links);
        });
    }


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
                }else {
                    tn=ptr.GetValue(t);//TODO: 不太清楚 java out参数 怎么做？ 
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
                }else {
                    tn=ptr.GetValue(t);//TODO: 不太清楚 java out参数 怎么做？ 
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
                } else {
                    tn=ptr.GetValue(t);//TODO: 不太清楚 java out参数 怎么做？ 
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
                }else {
                    tn=ptr.GetValue(t);//TODO: 不太清楚 java out参数 怎么做？ 
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
