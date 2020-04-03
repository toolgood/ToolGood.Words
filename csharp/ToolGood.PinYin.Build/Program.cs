using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToolGood.Bedrock;

namespace ToolGood.PinYin.Build
{
    class Program
    {
        static void Main(string[] args)
        {



            for (int i = 0; i < pyName.Count; i++) {
                pyName[i] = pyName[i].ToLower();
            }

            var pyText = File.ReadAllText("dict\\_py.txt");
            var pyLines = pyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            foreach (var line in pyLines) {
                var sp = line.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1) {
                    var key = sp[0];
                    List<int> indexs = new List<int>();
                    for (int i = 1; i < sp.Length; i++) {
                        var py = sp[i];
                        var idx = pyName.IndexOf(py) * 2 + 1;
                        if (idx==-1) {

                        }
                        indexs.Add(idx);
                    }
                    dict[key] = indexs;
                }
            }

            List<string> pyData = new List<string>();
            for (int i = 0x3400; i <= 0x9FFF; i++) {
                var c = ((char)i).ToString();
                if (dict.TryGetValue(c, out List<int> indexs)) {
                    List<string> idxs = new List<string>();
                    foreach (var index in indexs) {
                        idxs.Add(index.ToString("X"));
                    }
                    if (idxs[0]== "FFFFFFFF") {

                    }

                    pyData.Add(string.Join(",", idxs));
                } else {
                    pyData.Add("0");
                }
            }
            var outText = string.Join("\n", pyData);
            File.WriteAllText("pyIndex.txt", outText);




            //Dictionary<string, HashSet<string>> dict = new Dictionary<string, HashSet<string>>();
            //var files = Directory.GetFiles("dict", "*.txt");
            //foreach (var file in files) {
            //    var text = File.ReadAllText(file);
            //    var line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (var l in line) {
            //        if (string.IsNullOrWhiteSpace(l)) {
            //            continue;
            //        }
            //        var sp = l.Split("\t,:| '\"=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            //        for (int i = 1; i < sp.Length; i++) {
            //            pyName.Add(sp[i]);
            //        }
            //        HashSet<string> py;
            //        if (dict.TryGetValue(sp[0], out py) == false) {
            //            py = new HashSet<string>();
            //            dict[sp[0]] = py;
            //        }
            //        for (int i = 1; i < sp.Length; i++) {
            //            py.Add(sp[i]);
            //        }
            //    }
            //}
            //Dictionary<string, int> valuePairs = new Dictionary<string, int>();
            //foreach (var item in pyName) {
            //    int count = 0;
            //    if (valuePairs.TryGetValue(item, out count)) {

            //    }
            //    valuePairs[item] = count + 1;
            //}
            //var t = valuePairs.Where(q => q.Value < 2).Select(q => q.Key).OrderBy(q => q).ToList();


            //pyName = pyName.Distinct().OrderBy(q => q).ToList();



            //var ts = pyName.Where(q => q.Length > 5).ToList();
            //var ts1 = pyName.Where(q => q.Length == 5).ToList();
            //var ts2 = pyName.Where(q => q.Length == 4).ToList();
            //var ts3 = pyName.Where(q => q.Length == 3).ToList();
            //var ts4 = pyName.Where(q => q.Length == 2).ToList();
            //var ts5 = pyName.Where(q => q.Length == 1).ToList();
            //var str = "\"" + string.Join("\",\"", pyName) + "\"";
            //File.WriteAllText("pyName.txt", str);

            //List<string> ls = new List<string>();
            //foreach (var item in dict) {
            //    List<int> key = new List<int>();
            //    foreach (var v in item.Value) {
            //        key.Add(pyName.IndexOf(v));
            //    }
            //    ls.Add($"{item.Key} {string.Join(",", key)}");
            //}
            //var outText = string.Join("\n", ls);
            //File.WriteAllText("pyIndex.txt", outText);

            Compression("pyIndex.txt");
        }


        public static List<string> pyName = new List<string>
{
"A","Á","À","Ǎ","Ā","Ái","Ài","Ǎi","Āi","Án","Àn","Ǎn","Ān","Áng","Àng","Ǎng","Āng","Áo","Ào","Ǎo","Āo","Ba","Bá","Bà","Bǎ","Bā","Bai","Bái","Bài","Bǎi","Bāi","Bǎikè","Bàn","Bǎn","Bān","Báng","Bàng","Bǎng","Bāng","Báo","Bào","Bǎo","Bāo","Bei","Bèi","Běi","Bēi","Bèn","Běn","Bēn","Béng","Bèng","Běng","Bēng","Bí","Bì","Bǐ","Bī","Biàn","Biǎn","Biān","Biáo","Biào","Biǎo","Biāo","Bié","Biè","Biě","Biē","Bìn","Bīn","Bìng","Bǐng","Bīng","Bo","Bó","Bò","Bǒ","Bō","Bú","Bù","Bǔ","Bū","Cà","Cǎ","Cā","Cái","Cài","Cǎi","Cāi","Cán","Càn","Cǎn","Cān","Cáng","Càng","Cāng","Cáo","Cào","Cǎo","Cāo","Cè","Cén","Cēn","Céng","Cèng","Cēng","Chá","Chà","Chǎ","Chā","Chái","Chài","Chǎi","Chāi","Chán","Chàn","Chǎn","Chān","Cháng","Chàng","Chǎng","Chāng","Cháo","Chào","Chǎo","Chāo","Chè","Chě","Chē","Chen","Chén","Chèn","Chěn","Chēn","Chéng","Chèng","Chěng","Chēng","Chi","Chí","Chì","Chǐ","Chī","Chóng","Chòng","Chǒng","Chōng","Chou","Chóu","Chòu","Chǒu","Chōu","Chu","Chú","Chù","Chǔ","Chū","Chuà","Chuā","Chuái","Chuài","Chuǎi","Chuāi","Chuán","Chuàn","Chuǎn","Chuān","Chuáng","Chuàng","Chuǎng","Chuāng","Chuí","Chuì","Chuǐ","Chuī","Chún","Chǔn","Chūn","Chuò","Chuō","Cí","Cì","Cǐ","Cī","Cóng","Còng","Cōng","Còu","Cú","Cù","Cǔ","Cū","Cuán","Cuàn","Cuān","Cuì","Cuǐ","Cuī","Cún","Cùn","Cǔn","Cūn","Cuó","Cuò","Cuǒ","Cuō","Da","Dá","Dà","Dǎ","Dā","Dài","Dǎi","Dāi","Dàn","Dǎn","Dān","Dàng","Dǎng","Dāng","Dáo","Dào","Dǎo","Dāo","De","Dé","Dē","Děi","Dēi","Dèn","Dèng","Děng","Dēng","Dí","Dì","Dǐ","Dī","Diǎ","Diàn","Diǎn","Diān","Diào","Diǎo","Diāo","Dié","Diè","Diē","Dìng","Dǐng","Dīng","Diū","Dòng","Dǒng","Dōng","Dòu","Dǒu","Dōu","Dú","Dù","Dǔ","Dū","Duàn","Duǎn","Duān","Duì","Duǐ","Duī","Dùn","Dǔn","Dūn","Duó","Duò","Duǒ","Duō","É","È","Ě","Ē","Éi","Èi","Ěi","Ēi","En","Én","Èn","Ěn","Ēn","Ēng","Ér","Èr","Ěr","Fá","Fà","Fǎ","Fā","Fán","Fàn","Fǎn","Fān","Fáng","Fàng","Fǎng","Fāng","Féi","Fèi","Fěi","Fēi","Fén","Fèn","Fěn","Fēn","Féng","Fèng","Fěng","Fēng","Fēnwǎ","Fiào","Fó","Fóu","Fǒu","Fú","Fù","Fǔ","Fū","Gá","Gà","Gǎ","Gā","Gài","Gǎi","Gāi","Gàn","Gǎn","Gān","Gàng","Gǎng","Gāng","Gào","Gǎo","Gāo","Gé","Gè","Gě","Gē","Gěi","Gén","Gèn","Gěn","Gēn","Gèng","Gěng","Gēng","Gòng","Gǒng","Gōng","Gōngfēn","Gōnglǐ","Gòu","Gǒu","Gōu","Gú","Gù","Gǔ","Gū","Guà","Guǎ","Guā","Guái","Guài","Guǎi","Guāi","Guàn","Guǎn","Guān","Guàng","Guǎng","Guāng","Guì","Guǐ","Guī","Gùn","Gǔn","Guo","Guó","Guò","Guǒ","Guō","Há","Hà","Hǎ","Hā","Hái","Hài","Hǎi","Hāi","Han","Hán","Hàn","Hǎn","Hān","Háng","Hàng","Hāng","Háo","Hào","Hǎo","Hāo","Hé","Hè","Hē","Hēi","Hén","Hèn","Hěn","Héng","Hèng","Hēng","Hóng","Hòng","Hǒng","Hōng","Hóu","Hòu","Hǒu","Hōu","Hú","Hù","Hǔ","Hū","Huá","Huà","Huā","Huái","Huài","Huán","Huàn","Huǎn","Huān","Huáng","Huàng","Huǎng","Huāng","Huí","Huì","Huǐ","Huī","Hún","Hùn","Hūn","Huó","Huò","Huǒ","Huō","Jí","Jì","Jǐ","Jī","Jia","Jiá","Jià","Jiǎ","Jiā","Jiālún","Jiàn","Jiǎn","Jiān","Jiàng","Jiǎng","Jiāng","Jiáo","Jiào","Jiǎo","Jiāo","Jie","Jié","Jiè","Jiě","Jiē","Jìn","Jǐn","Jīn","Jìng","Jǐng","Jīng","Jiǒng","Jiōng","Jiú","Jiù","Jiǔ","Jiū","Jú","Jù","Jǔ","Jū","Juàn","Juǎn","Juān","Jué","Juè","Juě","Juē","Jùn","Jūn","Kǎ","Kā","Kài","Kǎi","Kāi","Kàn","Kǎn","Kān","Káng","Kàng","Kǎng","Kāng","Kào","Kǎo","Kāo","Kasei","Ké","Kè","Kě","Kē","Kēi","Kèn","Kěn","Kěng","Kēng","Kòng","Kǒng","Kōng","Kòu","Kǒu","Kōu","Kù","Kǔ","Kū","Kuà","Kuǎ","Kuā","Kuài","Kuǎi","Kuǎn","Kuān","Kuáng","Kuàng","Kuǎng","Kuāng","Kuí","Kuì","Kuǐ","Kuī","Kùn","Kǔn","Kūn","Kuò","La","Lá","Là","Lǎ","Lā","Lái","Lài","Lǎi","Lán","Làn","Lǎn","Láng","Làng","Lǎng","Lāng","Láo","Lào","Lǎo","Lāo","Le","Lè","Lē","Lei","Léi","Lèi","Lěi","Lēi","Léng","Lèng","Lěng","Lēng","Li","Lí","Lì","Lǐ","Lī","Liǎ","Lián","Liàn","Liǎn","Liáng","Liàng","Liǎng","Liáo","Liào","Liǎo","Liāo","Lie","Lié","Liè","Liě","Liē","Lín","Lìn","Lǐn","Līn","Líng","Lìng","Lǐng","Liú","Liù","Liǔ","Liū","Líwǎ","Lǐwǎ","Lóng","Lòng","Lǒng","Lou","Lóu","Lòu","Lǒu","Lōu","Lú","Lù","Lǔ","Lū","Lǘ","Lǜ","Lǚ","Luán","Luàn","Luǎn","Lüè","Lún","Lùn","Lǔn","Lūn","Luo","Luó","Luò","Luǒ","Luō","Ma","Má","Mà","Mǎ","Mā","Mái","Mài","Mǎi","Mán","Màn","Mǎn","Mān","Máng","Mǎng","Māng","Máo","Mào","Mǎo","Māo","Máowǎ","Me","Mè","Mē","Méi","Mèi","Měi","Mén","Mèn","Mēn","Méng","Mèng","Měng","Mēng","Mí","Mì","Mǐ","Mī","Mián","Miàn","Miǎn","Miáo","Miào","Miǎo","Miāo","Miè","Miē","Mín","Mǐn","Míng","Mìng","Mǐng","Miù","Mó","Mò","Mǒ","Mō","Móu","Mǒu","Mōu","Mú","Mù","Mǔ","Na","Ná","Nà","Nǎ","Nā","Nái","Nài","Nǎi","Nán","Nàn","Nǎn","Nān","Náng","Nàng","Nǎng","Nāng","Náo","Nào","Nǎo","Nāo","Ne","Né","Nè","Nèi","Něi","Nèn","Néng","Nèng","Něng","Ní","Nì","Nǐ","Nī","Nián","Niàn","Niǎn","Niān","Niáng","Niàng","Niào","Niǎo","Nié","Niè","Niē","Nín","Nǐn","Níng","Nìng","Nǐng","Niú","Niù","Niǔ","Niū","Nóng","Nòng","Nǒng","Nóu","Nòu","Nǒu","Nú","Nù","Nǔ","Nǜ","Nǚ","Nuán","Nuǎn","Nüè","Nún","Nuó","Nuò","Nuǒ","O","Ó","Ò","Ǒ","Ō","Òu","Ǒu","Ōu","Pá","Pà","Pā","Pái","Pài","Pǎi","Pāi","Pán","Pàn","Pǎn","Pān","Páng","Pàng","Pǎng","Pāng","Páo","Pào","Pǎo","Pāo","Péi","Pèi","Pěi","Pēi","Pén","Pèn","Pěn","Pēn","Péng","Pèng","Pěng","Pēng","Pi","Pí","Pì","Pǐ","Pī","Pián","Piàn","Piǎn","Piān","Piáo","Piào","Piǎo","Piāo","Piè","Piě","Piē","Pín","Pìn","Pǐn","Pīn","Píng","Pìng","Pǐng","Pīng","Pó","Pò","Pǒ","Pō","Póu","Pǒu","Pōu","Pú","Pù","Pǔ","Pū","Qi","Qí","Qì","Qǐ","Qī","Qià","Qiǎ","Qiā","Qián","Qiàn","Qiǎn","Qiān","Qiáng","Qiàng","Qiǎng","Qiāng","Qiānkè","Qiānwǎ","Qiáo","Qiào","Qiǎo","Qiāo","Qié","Qiè","Qiě","Qiē","Qín","Qìn","Qǐn","Qīn","Qíng","Qìng","Qǐng","Qīng","Qióng","Qiú","Qiù","Qiǔ","Qiū","Qú","Qù","Qǔ","Qū","Quán","Quàn","Quǎn","Quān","Qué","Què","Quē","Qún","Qūn","Rán","Rǎn","Ráng","Ràng","Rǎng","Rāng","Ráo","Rào","Rǎo","Ré","Rè","Rě","Rén","Rèn","Rěn","Réng","Rèng","Rēng","Rì","Róng","Ròng","Rǒng","Róu","Ròu","Rǒu","Rú","Rù","Rǔ","Rū","Ruán","Ruàn","Ruǎn","Ruí","Ruì","Ruǐ","Rún","Rùn","Ruó","Ruò","Sà","Sǎ","Sā","Sài","Sǎi","Sāi","San","Sàn","Sǎn","Sān","Sàng","Sǎng","Sāng","Sào","Sǎo","Sāo","Sè","Sē","Sēn","Sēng","Shá","Shà","Shǎ","Shā","Shài","Shǎi","Shāi","Shàn","Shǎn","Shān","Shang","Shàng","Shǎng","Shāng","Sháo","Shào","Shǎo","Shāo","Shé","Shè","Shě","Shē","Shéi","Shén","Shèn","Shěn","Shēn","Shéng","Shèng","Shěng","Shēng","Shi","Shí","Shì","Shǐ","Shī","Shíkě","Shíwǎ","Shòu","Shǒu","Shōu","Shú","Shù","Shǔ","Shū","Shuà","Shuǎ","Shuā","Shuài","Shuǎi","Shuāi","Shuàn","Shuān","Shuàng","Shuǎng","Shuāng","Shuí","Shuì","Shuǐ","Shùn","Shǔn","Shuò","Shuō","Sì","Sǐ","Sī","Sóng","Sòng","Sǒng","Sōng","Sòu","Sǒu","Sōu","Sú","Sù","Sū","Suàn","Suǎn","Suān","Suí","Suì","Suǐ","Suī","Sùn","Sǔn","Sūn","Suò","Suǒ","Suō","Ta","Tà","Tǎ","Tā","Tái","Tài","Tǎi","Tāi","Tán","Tàn","Tǎn","Tān","Táng","Tàng","Tǎng","Tāng","Táo","Tào","Tǎo","Tāo","Tè","Téng","Tèng","Tēng","Tí","Tì","Tǐ","Tī","Tián","Tiàn","Tiǎn","Tiān","Tiáo","Tiào","Tiǎo","Tiāo","Tié","Tiè","Tiě","Tiē","Tíng","Tìng","Tǐng","Tīng","Tóng","Tòng","Tǒng","Tōng","Tou","Tóu","Tòu","Tǒu","Tōu","Tu","Tú","Tù","Tǔ","Tū","Tuán","Tuàn","Tuǎn","Tuān","Tuí","Tuì","Tuǐ","Tuī","Tún","Tùn","Tǔn","Tūn","Tuó","Tuò","Tuǒ","Tuō","Wa","Wá","Wà","Wǎ","Wā","Wài","Wǎi","Wāi","Wán","Wàn","Wǎn","Wān","Wáng","Wàng","Wǎng","Wāng","Wéi","Wèi","Wěi","Wēi","Wén","Wèn","Wěn","Wēn","Wèng","Wěng","Wēng","Wò","Wǒ","Wō","Wú","Wù","Wǔ","Wū","Xí","Xì","Xǐ","Xī","Xiá","Xià","Xiǎ","Xiā","Xián","Xiàn","Xiǎn","Xiān","Xiáng","Xiàng","Xiǎng","Xiāng","Xiáo","Xiào","Xiǎo","Xiāo","Xié","Xiè","Xiě","Xiē","Xín","Xìn","Xǐn","Xīn","Xíng","Xìng","Xǐng","Xīng","Xióng","Xiòng","Xiǒng","Xiōng","Xiú","Xiù","Xiǔ","Xiū","Xú","Xù","Xǔ","Xū","Xuán","Xuàn","Xuǎn","Xuān","Xué","Xuè","Xuě","Xuē","Xún","Xùn","Xūn","Ya","Yá","Yà","Yǎ","Yā","Yán","Yàn","Yǎn","Yān","Yáng","Yàng","Yǎng","Yāng","Yáo","Yào","Yǎo","Yāo","Ye","Yé","Yè","Yě","Yē","Yi","Yí","Yì","Yǐ","Yī","Yín","Yìn","Yǐn","Yīn","Yíng","Yìng","Yǐng","Yīng","Yo","Yō","Yóng","Yòng","Yǒng","Yōng","Yóu","Yòu","Yǒu","Yōu","Yú","Yù","Yǔ","Yū","Yuán","Yuàn","Yuǎn","Yuān","Yuè","Yuě","Yuē","Yún","Yùn","Yǔn","Yūn","Zá","Zǎ","Zā","Zài","Zǎi","Zāi","Zàn","Zǎn","Zān","Zàng","Zǎng","Zāng","Záo","Zào","Zǎo","Zāo","Zé","Zè","Zéi","Zèn","Zěn","Zēn","Zèng","Zěng","Zēng","Zhá","Zhà","Zhǎ","Zhā","Zhái","Zhài","Zhǎi","Zhāi","Zhán","Zhàn","Zhǎn","Zhān","Zhàng","Zhǎng","Zhāng","Zháo","Zhào","Zhǎo","Zhāo","Zhe","Zhé","Zhè","Zhě","Zhē","Zhèi","Zhèn","Zhěn","Zhēn","Zhèng","Zhěng","Zhēng","Zhí","Zhì","Zhǐ","Zhī","Zhòng","Zhǒng","Zhōng","Zhóu","Zhòu","Zhǒu","Zhōu","Zhú","Zhù","Zhǔ","Zhū","Zhuǎ","Zhuā","Zhuài","Zhuǎi","Zhuāi","Zhuàn","Zhuǎn","Zhuān","Zhuàng","Zhuǎng","Zhuāng","Zhuì","Zhuǐ","Zhuī","Zhùn","Zhǔn","Zhūn","Zhuó","Zhuò","Zhuō","Zi","Zí","Zì","Zǐ","Zī","Zòng","Zǒng","Zōng","Zòu","Zǒu","Zōu","Zú","Zǔ","Zū","Zuàn","Zuǎn","Zuān","Zuì","Zuǐ","Zuī","Zùn","Zǔn","Zūn","Zuo","Zuó","Zuò","Zuǒ","Zuō"
     };

        private static void Compression(string file)
        {
            var bytes = File.ReadAllBytes(file);
            var bs = CompressionUtil.GzipCompress(bytes);
            Directory.CreateDirectory("dict");
            File.WriteAllBytes("dict\\" + file + ".z", bs);

            var bs2 = CompressionUtil.BrCompress(bytes);
            File.WriteAllBytes("dict\\" + file + ".br", bs2);
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

        static Dictionary<char, char> ToneDict;
        static string RemoveTone(string text)
        {
            if (ToneDict == null) {
                var tones = @"aāáǎàa|oōóǒòo|eēéěèe|iīíǐìi|uūúǔùu|vǖǘǚǜü".Split('|');
                var dict = new Dictionary<char, char>();
                foreach (var tone in tones) {
                    for (int i = 1; i < tone.Length; i++) {
                        dict[tone[i]] = tone[0];
                    }
                }
                ToneDict = dict;
            }
            var dic = ToneDict;
            var str = "";
            foreach (var t in text) {
                if (dic.TryGetValue(t, out char c)) {
                    str += c;
                } else {
                    str += t;
                }
            }
            return str;
        }

    }
}
