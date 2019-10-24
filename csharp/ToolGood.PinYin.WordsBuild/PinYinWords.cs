using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.PinYin.WordsBuild
{
    public class PinYinWords
    {
        public string Words { get; set; }
        public string PinYins { get; set; }

        private List<string> PinYinList { get; set; }

        public int[] GetPinYinIndex()
        {
            PinYinList = PinYins.Split('\'').ToList();
            int[] pys = new int[Words.Length];
            for (int i = 0; i < Words.Length; i++) {
                pys[i] = GetPyName(PinYinList[i]);
            }
            return pys;
        }

        #region GetPyName
        private  int GetPyName(string name)
        {
            name = name.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "")
                .Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").ToUpper();
            if (name.Length > 1) {
                name = name[0] + name.Substring(1).ToLower();
            }
            return pyName.IndexOf(name);
        }

        #region pyName
        internal readonly static List<string> pyName = new List<string>
        {
          "","A","Ai","An","Ang","Ao","Ba","Ba","Bai","Ban","Bang","Bao","Bei","Ben","Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Bun","Ca","Cai","Cal","Can","Cang","Cao","Ce","Cen","Ceng","Ceon","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng","Chi","Chong","Chou","Chu","Chua","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong","Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De","Dei","Den","Deng","Di","Dia","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dug","Dui","Dun","Duo","E","Ei","En","Eng","Eos","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fenwa","Fo","Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gi","Gong","Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han","Hang","Hao","He","Hei","Hen","Heng","Hol","Hong","Hou","Hu","Hua","Huai","Huan","Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing","Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Kasei","Ke","Ken","Keng","Keop","Keos","Keum","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo","La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang","Liao","Lie","Lin","Ling","Liu","Liwa","Long","Lou","Lu","Luan","Lue","Lun","Luo","Lv","Lve","Ma","Mai","Man","Mang","Mangmi","Mao","Maowa","Mas","Me","Mei","Men","Meng","Mi","Mian","Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Myeong","Na","Nai","Nan","Nang","Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning","Niu","Nong","Nou","Nu","Nuan","Nue","Nun","Nuo","Nv","Nve","O","Oes","Ou","Pa","Pai","Pan","Pang","Pao","Pei","Pen","Peng","Phas","Phdeng","Phos","Pi","Pian","Piao","Pie","Pin","Ping","Po","Pou","Ppun","Pu","Qi","Qia","Qian","Qiang","Qianwa","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu","Quan","Que","Qun","Ramo","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou","Ru","Ruan","Rui","Run","Ruo","Sa","Saeng","Sai","San","Sang","Sao","Se","Sen","Seng","Sha","Shai","Shan","Shang","Shao","She","Shei","Shen","Sheng","Shi","Shiwa","Shou","Shu","Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan","Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Tei","Teng","Teul","Ti","Tian","Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tunwa","Tuo","Wa","Wai","Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao","Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yai","Yan","Yang","Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun","Za","Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan","Zhang","Zhao","Zhe","Zhei","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan","Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
            };
        #endregion
        #endregion




        public override int GetHashCode()
        {
            return Words.GetHashCode();
        }
        public override string ToString()
        {
            return Words + "|" + PinYins;
        }
    }
}
