using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words
{
    public class TrieSearch
    {
        class TrieNode
        {
            public bool m_end;
            public Dictionary<char, TrieNode> m_values;
            private uint minflag = uint.MaxValue;
            private uint maxflag = uint.MinValue;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
            }

            public virtual bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (uint)c && maxflag >= (uint)c) {
                    return m_values.TryGetValue(c, out node);
                }
                node = null;
                return false;
            }

            public TrieNode Add(char c)
            {
                if (minflag > c) { minflag = c; }
                if (maxflag < c) { maxflag = c; }

                TrieNode subnode;
                if (!m_values.TryGetValue(c, out subnode)) {
                    subnode = new TrieNode();
                    m_values.Add(c, subnode);
                }
                return subnode;
            }

        }

        TrieNode _root = new TrieNode();

        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(string key)
        {
            if (string.IsNullOrEmpty(key)) {
                return;
            }
            TrieNode node = _root;
            for (int i = 0; i < key.Length; i++) {
                char c = key[i];
                node = node.Add(c);
            }
            node.m_end = true;
        }

        /// <summary>
        /// 检查是否包含非法字符
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>找到的第1个非法字符.没有则返回string.Empty</returns>
        public bool HasBadWord(string text)
        {

            for (int head = 0; head < text.Length; head++) {
                int index = head;
                TrieNode node = _root;
                while (node.TryGetValue(text[index], out node)) {
                    if (node.m_end) {
                        return true;
                    }
                    if (text.Length == ++index) {
                        break;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否包含非法字符
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>找到的第1个非法字符.没有则返回string.Empty</returns>
        public string FindFirst(string text)
        {

            for (int head = 0; head < text.Length; head++) {
                int index = head;
                TrieNode node = _root;
                while (node.TryGetValue(text[index], out node)) {
                    if (node.m_end) {
                        return text.Substring(head, index - head + 1);
                    }
                    if (text.Length == ++index) {
                        break;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 查找所有非法字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<string> FindAll(string text)
        {

            List<string> result = new List<string>();
            for (int head = 0; head < text.Length; head++) {
                int index = head;
                TrieNode node = _root;
                while (node.TryGetValue(text[index], out node)) {
                    if (node.m_end) {
                        result.Add(text.Substring(head, index - head + 1));
                    }
                    if (text.Length == ++index) {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 替换非法字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="mask">用于代替非法字符</param>
        /// <returns>替换后的字符串</returns>
        public string Replace(string text, char mask = '*')
        {
            char[] chars = null;
            for (int head = 0; head < text.Length; head++) {
                int index = head;
                TrieNode node = _root;
                while (node.TryGetValue(text[index], out node)) {
                    if (node.m_end) {
                        if (chars == null) chars = text.ToArray();
                        for (int i = head; i <= index; i++) {
                            chars[i] = mask;
                        }
                        head = index;
                    }
                    if (text.Length == ++index) {
                        break;
                    }
                }
            }
            return chars == null ? text : new string(chars);
        }
    }
}
