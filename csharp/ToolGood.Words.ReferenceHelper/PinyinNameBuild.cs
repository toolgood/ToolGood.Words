using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolGood.Bedrock;

namespace ToolGood.Words.ReferenceHelper
{
    public class PinyinNameBuild
    {

        public void CreateZip(string pyIndex, string pyName, string outFile)
        {
            var dict = BuildPinyinDict(pyIndex, pyName);

            List<string> ls = new List<string>();
            foreach (var item in dict) {
                List<string> idxs = new List<string>();
                foreach (var index in item.Value) {
                    idxs.Add(index.ToString("X"));
                }
                ls.Add($"{item.Key},{string.Join(",", idxs)}");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(outFile));
            File.WriteAllBytes(outFile, CompressionUtil.GzipCompress(Encoding.UTF8.GetBytes(string.Join("\n", ls))));
        }

        public void CreateBr(string pyIndex, string pyName, string outFile)
        {
            var dict = BuildPinyinDict(pyIndex, pyName);

            List<string> ls = new List<string>();
            foreach (var item in dict) {
                List<string> idxs = new List<string>();
                foreach (var index in item.Value) {
                    idxs.Add(index.ToString("X"));
                }
                ls.Add($"{item.Key},{string.Join(",", idxs)}");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(outFile));
            File.WriteAllBytes(outFile, CompressionUtil.BrCompress(Encoding.UTF8.GetBytes(string.Join("\n", ls))));
        }

        public void CreateJava(string pyIndex, string pyName, string outFile)
        {
            var dict = BuildPinyinDict(pyIndex, pyName);

            List<string> ls = new List<string>();
            foreach (var item in dict) {
                List<string> idxs = new List<string>();
                foreach (var index in item.Value) {
                    idxs.Add(index.ToString("X"));
                }
                ls.Add($"{item.Key},{string.Join(",", idxs)}");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(outFile));
            File.WriteAllText(outFile, string.Join("\n", ls));
        }


        public void CreateJs(string pyIndex, string pyName, string outFile)
        {
            var dict = BuildPinyinDict(pyIndex, pyName);


        }
        public void CreatePython(string pyIndex, string pyName, string outFile)
        {
            var dict = BuildPinyinDict(pyIndex, pyName);

        }



        private Dictionary<string, List<int>> BuildPinyinDict(string pyIndex, string pyName)
        {
            var pinyin = LoadPinyin(pyIndex);
            var mDict = LoadPinyinName(pyName);
            return BuildPinyinDict(pinyin, mDict);
        }
        private Dictionary<string, int> LoadPinyin(string pyIndex)
        {
            var txt = File.ReadAllText(pyIndex);
            var upyShow = txt.Split(',');

            Dictionary<string, int> pyDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < upyShow.Length; i += 2) {
                if (pyDict.ContainsKey(upyShow[i]) == false) {
                    pyDict[upyShow[i]] = i - 1;
                }
            }
            return pyDict;
        }
        internal Dictionary<string, List<string>> LoadPinyinName(string pyName)
        {
            var txts = File.ReadAllLines(pyName);
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var txt in txts) {
                var pinyin = txt.Split("\t,:| '\"-=>[]，　?".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                if (Regex.IsMatch(pinyin[0], @"[\t\r\n,.!@#$%^&*()_+\-=;\""''，。？、：“”‘’【】{}]")) {
                    continue;
                }
                for (int i = 1; i < pinyin.Length; i++) {
                    if (Regex.IsMatch(pinyin[i], "[\u3400-\u9fff]")) {
                        continue;
                    }
                }
                var list = new List<string>();
                for (int i = 1; i < pinyin.Length; i++) {
                    list.Add(pinyin[i].Trim());
                }
                result[pinyin[0]] = list;
            }
            return result;
        }


        internal Dictionary<string, List<int>> BuildPinyinDict(List<string> upyShow, Dictionary<string, List<string>> mDict)
        {
            Dictionary<string, int> pyDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < upyShow.Count; i += 2) {
                if (pyDict.ContainsKey(upyShow[i]) == false) {
                    pyDict[upyShow[i]] = i - 1;
                }
            }
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            foreach (var item in mDict) {
                var key = item.Key;
                List<int> list = new List<int>();
                foreach (var py in item.Value) {
                    var py2 = AddTone(py.ToLower());
                    list.Add(pyDict[py2]);
                }
                dict[key] = list;
            }
            return dict;
        }

        internal Dictionary<string, List<int>> BuildPinyinDict(Dictionary<string, int> pyDict, Dictionary<string, List<string>> mDict)
        {
            //Dictionary<string, int> pyDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            //for (int i = 0; i < upyShow.Count; i += 2) {
            //    if (pyDict.ContainsKey(upyShow[i]) == false) {
            //        pyDict[upyShow[i]] = i - 1;
            //    }
            //}
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            foreach (var item in mDict) {
                var key = item.Key;
                List<int> list = new List<int>();
                foreach (var py in item.Value) {
                    var py2 = AddTone(py.ToLower());
                    list.Add(pyDict[py2]);
                }
                dict[key] = list;
            }
            return dict;
        }

        static string AddTone(string pinyin)
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



    }
}
