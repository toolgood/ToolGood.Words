using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Words.internals;





namespace ToolGood.Words
{
    public static class WordsHelper
    {
        #region 拼音 操作
        /// <summary>
        /// 获取首字母，中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <returns></returns>
        public static string GetFirstPinYin(string text)
        {
            return PinYinDict.GetFirstPinYin(text, 0);
        }

        /// <summary>
        /// 获取拼音全拼, 不支持多音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        [Obsolete("请使用GetPinYin方法，此方法不支持多音")]
        public static string GetPinYinFast(string text, bool tone = false)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++) {
                var c = text[i];
                sb.Append(PinYinDict.GetPinYinFast(c, tone ? 1 : 0));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5]
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string GetPinYin(string text, bool tone = false)
        {
            return PinYinDict.GetPinYin(text, tone ? 1 : 0);
        }

        /// <summary>
        /// 获取所有拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="c">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static List<string> GetAllPinYin(char c, bool tone = false)
        {
            return PinYinDict.GetAllPinYin(c, tone ? 1 : 0);
        }

        /// <summary>
        /// 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string GetPinYinForName(string name, bool tone = false)
        {
            return string.Join("", PinYinDict.GetPinYinForName(name, tone ? 1 : 0));
        }

        /// <summary>
        /// 获取姓名拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static List<string> GetPinYinListForName(string name, bool tone = false)
        {
            return PinYinDict.GetPinYinForName(name, tone ? 1 : 0);
        }

        #endregion

        #region 字符串 转成 脏词检测字符串
        /// <summary>
        /// 转成 侦测字符串
        /// 1、转小写;2、全角转半角; 3、相似文字修改；4、繁体转简体
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSenseIllegalWords(string s)
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

        internal static string RemoveNontext(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            for (int i = 0; i < text.Length; i++) {
                var c = text[i];
                bool remove = true;

                if (c == ' ') {
                    remove = false;
                } else if (c < 2) {
                    remove = false;
                } else if (c < '0') { } else if (c <= '9') {
                    remove = false;
                } else if (c < 'a') { } else if (c <= 'z') {
                    remove = false;
                } else if (c < 0x4e00) { } else if (c <= 0x9fa5) {
                    remove = false;
                }
                if (remove) {
                    sb[i] = (char)1;
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 判断输入是否为中文
        /// <summary>
        /// 判断输入是否为中文  ,中文字符集为[0x4E00,0x9FA5]
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
        /// 判断输入是否全为中文,中文字符集为[0x4E00,0x9FA5]
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsAllChinese(string content)
        {
            if (Regex.IsMatch(content, @"^[\u4e00-\u9fa5]*$")) {
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 判断含有英语
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool HasEnglish(string content)
        {
            if (Regex.IsMatch(content, @"[A-Za-z]")) {
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 判断是否全部英语
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsAllEnglish(string content)
        {
            if (Regex.IsMatch(content, @"^[A-Za-z]*$")) {
                return true;
            } else {
                return false;
            }
        }

        #endregion

        #region 半角 全角 转换
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            StringBuilder sb = new StringBuilder(input);
            for (int i = 0; i < input.Length; i++) {
                var c = input[i];
                if (c == 32) {
                    sb[i] = (char)12288;
                } else if (c < 127) {
                    sb[i] = (char)(c + 65248);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 转半角的函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            StringBuilder sb = new StringBuilder(input);
            for (int i = 0; i < input.Length; i++) {
                var c = input[i];
                if (c == 12288) {
                    sb[i] = (char)32;
                } else if (c > 65280 && c < 65375) {
                    sb[i] = (char)(c - 65248);
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 繁体 简体 转换


        /// <summary>
        /// 转繁体中文
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type">0、繁体中文，1、港澳繁体，2、台湾正体 </param>
        /// <returns></returns>
        public static string ToTraditionalChinese(string text, int type = 0)
        {
            return Translate.ToTraditionalChinese(text, type);
        }

        /// <summary>
        /// 转简体中文
        /// </summary>
        /// <param name="text"></param>
        /// <param name="srcType">0、繁体中文，1、港澳繁体，2、台湾正体</param>
        /// <returns></returns>
        public static string ToSimplifiedChinese(string text, int srcType = 0)
        {
            return Translate.ToSimplifiedChinese(text, srcType);
        }
        /// <summary>
        /// 清理 简繁转换 缓存
        /// </summary>
        public static void ClearTranslate()
        {
            Translate.ClearTranslate();
        }


        #endregion

        #region 数字转中文大写
        /// <summary>
        /// 数字转中文大写
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ToChineseRMB(double x)
        {
            if (x == 0.0) { return "零元"; }
            string s = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        /// <summary>
        /// 中文转数字（支持中文大写）
        /// </summary>
        /// <param name="chineseString"></param>
        /// <returns></returns>
        public static decimal ToNumber(string chineseString)
        {
            return NumberConventer.ChnToArab(chineseString);
        }
        #endregion

        #region 转成数字
        /// <summary>
        /// 【中文】、【符号】，转成【数字】字符串
        /// </summary>
        /// <param name="chineseString"></param>
        /// <returns></returns>
        public static string TransitionToNumberString(string chineseString)
        {
            var str = new StringBuilder();
            Dictionary<char, char> dictionary = new Dictionary<char, char>();
            for (int i = 0; i < Dict.nums1.Length; i++) {
                dictionary[Dict.nums1[i]] = Dict.nums2[i];
            }

            for (int i = 0; i < chineseString.Length; i++) {
                var c = chineseString[i];
                char outc;
                if (dictionary.TryGetValue(c, out outc)) {
                    str.Append(outc);
                } else {
                    str.Append(c);
                }
            }
            return str.ToString();
        }

        #endregion

    }
}
