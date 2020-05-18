using Studyzy.IMEWLConverter.IME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.Bedrock;

namespace ToolGood.Transformation.Build
{
    class Program
    {
        const string HKVariants = "dict\\HKVariants.txt";
        const string HKVariantsPhrases = "dict\\HKVariantsPhrases.txt";
        const string HKVariantsRevPhrases = "dict\\HKVariantsRevPhrases.txt";
        const string STCharacters = "dict\\STCharacters.txt";
        const string STPhrases = "dict\\STPhrases.txt";
        const string TSCharacters = "dict\\TSCharacters.txt";
        const string TSPhrases = "dict\\TSPhrases.txt";
        const string TWPhrasesIT = "dict\\TWPhrasesIT.txt";
        const string TWPhrasesName = "dict\\TWPhrasesName.txt";
        const string TWPhrasesOther = "dict\\TWPhrasesOther.txt";
        const string TWVariants = "dict\\TWVariants.txt";
        const string TWVariantsRevPhrases = "dict\\TWVariantsRevPhrases.txt";

        // 
        const string STPhrases_Ext = "dict\\STPhrases_Ext.txt";
        const string TSPhrases_Ext = "dict\\TSPhrases_Ext.txt";


        /// <summary>
        /// 感谢 BYVoid 大佬
        /// dict 来源于  https://github.com/BYVoid/OpenCC
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            s2t();
            t2hk();
            t2tw();
            t2s();

            Compression("s2t.dat");
            Compression("t2hk.dat");
            Compression("t2tw.dat");
            Compression("t2s.dat");

        }

        /// <summary>
        /// 繁体转简体
        /// </summary>
        private static void t2s()
        {
            var tsc = ReadTexts(TSCharacters);
            var tsp = ReadTexts(TSPhrases);
            tsp.AddRange(ReadTexts(TSPhrases_Ext));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in tsc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }
            List<List<string>> rsp2 = SimplifyWords(tsp, dict, null);
            foreach (var item in rsp2) {
                dict[item[0]] = item[1];
            }


            List<string> list = new List<string>();
            List<string> keys = new List<string>();
            List<string> values = new List<string>();

            foreach (var item in dict.OrderBy(q=>q.Key.Length).OrderBy(q=>q.Key)) {
                list.Add($"{item.Key}\t{item.Value}");
                keys.Add(item.Key);
                values.Add(item.Value);
            }
            //foreach (var item in rsp2) {
            //    list.Add($"{item[0]}\t{item[1]}");
            //    keys.Add(item[0]);
            //    values.Add(item[1]);
            //}
            var str = string.Join("\n", list);
            File.WriteAllText("t2s.dat", str, Encoding.UTF8);
            File.WriteAllText("_t2s_t.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(keys));
            File.WriteAllText("_t2s_s.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(values));

        }

        /// <summary>
        /// 简体转繁体
        /// </summary>
        private static void s2t()
        {
            var stc = ReadTexts(STCharacters);
            var stp = ReadTexts(STPhrases);
            stp.AddRange(ReadTexts(STPhrases_Ext));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in stc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }

            List<List<string>> stp2 = SimplifyWords4(stp, dict, null);
            foreach (var item in stp2) {
                dict[item[0]] = item[1];
            }



            List<string> list = new List<string>();
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in dict.OrderBy(q=>q.Key.Length)) {
                list.Add($"{item.Key}\t{item.Value}");
                keys.Add(item.Key);
                values.Add(item.Value);
            }
            //foreach (var item in stp2) {
            //    list.Add($"{item[0]}\t{item[1]}");
            //    keys.Add(item[0]);
            //    values.Add(item[1]);
            //}
            var str = string.Join("\n", list);
            File.WriteAllText("s2t.dat", str, Encoding.UTF8);
            File.WriteAllText("_s2t_s.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(keys));
            File.WriteAllText("_s2t_t.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(values));
        }
        private static void t2hk()
        {
            var stc = ReadTexts(STCharacters);
            var stp = ReadTexts(STPhrases);
            stp.AddRange(ReadTexts(STPhrases_Ext));

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in stc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }

            //----------- 
            var hkv = ReadTexts(HKVariants);
            var hkvp = ReadTexts(HKVariantsPhrases);
            var hkvrp = ReadTexts(HKVariantsRevPhrases);
            hkv.AddRange(hkvp);
            hkv.AddRange(hkvrp);


            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            foreach (var st in hkv) {
                if (st[0].Length > 1) { continue; }
                if (st[1].Length > 1) { continue; }
                dict2[st[0]] = st[1];
            }

            List<List<string>> stp22 = SimplifyWords(hkv, dict, dict2);


            List<string> list = new List<string>();
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in dict2) {
                list.Add($"{item.Key}\t{item.Value}");
                keys.Add(item.Key);
                values.Add(item.Value);
            }
            foreach (var item in stp22) {
                list.Add($"{item[0]}\t{item[1]}");
                keys.Add(item[0]);
                values.Add(item[1]);
            }
            var str = string.Join("\n", list);
            File.WriteAllText("t2hk.dat", str, Encoding.UTF8);
            File.WriteAllText("_t2hk_t.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(keys));
            File.WriteAllText("_t2hk_hk.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(values));
        }
        private static void t2tw()
        {
            var stc = ReadTexts(STCharacters);
            var stp = ReadTexts(STPhrases);
            stp.AddRange(ReadTexts(STPhrases_Ext));

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in stc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }

            //----------- 
            var twv = ReadTexts(TWVariants);
            var twpit = ReadTexts(TWPhrasesIT);
            var twpn = ReadTexts(TWPhrasesName);
            var twpo = ReadTexts(TWPhrasesOther);
            var twvp = ReadTexts(TWVariantsRevPhrases);
            twv.AddRange(twpit);
            twv.AddRange(twpn);
            twv.AddRange(twpo);
            twv.AddRange(twvp);

            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            foreach (var st in twv) {
                if (st[0].Length > 1) { continue; }
                if (st[1].Length > 1) { continue; }
                dict2[st[0]] = st[1];
            }

            List<List<string>> stp22 = SimplifyWords(twv, dict, dict2);

            List<string> list = new List<string>();
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (var item in dict2) {
                list.Add($"{item.Key}\t{item.Value}");
                keys.Add(item.Key);
                values.Add(item.Value);
            }
            foreach (var item in stp22) {
                list.Add($"{item[0]}\t{item[1]}");
                keys.Add(item[0]);
                values.Add(item[1]);
            }
            var str = string.Join("\n", list);
            File.WriteAllText("t2tw.dat", str, Encoding.UTF8);
            File.WriteAllText("_t2tw_t.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(keys));
            File.WriteAllText("_t2tw_tw.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(values));
        }


        /// <summary>
        /// 精细 转换
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dict"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        private static List<List<string>> SimplifyWords(List<List<string>> src, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            List<List<string>> tarList = new List<List<string>>();
            List<List<string>> tempClearList = new List<List<string>>();

            // 保存
            foreach (var item in src) {
                if (item[0].Length == 1) { continue; } //防止一变多

                var tStr = ToTo(item[0], dict);
                if (dict2 != null) { tStr = ToTo(tStr, dict2); }
                if (tStr != item[1]) {
                    tarList.Add(item);
                } else {
                    tempClearList.Add(item);
                }
            }

            //清除重复的 词组
            tarList = SimplifyWords2(tarList, dict, dict2);

            // 由于算法是从前向后替换，只要保证前面的词组能够正确识别出来就可以了。
            List<string> firstChars = new List<string>();
            foreach (var item in tarList) {
                for (int i = 0; i < item[0].Length - 1; i++) {
                    firstChars.Add(item[0].Substring(0, item[0].Length - i));
                }
            }
            firstChars = firstChars.Distinct().OrderBy(q => q.Length).ToList();
            Words.WordsSearch wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(firstChars);


            List<List<string>> lastTempList = new List<List<string>>();
            foreach (var item in tempClearList) {
                var end = item[0].Length - 1;
                var all = wordsSearch.FindAll(item[0]);
                var f = all.Where(q => q.End == end).FirstOrDefault();
                if (f != null) {
                    lastTempList.Add(item);
                }
            }
 
            // 再来一次 清除重复的 词组
            lastTempList = SimplifyWords3(lastTempList, dict, dict2);

            // 
            var fullList = tarList.Select(q => q[0]).ToList();
            List<List<string>> containsTempList = new List<List<string>>();

            foreach (var item in tempClearList) {
                if (fullList.Contains(item[0])) {
                    containsTempList.Add(item);
                }
            }
            containsTempList = SimplifyWords2(containsTempList, dict, dict2);

            tarList.AddRange(lastTempList);
            tarList.AddRange(containsTempList);

            tarList = tarList.Distinct().ToList();
            tarList = tarList.OrderBy(q => q[0]).ToList();
            return tarList;
        }

        private static List<List<string>> SimplifyWords2(List<List<string>> tarList, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            tarList = tarList.OrderBy(q => q[0].Length).ToList();
            for (int i = tarList.Count - 1; i >= 0; i--) {
                var item = tarList[i];

                for (int j = tarList.Count - 1; j > i; j--) {
                    var item2 = tarList[j];
                    if (item2[0].Contains(item[0])) {
                        StringBuilder stringBuilder = new StringBuilder();
                        var index = item2[0].IndexOf(item[0]);
                        if (index > 0) {
                            var t = item2[0].Substring(0, index);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        stringBuilder.Append(item[1]);
                        if (index + item[0].Length + 1 <= item2[0].Length) {
                            var t = item2[0].Substring(index + item[0].Length);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            tarList.RemoveAt(j);
                        }
                    }
                }
            }
            return tarList;
        }

        private static List<List<string>> SimplifyWords3(List<List<string>> tarList, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            tarList = tarList.OrderBy(q => q[0].Length).ToList();
            for (int i = tarList.Count - 1; i >= 0; i--) {
                var item = tarList[i];

                for (int j = tarList.Count - 1; j > i; j--) {
                    var item2 = tarList[j];
                    if (item2[0].EndsWith(item[0])) {
                        StringBuilder stringBuilder = new StringBuilder();
                        var index = item2[0].LastIndexOf(item[0]);
                        if (index > 0) {
                            var t = item2[0].Substring(0, index);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        stringBuilder.Append(item[1]);
                        if (index + item[0].Length + 1 <= item2[0].Length) {
                            var t = item2[0].Substring(index + item[0].Length);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            tarList.RemoveAt(j);
                        }
                    }
                }
            }
            return tarList;
        }

        /// <summary>
        /// 仅用于简体
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dict"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        private static List<List<string>> SimplifyWords4(List<List<string>> src, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            List<List<string>> tarList = new List<List<string>>();
            List<List<string>> tempClearList = new List<List<string>>();

            // 保存
            foreach (var item in src) {
                if (item[0].Length == 1) { continue; } //防止一变多

                var tStr = ToTo(item[0], dict);
                if (dict2 != null) { tStr = ToTo(tStr, dict2); }
                if (tStr != item[1]) {
                    tarList.Add(item);
                } else {
                    tempClearList.Add(item);
                }
            }

            //清除重复的 词组
            tarList = SimplifyWords2(tarList, dict, dict2);

            // 由于算法是从前向后替换，只要保证前面的词组能够正确识别出来就可以了。
            List<string> firstChars = new List<string>();
            foreach (var item in tarList) {
                for (int i = 0; i < item[0].Length - 1; i++) {
                    var t = item[0].Substring(0, item[0].Length - i);
                    firstChars.Add(t);
                }
            }
            firstChars = firstChars.Distinct().OrderBy(q => q.Length).ToList();
            var srcWords = tarList.Select(q => q[0]).ToList();

            Words.WordsSearch wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(firstChars);
            Words.WordsSearch wordsSearch2 = new Words.WordsSearch();
            wordsSearch2.SetKeywords(srcWords);

            List<string> containsTempList = new List<string>();
            var words = GetWords();

            foreach (var item in words) {
                var end = item.Length - 1;
                var all = wordsSearch.FindAll(item);
                var f = all.Where(q => q.End == end).FirstOrDefault();
                if (f != null) {
                    if (wordsSearch2.ContainsAny(item) == false) {
                        containsTempList.Add(item);
                    }
                }
            }

            foreach (var item in tempClearList) {
                var end = item[0].Length - 1;
                var all = wordsSearch.FindAll(item[0]);
                var f = all.Where(q => q.End == end).FirstOrDefault();
                if (f != null) {
                    if (wordsSearch2.ContainsAny(item[0]) == false) {
                        containsTempList.Add(item[0]);
                    }
                }
            }

            containsTempList = containsTempList.Distinct().ToList();
            containsTempList = containsTempList.OrderBy(q => q.Length).ToList();
            // 清理 搜狗词库

            for (int i = 2; i < 8; i++) {
                var keywords = containsTempList.Where(q => q.Length <= i).ToList();
                wordsSearch = new Words.WordsSearch();
                wordsSearch.SetKeywords(keywords);

                for (int j = containsTempList.Count - 1; j >= i + 1; j--) {
                    var item = containsTempList[j];
                    if (item.Length <= i) { break; }

                    var end = item.Length - 1;
                    var all = wordsSearch.FindAll(item);
                    var f = all.Where(q => q.End == end).FirstOrDefault();
                    if (f != null) {
                        containsTempList.RemoveAt(j);
                    }
                }
            }

            containsTempList = containsTempList.Distinct().ToList();

            foreach (var item in containsTempList) {
                string s = "";
                foreach (var c in item) {
                    if (dict.TryGetValue(c.ToString(), out string v)) {
                        s += v;
                    } else {
                        s += c;
                    }
                }
                tarList.Add(new List<string>() { item, s });
            }

            tarList = tarList.Distinct().ToList();
            tarList = tarList.OrderBy(q => q[0]).ToList();
            return tarList;
        }




        //private static int GetLength(string text)
        //{
        //    var stringInfo = new System.Globalization.StringInfo(text);
        //    return stringInfo.LengthInTextElements;
        //}

        //public static String Substring(String text, int start, int end)
        //{
        //    var stringInfo = new System.Globalization.StringInfo(text);
        //    return stringInfo.SubstringByTextElements(start, end);
        //}

        private static string ToTo(string srcText, Dictionary<string, string> dict)
        {
            var str = "";
            foreach (var c in srcText) {
                var s = c.ToString();
                if (dict.TryGetValue(s, out string v)) {
                    str += v;
                } else {
                    str += s;
                }
            }
            return str;
        }

        private static void Compression(string file)
        {
            var bytes = File.ReadAllBytes(file);
            var bs = CompressionUtil.GzipCompress(bytes);
            Directory.CreateDirectory("dict");
            File.WriteAllBytes("dict\\" + file + ".z", bs);

            var bs2 = CompressionUtil.BrCompress(bytes);
            File.WriteAllBytes("dict\\" + file + ".br", bs2);
        }

        private static List<List<string>> ReadTexts(string file)
        {
            List<List<string>> list = new List<List<string>>();
            var ts = File.ReadAllLines(file);
            foreach (var t in ts) {
                if (string.IsNullOrEmpty(t) == false) {
                    var sp = t.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> ls = new List<string>();
                    foreach (var item in sp) {
                        ls.Add(item.Trim());
                    }
                    list.Add(ls);
                }
            }
            return list;
        }


        static List<string> GetWords()
        {
            var files = Directory.GetFiles("Scel");
            HashSet<string> list = new HashSet<string>();

            foreach (var file in files) {
                GetWords(file, list);
            }
            return list.ToList();
        }

        static List<string> GetWords(string file, HashSet<string> list)
        {
            SougouPinyinScel scel = new SougouPinyinScel();
            var t = scel.Import(file);

            foreach (var item in t) {
                list.Add(item.Word.Trim());
            }
            return list.ToList();
        }





    }
}
