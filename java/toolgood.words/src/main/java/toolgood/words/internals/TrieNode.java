package toolgood.words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public final class TrieNode implements Comparable<TrieNode> {

    public int Index;
    public int Layer;
    public char Char;
    public List<Integer> Results;
    public HashMap<Character, TrieNode> m_values;
    public TrieNode Failure;
    public TrieNode Parent;
    public boolean IsWildcard;
    public int WildcardLayer;
    public boolean HasWildcard;


    public TrieNode() {
        // m_values = new HashMap<Character, TrieNode>();
        // Results = new ArrayList<Integer>();
    }
    public boolean End(){
        return Results!=null;
    }

    public TrieNode Add(final Character c) {
        if(m_values==null){
            m_values = new HashMap<Character, TrieNode>();
        }
        if (m_values.containsKey(c)) {
            return m_values.get(c);
        }
        final TrieNode node = new TrieNode();
        node.Parent = this;
        node.Char = c;
        m_values.put(c, node);
        return node;
    }

    public void SetResults(final Integer index) {
        if(Results==null){
            Results = new ArrayList<Integer>();
        }
        if (Results.contains(index) == false) {
            Results.add(index);
        }
    }

    @Override
    public int compareTo(final TrieNode o) {
        return this.Layer - o.Layer  ;
    }
    /**
     * 伪释放
     */
    public void Dispose()
    {
        if (Results!=null) {
            Results.clear();
            Results = null;
        }
        if (m_values!=null) {
            m_values.clear();
            m_values = null;
        }
        Failure =null;
        Parent = null;
    }
}
