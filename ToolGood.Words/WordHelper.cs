using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        /// 获取拼音全拼  不支持多音
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        //[Obsolete("请使用GetPinYin方法，此方法不支持多音")]
        public static string GetPinYinFirst(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++) {
                var c = text[i];
                sb.Append(PinYinDict.GetFirstPinYin(c));
            }
            return sb.ToString();
        }

        ///// <summary>
        ///// 获取拼音全拼 支持多音
        ///// </summary>
        ///// <param name="text"></param>
        ///// <returns></returns>
        //public static string GetPinYin(string text)
        //{
        //    return PinYinDict.GetPinYin(text);
        //}

        /// <summary>
        /// 获取所有拼音
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<string> GetAllPinYin(char s)
        {
            return PinYinDict.GetAllPinYin(s);
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

        const string nums1 = "⓪①②③④⑤⑥⑦⑧⑨⑴⑵⑶⑷⑸⑹⑺⑻⑼⒈⒉⒊⒋⒌⒍⒎⒏⒐❶❷❸❹❺❻❼❽❾➀➁➂➃➄➅➆➇➈➊➋➌➍➎➏➐➑➒㈠㈡㈢㈣㈤㈥㈦㈧㈨㊀㊁㊂㊃㊄㊅㊆㊇㊈";
        const string nums2 = "0123456789123456789123456789123456789123456789123456789123456789";

        /// <summary>
        /// 转成 侦测字符串
        /// 1、转小写;2、全角转半角;3、相似文字修改
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSenseWord(string s)
        {
            StringBuilder ts = new StringBuilder(s);
            for (int i = 0; i < ts.Length; i++) {
                var c = ts[i];
                if ('A' <= c && c <= 'Z') {
                    ts[i] = (char)(c | 0x20);
                } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
                    var index = nums1.IndexOf(c);
                    if (index > -1) { ts[i] = nums2[index]; }
                } else if (c == 12288) {
                    ts[i] = ' ';
                } else if (c < 0x4e00) { } else if (c <= 0x9fff) {
                    char value;
                    if (Dict.TraditionalToSimplified(ts[i], out value)) { ts[i] = value; }
                } else if (c < 65280) { } else if (c < 65375) {
                    ts[i] = (char)(c - 65248);
                }

                //if ('A' <= c && c <= 'Z') {
                //    ts[i] = (char)(c | 0x20);
                //} else if (c == 12288) {
                //    ts[i] = ' ';

                //} else if (c > 65280 && c < 65375) {
                //    ts[i] = (char)(c - 65248);
                //} else if (c >= 0x4e00 && c <= 0x9fff) {
                //    char value;
                //    if (Dict.TraditionalToSimplified(ts[i], out value)) { ts[i] = value; }
                //} else if (c >= 9450 && c <= 12840) {//处理数字 
                //    var index = nums1.IndexOf(c);
                //    if (index > -1) { ts[i] = nums2[index]; }
                //}
            }
            return ts.ToString();
        }

        //internal static char getSenseChar(char c)
        //{
        //    if ('A' <= c && c <= 'Z') {
        //        return (char)(c | 0x20);
        //    } else if (c < 9450) { } else if (c <= 12840) {//处理数字 
        //        var index = nums1.IndexOf(c);
        //        if (index > -1) { return nums2[index]; }
        //    } else if (c == 12288) {
        //        return ' ';
        //    } else if (c < 0x4e00) { } else if (c <= 0x9fff) {
        //        char value;
        //        if (Dict.TraditionalToSimplified(c, out value)) {
        //            return value;
        //        }
        //    } else if (c < 65280) { } else if (c < 65375) {
        //        return (char)(c - 65248);
        //    }
        //    return c;
        //}


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
