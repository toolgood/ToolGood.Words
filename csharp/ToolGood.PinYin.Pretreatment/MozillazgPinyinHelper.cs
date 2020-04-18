using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace ToolGood.PinYin.Pretreatment
{
    public class MozillazgPinyinHelper
    {
        public static void ToStandardization()
        {
            Console.WriteLine("mozillazg 合并 并 转成标准拼音格式文本");
            if (File.Exists("mozillazg_pinyin.txt") == false) {
                Dictionary<string, List<string>> pysDict = new Dictionary<string, List<string>>();
                {
                    var files = Directory.GetFiles("pinyin-data");
                    foreach (var file in files) {
                        var txt = File.ReadAllText(file);
                        var lines = txt.Split('\n');
                        foreach (var line in lines) {
                            var t = Regex.Replace(line, "#.*", "").Replace("U+", "").Trim();
                            if (string.IsNullOrEmpty(t)) { continue; }

                            var sp = Regex.Replace(line, "(->|=>).*", "").Replace("U+", "").Split(" ,:#\r\n\t?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            List<string> pys = new List<string>();
                            for (int i = 1; i < sp.Length - 1; i++) {
                                pys.Add(sp[i]);
                            }
                            pysDict[sp.Last()] = pys;
                        }
                    }
                }
                {
                    var txt = File.ReadAllText("pinyin-data\\pinyin.txt");
                    var lines = txt.Split('\n');

                    foreach (var line in lines) {
                        var t = Regex.Replace(line, "#.*", "").Replace("U+", "").Trim();
                        if (string.IsNullOrEmpty(t)) { continue; }
                        var sp = Regex.Replace(line, "(->|=>).*", "").Replace("U+", "").Split(" ,:#\r\n\t?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        List<string> pys = new List<string>();
                        for (int i = 1; i < sp.Length - 1; i++) {
                            pys.Add(sp[i]);
                        }
                        pysDict[sp.Last()] = pys;
                    }
                }
                List<string> ls = new List<string>();
                foreach (var item in pysDict) {
                    ls.Add($"{item.Key} {string.Join(",", item.Value)}");
                }
                File.WriteAllText("mozillazg_pinyin.txt", string.Join("\n", ls));
            }


        }
        public static string DeUnicode(string str)
        {
            if (str.Length > 4) {
                var str1 = str.Substring(0, str.Length - 4);
                var str2 = str.Substring(str.Length - 4, 4);
                return DeUnicode(str1) + DeUnicode(str2);
            }
            return ((char)Convert.ToInt32(str, 16)).ToString();
        }



    }
}
