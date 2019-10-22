package ToolGood.Words.internals;

import static java.util.stream.Collectors.toMap;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Hashtable;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public abstract class BaseSearchEx{
    protected String[] _keywords;
    protected int[][] _guides;
    protected int[] _key;
    protected int[] _next;
    protected int[] _check;
    protected int[] _dict;

    public void Save(String filePath) throws IOException
    {
        File fi = new File(filePath);
        FileOutputStream fs = new FileOutputStream(fi);
        Save(fs);
        fs.close();
    }

    protected void Save(FileOutputStream bw) throws IOException
    {
        bw.write(_keywords.length);
        for (String item : _keywords) {
            byte[] bytes=item.getBytes();
            bw.write(bytes.length);  
            bw.write(bytes);
        }

        bw.write(_guides.length);
        for (int[] guide : _guides) {
            bw.write(guide.length);
            for (int item : guide) {
                bw.write(item);
            }
        }

        bw.write(_key.length);
        for (int item : _key) { bw.write(item); }
   
        bw.write(_next.length);
        for (int item : _next) { bw.write(item); }
 
        bw.write(_check.length);
        for (int item : _check) { bw.write(item); }

         bw.write(_dict.length);
        for (int item : _dict) { bw.write(item); }
    }
 
    public void Load(String filePath) throws FileNotFoundException,IOException
    {
        File fi = new File(filePath);
        FileInputStream fs = new FileInputStream(fi);
        Load(fs);
        fs.close();
    }

    protected void Load(FileInputStream br) throws IOException
    {
        int length =br.read();
        _keywords=new String[length];
        for (int i = 0; i < length; i++) {
            int l=br.read();
            byte[] bytes=new byte[l];
            br.read(bytes,0,l);
            _keywords[i]=new String(bytes);
        }
        length =br.read();
        _guides=new int[length][];
        for (int i = 0; i < length; i++) {
            int length2 = br.read();
            _guides[i]=new int[length2];
            for (int j = 0; j < length2; j++) {
                _guides[i][j]=br.read();
            }
        }

        length =br.read();
        _key=new int[length];
        for (int i = 0; i < length; i++) {
            _key[i]=br.read();
        }
 
        length =br.read();
        _next=new int[length];
        for (int i = 0; i < length; i++) {
            _next[i]=br.read();
        }

        length =br.read();
        _check=new int[length];
        for (int i = 0; i < length; i++) {
            _check[i]=br.read();
        }

        length =br.read();
        _dict=new int[length];
        for (int i = 0; i < length; i++) {
            _dict[i]=br.read();
        }
    }
 



    public void SetKeywords(List<String> keywords)
    {
        _keywords = keywords.toArray(new String[0]);
        int length = CreateDict(keywords);
        TrieNodeEx root = new TrieNodeEx();

        for (int i = 0; i < _keywords.length; i++) {
            String p = _keywords[i];
            TrieNodeEx nd = root;
            for (int j = 0; j < p.length(); j++) {
                nd = nd.Add((char)_dict[p.charAt(j)]);
            }
            nd.SetResults(i);
        }

        List<TrieNodeEx> nodes = new ArrayList<TrieNodeEx>();
        for (Character key : root.m_values.keySet()) {
            TrieNodeEx nd=root.m_values.get(key);
            nd.Failure = root;
            for (TrieNodeEx trans : nd.m_values.values()) {
                nodes.add(trans);
            }
        }
        while(nodes.size()!=0){
            List<TrieNodeEx> newNodes = new ArrayList<TrieNodeEx>();
            for (TrieNodeEx nd : nodes) {
                TrieNodeEx r = nd.Parent.Failure;
                int c =  (int)nd.Char;
                while (r != null && !r.m_values.containsKey( c)){r = r.Failure;} 
                if (r == null){
                    nd.Failure = root;
                }else {
                    nd.Failure = r.m_values.get(c);
                    for (Integer result : nd.Failure.Results) {
                        nd.SetResults(result);
                    }
                }
                
                for (TrieNodeEx child : nd.m_values.values()) {
                     newNodes.add(child);
                }
            }
            nodes = newNodes;
        }
        root.Failure = root;
        for (TrieNodeEx item : root.m_values.values()) {
            TryLinks(item);
        }
        build(root, length);
    }

    private void TryLinks(TrieNodeEx node)
    {
        node.Merge(node.Failure);
        for (TrieNodeEx item : node.m_values.values()) {
            TryLinks(item);
        }
    }

    private void build(TrieNodeEx root, int length)
    {
        TrieNodeEx[] has = new TrieNodeEx[0x00FFFFFF];
        length = root.Rank(has) + length + 1;
        _key = new int[length];
        _next = new int[length];
        _check = new int[length];
        List<Integer[]> guides = new ArrayList<Integer[]>();
        guides.add(new Integer[] { 0 });
        for (int i = 0; i < length; i++) {
            TrieNodeEx item = has[i];
            if (item == null) continue;
            _key[i] = item.Char;
            _next[i] = item.Next;
            if (item.End) {
                _check[i] = guides.size();
                Integer[] result= item.Results.toArray(new Integer[0]);
                guides.add(result);
            }
        }
        _guides=new int[guides.size()][];
        for (int i = 0; i < guides.size(); i++) {
            Integer[] array= guides.get(i);
            _guides[i]=new int[array.length];
            for (int j = 0; j < array.length; j++) {
                _guides[i][j]=array[j];
            }
        }
    }

    
    private int CreateDict(List<String> keywords)
    {
        Map<Character, Integer> dictionary = new Hashtable<Character, Integer>();

        for (String keyword : keywords) {
            for (int i = 0; i < keyword.length(); i++) {
                Character item = keyword.charAt(i);
                if (dictionary.containsKey(item)) {
                    if (i > 0)
                    dictionary.put(item, dictionary.get(item)+2);
                } else {
                    dictionary.put(item, i > 0 ? 2 : 1);
                }
            }
        }
        Map<Character, Integer> dictionary2 = dictionary
            .entrySet()
            .stream()
            .sorted(Collections.reverseOrder(Map.Entry.comparingByValue()))
            .collect(
                toMap(Map.Entry::getKey, Map.Entry::getValue, (e1, e2) -> e2,
                    LinkedHashMap::new));
 

        List<Character> list2 = new ArrayList<Character>();
        boolean sh = false;
        for (Character item : dictionary2.keySet()) {
            if (sh) {
                list2.add(item);
            } else {
                list2.add(0, item);
            }
            sh = !sh;
        }

        _dict = new int[Character.MAX_VALUE + 1];
        for (int i = 0; i < list2.size(); i++) {
            _dict[list2.get(i)] = i + 1;
        }
        return dictionary.size();
    }
}