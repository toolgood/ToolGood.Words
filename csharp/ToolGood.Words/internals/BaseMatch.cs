using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseMatch
    {
        protected TrieNode3[] _first;
        protected internal int[] _keywordLength;
        protected internal int[] _keywordIndex;
        protected internal string[] _matchKeywords;

        #region BuildFirstLayerTrieNode
        protected List<TrieNode> BuildFirstLayerTrieNode(List<string> keywords)
        {
            var root = new TrieNode();

            Dictionary<int, List<TrieNode>> allNodeLayers = new Dictionary<int, List<TrieNode>>();
            #region 第一次关键字
            for (int i = 0; i < keywords.Count; i++) {
                var p = keywords[i];
                var nd = root;
                var start = 0;
                while (p[start] == 0) { // 0 为 通配符
                    start++;
                }
                for (int j = start; j < p.Length; j++) {
                    nd = nd.Add((char)p[j]);
                    if (nd.Layer == 0) {
                        nd.Layer = j + 1 - start;
                        List<TrieNode> trieNodes;
                        if (allNodeLayers.TryGetValue(nd.Layer, out trieNodes) == false) {
                            trieNodes = new List<TrieNode>();
                            allNodeLayers[nd.Layer] = trieNodes;
                        }
                        trieNodes.Add(nd);
                    }
                }
                nd.SetResults(i);
            }
            #endregion

            #region 第二次关键字 通配符
            for (int i = 0; i < keywords.Count; i++) {
                var p = keywords[i];
                if (p.Contains((char)0) == false) {
                    continue;
                }
                var start = 0;
                while (p[start] == 0) { // 0 为 通配符
                    start++;
                }
                List<TrieNode> trieNodes = new List<TrieNode>() { root };

                for (int j = start; j < p.Length; j++) {
                    List<TrieNode> newTrieNodes = new List<TrieNode>();
                    var c = p[j];
                    if (c == 0) {
                        foreach (var nd in trieNodes) {
                            newTrieNodes.AddRange(nd.m_values.Values);
                        }
                    } else {
                        foreach (var nd in trieNodes) {
                            var nd2 = nd.Add(c);
                            if (nd2.Layer == 0) {
                                nd2.Layer = j + 1 - start;
                                List<TrieNode> tnodes;
                                if (allNodeLayers.TryGetValue(nd2.Layer, out tnodes) == false) {
                                    tnodes = new List<TrieNode>();
                                    allNodeLayers[nd.Layer] = tnodes;
                                }
                                tnodes.Add(nd2);
                            }
                            newTrieNodes.Add(nd2);
                        }
                    }
                    trieNodes = newTrieNodes;
                }
                foreach (var nd in trieNodes) {
                    nd.SetResults(i);
                }
            }
            #endregion

            #region 添加到 allNode
            var allNode = new List<TrieNode>();
            allNode.Add(root);
            foreach (var trieNodes in allNodeLayers) {
                foreach (var nd in trieNodes.Value) {
                    allNode.Add(nd);
                }
            }
            allNodeLayers.Clear();
            allNodeLayers = null;
            #endregion

            #region 第一次 Set Failure
            for (int i = 1; i < allNode.Count; i++) {
                var nd = allNode[i];
                nd.Index = i;
                TrieNode r = nd.Parent.Failure;
                char c = nd.Char;
                while (r != null && !r.m_values.ContainsKey(c)) r = r.Failure;
                if (r == null)
                    nd.Failure = root;
                else {
                    nd.Failure = r.m_values[c];
                    foreach (var result in nd.Failure.Results)
                        nd.SetResults(result);
                }
            }
            #endregion

            #region 第二次 Set Failure
            for (int i = 1; i < allNode.Count; i++) {
                var nd = allNode[i];
                if (nd.Layer == 1) { continue; }
                if (nd.m_values.ContainsKey((char)0)) {
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
            #endregion

            return allNode;
        }
        #endregion

        #region HasMatch
        protected bool HasMatch(string keyword)
        {
            for (int i = 0; i < keyword.Length; i++) {
                char c = keyword[i];
                if (c == '.' || c == '?' || c == '\\' || c == '[' || c == '(') {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region MatchKeywordBuild
        protected List<string> MatchKeywordBuild(string keyword)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<int, List<string>> parameterDict = new Dictionary<int, List<string>>();
            SeparateParameters(keyword, stringBuilder, parameterDict);

            if (parameterDict.Count == 0) {
                return new List<string>() { stringBuilder.ToString() };
            }
            List<string> parameters = new List<string>();
            KeywordBuild(parameterDict, 0, parameterDict.Keys.Count - 1, "", parameters);
            var keywordFmt = stringBuilder.ToString();
            HashSet<string> list = new HashSet<string>();
            foreach (var item in parameters) {
                list.Add(string.Format(keywordFmt, item.Split((char)1)));
            }
            return list.ToList();
        }

        private static void SeparateParameters(string keyword, StringBuilder stringBuilder, Dictionary<int, List<string>> parameterDict)
        {
            var index = 0;
            var parameterIndex = 0;

            while (index < keyword.Length) {
                var c = keyword[index];
                if (c == '.') {
                    if (index + 1 < keyword.Length && keyword[index + 1] == '?') {
                        parameterDict[parameterIndex] = new List<string>() { "", ((char)0).ToString() };
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index += 2;

                    } else {
                        stringBuilder.Append(((char)0));
                        index++;
                    }
                } else if (c == '\\') {
                    if (index + 2 < keyword.Length && keyword[index + 2] == '?') {
                        parameterDict[parameterIndex] = new List<string>() { "", keyword[index + 1].ToString() };
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index += 3;
                    } else if (index + 1 < keyword.Length) {
                        stringBuilder.Append(keyword[index + 1]);
                        index += 2;
                    } else {
                        throw new Exception($"【{keyword}】出错了，最后一位为\\");
                    }
                } else if (c == '[') {
                    index++;
                    var ps = new List<string>();
                    while (index < keyword.Length) {
                        c = keyword[index];
                        if (c == ']') {
                            break;
                        } else if (c == '\\') {
                            if (index + 1 < keyword.Length) {
                                ps.Add(keyword[index + 1].ToString());
                                index += 2;
                            }
                        } else {
                            ps.Add(c.ToString());
                            index++;
                        }
                    }
                    if (c != ']') {
                        throw new Exception($"【{keyword}】出错了，最后一位不为]");
                    }
                    if (index + 1 < keyword.Length && keyword[index + 1] == '?') {
                        ps.Insert(0, "");
                        parameterDict[parameterIndex] = ps;
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index += 2;
                    } else {
                        parameterDict[parameterIndex] = ps;
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index++;
                    }
                } else if (c == '(') {
                    index++;
                    var ps = new List<string>();
                    var words = "";
                    while (index < keyword.Length) {
                        c = keyword[index];
                        if (c == ')') {
                            break;
                        } else if (c == '|') {
                            ps.Add(words);
                            words = "";
                            index++;
                        } else if (c == '\\') {
                            if (index + 1 < keyword.Length) {
                                words += keyword[index + 1];
                                index += 2;
                            }
                        } else {
                            words += c;
                            index++;
                        }
                    }
                    ps.Add(words);
                    if (c != ')') {
                        throw new Exception($"【{keyword}】出错了，最后一位不为)");
                    }
                    if (index + 1 < keyword.Length && keyword[index + 1] == '?') {
                        ps.Insert(0, "");
                        parameterDict[parameterIndex] = ps;
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index += 2;
                    } else {
                        parameterDict[parameterIndex] = ps;
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index++;
                    }
                } else {
                    if (index + 1 < keyword.Length && keyword[index + 1] == '?') {
                        parameterDict[parameterIndex] = new List<string>() { "", c.ToString() };
                        stringBuilder.Append("{" + parameterIndex + "}");
                        parameterIndex++;
                        index += 2;
                    } else {
                        if (c == '{') {
                            stringBuilder.Append("{{");
                        } else if (c == '}') {
                            stringBuilder.Append("}}");
                        } else {
                            stringBuilder.Append(c);
                        }
                        index++;
                    }
                }
            }
        }

        private static void KeywordBuild(Dictionary<int, List<string>> parameterDict, int index, int end, string keyword, List<string> result)
        {
            const char span = (char)1;
            var list = parameterDict[index];
            if (index == end) {
                foreach (var item in list) {
                    result.Add((keyword + span + item).Substring(1));
                }
            } else {
                foreach (var item in list) {
                    KeywordBuild(parameterDict, index + 1, end, keyword + span + item, result);
                }
            }
        }

        #endregion

        #region SetKeywords
        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public virtual void SetKeywords(ICollection<string> keywords)
        {
            _matchKeywords = keywords.ToArray();
            List<string> newKeyword = new List<string>();
            List<int> newKeywordLength = new List<int>();
            List<int> newKeywordIndex = new List<int>();
            var index = 0;
            foreach (var keyword in keywords) {
                if (HasMatch(keyword) == false) {
                    newKeyword.Add(keyword);
                    newKeywordLength.Add(keyword.Length);
                    newKeywordIndex.Add(index);
                } else {
                    var list = MatchKeywordBuild(keyword);
                    foreach (var item in list) {
                        newKeyword.Add(item);
                        newKeywordLength.Add(item.Length);
                        newKeywordIndex.Add(index);
                    }
                }
                index++;
            }
            _keywordLength = newKeywordLength.ToArray();
            _keywordIndex = newKeywordIndex.ToArray();

            SetKeywords2(newKeyword);
        }

        #endregion

        #region SetKeywords2

        protected virtual void SetKeywords2(List<string> keywords)
        {
            List<TrieNode> allNode = BuildFirstLayerTrieNode(keywords);
            TrieNode root = allNode[0];

            var allNode2 = new List<TrieNode3>();
            for (int i = 0; i < allNode.Count; i++) {
                allNode2.Add(new TrieNode3());
            }

            for (int i = 0; i < allNode2.Count; i++) {
                var oldNode = allNode[i];
                var newNode = allNode2[i];

                foreach (var item in oldNode.m_values) {
                    var key = item.Key;
                    var index = item.Value.Index;
                    if (key == 0) {
                        newNode.HasWildcard = true;
                        newNode.WildcardNode = allNode2[index];
                        continue;
                    }
                    newNode.Add(key, allNode2[index]);
                }
                foreach (var item in oldNode.Results) {
                    if (oldNode.IsWildcard) {
                        if (keywords[item].Length > oldNode.WildcardLayer) {
                            newNode.SetResults(item);
                        }
                    } else {
                        newNode.SetResults(item);
                    }
                    //newNode.SetResults(item);
                }

                var failure = oldNode.Failure;
                while (failure != root) {
                    if (oldNode.IsWildcard && failure.Layer <= oldNode.WildcardLayer) {
                        break;
                    }
                    foreach (var item in failure.m_values) {
                        var key = item.Key;
                        var index = item.Value.Index;
                        if (key == 0) {
                            newNode.HasWildcard = true;
                            if (newNode.WildcardNode == null) {
                                newNode.WildcardNode = allNode2[index];
                            }
                            continue;
                        }
                        if (newNode.HasKey(key) == false) {
                            newNode.Add(key, allNode2[index]);
                        }
                    }
                    foreach (var item in failure.Results) {
                        if (oldNode.IsWildcard) {
                            if (keywords[item].Length > oldNode.WildcardLayer) {
                                newNode.SetResults(item);
                            }
                        } else {
                            newNode.SetResults(item);
                        }
                    }
                    failure = failure.Failure;
                }
            }
            allNode.Clear();
            allNode = null;
            root = null;

            //var root2 = allNode2[0];
            TrieNode3[] first = new TrieNode3[char.MaxValue + 1];
            foreach (var item in allNode2[0].m_values) {
                first[item.Key] = item.Value;
            }
            _first = first;
        } 
        #endregion



    }
}
