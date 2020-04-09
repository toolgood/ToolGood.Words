package toolgood.words.internals;

import toolgood.words.NumHelper;

import static java.util.stream.Collectors.toMap;

import java.io.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Hashtable;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public abstract class BaseSearchEx {
    protected String[] _keywords;
    protected int[][] _guides;
    protected int[] _key;
    protected int[] _next;
    protected int[] _check;
    protected int[] _dict;

    /**
     * 保存
     *
     * @param filePath 文件地址
     * @throws IOException
     */
    public void Save(String filePath) throws IOException {
        File fi = new File(filePath);
        FileOutputStream fs = new FileOutputStream(fi);
        Save(fs);
        fs.close();
    }

    protected void Save(FileOutputStream bw) throws IOException {
        bw.write(NumHelper.serialize(_keywords.length));
        for (String item : _keywords) {
            byte[] bytes = item.getBytes();
            bw.write(NumHelper.serialize(bytes.length));
            bw.write(bytes);
        }

        bw.write(NumHelper.serialize(_guides.length));
        for (int[] guide : _guides) {
            bw.write(NumHelper.serialize(guide.length));
            for (int item : guide) {
                bw.write(NumHelper.serialize(item));
            }
        }

        bw.write(NumHelper.serialize(_key.length));
        for (int item : _key) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(NumHelper.serialize(_next.length));
        for (int item : _next) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(NumHelper.serialize(_check.length));
        for (int item : _check) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(NumHelper.serialize(_dict.length));
        for (int item : _dict) {
            bw.write(NumHelper.serialize(item));
        }
    }

    /**
     * 加载
     *
     * @param filePath
     * @throws FileNotFoundException
     * @throws IOException
     */
    public void Load(String filePath) throws FileNotFoundException, IOException {
        File fi = new File(filePath);
        InputStream in = new BufferedInputStream(new FileInputStream(fi));
        Load(in);
        in.close();
    }

    public void Load(InputStream br) throws IOException {
        int length = NumHelper.read(br);
        _keywords = new String[length];
        for (int i = 0; i < length; i++) {
            int l = NumHelper.read(br);
            byte[] bytes = new byte[l];
            br.read(bytes, 0, l);
            _keywords[i] = new String(bytes);
        }
        length = NumHelper.read(br);
        _guides = new int[length][];
        for (int i = 0; i < length; i++) {
            int length2 = NumHelper.read(br);
            _guides[i] = new int[length2];
            for (int j = 0; j < length2; j++) {
                _guides[i][j] = NumHelper.read(br);
            }
        }

        length = NumHelper.read(br);
        _key = new int[length];
        for (int i = 0; i < length; i++) {
            _key[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _next = new int[length];
        for (int i = 0; i < length; i++) {
            _next[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _check = new int[length];
        for (int i = 0; i < length; i++) {
            _check[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _dict = new int[length];
        for (int i = 0; i < length; i++) {
            _dict[i] = NumHelper.read(br);
        }
    }

    /**
     * 设置关键字
     *
     * @param keywords
     */
    public void SetKeywords(List<String> keywords) {
        _keywords = keywords.toArray(new String[0]);

        SetKeywords();
    }

    private void SetKeywords() {
        TrieNode root = new TrieNode();
        Map<Integer,List<TrieNode>> allNodeLayers=new Hashtable<Integer,List<TrieNode>>();
        for (int i = 0; i < _keywords.length; i++) {
            String p = _keywords[i];
            TrieNode nd = root;
            for (int j = 0; j < p.length(); j++) {
                nd = nd.Add(p.charAt(j));
                if (nd.Layer == 0) {
                    nd.Layer = j + 1;
                    if(allNodeLayers.containsKey(nd.Layer)==false){
                        List<TrieNode> nodes=new ArrayList<TrieNode>();
                        nodes.add(nd);
                        allNodeLayers.put(nd.Layer, nodes);
                    }else {
                        allNodeLayers.get(nd.Layer).add(nd);
                    }                }
            }
            nd.SetResults(i);
        }

        List<TrieNode> allNode = new ArrayList<TrieNode>();
        allNode.add(root);
        for (int i = 0; i < allNodeLayers.size(); i++) { //注意 这里不能用 keySet()
            List<TrieNode> nodes = allNodeLayers.get(i+1);
            for (int j = 0; j < nodes.size(); j++) {
                allNode.add(nodes.get(j));
            }
        }
        allNodeLayers.clear();
        allNodeLayers=null;

        
        for (int i = 1; i < allNode.size(); i++) {
            TrieNode nd = allNode.get(i);
            nd.Index = i;
            TrieNode r = nd.Parent.Failure;
            Character c = nd.Char;
            while (r != null && !r.m_values.containsKey(c)) r = r.Failure;
            if (r == null)
                nd.Failure = root;
            else {
                nd.Failure = r.m_values.get(c);
                for (Integer result : nd.Failure.Results) {
                    nd.SetResults(result);
                }
            }
        }
        root.Failure = root;
 

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 1; i < allNode.size(); i++) {
            stringBuilder.append(allNode.get(i).Char);
        }
        Integer length = CreateDict(stringBuilder.toString());
        stringBuilder = null;

        List<TrieNodeEx> allNode2 = new ArrayList<TrieNodeEx>();
        for (int i = 0; i < allNode.size(); i++) {
            TrieNodeEx nd = new TrieNodeEx();
            nd.Index = i;
            allNode2.add(nd);
        }
        for (int i = 0; i < allNode2.size(); i++) {
            TrieNode oldNode = allNode.get(i);
            TrieNodeEx newNode = allNode2.get(i);
            newNode.Char = _dict[oldNode.Char];

            for (Character key : oldNode.m_values.keySet()) {
                TrieNode nd = oldNode.m_values.get(key);
                newNode.Add(_dict[key], allNode2.get(nd.Index));
            }
            oldNode.Results.forEach(item -> {
                newNode.SetResults(item);
            });
            oldNode = oldNode.Failure;
            while (oldNode != root) {
                for (Character key : oldNode.m_values.keySet()) {
                    if (newNode.HasKey(_dict[key]) == false) {
                        TrieNode nd = oldNode.m_values.get(key);
                        newNode.Add(_dict[key], allNode2.get(nd.Index));
                    }
                }
                oldNode.Results.forEach(item -> {
                    newNode.SetResults(item);
                });
                oldNode = oldNode.Failure;
            }
        }
        allNode.clear();
        allNode = null;
        root = null;

        build(allNode2, length);
    }

    private void build(List<TrieNodeEx> nodes, int length) {
        Integer[] has = new Integer[0x00FFFFFF];
        boolean[] seats = new boolean[0x00FFFFFF];
        boolean[] seats2 = new boolean[0x00FFFFFF];
        Integer start = 1;
        Integer oneStart = 1;
        for (int i = 0; i < nodes.size(); i++) {
            TrieNodeEx node = nodes.get(i);
            node.Rank(oneStart, start, seats, seats2, has);
        }
        Integer maxCount = has.length - 1;
        while (has[maxCount] == null) {
            maxCount--;
        }
        length = maxCount + length + 1;

        // length = root.Rank(has) + length + 1;
        _key = new int[length];
        _next = new int[length];
        _check = new int[length];
        List<Integer[]> guides = new ArrayList<Integer[]>();
        guides.add(new Integer[] { 0 });
        for (int i = 0; i < length; i++) {
            if (has[i] == null)
                continue;
            TrieNodeEx item = nodes.get(has[i]);
            _key[i] = item.Char;
            _next[i] = item.Next;
            if (item.End) {
                _check[i] = guides.size();
                Integer[] result = item.Results.toArray(new Integer[0]);
                guides.add(result);
            }
        }
        _guides = new int[guides.size()][];
        for (int i = 0; i < guides.size(); i++) {
            Integer[] array = guides.get(i);
            _guides[i] = new int[array.length];
            for (int j = 0; j < array.length; j++) {
                _guides[i][j] = array[j];
            }
        }

    }

    private int CreateDict(String keywords) {
        Map<Character, Integer> dictionary = new Hashtable<Character, Integer>();
        for (int i = 0; i < keywords.length(); i++) {
            Character item = keywords.charAt(i);
            if (dictionary.containsKey(item)) {
                dictionary.put(item, dictionary.get(item) + 1);
            } else {
                dictionary.put(item, 1);
            }
        }
        Map<Character, Integer> dictionary2 = dictionary.entrySet().stream()
                .sorted(Collections.reverseOrder(Map.Entry.comparingByValue()))
                .collect(toMap(Map.Entry::getKey, Map.Entry::getValue, (e1, e2) -> e2, LinkedHashMap::new));

        List<Character> list2 = new ArrayList<Character>();
        for (Character item : dictionary2.keySet()) {
            list2.add(item);
        }

        _dict = new int[Character.MAX_VALUE + 1];
        for (int i = 0; i < list2.size(); i++) {
            _dict[list2.get(i)] = i + 1;
        }
        return dictionary.size();
    }

}