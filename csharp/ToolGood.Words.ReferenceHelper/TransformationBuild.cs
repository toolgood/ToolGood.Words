using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Bedrock;

namespace ToolGood.Words.ReferenceHelper
{
    public class TransformationBuild
    {
        private string fenciFile;


        public void CreateZip(string outFile, string fenciFile)
        {
            this.fenciFile = fenciFile;
            Directory.CreateDirectory(outFile);
            var ts = t2s();
            WriteGzip(outFile + "/t2s.dat.z", ts);
            var st = s2t();
            WriteGzip(outFile + "/s2t.dat.z", st);
            var hk = t2hk();
            WriteGzip(outFile + "/t2hk.dat.z", hk);
            var tw = t2tw();
            WriteGzip(outFile + "/t2tw.dat.z", tw);
        }

        private void WriteGzip(string outFile, Dictionary<string, string> ts)
        {
            List<string> list = new List<string>();
            foreach (var item in ts) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            var str = string.Join("\n", list);
            File.WriteAllBytes(outFile, CompressionUtil.GzipCompress(Encoding.UTF8.GetBytes(str)));
            //File.WriteAllText(outFile, str, Encoding.UTF8);
        }


        public void CreateBr(string outFile, string fenciFile)
        {
            this.fenciFile = fenciFile;
            Directory.CreateDirectory(outFile);
            var ts = t2s();
            WriteBr(outFile + "/t2s.dat.br", ts);
            var st = s2t();
            WriteBr(outFile + "/s2t.dat.br", st);
            var hk = t2hk();
            WriteBr(outFile + "/t2hk.dat.br", hk);
            var tw = t2tw();
            WriteBr(outFile + "/t2tw.dat.br", tw);
        }
        private void WriteBr(string outFile, Dictionary<string, string> ts)
        {
            List<string> list = new List<string>();
            foreach (var item in ts) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            var str = string.Join("\n", list);
            File.WriteAllBytes(outFile, CompressionUtil.BrCompress(Encoding.UTF8.GetBytes(str)));
            //File.WriteAllText(outFile, str, Encoding.UTF8);
        }

        public void CreateJava(string outFile, string fenciFile)
        {
            this.fenciFile = fenciFile;
            Directory.CreateDirectory(outFile);
            var ts = t2s();
            WriteJava(outFile + "/t2s.dat", ts);
            var st = s2t();
            WriteJava(outFile + "/s2t.dat", st);
            var hk = t2hk();
            WriteJava(outFile + "/t2hk.dat", hk);
            var tw = t2tw();
            WriteJava(outFile + "/t2tw.dat", tw);
        }
        private void WriteJava(string outFile, Dictionary<string, string> ts)
        {
            List<string> list = new List<string>();
            foreach (var item in ts) {
                list.Add($"{item.Key}\t{item.Value}");
            }
            var str = string.Join("\n", list);
            File.WriteAllText(outFile, str, Encoding.UTF8);
        }


        public void CreateJs(string outFile, string fenciFile)
        {

        }
        public void CreatePython(string outFile, string fenciFile)
        {

        }




        /// <summary>
        /// 繁体转简体
        /// </summary>
        private Dictionary<string, string> t2s()
        {
            var files = Directory.GetFiles("dict", "TS*.txt");
            var txts = new List<List<string>>();
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext")) {
                    ReadTexts(item, txts);
                }
            }
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext") == false) {
                    ReadTexts(item, txts);
                }
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in txts) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                if (dict.ContainsKey(st[0]) == false) {
                    dict[st[0]] = st[1];
                }
            }
            List<List<string>> rsp2 = SimplifyWords(txts, dict, null);
            foreach (var item in rsp2) {
                dict[item[0]] = item[1];
            }

            Dictionary<string, string> result = new Dictionary<string, string>();
            var keys = dict.Keys.OrderBy(q => q.Length).OrderBy(q => q).ToList();
            foreach (var item in keys) {
                result.Add(item, dict[item]);
            }
            keys = null;
            dict = null;
            txts = null;
            return result;
        }
        /// <summary>
        /// 简体转繁体
        /// </summary>
        private Dictionary<string, string> s2t()
        {
            var files = Directory.GetFiles("dict", "ST*.txt");
            var txts = new List<List<string>>();
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext")) {
                    ReadTexts(item, txts);
                }
            }
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext") == false) {
                    ReadTexts(item, txts);
                }
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in txts) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                if (dict.ContainsKey(st[0]) == false) {
                    dict[st[0]] = st[1];
                }
            }
            List<List<string>> stp2 = SimplifyWords4(txts, dict, null);
            foreach (var item in stp2) {
                dict[item[0]] = item[1];
            }

            Dictionary<string, string> result = new Dictionary<string, string>();
            var keys = dict.Keys.OrderBy(q => q.Length).OrderBy(q => q).ToList();
            foreach (var item in keys) {
                result.Add(item, dict[item]);
            }
            keys = null;
            dict = null;
            txts = null;
            return result;
        }

        private Dictionary<string, string> t2hk()
        {
            var files = Directory.GetFiles("dict", "ST*.txt");
            var txts = new List<List<string>>();
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext")) {
                    ReadTexts(item, txts);
                }
            }
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext") == false) {
                    ReadTexts(item, txts);
                }
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in txts) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                if (dict.ContainsKey(st[0]) == false) {
                    dict[st[0]] = st[1];
                }
            }
            //----------- 
            files = Directory.GetFiles("dict", "HK*.txt");
            var txts2 = new List<List<string>>();
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext")) {
                    ReadTexts(item, txts2);
                }
            }
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext") == false) {
                    ReadTexts(item, txts2);
                }
            }

            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            foreach (var st in txts2) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                if (st[0].Length > 1) { continue; }
                if (st[1].Length > 1) { continue; }
                if (dict2.ContainsKey(st[0]) == false) {
                    dict2[st[0]] = st[1];
                }
            }

            List<List<string>> stp22 = SimplifyWords(txts2, dict, dict2);

            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in dict2) {
                result.Add(item.Key, item.Value);
            }
            foreach (var item in stp22) {
                result.Add(item[0], item[1]);
            }
            return result;

        }

        private Dictionary<string, string> t2tw()
        {
            var files = Directory.GetFiles("dict", "ST*.txt");
            var txts = new List<List<string>>();
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext")) {
                    ReadTexts(item, txts);
                }
            }
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext") == false) {
                    ReadTexts(item, txts);
                }
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var st in txts) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                if (dict.ContainsKey(st[0]) == false) {
                    dict[st[0]] = st[1];
                }
            }
            //--------------------
            files = Directory.GetFiles("dict", "TW*.txt");
            var txts2 = new List<List<string>>();
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext")) {
                    ReadTexts(item, txts2);
                }
            }
            foreach (var item in files) {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("_Ext") == false) {
                    ReadTexts(item, txts2);
                }
            }

            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            foreach (var st in txts2) {
                if (st.Count == 2 && st[1] == st[0]) { continue; }
                if (st[0].Length > 1) { continue; }
                if (st[1].Length > 1) { continue; }
                if (dict2.ContainsKey(st[0]) == false) {
                    dict2[st[0]] = st[1];
                }
            }

            List<List<string>> stp22 = SimplifyWords(txts2, dict, dict2);

            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in dict2) {
                result.Add(item.Key, item.Value);
            }
            foreach (var item in stp22) {
                result.Add(item[0], item[1]);
            }
            return result;
        }



        /// <summary>
        /// 精细 转换
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dict"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        private List<List<string>> SimplifyWords(List<List<string>> src, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            List<List<string>> tarList = new List<List<string>>();
            List<List<string>> tempClearList = new List<List<string>>();

            // 保存
            foreach (var item in src) {
                if (item[0].Length == 1) { continue; } //防止一变多

                var tStr = ToTo(item[0], dict);
                if (dict2 != null) { tStr = ToTo(tStr, dict2); }
                if (tStr != item[1]) {
                    tarList.Add(item);
                } else {
                    tempClearList.Add(item);
                }
            }

            //清除重复的 词组
            tarList = SimplifyWords2(tarList, dict, dict2);

            // 由于算法是从前向后替换，只要保证前面的词组能够正确识别出来就可以了。
            List<string> firstChars = new List<string>();
            foreach (var item in tarList) {
                for (int i = 0; i < item[0].Length - 1; i++) {
                    firstChars.Add(item[0].Substring(0, item[0].Length - i));
                }
            }
            firstChars = firstChars.Distinct().OrderBy(q => q.Length).ToList();
            Words.WordsSearch wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(firstChars);


            List<List<string>> lastTempList = new List<List<string>>();
            foreach (var item in tempClearList) {
                var end = item[0].Length - 1;
                var all = wordsSearch.FindAll(item[0]);
                var f = all.Where(q => q.End == end).FirstOrDefault();
                if (f != null) {
                    lastTempList.Add(item);
                }
            }

            // 再来一次 清除重复的 词组
            lastTempList = SimplifyWords3(lastTempList, dict, dict2);

            // 
            var fullList = tarList.Select(q => q[0]).ToList();
            List<List<string>> containsTempList = new List<List<string>>();

            foreach (var item in tempClearList) {
                if (fullList.Contains(item[0])) {
                    containsTempList.Add(item);
                }
            }
            containsTempList = SimplifyWords2(containsTempList, dict, dict2);

            tarList.AddRange(lastTempList);
            tarList.AddRange(containsTempList);

            tarList = tarList.Distinct().ToList();
            tarList = tarList.OrderBy(q => q[0]).ToList();
            return tarList;
        }

        private List<List<string>> SimplifyWords2(List<List<string>> tarList, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            tarList = tarList.OrderBy(q => q[0].Length).ToList();
            for (int i = tarList.Count - 1; i >= 0; i--) {
                var item = tarList[i];

                for (int j = tarList.Count - 1; j > i; j--) {
                    var item2 = tarList[j];
                    if (item2[0].Contains(item[0])) {
                        StringBuilder stringBuilder = new StringBuilder();
                        var index = item2[0].IndexOf(item[0]);
                        if (index > 0) {
                            var t = item2[0].Substring(0, index);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        stringBuilder.Append(item[1]);
                        if (index + item[0].Length + 1 <= item2[0].Length) {
                            var t = item2[0].Substring(index + item[0].Length);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            tarList.RemoveAt(j);
                        }
                    }
                }
            }
            return tarList;
        }

        private List<List<string>> SimplifyWords3(List<List<string>> tarList, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            tarList = tarList.OrderBy(q => q[0].Length).ToList();
            for (int i = tarList.Count - 1; i >= 0; i--) {
                var item = tarList[i];

                for (int j = tarList.Count - 1; j > i; j--) {
                    var item2 = tarList[j];
                    if (item2[0].EndsWith(item[0])) {
                        StringBuilder stringBuilder = new StringBuilder();
                        var index = item2[0].LastIndexOf(item[0]);
                        if (index > 0) {
                            var t = item2[0].Substring(0, index);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        stringBuilder.Append(item[1]);
                        if (index + item[0].Length + 1 <= item2[0].Length) {
                            var t = item2[0].Substring(index + item[0].Length);
                            t = ToTo(t, dict);
                            if (dict2 != null) {
                                t = ToTo(t, dict2);
                            }
                            stringBuilder.Append(t);
                        }
                        if (stringBuilder.ToString() == item2[1]) {
                            tarList.RemoveAt(j);
                        }
                    }
                }
            }
            return tarList;
        }

        /// <summary>
        /// 仅用于简体
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dict"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        private List<List<string>> SimplifyWords4(List<List<string>> src, Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            List<List<string>> tarList = new List<List<string>>();
            List<List<string>> tempClearList = new List<List<string>>();

            // 保存
            foreach (var item in src) {
                if (item[0].Length == 1) { continue; } //防止一变多

                var tStr = ToTo(item[0], dict);
                if (dict2 != null) { tStr = ToTo(tStr, dict2); }
                if (tStr != item[1]) {
                    tarList.Add(item);
                } else {
                    tempClearList.Add(item);
                }
            }

            //清除重复的 词组
            tarList = SimplifyWords2(tarList, dict, dict2);

            // 由于算法是从前向后替换，只要保证前面的词组能够正确识别出来就可以了。
            List<string> firstChars = new List<string>();
            foreach (var item in tarList) {
                for (int i = 0; i < item[0].Length - 1; i++) {
                    var t = item[0].Substring(0, item[0].Length - i);
                    firstChars.Add(t);
                }
            }
            firstChars = firstChars.Distinct().OrderBy(q => q.Length).ToList();
            var srcWords = tarList.Select(q => q[0]).ToList();

            Words.WordsSearch wordsSearch = new Words.WordsSearch();
            wordsSearch.SetKeywords(firstChars);
            Words.WordsSearch wordsSearch2 = new Words.WordsSearch();
            wordsSearch2.SetKeywords(srcWords);

            List<string> containsTempList = new List<string>();
            var words = GetWords();
            if (words != null && words.Count > 0) {
                foreach (var item in words) {
                    var end = item.Length - 1;
                    var all = wordsSearch.FindAll(item);
                    var f = all.Where(q => q.End == end).FirstOrDefault();
                    if (f != null) {
                        if (wordsSearch2.ContainsAny(item) == false) {
                            containsTempList.Add(item);
                        }
                    }
                }
            }

            foreach (var item in tempClearList) {
                var end = item[0].Length - 1;
                var all = wordsSearch.FindAll(item[0]);
                var f = all.Where(q => q.End == end).FirstOrDefault();
                if (f != null) {
                    if (wordsSearch2.ContainsAny(item[0]) == false) {
                        containsTempList.Add(item[0]);
                    }
                }
            }

            containsTempList = containsTempList.Distinct().ToList();
            containsTempList = containsTempList.OrderBy(q => q.Length).ToList();
            // 清理 搜狗词库

            for (int i = 2; i < 8; i++) {
                var keywords = containsTempList.Where(q => q.Length <= i).ToList();
                wordsSearch = new Words.WordsSearch();
                wordsSearch.SetKeywords(keywords);

                for (int j = containsTempList.Count - 1; j >= i + 1; j--) {
                    var item = containsTempList[j];
                    if (item.Length <= i) { break; }

                    var end = item.Length - 1;
                    var all = wordsSearch.FindAll(item);
                    var f = all.Where(q => q.End == end).FirstOrDefault();
                    if (f != null) {
                        containsTempList.RemoveAt(j);
                    }
                }
            }

            containsTempList = containsTempList.Distinct().ToList();

            foreach (var item in containsTempList) {
                string s = "";
                foreach (var c in item) {
                    if (dict.TryGetValue(c.ToString(), out string v)) {
                        s += v;
                    } else {
                        s += c;
                    }
                }
                tarList.Add(new List<string>() { item, s });
            }

            tarList = tarList.Distinct().ToList();
            tarList = tarList.OrderBy(q => q[0]).ToList();
            return tarList;
        }

        private static string ToTo(string srcText, Dictionary<string, string> dict)
        {
            var str = "";
            foreach (var c in srcText) {
                var s = c.ToString();
                if (dict.TryGetValue(s, out string v)) {
                    str += v;
                } else {
                    str += s;
                }
            }
            return str;
        }


        private void ReadTexts(string file, List<List<string>> list)
        {
            var ts = File.ReadAllLines(file);
            foreach (var t in ts) {
                if (string.IsNullOrEmpty(t) == false) {
                    var sp = t.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> ls = new List<string>();
                    foreach (var item in sp) {
                        ls.Add(item.Trim());
                    }
                    list.Add(ls);
                }
            }
        }


        private List<string> _fencis;

        List<string> GetWords()
        {
            if (_fencis == null) {
                if (fenciFile == null) {
                    return null;
                }
                if (File.Exists(fenciFile)) {
                    var txts = File.ReadAllLines(fenciFile);
                    List<string> r = new List<string>();
                    foreach (var txt in txts) {
                        if (string.IsNullOrEmpty(txt)) {
                            continue;
                        }
                        var sp = txt.Split("\t,:| '\"-=>[]，　123456789?".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        r.Add(sp[0]);
                    }
                    _fencis = r;
                }
            }
            return _fencis;
        }
    }
}
