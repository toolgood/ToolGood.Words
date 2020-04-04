using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    //public internal class Dict
    class Dict
    {
        internal static string _Simplified;
        public static string Simplified {
            get {
                if (_Simplified == null) {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    var dict1 = Translate.GetTransformationDict("t2hk.dat");
                    var dict2 = Translate.GetTransformationDict("t2tw.dat");
                    var dict3 = Translate.GetTransformationDict("t2s.dat");
                    foreach (var item in dict1) {
                        if (item.Key.Length > 1 || item.Value.Length > 1) { continue; }
                        dict[item.Value] = item.Key;
                    }
                    foreach (var item in dict2) {
                        if (item.Key.Length > 1 || item.Value.Length > 1) { continue; }
                        dict[item.Value] = item.Key;
                    }

                    var str2 = "";
                    for (int i = 0x4e00; i <= 0x9fa5; i++) {
                        var m = ((char)i).ToString();
                        if (dict3.TryGetValue(m, out string v2)) {
                            if (v2.Length == 1) {
                                str2 += v2;
                            } else {
                                str2 += m;
                            }
                        } else {
                            if (dict.TryGetValue(m, out string v)) {
                                m = v;
                            }
                            if (dict3.TryGetValue(m, out string v3)) {
                                m = v3;
                            }
                            str2 += m;
                        }
                    }
                    _Simplified = str2;
                }
                return _Simplified;
            }
        }



        public const string nums1 = "⓪０零º₀⓿○" +
                                    "１２３４５６７８９" +
                                    "一二三四五六七八九" +
                                    "壹贰叁肆伍陆柒捌玖" +
                                    "¹²³⁴⁵⁶⁷⁸⁹" +
                                    "₁₂₃₄₅₆₇₈₉" +
                                    "①②③④⑤⑥⑦⑧⑨" +
                                    "⑴⑵⑶⑷⑸⑹⑺⑻⑼" +
                                    "⒈⒉⒊⒋⒌⒍⒎⒏⒐" +
                                    "❶❷❸❹❺❻❼❽❾" +
                                    "➀➁➂➃➄➅➆➇➈" +
                                    "➊➋➌➍➎➏➐➑➒" +
                                    "㈠㈡㈢㈣㈤㈥㈦㈧㈨" +
                                    "⓵⓶⓷⓸⓹⓺⓻⓼⓽" +
                                    "㊀㊁㊂㊃㊄㊅㊆㊇㊈";
        public const string nums2 = "0000000" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789" +
                                    "123456789";

        //internal static char GetFirstPinYin(char k)
        //{
        //    if (k >= 0x4e00 && k <= 0x9faf) {
        //        return strChineseFirstPY[k - 0x4e00];
        //    }
        //    return k;
        //}
    }
}
