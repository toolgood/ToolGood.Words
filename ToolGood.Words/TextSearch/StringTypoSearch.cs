//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using ToolGood.Words.internals;

//namespace ToolGood.Words
//{
//    /// <summary>
//    /// 错字搜索--英文数字搜索,不包含中文转数字，支持链接
//    /// </summary>
//    public class StringTypoSearch : IllegalWordsSearchBase
//    {
//        #region 私有变量
//        private int _maxJump = 10;//设置一个最大大跳跃点
//        private char[] toWord;
//        private Dictionary<char, string> toWordDict;
//        #endregion

//        #region 构造函数
//        private const string baseTypo =
//            "0 0\r1 1\r2 2\r3 3\r4 4\r5 5\r6 6\r7 7\r8 8\r9 9\r" +
//            "０ 0\r１ 1\r２ 2\r３ 3\r４ 4\r５ 5\r６ 6\r７ 7\r８ 8\r９ 9\r" +
//            "A a\rB b\rC c\rD d\rE e\rF f\rG g\rH h\rI i\rJ j\rK k\rL l\rM m\rN n\rO o\rP p\rQ q\rR r\rS s\rT t\rU u\rV v\rW w\rX x\rY y\rZ z\r" +
//            "a a\rb b\rc c\rd d\re e\rf f\rg g\rh h\ri i\rj j\rk k\rl l\rm m\rn n\ro o\rp p\rq q\rr r\rs s\rt t\ru u\rv v\rw w\rx x\ry y\rz z\r" +
//            "Ａ a\rＢ b\rＣ c\rＤ d\rＥ e\rＦ f\rＧ g\rＨ h\rＩ i\rＪ j\rＫ k\rＬ l\rＭ m\rＮ n\rＯ o\rＰ p\rＱ q\rＲ r\rＳ s\rＴ t\rＵ u\rＶ v\rＷ w\rＸ x\rＹ y\rＺ z\r" +
//            "ａ a\rｂ b\rｃ c\rｄ d\rｅ e\rｆ f\rｇ g\rｈ h\rｉ i\rｊ j\rｋ k\rｌ l\rｍ m\rｎ n\rｏ o\rｐ p\rｑ q\rｒ r\rｓ s\rｔ t\rｕ u\rｖ v\rｗ w\rｘ x\rｙ y\rｚ z\r" +
//            "# #\r$ $\r% %\r& &\r+ +\r- -\r. .\r/ /\r: :\r= =\r? ?\r@ @\r_ _\r\\ \\\r" +
//            "＃ #\r＄ $\r％ %\r＆ &\r＋ +\r－ -\r． .\r／ /\r： :\r＝ =\r？ ?\r＠ @\r＿ _\r＼ \\\r";
//        private const string defaultTypo = "⓪ 0\r零 0\rº 0\r₀ 0\r⓿ 0\r○ 0\r〇 0\r" +
//                                          "⒜ a\r⒝ b\r⒞ c\r⒟ d\r⒠ e\r⒡ f\r⒢ g\r⒣ h\r⒤ i\r⒥ j\r⒦ k\r⒧ l\r⒨ m\r⒩ n\r⒪ o\r⒫ p\r⒬ q\r⒭ r\r⒮ s\r⒯ t\r⒰ u\r⒱ v\r⒲ w\r⒳ x\r⒴ y\r⒵ z\r" +
//                                          "Ⓐ a\rⒷ b\rⒸ c\rⒹ d\rⒺ e\rⒻ f\rⒼ g\rⒽ h\rⒾ i\rⒿ j\rⓀ k\rⓁ l\rⓂ m\rⓃ n\rⓄ o\rⓅ p\rⓆ q\rⓇ r\rⓈ s\rⓉ t\rⓊ u\rⓋ v\rⓌ w\rⓍ x\rⓎ y\rⓏ z\r" +
//                                          "ⓐ a\rⓑ b\rⓒ c\rⓓ d\rⓔ e\rⓕ f\rⓖ g\rⓗ h\rⓘ i\rⓙ j\rⓚ k\rⓛ l\rⓜ m\rⓝ n\rⓞ o\rⓟ p\rⓠ q\rⓡ r\rⓢ s\rⓣ t\rⓤ u\rⓥ v\rⓦ w\rⓧ x\rⓨ y\rⓩ z\r" +
//                                         "一 1\r二 2\r三 3\r四 4\r五 5\r六 6\r七 7\r八 8\r九 9\r" +
//                                         "壹 1\r贰 2\r叁 3\r肆 4\r伍 5\r陆 6\r柒 7\r捌 8\r玖 9\r" +
//                                          "。 .\r点 .\r點 .\r— 1\r貮 2\r参 3\r陸 6\r➉ 10\r➓ 10\r" +
//                                         "¹ 1\r² 2\r³ 3\r⁴ 4\r⁵ 5\r⁶ 6\r⁷ 7\r⁸ 8\r⁹ 9\r" +
//                                         "₁ 1\r₂ 2\r₃ 3\r₄ 4\r₅ 5\r₆ 6\r₇ 7\r₈ 8\r₉ 9\r" +
//                                         "① 1\r② 2\r③ 3\r④ 4\r⑤ 5\r⑥ 6\r⑦ 7\r⑧ 8\r⑨ 9\r" +
//                                         "⑴ 1\r⑵ 2\r⑶ 3\r⑷ 4\r⑸ 5\r⑹ 6\r⑺ 7\r⑻ 8\r⑼ 9\r" +
//                                         "⒈ 1\r⒉ 2\r⒊ 3\r⒋ 4\r⒌ 5\r⒍ 6\r⒎ 7\r⒏ 8\r⒐ 9\r" +
//                                         "❶ 1\r❷ 2\r❸ 3\r❹ 4\r❺ 5\r❻ 6\r❼ 7\r❽ 8\r❾ 9\r" +
//                                         "➀ 1\r➁ 2\r➂ 3\r➃ 4\r➄ 5\r➅ 6\r➆ 7\r➇ 8\r➈ 9\r" +
//                                         "➊ 1\r➋ 2\r➌ 3\r➍ 4\r➎ 5\r➏ 6\r➐ 7\r➑ 8\r➒ 9\r" +
//                                         "㈠ 1\r㈡ 2\r㈢ 3\r㈣ 4\r㈤ 5\r㈥ 6\r㈦ 7\r㈧ 8\r㈨ 9\r㈩ 10\r" +
//                                         "⓵ 1\r⓶ 2\r⓷ 3\r⓸ 4\r⓹ 5\r⓺ 6\r⓻ 7\r⓼ 8\r⓽ 9\r" +
//                                         "㊀ 1\r㊁ 2\r㊂ 3\r㊃ 4\r㊄ 5\r㊅ 6\r㊆ 7\r㊇ 8\r㊈ 9\r" +
//                                         //"㊉ 10\r㈩ 10\r" +
//                                         "❿ 10\r⓫ 11\r⓬ 12\r⓭ 13\r⓮ 14\r⓯ 15\r⓰ 16\r⓱ 17\r⓲ 18\r⓳ 19\r⓴ 20\r" +
//                                         "⑽ 10\r⑾ 11\r⑿ 12\r⒀ 13\r⒁ 14\r⒂ 15\r⒃ 16\r⒄ 17\r⒅ 18\r⒆ 19\r⒇ 20\r" +
//                                         "⒑ 10\r⒒ 11\r⒓ 12\r⒔ 13\r⒕ 14\r⒖ 15\r⒗ 16\r⒘ 17\r⒙ 18\r⒚ 19\r⒛ 20\r" +
//                                         "⑩ 10\r⑪ 11\r⑫ 12\r⑬ 13\r⑭ 14\r⑮ 15\r⑯ 16\r⑰ 17\r⑱ 18\r⑲ 19\r⑳ 20\r" +
//                                         "㉑ 21\r㉒ 22\r㉓ 23\r㉔ 24\r㉕ 25\r㉖ 26\r㉗ 27\r㉘ 28\r㉙ 29\r㉚ 30\r" +
//                                         "㉛ 31\r㉜ 32\r㉝ 33\r㉞ 34\r㉟ 35\r㊱ 36\r㊲ 37\r㊳ 38\r㊴ 39\r㊵ 40\r" +
//                                         "㊶ 41\r㊷ 42\r㊸ 43\r㊹ 44\r㊺ 45\r㊻ 46\r㊼ 47\r㊽ 48\r㊾ 49\r㊿ 50\r";




//        public StringTypoSearch()
//        {
//            toWord = new char[char.MaxValue + 1];
//            toWordDict = new Dictionary<char, string>();

//            buildWordConverter(defaultTypo);
//            buildWordConverter(baseTypo);
//        }

//        private void buildWordConverter(string typoText)
//        {
//            var ts = typoText.Replace("\n", "").Split('\r');
//            var splits = " \t".ToCharArray();
//            foreach (var t in ts) {
//                var sp = t.Split(splits, StringSplitOptions.RemoveEmptyEntries);
//                if (sp.Length < 2) continue;
//                if (sp[1].Length == 1) {
//                    toWord[(int)sp[0][0]] = sp[1][0];
//                } else {
//                    toWord[(int)sp[0][0]] = (char)1;
//                    toWordDict[sp[0][0]] = sp[1];
//                }
//            }
//        }


//        #endregion




//        #region 保存到文件

//        protected internal virtual void Save(BinaryWriter bw)
//        {
//            base.Save(bw);

//            bw.Write(_maxJump);
//            var bs = CharArrToByteArr(toWord);
//            bw.Write(bs.Length);
//            bw.Write(bs);

//            bw.Write(toWordDict.Count);

//            foreach (var item in toWordDict) {
//                bw.Write(item.Key);
//                bw.Write(item.Value);
//            }
//        }

//        private byte[] CharArrToByteArr(char[] intArr)
//        {
//            int intSize = sizeof(char) * intArr.Length;
//            byte[] bytArr = new byte[intSize];
//            Buffer.BlockCopy(intArr, 0, bytArr, 0, intSize);
//            return bytArr;
//        }

//        #endregion

//        #region 加载文件

//        protected internal override void Load(BinaryReader br)
//        {
//            base.Load(br);

//            _maxJump = br.ReadInt32();
//            var length = br.ReadInt32();
//            toWord = ByteArrToCharArr(br.ReadBytes(length));

//            toWordDict = new Dictionary<char, string>();
//            length = br.ReadInt32();
//            for (int i = 0; i < length; i++) {
//                var c = br.ReadChar();
//                var t = br.ReadString();
//                toWordDict[c] = t;
//            }
//        }

//        private char[] ByteArrToCharArr(byte[] btArr)
//        {
//            int intSize = btArr.Length / sizeof(char);
//            char[] intArr = new char[intSize];
//            Buffer.BlockCopy(btArr, 0, intArr, 0, intSize);
//            return intArr;
//        }

//        #endregion



//        #region 设置关键字
//        private string ToSenseWord(string text)
//        {
//            StringBuilder stringBuilder = new StringBuilder(text.Length);
//            for (int i = 0; i < text.Length; i++) {
//                stringBuilder.Append(ToSenseWord(text[i]));
//                //stringBuilder[i] = ToSenseWord(text[i]);
//            }
//            return stringBuilder.ToString();
//        }

//        private char ToSenseWord(char c)
//        {
//            if (c >= 'A' && c <= 'Z') return (char)(c | 0x20);
//            if (c == 12288) return ' ';
//            if (c >= 65280 && c < 65375) {
//                var k = (c - 65248);
//                if ('A' <= k && k <= 'Z') {
//                    k = k | 0x20;
//                }
//                return (char)k;
//            }
//            return c;
//        }
//        /// <summary>
//        /// 设置关键字，设置前请核对UseDBCcaseConverter、UseSimplifiedChineseConverter的值，两值可对关键字有影响
//        /// </summary>
//        /// <param name="keywords">关键字列表</param>
//        public override void SetKeywords(ICollection<string> keywords)
//        {
//            HashSet<string> kws = new HashSet<string>();
//            foreach (var item in keywords) {
//                kws.Add(ToSenseWord(item));
//            }
//            base.SetKeywords(kws);
//        }
//        #endregion


//    }
//}
