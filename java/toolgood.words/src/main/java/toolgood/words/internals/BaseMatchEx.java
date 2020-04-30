package toolgood.words.internals;

import static java.util.stream.Collectors.toMap;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public class BaseMatchEx extends BaseMatch {
    protected int[] _dict;
    protected int[] _firstIndex;
    protected int[] _min;
    protected int[] _max;

    protected IntDictionary[] _nextIndex;
    protected int[] _wildcard;
    protected int[] _end;
    protected int[] _resultIndex;


    @Override
    protected   void SetKeywords2(List<String> keywords)
    {
        List<TrieNode> allNode = BuildFirstLayerTrieNode(keywords);
        TrieNode root = allNode.get(0);

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 1; i < allNode.size(); i++) {
            stringBuilder.append(allNode.get(i).Char);
        }
        CreateDict(stringBuilder.toString());
        stringBuilder = null;


        List<TrieNode3Ex> allNode2 = new ArrayList<TrieNode3Ex>();
        for (int i = 0; i < allNode.size(); i++) {
            TrieNode3Ex node3=new TrieNode3Ex();
            node3.Index=i;
            allNode2.add(node3); ;
        }

        for (int i = 0; i < allNode2.size(); i++) {
            TrieNode oldNode = allNode.get(i);
            TrieNode3Ex newNode = allNode2.get(i);

            for (Character item : oldNode.m_values.keySet()) {
                int key = _dict[item];
                int index = oldNode.m_values.get(item).Index;
                if (key == 0) {
                    newNode.HasWildcard = true;
                    newNode.WildcardNode = allNode2.get(index) ;
                    continue;
                }
                newNode.Add((char)key, allNode2.get(index) );
            }
            for (int item : oldNode.Results) {
                if (oldNode.IsWildcard) {
                    if (keywords.get(item).length() > oldNode.WildcardLayer) {
                        newNode.SetResults(item);
                    }
                } else {
                    newNode.SetResults(item);
                }
                //newNode.SetResults(item);
            }

            TrieNode failure = oldNode.Failure;
            while (failure != root) {
                if (oldNode.IsWildcard && failure.Layer <= oldNode.WildcardLayer) {
                    break;
                }
                for (Character item : failure.m_values.keySet()) {
                    int key = _dict[item];
                    int index = failure.m_values.get(item).Index;
                    if (key == 0) {
                        newNode.HasWildcard = true;
                        if (newNode.WildcardNode == null) {
                            newNode.WildcardNode = allNode2.get(index);
                        }
                        continue;
                    }
                    if (newNode.HasKey((char)key) == false) {
                        newNode.Add((char)key, allNode2.get(index));
                    }
                }
                for (int item : failure.Results) {
                    if (oldNode.IsWildcard) {
                        if (keywords.get(item).length() > oldNode.WildcardLayer) {
                            newNode.SetResults(item);
                        }
                    } else {
                        newNode.SetResults(item);
                    }
                }
                failure = failure.Failure;
            }
        }
        allNode.clear();
        allNode = null;
        root = null;


        List<Integer> min = new ArrayList<Integer>();
        List<Integer> max = new ArrayList<Integer>();
        List<Integer> wildcard = new ArrayList<Integer>();
        List<Map<Integer, Integer>> nextIndexs = new ArrayList<Map<Integer, Integer>>();
        List<Integer> end = new ArrayList<Integer>() ;
        end.add(0);
        List<Integer> resultIndex = new ArrayList<Integer>();
        for (int i = 0; i < allNode2.size(); i++) {
            Map<Integer, Integer> dict = new HashMap<Integer, Integer>();
            TrieNode3Ex node = allNode2.get(i);
            min.add(node.minflag);
            max.add(node.maxflag);

            if (node.HasWildcard) {
                wildcard.add(node.WildcardNode.Index);
            } else {
                wildcard.add(0);
            }

            if (i > 0) {
                for (Character item : node.m_values.keySet()) {
                    dict.put((Integer)(int)(item) , node.m_values.get(item).Index);
                }
            }
            for (int item : node.Results) {
                resultIndex.add(item);
            }
            end.add(resultIndex.size());
            nextIndexs.add(dict);
        }
        int[] first = new int[Character.MAX_VALUE + 1];
        for (Character item : allNode2.get(0).m_values.keySet()) {
            first[item] = allNode2.get(0).m_values.get(item).Index;
        }

        _firstIndex = first;
        _min = new int[min.size()];
        _max = new int[min.size()];
        for (int i = 0; i < min.size(); i++) {
            _min[i] = (int) (min.get(i));
            _max[i] = (int) (max.get(i));
        }
        _nextIndex = new IntDictionary[nextIndexs.size()];
        for (int i = 0; i < nextIndexs.size(); i++) {
            IntDictionary dictionary = new IntDictionary();
            dictionary.SetDictionary(nextIndexs.get(i));
            _nextIndex[i] = dictionary;
        }
        _wildcard= new int[wildcard.size()];
        for (int i = 0; i < wildcard.size(); i++) {
            _wildcard[i] = (int) (wildcard.get(i));
        }
        _end = new int[end.size()];
        for (int i = 0; i < end.size(); i++) {
            _end[i] = (int) (end.get(i));
        }
        _resultIndex = new int[resultIndex.size()];
        for (int i = 0; i < resultIndex.size(); i++) {
            _resultIndex[i] = (int) (resultIndex.get(i));
        }
        allNode2.clear();
        allNode2 = null;
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