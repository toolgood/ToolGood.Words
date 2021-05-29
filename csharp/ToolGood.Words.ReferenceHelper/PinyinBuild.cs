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
                        var sp = pinyin.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var py in sp) {
                            AddDict((char)parsedValue, py);
                        }

                    } else {
                        ConvertSmpToUtf16(parsedValue, out char leadingSurrogate, out char trailingSurrogate);
                        var sp = pinyin.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var py in sp) {
                            AddDict(leadingSurrogate, trailingSurrogate, py);
                        }
                    }
                } else {
                    var sp = t.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
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
            var list = new List<string>();
            for (int i = 1; i < pinyin.Length; i++) {
                list.Add(pinyin[i].Trim());
            }
            mDict[words] = list;
        }


        public void OutputSingleFile(string file)
        {
            var fullPath = Path.GetFullPath(file);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var fs = File.Open(fullPath, FileMode.Create);
            StreamWriter binaryWriter = new StreamWriter(fs);

            foreach (var item in sDict) {
                binaryWriter.Write(item.Key);
                binaryWriter.Write(",");
                for (int i = 0; i < item.Value.Count; i++) {
                    if (i>0) {
                        binaryWriter.Write(",");
                    }
                    binaryWriter.Write(item.Value[i]);
                }
                binaryWriter.Write("\r\n");
            }

            foreach (var item in sDict2) {
                binaryWriter.Write(new char[] { item.Key.Item1, item.Key.Item2 });
                binaryWriter.Write(",");
                for (int i = 0; i < item.Value.Count; i++) {
                    if (i > 0) {
                        binaryWriter.Write(",");
                    }
                    binaryWriter.Write(item.Value[i]);
                }
                binaryWriter.Write("\r\n");
            }
            foreach (var item in mDict) {
                binaryWriter.Write(item.Key);
                binaryWriter.Write(",");
                for (int i = 0; i < item.Value.Count; i++) {
                    if (i > 0) {
                        binaryWriter.Write(",");
                    }
                    binaryWriter.Write(item.Value[i]);
                }
                binaryWriter.Write("\r\n");
            }
            binaryWriter.Close();
            fs.Close();

        }

        private static void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate)
        {
            int utf32 = (int)(smpChar - UNICODE_PLANE01_START);
            leadingSurrogate = (char)((utf32 / 0x400) + HIGH_SURROGATE_START);
            trailingSurrogate = (char)((utf32 % 0x400) + LOW_SURROGATE_START);
        }

    }
}
