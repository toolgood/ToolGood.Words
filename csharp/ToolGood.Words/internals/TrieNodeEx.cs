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

        public void Rank(ref Int32 oneStart, ref Int32 start, bool[] seats, bool[] seats2, int[] has)
        {
            if (maxflag == 0) return;
            if (minflag == maxflag) {
                RankOne(ref oneStart, seats, has);
                return;
            }
            var keys = m_values.Select(q => (Int32)q.Key).OrderByDescending(q => q).ToList();
            var length = keys.Count - 1;
            int[] moves = new int[keys.Count - 1];
            for (int i = 1; i < keys.Count; i++) {
                moves[i - 1] = maxflag - keys[i];
            }

            while (has[start] != 0) { start++; }
            var s = start < (Int32)minflag ? (Int32)minflag : start;

            for (int i = s; i < s + (maxflag - minflag); i++) {
                if (has[i] != 0) {
                    for (int j = 0; j < length; j++) {
                        var p = i + moves[j];
                        if (seats2[p] == false) {
                            seats2[p] = true;
                        }
                    }
                }
            }
            var max = 0;
            for (int i = s + (maxflag - minflag); i < has.Length; i++) {
                if (has[i] == 0) {
                    if (seats2[i]) { continue; }
                    var next = i - (Int32)maxflag;
                    if (seats[next]) continue;
                    SetSeats(next, seats, has);
                    max = i;
                    break;
                } else {
                    for (int j = 0; j < length; j++) {
                        var p = i + moves[j];
                        if (seats2[p] == false) {
                            seats2[p] = true;
                        }
                    }
                }
            }
            start += keys.Count / 2;
            Array.Clear(seats2, start, max + maxflag - start + 1);
        }

        private void RankOne(ref int start, bool[] seats, int[] has)
        {
            while (has[start] != 0) { start++; }
            var s = start < (Int32)minflag ? (Int32)minflag : start;

            for (Int32 i = s; i < has.Length; i++) {
                if (has[i] == 0) {
                    var next = i - (Int32)minflag;
                    if (seats[next]) continue;
                    SetSeats(next, seats, has);
                    break;
                }
            }
            start++;
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
