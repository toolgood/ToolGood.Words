package toolgood.words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class TrieNode3 {
    public boolean End;
    public boolean HasWildcard;
    public List<Integer> Results;
    public HashMap<Character, TrieNode3> m_values;
    private int minflag = Integer.MAX_VALUE;
    private int maxflag = 0;
    public TrieNode3 WildcardNode;


    public TrieNode3()
    {
        Results = new ArrayList<Integer>();
        m_values = new HashMap<Character, TrieNode3>();
    }

    public void Add(final char c, final TrieNode3 node3) {
        if (minflag > c) {
            minflag = c;
        }
        if (maxflag < c) {
            maxflag = c;
        }
        m_values.put(c, node3);
    }

    public void SetResults(final int index) {
        if (End == false) {
            End = true;
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

    public TrieNode3 GetValue(final char c) {
        return m_values.get(c);
    }
}