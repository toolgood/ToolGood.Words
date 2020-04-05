package toolgood.words.internals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class TrieNode implements Comparable<TrieNode> {

    public int Index;
    public int Layer;
    public boolean End;
    public char Char;
    public List<Integer> Results;
    public HashMap<Character, TrieNode> m_values;
    public TrieNode Failure;
    public TrieNode Parent;

    public TrieNode() {
        m_values = new HashMap<Character, TrieNode>();
        Results = new ArrayList<Integer>();
    }

    public TrieNode Add(final Character c) {
        if (m_values.containsKey(c)) {
            return m_values.get(c);
        }
        final TrieNode node = new TrieNode();
        node.Parent = this;
        node.Char = c;
        m_values.put(c, node);
        return node;
    }

    public void SetResults(Integer index) {
        if (End == false) {
            End = true;
        }
        if (Results.contains(index) == false) {
            Results.add(index);
        }
    }

    @Override
    public int compareTo(TrieNode o) {
        return this.Layer - o.Layer  ;
    }
}
