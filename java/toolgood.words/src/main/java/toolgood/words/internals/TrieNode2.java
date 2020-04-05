package toolgood.words.internals;

import java.util.List;
import java.util.ArrayList;
import java.util.HashMap;


public class TrieNode2{
    public boolean End;
    public List<Integer> Results;
    public HashMap<Character, TrieNode2> m_values;
    private int minflag = Integer.MAX_VALUE;
    private int maxflag = Integer.MIN_VALUE;

    public TrieNode2()
    {
        Results = new ArrayList<Integer>();
        m_values = new HashMap<Character, TrieNode2>();
    }

    public void Add(char c, TrieNode2 node3)
    {
        if (minflag > c) { minflag = c; }
        if (maxflag < c) { maxflag = c; }
        m_values.put(c, node3);
    }

    public void SetResults(Integer index)
    {
        if (End == false) {
            End = true;
        }
        if (Results.contains(index) == false) {
            Results.add(index);
        }
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
}