using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

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
                if (noneTomeDict.TryGetValue(t,out List<string> pys)) {
                    if (pys.Contains(rtome) ==false) {
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



    }
}
