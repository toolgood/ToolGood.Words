using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;


namespace ToolGood.Words
{
    public class StringSearch: BaseSearch
    {
        public string FindFirst(string text)
        {
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        return tn.Results[0];
                    }
                }
                ptr = tn;
            }
            return null;
        }

        public List<string> FindAll(string text)
        {
            TrieNode ptr = null;
            List<string> list = new List<string>();

            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        foreach (var item in tn.Results) {
                            list.Add(item);
                        }
                    }
                }
                ptr = tn;
            }
            return list;
        }

        public bool ContainsAny(string text)
        {
            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        return true;
                    }
                }
                ptr = tn;
            }
            return false;
        }

        public string Replace(string text,char replaceChar='*')
        {
            StringBuilder result = new StringBuilder(text);

            TrieNode ptr = null;
            for (int i = 0; i < text.Length; i++) {
                TrieNode tn;
                if (ptr == null) {
                    tn = _first[text[i]];
                } else {
                    if (ptr.TryGetValue(text[i], out tn) == false) {
                        tn = _first[text[i]];
                    }
                }
                if (tn != null) {
                    if (tn.End) {
                        var MaxLength = 0;
                        for (int j = 0; j < tn.Results.Count; j++) {
                            if (tn.Results[j].Length > MaxLength) {
                                MaxLength = tn.Results[j].Length;
                            }
                        }

                        var start = i + 1 - MaxLength;
                        for (int j = start; j <= i; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
                ptr = tn;
            }
            return result.ToString();
        }

    }
}
