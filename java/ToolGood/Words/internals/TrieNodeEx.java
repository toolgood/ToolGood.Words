package ToolGood.Words.internals;

import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;

public class TrieNodeEx{
    public TrieNodeEx Parent;
    public TrieNodeEx Failure;
    public Character Char;
    public boolean End;
    public List<Integer> Results;
    public Map<Character, TrieNodeEx> m_values;
    public Map<Character, TrieNodeEx> merge_values;
    private Integer minflag = Integer.MAX_VALUE;
    private Integer maxflag = 0;
    public Integer Next;
    private Integer Count;


    public TrieNodeEx()
    {
        m_values = new Hashtable<Character, TrieNodeEx>();
        merge_values = new Hashtable<Character, TrieNodeEx>();
        Results = new ArrayList<Integer>();
    }

    public boolean TryGetValue(Character c,   TrieNodeEx node)
    {
        if (minflag <=  c && maxflag >=  c) {
            if( m_values.containsKey(c)){
                node=m_values.get(c);
                return true;
            }
            return false;           }
        node = null;
        return false;
    }

    public TrieNodeEx Add(Character c)
    {
        if( m_values.containsKey(c)){
            return m_values.get(c);
        }
        if (minflag > c) { minflag = (int)c; }
        if (maxflag < c) { maxflag = (int)c; }

        TrieNodeEx node = new TrieNodeEx();
        node.Parent = this;
        node.Char = c;
        m_values.put(c, node);
        Count++;
        return node;
    }

    public void SetResults(int text)
    {
        if (End == false) {
            End = true;
        }
        if (Results.contains(text) == false) {
            Results.add(text);
        }
    }

    public void Merge(TrieNodeEx node)
    {
        TrieNodeEx nd = node;
        while ((int)nd.Char !=  0) {
            node.m_values.forEach((key,value)->{
                if(m_values.containsKey(key)==false){
                    if(merge_values.containsKey(key)==false){
                        if (minflag > key) { minflag = (int)key; }
                        if (maxflag < key) { maxflag = (int)key; }
                        merge_values.put(key, value);
                        Count++;
                    }
                }
            });
            nd = nd.Failure;
        }
    }

    public int Rank(TrieNodeEx[] has)
    {
        boolean[] seats = new boolean[has.length];
        Integer  start = 1;

        has[0] = this;

        Rank(start, seats, has);
        int maxCount = has.length - 1;
        while (has[maxCount] == null) { maxCount--; }
        return maxCount;
    }

    private void Rank(Integer  start, boolean[] seats, TrieNodeEx[] has)
    {
        if (maxflag == 0) return;

        List<Integer> keys=new ArrayList<Integer>();
        m_values.forEach((k,v)->{ keys.add((int)k); });   
        merge_values.forEach((k,v)->{ keys.add((int)k); });

        while (has[start] != null) { start++; }

        int s = start < (int)minflag ? (int)minflag : start;

        for (int i = s; i < has.length; i++) {
            if (has[i] == null) {
                Integer next = i - (int)minflag;
                //if (next < 0) continue;
                if (seats[next]) continue;

                boolean isok = true;
                for (Integer item : keys) {
                    if (has[next + item] != null) { isok = false; break; }
                }
   
                if (isok) {
                    SetSeats(next, seats, has);
                    break;
                }
            }
        }
        start += keys.size() / 2;

        //var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);


        for (Character key : m_values.keySet()) {
            TrieNodeEx value=m_values.get(key);
            value.Rank(start, seats, has);
        }
    }


    private void SetSeats(Integer next, boolean[] seats, TrieNodeEx[] has)
    {
        Next = next;
        seats[next] = true;

        merge_values.forEach((key,value)->{
            int position = next + key;
            has[position] = value;
        });

         m_values.forEach((key,value)->{
            int position = next + key;
            has[position] = value;
        });
    }



}