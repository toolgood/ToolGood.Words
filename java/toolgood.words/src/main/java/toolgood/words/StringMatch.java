package toolgood.words;

import java.util.ArrayList;
import java.util.List;

import toolgood.words.internals.BaseMatch;
import toolgood.words.internals.TrieNode3;

/**
 * 文本搜索匹配, ,支持 部分 正则 如 . ? [ ] \ ( | ) ,不支持( )内再嵌套( )
 */
public class StringMatch extends BaseMatch {

    /**
     * 在文本中查找第一个关键字
     * 
     * @param text 文本
     * @return
     */
    public String FindFirst(String text) {
        TrieNode3 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);

            TrieNode3 tn;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    if (ptr.HasWildcard) {
                        String result = FindFirst(text, i + 1, ptr.WildcardNode);
                        if (result != null) {
                            return result;
                        }
                    }
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    int length = _keywordLength[tn.Results.get(0)];
                    int s = i - length + 1;
                    if (s >= 0) {
                        return text.substring(s, i + 1);
                    }
                }
            }
            ptr = tn;
        }
        return null;
    }

    private String FindFirst(String text, int index, TrieNode3 ptr) {
        for (int i = index; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr.HasKey(t) == false) {
                if (ptr.HasWildcard) {
                    String result = FindFirst(text, i + 1, ptr.WildcardNode);
                    if (result != null) {
                        return result;
                    }
                }
                return null;
            }
            tn = ptr.GetValue(t);

            if (tn.End) {
                int length = _keywordLength[tn.Results.get(0)];
                int s = i - length + 1;
                if (s >= 0) {
                    return text.substring(s, i + 1);
                }
            }
            ptr = tn;
        }
        return null;
    }

    /**
     * 在文本中查找所有的关键字
     * 
     * @param text 文本
     * @return
     */
    public List<String> FindAll(String text) {
        TrieNode3 ptr = null;
        List<String> result = new ArrayList<String>();

        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    if (ptr.HasWildcard) {
                        FindAll(text, i + 1, ptr.WildcardNode, result);
                    }
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    for (Integer item : tn.Results) {
                        int length = _keywordLength[item];
                        int s = i - length + 1;
                        if (s >= 0) {
                            String key = text.substring(s, i + 1);
                            result.add(key);

                        }
                    }
                }
            }
            ptr = tn;
        }
        return result;
    }

    private void FindAll(String text, int index, TrieNode3 ptr, List<String> result) {
        for (int i = index; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr.HasKey(t) == false) {
                if (ptr.HasWildcard) {
                    FindAll(text, i + 1, ptr.WildcardNode, result);
                }
                return;
            } else {
                tn = ptr.GetValue(t);
            }
            if (tn.End) {
                for (Integer item : tn.Results) {
                    int length = _keywordLength[item];
                    int s = i - length + 1;
                    if (s >= 0) {
                        String key = text.substring(s, i + 1);
                        result.add(key);
                    }
                }
            }
            ptr = tn;
        }
    }
    /**
     * 判断文本是否包含关键字
     * 
     * @param text 文本
     * @return
     */
    public boolean ContainsAny(String text) {
        TrieNode3 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    if (ptr.HasWildcard) {
                        boolean result = ContainsAny(text, i + 1, ptr.WildcardNode);
                        if (result) {
                            return true;
                        }
                    }
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    int length = _keywordLength[tn.Results.get(0)];
                    int s = i - length + 1;
                    if (s >= 0) {
                        return true;
                    }
                }
            }
            ptr = tn;
        }
        return false;
    }

    private boolean ContainsAny(String text, int index, TrieNode3 ptr) {
        for (int i = index; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr.HasKey(t) == false) {
                if (ptr.HasWildcard) {
                    return ContainsAny(text, i + 1, ptr.WildcardNode);
                }
                return false;
            }
            tn = ptr.GetValue(t);

            if (tn.End) {
                int length = _keywordLength[tn.Results.get(0)];
                int s = i - length + 1;
                if (s >= 0) {
                    return true;
                }
            }
            ptr = tn;
        }
        return false;
    }

    /**
     * 在文本中替换所有的关键字, 替换符默认为 *
     * 
     * @param text 文本
     * @return
     */
    public String Replace(String text) {
        return Replace(text, '*');
    }

    /**
     * 在文本中替换所有的关键字
     * 
     * @param text        文本
     * @param replaceChar 替换符
     * @return
     */
    public String Replace(String text, char replaceChar) {
        StringBuilder result = new StringBuilder(text);

        TrieNode3 ptr = null;
        for (int i = 0; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr == null) {
                tn = _first[t];
            } else {
                if (ptr.HasKey(t) == false) {
                    if (ptr.HasWildcard) {
                        Replace(text, i + 1, ptr.WildcardNode, replaceChar, result);
                    }
                    tn = _first[t];
                } else {
                    tn = ptr.GetValue(t);
                }
            }
            if (tn != null) {
                if (tn.End) {
                    int maxLength = _keywordLength[tn.Results.get(0)];
                    int start = i + 1 - maxLength;
                    if (start >= 0) {
                        for (int j = start; j <= i; j++) {
                            result.setCharAt(j, replaceChar);
                        }
                    }
                }
            }
            ptr = tn;
        }
        return result.toString();
    }

    private void Replace(String text, int index, TrieNode3 ptr, char replaceChar, StringBuilder result) {
        for (int i = index; i < text.length(); i++) {
            Character t = text.charAt(i);
            TrieNode3 tn;
            if (ptr.HasKey(t) == false) {
                if (ptr.HasWildcard) {
                    Replace(text, i + 1, ptr.WildcardNode, replaceChar, result);
                }
                return;
            }
            tn = ptr.GetValue(t);
            if (tn.End) {
                int maxLength = _keywordLength[tn.Results.get(0)];
                int start = i + 1 - maxLength;
                if (start >= 0) {
                    for (int j = start; j <= i; j++) {
                        result.setCharAt(j, replaceChar);
                    }
                }
            }
            ptr = tn;
        }
    }
}