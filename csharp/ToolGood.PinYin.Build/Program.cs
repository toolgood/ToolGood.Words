using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToolGood.Bedrock;

namespace ToolGood.PinYin.Build
{
    class Program
    {
        private static List<string> pyName = new List<string>() { "" };
        static void Main(string[] args)
        {
            Dictionary<string, HashSet<string>> dict = new Dictionary<string, HashSet<string>>();
            var files = Directory.GetFiles("dict", "*.txt");
            foreach (var file in files) {
                var text = File.ReadAllText(file);
                var line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var l in line) {
                    if (string.IsNullOrWhiteSpace(l)) {
                        continue;
                    }
                    var sp = l.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < sp.Length; i++) {
                        pyName.Add(sp[i]);
                    }
                    HashSet<string> py;
                    if (dict.TryGetValue(sp[0], out py) == false) {
                        py = new HashSet<string>();
                        dict[sp[0]] = py;
                    }
                    for (int i = 1; i < sp.Length; i++) {
                        py.Add(sp[i]);
                    }
                }
            }
            Dictionary<string, int> valuePairs = new Dictionary<string, int>();
            foreach (var item in pyName) {
                int count = 0;
                if (valuePairs.TryGetValue(item,out count)) {

                }
                valuePairs[item] = count + 1;
            }
            var t = valuePairs.Where(q => q.Value < 2).Select(q => q.Key).OrderBy(q=>q).ToList();


            pyName = pyName.Distinct().OrderBy(q => q).ToList();



            var ts = pyName.Where(q => q.Length > 5).ToList();
            var ts1 = pyName.Where(q => q.Length == 5).ToList();
            var ts2 = pyName.Where(q => q.Length == 4).ToList();
            var ts3 = pyName.Where(q => q.Length == 3).ToList();
            var ts4 = pyName.Where(q => q.Length == 2).ToList();
            var ts5 = pyName.Where(q => q.Length == 1).ToList();
            var str = "\"" + string.Join("\",\"", pyName) + "\"";
            File.WriteAllText("pyName.txt", str);

            List<string> ls = new List<string>();
            foreach (var item in dict) {
                List<int> key = new List<int>();
                foreach (var v in item.Value) {
                    key.Add(pyName.IndexOf(v));
                }
                ls.Add($"{item.Key} {string.Join(",", key)}");
            }
            var outText = string.Join("\n", ls);
            File.WriteAllText("pyIndex.txt", outText);

            Compression("pyIndex.txt");
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

        static int GetToneIndex(string text)
        {
            var tone = @"aāáǎàa|oōóǒòo|eēéěèe|iīíǐìi|uūúǔùu|vǖǘǚǜü"
                    .Replace("a", " ").Replace("o", " ").Replace("i", " ")
                    .Replace("u", " ").Replace("v", " ")
                    .Split('|');
            foreach (var c in text)
            {
                foreach (var to in tone)
                {
                    var index = to.IndexOf(c);
                    if (index > 0)
                    {
                        return index;
                    }
                }
            }
            return 5;
        }

        static Dictionary<char, char> ToneDict;
        static string RemoveTone(string text)
        {
            if (ToneDict == null)
            {
                var tones = @"aāáǎàa|oōóǒòo|eēéěèe|iīíǐìi|uūúǔùu|vǖǘǚǜü".Split('|');
                var dict = new Dictionary<char, char>();
                foreach (var tone in tones)
                {
                    for (int i = 1; i < tone.Length; i++)
                    {
                        dict[tone[i]] = tone[0];
                    }
                }
                ToneDict = dict;
            }
            var dic = ToneDict;
            var str = "";
            foreach (var t in text)
            {
                if (dic.TryGetValue(t, out char c))
                {
                    str += c;
                }
                else
                {
                    str += t;
                }
            }
            return str;
        }

    }
}
