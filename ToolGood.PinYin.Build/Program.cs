using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace ToolGood.PinYin.Build
{
    class Program
    {
        static void Main(string[] args)
        {
            List<HashSet<int>> t = new List<HashSet<int>>();
            int index = 0x4e00;
            while (index <= 0x9fff) {
                var ch = (char)index;
                HashSet<int> ls = new HashSet<int>();
                try {
                    var chinese = new ChineseChar(ch);
                    var gpy = PinYinConverter.Get(ch.ToString());
                    if (gpy != ch.ToString()) {
                        ls.Add(GetPyName(gpy));
                    }


                    for (int i = 0; i < chinese.PinyinCount; i++) {
                        var py = chinese.Pinyins[i].Replace("YAI","YA");
                        var py2 = GetPyName(py);
                        if (py2==-1) {

                        }
                        ls.Add(py2);
                    }
                } catch (Exception) { }
                t.Add(ls);
                index++;
            }
            index = 0;
            List<short> node = new List<short>();
            //StringBuilder node = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < t.Count; i++) {
                var item = t[i];
                //node.Append(",");
                if (item.Count==0) {
                    node.Add(-1);
                } else {
                    node.Add((short)index);
                    index += item.Count;
                    foreach (var pyNum in item) {
                        sb.Append(",");
                        sb.Append(pyNum);
                    }
                }
            }
            index++;
            node.Add((short)index);
            for (int i = node.Count - 1; i >= 0; i--) {
                if (node[i]==-1) {
                    node[i] = node[i + 1];
                }

            }


            File.WriteAllText("1.txt", string.Join(",",node));
            sb.Remove(0, 1);
            File.WriteAllText("2.txt", sb.ToString());

        }
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
             "Zui", "Zun", "Zuo","Pou","Dia","Cen","Dei","Ca","Nve","Lve","Shei","Zhei"
       };
    }
}
