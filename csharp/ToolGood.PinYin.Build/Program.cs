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
            var files = Directory.GetFiles("dict","*.txt");
            foreach (var file in files) {
                var text = File.ReadAllText(file);
                var line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var l in line) {
                    if (string.IsNullOrWhiteSpace(l)) {
                        continue;
                    }
                    var sp = l.Split("\t,:| ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
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
            pyName = pyName.Distinct().OrderBy(q => q).ToList();
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



    }
}
