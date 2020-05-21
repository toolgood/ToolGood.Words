//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ToolGood.Words.internals
//{
//    class TrieNode4Ex
//    {
//        public int Index;
//        public bool End;
//        public List<int> Results;
//        public Dictionary<char, TrieNode4Ex> m_values;
//        public ushort minflag = ushort.MaxValue;
//        public ushort maxflag = ushort.MinValue;


//        public Dictionary<char, TrieNode4Ex> m_values2;
//        public ushort minflag2 = ushort.MaxValue;
//        public ushort maxflag2 = ushort.MinValue;


//        public TrieNode4Ex()
//        {
//            Results = new List<int>();
//            m_values = new Dictionary<char, TrieNode4Ex>();
//            m_values2 = new Dictionary<char, TrieNode4Ex>();
//        }

//        public void Add(char c, TrieNode4Ex node3)
//        {
//            if (minflag > c) { minflag = c; }
//            if (maxflag < c) { maxflag = c; }
//            m_values.Add(c, node3);
//        }

//        public void Add2(char c, TrieNode4Ex node3)
//        {
//            if (minflag2 > c) { minflag2 = c; }
//            if (maxflag2 < c) { maxflag2 = c; }
//            m_values2.Add(c, node3);
//        }

//        public void SetResults(int index)
//        {
//            if (End == false) {
//                End = true;
//            }
//            if (Results.Contains(index) == false) {
//                Results.Add(index);
//            }
//        }

//        public bool HasKey(char c)
//        {
//            return m_values.ContainsKey(c);
//        }


//    }
//}
