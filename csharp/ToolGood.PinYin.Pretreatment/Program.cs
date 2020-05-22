using Microsoft.International.Converters.PinYinConverter;
using Studyzy.IMEWLConverter.IME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ToolGood.PinYin.Pretreatment;
using ToolGood.Words;
using ToolGood.Words.internals;

namespace ToolGood.Pinyin.Pretreatment
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // 感谢 hotoo 大佬 整理的拼音 拼音范围 0x4e00-0x9fa5
            // https://github.com/hotoo/pinyin
            // 感谢  mozillazg 大佬 整理的拼音 并指出 拼音来源
            // 拼音来源
            // https://github.com/mozillazg/pinyin-data
            // https://github.com/mozillazg/phrase-pinyin-data

            MozillazgPinyinHelper.ToStandardization();

            var start = "𠀀";// '\ud840' '\udc00' - '\udfff'  
            var end = "𫠝";// '\ud86e' '\udc1d'
            var mid = "𬺓";
            mid = "𬀩";
            StringBuilder stringBuilder = new StringBuilder(start);
            stringBuilder[0] = (char)0xd840;
            stringBuilder[1] = (char)0xdfff;

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
                keys.RemoveAll(q => q.Contains("大厦") && q.Length > 7);
                keys.RemoveAll(q => q.Contains("公司") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("大楼") && q.Length > 5);

                keys.RemoveAll(q => q.Contains("而") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("的") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("和") && q.Length > 7);
                keys.RemoveAll(q => q.Contains("或") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("与") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("用") && q.Length > 5);
                keys.RemoveAll(q => q.Contains("地") && q.Length > 5);
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
                            var py = ReplaceForPinyin(key, all, dict);
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
                    var pyf = WordsHelper.GetPinyinFast(item).ToLower();

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
                var pinyin_1 = GetPinyin();
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
                ones = ones.OrderBy(q => q).ToList();
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
                    line = Regex.Replace(line, "//.*[\r\n]?", "");
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split(" \"': ,\r[]?".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0][0];
                    sps.RemoveAt(0);
                    pysDict[key] = sps;
                }

                txt = File.ReadAllText("pinyin_3_one.txt");
                lines = txt.Split('\n').ToList();
                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    line = Regex.Replace(line, "//.*[\r\n]?", "");
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split(" \"': ,\r[]?".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0][0];
                    if (pysDict.ContainsKey(key) == false) {
                        sps.RemoveAt(0);
                        pysDict[key] = sps;
                    }
                }
                Dictionary<char, List<string>> nopysDict = new Dictionary<char, List<string>>();

                for (int i = 0x3400; i <= 0x9FFF; i++) {
                    var c = (char)i;
                    if (pysDict.ContainsKey(c) == false) {
                        var pys = WordsHelper.GetAllPinyin(c);
                        pysDict[c] = pys;
                        nopysDict[c] = pys;
                    }
                }

                List<string> ls = new List<string>();
                foreach (var item in pysDict) {
                    ls.Add($"{item.Key} {string.Join(",", item.Value)}");
                }

                List<string> ls2 = new List<string>();
                foreach (var item in nopysDict) {
                    ls2.Add($"{item.Key} {string.Join(",", item.Value)}");
                }

                ls = ls.OrderBy(q => q).ToList();
                for (int i = ls.Count - 1; i >= 0; i--) {
                    var l = ls[i];
                    if (l[0] > 0x9FFF) {
                        ls.RemoveAt(i);
                    }
                }


                var str = string.Join("\n", ls);
                File.WriteAllText("pinyin_5_one.txt", string.Join("\n", ls));
                File.WriteAllText("pinyin_5_oneNoPy.txt", string.Join("\n", ls2));
            }
            Console.WriteLine("第八步补 获取拼音集 ");
            if (File.Exists("pinyin_5_1_one.txt") == false) {
                var pyText = File.ReadAllText("pinyin_5_one.txt");
                var pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

                foreach (var line in pyLines) {
                    var sp = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    List<string> pys = new List<string>();

                    for (int i = 1; i < sp.Length; i++) {
                        pys.Add(sp[i]);
                    }
                    dict[sp[0]] = pys;
                }
                for (int i = 0x3400; i <= 0x9fff; i++) {
                    var c = ((char)i);
                    var cStr = c.ToString();
                    if (cStr == "色") {

                    }
                    if (ChineseChar.IsValidChar(c)) {
                        List<string> pys2 = new List<string>();
                        ChineseChar chineseChar = new ChineseChar(c);
                        foreach (string value in chineseChar.Pinyins) {
                            if (string.IsNullOrEmpty(value)) { continue; }
                            if (value.EndsWith("5")==false && value.EndsWith("0")==false) {
                                pys2.Add(AddTone(value));
                            } else {

                            }
                        }
                        List<string> pys;
                        if (dict.TryGetValue(cStr, out pys) == false) {
                            pys = new List<string>();
                        }
                        pys2.RemoveAll(q => pys.Contains(q));
                        if (pys2.Count > 0) {
                            pys.AddRange(pys2);
                            dict[cStr] = pys;
                        }
                    }
                }
                List<string> ls2 = new List<string>();
                foreach (var item in dict) {
                    ls2.Add($"{item.Key} {string.Join(",", item.Value)}");
                }
                File.WriteAllText("pinyin_5_1_one.txt", string.Join("\n", ls2));

                Dictionary<string, List<string>> pysDict = new Dictionary<string, List<string>>();
                var txt = File.ReadAllText("pinyin_3_one.txt");
                var lines = txt.Split('\n').ToList();
                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    line = Regex.Replace(line, "//.*[\r\n]?", "");
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0];
                    sps.RemoveAt(0);
                    pysDict[key] = sps;
                }
                txt = File.ReadAllText("dict/dict-pinyin.txt");
                lines = txt.Split('\n').ToList();
                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    line = Regex.Replace(line, "//.*[\r\n]?", "");
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0];
                    sps.RemoveAt(0);
                    pysDict[key] = sps;
                }
                txt = File.ReadAllText("dict/mozillazg_pinyin.txt");
                lines = txt.Split('\n').ToList();
                for (int i = lines.Count - 1; i >= 0; i--) {
                    var line = lines[i];
                    line = Regex.Replace(line, "//.*[\r\n]?", "");
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    var sps = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sps[0];
                    sps.RemoveAt(0);
                    pysDict[key] = sps;
                }

                List<string> ls3 = new List<string>();
                foreach (var item in pysDict) {
                    if (item.Key.Length == 1) {
                        continue;
                    }
                    if (item.Key[0] >= 0xd840 && item.Key[0] <= 0xd86e) {
                        if (item.Key[1] >= 0xdc00 && item.Key[1] <= 0xdfff) {
                            ls3.Add($"{item.Key} {string.Join(",", item.Value)}");
                        }
                    }
                }

                File.WriteAllText("pinyin_5_2_one.txt", string.Join("\n", ls3));
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
                List<string> ls2 = new List<string>();
                List<string> ls3 = new List<string>();
                List<string> ls4 = new List<string>();
                List<string> ls5 = new List<string>();
                foreach (var item in dict) {
                    ls.Add($"{item.Value} {  item.Key}");
                    ls2.Add("\"" + item.Key.ToUpper()[0] + item.Key.ToLower().Substring(1) + "\"");
                    ls3.Add("\"" + item.Value.ToUpper()[0] + item.Value.ToLower().Substring(1) + "\"");
                    ls4.Add("\"" + item.Value.ToUpper()[0] + item.Value.ToLower().Substring(1) + "\",\"" + item.Key.ToUpper()[0] + item.Key.ToLower().Substring(1) + "\"");
                    ls5.Add(item.Value);
                    ls5.Add(item.Key);
                }

                ls = ls.OrderBy(q => q).ToList();
                ls2 = ls2.OrderBy(q => q).ToList();
                ls3 = ls3.Distinct().OrderBy(q => q).ToList();
                ls4 = ls4.Distinct().OrderBy(q => q).ToList();
                ls5 = ls5.Distinct().OrderBy(q => q).ToList();
                File.WriteAllText("pinyin_6_pys.txt", string.Join("\n", ls));
                File.WriteAllText("pinyin_6_pys_one.txt", string.Join(",", ls2));
                File.WriteAllText("pinyin_6_pys_one2.txt", string.Join(",", ls3));
                File.WriteAllText("pinyin_6_pys_all.txt", string.Join(",", ls4));
                File.WriteAllText("pinyin_6_pys_all2.txt", string.Join(",", ls5));
            }

            Console.WriteLine("第十步 检查拼音集的拼音是否错误 ");
            //if (File.Exists("pinyin_7_pys.txt") == false) {
            //    var txt = File.ReadAllText("pinyin_4_ok.txt");
            //    var lines = txt.Split('\n').ToList();

            //    List<string> ls = new List<string>();
            //    foreach (var line in lines) {
            //        GetBuildPinyin(line, ls);
            //    }

            //    File.WriteAllText("pinyin_7_pys.txt", string.Join("\n", ls));
            //}

            Console.WriteLine("第十一步 检查拼音集的拼音是否错误 ");
            if (File.Exists("pinyin_8_mores_error.txt") == false) {
                var txt = File.ReadAllText("pinyin_4_ok.txt");
                var lines = txt.Split('\n').ToList();

                List<string> errors = new List<string>();
                for (int i = 0; i < lines.Count - 1; i++) {
                    var l1 = lines[i].Split(',');
                    var l2 = lines[i + 1].Split(',');
                    if (l1[0] == l2[0] && lines[i] != lines[i + 1]) {
                        errors.Add(lines[i]);
                        errors.Add(lines[i + 1]);
                    }
                }
                lines.RemoveAll(q => errors.Contains(q));

                File.WriteAllText("pinyin_8_mores_error.txt", string.Join("\n", errors));
                File.WriteAllText("pinyin_8_mores_ok.txt", string.Join("\n", lines));
            }


            Console.WriteLine("第十二步 使用搜狗拼音  ");
            if (File.Exists("pinyin_9_mores_error.txt") == false) {
                var dictTxt = File.ReadAllText("scel_1.txt");
                Dictionary<string, string> dict = new Dictionary<string, string>();
                var dictLines = dictTxt.Split('\n').ToList();
                foreach (var line in dictLines) {
                    if (string.IsNullOrEmpty(line)) {
                        continue;
                    }
                    var sp = line.Split(' ');
                    dict[sp[0]] = sp[1];
                }

                var txt = File.ReadAllText("pinyin_8_mores_ok.txt");
                var lines = txt.Split('\n').ToList();
                List<string> errors = new List<string>();
                List<string> oks = new List<string>();
                List<string> unfind = new List<string>();
                foreach (var line in lines) {
                    var sp = line.Split(',').ToList();
                    if (dict.TryGetValue(sp[0], out string py)) {
                        sp.RemoveAt(0);
                        var py2 = RemoveTone(string.Join(",", sp));
                        if (py.ToLower() == py2.ToLower() || py.ToLower() == py2.ToLower().Replace("v", "u")) {
                            oks.Add(line);
                        } else {

                            //var py3 = WordsHelper.GetAllPinyin(sp[0], true);
                            errors.Add(line + " | " + py);
                        }
                    } else {
                        unfind.Add(line);
                    }
                }
                File.WriteAllText("pinyin_9_mores_error.txt", string.Join("\n", errors));
                File.WriteAllText("pinyin_9_mores_ok.txt", string.Join("\n", oks));
                File.WriteAllText("pinyin_9_mores_unfind.txt", string.Join("\n", unfind));
            }

            Console.WriteLine("第十三步 修复错误拼音  ");
            if (File.Exists("pinyin_10_mores_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_9_mores_error.txt");
                var lines = txt.Split('\n').ToList();

                List<string> ls = new List<string>();
                List<string> errors = new List<string>();
                foreach (var line in lines) {
                    var lsp = line.Split(" |".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    var tarPy = lsp[1].Split(",");
                    var sp = lsp[0].Split(',');

                    for (int i = 1; i < sp.Length; i++) {
                        var py = RemoveTone(sp[i]).ToLower();//.Replace("v", "u");
                        var py2 = RemoveTone(sp[i]).ToLower().Replace("v", "u");
                        var tpy2 = tarPy[i - 1];
                        if (py != tpy2 && py2 != tpy2) {
                            var c = sp[0][i - 1];
                            var pys = WordsHelper.GetAllPinyin(c, true);
                            var count = 0; //读音为1种，则成功
                            var trypy = "";// 读音
                            foreach (var item in pys) {
                                var itemp = RemoveTone(item.ToLower());
                                if (itemp == tarPy[i - 1] || itemp.Replace("v", "u") == tarPy[i - 1]) {
                                    //sp[i] = item.ToLower();
                                    trypy = item.ToLower();
                                    count++;
                                }
                            }
                            if (count == 1) {
                                sp[i] = trypy;
                            }
                        }
                    }
                    var isok = true;
                    for (int i = 1; i < sp.Length; i++) {
                        var py = RemoveTone(sp[i]).ToLower();//.Replace("v", "u");
                        if (py != tarPy[i - 1] && py.Replace("v", "u") != tarPy[i - 1]) {
                            isok = false;
                            break;
                        }
                    }
                    if (isok) {
                        ls.Add(string.Join(",", sp));
                    } else {
                        errors.Add(line);
                    }
                }
                File.WriteAllText("pinyin_10_mores_ok.txt", string.Join("\n", ls));
                File.WriteAllText("pinyin_10_mores_error.txt", string.Join("\n", errors));

            }

            Console.WriteLine("第十四步 添加搜狗拼音词组  ");
            if (File.Exists("pinyin_11_mores_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_9_mores_ok.txt");
                var lines = txt.Split('\n').ToList();
                txt = File.ReadAllText("pinyin_10_mores_ok.txt");
                var lines2 = txt.Split('\n').ToList();
                lines.AddRange(lines2);
                List<string> oldKeys = new List<string>();
                foreach (var item in lines) {
                    var sp = item.Split(',');
                    oldKeys.Add(sp[0]);
                }


                Dictionary<string, int> remove = new Dictionary<string, int>();

                txt = File.ReadAllText("scel_2.txt");
                lines = txt.Split('\n').ToList();
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                foreach (var item in lines) {
                    if (string.IsNullOrWhiteSpace(item)) { continue; }
                    var sp = item.Split(" ,|".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    var key = sp[0];
                    if (oldKeys.Contains(key)) { continue; }
                    sp.RemoveAt(0);

                    var isok = true;
                    for (int i = 0; i < key.Length; i++) {
                        int count;
                        if (remove.TryGetValue(key[i] + sp[i], out count)) {
                            if (i == 0) {
                                if (count > 2) {
                                    isok = false;
                                    break;
                                }
                            } else {
                                isok = false;
                                break;
                            }
                        }
                    }
                    if (isok == false) { continue; }


                    List<string> pys = new List<string>();
                    for (int i = 0; i < key.Length; i++) {
                        var ps = WordsHelper.GetAllPinyin(key[i], true);
                        var count = 0; //读音为1种，则成功
                        var count2 = 0; //读音为1种，则成功
                        var trypy = "";// 读音
                        foreach (var p in ps) {
                            var itemp = RemoveTone(p.ToLower());
                            if (itemp == sp[i] || itemp.Replace("v", "u") == sp[i]) {
                                if (count == 0) { trypy = p.ToLower(); }
                                count++;
                                count2++;
                                if (count > 1 && i == 0 && p.ToLower() == itemp) { count--; }
                                if (count2 > 1) {
                                    remove[key[i] + sp[i]] = count2;
                                }
                            }
                        }
                        if (count == 1) {
                            pys.Add(trypy);
                        }
                    }
                    if (sp.Count == pys.Count) {
                        dict[key] = pys;
                    }
                }

                var ls = new List<string>();
                foreach (var key in dict) {
                    ls.Add($"{key.Key},{string.Join(",", key.Value)}");
                }
                File.WriteAllText("pinyin_11_mores_ok.txt", string.Join("\n", ls));
            }



            Console.WriteLine("第N步 合并拼音组  ");
            if (File.Exists("pinyin_n_mores_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_9_mores_ok.txt");
                var lines = txt.Split('\n').ToList();
                txt = File.ReadAllText("pinyin_10_mores_ok.txt");
                var lines2 = txt.Split('\n').ToList();
                lines.AddRange(lines2);
                txt = File.ReadAllText("pinyin_11_mores_ok.txt");
                lines2 = txt.Split('\n').ToList();
                lines.AddRange(lines2);

                lines = lines.OrderBy(q => q).ToList();
                File.WriteAllText("pinyin_n_mores_ok.txt", string.Join("\n", lines));
            }

            Console.WriteLine("第N+1步 缩小拼音组  ");
            if (File.Exists("pinyin_n1_mores_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_n_mores_ok.txt");
                var lines = txt.Split('\n').ToList();

                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                foreach (var line in lines) {
                    var sp = line.Split(',').ToList();
                    var key = sp[0];
                    sp.RemoveAt(0);
                    dict[key] = sp;
                }

                var keys = dict.Select(q => q.Key).OrderBy(q => q.Length).ToList();
                for (int i = 3; i < 5; i++) {
                    var keywords = keys.Where(q => q.Length <= i).ToList();
                    WordsSearch wordsSearch = new WordsSearch();
                    wordsSearch.SetKeywords(keywords);
                    for (int j = keys.Count - 1; j >= 0; j--) {
                        var k = keys[j];
                        if (k.Length <= i) { break; }
                        var all = wordsSearch.FindAll(k);
                        if (all.Count > 1 && all.Any(q => q.Start == 0) && all.Sum(q => q.Keyword.Length) >= k.Length) {
                            var py = string.Join(",", dict[k]);
                            var rootNode = GetTextNode(k, all);
                            var keyspss = rootNode.GetFullTextLine();
                            foreach (var item in keyspss) {
                                List<string> py2 = new List<string>();
                                var sp = item.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                foreach (var s in sp) {
                                    py2.AddRange(dict[s]);
                                }
                                var py3 = string.Join(",", py2);
                                if (py3 == py) {
                                    keys.RemoveAt(j);
                                    break;
                                }
                            }

                        }
                    }
                }

                var ls = new List<string>();
                foreach (var key in keys) {
                    ls.Add($"{key},{string.Join(",", dict[key])}");
                }
                File.WriteAllText("pinyin_n1_mores_ok.txt", string.Join("\n", ls));
            }

            Console.WriteLine("第N+2步 修复拼音组  ");
            if (File.Exists("pinyin_n2_mores_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_n1_mores_ok.txt");
                var lines = txt.Split('\n').ToList();

                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                foreach (var line in lines) {
                    var sp = line.Split(',').ToList();
                    var key = sp[0];
                    if (string.IsNullOrEmpty(key)) { continue; }
                    sp.RemoveAt(0);

                    for (int i = 0; i < sp.Count; i++) {
                        var py = sp[i];
                        var pyIndex = GetToneIndex(py);
                        py = RemoveTone(py);
                        py = AddTone(py + pyIndex.ToString());
                        sp[i] = py;
                    }

                    dict[key] = sp;
                }
                var ls = new List<string>();
                foreach (var key in dict) {
                    ls.Add($"{key.Key},{string.Join(",", key.Value)}");
                }
                File.WriteAllText("pinyin_n2_mores_ok.txt", string.Join("\n", ls));
            }

            Console.WriteLine("第N+3步 修复拼音组  ");
            if (File.Exists("pinyin_n3_mores_ok.txt") == false) {
                var txt = File.ReadAllText("pinyin_n2_mores_ok.txt");
                var lines = txt.Split('\n').ToList();

                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                var pdict = PinyinDict.PyShow;
                for (int j = 0; j < pdict.Length; j++) {
                    pdict[j] = pdict[j].ToLower();
                }

                foreach (var line in lines) {
                    var sp = line.Split(',').ToList();
                    var key = sp[0];
                    sp.RemoveAt(0);

                    for (int i = 0; i < sp.Count; i++) {
                        var py = sp[i];
                        if (pdict.Contains(py) == false) {
                            var pys = WordsHelper.GetAllPinyin(key[i], true);
                            for (int j = 0; j < pys.Count; j++) {
                                pys[j] = pys[j].ToLower();
                            }
                            var count = 0; //读音为1种，则成功
                            var trypy = "";// 读音
                            var tarPy = RemoveTone(py);
                            foreach (var item in pys) {
                                var itemp = RemoveTone(item.ToLower());
                                if (itemp == tarPy || itemp.Replace("v", "u") == tarPy) {
                                    trypy = item.ToLower();
                                    count++;
                                    if (item.ToLower() == itemp) {
                                        if (count > 1) {
                                            count--;
                                        }
                                    }
                                }
                            }
                            if (count == 1) {
                                py = trypy;
                            }
                        }
                        sp[i] = py;
                    }
                    dict[key] = sp;
                }


                var ls = new List<string>();
                foreach (var key in dict) {
                    ls.Add($"{key.Key},{string.Join(",", key.Value)}");
                }
                File.WriteAllText("pinyin_n3_mores_ok.txt", string.Join("\n", ls));
            }



            // 百度查词组
            //https://hanyu.baidu.com/s?wd=%E6%8C%87%E7%BB%84%E8%AF%8D&from=poem
        }

        static TextNode GetTextNode(string txt, List<WordsSearchResult> results)
        {
            TextNode[] nodes = new TextNode[txt.Length + 1];
            for (int i = 0; i < nodes.Length; i++) { nodes[i] = new TextNode(); }
            nodes[nodes.Length - 1].IsEnd = true;

            foreach (var item in results) {
                TextLine line = new TextLine() {
                    Start = item.Start,
                    End = item.End,
                    Keyword = item.Keyword,
                };
                nodes[item.Start].Children.Add(line);
                line.Next = nodes[item.End + 1];
            }
            return nodes[0];
        }


        static void GetBuildPinyin(string c, List<string> ls)
        {
            var url = "https://www.baidu.com/baidu?wd={0}&ie=utf-8";
            var ch = HttpUtility.UrlEncode(c).ToUpper();
            var u = string.Format(url, ch);

            WebClientEx web = new WebClientEx();
            web.ResetHeaders();
            web.Encoding = Encoding.UTF8;
            var html = web.DownloadString(u);
            html = Regex.Replace(html, "<!.*\n", "");
            html = Regex.Replace(html, @"<head[^>]*?>[\s\S]*?</head>", "");
            html = Regex.Replace(html, @"<script[^>]*?>[\s\S]*?</script>", "");
            html = Regex.Replace(html, @"<style[^>]*?>[\s\S]*?</style>", "");

            HtmlToText convert = new HtmlToText();
            var t = convert.Convert(html);

            var ms = Regex.Matches(t, "(拼音|读音|发音)(:|：)?[a-zāáǎàōóǒòēéěèīíǐìūúǔùǖǘǚǜü].*");
            foreach (Match item in ms) {
                ls.Add($"{c} {item.Value}");
            }
            ms = Regex.Matches(t, "(同|读作|异体字:).+");
            foreach (Match item in ms) {
                ls.Add($"{c} {item.Value}");
            }
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

        static string AddTone(string pinyin)
        {
            pinyin = pinyin.ToLower();
            if (pinyin.Contains("j") || pinyin.Contains("q") || pinyin.Contains("x")) {
                pinyin = pinyin.Replace("v", "u");
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
            pinyin = Regex.Replace(pinyin, @"\d", "");

            return pinyin;
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

        static string ReplaceForPinyin(string key, List<WordsSearchResult> all, Dictionary<string, string> dict)
        {
            LineNode[] lineNodes = new LineNode[key.Length + 1];
            for (int i = 0; i < lineNodes.Length; i++) { lineNodes[i] = new LineNode(); }
            lineNodes[lineNodes.Length - 1].IsEnd = true;

            foreach (var item in all) {
                LineText lineText = new LineText() {
                    Start = item.Start,
                    End = item.End,
                    Words = item.Keyword,
                    Pinyin = dict[item.Keyword],
                    NextNode = lineNodes[item.End + 1]
                };
                lineNodes[item.Start].Children.Add(lineText);
            }
            return GetPinyin(lineNodes[0], "");
        }
        static string GetPinyin(LineNode node, string pinyin)
        {
            if (node.IsEnd) {
                return string.Join(",", pinyin.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            foreach (var item in node.Children) {
                var py = pinyin + "," + item.Pinyin;
                var s = GetPinyin(item.NextNode, py);
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
                var py = string.Join(",", item.Pinyin);

                list.Add($"{w} {py}");
            }
        }

        static List<string> GetPinyin()
        {
            var files = Directory.GetFiles("dict", "*.txt");
            HashSet<string> list = new HashSet<string>();

            foreach (var file in files) {
                GetPinyin(file, list);
            }
            return list.OrderBy(q => q).ToList();
        }

        static void GetPinyin(string file, HashSet<string> list)
        {
            var text = File.ReadAllText(file);
            var line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in line) {
                var t = Regex.Replace(l, "//.*[\r\n]?", "");
                if (string.IsNullOrWhiteSpace(t)) {
                    continue;
                }
                var sp = t.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
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
            public string Pinyin;
            public LineNode NextNode;
        }

    }
}
