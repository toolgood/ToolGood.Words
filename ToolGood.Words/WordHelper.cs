using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace ToolGood.Words
{
    public static class WordHelper
    {
        /// <summary>
        /// 判断输入是否为中文  
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool HasChinese(string content)
        {
            if (Regex.IsMatch(content, @"[\u4e00-\u9fa5]")) {
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 获取首字母
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetFirstPinYin(string text)
        {
            var ts = text.ToArray();
            for (int i = 0; i < ts.Length; i++) {
                ts[i] = Dict.GetFirstPinYin(ts[i]);
            }
            return new string(ts);
        }

        /// <summary>
        /// 获取拼音全拼
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetPinYin(string text)
        {
            return PinYinConverter.Get(text);
        }


        /// <summary>
        /// 数字转中文大写
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ToChineseRMB(double x)
        {
            string s = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        /// <summary>
        /// 转成 侦测字符串
        /// 1、转小写;2、全角转半角;3、相似文字修改
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static string ToSenseWord(string s)
        {
            var a = "āáǎàōóǒòēéěèīíǐìūúǔùǖǘǚǜüńňêɑɡ①②③④⑤⑥⑦⑧⑨㈠㈡㈢㈣㈤㈥㈦㈧㈨⒈⒉⒊⒋⒌⒍⒎⒏⒐ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅰⅱⅲⅳⅴⅵⅶⅷⅸ";
            var b = "aaaaooooeeeeiiiiuuuuuuuuunnneamg123456789123456789123456789123456789123456789";

            s = ToSimplifiedChinese(s);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++) {
                var c = s[i];
                if ('A' <= c && c <= 'Z') { sb.Append((char)(c | 0x20)); continue; }
                if (c == 12288) { sb.Append(' '); continue; }
                if (c > 65280 && c < 65375) { sb.Append((char)(c - 65248)); continue; }

                var index = a.IndexOf(c);
                if (index > -1) { sb.Append(b[index]); continue; }

                sb.Append(c);
            }

            return sb.ToString();
        }

        #region 半角 全角 转换
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++) {
                if (c[i] == 32) {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 转半角的函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++) {
                if (c[i] == 12288) {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        #endregion

        #region 繁体 简体 转换
        /// <summary>
        /// 转繁体中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToTraditionalChinese(string text)
        {
            var ts = text.ToArray();
            for (int i = 0; i < ts.Length; i++) {
                char value;
                if (Dict.SimplifiedToTraditional(ts[i], out value)) {
                    ts[i] = value;
                }
            }
            return new string(ts);
        }
        /// <summary>
        /// 转简体中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSimplifiedChinese(string text)
        {
            var ts = text.ToArray();
            for (int i = 0; i < ts.Length; i++) {
                char value;
                if (Dict.TraditionalToSimplified(ts[i], out value)) {
                    ts[i] = value;
                }
            }
            return new string(ts);
        }
        #endregion
    }
}
