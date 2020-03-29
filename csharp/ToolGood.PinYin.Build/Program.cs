using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using ToolGood.Bedrock;

namespace ToolGood.PinYin.Build
{
    class Program
    {
        static void Main(string[] args)
        {
            buildPinYinDict();
            buildPinYinIndex();
        }

        #region 生成拼音索引
        static void buildPinYinIndex()
        {
            buildPinYinIndex(0x4e00, 0x9fcb, "pyIndex.txt", "pyData.txt");
            //buildPinYinIndex(0x3400, 0x4DB5, "pyIndex2.txt", "pyData2.txt");
        }
        static void buildPinYinIndex(int start, int end, string outIndexFile, string outDataFile)
        {
            var index = start;
            List<HashSet<int>> t = new List<HashSet<int>>();
            while (index <= end) {
                var ch = (char)index;

                HashSet<int> ls = new HashSet<int>();
                var p1 = getPinYin(ch);//此方法查找最常用的拼音， 此处可能会出错, 出错字符如 芃

                HashSet<int> ls2 = new HashSet<int>();
                getPinYin2(ch, ls2);
                if (ls2.Count > 0) {
                    if (ls2.Contains(p1)) {
                        ls.Add(p1);
                    }
                    foreach (var item in ls2) {
                        ls.Add(item);
                    }
                }


                if (ls.Count == 0) {
                    char c;
                    Dict.TraditionalToSimplified(ch, out c);
                    getPinYin2(c, ls);
                }

                if (ls.Count == 0) {
                    var py3 = GetPyName2(ch.ToString());
                    if (py3 > 0) ls.Add(py3);
                    if (ch == '刓') ls.Add(GetPyName("Liang"));
                }
                if (ls.Count == 0) {
                    getPinYin3(ch, ls);
                }

                t.Add(ls);
                index++;
            }

            List<string> ls22 = new List<string>();
            for (int i = start; i <= end; i++) {
                var idx = i - start;
                if (t[idx].Count>0) {
                    string c = ((char)i).ToString();
                    c += " ";
                    c += string.Join(",", t[idx]);
                    ls22.Add(c);
                }
            }
            var outText = string.Join("\n", ls22);
            File.WriteAllText(outIndexFile, outText);

            Compression(outIndexFile);
            //index = 0;
            //List<short> node = new List<short>();
            ////StringBuilder node = new StringBuilder();
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < t.Count; i++) {
            //    var item = t[i];
            //    //node.Append(",");
            //    if (item.Count == 0) {
            //        node.Add(-1);
            //    } else {
            //        node.Add((short)index);
            //        index += item.Count;
            //        foreach (var pyNum in item) {
            //            sb.Append(",");
            //            sb.Append(pyNum);
            //        }
            //    }
            //}
            ////index++;
            //node.Add((short)index);
            //for (int i = node.Count - 1; i >= 0; i--) {
            //    if (node[i] == -1) {
            //        node[i] = node[i + 1];
            //    }

            //}
            //File.WriteAllText(outIndexFile, string.Join(",", node));
            //sb.Remove(0, 1);
            //File.WriteAllText(outDataFile, sb.ToString());
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

        static int getPinYin(char ch)
        {
            try {
                var gpy = PinYinConverter.Get(ch.ToString());
                if (gpy != ch.ToString()) {
                    var py2 = GetPyName(gpy);
                    if (py2 > 0) {
                        return py2;
                    }
                }
            } catch (Exception) { }
            return -1;
        }
        static void getPinYin2(char ch, HashSet<int> ls)
        {
            try {
                var chinese = new ChineseChar(ch);
                for (int i = 0; i < chinese.PinyinCount; i++) {
                    var py = chinese.Pinyins[i];//.Replace("YAI", "YA");
                    var py2 = GetPyName(py);
                    if (py2 == -1) {
                        throw new Exception("");
                    }
                    ls.Add(py2);
                }
            } catch (Exception) { }
        }
        static void getPinYin3(char ch, HashSet<int> ls)
        {
            var texts = File.ReadAllLines("_pyyin.txt");
            foreach (var text in texts) {
                var sp = text.Trim().Split(':');
                if (sp[0][0] == ch) {
                    for (int i = 1; i < sp.Length; i++) {
                        var py = GetPyName(sp[i]);
                        ls.Add(py);
                    }

                }
            }
        }
        #endregion


        #region 生成拼音字典
        static void buildPinYinDict()
        {
            var texts = File.ReadAllLines("_pyyin.txt");
            foreach (var text in texts) {
                var sp = text.Trim().Split(':');
                for (int i = 1; i < sp.Length; i++) {
                    var py = GetPy(sp[i]);
                    if (pyName.Contains(py) == false) {
                        pyName.Add(py);
                    }
                }
            }
            //pyName.Remove("");
            pyName = pyName.Distinct().ToList();
            pyName = pyName.OrderBy(q => q).ToList();
            var str = "\"" + string.Join("\",\"", pyName) + "\"";
            File.WriteAllText("pyName.txt", str);
        }

        private static string GetPy(string name)
        {
            name = name.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "")
                .Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");

            name = name.ToUpper();
            if (name.Length > 1) {
                name = name[0] + name.Substring(1).ToLower();
            }
            return name;
        }
        private static List<string> pyName = new List<string>
 {
            "", "A", "Ai", "An", "Ang", "Ao", "Ba", "Bai", "Ban", "Bang", "Bao", "Bei",
             "Ben", "Beng", "Bi", "Bian", "Biao", "Bie", "Bin", "Bing", "Bo", "Bu",
             "Ba", "Cai", "Can", "Cang", "Cao", "Ce", "Ceng", "Cha", "Chai", "Chan",
             "Chang", "Chao", "Che", "Chen", "Cheng", "Chi", "Chong", "Chou", "Chu",
             "Chuai", "Chuan", "Chuang", "Chui", "Chun", "Chuo", "Ci", "Cong", "Cou",
             "Cu", "Cuan", "Cui", "Cun", "Cuo", "Da", "Dai", "Dan", "Dang", "Dao", "De",
             "Deng", "Di", "Dian", "Diao", "Die", "Ding", "Diu", "Dong", "Dou", "Du",
             "Duan", "Dui", "Dun", "Duo", "E", "En", "Er", "Fa", "Fan", "Fang", "Fei",
             "Fen", "Feng", "Fo", "Fou", "Fu", "Ga", "Gai", "Gan", "Gang", "Gao", "Ge",
             "Gei", "Gen", "Geng", "Gong", "Gou", "Gu", "Gua", "Guai", "Guan", "Guang",
             "Gui", "Gun", "Guo", "Ha", "Hai", "Han", "Hang", "Hao", "He", "Hei", "Hen",
             "Heng", "Hong", "Hou", "Hu", "Hua", "Huai", "Huan", "Huang", "Hui", "Hun",
             "Huo", "Ji", "Jia", "Jian", "Jiang", "Jiao", "Jie", "Jin", "Jing", "Jiong",
             "Jiu", "Ju", "Juan", "Jue", "Jun", "Ka", "Kai", "Kan", "Kang", "Kao", "Ke",
             "Ken", "Keng", "Kong", "Kou", "Ku", "Kua", "Kuai", "Kuan", "Kuang", "Kui",
             "Kun", "Kuo", "La", "Lai", "Lan", "Lang", "Lao", "Le", "Lei", "Leng", "Li",
             "Lia", "Lian", "Liang", "Liao", "Lie", "Lin", "Ling", "Liu", "Long", "Lou",
             "Lu", "Lv", "Luan", "Lue", "Lun", "Luo", "Ma", "Mai", "Man", "Mang", "Mao",
             "Me", "Mei", "Men", "Meng", "Mi", "Mian", "Miao", "Mie", "Min", "Ming", "Miu",
             "Mo", "Mou", "Mu", "Na", "Nai", "Nan", "Nang", "Nao", "Ne", "Nei", "Nen",
             "Neng", "Ni", "Nian", "Niang", "Niao", "Nie", "Nin", "Ning", "Niu", "Nong",
             "Nu", "Nv", "Nuan", "Nue", "Nuo", "O", "Ou", "Pa", "Pai", "Pan", "Pang",
             "Pao", "Pei", "Pen", "Peng", "Pi", "Pian", "Piao", "Pie", "Pin", "Ping",
             "Po", "Pu", "Qi", "Qia", "Qian", "Qiang", "Qiao", "Qie", "Qin", "Qing",
             "Qiong", "Qiu", "Qu", "Quan", "Que", "Qun", "Ran", "Rang", "Rao", "Re",
             "Ren", "Reng", "Ri", "Rong", "Rou", "Ru", "Ruan", "Rui", "Run", "Ruo",
             "Sa", "Sai", "San", "Sang", "Sao", "Se", "Sen", "Seng", "Sha", "Shai",
             "Shan", "Shang", "Shao", "She", "Shen", "Sheng", "Shi", "Shou", "Shu",
             "Shua", "Shuai", "Shuan", "Shuang", "Shui", "Shun", "Shuo", "Si", "Song",
             "Sou", "Su", "Suan", "Sui", "Sun", "Suo", "Ta", "Tai", "Tan", "Tang",
             "Tao", "Te", "Teng", "Ti", "Tian", "Tiao", "Tie", "Ting", "Tong", "Tou",
             "Tu", "Tuan", "Tui", "Tun", "Tuo", "Wa", "Wai", "Wan", "Wang", "Wei",
             "Wen", "Weng", "Wo", "Wu", "Xi", "Xia", "Xian", "Xiang", "Xiao", "Xie",
             "Xin", "Xing", "Xiong", "Xiu", "Xu", "Xuan", "Xue", "Xun", "Ya", "Yan",
             "Yang", "Yao", "Ye", "Yi", "Yin", "Ying", "Yo", "Yong", "You", "Yu",
             "Yuan", "Yue", "Yun", "Za", "Zai", "Zan", "Zang", "Zao", "Ze", "Zei",
             "Zen", "Zeng", "Zha", "Zhai", "Zhan", "Zhang", "Zhao", "Zhe", "Zhen",
             "Zheng", "Zhi", "Zhong", "Zhou", "Zhu", "Zhua", "Zhuai", "Zhuan",
             "Zhuang", "Zhui", "Zhun", "Zhuo", "Zi", "Zong", "Zou", "Zu", "Zuan",
             "Zui", "Zun", "Zuo","Pou","Dia","Cen","Dei","Ca","Nve","Lve","Shei","Zhei",
             "Ei","Chua","Nou","Tei","Yai"
       };
        #endregion



        //private 
        private static Dictionary<string, string> _dict;

        private static Dictionary<string, string> getDict()
        {
            if (_dict == null) {
                var dict2 = "诘|Ji|揲|Ye|棓|Bei|足|Ju|栟|Ben|咯|Luo|迹|Gui|欻|Chua|耨|Nou|埏|Yan|囋|Can|噭|Chi|案|Wan|燝|Zhu|膻|Dan|汝|Zhuang|艹|Ao|磹|Tan|厖|Pang|观|Guang|窾|Kua|搂|Sou|继|Xu|房|Pang|黮|Shen|愬|Shuo|矜|Guan|盻|Pan|射|Ye|景|Ying|潠|Xun|蓧|Di|黈|Tou|从|Zong|洞|Tong|譳|Rou|鸊|Pi|桁|Hang|槱|Chao|被|Pi|擘|Bai|岂|Kai|铦|Kuo|瑱|Zhen|囝|Nan|嬛|Huan|乐|Lao|崚|Leng|蹻|Jue|浰|Li|摵|Se|梴|Yan|嶰|Jie|谌|Shen|撍|Qian|穞|Lu|黾|Meng|隩|Ao|刓|Liang|墄|Qi|擿|Zhe|能|Nan|居|Ji|及|Xi|揭|Qi|吾|Yu|扐|Cai|刓|Shu|啜|Shu|晻|Yan|兼|Xian|忒|Tei|痁|Dian|莫|Mu|宕|Tan|摘|Ti|灒|Cuan|什|Za|适|Di|逤|Suo|螫|Zhe|伈|Xin|扢|Jie|花|Hu|么|Mo|餧|Si|箐|Jing|禜|Ying|庳|Bei|硾|Chui|燋|Zhuo|棽|Shen|濊|Hun|泽|Shi|漱|Shou|摄|Nie|耆|Shi";
                dict2 += "|㘄|Leng|䉄|Leng|䬋|Leng|䮚|Leng|䚏|Leng|䚏|Li|䚏|Lin|㭁|Reng|䖆|Niang";
                var sp = dict2.Split('|');
                _dict = new Dictionary<string, string>();
                for (int i = 0; i < sp.Length; i += 2) {
                    _dict[sp[i]] = sp[i + 1];
                }
            }
            return _dict;
        }

        private static int GetPyName2(string key)
        {
            var dict = getDict();
            string py;
            if (dict.TryGetValue(key, out py)) {
                return pyName.IndexOf(py);
            }
            return -1;
        }


        private static int GetPyName(string name)
        {
            name = name.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "")
                .Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
            name = name.ToUpper();
            if (name.Length > 1) {
                name = name[0] + name.Substring(1).ToLower();
            }
            return pyName.IndexOf(name);
        }



    }
}
