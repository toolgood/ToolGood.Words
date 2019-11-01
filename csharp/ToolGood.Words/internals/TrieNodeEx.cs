using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    class TrieNodeEx
    {
        public TrieNodeEx Parent;
        public TrieNodeEx Failure;
        public char Char;
        internal bool End;
        internal List<Int32> Results;
        internal Dictionary<char, TrieNodeEx> m_values;
        internal Dictionary<char, TrieNodeEx> merge_values;
        private Int32 minflag = Int32.MaxValue;
        private Int32 maxflag = 0;
        internal Int32 Next;
        private Int32 Count;

        public TrieNodeEx()
        {
            m_values = new Dictionary<char, TrieNodeEx>();
            merge_values = new Dictionary<char, TrieNodeEx>();
            Results = new List<Int32>();
        }

        public bool TryGetValue(char c, out TrieNodeEx node)
        {
            if (minflag <= (Int32)c && maxflag >= (Int32)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }

        public TrieNodeEx Add(char c)
        {
            TrieNodeEx node;

            if (m_values.TryGetValue(c, out node)) {
                return node;
            }

            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }

            node = new TrieNodeEx();
            node.Parent = this;
            node.Char = c;
            m_values[c] = node;
            Count++;
            return node;
        }

        public void SetResults(Int32 text)
        {
            if (End == false) {
                End = true;
            }
            if (Results.Contains(text) == false) {
                Results.Add(text);
            }
        }

        public void Merge(TrieNodeEx node)
        {
            var nd = node;
            while (nd.Char != 0) {
                foreach (var item in node.m_values) {
                    if (m_values.ContainsKey(item.Key) == false) {
                        if (merge_values.ContainsKey(item.Key) == false) {
                            if (minflag > item.Key) { minflag = item.Key; }
                            if (maxflag < item.Key) { maxflag = item.Key; }
                            merge_values[item.Key] = item.Value;
                            Count++;
                        }
                    }
                }
                nd = nd.Failure;
            }
        }

        public Int32 Rank(TrieNodeEx[] has)
        {
            bool[] seats = new bool[has.Length];
            Int32 start = 1;

            has[0] = this;

            Rank(ref start, seats, has);
            Int32 maxCount = has.Length - 1;
            while (has[maxCount] == null) { maxCount--; }
            return maxCount;
        }

        private void Rank(ref Int32 start, bool[] seats, TrieNodeEx[] has)
        {
            if (maxflag == 0) return;
            var keys = m_values.Select(q => (Int32)q.Key).ToList();
            keys.AddRange(merge_values.Select(q => (Int32)q.Key).ToList());

            while (has[start] != null) { start++; }
            var s = start < (Int32)minflag ? (Int32)minflag : start;

            for (Int32 i = s; i < has.Length; i++) {
                if (has[i] == null) {
                    var next = i - (Int32)minflag;
                    //if (next < 0) continue;
                    if (seats[next]) continue;

                    var isok = true;
                    foreach (var item in keys) {
                        if (has[next + item] != null) { isok = false; break; }
                    }
                    if (isok) {
                        SetSeats(next, seats, has);
                        break;
                    }
                }
            }
            start += keys.Count / 2;

            var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);
            foreach (var key in keys2) {
                key.Value.Rank(ref start, seats, has);
            }
        }


        private void SetSeats(Int32 next, bool[] seats, TrieNodeEx[] has)
        {
            Next = next;
            seats[next] = true;

            foreach (var item in merge_values) {
                var position = next + item.Key;
                has[position] = item.Value;
            }

            foreach (var item in m_values) {
                var position = next + item.Key;
                has[position] = item.Value;
            }

        }


    }

}
