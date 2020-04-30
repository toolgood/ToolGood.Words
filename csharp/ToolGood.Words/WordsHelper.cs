using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
 
    public static class WordsHelper
    {
        #region 拼音 操作
        /// <summary>
        /// 获取所有拼音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="c">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static List<string> GetAllPinyin(char c, bool tone = false)
        {
            return PinyinDict.GetAllPinyin(c, tone ? 1 : 0);
        }

        /// <summary>
        /// 获取拼音全拼, 不支持多音,中文字符集为[0x3400,0x9FD5]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        [Obsolete("请使用GetPinyin方法，此方法不支持多音")]
        public static string GetPinyinFast(string text, bool tone = false)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++) {
                var c = text[i];
                sb.Append(PinyinDict.GetPinyinFast(c, tone ? 1 : 0));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取首字母，中文字符集为[0x3400,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <returns></returns>
        public static string GetFirstPinyin(string text)
        {
            return PinyinDict.GetFirstPinyin(text, 0);
        }


        /// <summary>
        /// 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string GetPinyin(string text, bool tone = false)
        {
            return string.Join("", PinyinDict.GetPinyinList(text, tone ? 1 : 0));
        }

        /// <summary>
        /// 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="splitSpan">分隔符</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string GetPinyin(string text,string splitSpan, bool tone = false)
        {
            return string.Join(splitSpan, PinyinDict.GetPinyinList(text, tone ? 1 : 0));
        }


        /// <summary>
        /// 获取拼音全拼,支持多音,中文字符集为[0x4E00,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string[] GetPinyinList(string text, bool tone = false)
        {
            return PinyinDict.GetPinyinList(text, tone ? 1 : 0);
        }



        /// <summary>
        /// 获取姓名拼音,中文字符集为[0x3400,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string GetPinyinForName(string name, bool tone = false)
        {
            return string.Join("", PinyinDict.GetPinyinForName(name, tone ? 1 : 0));
        }

        /// <summary>
        /// 获取姓名拼音,中文字符集为[0x3400,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="splitSpan">分隔符</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static string GetPinyinForName(string name, string splitSpan, bool tone = false)
        {
            return string.Join(splitSpan, PinyinDict.GetPinyinForName(name, tone ? 1 : 0));
        }

        /// <summary>
        /// 获取姓名拼音,中文字符集为[0x3400,0x9FD5],[0x20000-0x2B81D]，注：偏僻汉字很多未验证
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="tone">是否带声调</param>
        /// <returns></returns>
        public static List<string> GetPinyinListForName(string name, bool tone = false)
        {
            return PinyinDict.GetPinyinForName(name, tone ? 1 : 0);
        }

        #endregion

        #region 判断输入是否为中文
        /// <summary>
        /// 判断输入是否为中文  ,中文字符集为[0x4E00,0x9FA5][0x3400,0x4db5]
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool HasChinese(string content)
        {
            if (Regex.IsMatch(content, @"[\u3400-\u4db5\u4e00-\u9fd5]")) {
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 判断输入是否全为中文,中文字符集为[0x4E00,0x9FA5][0x3400,0x4db5]
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsAllChinese(string content)
        {
            if (Regex.IsMatch(content, @"^[\u3400-\u4db5\u4e00-\u9fd5]*$")) {
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



    }
}
