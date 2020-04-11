using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    public abstract class BaseMatchEx : BaseMatch
    {
        protected int[] _dict;
        protected int[] _first;
        protected int[] _min;
        protected int[] _max;

        protected IntDictionary[] _nextIndex;
        protected int[] _end;
        protected int[] _resultIndex;


        #region SetKeywords2
        protected override void SetKeywords2(List<string> keywords)
        {
            List<TrieNode> allNode = BuildFirstLayerTrieNode(keywords);
            TrieNode root = allNode[0];

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < allNode.Count; i++) {
                stringBuilder.Append(allNode[i].Char);
            }
            var length = CreateDict(stringBuilder.ToString());
            stringBuilder = null;


            var allNode2 = new List<TrieNode3Ex>();
            for (int i = 0; i < allNode.Count; i++) {
                allNode2.Add(new TrieNode3Ex());
            }

            for (int i = 0; i < allNode2.Count; i++) {
                var oldNode = allNode[i];
                var newNode = allNode2[i];

                foreach (var item in oldNode.m_values) {
                    var key = _dict[item.Key];
                    var index = item.Value.Index;
                    if (key == 0) {
                        newNode.HasWildcard = true;
                        newNode.WildcardNode = allNode2[index];
                        continue;
                    }
                    newNode.Add((char)key, allNode2[index]);
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
                        var key = _dict[item.Key];
                        var index = item.Value.Index;
                        if (key == 0) {
                            newNode.HasWildcard = true;
                            if (newNode.WildcardNode == null) {
                                newNode.WildcardNode = allNode2[index];
                            }
                            continue;
                        }
                        if (newNode.HasKey((char)key) == false) {
                            newNode.Add((char)key, allNode2[index]);
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


            var min = new List<int>();
            var max = new List<int>();
            var nextIndexs = new List<Dictionary<int, int>>();
            var end = new List<int>() { 0 };
            var resultIndex = new List<int>();
            for (int i = 0; i < allNode2.Count; i++) {
                var dict = new Dictionary<int, int>();
                var node = allNode2[i];
                min.Add(node.minflag);
                max.Add(node.maxflag);

                if (i > 0) {
                    foreach (var item in node.m_values) {
                        dict[item.Key] = item.Value.Index;
                    }
                }
                foreach (var item in node.Results) {
                    resultIndex.Add(item);
                }
                end.Add(resultIndex.Count);
                nextIndexs.Add(dict);
            }
            var first = new int[Char.MaxValue + 1];
            foreach (var item in allNode2[0].m_values) {
                first[item.Key] = item.Value.Index;
            }

            _first = first;
            _min = min.ToArray();
            _max = max.ToArray();
            _nextIndex = new IntDictionary[nextIndexs.Count];
            for (int i = 0; i < nextIndexs.Count; i++) {
                IntDictionary dictionary = new IntDictionary();
                dictionary.SetDictionary(nextIndexs[i]);
                _nextIndex[i] = dictionary;
            }
            _end = end.ToArray();
            _resultIndex = resultIndex.ToArray();

            allNode2.Clear();
            allNode2 = null;

        }
        #endregion

        #region 生成映射字典

        private int CreateDict(string keywords)
        {
            Dictionary<char, Int32> dictionary = new Dictionary<char, Int32>();
            foreach (var item in keywords) {
                if (dictionary.ContainsKey(item)) {
                    dictionary[item] += 1;
                } else {
                    dictionary[item] = 1;
                }
            }
            var list = dictionary.OrderByDescending(q => q.Value).Select(q => q.Key).ToList();
            var list2 = new List<char>();
            var sh = false;
            foreach (var item in list) {
                if (sh) {
                    list2.Add(item);
                } else {
                    list2.Insert(0, item);
                }
                sh = !sh;
            }
            _dict = new int[char.MaxValue + 1];
            for (Int32 i = 0; i < list2.Count; i++) {
                _dict[list2[i]] = (int)(i + 1);
            }
            return dictionary.Count;
        }

        #endregion
    }
}
