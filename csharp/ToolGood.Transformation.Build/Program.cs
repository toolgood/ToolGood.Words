using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.Words;

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


        /// <summary>
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
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in tsc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }
            //排除 一些重复的 繁体转简体 ，
            List<List<string>> rsp2 = new List<List<string>>();
            foreach (var item in tsp) {
                var tStr = ToTo(item[0], dict);
                if (tStr != item[1]) {
                    rsp2.Add(item);
                }
            }
            //清除重复的 词组
            rsp2 = rsp2.OrderBy(q => q[0].Length).ToList();
            for (int i = rsp2.Count - 1; i >= 0; i--) {
                var item = rsp2[i];
                if (item[0].Length != item[1].Length) { continue; }

                for (int j = rsp2.Count - 1; j > i; j--) {
                    var item2 = rsp2[j];
                    if (item2[0].Contains(item[0])) {
                        var t = ToTo(item2[0], dict);
                        StringBuilder stringBuilder = new StringBuilder(t);
                        var index = item2[0].IndexOf(item[0]);
                        for (int k = 0; k < item[0].Length; k++) {
                            stringBuilder[index + k] = item[1][k];
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            rsp2.RemoveAt(j);
                        }
                    }
                }
            }
            rsp2 = rsp2.OrderBy(q => q[0]).ToList();


            List<string> list = new List<string>();
            foreach (var item in dict) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            foreach (var item in rsp2) {
                list.Add($"{item[0]}\t{item[1]}");
            }
            var str = string.Join("\n", list);
            File.WriteAllText("t2s.dat", str, Encoding.UTF8);
        }

        /// <summary>
        /// 简体转繁体
        /// </summary>
        private static void s2t()
        {
            var stc = ReadTexts(STCharacters);
            var stp = ReadTexts(STPhrases);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in stc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }
            //排除 一些重复的 简体转繁体 ，
            List<List<string>> stp2 = new List<List<string>>();
            foreach (var item in stp) {
                var tStr = ToTo(item[0], dict);
                if (tStr != item[1]) {
                    stp2.Add(item);
                }
            }
            //清除重复的 词组
            stp2 = stp2.OrderBy(q => q[0].Length).ToList();
            for (int i = stp2.Count - 1; i >= 0; i--) {
                var item = stp2[i];
                if (item[0].Length != item[1].Length) { continue; }

                for (int j = stp2.Count - 1; j > i; j--) {
                    var item2 = stp2[j];
                    if (item2[0].Contains(item[0])) {
                        var t = ToTo(item2[0], dict);
                        StringBuilder stringBuilder = new StringBuilder(t);
                        var index = item2[0].IndexOf(item[0]);
                        for (int k = 0; k < item[0].Length; k++) {
                            stringBuilder[index + k] = item[1][k];
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            stp2.RemoveAt(j);
                        }
                    }
                }
            }
            stp2 = stp2.OrderBy(q => q[0]).ToList();


            List<string> list = new List<string>();
            foreach (var item in dict) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            foreach (var item in stp2) {
                list.Add($"{item[0]}\t{item[1]}");
            }
            var str = string.Join("\n", list);
            File.WriteAllText("s2t.dat", str, Encoding.UTF8);
        }
        private static void t2hk()
        {
            var stc = ReadTexts(STCharacters);
            var stp = ReadTexts(STPhrases);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in stc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }
            //排除 一些重复的 简体转繁体 ，
            List<List<string>> stp2 = new List<List<string>>();
            foreach (var item in stp) {
                var tStr = ToTo(item[0], dict);
                if (tStr != item[1]) {
                    stp2.Add(item);
                }
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

            //排除 一些重复的 简体转繁体 ，
            List<List<string>> stp22 = new List<List<string>>();
            foreach (var item in hkv) {
                if (item[0].Length == 1) { continue; } //防止一变多

                var tStr1 = ToTo(item[0], dict);
                var tStr = ToTo(tStr1, dict2);
                if (tStr != item[1]) {
                    stp22.Add(item);
                }
            }

            stp22 = stp22.OrderBy(q => q[0].Length).ToList();
            for (int i = stp22.Count - 1; i >= 0; i--) {
                var item = stp22[i];
                if (item[0].Length != item[1].Length) { continue; }

                for (int j = stp22.Count - 1; j > i; j--) {
                    var item2 = stp22[j];
                    if (item2[0].Contains(item[0])) {
                        var t = ToTo(item2[0], dict);
                        t = ToTo(t, dict2);
                        StringBuilder stringBuilder = new StringBuilder(t);
                        var index = item2[0].IndexOf(item[0]);
                        for (int k = 0; k < item[0].Length; k++) {
                            stringBuilder[index + k] = item[1][k];
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            stp22.RemoveAt(j);
                        }
                    }
                }
            }
            stp22 = stp22.OrderBy(q => q[0]).ToList();

            List<string> list = new List<string>();
            foreach (var item in dict2) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            foreach (var item in stp22) {
                list.Add($"{item[0]}\t{item[1]}");
            }
            var str = string.Join("\n", list);
            File.WriteAllText("t2hk.dat", str, Encoding.UTF8);
        }
        private static void t2tw()
        {
            var stc = ReadTexts(STCharacters);
            var stp = ReadTexts(STPhrases);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in stc) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                dict[st[0]] = st[1];
            }
            //排除 一些重复的 简体转繁体 ，
            List<List<string>> stp2 = new List<List<string>>();
            foreach (var item in stp) {
                var tStr = ToTo(item[0], dict);
                if (tStr != item[1]) {
                    stp2.Add(item);
                }
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
            //排除 一些重复的 简体转繁体 ，
            List<List<string>> stp22 = new List<List<string>>();
            foreach (var item in twv) {
                if (item[0].Length == 1) { continue; } //防止一变多

                var tStr1 = ToTo(item[0], dict);
                var tStr = ToTo(tStr1, dict2);
                if (tStr != item[1]) {
                    stp22.Add(item);
                }
            }

            stp22 = stp22.OrderBy(q => q[0].Length).ToList();
            for (int i = stp22.Count - 1; i >= 0; i--) {
                var item = stp22[i];
                if (item[0].Length != item[1].Length) { continue; }

                for (int j = stp22.Count - 1; j > i; j--) {
                    var item2 = stp22[j];
                    if (item2[0].Contains(item[0])) {
                        var t = ToTo(item2[0], dict);
                        t = ToTo(t, dict2);
                        StringBuilder stringBuilder = new StringBuilder(t);
                        var index = item2[0].IndexOf(item[0]);
                        for (int k = 0; k < item[0].Length; k++) {
                            stringBuilder[index + k] = item[1][k];
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            stp22.RemoveAt(j);
                        }
                    }
                }
            }
            stp22 = stp22.OrderBy(q => q[0]).ToList();


            List<string> list = new List<string>();
            foreach (var item in dict2) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            foreach (var item in stp22) {
                list.Add($"{item[0]}\t{item[1]}");
            }
            var str = string.Join("\n", list);
            File.WriteAllText("t2tw.dat", str, Encoding.UTF8);
        }

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



    }
}
