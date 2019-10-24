package ToolGood.Words.internals;

import java.util.Hashtable;
import java.util.Map;

public class TrieNode2{
    public boolean End=false;
    public Map<String, Integer> Results;
    public Map<Character, TrieNode2> m_values;
    private int minflag = Integer.MAX_VALUE;
    private int maxflag = Integer.MIN_VALUE;


    public TrieNode2()
    {
        m_values = new Hashtable<Character, TrieNode2>();
        Results = new Hashtable<String, Integer>();
    }

    public boolean HasKey(Character c)
    {
        if (minflag <= c && maxflag >= c) {
            return m_values.containsKey(c);
        }
        return false;
    }
    public TrieNode2 GetValue(Character c ){
        return m_values.get(c);
    }

    public TrieNode2 Add(char c)
    {
        if( m_values.containsKey(c)){
            return m_values.get(c);
        }

        if (minflag > c) { minflag = c; }
        if (maxflag < c) { maxflag = c; }
        TrieNode2 node  = new TrieNode2();
        m_values.put(c, node);
        return node;
    }
 

    public void SetResults(String text, int index)
    {
        if (End == false) {
            End = true;
        }
        Results.put(text, index);
    }

    public void Merge(TrieNode2 node, Map<TrieNode2, TrieNode2> links)
    {
        if (node.End==true) {
            if (End == false) {
                End = true;
            }
            node.Results.forEach((key,Value)->{
                if (Results.containsKey(key) == false) {
                    Results.put(key,Value);
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
            TrieNode2 node2= links.get(node);
            Merge(node2, links);
        }
    }

 

}