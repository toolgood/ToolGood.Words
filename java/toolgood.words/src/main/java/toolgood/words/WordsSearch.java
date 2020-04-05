package toolgood.words;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;

import toolgood.words.internals.TrieNode;
import toolgood.words.internals.TrieNode2;

public class WordsSearch{
    protected TrieNode2[] _first = new TrieNode2[Character.MAX_VALUE + 1];
    String[] _keywords;
    public  String[] _others;
    int[] _indexs;


    /**
     * 设置关键字
     * @param keywords
     */
    public void SetKeywords(List<String> keywords)
    {
        _keywords= keywords.toArray(_keywords);
        _indexs=new int[_keywords.length];
        for (int i = 0; i < keywords.size(); i++) {
            _indexs[i]=i;
        }
        SetKeywords();
    }
    /**
     * 设置关键字
     * @param _keywords
     */
    public void SetKeywords(Map<String,Integer> keywords)
    {
        int size = keywords.size();
        Integer i=0;
        _keywords=new String[size];
        _indexs=new int[size];

        for (String p : keywords.keySet()) {
            _keywords[i]=p;
            _indexs[i]=keywords.get(p);
            i++;
        }
        SetKeywords();
    }

    private void SetKeywords()
    {
        TrieNode root = new TrieNode();

        List<TrieNode> allNode = new ArrayList<TrieNode>();
        allNode.add(root);

        for (int i = 0; i < _keywords.length; i++) {
            String p = _keywords[i];
            TrieNode nd = root;
            for (int j = 0; j < p.length(); j++) {
                nd = nd.Add(p.charAt(j) );
                if (nd.Layer == 0) {
                    nd.Layer = j + 1;
                    allNode.add(nd);
                }
            }
            nd.SetResults(i);
        }

        List<TrieNode> nodes = new ArrayList<TrieNode>();
        for (Character key : root.m_values.keySet()) {
            TrieNode nd = root.m_values.get(key);
            nd.Failure = root;
            for (TrieNode trans : nd.m_values.values()) {
                nodes.add(trans);
            }
        }
        while (nodes.size() != 0) {
            List<TrieNode> newNodes = new ArrayList<TrieNode>();
            for (TrieNode nd : nodes) {
                TrieNode r = nd.Parent.Failure;
                Character c = nd.Char;
                while (r != null && !r.m_values.containsKey(c)) {
                    r = r.Failure;
                }
                if (r == null) {
                    nd.Failure = root;
                } else {
                    nd.Failure = r.m_values.get(c);
                    for (Integer result : nd.Failure.Results) {
                        nd.SetResults(result);
                    }
                }

                for (TrieNode child : nd.m_values.values()) {
                    newNodes.add(child);
                }
            }
            nodes = newNodes;
        }
        root.Failure = root;

        Collections.sort(allNode);
        for (int i = 0; i < allNode.size(); i++) { allNode.get(i).Index = i; }
        

        List<TrieNode2> allNode2 = new ArrayList<TrieNode2>();
        for (int i = 0; i < allNode.size(); i++) {
            allNode2.add(new TrieNode2());
        }
        for (int i = 0; i < allNode2.size(); i++) {
            TrieNode oldNode = allNode.get(i);
            TrieNode2 newNode = allNode2.get(i);

            for (Character key : oldNode.m_values.keySet()) {
                TrieNode nd = oldNode.m_values.get(key);
                newNode.Add(key,allNode2.get(nd.Index) );
            }
            oldNode.Results.forEach(item->{
                newNode.SetResults(item);
            });

            if (oldNode.Failure != root) {
                for (Character key : oldNode.Failure.m_values.keySet()) {
                    TrieNode nd = oldNode.Failure.m_values.get(key);
                    newNode.Add(key,allNode2.get(nd.Index) );
                }
                oldNode.Failure.Results.forEach(item->{
                    newNode.SetResults(item);
                });
            }
        }
        allNode.clear();
        allNode = null;
        root = null;

        TrieNode2[] first = new TrieNode2[Character.MAX_VALUE + 1];
        TrieNode2 root2=allNode2.get(0);
        for (Character key : root2.m_values.keySet()) {
            TrieNode2 nd = root2.m_values.get(key);
            first[(int)key] = nd;
        }
        _first = first;
    }


    /**
     * 在文本中查找第一个关键字
     * @param text 文本
     * @return
     */
    public WordsSearchResult FindFirst(String text)
    {
        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode2 tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                }else {
                    tn=ptr.GetValue(t); 
                }
            }
            if (tn != null) {
                if (tn.End) {
                    for(Integer index : tn.Results){
                        String key=_keywords[index] ;
                        return new WordsSearchResult(key, i + 1 - key.length(), i, _indexs[index]);
                    }
                }
            }
            ptr = tn;
        }
        return null;
    }
    /**
     * 在文本中查找所有的关键字
     * @param text 文本
     * @return
     */
    public List<WordsSearchResult> FindAll(String text)
    {
        TrieNode2 ptr = null;
        List<WordsSearchResult> list = new ArrayList<WordsSearchResult>();

        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode2 tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                }else {
                    tn=ptr.GetValue(t);  
                }
            }
            if (tn != null) {
                if (tn.End) {
                    for( Integer  index : tn.Results){
                        String key= _keywords[index];
                        WordsSearchResult item= new WordsSearchResult(key, i + 1 - key.length(), i, _indexs[index]);
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
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(String text)
    {
        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode2 tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                }else {
                    tn=ptr.GetValue(t); 
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
    public String Replace(String text,Character replaceChar)
    {
        StringBuilder result = new StringBuilder(text);

        TrieNode2 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t=text.charAt(i);
            TrieNode2 tn=null;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    tn = _first[t];
                }else {
                    tn=ptr.GetValue(t); 
                }
            }
            if (tn != null) {
                if (tn.End) {
                    int maxLength =_keywords[tn.Results.get(0)].length();
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