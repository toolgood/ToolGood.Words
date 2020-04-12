package toolgood.words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class TrieNode3Ex {
    public int Index;
    public boolean End;
    public boolean HasWildcard;
    public List<Integer> Results;
    public HashMap<Character, TrieNode3Ex> m_values;
    private int minflag = Integer.MAX_VALUE;
    private int maxflag = Integer.MIN_VALUE;
    public TrieNode3 WildcardNode;


    public TrieNode3Ex()
    {
        Results = new ArrayList<Integer>();
        m_values = new HashMap<Character, TrieNode3Ex>();
    }

    public void Add(char c, TrieNode3Ex node3)
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
    public TrieNode3Ex GetValue(Character c ){
        return m_values.get(c);
    }
}