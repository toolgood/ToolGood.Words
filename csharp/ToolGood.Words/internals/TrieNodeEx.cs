using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.internals
{
    class TrieNodeEx
    {
        internal int Char;
        internal bool End;
        internal int Index;
        internal List<Int32> Results;
        internal Dictionary<int, TrieNodeEx> m_values;
        private Int32 minflag = Int32.MaxValue;
        private Int32 maxflag = 0;
        internal Int32 Next;
        private Int32 Count;

        public TrieNodeEx()
        {
            m_values = new Dictionary<int, TrieNodeEx>();
            Results = new List<Int32>();
        }

        public bool TryGetValue(int c, out TrieNodeEx node)
        {
            if (minflag <= (Int32)c && maxflag >= (Int32)c) {
                return m_values.TryGetValue(c, out node);
            }
            node = null;
            return false;
        }
        public void Add(int c, TrieNodeEx node3)
        {
            if (minflag > c) { minflag = c; }
            if (maxflag < c) { maxflag = c; }
            m_values.Add(c, node3);
            Count++;
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

        public bool HasKey(int c)
        {
            return m_values.ContainsKey(c);
        }

        //public Int32 Rank(TrieNodeEx[] has)
        //{
        //    bool[] seats = new bool[has.Length];
        //    Int32 start = 1;

        //    has[0] = this;
        //    List<TrieNodeEx> trieNodes = new List<TrieNodeEx>() { this };

        //    while (trieNodes.Count > 0) {
        //        List<TrieNodeEx> nexts = new List<TrieNodeEx>();
        //        foreach (var node in trieNodes) {
        //            node.Rank(ref start, seats, has, nexts);
        //        }
        //        trieNodes = nexts;
        //    }
        //    Int32 maxCount = has.Length - 1;
        //    while (has[maxCount] == null) { maxCount--; }
        //    return maxCount;
        //}

        public void Rank(ref Int32 start, bool[] seats, int[] has)
        {
            if (maxflag == 0) return;
            //if (HasRank) { return; }
            //HasRank = true;
            var keys = m_values.Select(q => (Int32)q.Key).ToList();

            while (has[start] != 0) { start++; }
            var s = start < (Int32)minflag ? (Int32)minflag : start;

            for (Int32 i = s; i < has.Length; i++) {
                if (has[i] == 0) {
                    var next = i - (Int32)minflag;
                    //if (next < 0) continue;
                    if (seats[next]) continue;

                    var isok = true;
                    foreach (var item in keys) {
                        if (has[next + item] != 0) { isok = false; break; }
                    }
                    if (isok) {
                        SetSeats(next, seats, has);
                        break;
                    }
                }
            }
            start += keys.Count * keys.Count / (maxflag - minflag + 1);

            //var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);
            //foreach (var key in keys2) {
            //    nexts.Add(key.Value);
            //}
        }


        private void SetSeats(Int32 next, bool[] seats, int[] has)
        {
            Next = next;
            seats[next] = true;

            foreach (var item in m_values) {
                var position = next + item.Key;
                has[position] = item.Value.Index;
            }

        }


    }

}
