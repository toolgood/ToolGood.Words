package ToolGood.Words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class TrieNode{
    public boolean End=false;
    public List<String> Results;
    public Map<Character, TrieNode> m_values;
    private int minflag = Integer.MAX_VALUE;
    private int maxflag = Integer.MIN_VALUE;

    public TrieNode()
    {
        m_values = new HashMap<Character, TrieNode>();
        Results = new ArrayList<String>();
    }

    public boolean TryGetValue(Character c, TrieNode node)
    {
        if (minflag <= (int)c && maxflag >= (int)c) {
            if( m_values.containsKey(c)){
                node=m_values.get(c);
                return true;
            }
            return false;
        }
        node = null;
        return false;
    }
    public List<TrieNode> Transitions()
    {
        List<TrieNode> list =new ArrayList<TrieNode>();
        m_values.forEach((k,v)->{
            list.add(v);
        });
        return list;
    }

    public TrieNode Add(char c)
    {
        if( m_values.containsKey(c)){
            return m_values.get(c);
        }

        if (minflag > c) { minflag = c; }
        if (maxflag < c) { maxflag = c; }
        TrieNode node  = new TrieNode();
        m_values.put(c, node);
        return node;
    }

    public void SetResults(String text)
    {
        if (End == false) {
            End = true;
        }
        if (Results.contains(text)==false) {
            Results.add(text);
        }
    }


    public void Merge(TrieNode node, Map<TrieNode, TrieNode> links)
    {
        if (node.End==true) {
            if (End == false) {
                End = true;
            }
            node.Results.forEach((item)->{
                if (Results.contains(item) == false) {
                    Results.add(item);
                }
            });
        }
        node.m_values.forEach((Key,Value)->{
            if ( m_values.containsKey(Key) == false) {
                if (minflag > Key) { minflag = Key; }
                if (maxflag < Key) { maxflag = Key; }
                m_values.put(Key, Value);
            }
        });
        if(links.containsKey(node)){
            TrieNode node2= links.get(node);
            Merge(node2, links);
        }
    }

    public TrieNode[] ToArray()
    {
        TrieNode[] first = new TrieNode[Character.MAX_VALUE + 1];
        m_values.forEach((k,v)->{
            first[k]=v;
        });
        return first;
    }


}
