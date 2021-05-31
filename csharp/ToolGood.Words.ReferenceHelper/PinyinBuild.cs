using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using ToolGood.Bedrock;
using ToolGood.PinYin.Build.Pinyin;

namespace ToolGood.Words.ReferenceHelper
{
    public class PinyinBuild
    {
        private const char HIGH_SURROGATE_START = '\uD800';
        private const char HIGH_SURROGATE_END = '\uDBFF';
        private const char LOW_SURROGATE_START = '\uDC00';
        private const char LOW_SURROGATE_END = '\uDFFF';
        private const int UNICODE_PLANE00_END = 0x00FFFF;
        private const int UNICODE_PLANE01_START = 0x10000;
        private const int UNICODE_PLANE16_END = 0x10FFFF;
        private static Regex FileTypeReg = new Regex(@"dict\[0x([0-9a-fA-F]+)\] *= *""([^""]+)"";", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Dictionary<char, List<string>> sDict = new Dictionary<char, List<string>>();
        public Dictionary<Tuple<char, char>, List<string>> sDict2 = new Dictionary<Tuple<char, char>, List<string>>();
        public Dictionary<string, List<string>> mDict = new Dictionary<string, List<string>>();

        private Dictionary<string, List<string>> noneTomeDict = new Dictionary<string, List<string>>();

        #region Load
        public void Load(List<String> files)
        {
            files = files.Distinct().ToList();
            foreach (var file in files) {
                Load(file);
            }
        }
        private void Load(string file)
        {
            if (File.Exists(file) == false) { return; }
            var txts = File.ReadAllLines(file);

            foreach (var txt in txts) {
                var t = txt.Trim();
                if (string.IsNullOrEmpty(t)) { continue; }
                t = t.Split("/*")[0];
                t = t.Split("//")[0];
                if (string.IsNullOrEmpty(t)) { continue; }
                var m = FileTypeReg.Match(t);
                if (m.Success) {
                    var num = m.Groups[1].Value;
                    var pinyin = m.Groups[2].Value;
                    uint parsedValue;
                    uint.TryParse(num, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out parsedValue);

                    if (parsedValue <= UNICODE_PLANE00_END) {
                        var sp = pinyin.Split("\t,:| '\"-=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var py in sp) {
                            AddDict((char)parsedValue, py);
                        }

                    } else {
                        ConvertSmpToUtf16(parsedValue, out char leadingSurrogate, out char trailingSurrogate);
                        var sp = pinyin.Split("\t,:| '\"-=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var py in sp) {
                            AddDict(leadingSurrogate, trailingSurrogate, py);
                        }
                    }
                } else {
                    var sp = t.Split("\t,:| '\"-=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (sp.Length < 2) { continue; }
                    var s = sp[0];
                    if (s.Length == 1) {
                        var ch = s[0];
                        for (int i = 1; i < sp.Length; i++) {
                            AddDict(ch, sp[i]);
                        }
                    } else if (s.Length == 2 && s[0] >= HIGH_SURROGATE_START && s[0] <= HIGH_SURROGATE_END
                        && s[1] >= LOW_SURROGATE_START && s[1] <= LOW_SURROGATE_END) {
                        var ch1 = s[0];
                        var ch2 = s[1];
                        for (int i = 1; i < sp.Length; i++) {
                            AddDict(ch1, ch2, sp[i]);
                        }
                    } else {
                        AddDict(s, sp);
                    }
                }
            }
        }
        private void AddDict(char ch, string pinyin)
        {
            List<string> pys;
            if (sDict.TryGetValue(ch, out pys) == false) {
                pys = new List<string>();
                sDict[ch] = pys;
            }
            if (pys.Contains(pinyin.Trim()) == false) {
                pys.Add(pinyin.Trim());
            }
        }
        private void AddDict(char ch1, char ch2, string pinyin)
        {
            var ch = Tuple.Create(ch1, ch2);
            List<string> pys;
            if (sDict2.TryGetValue(ch, out pys) == false) {
                pys = new List<string>();
                sDict2[ch] = pys;
            }
            if (pys.Contains(pinyin.Trim()) == false) {
                pys.Add(pinyin.Trim());
            }
        }

        private void AddDict(string words, string[] pinyin)
        {
            if (mDict.ContainsKey(words)) {
                return;
            }
            if (Regex.IsMatch(words, @"[\t\r\n,.!@#$%^&*()_+\-=;\""''，。？、：“”‘’【】{}]")) {
                return;
            }
            for (int i = 1; i < pinyin.Length; i++) {
                if (Regex.IsMatch(pinyin[i], "[\u3400-\u9fff]")) {
                    return;
                }
            }

            var list = new List<string>();
            for (int i = 1; i < pinyin.Length; i++) {
                list.Add(pinyin[i].Trim());
            }
            mDict[words] = list;
        }
        private void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate)
        {
            int utf32 = (int)(smpChar - UNICODE_PLANE01_START);
            leadingSurrogate = (char)((utf32 / 0x400) + HIGH_SURROGATE_START);
            trailingSurrogate = (char)((utf32 % 0x400) + LOW_SURROGATE_START);
        }
        #endregion

        #region CheckPinyin
        public void CheckPinyin(string rightFilePath, string errorFilePath)
        {
            var rightFilePathFull = Path.GetFullPath(rightFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(rightFilePathFull));
            var fs = File.Open(rightFilePathFull, FileMode.Create);
            StreamWriter rightWriter = new StreamWriter(fs);

            var errorFilePathFUll = Path.GetFullPath(errorFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(errorFilePathFUll));
            var fs2 = File.Open(errorFilePathFUll, FileMode.Create);
            StreamWriter errorWriter = new StreamWriter(fs2);

            foreach (var item in sDict) {
                var srcPinyins = item.Value;
                var pys = CheckPinyin(srcPinyins);
                if (IsSome(srcPinyins, pys) == false) {
                    errorWriter.Write(item.Key);
                    errorWriter.Write("\t");
                    WriteList(errorWriter, item.Value);
                    errorWriter.Write("\r\n");
                    sDict[item.Key] = pys;
                }
                rightWriter.Write(item.Key);
                rightWriter.Write("\t");
                WriteList(rightWriter, pys);
                rightWriter.Write("\r\n");
            }
            foreach (var item in sDict2) {
                var srcPinyins = item.Value;
                var pys = CheckPinyin(srcPinyins);
                if (IsSome(srcPinyins, pys) == false) {
                    errorWriter.Write(new char[] { item.Key.Item1, item.Key.Item2 });
                    errorWriter.Write("\t");
                    WriteList(errorWriter, item.Value);
                    errorWriter.Write("\r\n");
                    sDict2[item.Key] = pys;
                }
                rightWriter.Write(new char[] { item.Key.Item1, item.Key.Item2 });
                rightWriter.Write("\t");
                WriteList(rightWriter, pys);
                rightWriter.Write("\r\n");
            }

            BuildNoneTomeDict();
            foreach (var item in mDict) {
                var srcPinyins = item.Value;
                var pys = CheckPinyin2(item.Key, srcPinyins);
                if (pys == null) {
                    errorWriter.Write(item.Key);
                    errorWriter.Write("\t");
                    WriteList(errorWriter, item.Value);
                    errorWriter.Write("  // 没有拼音 或 拼音数量出错 \r\n");
                    mDict[item.Key] = pys;
                    continue;
                }
                if (pys == null || IsSome(srcPinyins, pys) == false) {
                    errorWriter.Write(item.Key);
                    errorWriter.Write("\t");
                    WriteList(errorWriter, item.Value);
                    errorWriter.Write("\r\n");
                    mDict[item.Key] = pys;
                }
                rightWriter.Write(item.Key);
                rightWriter.Write("\t");
                WriteList(rightWriter, pys);
                rightWriter.Write("\r\n");
            }
            noneTomeDict.Clear();

            errorWriter.Close();
            fs2.Close();
            rightWriter.Close();
            fs2.Close();
        }
        public void BuildNoneTomeDict()
        {
            foreach (var item in sDict) {
                List<string> pys = new List<string>();
                foreach (var py in item.Value) {
                    var p = RemoveTone(py);
                    if (pys.Contains(p) == false) {
                        pys.Add(p);
                    }
                }
                noneTomeDict[item.Key.ToString()] = pys;
            }

            foreach (var item in sDict2) {
                List<string> pys = new List<string>();
                foreach (var py in item.Value) {
                    var p = RemoveTone(py);
                    if (pys.Contains(p) == false) {
                        pys.Add(p);
                    }
                }
                noneTomeDict[new string(new char[] { item.Key.Item1, item.Key.Item2 })] = pys;
            }
        }



        private bool IsSome(List<string> strList, List<string> strList2)
        {
            if (strList.Count != strList2.Count) {
                return false;
            }
            for (int i = 0; i < strList.Count; i++) {
                if (strList[i] != strList2[i]) {
                    return false;
                }
            }
            return true;
        }
        private List<string> CheckPinyin(List<string> strList2)
        {
            List<string> list = new List<string>();
            foreach (var item in strList2) {
                var py = CheckPinyin(item);
                if (py != null && list.Contains(py) == false) {
                    list.Add(py);
                }
            }
            return list;
        }
        private List<string> CheckPinyin2(string words, List<string> strList2)
        {
            var w = new System.Globalization.StringInfo(words);
            var len = w.LengthInTextElements;
            if (len != strList2.Count) { return null; }

            List<string> list = new List<string>();
            for (int i = 0; i < strList2.Count; i++) {
                var py = strList2[i];
                var rtome = RemoveTone(py);
                var t = w.SubstringByTextElements(i, 1);
                if (noneTomeDict.TryGetValue(t, out List<string> pys)) {
                    if (pys.Contains(rtome) == false) {
                        return null;
                    }
                }
            }

            foreach (var item in strList2) {
                var py = CheckPinyin(item);
                list.Add(py);
            }
            return list;
        }

        private string CheckPinyin(string py)
        {
            if (CanRemoveTone(py)) {
                try {
                    py = py.ToLower();
                    var index = GetToneIndex(py);
                    py = AddTone(RemoveTone(py) + index.ToString());
                } catch (Exception) {
                    return null;
                }
            }
            return py;
        }



        private string RemoveTone(string pinyin)
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

        private string AddTone(string pinyin)
        {
            pinyin = pinyin.ToLower();
            if (pinyin.Contains("j") || pinyin.Contains("q") || pinyin.Contains("x")) {
                pinyin = pinyin.Replace("v", "u");
                pinyin = pinyin.Replace("ü", "u");
            }
            if (pinyin.Contains("iou")) {
                pinyin = pinyin.Replace("iou", "iu");
            }
            if (pinyin.Contains("uei")) {
                pinyin = pinyin.Replace("uei", "ui");
            }
            if (pinyin.Contains("uen")) {
                pinyin = pinyin.Replace("uen", "un");
            }

            if (pinyin.EndsWith("1")) {
                if (pinyin.Contains("a")) {
                    pinyin = pinyin.Replace("a", "ā");
                } else if (pinyin.Contains("o")) {
                    pinyin = pinyin.Replace("o", "ō");
                } else if (pinyin.Contains("e")) {
                    pinyin = pinyin.Replace("e", "ē");

                } else if (pinyin.Contains("iu")) {
                    pinyin = pinyin.Replace("u", "ū");
                } else if (pinyin.Contains("ui")) {
                    pinyin = pinyin.Replace("i", "ī");

                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ū");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "ī");
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

                } else if (pinyin.Contains("iu")) {
                    pinyin = pinyin.Replace("u", "ú");
                } else if (pinyin.Contains("ui")) {
                    pinyin = pinyin.Replace("i", "í");


                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ú");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "í");
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

                } else if (pinyin.Contains("iu")) {
                    pinyin = pinyin.Replace("u", "ǔ");
                } else if (pinyin.Contains("ui")) {
                    pinyin = pinyin.Replace("i", "ǐ");

                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ǔ");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "ǐ");
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

                } else if (pinyin.Contains("iu")) {
                    pinyin = pinyin.Replace("u", "ù");
                } else if (pinyin.Contains("ui")) {
                    pinyin = pinyin.Replace("i", "ì");

                } else if (pinyin.Contains("u")) {
                    pinyin = pinyin.Replace("u", "ù");
                } else if (pinyin.Contains("i")) {
                    pinyin = pinyin.Replace("i", "ì");
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
            pinyin = pinyin.Replace("v", "ü");
            pinyin = Regex.Replace(pinyin, @"\d", "");

            return pinyin;
        }

        private int GetToneIndex(string text)
        {
            if (text == "ǹ") {
                return 4;
            }
            if (text == "ǹg") {
                return 4;
            }
            if (text == "ň") {
                return 3;
            }
            if (text == "ňg") {
                return 3;
            }
            var tone = @"aāáǎàa|oōóǒòo|eēéěèe|iīíǐìi|uūúǔùu|vǖǘǚǜü"
                    .Replace("a", " ").Replace("o", " ").Replace("i", " ")
                    .Replace("u", " ").Replace("v", " ").Replace("ü", " ")
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
        private bool CanRemoveTone(string text)
        {
            List<string> pys = new List<string>(){"fēnwǎ", "shíkě", "bǎikè", "lǐwǎ","líwǎ","pútí",
                "jiālún", "shíwǎ", "máowǎ", "qiānwǎ", "gōngfēn", "qiānkè", "gōngfēn", "gōnglǐ", "yīngmǔ" };
            return !pys.Contains(text);
        }


        #endregion

        #region OutputSingleFile
        public void OutputSingleFile(string file)
        {
            var fullPath = Path.GetFullPath(file);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var fs = File.Open(fullPath, FileMode.Create);
            StreamWriter binaryWriter = new StreamWriter(fs);

            foreach (var item in sDict) {
                binaryWriter.Write(item.Key);
                binaryWriter.Write("\t");
                WriteList(binaryWriter, item.Value);
                binaryWriter.Write("\r\n");
            }

            foreach (var item in sDict2) {
                binaryWriter.Write(new char[] { item.Key.Item1, item.Key.Item2 });
                binaryWriter.Write("\t");
                WriteList(binaryWriter, item.Value);
                binaryWriter.Write("\r\n");
            }
            foreach (var item in mDict) {
                binaryWriter.Write(item.Key);
                binaryWriter.Write("\t");
                WriteList(binaryWriter, item.Value);
                binaryWriter.Write("\r\n");
            }
            binaryWriter.Close();
            fs.Close();

        }

        private static void WriteList(StreamWriter binaryWriter, List<string> List)
        {
            for (int i = 0; i < List.Count; i++) {
                if (i > 0) {
                    binaryWriter.Write(",");
                }
                binaryWriter.Write(List[i]);
            }
        }
        #endregion

        #region CreateZip CreateBr CreateJava CreateJs CreatePython

        public void CreateZip(string zipPath, bool useMin)
        {
            if (useMin) { RemoveChar(); }
            var pyShow = GetPinyins();
            var dict = BuildPinyinDict(pyShow);

            var outPy = BuildSingleWord(pyShow, dict);
            Directory.CreateDirectory(zipPath);
            File.WriteAllBytes(zipPath + "/pyIndex.txt.z", CompressionUtil.GzipCompress(Encoding.UTF8.GetBytes(outPy)));
            if (useMin == false) {
                var outPy2 = BuildData20000(dict);
                File.WriteAllBytes(zipPath + "/pyIndex2.txt.z", CompressionUtil.GzipCompress(Encoding.UTF8.GetBytes(outPy2)));
            }
            var words = BuildMiniWords(pyShow, dict);

            var ls = new List<string>();
            foreach (var item in words) {
                var str = item;
                List<int> pys = dict[str];
                foreach (var py in pys) {
                    str += "," + py.ToString("X");
                }
                ls.Add(str);
            }
            ls = ls.OrderBy(q => q).ToList();
            File.WriteAllBytes(zipPath + "/pyWords.txt.z", CompressionUtil.GzipCompress(Encoding.UTF8.GetBytes(string.Join("\n", ls))));

        }

        public void CreateBr(string brPath, bool useMin)
        {
            if (useMin) { RemoveChar(); }
            var pyShow = GetPinyins();
            var dict = BuildPinyinDict(pyShow);

            var outPy = BuildSingleWord(pyShow, dict);
            Directory.CreateDirectory(brPath);
            File.WriteAllBytes(brPath + "/pyIndex.txt.br", CompressionUtil.BrCompress(Encoding.UTF8.GetBytes(outPy)));
            if (useMin == false) {
                var outPy2 = BuildData20000(dict);
                File.WriteAllBytes(brPath + "/pyIndex2.txt.br", CompressionUtil.BrCompress(Encoding.UTF8.GetBytes(outPy2)));
            }
            var words = BuildMiniWords(pyShow, dict);

            var ls = new List<string>();
            foreach (var item in words) {
                var str = item;
                List<int> pys = dict[str];
                foreach (var py in pys) {
                    str += "," + py.ToString("X");
                }
                ls.Add(str);
            }
            ls = ls.OrderBy(q => q).ToList();
            File.WriteAllBytes(brPath + "/pyWords.txt.br", CompressionUtil.BrCompress(Encoding.UTF8.GetBytes(string.Join("\n", ls))));
        }
        public void CreateJava(string javaPath, bool useMin)
        {
            if (useMin) { RemoveChar(); }
            var pyShow = GetPinyins();
            var dict = BuildPinyinDict(pyShow);

            var outPy = BuildSingleWord(pyShow, dict);
            Directory.CreateDirectory(javaPath);
            File.WriteAllText(javaPath + "/pyIndex.txt", outPy, Encoding.UTF8);
            if (useMin == false) {
                var outPy2 = BuildData20000(dict);
                File.WriteAllText(javaPath + "/pyIndex2.txt", outPy2, Encoding.UTF8);
            }
            var words = BuildMiniWords(pyShow, dict);

            var ls = new List<string>();
            foreach (var item in words) {
                var str = item;
                List<int> pys = dict[str];
                foreach (var py in pys) {
                    str += "," + py.ToString("X");
                }
                ls.Add(str);
            }
            ls = ls.OrderBy(q => q).ToList();
            File.WriteAllText(javaPath + "/pyWords.txt", string.Join("\n", ls), Encoding.UTF8);

        }
        public void CreateJs(string jsPath, bool useMin)
        {
            if (useMin) { RemoveChar(); }
            var upyShow = GetPinyins();


        }
        public void CreatePython(string python, bool useMin)
        {
            if (useMin) { RemoveChar(); }
            var upyShow = GetPinyins();


        }


        internal void RemoveChar()
        {
            var keys = sDict.Keys.ToList();
            foreach (var key in keys) {
                if (key < 0x3400 && key > 0x9fd5) {
                    sDict.Remove(key);
                }
            }
            sDict2.Clear();

            var keys2 = mDict.Keys.ToList();
            foreach (var key in keys2) {
                bool remove = false;
                foreach (var ch in key) {
                    if (ch < 0x3400 && ch > 0x9fd5) {
                        remove = true;
                        break;
                    }
                }
                if (remove) {
                    mDict.Remove(key);
                }
            }
        }

        internal List<string> GetPinyins()
        {
            HashSet<string> pys = new HashSet<string>();
            foreach (var item in sDict) {
                foreach (var py in item.Value) {
                    pys.Add(py);
                }
            }
            foreach (var item in sDict2) {
                foreach (var py in item.Value) {
                    pys.Add(py);
                }
            }
            foreach (var item in mDict) {
                foreach (var py in item.Value) {
                    pys.Add(py);
                }
            }
            var result = new List<string>();
            foreach (var item in pys) {
                result.Add(item);
            }
            result = result.OrderBy(q => q).ToList();

            var pyShow = new List<string>() { "" };
            foreach (var item in result) {
                var py = RemoveTone(item);
                pyShow.Add(py.ToUpper()[0] + py.Substring(1));
                pyShow.Add(item.ToUpper()[0] + item.Substring(1));
            }
            return pyShow;
        }

        internal Dictionary<string, List<int>> BuildPinyinDict(List<string> upyShow)
        {
            Dictionary<string, int> pyDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < upyShow.Count; i += 2) {
                if (pyDict.ContainsKey(upyShow[i]) == false) {
                    pyDict[upyShow[i]] = i - 1;
                }
            }
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            foreach (var item in sDict) {
                var key = item.Key.ToString();
                List<int> list = new List<int>();
                foreach (var py in item.Value) {
                    list.Add(pyDict[py]);
                }
                dict[key] = list;
            }
            foreach (var item in sDict2) {
                var key = new string(new char[] { item.Key.Item1, item.Key.Item2 });
                List<int> list = new List<int>();
                foreach (var py in item.Value) {
                    list.Add(pyDict[py]);
                }
                dict[key] = list;
            }
            foreach (var item in mDict) {
                var key = item.Key;
                List<int> list = new List<int>();
                foreach (var py in item.Value) {
                    list.Add(pyDict[py]);
                }
                dict[key] = list;
            }
            return dict;
        }

        internal string BuildSingleWord(List<string> pyShow, Dictionary<string, List<int>> dict)
        {
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
            return outText;
        }

        internal string BuildData20000(Dictionary<string, List<int>> dict)
        {
            //List<List<string>> pyData20000 = new List<List<string>>();
            string outText = null;
            for (int i = 0xd840; i <= 0xd86e; i++) {
                List<string> data20000 = new List<string>();
                for (int j = 0xdc00; j <= 0xdfff; j++) {
                    if (dict.TryGetValue(new string(new char[] { (char)i, (char)j }), out List<int> indexs)) {
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
                //pyData20000.Add(data20000);
                if (outText != null) {
                    outText += "\n";
                }
                outText += string.Join("\t", data20000);
            }
            return outText;
        }


        internal List<string> BuildMiniWords(List<string> pyShow, Dictionary<string, List<int>> dict)
        {
            Dictionary<string, List<string>> tempClearWords = new Dictionary<string, List<string>>();
            List<string> tempClearKeys = new List<string>();
            foreach (var item in mDict) {
                var sinfo = new StringInfo(item.Key);
                var allSome = true;
                for (int i = 0; i < sinfo.LengthInTextElements; i++) {
                    var t = sinfo.SubstringByTextElements(i, 1);
                    if (pyShow[dict[t][0]] != item.Value[i]) {
                        allSome = false;
                        break;
                    }
                }
                if (allSome) {
                    tempClearWords[item.Key] = item.Value;
                    tempClearKeys.Add(item.Key);
                }
            }

            var pyWords2 = new Dictionary<string, List<string>>();
            foreach (var item in mDict) {
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
                        if (sDict.ContainsKey(key[j]) == false) {
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
                var py = Words.WordsHelper.GetPinyinFast(item.Key, true).ToLower();
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
                //if (item.Length >= 7) { continue; } //排除诗句 歇后语
                var all = wordsSearch.FindAll(item);
                if (all.Any(q => q.End + 1 == item.Length)) {
                    AddKeys2.Add(item);
                }
            }

            AddKeys.AddRange(AddKeys2);
            AddKeys.AddRange(keys);
            AddKeys = AddKeys.Distinct().ToList();

            return AddKeys;
        }

        internal void CreatePyShow(string file, bool useMin)
        {
            if (useMin) { RemoveChar(); }
            var upyShow = GetPinyins();
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllText(file, string.Join(",", upyShow));
        }

        #endregion

 

    }
}
