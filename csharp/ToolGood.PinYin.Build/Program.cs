using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ToolGood.Bedrock;

namespace ToolGood.PinYin.Build
{
    class Program
    {
        static void Main(string[] args)
        {
            // 生成单字拼音
            var pyShow = new List<string>() { "" };
            var upyShow = new List<string>();

            var pyText = File.ReadAllText("dict\\_py.txt");
            var pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < sp.Length; i++) {
                    var py = sp[i];
                    //pyName.Add(py.ToUpper()[0] + py.Substring(1));
                    upyShow.Add(py.ToLower());
                }
            }
            upyShow = upyShow.Distinct().OrderBy(q => q).ToList();
            foreach (var item in upyShow) {
                var py = RemoveTone(item);
                pyShow.Add(py.ToUpper()[0] + py.Substring(1));
                pyShow.Add(item.ToUpper()[0] + item.Substring(1));
            }


            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>-[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1) {
                    var key = sp[0];
                    List<int> indexs = new List<int>();
                    for (int i = 1; i < sp.Length; i++) {
                        var py = sp[i];
                        var idx = upyShow.IndexOf(py) * 2 + 1;
                        if (idx == -1) {
                            throw new Exception("");
                        }
                        indexs.Add(idx);
                    }
                    dict[key] = indexs;
                }
            }

            List<string> pyData = new List<string>();
            for (int i = 0x3400; i <= 0x9fd5; i++) {
                var c = ((char)i).ToString();
                if (dict.TryGetValue(c, out List<int> indexs)) {
                    List<string> idxs = new List<string>();
                    foreach (var index in indexs) {
                        idxs.Add(index.ToString("X"));
                    }
                    if (idxs[0] == "FFFFFFFF") {
                        throw new Exception("");
                    }

                    pyData.Add(string.Join(",", idxs));
                } else {
                    pyData.Add("0");
                }
            }
            var outText = string.Join(",", pyShow);
            outText += "\n" + string.Join("\n", pyData);
            File.WriteAllText("pyIndex.txt", outText);
            Compression("pyIndex.txt");

            File.WriteAllText("_pyShow.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyShow));

            List<int> pyIndex2 = new List<int>() { 0 };
            List<int> pyData2 = new List<int>();
            for (int i = 0; i < pyData.Count; i++) {
                var idxs = pyData[i];
                if (idxs != "0") {
                    foreach (var idx in idxs.Split(',')) {
                        pyData2.Add(ushort.Parse(idx, System.Globalization.NumberStyles.HexNumber));
                    }
                }
                pyIndex2.Add((ushort)pyData2.Count);
            }
            File.WriteAllText("_pyIndex.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyIndex2));
            File.WriteAllText("_pyData.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyData2));


            // 获取 姓名拼音
            Dictionary<string, List<int>> pyName = new Dictionary<string, List<int>>();
            pyText = File.ReadAllText("dict\\_pyName.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>-[]，　?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1) {
                    var key = sp[0];
                    if (key == "单") {

                    }
                    List<int> indexs = new List<int>();
                    for (int i = 1; i < sp.Length; i++) {
                        var py = sp[i];
                        py = AddTone(py);
                        var idx = upyShow.IndexOf(py) * 2 + 1;
                        if (idx == -1) {
                            throw new Exception("");
                        }
                        indexs.Add(idx);
                    }
                    pyName[key] = indexs;
                }
            }
            List<string> ls = new List<string>();
            foreach (var item in pyName) {
                List<int> idx = new List<int>();
                List<string> idxs = new List<string>();
                foreach (var index in item.Value) {
                    idxs.Add(index.ToString("X"));
                }
                ls.Add($"{item.Key},{string.Join(",", idxs)}");
            }
            File.WriteAllText("pyName.txt", string.Join("\n", ls));
            Compression("pyName.txt");

            File.WriteAllText("_pyName.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyName));

            //生成多字拼音
            Dictionary<string, List<string>> pyWords = new Dictionary<string, List<string>>();
            pyText = File.ReadAllText("dict\\_word.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var key = sp[0];
                if (key.Length == 1) { continue; }
                sp.RemoveAt(0);
                pyWords[key] = sp;
            }
            // 搜狗拼音也有错误的
            pyText = File.ReadAllText("dict\\_wordRevise.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split(", []=|:\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var key = sp[0];
                if (key.Length == 1) { continue; }
                sp.RemoveAt(0);
                pyWords[key] = sp;
            }



            Dictionary<string, List<string>> tempClearWords = new Dictionary<string, List<string>>();
            List<string> tempClearKeys = new List<string>();

            foreach (var item in pyWords) {
                var py = Words.WordsHelper.GetPinYinFast(item.Key, true).ToLower();
                if (py == string.Join("", item.Value)) {
                    tempClearWords[item.Key] = item.Value;
                    tempClearKeys.Add(item.Key);
                }
            }
            var pyWords2 = new Dictionary<string, List<string>>();
            foreach (var item in pyWords) {
                pyWords2[item.Key] = item.Value;
            }

            foreach (var item in tempClearWords) {
                pyWords2.Remove(item.Key);
            }

            List<string> AddKeys = new List<string>();
            var keys = pyWords2.Select(q => q.Key).ToList();
            Words.WordsSearch wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(keys);
            foreach (var item in tempClearKeys) {
                if (wordsSearch.ContainsAny(item)) {
                    AddKeys.Add(item);
                }
            }

            List<string> AddKeys2 = new List<string>();
            List<string> keys2 = new List<string>();
            foreach (var item in pyWords2) {
                var py = Words.WordsHelper.GetPinYinFast(item.Key, true).ToLower();
                if (RemoveTone(py) != RemoveTone(string.Join("", item.Value))) {
                    for (int i = 0; i < item.Key.Length - 1; i++) {
                        keys2.Add(item.Key.Substring(0, i + 1));
                    }
                }
            }
            keys2 = keys2.Distinct().ToList();
            wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(keys2);
            foreach (var item in tempClearKeys) {
                if (item.Length >= 4) { continue; } //排除诗句 歇后语
                var all = wordsSearch.FindAll(item);
                if (all.Any(q => q.End + 1 == item.Length)) {
                    AddKeys2.Add(item);
                }
            }

            AddKeys.AddRange(AddKeys2);
            AddKeys.AddRange(keys);
            AddKeys = AddKeys.Distinct().ToList();

            ls = new List<string>();
            foreach (var item in AddKeys) {
                var str = item;
                List<string> pys = pyWords[str];
                foreach (var py in pys) {
                    var idx = upyShow.IndexOf(py) * 2 + 1;
                    if (idx == -1) {
                        throw new Exception("");
                    }
                    str += "," + idx.ToString("X");
                }
                ls.Add(str);
            }
            ls = ls.OrderBy(q => q).ToList();
            File.WriteAllText("pyWords.txt", string.Join("\n", ls));
            Compression("pyWords.txt");
            File.WriteAllText("pyWords.js.txt", string.Join("|", ls));

            File.WriteAllText("_pyWordsKey.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(AddKeys));

            pyIndex2 = new List<int>() { 0 };
            pyData2 = new List<int>();
            foreach (var item in AddKeys) {
                var str = item;
                List<string> pys = pyWords[str];
                foreach (var py in pys) {
                    var idx = upyShow.IndexOf(py) * 2 + 1;
                    if (idx == -1) {
                        throw new Exception("");
                    }
                    pyData2.Add(idx);
                }
                pyIndex2.Add(pyData2.Count);
            }
            File.WriteAllText("_pyWordsIndex.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyIndex2));
            File.WriteAllText("_pyWordsData.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyData2));


        }

        static string RemoveTone(string pinyin)
        {
            string s = "āáǎàōóǒòēéěèīíǐìūúǔùǖǘǚǜüńň";
            string t = "aaaaooooeeeeiiiiuuuuvvvvvnnm";
            for (int i = 0; i < s.Length; i++) {
                pinyin = pinyin.Replace(s[i].ToString(), t[i].ToString());
            }
            return pinyin;
        }


        static string AddTone(string pinyin)
        {
            pinyin = pinyin.ToLower();
            if (pinyin.EndsWith("1")) {
                if (pinyin.Contains("a")) {
                    pinyin = pinyin.Replace("a", "ā");
                } else if (pinyin.Contains("o")) {
                    pinyin = pinyin.Replace("o", "ō");
                } else if (pinyin.Contains("e")) {
                    pinyin = pinyin.Replace("e", "ē");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "ī");
                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ū");
                } else if (pinyin.Contains("v")) {
                    pinyin = pinyin.Replace("v", "ǖ");
                } else {
                    throw new Exception("");
                }
            } else if (pinyin.EndsWith("2")) {
                if (pinyin.Contains("a")) {
                    pinyin = pinyin.Replace("a", "á");
                } else if (pinyin.Contains("o")) {
                    pinyin = pinyin.Replace("o", "ó");
                } else if (pinyin.Contains("e")) {
                    pinyin = pinyin.Replace("e", "é");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "í");
                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ú");
                } else if (pinyin.Contains("v")) {
                    pinyin = pinyin.Replace("v", "ǘ");
                } else {
                    throw new Exception("");
                }
            } else if (pinyin.EndsWith("3")) {
                if (pinyin.Contains("a")) {
                    pinyin = pinyin.Replace("a", "ǎ");
                } else if (pinyin.Contains("o")) {
                    pinyin = pinyin.Replace("o", "ǒ");
                } else if (pinyin.Contains("e")) {
                    pinyin = pinyin.Replace("e", "ě");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "ǐ");
                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ǔ");
                } else if (pinyin.Contains("v")) {
                    pinyin = pinyin.Replace("v", "ǚ");
                } else {
                    throw new Exception("");
                }
            } else if (pinyin.EndsWith("4")) {
                if (pinyin.Contains("a")) {
                    pinyin = pinyin.Replace("a", "à");
                } else if (pinyin.Contains("o")) {
                    pinyin = pinyin.Replace("o", "ò");
                } else if (pinyin.Contains("e")) {
                    pinyin = pinyin.Replace("e", "è");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "ì");
                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ù");
                } else if (pinyin.Contains("v")) {
                    pinyin = pinyin.Replace("v", "ǜ");
                } else {
                    throw new Exception("");
                }
            } else if (pinyin.EndsWith("0") || pinyin.EndsWith("5")) {
                if (pinyin.Contains("a")) {
                } else if (pinyin.Contains("o")) {
                } else if (pinyin.Contains("e")) {
                } else if (pinyin.Contains("i")) {
                } else if (pinyin.Contains("u")) {
                } else if (pinyin.Contains("v")) {
                    pinyin = pinyin.Replace("v", "ü");
                } else {
                    throw new Exception("");
                }
            } else if (pinyin.EndsWith("6") || pinyin.EndsWith("7") || pinyin.EndsWith("8") || pinyin.EndsWith("9")) {
                throw new Exception("");
            }
            pinyin = Regex.Replace(pinyin, @"\d", "");

            return pinyin;
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
            foreach (var c in text) {
                foreach (var to in tone) {
                    var index = to.IndexOf(c);
                    if (index > 0) {
                        return index;
                    }
                }
            }
            return 5;
        }



    }
}
