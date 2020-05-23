using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Bedrock;

namespace ToolGood.PinYin.Build.FirstPinyin
{
    public class Program2
    {
        public static void Main2(string[] args)
        {
            // 生成单字拼音
            var pyShow = new List<string>() { "" };
            var upyShow = new List<string>();
            var singleWord = new List<string>();

            #region 生成全部拼音

            var pyText = File.ReadAllText("dict\\_py.txt");
            var pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                //Debug.WriteLine(line);
                for (int i = 1; i < sp.Length; i++) {
                    var py = sp[i];
                    py = py.ToLower();
                    py = RemoveTone(py)[0].ToString();
                    upyShow.Add(py.ToLower());
                }
            }
            pyText = File.ReadAllText("dict\\_py2.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < sp.Length; i++) {
                    var py = sp[i];
                    py = py.ToLower();
                    py = RemoveTone(py)[0].ToString();
                    upyShow.Add(py.ToLower());
                }
            }
            pyText = File.ReadAllText("dict\\_word.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var key = sp[0];
                if (key.Length == 1) { continue; }
                for (int i = 1; i < sp.Count; i++) {
                    var py = sp[i];
                    py = py.ToLower();
                    py = RemoveTone(py)[0].ToString();
                    upyShow.Add(py.ToLower());
                }
            }

            upyShow = upyShow.Distinct().OrderBy(q => q).ToList();
            foreach (var item in upyShow) {
                pyShow.Add(item.ToUpper());
            }
            #endregion
            #region 生成单字拼音1 

            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            pyText = File.ReadAllText("dict\\_py.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>-[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1) {
                    var key = sp[0];
                    HashSet<int> indexs = new HashSet<int>();
                    for (int i = 1; i < sp.Length; i++) {
                        var py = sp[i].Replace("v", "ü").Replace("ǹ", "èn").Replace("ň", "ěn");
                        py = RemoveTone(py)[0].ToString();
                        var idx = upyShow.IndexOf(py) + 1;
                        if (idx == -1) {
                            throw new Exception("");
                        }
                        indexs.Add(idx);
                    }
                    dict[key] = indexs.ToList();
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
                    if (indexs.Count == 1) {
                        singleWord.Add(c);
                    }
                    pyData.Add(string.Join(",", idxs));
                } else {
                    pyData.Add("0");
                }
            }
            var outText = string.Join(",", pyShow);
            outText += "\n" + string.Join("\n", pyData);
            Directory.CreateDirectory("fpy");
            File.WriteAllText("fpy\\pyIndex.txt", outText);
            Compression("fpy\\pyIndex.txt");

            File.WriteAllText("fpy\\_pyShow.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyShow));

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
            File.WriteAllText("fpy\\_pyIndex.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyIndex2));
            File.WriteAllText("fpy\\_pyData.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyData2));
            #endregion

            // 生成单字拼音 \U20000以上
            #region 生成单字拼音 \U20000以上
            Dictionary<string, List<int>> py20000 = new Dictionary<string, List<int>>();
            pyText = File.ReadAllText("dict\\_py2.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>-[]，　?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1) {
                    var key = sp[0];

                    HashSet<int> indexs = new HashSet<int>();
                    for (int i = 1; i < sp.Length; i++) {
                        var py = sp[i].Replace("v", "ü").Replace("ǹ", "èn").Replace("ň", "ěn");
                        py = RemoveTone(py)[0].ToString();

                        var idx = upyShow.IndexOf(py) + 1;
                        if (idx == -1) {
                            throw new Exception("");
                        }
                        indexs.Add(idx);
                    }
                    py20000[key] = indexs.ToList();
                }
            }
            List<List<string>> pyData20000 = new List<List<string>>();
            outText = null;
            for (int i = 0xd840; i <= 0xd86e; i++) {
                List<string> data20000 = new List<string>();
                StringBuilder stringBuilder = new StringBuilder("𠀀");
                stringBuilder[0] = (char)i;
                for (int j = 0xdc00; j <= 0xdfff; j++) {
                    stringBuilder[1] = (char)j;
                    var c = stringBuilder.ToString();
                    if (py20000.TryGetValue(c, out List<int> indexs)) {
                        List<string> idxs = new List<string>();
                        foreach (var index in indexs) {
                            idxs.Add(index.ToString("X"));
                        }
                        if (idxs[0] == "FFFFFFFF") {
                            throw new Exception("");
                        }
                        data20000.Add(string.Join(",", idxs));
                    } else {
                        data20000.Add("0");
                    }
                }
                pyData20000.Add(data20000);
                if (outText != null) {
                    outText += "\n";
                }
                outText += string.Join("\t", data20000);
            }
            File.WriteAllText("fpy\\pyIndex2.txt", outText);
            Compression("fpy\\pyIndex2.txt");
            #endregion

            // 获取 姓名拼音
            #region 姓名拼音
            Dictionary<string, List<int>> pyName = new Dictionary<string, List<int>>();
            pyText = File.ReadAllText("dict\\_pyName.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>-[]，　?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1) {
                    var key = sp[0];
                    List<int> indexs = new List<int>();
                    for (int i = 1; i < sp.Length; i++) {
                        var py = sp[i];
                        py = RemoveTone(py)[0].ToString();
                        var idx = upyShow.IndexOf(py) + 1;
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
            File.WriteAllText("fpy\\pyName.txt", string.Join("\n", ls));
            Compression("fpy\\pyName.txt");

            File.WriteAllText("fpy\\_pyName.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyName));
            #endregion

            //生成多字拼音
            #region 加载词组
            Dictionary<string, List<string>> pyWords = new Dictionary<string, List<string>>();
            pyText = File.ReadAllText("dict\\_word.txt");
            pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in pyLines) {
                var sp = line.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var key = sp[0];
                if (key.Length == 1) { continue; }
                sp.RemoveAt(0);
                for (int i = 0; i < sp.Count; i++) {
                    sp[i] = RemoveTone(sp[i])[0].ToString();
                }
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
                for (int i = 0; i < sp.Count; i++) {
                    sp[i] = RemoveTone(sp[i])[0].ToString();
                }
                pyWords[key] = sp;
            }
            #endregion


            //Words.StringSearchEx stringSearch = new Words.StringSearchEx();
            //stringSearch.SetKeywords(pyWords.Keys.ToList());

            Dictionary<string, List<string>> tempClearWords = new Dictionary<string, List<string>>();
            List<string> tempClearKeys = new List<string>();

            foreach (var item in pyWords) {
                var py = Regex.Replace(Words.WordsHelper.GetPinyinFast(item.Key, false), "[a-z]", "").ToLower();
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
            var keys = pyWords2.Select(q => q.Key).OrderBy(q => q).ToList();

            var index_remove = 1;
            var oldkey = keys[0];
            while (index_remove < keys.Count) {
                var key = keys[index_remove];
                if (key.StartsWith(oldkey)) {
                    bool remove = true;
                    for (int j = oldkey.Length; j < key.Length; j++) {
                        if (singleWord.Contains(key[j].ToString()) == false) {
                            remove = false;
                            break;
                        }
                    }
                    if (remove) {
                        keys.RemoveAt(index_remove);
                        pyWords2.Remove(key);
                    } else {
                        index_remove++;
                        oldkey = key;
                    }
                } else {
                    index_remove++;
                    oldkey = key;
                }
            }

            List<string> AddKeys = new List<string>();
            Words.WordsSearch wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(keys);
            foreach (var item in tempClearKeys) {
                if (wordsSearch.ContainsAny(item)) {
                    AddKeys.Add(item);
                }
            }


            HashSet<string> starts = new HashSet<string>();
            HashSet<string> ends = new HashSet<string>();
            foreach (var item in tempClearKeys) {
                for (int i = 1; i < item.Length; i++) {
                    var start = item.Substring(0, item.Length - i);
                    var end = item.Substring(i);

                    ends.Add(start);
                    starts.Add(end);
                }
            }


            List<string> AddKeys2 = new List<string>();
            List<string> keys2 = new List<string>();
            List<string> splitWords = new List<string>();
            foreach (var item in pyWords2) {
                var py = Regex.Replace(Words.WordsHelper.GetPinyinFast(item.Key, false), "[a-z]", "").ToLower();
                if (RemoveTone(py) != RemoveTone(string.Join("", item.Value))) {
                    for (int i = 1; i < item.Key.Length; i++) {
                        var start = item.Key.Substring(0, item.Key.Length - i);
                        if (keys2.Contains(start)) { continue; }
                        var end = item.Key.Substring(i);

                        if (starts.Contains(start) && ends.Contains(end)) {
                            keys2.Add(start);
                            splitWords.Add(start + "|" + end);
                        }
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
            //AddKeys.RemoveAll(q => q.Length >= 3 && q.EndsWith("县"));
            //AddKeys.RemoveAll(q => q.Length >= 3 && q.EndsWith("市"));
            //AddKeys.RemoveAll(q => q.Length >= 3 && q.EndsWith("州"));
            //AddKeys.RemoveAll(q => q.Length >= 3 && q.EndsWith("人"));
            //AddKeys.RemoveAll(q => q.Length >= 3 && q.EndsWith("盟"));
            //AddKeys.RemoveAll(q => q.Length >= 3 && q.EndsWith("党"));


            ls = new List<string>();
            foreach (var item in AddKeys) {
                var str = item;
                List<string> pys = pyWords[str];
                foreach (var py in pys) {
                    var py2 = py.Replace("v", "ü").Replace("ǹ", "èn").Replace("ň", "ěn");
                    var idx = upyShow.IndexOf(py2) + 1;
                    if (idx == -1) {
                        throw new Exception("");
                    }
                    str += "," + idx.ToString("X");
                }
                ls.Add(str);
            }
            ls = ls.OrderBy(q => q).ToList();
            File.WriteAllText("fpy\\pyWords.txt", string.Join("\n", ls));
            Compression("fpy\\pyWords.txt");
            //File.WriteAllText("pyWords.js.txt", string.Join("|", ls));

            File.WriteAllText("fpy\\_pyWordsKey.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(AddKeys));

            pyIndex2 = new List<int>() { 0 };
            pyData2 = new List<int>();
            foreach (var item in AddKeys) {
                var str = item;
                List<string> pys = pyWords[str];
                foreach (var py in pys) {
                    var py2 = py.Replace("v", "ü").Replace("ǹ", "èn").Replace("ň", "ěn");
                    var idx = upyShow.IndexOf(py2) + 1;
                    if (idx == -1) {
                        throw new Exception("");
                    }
                    pyData2.Add(idx);
                }
                pyIndex2.Add(pyData2.Count);
            }
            File.WriteAllText("fpy\\_pyWordsIndex.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyIndex2));
            File.WriteAllText("fpy\\_pyWordsData.js.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pyData2));



            PinyinDictBuild2.InitPy();
            PinyinDictBuild2.WritePinyinDat();
            PinyinDictBuild2.WritePinyinBigDat();
            Compression("fpy\\Pinyin.dat");
            Compression("fpy\\PinyinBig.dat");

        }

        static string RemoveTone(string pinyin)
        {
            if (pinyin == "ǹ") {
                return "en";
            }
            if (pinyin == "ǹg") {
                return "eng";
            }
            if (pinyin == "ň") {
                return "en";
            }
            if (pinyin == "ňg") {
                return "eng";
            }
            string s = "āáǎàōóǒòēéěèīíǐìūúǔùǖǘǚǜüńň";
            string t = "aaaaooooeeeeiiiiuuuuvvvvvnnm";
            for (int i = 0; i < s.Length; i++) {
                pinyin = pinyin.Replace(s[i].ToString(), t[i].ToString());
            }
            return pinyin;
        }

        private static void Compression(string file)
        {
            var bytes = File.ReadAllBytes(file);
            var bs = CompressionUtil.GzipCompress(bytes);
            Directory.CreateDirectory("dict\\fpy");
            File.WriteAllBytes("dict\\" + file + ".z", bs);

            var bs2 = CompressionUtil.BrCompress(bytes);
            File.WriteAllBytes("dict\\" + file + ".br", bs2);
        }



    }
}
