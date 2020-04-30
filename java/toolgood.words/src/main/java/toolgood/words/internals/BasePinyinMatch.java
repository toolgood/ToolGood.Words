package toolgood.words.internals;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import toolgood.words.WordsSearch;
import toolgood.words.WordsSearchResult;

public class BasePinyinMatch {

    public class PinyinSearch extends BaseSearch {
        String[][] _keywordPinyins;
        int[] _indexs;

        public void SetIndexs(final int[] indexs) {
            _indexs = indexs;
        }
        public void SetIndexs(List<Integer> indexs) {
            _indexs=new int[indexs.size()];
            for (int i = 0; i < indexs.size(); i++) {
                _indexs[i]=indexs.get(i);
            }
        }

        public void SetKeywords2(List<TwoTuple<String, String[]>> keywords) {
            _keywords = new String[keywords.size()];
            _keywordPinyins = new String[keywords.size()][];
            for (int i = 0; i < keywords.size(); i++) {
                _keywords[i] = keywords.get(i).Item1;
                _keywordPinyins[i] = keywords.get(i).Item2;
            }
            SetKeywords();
        }

        public boolean Find(final String text, final String hz, final String[] pinyins) {
            TrieNode2 ptr = null;
            for (int i = 0; i < text.length(); i++) {
                final Character t = text.charAt(i);
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.HasKey(t) == false) {
                        tn = _first[t];
                    } else {
                        tn = ptr.GetValue(t);
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        for (final int result : tn.Results) {
                            final String keyword = _keywords[result];
                            final int start = i + 1 - keyword.length();
                            boolean isok = true;
                            final String[] keywordPinyins = _keywordPinyins[result];

                            for (int j = 0; j < keyword.length(); j++) {
                                final int idx = start + j;
                                final String py = keywordPinyins[j];
                                if (py.length() == 1 && py.charAt(0) >= 0x3400 && py.charAt(0) <= 0x9fd5) {
                                    if (hz.charAt(idx) != py.charAt(0)) {
                                        isok = false;
                                        break;
                                    }
                                } else {
                                    if (pinyins[idx].startsWith(py) == false) {
                                        isok = false;
                                        break;
                                    }
                                }
                            }
                            if (isok) {
                                return true;
                            }
                        }
                    }
                }
                ptr = tn;
            }
            return false;
        }

        public boolean Find2(final String text, final String hz, final String[] pinyins, final int keysCount) {
            int findCount = 0;
            int lastWordsIndex = -1;
            TrieNode2 ptr = null;
            for (int i = 0; i < text.length(); i++) {
                final Character t = text.charAt(i);
                TrieNode2 tn;
                if (ptr == null) {
                    tn = _first[t];
                } else {
                    if (ptr.HasKey(t) == false) {
                        tn = _first[t];
                    } else {
                        tn = ptr.GetValue(t);
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        for (final Integer result : tn.Results) {
                            final int index = _indexs[result];
                            if (index != findCount) {
                                continue;
                            }

                            final String keyword = _keywords[result];
                            final int start = i + 1 - keyword.length();
                            if (lastWordsIndex >= start) {
                                continue;
                            }

                            boolean isok = true;
                            final String[] keywordPinyins = _keywordPinyins[result];

                            for (int j = 0; j < keyword.length(); j++) {
                                final int idx = start + j;
                                final String py = keywordPinyins[j];
                                if (py.length() == 1 && py.charAt(0) >= 0x3400 && py.charAt(0) <= 0x9fd5) {
                                    if (hz.charAt(idx) != py.charAt(0)) {
                                        isok = false;
                                        break;
                                    }
                                } else {
                                    if (pinyins[idx].startsWith(py) == false) {
                                        isok = false;
                                        break;
                                    }
                                }
                            }
                            if (isok) {
                                findCount++;
                                lastWordsIndex = i;
                                if (findCount == keysCount) {
                                    return true;
                                }
                                break;
                            }
                        }
                    }
                }
                ptr = tn;
            }
            return false;
        }

    }

    protected void MergeKeywords(String[] keys, int id, String keyword, List<TwoTuple<String, String[]>> list)
            throws NumberFormatException, IOException {
        if (id >= keys.length) {
            TwoTuple<String, String[]> tuple = new TwoTuple<String, String[]>(keyword, keys);
            list.add(tuple);
            return;
        }
        String key = keys[id];
        if (key.charAt(0) >= 0x3400 && key.charAt(0) <= 0x9fd5) {
            List<String> all = PinyinDict.GetAllPinyin(key.charAt(0), 0);
            Set<Character> fpy = new HashSet<Character>();
            for (String item : all) {
                fpy.add(item.charAt(0));
            }
            for (Character item : fpy) {
                MergeKeywords(keys, id + 1, keyword + item, list);
            }
        } else {
            MergeKeywords(keys, id + 1, keyword + key.charAt(0), list);
        }
    }

    protected void MergeKeywords(String[] keys, int id, String keyword, List<TwoTuple<String, String[]>> list,
            int index, List<Integer> indexs) throws NumberFormatException, IOException {
        if (id >= keys.length) {
            TwoTuple<String, String[]> tuple = new TwoTuple<String, String[]>(keyword, keys);
            list.add(tuple);
            indexs.add(index);
            return;
        }
        String key = keys[id];
        if (key.charAt(0) >= 0x3400 && key.charAt(0) <= 0x9fd5) {
            List<String> all = PinyinDict.GetAllPinyin(key.charAt(0), 0);
            Set<Character> fpy = new HashSet<Character>();
            for (String item : all) {
                fpy.add(item.charAt(0));
            }
            for (Character item : fpy) {
                MergeKeywords(keys, id + 1, keyword + item, list, index, indexs);
            }
        } else {
            MergeKeywords(keys, id + 1, keyword + key.charAt(0), list, index, indexs);
        }
    }

    protected List<String> SplitKeywords(String key) throws NumberFormatException, IOException {
        InitPinyinSearch();
        List<TextNode> textNodes = new ArrayList<TextNode>();
        for (int i = 0; i <= key.length(); i++) {
            textNodes.add(new TextNode());
        }
        textNodes.get(textNodes.size() - 1).End = true;

        for (int i = 0; i < key.length(); i++) {
            TextLine line = new TextLine();
            line.Next = textNodes.get(i + 1);
            line.Words = ((Character) key.charAt(i)).toString();
            textNodes.get(i).Children.add(line);
        }

        List<WordsSearchResult> all = _wordsSearch.FindAll(key);
        for (WordsSearchResult searchResult : all) {
            TextLine line = new TextLine();
            line.Next = textNodes.get(searchResult.End + 1);
            line.Words = searchResult.Keyword;
            textNodes.get(searchResult.Start).Children.add(line);
        }

        List<String> list = new ArrayList<String>();
        BuildKsywords(textNodes.get(0), 0, "", list);
        return list;
    }

    private void BuildKsywords(TextNode textNode, int id, String keywords, List<String> list) {
        if (textNode.End) {
            String k = keywords.substring(1);
            if (list.contains(k) == false) {
                list.add(k);
            }
            return;
        }
        for (TextLine item : textNode.Children) {
            BuildKsywords(item.Next, id + 1, keywords + (char) 0 + item.Words, list);
        }
    }

    class TextNode {
        public boolean End;
        public List<TextLine> Children = new ArrayList<TextLine>();
    }

    class TextLine {
        public String Words;
        public TextNode Next;
    }

    private static WordsSearch _wordsSearch;

    private void InitPinyinSearch() throws NumberFormatException, IOException {
        if (_wordsSearch == null) {
            List<String> allPinyins = new ArrayList<String>();
            String[] pys = PinyinDict.getPyShow();
            for (int i = 1; i < pys.length; i += 2) {
                String py = pys[i].toUpperCase();
                for (int j = 1; j <= py.length(); j++) {
                    String key = py.substring(0, j);
                    if (allPinyins.contains(key) == false) {
                        allPinyins.add(key);
                    }

                }
            }
            WordsSearch wordsSearch = new WordsSearch();
            wordsSearch.SetKeywords(allPinyins);
            _wordsSearch = wordsSearch;
        }
    }
}