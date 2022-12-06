package toolgood.words.internals;

import static java.util.stream.Collectors.toMap;

import java.io.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Hashtable;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import toolgood.words.NumHelper;

public class BaseSearchEx3 {
    protected int[] _dict;
    protected int[] _first;

    protected IntDictionary[] _nextIndex;
    protected int[] _end;
    protected int[] _resultIndex;
    protected String[] _keywords;

    /**
     * 保存, 修改于2020-08-06，使用utf-8保存，与以前数据可能会不同
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
            byte[] bytes = item.getBytes("utf-8");
            bw.write(NumHelper.serialize(bytes.length));
            bw.write(bytes);
        }

        bw.write(NumHelper.serialize(_dict.length));
        for (int item : _dict) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(NumHelper.serialize(_first.length));
        for (int item : _first) {
            bw.write(NumHelper.serialize(item));
        }
        bw.write(NumHelper.serialize(_end.length));
        for (int item : _end) {
            bw.write(NumHelper.serialize(item));
        }
        bw.write(NumHelper.serialize(_resultIndex.length));
        for (int item : _resultIndex) {
            bw.write(NumHelper.serialize(item));
        }

        bw.write(NumHelper.serialize(_nextIndex.length));
        for (int i = 0; i < _nextIndex.length; i++) {
            int[] keys = _nextIndex[i].getKeys();
            bw.write(NumHelper.serialize(keys.length));
            for (int item : keys) {
                bw.write(NumHelper.serialize(item));
            }

            int[] values = _nextIndex[i].getValues();
            bw.write(NumHelper.serialize(values.length));
            for (int item : values) {
                bw.write(NumHelper.serialize(item));
            }
        }
    }

    /**
     * 加载, 修改于2020-08-06，使用utf-8加载，加载以前数据可能会出错
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
            _keywords[i] = new String(bytes, "utf-8");
        }

        length = NumHelper.read(br);
        _dict = new int[length];
        for (int i = 0; i < length; i++) {
            _dict[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _first = new int[length];
        for (int i = 0; i < length; i++) {
            _first[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _end = new int[length];
        for (int i = 0; i < length; i++) {
            _end[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _resultIndex = new int[length];
        for (int i = 0; i < length; i++) {
            _resultIndex[i] = NumHelper.read(br);
        }

        length = NumHelper.read(br);
        _nextIndex = new IntDictionary[length];
        for (int i = 0; i < length; i++) {
            int l2 = NumHelper.read(br);
            int[] keys = new int[l2];
            for (int j = 0; j < keys.length; j++) {
                keys[j] = NumHelper.read(br);
            }

            l2 = NumHelper.read(br);
            int[] values = new int[l2];
            for (int j = 0; j < values.length; j++) {
                values[j] = NumHelper.read(br);
            }
            _nextIndex[i] = new IntDictionary();
            _nextIndex[i].SetDictionary(keys, values);
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
        Map<Integer, List<TrieNode>> allNodeLayers = new TreeMap<Integer, List<TrieNode>>();
        for (int i = 0; i < _keywords.length; i++) {
            String p = _keywords[i];
            TrieNode nd = root;
            for (int j = 0; j < p.length(); j++) {
                nd = nd.Add(p.charAt(j));
                if (nd.Layer == 0) {
                    nd.Layer = j + 1;
                    if (allNodeLayers.containsKey(nd.Layer) == false) {
                        List<TrieNode> nodes = new ArrayList<TrieNode>();
                        nodes.add(nd);
                        allNodeLayers.put(nd.Layer, nodes);
                    } else {
                        allNodeLayers.get(nd.Layer).add(nd);
                    }
                }
            }
            nd.SetResults(i);
        }

        List<TrieNode> allNode = new ArrayList<TrieNode>();
        allNode.add(root);
        for (int i = 0; i < allNodeLayers.size(); i++) { // 注意 这里不能用 keySet()
            List<TrieNode> nodes = allNodeLayers.get(i + 1);
            for (int j = 0; j < nodes.size(); j++) {
                allNode.add(nodes.get(j));
            }
        }
        allNodeLayers.clear();
        allNodeLayers = null;

        for (int i = 1; i < allNode.size(); i++) {
            TrieNode nd = allNode.get(i);
            nd.Index = i;
            TrieNode r = nd.Parent.Failure;
            Character c = nd.Char;
            while (r != null && (r.m_values == null || !r.m_values.containsKey(c)))
                r = r.Failure;
            if (r == null)
                nd.Failure = root;
            else {
                nd.Failure = r.m_values.get(c);
                if (nd.Failure.Results != null) {
                    for (Integer result : nd.Failure.Results) {
                        nd.SetResults(result);
                    }
                }
            }
        }
        root.Failure = root;

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 1; i < allNode.size(); i++) {
            stringBuilder.append(allNode.get(i).Char);
        }
        CreateDict(stringBuilder.toString());
        stringBuilder = null;

        int[] first = new int[Character.MAX_VALUE + 1];
        for (Character key : allNode.get(0).m_values.keySet()) {
            TrieNode nd = allNode.get(0).m_values.get(key);
            first[_dict[key]] = nd.Index;
        }
        _first = first;

        List<Integer> resultIndex2 = new ArrayList<>();
        List<Boolean> isEndStart = new ArrayList<>();
        IntDictionary[] _nextIndex2 = new IntDictionary[allNode.size()];

        for (int i = allNode.size() - 1; i >= 0; i--) {
            Map<Integer, Integer> dict = new TreeMap<Integer, Integer>();
            List<Integer> result = new ArrayList<>();
            TrieNode oldNode = allNode.get(i);
            if (oldNode.m_values != null) {
                for (Character key : oldNode.m_values.keySet()) {
                    TrieNode nd = oldNode.m_values.get(key);
                    dict.put(_dict[key], nd.Index);
                }
            }
            if (oldNode.Results != null) {
                oldNode.Results.forEach(item -> {
                    if (result.contains(item) == false) {
                        result.add(item);
                    }
                });
            }

            oldNode = oldNode.Failure;
            while (oldNode != root) {
                if (oldNode.m_values != null) {
                    for (Character key : oldNode.m_values.keySet()) {
                        TrieNode nd = oldNode.m_values.get(key);
                        if (dict.containsKey(_dict[key]) == false) {
                            dict.put(_dict[key], nd.Index);
                        }
                    }
                }
                if (oldNode.Results != null) {
                    oldNode.Results.forEach(item -> {
                        if (result.contains(item) == false) {
                            result.add(item);
                        }
                    });
                }
                oldNode = oldNode.Failure;
            }
            _nextIndex2[i] = new IntDictionary();
            _nextIndex2[i].SetDictionary(dict);

            if (result.size() > 0) {
                for (int j = result.size() - 1; j >= 0; j--) {
                    resultIndex2.add(result.get(j));
                    isEndStart.add(false);
                }
                isEndStart.set(isEndStart.size() - 1, true);
            } else {
                resultIndex2.add(-1);
                isEndStart.add(true);
            }
            dict = null;
            allNode.get(i).Dispose();
        }
        allNode.clear();
        allNode = null;
        root = null;
        _nextIndex = _nextIndex2;

        List<Integer> resultIndex = new ArrayList<>();
        List<Integer> end = new ArrayList<>();
        for (int i = isEndStart.size() - 1; i >= 0; i--) {
            if (isEndStart.get(i)) {
                end.add(resultIndex.size());
            }
            if (resultIndex2.get(i) > -1) {
                resultIndex.add(resultIndex2.get(i));
            }
        }
        end.add(resultIndex.size());
        _resultIndex = new int[resultIndex.size()];
        for (int i = 0; i < resultIndex.size(); i++) {
            _resultIndex[i] = (int) (resultIndex.get(i));
        }

        _end = new int[end.size()];
        for (int i = 0; i < end.size(); i++) {
            _end[i] = (int) (end.get(i));
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