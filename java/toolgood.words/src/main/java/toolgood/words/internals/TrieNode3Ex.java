package toolgood.words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public final class TrieNode3Ex {
    public int Index;
    public boolean HasWildcard;
    public List<Integer> Results;
    public HashMap<Character, TrieNode3Ex> m_values;
    public int minflag = Integer.MAX_VALUE;
    public int maxflag = 0;
    public TrieNode3Ex WildcardNode;


    public TrieNode3Ex()
    {
        // Results = new ArrayList<Integer>();
        // m_values = new HashMap<Character, TrieNode3Ex>();
    }
    public boolean End(){
        return Results!=null;
    }

    public void Add(final char c, final TrieNode3Ex node3) {
        if (minflag > c) {
            minflag = c;
        }
        if (maxflag < c) {
            maxflag = c;
        }
        if(m_values==null){
            m_values = new HashMap<Character, TrieNode3Ex>();
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

    public boolean HasKey(final char c) {
        if (minflag <= c && maxflag >= c) {
            return m_values.containsKey(c);
        }
        return false;
    }

    public TrieNode3Ex GetValue(final char c) {
        return m_values.get(c);
    }
}