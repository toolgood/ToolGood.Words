package ToolGood.Words.internals;

import java.util.Hashtable;
import java.util.List;
import java.util.Map;

import org.springframework.util.StringUtils;

public abstract class BaseSearch {
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

}