using Studyzy.IMEWLConverter.IME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToolGood.Words;

namespace ToolGood.PinYin.Pretreatment
{
    class Program
    {
        static void Main(string[] args)
        {
            // 预处理
            // 第一步 处理搜狗词库
            Console.WriteLine("第一步 处理搜狗词库");
            if (File.Exists("scel_1.txt") == false) {
                var scel_1 = GetWords();
                File.WriteAllText("scel_1.txt", string.Join("\n", scel_1));
                scel_1.Clear();
            }
            // 第二步 精简词库              
            Console.WriteLine("第二步 精简词库");
            if (File.Exists("scel_2.txt") == false) {
                var txt = File.ReadAllText("scel_1.txt");
                var lines = txt.Split('\n');
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var item in lines) {
                    var sp = item.Split(' ');
                    dict[sp[0]] = sp[1];
                }
                lines = null;
                txt = null;
                List<string> keys = dict.Select(q => q.Key).OrderBy(q => q.Length).ToList();


                #region 删除通用的
                keys.RemoveAll(q => q.Contains("公司") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("的") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("和") && q.Length > 7);
                keys.RemoveAll(q => q.Contains("大厦") && q.Length > 7);
                keys.RemoveAll(q => q.Contains("或") && q.Length > 7);
                keys.RemoveAll(q => q.Contains("与") && q.Length > 7);
                keys.RemoveAll(q => q.Contains("用") && q.Length > 7);
                #endregion

                #region 删除结尾

                keys.RemoveAll(q => q.EndsWith("厂") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("街道") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("办") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("乡") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("会社") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("大楼") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("症") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("小区") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("社区") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("胡同") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("管委会") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("村") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("液") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("国") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("场") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("室") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("征") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("胶囊") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("处") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("酶") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("片") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("器") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("病") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("证") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("剂") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("钉") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("素") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("术") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("因子") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("细胞") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("系统") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("炎") && q.Length > 5);
                keys.RemoveAll(q => q.EndsWith("区") && q.Length > 7);
                keys.RemoveAll(q => q.EndsWith("寓") && q.Length > 7);
                keys.RemoveAll(q => q.EndsWith("装置") && q.Length > 7);
                #endregion


                keys.RemoveAll(q => q.StartsWith("复方") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("一次性") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("因为") && q.Length > 5);

                keys.RemoveAll(q => q.StartsWith("中国") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("日本") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("德国") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("法国") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("英国") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("美国") && q.Length > 5);

                #region 去省市
                keys.RemoveAll(q => q.StartsWith("蒙古") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("北京") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("天津") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("河北") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("山西") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("内蒙古") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("辽宁") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("吉林") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("黑龙江") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("上海") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("江苏") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("浙江") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("安徽") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("福建") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("江西") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("山东") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("河南") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("湖北") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("湖南") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("广东") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("广西") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("海南") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("四川") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("贵州") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("云南") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("重庆") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("西藏") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("陕西") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("甘肃") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("青海") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("宁夏") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("新疆") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("香港") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("澳门") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("台湾") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("石家庄") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("太原") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("呼和浩特") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("沈阳") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("长春") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("哈尔滨") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("南京") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("杭州") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("合肥") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("福州") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("南昌") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("济南") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("郑州") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("武汉") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("长沙") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("广州") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("南宁") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("海口") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("成都") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("贵阳") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("昆明") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("拉萨") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("西安") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("兰州") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("西宁") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("银川") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("乌鲁木齐") && q.Length > 5);
                keys.RemoveAll(q => q.StartsWith("台北") && q.Length > 5);
                #endregion


                for (int i = 3; i < 6; i++) {
                    var keywords = keys.Where(q => q.Length <= i).ToList();
                    WordsSearch wordsSearch = new WordsSearch();
                    wordsSearch.SetKeywords(keywords);

                    for (int j = keys.Count - 1; j >= 0; j--) {
                        var key = keys[j];
                        if (key.Length <= i) { break; }

                        var all = wordsSearch.FindAll(key);
                        if (all.Count > 1) {
                            // Console.WriteLine(key);
                            //进行拼音测试，相同则删除
                            var py = ReplaceForPinYin(key, all, dict);
                            if (py != null) {
                                py = py.ToLower();
                                if (dict[key].ToLower().Trim() == py) {
                                    keys.RemoveAt(j);
                                }
                            }

                        }
                    }
                    wordsSearch = null;
                    GC.Collect();
                }

                for (int i = keys.Count - 1; i >= 0; i--) {
                    var item = keys[i];
                    if (item.Length < 5) break;
                    var pyf = WordsHelper.GetPinYinFast(item).ToLower();

                    if (dict[item].ToLower().Replace(",", "") == pyf) {
                        keys.RemoveAt(i);
                    }
                }

                var fs = File.Open("scel_2.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                foreach (var item in keys) {
                    sw.WriteLine($"{item} {dict[item]}");
                }
                sw.Close();
                fs.Close();
            }

            // 第三步 获取词的所有拼音
            Console.WriteLine("第三步 获取词的所有拼音");
            if (File.Exists("scel_3.txt") == false) {
                Dictionary<char, HashSet<string>> dict = new Dictionary<char, HashSet<string>>();
                var txt = File.ReadAllText("scel_2.txt");
                var lines = txt.Split('\n');
                foreach (var item in lines) {
                    if (string.IsNullOrWhiteSpace(item)) { continue; }
                    var sp = item.Trim().Split(' ');
                    var pys = sp[1].Split(',');
                    for (int i = 0; i < sp[0].Length; i++) {
                        var c = sp[0][i];
                        HashSet<string> list;
                        if (dict.TryGetValue(c, out list) == false) {
                            list = new HashSet<string>();
                            dict[c] = list;
                        }
                        list.Add(pys[i]);
                    }
                }

                var fs = File.Open("scel_3.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                foreach (var item in dict) {
                    sw.WriteLine($"{item.Key} {string.Join(",", item.Value)}");
                }
                sw.Close();
                fs.Close();
            }

            // 第四步 获取网上的拼音
            Console.WriteLine("第四步 获取网上的拼音");
            if (File.Exists("pinyin_1.txt") == false) {
                var pinyin_1 = GetPinYin();
                File.WriteAllText("pinyin_1.txt", string.Join("\n", pinyin_1));
                pinyin_1.Clear();
            }

            // 第五步 分离 单字拼音 和 词组拼音
            Console.WriteLine("第五步 分离 单字拼音 和 词组拼音");
            if (File.Exists("pinyin_2_one.txt") == false) {
                var txt = File.ReadAllText("pinyin_1.txt");
                var lines = txt.Split('\n');
                List<string> ones = new List<string>();
                List<string> mores = new List<string>();
                foreach (var line in lines) {
                    var sp = line.Split(',');
                    if (GetLength(sp[0]) == 1) {
                        ones.Add(line);
                    } else {
                        mores.Add(line);
                    }
                }
                File.WriteAllText("pinyin_2_one.txt", string.Join("\n", ones));
                File.WriteAllText("pinyin_2_more.txt", string.Join("\n", mores));
                ones.Clear();
                mores.Clear();
            }

            // 第六步 简单 合并 单字拼音， 防止常用拼音被覆盖
            Console.WriteLine("第六步 简单 合并 单字拼音， 防止常用拼音被覆盖");
            if (File.Exists("pinyin_3_one.txt") == false) {
                var txt = File.ReadAllText("pinyin_2_one.txt");
                var lines = txt.Split('\n').ToList();
                lines = lines.OrderBy(q => q).ToList();
                for (int i = lines.Count - 1; i >= 1; i--) {
                    if (lines[i].StartsWith(lines[i - 1])) {
                        lines.RemoveAt(i);
                    }
                }
                File.WriteAllText("pinyin_3_one.txt", string.Join("\n", lines));
            }

            // 第七步 检查 拼音数 与 词组长度不一样的
            Console.WriteLine("第七步 检查 拼音数 与 词组长度不一样的");
            if (File.Exists("pinyin_4_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_2_more.txt");
                var lines = txt.Split('\n');
                List<string> oks = new List<string>();
                List<string> errors = new List<string>();
                foreach (var line in lines) {
                    var sp = line.Split(',');
                    if (GetLength(sp[0]) == sp.Length - 1) {
                        oks.Add(line);
                    } else {
                        errors.Add(line);
                    }
                }
                File.WriteAllText("pinyin_4_ok.txt", string.Join("\n", oks));
                File.WriteAllText("pinyin_4_error.txt", string.Join("\n", errors));
            }

            // 第八步 单字拼音
            Console.WriteLine("第八步 检查 单字拼音 是否出错");
            if (File.Exists("pinyin_5_one.txt") == false) {
                var txt = File.ReadAllText("dict/dict-pinyin.txt");
                var lines = txt.Split('\n').ToList();
                Dictionary<char, List<string>> pysDict = new Dictionary<char, List<string>>();

                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split(" \"': ,\r[]".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0][0];
                    sps.RemoveAt(0);
                    pysDict[key] = sps;
                }

                txt = File.ReadAllText("pinyin_3_one.txt");
                lines = txt.Split('\n').ToList();
                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split(" \"': ,\r[]".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0][0];
                    if (pysDict.ContainsKey(key) == false) {
                        sps.RemoveAt(0);
                        pysDict[key] = sps;
                    }
                }
                Dictionary<char, List<string>> nopysDict = new Dictionary<char, List<string>>();

                for (int i = 0x4e00; i <= 0x9FA5; i++) {
                    var c = (char)i;
                    if (pysDict.ContainsKey(c) == false) {
                        var pys = WordsHelper.GetAllPinYin(c);
                        pysDict[c] = pys;
                        nopysDict[c] = pys;
                    }
                }

                for (int i = 0x3400; i <= 0x4DB5; i++) {
                    var c = (char)i;
                    if (pysDict.ContainsKey(c) == false) {
                        var pys = WordsHelper.GetAllPinYin(c);
                        pysDict[c] = pys;
                        nopysDict[c] = pys;
                    }
                }
 
                List<string> ls = new List<string>();
                foreach (var item in pysDict) {
                    ls.Add($"{item.Key} {string.Join(",", item.Value)}");
                }
                ls = ls.OrderBy(q => q).ToList();

                List<string> ls2 = new List<string>();
                foreach (var item in nopysDict) {
                    ls2.Add($"{item.Key} {string.Join(",", item.Value)}");
                }
                ls2 = ls2.OrderBy(q => q).ToList();

                File.WriteAllText("pinyin_5_one.txt", string.Join("\n", ls));
                File.WriteAllText("pinyin_5_oneNoPy.txt", string.Join("\n", ls2));
            }

            // 第九步 获取拼音集
            Console.WriteLine("第九步 获取拼音集 ");
            if (File.Exists("pinyin_6_pys.txt") == false) {
                var txt = File.ReadAllText("pinyin_5_one.txt");
                var lines = txt.Split('\n').ToList();

                Dictionary<string, string> dict = new Dictionary<string, string>();

                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split(" \"': ,\r".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    for (int j = 1; j < sps.Count; j++) {
                        dict[sps[j]] = RemoveTone(sps[j]);
                    }
                }
                List<string> ls = new List<string>();
                foreach (var item in dict) {
                    ls.Add($"{item.Key} {  item.Value}");
                }
                ls = ls.OrderBy(q => q).ToList();
                File.WriteAllText("pinyin_6_pys.txt", string.Join("\n", ls));
            }





        }


        static string RemoveTone(string pinyin)
        {
            string s = "āáǎàōóǒòēéěèīíǐìūúǔùǖǘǚǜü";
            string t = "aaaaooooeeeeiiiiuuuuvvvvv";
            for (int i = 0; i < s.Length; i++) {
                pinyin = pinyin.Replace(s[i].ToString(), t[i].ToString());
            }
            return pinyin;
        }

        static string ReplaceForPinYin(string key, List<WordsSearchResult> all, Dictionary<string, string> dict)
        {
            LineNode[] lineNodes = new LineNode[key.Length + 1];
            for (int i = 0; i < lineNodes.Length; i++) { lineNodes[i] = new LineNode(); }
            lineNodes[lineNodes.Length - 1].IsEnd = true;

            foreach (var item in all) {
                LineText lineText = new LineText() {
                    Start = item.Start,
                    End = item.End,
                    Words = item.Keyword,
                    PinYin = dict[item.Keyword],
                    NextNode = lineNodes[item.End + 1]
                };
                lineNodes[item.Start].Children.Add(lineText);
            }
            return GetPinYin(lineNodes[0], "");
        }
        static string GetPinYin(LineNode node, string pinyin)
        {
            if (node.IsEnd) {
                return string.Join(",", pinyin.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            foreach (var item in node.Children) {
                var py = pinyin + "," + item.PinYin;
                var s = GetPinYin(item.NextNode, py);
                if (s != null) {
                    return s;
                }
            }
            return null;
        }




        static List<string> GetWords()
        {
            var files = Directory.GetFiles("Scel");
            HashSet<string> list = new HashSet<string>();

            foreach (var file in files) {
                GetWords(file, list);
            }
            return list.OrderBy(q => q).ToList();
        }

        static void GetWords(string file, HashSet<string> list)
        {
            SougouPinyinScel scel = new SougouPinyinScel();
            var t = scel.Import(file);

            foreach (var item in t) {
                var w = item.Word.Trim();
                var py = string.Join(",", item.PinYin);

                list.Add($"{w} {py}");
            }
        }

        static List<string> GetPinYin()
        {
            var files = Directory.GetFiles("dict", "*.txt");
            HashSet<string> list = new HashSet<string>();

            foreach (var file in files) {
                GetPinYin(file, list);
            }
            return list.OrderBy(q => q).ToList();
        }

        static void GetPinYin(string file, HashSet<string> list)
        {
            var text = File.ReadAllText(file);
            var line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in line) {
                if (string.IsNullOrWhiteSpace(l)) {
                    continue;
                }
                var sp = l.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                list.Add(string.Join(",", sp));
            }
        }


        private static int GetLength(string text)
        {
            var stringInfo = new System.Globalization.StringInfo(text);
            return stringInfo.LengthInTextElements;
        }

        public static String Substring(String text, int start, int end)
        {
            var stringInfo = new System.Globalization.StringInfo(text);
            return stringInfo.SubstringByTextElements(start, end);
        }
        public class LineNode
        {
            public bool IsEnd { get; set; }
            public List<LineText> Children = new List<LineText>();
        }

        public class LineText
        {
            public int Start;
            public int End;
            public string Words;
            public string PinYin;
            public LineNode NextNode;
        }

    }
}
