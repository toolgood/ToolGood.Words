using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
#if NETSTANDARD2_1
using ZIPStream = System.IO.Compression.BrotliStream;
#else
using ZIPStream = System.IO.Compression.GZipStream;
#endif

namespace ToolGood.Words.internals
{
    class Translate
    {
        private static WordsSearch s2tSearch;
        private static WordsSearch t2sSearch;
        private static WordsSearch t2twSearch;
        private static WordsSearch tw2tSearch;
        private static WordsSearch t2hkSearch;
        private static WordsSearch hk2tSearch;


        /// <summary>
        /// 转繁体中文
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type">0、繁体中文，1、港澳繁体，2、台湾正体 </param>
        /// <returns></returns>
        public static string ToTraditionalChinese(string text, int type = 0)
        {
            if (type > 2 || type < 0) { throw new Exception("type 不支持该类型"); }

            var s2t = GetWordsSearch(true, 0);
            text = TransformationReplace(text, s2t);
            if (type > 0) {
                var t2 = GetWordsSearch(true, type);
                text = TransformationReplace(text, t2);
            }
            return text;
        }

        /// <summary>
        /// 转简体中文
        /// </summary>
        /// <param name="text"></param>
        /// <param name="srcType">0、繁体中文，1、港澳繁体，2、台湾正体</param>
        /// <returns></returns>
        public static string ToSimplifiedChinese(string text, int srcType = 0)
        {
            if (srcType > 2 || srcType < 0) { throw new Exception("srcType 不支持该类型"); }
            if (srcType > 0) {
                var t2 = GetWordsSearch(false, srcType);
                text = TransformationReplace(text, t2);
            }
            var s2t = GetWordsSearch(false, 0);
            text = TransformationReplace(text, s2t);
            return text;
        }

        private static string TransformationReplace(string text, WordsSearch wordsSearch)
        {
            var ts = wordsSearch.FindAll(text);
            StringBuilder sb = new StringBuilder();
            var index = 0;
            while (index < text.Length) {
                var t = ts.Where(q => q.Start == index).OrderByDescending(q => q.End).FirstOrDefault();
                if (t == null) {
                    sb.Append(text[index]);
                    index++;
                } else {
                    sb.Append(wordsSearch._others[t.Index]);
                    index = t.End + 1;
                }
            }
            return sb.ToString();
        }
        private static object lockObj = new object();
        private static WordsSearch GetWordsSearch(bool s2t, int srcType)
        {
            if (s2t) {
                if (srcType == 0) {
                    if (s2tSearch == null) {
                        lock (lockObj) {
                            if (s2tSearch == null) {
                                s2tSearch = BuildWordsSearch("s2t.dat", false);
                            }
                        }
                    }
                    return s2tSearch;
                } else if (srcType == 1) {
                    if (t2hkSearch == null) {
                        lock (lockObj) {
                            if (t2hkSearch == null) {
                                t2hkSearch = BuildWordsSearch("t2hk.dat", false);
                            }
                        }
                    }
                    return t2hkSearch;
                } else if (srcType == 2) {
                    if (t2twSearch == null) {
                        lock (lockObj) {
                            if (t2twSearch == null) {
                                t2twSearch = BuildWordsSearch("t2tw.dat", false);
                            }
                        }
                    }
                    return t2twSearch;
                }
            } else {
                if (srcType == 0) {
                    if (t2sSearch == null) {
                        lock (lockObj) {
                            if (t2sSearch == null) {
                                t2sSearch = BuildWordsSearch("t2s.dat", false);
                            }
                        }
                    }
                    return t2sSearch;
                } else if (srcType == 1) {
                    if (hk2tSearch == null) {
                        lock (lockObj) {
                            if (hk2tSearch == null) {
                                hk2tSearch = BuildWordsSearch("t2hk.dat", true);
                            }
                        }
                    }
                    return hk2tSearch;
                } else if (srcType == 2) {
                    if (tw2tSearch == null) {
                        lock (lockObj) {
                            if (tw2tSearch == null) {
                                tw2tSearch = BuildWordsSearch("t2tw.dat", true);
                            }
                        }
                    }
                    return tw2tSearch;
                }
            }
            return null;
        }

        private static WordsSearch BuildWordsSearch(string fileName, bool reverse)
        {
            var dict = GetTransformationDict(fileName);
            WordsSearch wordsSearch = new WordsSearch();
            if (reverse) {
                wordsSearch.SetKeywords(dict.Select(q => q.Value).ToList());
                wordsSearch._others = dict.Select(q => q.Key).ToArray();
            } else {
                wordsSearch.SetKeywords(dict.Select(q => q.Key).ToList());
                wordsSearch._others = dict.Select(q => q.Value).ToArray();
            }
            return wordsSearch;
        }

        internal static Dictionary<string, string> GetTransformationDict(string fileName)
        {
            var ass = typeof(WordsHelper).Assembly;
            var dir = Path.GetDirectoryName(ass.Location);
            var file = Path.Combine(dir, fileName);
            string tStr;
            //if (File.Exists(file)) {
            //    tStr = File.ReadAllText(file);
            //} else {
#if NETSTANDARD2_1
                var resourceName = "ToolGood.Words.dict." + fileName + ".br";
#else
            var resourceName = "ToolGood.Words.dict." + fileName + ".z";
#endif
            Stream sm = ass.GetManifestResourceStream(resourceName);
            byte[] bs = new byte[sm.Length];
            sm.Read(bs, 0, (int)sm.Length);
            sm.Close();
            var bytes = Decompress(bs);
            tStr = Encoding.UTF8.GetString(bytes);
            //}
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var sp = tStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in sp) {
                var ss = s.Split('\t');
                if (ss.Length < 2) { continue; }
                dict[ss[0]] = ss[1];
            }
            return dict;
        }
        private static byte[] Decompress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;
            try {
                using (MemoryStream stream = new MemoryStream(data)) {
                    using (var zStream = new ZIPStream(stream, CompressionMode.Decompress)) {
                        using (var resultStream = new MemoryStream()) {
                            zStream.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    }
                }
            } catch {
                return data;
            }
        }
    }
}
