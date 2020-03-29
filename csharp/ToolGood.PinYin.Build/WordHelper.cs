using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words;

namespace ToolGood.PinYin.Build
{
    internal class Dict
    {
        internal static bool TraditionalToSimplified(char t, out char s)
        {
            //if (t >= 0x4e00 && t <= 0x9FA5) {
            //    var v = Words.Dict.Simplified[t - 0x4e00];
            //    if (v!=t) {
            //        s = v;
            //        return true;
            //    }
            //}

            var ts = t.ToString();
            var tt = WordsHelper.ToSimplifiedChinese(ts);
            if (tt == ts) {
                tt = WordsHelper.ToSimplifiedChinese(ts, 1);
                if (tt == ts) {
                    tt = WordsHelper.ToSimplifiedChinese(ts, 2);
                }
            }
            if (tt != ts && tt.Length == 1) {
                s = tt[0];
                return true;
            }
            s = t;
            return false;
 
        }
    }
}
