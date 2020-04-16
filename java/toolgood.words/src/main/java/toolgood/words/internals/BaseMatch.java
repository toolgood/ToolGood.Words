package toolgood.words.internals;

import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;

public class BaseMatch {
    protected TrieNode3[] _first;
    protected int[] _keywordLength;
    protected int[] _keywordIndex;
    protected String[] _matchKeywords;

    protected List<TrieNode> BuildFirstLayerTrieNode(List<String> keywords) {
        TrieNode root = new TrieNode();

        Map<Integer, List<TrieNode>> allNodeLayers = new Hashtable<Integer, List<TrieNode>>();
        // 第一次关键字
        for (int i = 0; i < keywords.size(); i++) {
            String p = keywords.get(i);
            TrieNode nd = root;
            int start = 0;
            while (p.charAt(start) == 0) { // 0 为 通配符
                start++;
            }
            for (int j = start; j < p.length(); j++) {
                nd = nd.Add(p.charAt(j));
                if (nd.Layer == 0) {
                    nd.Layer = j + 1 - start;
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
        Character z = 0;
        // 第二次关键字 通配符
        for (int i = 0; i < keywords.size(); i++) {
            String p = keywords.get(i);
            if (p.contains(z.toString()) == false) {
                continue;
            }
            int start = 0;
            while (p.charAt(start) == 0) { // 0 为 通配符
                start++;
            }
            List<TrieNode> trieNodes = new ArrayList<TrieNode>();
            trieNodes.add(root);

            for (int j = start; j < p.length(); j++) {
                List<TrieNode> newTrieNodes = new ArrayList<TrieNode>();
                Character c = p.charAt(j);
                if (c == 0) {
                    for (TrieNode nd : trieNodes) {
                        for (Character key : nd.m_values.keySet()) {
                            newTrieNodes.add(nd.m_values.get(key));
                        }
                    }
                } else {
                    for (TrieNode nd : trieNodes) {
                        TrieNode nd2 = nd.Add(c);
                        if (nd2.Layer == 0) {
                            nd2.Layer = j + 1 - start;
                            if (allNodeLayers.containsKey(nd2.Layer) == false) {
                                List<TrieNode> nodes = new ArrayList<TrieNode>();
                                nodes.add(nd2);
                                allNodeLayers.put(nd2.Layer, nodes);
                            } else {
                                allNodeLayers.get(nd2.Layer).add(nd2);
                            }
                            // List<TrieNode> tnodes;
                            // if (allNodeLayers.TryGetValue(nd2.Layer, tnodes) == false) {
                            // tnodes = new ArrayList<TrieNode>();
                            // allNodeLayers[nd.Layer] = tnodes;
                            // }
                            // tnodes.add(nd2);
                        }
                        newTrieNodes.add(nd2);
                    }
                }
                trieNodes = newTrieNodes;
            }
            for (TrieNode nd : trieNodes) {
                nd.SetResults(i);
            }
        }

        // 添加到 allNode
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

        // 第一次 Set Failure
        for (int i = 1; i < allNode.size(); i++) {
            TrieNode nd = allNode.get(i);
            nd.Index = i;
            TrieNode r = nd.Parent.Failure;
            char c = nd.Char;
            while (r != null && !r.m_values.containsKey(c))
                r = r.Failure;
            if (r == null)
                nd.Failure = root;
            else {
                nd.Failure = r.m_values.get(c);
                for (Integer result : nd.Failure.Results)
                    nd.SetResults(result);
            }
        }

        // 第二次 Set Failure
        Character zore = 0;
        for (int i = 1; i < allNode.size(); i++) {
            TrieNode nd = allNode.get(i);
            if (nd.Layer == 1) {
                continue;
            }

            if (nd.m_values.containsKey(zore)) {
                nd.HasWildcard = true;
            }
            if (nd.Failure.HasWildcard) {
                nd.HasWildcard = true;
            }
            if (nd.Char == 0) {
                nd.IsWildcard = true;
                continue;
            } else if (nd.Parent.IsWildcard) {
                nd.IsWildcard = true;
                nd.WildcardLayer = nd.Parent.WildcardLayer + 1;
                if (nd.Failure != root) {
                    if (nd.Failure.Layer <= nd.WildcardLayer) {
                        nd.Failure = root;
                    }
                }
                continue;
            }
        }
        root.Failure = root;

        return allNode;
    }

    protected boolean HasMatch(String keyword) {
        for (int i = 0; i < keyword.length(); i++) {
            Character c = keyword.charAt(i);
            if (c == '.' || c == '?' || c == '\\' || c == '[' || c == '(') {
                return true;
            }
        }
        return false;
    }

    protected List<String> MatchKeywordBuild(String keyword) throws Exception {
        StringBuilder stringBuilder = new StringBuilder();
        Map<Integer, List<String>> parameterDict = new Hashtable<Integer, List<String>>();
        SeparateParameters(keyword, stringBuilder, parameterDict);

        if (parameterDict.size() == 0) {
            List<String> al = new ArrayList<String>();
            al.add(stringBuilder.toString());
            return al;
        }
        List<String> parameters = new ArrayList<String>();
        KeywordBuild(parameterDict, 0, parameterDict.keySet().size() - 1, "", parameters);
        String keywordFmt = stringBuilder.toString();
        List<String> list = new ArrayList<String>();

        String z = ((Character) (char) 0).toString();
        for (int i = 0; i < parameters.size(); i++) {
            String item = parameters.get(i);
            String[] items = item.split(z);
            Object[] ls = new Object[items.length];
            for (int j = 0; j < ls.length; j++) {
                ls[j] = items[j];
            }
            String t = String.format(keywordFmt, ls);
            if (list.contains(t) == false) {
                list.add(t);
            }
        }
        return list;
    }

    private void SeparateParameters(String keyword, StringBuilder stringBuilder,
            Map<Integer, List<String>> parameterDict) throws Exception {
        int index = 0;
        int parameterIndex = 0;
        Character zore = 0;

        while (index < keyword.length()) {
            Character c = keyword.charAt(index);
            if (c == '.') {
                if (index + 1 < keyword.length() && keyword.charAt(index + 1) == '?') {
                    List<String> lt = new ArrayList<String>();
                    lt.add("");
                    lt.add(zore.toString());
                    parameterDict.put(parameterIndex, lt);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index += 2;

                } else {
                    stringBuilder.append(((char) 0));
                    index++;
                }
            } else if (c == '\\') {
                if (index + 2 < keyword.length() && keyword.charAt(index + 2) == '?') {
                    List<String> lt = new ArrayList<String>();
                    lt.add("");
                    lt.add(((Character) keyword.charAt(index + 1)).toString());
                    parameterDict.put(parameterIndex, lt);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index += 3;
                } else if (index + 1 < keyword.length()) {
                    stringBuilder.append(keyword.charAt(index + 1));
                    index += 2;
                } else {
                    throw new Exception("【{keyword}】出错了，最后一位为\\");
                }
            } else if (c == '[') {
                index++;
                List<String> ps = new ArrayList<String>();
                while (index < keyword.length()) {
                    c = keyword.charAt(index);
                    if (c == ']') {
                        break;
                    } else if (c == '\\') {
                        if (index + 1 < keyword.length()) {
                            ps.add(((Character) keyword.charAt(index + 1)).toString());
                            index += 2;
                        }
                    } else {
                        ps.add(c.toString());
                        index++;
                    }
                }
                if (c != ']') {
                    throw new Exception("【{keyword}】出错了，最后一位不为]");
                }
                if (index + 1 < keyword.length() && keyword.charAt(index + 1) == '?') {
                    ps.add("");
                    parameterDict.put(parameterIndex, ps);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index += 2;
                } else {
                    parameterDict.put(parameterIndex, ps);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index++;
                }
            } else if (c == '(') {
                index++;
                List<String> ps = new ArrayList<String>();
                String words = "";
                while (index < keyword.length()) {
                    c = keyword.charAt(index);
                    if (c == ')') {
                        break;
                    } else if (c == '|') {
                        ps.add(words);
                        words = "";
                        index++;
                    } else if (c == '\\') {
                        if (index + 1 < keyword.length()) {
                            words += keyword.charAt(index + 1);
                            index += 2;
                        }
                    } else {
                        words += c;
                        index++;
                    }
                }
                ps.add(words);
                if (c != ')') {
                    throw new Exception("【{keyword}】出错了，最后一位不为)");
                }
                if (index + 1 < keyword.length() && keyword.charAt(index + 1) == '?') {
                    ps.add("");
                    parameterDict.put(parameterIndex, ps);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index += 2;
                } else {
                    parameterDict.put(parameterIndex, ps);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index++;
                }
            } else {
                if (index + 1 < keyword.length() && keyword.charAt(index + 1) == '?') {
                    List<String> lt = new ArrayList<String>();
                    lt.add("");
                    lt.add(c.toString());
                    parameterDict.put(parameterIndex, lt);
                    stringBuilder.append("%" + parameterIndex + "$s");
                    parameterIndex++;
                    index += 2;
                } else {
                    if (c == '{') {
                        stringBuilder.append("{{");
                    } else if (c == '}') {
                        stringBuilder.append("}}");
                    } else {
                        stringBuilder.append(c);
                    }
                    index++;
                }
            }
        }

    }

    private static void KeywordBuild(Map<Integer, List<String>> parameterDict, int index, int end, String keyword,
            List<String> result) {
        Character span = (char) 1;
        List<String> list = parameterDict.get(index);
        if (index == end) {
            for (int i = 0; i < list.size(); i++) {
                String item = list.get(i);
                result.add((keyword + span + item).substring(1));
            }
        } else {
            for (int i = 0; i < list.size(); i++) {
                String item = list.get(i);
                KeywordBuild(parameterDict, index + 1, end, keyword + span + item, result);
            }
        }
    }

    /**
     * 设置关键字
     * 
     * @param keywords 关键字列表
     * @throws Exception
     */
    public void SetKeywords(List<String> keywords) throws Exception {
        _matchKeywords = keywords.toArray(new String[0]);
        List<String> newKeyword = new ArrayList<String>();
        List<Integer> newKeywordLength = new ArrayList<Integer>();
        List<Integer> newKeywordIndex = new ArrayList<Integer>();
        Integer index = 0;
        for (String keyword : keywords) {
            if (HasMatch(keyword) == false) {
                newKeyword.add(keyword);
                newKeywordLength.add(keyword.length());
                newKeywordIndex.add(index);
            } else {
                List<String> list = MatchKeywordBuild(keyword);
                for (String item : list) {
                    newKeyword.add(item);
                    newKeywordLength.add(item.length());
                    newKeywordIndex.add(index);
                }
            }
            index++;
        }
        _keywordLength = new int[newKeywordLength.size()];
        for (int i = 0; i < _keywordLength.length; i++) {
            _keywordLength[i] = newKeywordLength.get(i);
        }
        _keywordIndex = new int[newKeywordIndex.size()];
        for (int j = 0; j < _keywordIndex.length; j++) {
            _keywordIndex[j] = newKeywordIndex.get(j);
        }

        SetKeywords2(newKeyword);
    }

    protected void SetKeywords2(List<String> keywords) {
        List<TrieNode> allNode = BuildFirstLayerTrieNode(keywords);
        TrieNode root = allNode.get(0);

        List<TrieNode3> allNode2 = new ArrayList<TrieNode3>();
        for (int i = 0; i < allNode.size(); i++) {
            allNode2.add(new TrieNode3());
        }

        for (int i = 0; i < allNode2.size(); i++) {
            TrieNode oldNode = allNode.get(i);
            TrieNode3 newNode = allNode2.get(i);

            for (Character key : oldNode.m_values.keySet()) {
                int index = oldNode.m_values.get(key).Index;
                if (key == 0) {
                    newNode.HasWildcard = true;
                    newNode.WildcardNode = allNode2.get(index);
                    continue;
                }
                newNode.Add(key, allNode2.get(index));
            }
            for (Integer item : oldNode.Results) {
                if (oldNode.IsWildcard) {
                    if (keywords.get(item).length() > oldNode.WildcardLayer) {
                        newNode.SetResults(item);
                    }
                } else {
                    newNode.SetResults(item);
                }
            }

            TrieNode failure = oldNode.Failure;
            while (failure != root) {
                if (oldNode.IsWildcard && failure.Layer <= oldNode.WildcardLayer) {
                    break;
                }
                for (Character key : failure.m_values.keySet()) {
                    int index = failure.m_values.get(key).Index;
                    if (key == 0) {
                        newNode.HasWildcard = true;
                        if (newNode.WildcardNode == null) {
                            newNode.WildcardNode = allNode2.get(index);
                        }
                        continue;
                    }
                    if (newNode.HasKey(key) == false) {
                        newNode.Add(key, allNode2.get(index));
                    }
                }
                for (Integer item : failure.Results) {
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

        // var root2 = allNode2[0];
        TrieNode3[] first = new TrieNode3[Character.MAX_VALUE + 1];
        for (Character key : allNode2.get(0).m_values.keySet()) {
            first[key] = allNode2.get(0).m_values.get(key);
        }
        _first = first;
    }

}