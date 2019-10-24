using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Contrast
{
    class WordTest
    {
        #region ToSenseWord
        public static string ToSenseWord1(string s)
        {
            StringBuilder ts = new StringBuilder(s);
            for (int i = 0; i < ts.Length; i++) {
                var c = (int)ts[i];
                if ('A' <= c && c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(ts[i]);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    ts[i] = Dict.Simplified[c - 0x4e00];
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord2(string s)
        {
            StringBuilder ts = new StringBuilder(s);
            for (int i = 0; i < ts.Length; i++) {
                var c = (int)ts[i];
                if ('A' <= c && c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(ts[i]);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    var k = Dict.Simplified[c - 0x4e00];
                    if (k != c) {
                        ts[i] = k;
                    }
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord3(string s)
        {
            StringBuilder ts = new StringBuilder(s);
            for (int i = 0; i < ts.Length; i++) {
                var c = ts[i];
                if ('A' <= c && c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(c);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    ts[i] = Dict.Simplified[c - 0x4e00];
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord4(string s)
        {
            var ts = s.ToArray();
            for (int i = 0; i < ts.Length; i++) {
                var c = (int)ts[i];
                if ('A' <= c && c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(ts[i]);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    var k = Dict.Simplified[c - 0x4e00];
                    if (k != c) {
                        ts[i] = k;
                    }
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord5(string s)
        {
            StringBuilder ts = new StringBuilder(s);
            for (int i = 0; i < ts.Length; i++) {
                var c = (int)ts[i];
                if (c < 'A') { } else if (c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(ts[i]);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    var k = Dict.Simplified[c - 0x4e00];
                    if (k != c) {
                        ts[i] = k;
                    }
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord6(string s)
        {
            StringBuilder ts = new StringBuilder(s);
            for (int i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c < 'A') { } else if (c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(c);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    var k = Dict.Simplified[c - 0x4e00];
                    if (k != c) {
                        ts[i] = k;
                    }
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord7(string s)
        {
            StringBuilder ts = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c < 'A') { ts.Append(c); } else if (c <= 'Z') {
                    ts.Append((char)(c | 0x20));
                } else if (c < 9450) { ts.Append(c); } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(ts[i]);
                    if (index > -1) {
                        ts.Append(Dict.nums2[index]);
                    } else {
                        ts.Append(c);
                    }
                } else if (c == 12288) {
                    ts.Append(' ');
                } else if (c < 0x4e00) { ts.Append(c); } else if (c <= 0x9fa5) {
                    var k = Dict.Simplified[c - 0x4e00];
                    if (k != c) {
                        ts.Append(k);
                    } else {
                        ts.Append(c);
                    }
                } else if (c < 65280) { ts.Append(c); } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts.Append(k);
                } else {
                    ts.Append(c);
                }
            }
            return ts.ToString();
        }
        public static string ToSenseWord8(string s)
        {
            var ts = s.ToArray();
            for (int i = 0; i < ts.Length; i++) {
                var c = (int)ts[i];
                if ('A' <= c && c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = Dict.nums1.IndexOf(ts[i]);
                    if (index > -1) { ts[i] = Dict.nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    var k = Dict.Simplified[c - 0x4e00];
                    if (k != c) {
                        ts[i] = k;
                    }
                } else if (c < 65280) { } else if (c < 65375) {
                    var k = (c - 65248);
                    if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                    ts[i] = (char)k;
                }
            }
            return new string(ts);
        }
        public unsafe static string ToSenseWord10(string text)
        {
            fixed (char* newText = text) {
                char* itor = newText;
                char* end = newText + text.Length;
                char c;

                while (itor < end) {
                    c = *itor;

                    if (c < 'A') { } else if (c <= 'Z') {
                        *itor = (char)(c | 0x20);
                    } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                        var index = Dict.nums1.IndexOf(c);
                        if (index > -1) { *itor = Dict.nums2[index]; }
                    } else if (c == 12288) {
                        *itor = ' ';
                    } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                        var k = Dict.Simplified[c - 0x4e00];
                        if (k != c) {
                            *itor = k;
                        }
                    } else if (c < 65280) { } else if (c < 65375) {
                        var k = (c - 65248);
                        if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                        *itor = (char)k;
                    }
                    ++itor;
                }
            }

            return text;
        }
        public unsafe static string ToSenseWord9(string text)
        {
            fixed (char* newText = text) {
                char* itor = newText;
                char* end = newText + text.Length;
                char c;

                while (itor < end) {
                    c = *itor;

                    if ('A' <= c && c <= 'Z') {
                        *itor = (char)(c | 0x20);
                    } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                        var index = Dict.nums1.IndexOf(c);
                        if (index > -1) { *itor = Dict.nums2[index]; }
                    } else if (c == 12288) {
                        *itor = ' ';
                    } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                        var k = Dict.Simplified[c - 0x4e00];
                        if (k != c) {
                            *itor = k;
                        }
                    } else if (c < 65280) { } else if (c < 65375) {
                        var k = (c - 65248);
                        if ('A' <= k && k <= 'Z') { k = k | 0x20; }
                        *itor = (char)k;
                    }
                    ++itor;
                }
            }

            return text;
        }

        #endregion

        #region RemoveNontext
        const string bitType = "00000000000000000000000000000000000000000000000011111111110000000zzzzzzzzzzzzzzzzzzzzzzzzzz000000zzzzzzzzzzzzzzzzzzzzzzzzzz00000";
        public static bool[] GetDisablePostion1(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var b = getType(text[1]);
            var some = getType(text[0]) == b;
            for (int i = 2; i < text.Length; i++) {
                var c = getType(text[i]);
                var some2 = b == c;
                if (some && some2 && b != 0) {
                    bs[i] = true;
                }
                b = c;
                some = some2;
            }
            return bs;
        }
        public static bool[] GetDisablePostion2(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var a = getType(text[0]);
            var b = getType(text[1]);
            for (int i = 2; i < text.Length; i++) {
                var c = getType(text[i]);
                if (a == b && b == c && b != 0) {
                    bs[i] = true;
                }
                a = b;
                b = c;
            }
            return bs;
        }
        public static BitArray GetDisablePostion3(string text)
        {
            BitArray ba = new BitArray(text.Length);
            var a = getType(text[0]);
            var b = getType(text[1]);
            for (int i = 2; i < text.Length; i++) {
                var c = getType(text[i]);
                if (a == b && b == c && b != 0) {
                    ba[i] = true;
                }
                a = b;
                b = c;
            }
            return ba;
        }

        public static bool[] GetDisablePostion4(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var a = text[0] < 127 ? bitType[text[0]] : '0';
            var b = text[1] < 127 ? bitType[text[1]] : '0';

            for (int i = 2; i < text.Length; i++) {
                var c0 = text[i];
                if (c0 < 127) {
                    var c = bitType[c0];
                    if (a == b && b == c && b != '0') {
                        bs[i] = true;
                    }
                    a = b;
                    b = c;
                } else {
                    a = b;
                    b = '0';
                }
            }
            return bs;
        }

        public static bool[] GetDisablePostion5(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 3) return bs;
            var a = text[0] < 127 ? bitType[text[0]] : '0';
            var b = text[1] < 127 ? bitType[text[1]] : '0';

            var i = 2;
            while (i < text.Length) {
                var c = text[i] < 127 ? bitType[text[i]] : '0';
                if (b == '0') {
                    a = c;
                    i++;
                    b = text[i] < 127 ? bitType[text[i]] : '0';
                } else if (a == b && b == c) {
                    bs[i] = true;
                    a = b;
                    b = c;
                }
                i++;
            }
            return bs;
        }

        public static bool[] GetDisablePostion6(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var a = text[0] < 127 ? bitType[text[0]] : '0';
            var b = text[1] < 127 ? bitType[text[1]] : '0';

            for (int i = 2; i < text.Length; i++) {
                if (b != '0') {
                    var c0 = text[i];
                    if (c0 < 127) {
                        var c = bitType[c0];
                        if (a == b && b == c && b != '0') {
                            bs[i] = true;
                        }
                        a = b;
                        b = c;
                    } else {
                        a = b;
                        b = '0';
                    }
                } else {
                    a = '0';
                    b = text[i] < 127 ? bitType[text[i]] : '0';
                }
            }
            return bs;
        }

        public static bool[] GetDisablePostion7(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var a = text[0] < 127 ? bitType[text[0]] : '0';
            for (int i = 1; i < text.Length; i++) {
                var c0 = text[i];
                if (c0 < 127) {
                    var c = bitType[c0];
                    if (c == a && c != '0') {
                        bs[i] = true;
                    } else {
                        if (bs[i - 1] != false) {
                            bs[i - 1] = false;
                        }
                    }
                    a = c;
                } else {
                    if (bs[i - 1] != false) {
                        bs[i - 1] = false;
                    }
                    a = '0';
                }
            }
            return bs;
        }

        public static bool[] GetDisablePostion8(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var a = text[0] < 127 ? bitType[text[0]] : '0';
            var b = text[1] < 127 ? bitType[text[1]] : '0';

            for (int i = 2; i < text.Length; i++) {
                var c0 = text[i];
                if (c0 < 127) {
                    var c = bitType[c0];
                    int d = a + b + c;
                    if (d == 147 || d == 366) {//if (a == b && b == c && b != '0') 
                        bs[i] = true;
                    }
                    a = b;
                    b = c;
                } else {
                    a = b;
                    b = '0';
                }
            }
            return bs;
        }

        public static bool[] GetDisablePostion9(string text)
        {
            bool[] bs = new bool[text.Length];
            if (text.Length < 2) return bs;
            var a0 = text[0];
            var a = a0 < 127 ? bitType[a0] : '0';
            var b0 = text[1];
            var b = b0 < 127 ? bitType[b0] : '0';

            int i = 2;
            while (i < text.Length) {
                var c0 = text[i];
                if (c0 < 127) {
                    var c = bitType[c0];
                    int d = a + b + c;
                    if (d == 147 || d == 366) {
                        bs[i] = true;
                    }
                    a = b;
                    b = c;
                } else {
                    a = b;
                    b = '0';
                }
                i++;
            }
            return bs;
        }



        public static byte getType(char c)
        {
            if (c < '0') { return 0; } else if (c <= '9') {
                return 1;
            } else if (c < 'A') { return 0; } else if (c <= 'Z') {
                return 2;
            } else if (c < 'a') { return 0; } else if (c <= 'z') {
                return 2;
            }
            return 0;
        }


        #endregion


    }
}
