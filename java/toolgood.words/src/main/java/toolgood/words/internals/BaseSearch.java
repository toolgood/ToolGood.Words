package toolgood.words.internals;

import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;

public class BaseSearch {
    protected TrieNode2[] _first = new TrieNode2[Character.MAX_VALUE + 1];
    protected String[] _keywords;


    /**
     * 设置关键字
     * 
     * @param keywords 关键字列表
     */
    public void SetKeywords(List<String> keywords) {
        _keywords = new String[keywords.size()];
        _keywords = keywords.toArray(_keywords);
        SetKeywords();
    }

    protected void SetKeywords() {
        TrieNode root = new TrieNode();
        Map<Integer, List<TrieNode>> allNodeLayers = new Hashtable<Integer, List<TrieNode>>();
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
            while (r != null && !r.m_values.containsKey(c))
                r = r.Failure;
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

        List<TrieNode2> allNode2 = new ArrayList<TrieNode2>();
        for (int i = 0; i < allNode.size(); i++) {
            allNode2.add(new TrieNode2());
        }
        for (int i = 0; i < allNode2.size(); i++) {
            TrieNode oldNode = allNode.get(i);
            TrieNode2 newNode = allNode2.get(i);

            for (Character key : oldNode.m_values.keySet()) {
                TrieNode nd = oldNode.m_values.get(key);
                newNode.Add(key, allNode2.get(nd.Index));
            }
            oldNode.Results.forEach(item -> {
                newNode.SetResults(item);
            });

            oldNode = oldNode.Failure;
            while (oldNode != root) {
                for (Character key : oldNode.m_values.keySet()) {
                    TrieNode nd = oldNode.m_values.get(key);
                    if (newNode.HasKey(key) == false) {
                        newNode.Add(key, allNode2.get(nd.Index));
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

        TrieNode2[] first = new TrieNode2[Character.MAX_VALUE + 1];
        TrieNode2 root2 = allNode2.get(0);
        for (Character key : root2.m_values.keySet()) {
            TrieNode2 nd = root2.m_values.get(key);
            first[(int) key] = nd;
        }
        _first = first;
    }


}