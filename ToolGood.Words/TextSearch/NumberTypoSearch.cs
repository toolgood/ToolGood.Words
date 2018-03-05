using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ToolGood.Words.internals;

namespace ToolGood.Words
{
    /// <summary>
    /// 数字错字搜索
    /// 目前未针对❿这些符号进行转化,
    /// 建议：先对❿等符号强制转化成数字，然后使用本类
    /// </summary>
    public class NumberTypoSearch
    {
        #region Class
        class TrieNode
        {
            public TrieNode Parent;
            public TrieNode Failure;
            public char Char;
            internal bool End;
            internal List<int> Results;
            internal Dictionary<char, TrieNode> m_values;
            internal Dictionary<char, TrieNode> merge_values;
            private int minflag = int.MaxValue;
            private int maxflag = 0;
            internal int Next;
            private int Count;

            public TrieNode()
            {
                m_values = new Dictionary<char, TrieNode>();
                merge_values = new Dictionary<char, TrieNode>();
                Results = new List<int>();
            }

            public bool TryGetValue(char c, out TrieNode node)
            {
                if (minflag <= (int)c && maxflag >= (int)c) {
                    return m_values.TryGetValue(c, out node);
                }
                node = null;
                return false;
            }

            public TrieNode Add(char c)
            {
                TrieNode node;

                if (m_values.TryGetValue(c, out node)) {
                    return node;
                }

                if (minflag > c) { minflag = c; }
                if (maxflag < c) { maxflag = c; }

                node = new TrieNode();
                node.Parent = this;
                node.Char = c;
                m_values[c] = node;
                Count++;
                return node;
            }

            public void SetResults(int text)
            {
                if (End == false) {
                    End = true;
                }
                if (Results.Contains(text) == false) {
                    Results.Add(text);
                }
            }

            public void Merge(TrieNode node)
            {
                var nd = node;
                while (nd.Char != 0) {
                    foreach (var item in node.m_values) {
                        if (m_values.ContainsKey(item.Key) == false) {
                            if (merge_values.ContainsKey(item.Key) == false) {
                                if (minflag > item.Key) { minflag = item.Key; }
                                if (maxflag < item.Key) { maxflag = item.Key; }
                                merge_values[item.Key] = item.Value;
                                Count++;
                            }
                        }
                    }
                    nd = nd.Failure;
                }
            }

            public int Rank(TrieNode[] has)
            {
                bool[] seats = new bool[has.Length];
                int start = 1;

                has[0] = this;

                Rank(ref start, seats, has);
                int maxCount = has.Length - 1;
                while (has[maxCount] == null) { maxCount--; }
                return maxCount;
            }

            private void Rank(ref int start, bool[] seats, TrieNode[] has)
            {
                if (maxflag == 0) return;
                var keys = m_values.Select(q => (int)q.Key).ToList();
                keys.AddRange(merge_values.Select(q => (int)q.Key).ToList());

                while (has[start] != null) { start++; }
                var s = start < (int)minflag ? (int)minflag : start;

                for (int i = s; i < has.Length; i++) {
                    if (has[i] == null) {
                        var next = i - (int)minflag;
                        //if (next < 0) continue;
                        if (seats[next]) continue;

                        var isok = true;
                        foreach (var item in keys) {
                            if (has[next + item] != null) { isok = false; break; }
                        }
                        if (isok) {
                            SetSeats(next, seats, has);
                            break;
                        }
                    }
                }
                start += keys.Count / 2;

                var keys2 = m_values.OrderByDescending(q => q.Value.Count).ThenByDescending(q => q.Value.maxflag - q.Value.minflag);
                foreach (var key in keys2) {
                    key.Value.Rank(ref start, seats, has);
                }
            }


            private void SetSeats(int next, bool[] seats, TrieNode[] has)
            {
                Next = next;
                seats[next] = true;

                foreach (var item in merge_values) {
                    var position = next + item.Key;
                    has[position] = item.Value;
                }

                foreach (var item in m_values) {
                    var position = next + item.Key;
                    has[position] = item.Value;
                }

            }


        }
        #endregion

        #region 私有变量
        private string[] _keywords;
        private int[][] _guides;
        private int[] _key;
        private int[] _next;
        private int[] _check;
        private int[] _dict;

        /// <summary>
        /// 使用跳词过滤器
        /// </summary> 
        public bool UseSkipWordFilter = false; //使用跳词过滤器
        protected const string _skipList = " \t\r\n~!@#$%^&*()_+-=【】、[]{}|;':\"，。、《》？αβγδεζηθικλμνξοπρστυφχψωΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ⒈⒉⒊⒋⒌⒍⒎⒏⒐⒑⒒⒓⒔⒕⒖⒗⒘⒙⒚⒛㈠㈡㈢㈣㈤㈥㈦㈧㈨㈩①②③④⑤⑥⑦⑧⑨⑩⑴⑵⑶⑷⑸⑹⑺⑻⑼⑽⑾⑿⒀⒁⒂⒃⒄⒅⒆⒇≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√§№☆★○●◎◇◆□℃‰€■△▲※→←↑↓〓¤°＃＆＠＼︿＿￣―♂♀┌┍┎┐┑┒┓─┄┈├┝┞┟┠┡┢┣│┆┊┬┭┮┯┰┱┲┳┼┽┾┿╀╁╂╃└┕┖┗┘┙┚┛━┅┉┤┥┦┧┨┩┪┫┃┇┋┴┵┶┷┸┹┺┻╋╊╉╈╇╆╅╄";
        protected bool[] _skipBitArray;

        /// <summary>
        /// 使用黑名单过滤器
        /// </summary>
        public bool UseBlacklistFilter = false;
        protected int[] _blacklist;


        private int _maxJump = 5;//设置一个最大大跳跃点
        private char[] toWord;
        #endregion


        #region 构造函数
        private const string baseTypo =
          "0 0\r1 1\r2 2\r3 3\r4 4\r5 5\r6 6\r7 7\r8 8\r9 9\r" +
          "０ 0\r１ 1\r２ 2\r３ 3\r４ 4\r５ 5\r６ 6\r７ 7\r８ 8\r９ 9\r" +
          "o 0\rO 0\rl 1\r";

        private const string defaultTypo = "⓪ 0\r零 0\rº 0\r₀ 0\r⓿ 0\r○ 0\r〇 0\r" +
                                         "一 1\r二 2\r三 3\r四 4\r五 5\r六 6\r七 7\r八 8\r九 9\r" +
                                         "壹 1\r贰 2\r叁 3\r肆 4\r伍 5\r陆 6\r柒 7\r捌 8\r玖 9\r" +
                                          "— 1\r貮 2\r参 3\r陸 6\r" +
                                         "¹ 1\r² 2\r³ 3\r⁴ 4\r⁵ 5\r⁶ 6\r⁷ 7\r⁸ 8\r⁹ 9\r" +
                                         "₁ 1\r₂ 2\r₃ 3\r₄ 4\r₅ 5\r₆ 6\r₇ 7\r₈ 8\r₉ 9\r" +
                                         "① 1\r② 2\r③ 3\r④ 4\r⑤ 5\r⑥ 6\r⑦ 7\r⑧ 8\r⑨ 9\r" +
                                         "⑴ 1\r⑵ 2\r⑶ 3\r⑷ 4\r⑸ 5\r⑹ 6\r⑺ 7\r⑻ 8\r⑼ 9\r" +
                                         "⒈ 1\r⒉ 2\r⒊ 3\r⒋ 4\r⒌ 5\r⒍ 6\r⒎ 7\r⒏ 8\r⒐ 9\r" +
                                         "❶ 1\r❷ 2\r❸ 3\r❹ 4\r❺ 5\r❻ 6\r❼ 7\r❽ 8\r❾ 9\r" +
                                         "➀ 1\r➁ 2\r➂ 3\r➃ 4\r➄ 5\r➅ 6\r➆ 7\r➇ 8\r➈ 9\r" +
                                         "➊ 1\r➋ 2\r➌ 3\r➍ 4\r➎ 5\r➏ 6\r➐ 7\r➑ 8\r➒ 9\r" +
                                         "㈠ 1\r㈡ 2\r㈢ 3\r㈣ 4\r㈤ 5\r㈥ 6\r㈦ 7\r㈧ 8\r㈨ 9\r" +
                                         "⓵ 1\r⓶ 2\r⓷ 3\r⓸ 4\r⓹ 5\r⓺ 6\r⓻ 7\r⓼ 8\r⓽ 9\r" +
                                         "㊀ 1\r㊁ 2\r㊂ 3\r㊃ 4\r㊄ 5\r㊅ 6\r㊆ 7\r㊇ 8\r㊈ 9\r";

        /// <summary>
        ///  特殊数字符号转成常用数字
        ///  数字同音汉字转成常用数字
        ///  符号【点】转【.】
        /// --------------------以下为【数字同音汉字转成常用数字】-------------
        ///  "yi": "一以已意议义益亿易医艺食依移衣异伊仪宜射遗疑毅谊亦疫役忆抑尾乙译翼蛇溢椅沂泄逸蚁夷邑怡绎彝裔姨熠贻矣屹颐倚诣胰奕翌疙弈轶蛾驿壹猗臆弋铱旖漪迤佚翊诒怿痍懿饴峄揖眙镒仡黟肄咿翳挹缢呓刈咦嶷羿钇殪荑薏蜴镱噫癔苡悒嗌瘗衤佾埸圯舣酏劓",
        ///  "er": "而二尔儿耳迩饵洱贰铒珥佴鸸鲕",
        ///  "san": "三参散伞叁糁馓毵",
        ///  "shan": "山单善陕闪衫擅汕扇掺珊禅删膳缮赡鄯栅煽姗跚鳝嬗潸讪舢苫疝掸膻钐剡蟮芟埏彡骟",
        ///  "si": "司四思斯食私死似丝饲寺肆撕泗伺嗣祀厮驷嘶锶俟巳蛳咝耜笥纟糸鸶缌澌姒汜厶兕",
        ///  "shi": "是时实事市十使世施式势视识师史示石食始士失适试什泽室似诗饰殖释驶氏硕逝湿蚀狮誓拾尸匙仕柿矢峙侍噬嗜栅拭嘘屎恃轼虱耆舐莳铈谥炻豕鲥饣螫酾筮埘弑礻蓍鲺贳",
        ///  "wu": "务物无五武午吴舞伍污乌误亡恶屋晤悟吾雾芜梧勿巫侮坞毋诬呜钨邬捂鹜兀婺妩於戊鹉浯蜈唔骛仵焐芴鋈庑鼯牾怃圬忤痦迕杌寤阢",
        ///  "liu": "流刘六留柳瘤硫溜碌浏榴琉馏遛鎏骝绺镏旒熘鹨锍",
        ///  "qi": "企其起期气七器汽奇齐启旗棋妻弃揭枝歧欺骑契迄亟漆戚岂稽岐琦栖缉琪泣乞砌祁崎绮祺祈凄淇杞脐麒圻憩芪伎俟畦耆葺沏萋骐鳍綦讫蕲屺颀亓碛柒啐汔綮萁嘁蛴槭欹芑桤丌蜞",
        ///  "ba": "把八巴拔伯吧坝爸霸罢芭跋扒叭靶疤笆耙鲅粑岜灞钯捌菝魃茇",
        ///  "jiu": "就究九酒久救旧纠舅灸疚揪咎韭玖臼柩赳鸠鹫厩啾阄桕僦鬏",
        ///  "ling": "领令另零灵龄陵岭凌玲铃菱棱伶羚苓聆翎泠瓴囹绫呤棂蛉酃鲮柃",
        ///  "lin": "林临邻赁琳磷淋麟霖鳞凛拎遴蔺吝粼嶙躏廪檩啉辚膦瞵懔",
        /// 注：
        /// 多音字类型一：食（转成4）、栅（转成3）、似（转成4）、俟、耆
        /// 多音字类型二：射、尾、蛇、泄、疙、蛾、诒、嗌、参(转成3）、单、掺、禅、掸、剡、彡(转成3)、纟、糸、泽、殖、
        ///     硕、匙、峙（转成4）、嘘、酾、亡、恶、於、唔(转成5)、碌、揭、枝、亟、稽、缉、伎、啐、丌、伯、棱
        /// 特列汉字：删（不转化）、十、拾
        /// </summary>
        private const string miniPinyin =
                "以 1\r已 1\r意 1\r议 1\r义 1\r益 1\r亿 1\r易 1\r医 1\r艺 1\r依 1\r移 1\r衣 1\r异 1\r伊 1\r仪 1\r宜 1\r遗 1\r疑 1\r毅 1\r谊 1\r亦 1\r疫 1\r役 1\r忆 1\r抑 1\r乙 1\r译 1\r翼 1\r溢 1\r椅 1\r沂 1\r逸 1\r蚁 1\r夷 1\r邑 1\r绎 1\r彝 1\r裔 1\r姨 1\r矣 1\r屹 1\r颐 1\r倚 1\r诣 1\r胰 1\r翌 1\r臆 1\r铱 1\r揖 1\r肄 1\r" +
                "而 2\r尔 2\r儿 2\r耳 2\r饵 2\r洱 2\r" +
                "散 3\r伞 3\r山 3\r善 3\r陕 3\r闪 3\r衫 3\r擅 3\r汕 3\r扇 3\r珊 3\r删 3\r膳 3\r缮 3\r赡 3\r煽 3\r苫 3\r" +
                "司 4\r思 4\r斯 4\r私 4\r死 4\r丝 4\r饲 4\r寺 4\r撕 4\r伺 4\r嗣 4\r嘶 4\r巳 4\r是 4\r时 4\r实 4\r事 4\r市 4\r使 4\r世 4\r施 4\r式 4\r势 4\r视 4\r识 4\r师 4\r史 4\r示 4\r石 4\r始 4\r士 4\r失 4\r适 4\r试 4\r什 4\r室 4\r诗 4\r饰 4\r释 4\r驶 4\r氏 4\r逝 4\r湿 4\r蚀 4\r狮 4\r誓 4\r尸 4\r仕 4\r柿 4\r矢 4\r侍 4\r噬 4\r嗜 4\r拭 4\r屎 4\r恃 4\r虱 4\r" +
                "务 5\r物 5\r无 5\r武 5\r午 5\r吴 5\r舞 5\r污 5\r乌 5\r误 5\r屋 5\r晤 5\r悟 5\r吾 5\r雾 5\r芜 5\r梧 5\r勿 5\r巫 5\r侮 5\r坞 5\r毋 5\r诬 5\r呜 5\r钨 5\r捂 5\r戊 5\r" +
                "流 6\r刘 6\r留 6\r柳 6\r瘤 6\r硫 6\r溜 6\r榴 6\r琉 6\r馏 6\r" +
                "企 7\r其 7\r起 7\r期 7\r气 7\r器 7\r汽 7\r奇 7\r齐 7\r启 7\r旗 7\r棋 7\r妻 7\r弃 7\r歧 7\r欺 7\r骑 7\r契 7\r迄 7\r漆 7\r戚 7\r岂 7\r栖 7\r泣 7\r乞 7\r砌 7\r祁 7\r崎 7\r祈 7\r凄 7\r脐 7\r畦 7\r沏 7\r讫 7\r" +
                "把 8\r巴 8\r拔 8\r吧 8\r坝 8\r爸 8\r霸 8\r罢 8\r芭 8\r跋 8\r扒 8\r叭 8\r靶 8\r疤 8\r笆 8\r耙 8\r" +
                "就 9\r究 9\r酒 9\r久 9\r救 9\r旧 9\r纠 9\r舅 9\r灸 9\r疚 9\r揪 9\r咎 9\r韭 9\r臼 9\r厩 9\r" +
                "领 0\r令 0\r另 0\r灵 0\r龄 0\r陵 0\r岭 0\r凌 0\r玲 0\r铃 0\r菱 0\r伶 0\r羚 0\r林 0\r临 0\r邻 0\r赁 0\r琳 0\r磷 0\r淋 0\r霖 0\r鳞 0\r凛 0\r拎 0\r吝 0\r";

        private const string pinyinAppend = "勼 9\r食 4\r栅 3\r似 4\r彡 3\r峙 4\r唔 5\r" +
                "怡 1\r熠 1\r贻 1\r奕 1\r弈 1\r轶 1\r驿 1\r猗 1\r弋 1\r旖 1\r漪 1\r迤 1\r佚 1\r翊 1\r怿 1\r痍 1\r懿 1\r饴 1\r峄 1\r眙 1\r镒 1\r仡 1\r黟 1\r咿 1\r翳 1\r挹 1\r缢 1\r呓 1\r刈 1\r咦 1\r嶷 1\r羿 1\r钇 1\r殪 1\r荑 1\r薏 1\r蜴 1\r镱 1\r噫 1\r癔 1\r苡 1\r悒 1\r瘗 1\r衤 1\r佾 1\r埸 1\r圯 1\r舣 1\r酏 1\r劓 1\r" +
                "迩 2\r铒 2\r珥 2\r佴 2\r鸸 2\r鲕 2\r" +
                "糁 3\r馓 3\r毵 3\r鄯 3\r姗 3\r跚 3\r鳝 3\r嬗 3\r潸 3\r讪 3\r舢 3\r疝 3\r膻 3\r钐 3\r蟮 3\r芟 3\r埏 3\r骟 3\r" +
                "泗 4\r祀 4\r厮 4\r驷 4\r锶 4\r蛳 4\r咝 4\r耜 4\r笥 4\r鸶 4\r缌 4\r澌 4\r姒 4\r汜 4\r厶 4\r兕 4\r轼 4\r舐 4\r莳 4\r铈 4\r谥 4\r炻 4\r豕 4\r鲥 4\r饣 4\r螫 4\r筮 4\r埘 4\r弑 4\r礻 4\r蓍 4\r鲺 4\r贳 4\r" +
                "邬 5\r鹜 5\r兀 5\r婺 5\r妩 5\r鹉 5\r浯 5\r蜈 5\r骛 5\r仵 5\r焐 5\r芴 5\r鋈 5\r庑 5\r鼯 5\r牾 5\r怃 5\r圬 5\r忤 5\r痦 5\r迕 5\r杌 5\r寤 5\r阢 5\r" +
                "浏 6\r遛 6\r鎏 6\r骝 6\r绺 6\r镏 6\r旒 6\r熘 6\r鹨 6\r锍 6\r" +
                "岐 7\r琦 7\r琪 7\r绮 7\r祺 7\r淇 7\r杞 7\r麒 7\r圻 7\r憩 7\r芪 7\r葺 7\r萋 7\r骐 7\r鳍 7\r綦 7\r蕲 7\r屺 7\r颀 7\r亓 7\r碛 7\r汔 7\r綮 7\r萁 7\r嘁 7\r蛴 7\r槭 7\r欹 7\r芑 7\r桤 7\r蜞 7\r" +
                "鲅 8\r粑 8\r岜 8\r灞 8\r钯 8\r菝 8\r魃 8\r茇 8\r" +
                "柩 9\r赳 9\r鸠 9\r鹫 9\r啾 9\r阄 9\r桕 9\r僦 9\r鬏 9\r" +
                "苓 0\r聆 0\r翎 0\r泠 0\r瓴 0\r囹 0\r绫 0\r呤 0\r棂 0\r蛉 0\r酃 0\r鲮 0\r柃 0\r麟 0\r遴 0\r蔺 0\r粼 0\r嶙 0\r躏 0\r廪 0\r檩 0\r啉 0\r辚 0\r膦 0\r瞵 0\r懔 0\r"
            ;



        public NumberTypoSearch() : base()
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            for (int i = 0; i < _skipList.Length; i++) {
                _skipBitArray[_skipList[i]] = true;
            }
            _blacklist = new int[0];

            toWord = new char[char.MaxValue + 1];
            //toWordDict = new Dictionary<char, string>();
            buildWordConverter(miniPinyin);
            buildWordConverter(pinyinAppend);

            buildWordConverter(defaultTypo);
            buildWordConverter(baseTypo);
        }

        private void buildWordConverter(string typoText)
        {
            var ts = typoText.Replace("\n", "").Split('\r');
            var splits = " \t".ToCharArray();
            foreach (var t in ts) {
                var sp = t.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length < 2) continue;
                toWord[(int)sp[0][0]] = sp[1][0];
            }
        }
        #endregion

        #region 查找 替换 查找第一个关键字 判断是否包含关键字

        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public List<IllegalWordsSearchResult> FindAll(string text, BlacklistType blacklist = BlacklistType.All)
        {
            return FindAll(text, (int)blacklist);
        }
        /// <summary>
        /// 在文本中查找所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public List<IllegalWordsSearchResult> FindAll(string text, int flag)
        {
            List<IllegalWordsSearchResult> results = new List<IllegalWordsSearchResult>();
            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            int jump = 0;

            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        if (TestBlacklist(findIndex, flag)) {
                            foreach (var item in _guides[findIndex]) {
                                var r = GetIllegalResult(_keywords[item], i - 1, text, p, pIndex);
                                if (r != null) { results.Add(r); }
                            }
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }

            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    foreach (var item in _guides[findIndex]) {
                        var r = GetIllegalResult(_keywords[item], text.Length - 1, text, p, pIndex);
                        if (r != null) { results.Add(r); }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public IllegalWordsSearchResult FindFirst(string text, BlacklistType blacklist = BlacklistType.All)
        {
            return FindFirst(text, (int)blacklist);
        }
        /// <summary>
        /// 在文本中查找第一个关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public IllegalWordsSearchResult FindFirst(string text, int flag)
        {
            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            int jump = 0;

            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        if (TestBlacklist(findIndex, flag)) {
                            foreach (var item in _guides[findIndex]) {
                                var r = GetIllegalResult(_keywords[item], i - 1, text, p, pIndex);
                                if (r != null) return r;
                            }
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }

            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    foreach (var item in _guides[findIndex]) {
                        var r = GetIllegalResult(_keywords[item], text.Length - 1, text, p, pIndex);
                        if (r != null) return r;
                    }
                }
            }


            return null;
        }



        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public bool ContainsAny(string text, BlacklistType blacklist = BlacklistType.All)
        {
            return ContainsAny(text, (int)blacklist);
        }
        /// <summary>
        /// 判断文本是否包含关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public bool ContainsAny(string text, int flag)
        {
            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            int jump = 0;

            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        if (TestBlacklist(findIndex, flag)) {
                            var s = FindStart(_keywords[_guides[findIndex][0]], i - 1, text, p, pIndex);
                            if (s != -1) { return true; }
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }

            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    var s = FindStart(_keywords[_guides[findIndex][0]], text.Length - 1, text, p, pIndex);
                    if (s != -1) { return true; }
                }
            }
            return false;
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <param name="blacklist">黑名单</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar = '*', BlacklistType blacklist = BlacklistType.All)
        {
            return Replace(text, replaceChar, (int)blacklist);
        }

        /// <summary>
        /// 在文本中替换所有的关键字
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replaceChar">替换符</param>
        /// <param name="flag">黑名单</param>
        /// <returns></returns>
        public string Replace(string text, char replaceChar, int flag)
        {
            StringBuilder result = new StringBuilder(text);

            int[] pIndex = new int[text.Length];
            var p = 0;
            int findIndex = 0;
            int jump = 0;


            for (int i = 0; i < text.Length; i++) {
                var c = toWord[text[i]];
                if (p != 0) {
                    pIndex[i] = p;
                    if (findIndex != 0 && c == 0) {
                        if (TestBlacklist(findIndex, flag)) {
                            var keyword = _keywords[_guides[findIndex][0]];
                            var start = FindStart(keyword, i - 1, text, p, pIndex);
                            if (start != -1) {
                                for (int j = start; j < i; j++) { result[j] = replaceChar; }
                            }
                        }
                    }
                }

                if (UseSkipWordFilter && _skipBitArray[c]) { findIndex = 0; continue; }//使用跳词
                var t = _dict[ToSenseWord(c)];
                if (t == 0) { jump++; if (jump == _maxJump) { p = 0; findIndex = 0; jump = 0; } continue; }//不在字表中，跳过

                var next = _next[p] + t;
                bool find = _key[next] == t;
                if (find == false && p != 0) {
                    p = 0;
                    next = _next[0] + t;
                    find = _key[next] == t;
                }
                if (find) {
                    findIndex = _check[next];
                    p = next;
                }
            }
            if (findIndex != 0) {
                if (TestBlacklist(findIndex, flag)) {
                    var keyword = _keywords[_guides[findIndex][0]];
                    var start = FindStart(keyword, text.Length - 1, text, p, pIndex);
                    if (start != -1) {
                        for (int j = start; j < text.Length; j++) {
                            result[j] = replaceChar;
                        }
                    }
                }
            }
            return result.ToString();
        }


        protected bool IsEnglishOrNumber(char c)
        {
            if (c < 128) {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                    return true;
                }
            }
            return false;
        }

        private int FindStart(string keyword, int end, string srcText, int p, int[] pIndex)
        {
            var n = keyword.Length;
            var start = end;
            int pp = p;
            while (n > 0) {
                var pi = pIndex[start--];
                if (pi != pp) { n--; pp = pi; }
                if (start == -1) return 0;
            }
            if (IsEnglishOrNumber(srcText[start])) {
                return -1;
            }
            start++;
            return start;
        }

        private IllegalWordsSearchResult GetIllegalResult(string keyword, int end, string srcText, int p, int[] pIndex)
        {
            var start = FindStart(keyword, end, srcText, p, pIndex);
            if (start == -1) {
                return null;
            }
            return new IllegalWordsSearchResult(keyword, start, end, srcText);
        }

        private bool TestBlacklist(int index, int flag)
        {
            if (UseBlacklistFilter) {
                var b = _blacklist[index];
                return (b | flag) == b;
            }
            return true;
        }
        #endregion


        #region 保存到文件
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var fs = File.Open(filePath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            Save(bw);
#if NETSTANDARD1_3
            bw.Dispose();
            fs.Dispose();
#else
            bw.Close();
            fs.Close();
#endif
        }

        /// <summary>
        /// 保存到Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            Save(bw);
#if NETSTANDARD1_3
            bw.Dispose();
#else
            bw.Close();
#endif
        }

        protected internal virtual void Save(BinaryWriter bw)
        {
            bw.Write(_keywords.Length);
            foreach (var item in _keywords) {
                bw.Write(item);
            }

            List<int> guideslist = new List<int>();
            guideslist.Add(_guides.Length);
            foreach (var guide in _guides) {
                guideslist.Add(guide.Length);
                foreach (var item in guide) {
                    guideslist.Add(item);
                }
            }
            var bs = IntArrToByteArr(guideslist.ToArray());
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_key);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_next);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_check);
            bw.Write(bs.Length);
            bw.Write(bs);

            bs = IntArrToByteArr(_dict);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseSkipWordFilter);
            bs = BoolArrToByteArr(_skipBitArray);
            bw.Write(bs.Length);
            bw.Write(bs);

            bw.Write(UseBlacklistFilter);
            bs = IntArrToByteArr(_blacklist);
            bw.Write(bs.Length);
            bw.Write(bs);
            bw.Write(_maxJump);
            bs = CharArrToByteArr(toWord);
            bw.Write(bs.Length);
            bw.Write(bs);
        }

        private byte[] CharArrToByteArr(char[] intArr)
        {
            int intSize = sizeof(char) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }

        protected byte[] IntArrToByteArr(int[] intArr)
        {
            int intSize = sizeof(int) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        protected byte[] BoolArrToByteArr(bool[] intArr)
        {
            int intSize = sizeof(bool) * intArr.Length;
            byte[] bytArr = new byte[intSize];
            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
            return bytArr;
        }
        #endregion

        #region 加载文件
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Load(string filePath)
        {
            var fs = File.OpenRead(filePath);
            BinaryReader br = new BinaryReader(fs);
            Load(br);
#if NETSTANDARD1_3
            br.Dispose();
            fs.Dispose();
#else
            br.Close();
            fs.Close();
#endif
        }
        /// <summary>
        /// 加载Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            Load(br);
#if NETSTANDARD1_3
            br.Dispose();
#else
            br.Close();
#endif
        }

        protected internal virtual void Load(BinaryReader br)
        {
            var length = br.ReadInt32();
            _keywords = new string[length];
            for (int i = 0; i < length; i++) {
                _keywords[i] = br.ReadString();
            }

            length = br.ReadInt32();
            var bs = br.ReadBytes(length);
            using (MemoryStream ms = new MemoryStream(bs)) {
                BinaryReader b = new BinaryReader(ms);
                var length2 = b.ReadInt32();
                _guides = new int[length2][];
                for (int i = 0; i < length2; i++) {
                    var length3 = b.ReadInt32();
                    _guides[i] = new int[length3];
                    for (int j = 0; j < length3; j++) {
                        _guides[i][j] = b.ReadInt32();
                    }
                }
            }

            length = br.ReadInt32();
            _key = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _next = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _check = ByteArrToIntArr(br.ReadBytes(length));

            length = br.ReadInt32();
            _dict = ByteArrToIntArr(br.ReadBytes(length));



            UseSkipWordFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _skipBitArray = ByteArrToBoolArr(br.ReadBytes(length));

            UseBlacklistFilter = br.ReadBoolean();
            length = br.ReadInt32();
            _blacklist = ByteArrToIntArr(br.ReadBytes(length));

            _maxJump = br.ReadInt32();
            length = br.ReadInt32();
            toWord = ByteArrToCharArr(br.ReadBytes(length));

        }

        private char[] ByteArrToCharArr(byte[] btArr)
        {
            int intSize = btArr.Length / sizeof(char);
            char[] intArr = new char[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, intSize);
            return intArr;
        }

        protected int[] ByteArrToIntArr(byte[] btArr)
        {
            int intSize = btArr.Length / sizeof(int);
            int[] intArr = new int[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, intSize);
            return intArr;
        }

        protected bool[] ByteArrToBoolArr(byte[] btArr)
        {
            int intSize = btArr.Length / sizeof(bool);
            bool[] intArr = new bool[intSize];
            Buffer.BlockCopy(btArr, 0, intArr, 0, intSize);
            return intArr;
        }
        #endregion

        #region 设置跳词
        /// <summary>
        /// 设置跳词，搜索匹配前，请设置UseSkipWordFilter=true
        /// </summary>
        /// <param name="skipList"></param>
        public void SetSkipWords(string skipList)
        {
            _skipBitArray = new bool[char.MaxValue + 1];
            if (skipList == null) {
                for (int i = 0; i < _skipList.Length; i++) {
                    _skipBitArray[_skipList[i]] = true;
                }
            }
        }
        #endregion

        #region 设置黑名单
        /// <summary>
        /// 设置黑名单，搜索匹配前，请设置UseBlacklistFilter=true
        /// </summary>
        /// <param name="blacklist"></param>
        public void SetBlacklist(BlacklistType[] blacklist)
        {
            if (_keywords == null) {
                throw new NullReferenceException("请先使用SetKeywords方法设置关键字！");
            }
            if (blacklist.Length != _keywords.Length) {
                throw new ArgumentException("请关键字与黑名单列表的长度要一样长！");
            }

            var list = new int[blacklist.Length];
            for (int i = 0; i < blacklist.Length; i++) {
                list[i] = (int)blacklist[i];
            }
            _blacklist = list;
        }

        /// <summary>
        /// 设置黑名单，搜索匹配前，请设置UseBlacklistFilter=true
        /// </summary>
        /// <param name="blacklist"></param>
        public void SetBlacklist(int[] blacklist)
        {
            if (_keywords == null) {
                throw new NullReferenceException("请先使用SetKeywords方法设置关键字！");
            }
            if (blacklist.Length != _keywords.Length) {
                throw new ArgumentException("请关键字与黑名单列表的长度要一样长！");
            }

            _blacklist = blacklist;
        }
        #endregion

        #region 设置关键字
        private string ToSenseWord(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++) {
                stringBuilder.Append(ToSenseWord(text[i]));
                //stringBuilder[i] = ToSenseWord(text[i]);
            }
            return stringBuilder.ToString();
        }

        private char ToSenseWord(char c)
        {
            if (c >= 'A' && c <= 'Z') return (char)(c | 0x20);
            if (c == 12288) return ' ';
            if (c >= 65280 && c < 65375) {
                var k = (c - 65248);
                if ('A' <= k && k <= 'Z') {
                    k = k | 0x20;
                }
                return (char)k;
            }
            return c;
        }
        /// <summary>
        /// 设置关键字，设置前请核对UseDBCcaseConverter、UseSimplifiedChineseConverter的值，两值可对关键字有影响
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        public void SetKeywords(ICollection<string> keywords)
        {
            HashSet<string> kws = new HashSet<string>();
            foreach (var item in keywords) {
                kws.Add(ToSenseWord(item));
            }
            setKeywords(kws);
        }

        /// <summary>
        /// 设置关键字
        /// </summary>
        /// <param name="keywords">关键字列表</param>
        private void setKeywords(ICollection<string> keywords)
        {
            _keywords = keywords.ToArray();
            var length = CreateDict(keywords);
            var root = new TrieNode();

            for (int i = 0; i < _keywords.Length; i++) {
                var p = _keywords[i];
                var nd = root;
                for (int j = 0; j < p.Length; j++) {
                    nd = nd.Add((char)_dict[p[j]]);
                }
                nd.SetResults(i);
            }

            List<TrieNode> nodes = new List<TrieNode>();
            // Find failure functions
            //ArrayList nodes = new ArrayList();
            // level 1 nodes - fail to root node
            foreach (TrieNode nd in root.m_values.Values) {
                nd.Failure = root;
                foreach (TrieNode trans in nd.m_values.Values) nodes.Add(trans);
            }
            // other nodes - using BFS
            while (nodes.Count != 0) {
                List<TrieNode> newNodes = new List<TrieNode>();

                //ArrayList newNodes = new ArrayList();
                foreach (TrieNode nd in nodes) {
                    TrieNode r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.m_values.ContainsKey(c)) r = r.Failure;
                    if (r == null)
                        nd.Failure = root;
                    else {
                        nd.Failure = r.m_values[c];
                        foreach (var result in nd.Failure.Results)
                            nd.SetResults(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TrieNode child in nd.m_values.Values)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
            root.Failure = root;
            foreach (var item in root.m_values.Values) {
                TryLinks(item);
            }
            build(root, length);
        }
        private void TryLinks(TrieNode node)
        {
            node.Merge(node.Failure);
            foreach (var item in node.m_values.Values) {
                TryLinks(item);
            }
        }

        private void build(TrieNode root, int length)
        {
            TrieNode[] has = new TrieNode[0x00FFFFFF];
            length = root.Rank(has) + length + 1;
            _key = new int[length];
            _next = new int[length];
            _check = new int[length];
            List<int[]> guides = new List<int[]>();
            guides.Add(new int[] { 0 });
            for (int i = 0; i < length; i++) {
                var item = has[i];
                if (item == null) continue;
                _key[i] = item.Char;
                _next[i] = item.Next;
                if (item.End) {
                    _check[i] = guides.Count;
                    guides.Add(item.Results.ToArray());
                }
            }
            _guides = guides.ToArray();
        }

        #endregion

        #region 生成映射字典

        private int CreateDict(ICollection<string> keywords)
        {
            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            foreach (var keyword in keywords) {
                for (int i = 0; i < keyword.Length; i++) {
                    var item = keyword[i];
                    if (dictionary.ContainsKey(item)) {
                        if (i > 0)
                            dictionary[item] += 2;
                    } else {
                        dictionary[item] = i > 0 ? 2 : 1;
                    }
                }
            }
            var list = dictionary.OrderByDescending(q => q.Value).Select(q => q.Key).ToList();
            var list2 = new List<char>();
            var sh = false;
            foreach (var item in list) {
                if (sh) {
                    list2.Add(item);
                } else {
                    list2.Insert(0, item);
                }
                sh = !sh;
            }
            _dict = new int[char.MaxValue + 1];
            for (int i = 0; i < list2.Count; i++) {
                _dict[list2[i]] = i + 1;
            }
            return dictionary.Count;
        }
        #endregion
    }
}
