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
        public int Count;

        public TrieNodeEx()
        {
            m_values = new Dictionary<int, TrieNodeEx>();
            Results = new List<Int32>();
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

        public int Rank(ref Int32 oneStart, ref Int32 start, bool[] seats, bool[] seats2, int[] has)
        {
            if (maxflag == 0) return 0;
            if (minflag == maxflag) {
                RankOne(ref oneStart, seats, has);
                return 0;
            }
            var keys = m_values.Select(q => (Int32)q.Key).OrderByDescending(q => q).ToList();
            var length = keys.Count - 1;
            int[] moves = new int[keys.Count - 1];
            for (int i = 1; i < keys.Count; i++) {
                moves[i - 1] = maxflag - keys[i];
            }

            while (has[start] != 0) { start++; }
            var s = start < (Int32)minflag ? (Int32)minflag : start;
            var next= s-minflag;
            var e = next+maxflag;
            while(e<has.Length){
                if(seats2[e]==false && seats[next]==false){
                    var isok=true;
                    for (int i = 0; i < keys.Count; i++)
                    {
                        var position=next+keys[i];
                        if (has[position]>0)
                        {
                            for (int j = 0; j < length; j++)
                            {
                                seats2[position+moves[j]]=true;
                            }
                            isok=false;
                            break;
                        }
                    }
                    if(isok){
                        SetSeats(next, seats, has);
                        start += keys.Count / 2;
                        Array.Clear(seats2, start, e + maxflag - start + 1 );
                        return next;
                    }
                }
                next++;
                e++;
            }
            throw new Exception("");
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
