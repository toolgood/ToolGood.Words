package toolgood.words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public final class TrieNode2Ex {
    public int Index;
    public List<Integer> Results;
    public HashMap<Integer, TrieNode2Ex> m_values;
    public int minflag = Integer.MAX_VALUE;
    public int maxflag = 0;

    public TrieNode2Ex()
    {
        // Results = new ArrayList<Integer>();
        // m_values = new HashMap<Integer, TrieNode2Ex>();
    }
    public boolean End(){
        return Results!=null;
    }

    public void Add(final int c, final TrieNode2Ex node3) {
        if (minflag > c) {
            minflag = c;
        }
        if (maxflag < c) {
            maxflag = c;
        }
        if(m_values==null){
            m_values = new HashMap<Integer, TrieNode2Ex>();
        }
        m_values.put(c, node3);
    }

    public void SetResults(final int index) {
        if(Results==null){
            Results = new ArrayList<Integer>();
        }
        if (Results.contains(index) == false) {
            Results.add(index);
        }
    }

    public boolean HasKey(final int c) {
        if (minflag <= c && maxflag >= c) {
            return m_values.containsKey(c);
        }
        return false;
    }

    public TrieNode2Ex GetValue(final int c) {
        return m_values.get(c);
    }

}