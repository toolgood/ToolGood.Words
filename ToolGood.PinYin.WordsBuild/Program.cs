using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words;
using System.IO;
using System.Diagnostics;
using Studyzy.IMEWLConverter.IME;

namespace ToolGood.PinYin.WordsBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            SougouPinyinScel file = new SougouPinyinScel();
            var t = file.Import("Scel/虫蛇类名词.scel");
            foreach (var item in t) {
                var t1 = item.PinYinString;
                var t2 = item.PinYinString;

            }



            //tt();
            ////tt3();
            //var dict = new Dictionary<string, string>();
            //var list = new List<string>();
            //for (int i = 2; i < 5; i++) {
            //    GetPyDict(dict, list, i);
            //}
            //var words = dict.Keys.ToList();
            //var pys = dict.Values.ToList();

            //File.WriteAllText("word2.txt", string.Join("|", words));
            //File.WriteAllText("pys.txt", string.Join("\t", pys));

        }

        #region GetPyName
        private static int GetPyName(string name)
        {
            name = name.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "")
                .Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
            if (name.Length > 1) {
                name = name[0] + name.Substring(1).ToLower();
            }
            return pyName.IndexOf(name);
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
             "Ei","Chua","Nou","Tei"
       };
        #endregion




        static void GetPyDict(Dictionary<string, string> dict, List<string> list, int length)
        {
            var fs = File.OpenRead("1.txt");
            StreamReader sr = new StreamReader(fs);

            do {
                var text = sr.ReadLine();
                var sp = text.Split(' ');
                if (sp[0].Length == length) {
                    bool jump = false;
                    var str = "";
                    var s = sp[0].Trim();
                    foreach (var item in list) {
                        if (s.Contains(item)) {
                            str = item;
                            jump = true;
                            break;
                        }
                    }
                    if (jump) {
                        s = s.Replace(str, dict[str]);
                        s = WordHelper.GetPinYinFirst(s).Replace("|", "").ToLower();
                        var py = sp[1].Replace("|", "").ToLower();
                        if (s == py) {
                            jump = true;
                        }
                    }
                    if (jump == false) {
                        dict[s] = sp[1];
                        list.Insert(0, s);
                    }
                }

            } while (sr.EndOfStream == false);



        }


        //查找  长度为2的多拼音
        static void tt3()
        {
            var fs = File.OpenRead("1.txt");
            StreamReader sr = new StreamReader(fs);
            Dictionary<string, string> dict = new Dictionary<string, string>();


            do {
                var text = sr.ReadLine();
                var sp = text.Split(' ');
                if (sp[0].Length == 2) {
                    dict.Add(sp[0], sp[1]);
                }
            } while (sr.EndOfStream == false);
            var list = dict.Keys.ToList();
            var py = dict.Values.ToList();

            File.WriteAllText("词长为2的数组.txt", string.Join("|", list));
        }

        //找查 未包括的拼音
        static void tt2()
        {
            var fs = File.OpenRead("1.txt");
            StreamReader sr = new StreamReader(fs);
            HashSet<string> list = new HashSet<string>();

            do {
                var text = sr.ReadLine();
                var sp = text.Split(' ');
                for (int i = 0; i < sp[0].Length; i++) {
                    var c = sp[0][i];
                    var all = WordHelper.GetAllPinYin(c);
                    var pys = sp[1].Split('|');
                    var py = pys[i].ToUpper();
                    if (py.Length > 0) {
                        py = py[0] + pys[i].Substring(1);
                    }

                    if (all.Count == 0 || all.Contains(py) == false) {
                        list.Add(c + "|" + py);
                    }
                }
            } while (sr.EndOfStream == false);

            File.WriteAllText("2.txt", string.Join("|", list));

        }

        //找查 多音字出错的词汇
        static void tt()
        {
            var fs = File.OpenRead("words.txt");
            StreamReader sr = new StreamReader(fs);
            StringBuilder sb = new StringBuilder();

            do {
                var text = sr.ReadLine();
                var sp = text.Split(' ');
                var py = WordHelper.GetPinYinFirst(sp[0]).ToLower();
                var py2 = sp[1].Replace("|", "");
                if (py != py2) {
                    sb.AppendFormat("{0} {1}\r\n", sp[0], sp[1]);
                }
            } while (sr.EndOfStream == false);

            File.WriteAllText("1.txt", sb.ToString());
        }

    }
}
