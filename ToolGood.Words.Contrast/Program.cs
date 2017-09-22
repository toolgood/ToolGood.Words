using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sinan.Util;
using ToolGood.Words;

namespace ToolGood.Words.Contrast
{
    class Program
    {
        static TrieFilter tf1 = new TrieFilter();
        static FastFilter ff = new FastFilter();
        static StringSearch stringSearch = new StringSearch();
        static StringSearchEx stringSearchEx = new StringSearchEx();
        static WordsSearch wordsSearch = new WordsSearch();
        static WordsSearchEx wordsSearchEx = new WordsSearchEx();
        static IllegalWordsQuickSearch illegalWordsQuickSearch = new IllegalWordsQuickSearch();
        static IllegalWordsSearch illegalWordsSearch = new IllegalWordsSearch();
        static Regex re;
        static Regex re2;

        static void Main(string[] args)
        {
            ReadBadWord();
            var text = File.ReadAllText("Talk.txt");
            //var l = text.Length;
            stringSearchEx.Save("test.ini");
            var tt = stringSearchEx.FindAll(text);

            //word2.Load("test.ini");
            Console.Write("-------------------- SetKeywords Test --------------------\r\n");

            Run(1, "StringSearch.SetKeywords  ", () => {
                List<string> list = new List<string>();
                using (StreamReader sw = new StreamReader(File.OpenRead("BadWord.txt"))) {
                    string key = sw.ReadLine();
                    while (key != null) {
                        if (key != string.Empty) {
                            list.Add(key);
                        }
                        key = sw.ReadLine();
                    }
                }
                StringSearch s = new StringSearch();
                s.SetKeywords(list);
            });
            Run(1, "StringSearchEx.SetKeywords  ", () => {
                List<string> list = new List<string>();
                using (StreamReader sw = new StreamReader(File.OpenRead("BadWord.txt"))) {
                    string key = sw.ReadLine();
                    while (key != null) {
                        if (key != string.Empty) {
                            list.Add(key);
                        }
                        key = sw.ReadLine();
                    }
                }
                StringSearchEx s = new StringSearchEx();
                s.SetKeywords(list);
            });
            Run(1, "StringSearchEx.Load  ", () => {
                StringSearchEx s = new StringSearchEx();
                s.Load("test.ini");
            });




            //var ts1 = word.FindAll(text);
            //var ts = word2.FindAll(text);
            Console.Write("-------------------- ToSenseWord Test --------------------\r\n");

            Run("ToSenseWord1  ", () => { WordTest.ToSenseWord1(text); });
            Run("ToSenseWord2  ", () => { WordTest.ToSenseWord2(text); });
            Run("ToSenseWord3  ", () => { WordTest.ToSenseWord3(text); });
            Run("ToSenseWord4  ", () => { WordTest.ToSenseWord4(text); });
            Run("ToSenseWord5  ", () => { WordTest.ToSenseWord5(text); });
            Run("ToSenseWord6  ", () => { WordTest.ToSenseWord6(text); });
            Run("ToSenseWord7  ", () => { WordTest.ToSenseWord7(text); });
            Run("ToSenseWord8  ", () => { WordTest.ToSenseWord8(text); });
            Run("ToSenseWord9  ", () => { WordTest.ToSenseWord9(text); });
            Run("ToSenseWord10  ", () => { WordTest.ToSenseWord10(text); });

            Run("GetDisablePostion1  ", () => { WordTest.GetDisablePostion1(text); });
            Run("GetDisablePostion2  ", () => { WordTest.GetDisablePostion2(text); });
            Run("GetDisablePostion3  ", () => { WordTest.GetDisablePostion3(text); });
            Run("GetDisablePostion4  ", () => { WordTest.GetDisablePostion4(text); });
            Run("GetDisablePostion5  ", () => { WordTest.GetDisablePostion5(text); });
            Run("GetDisablePostion6  ", () => { WordTest.GetDisablePostion6(text); });
            Run("GetDisablePostion7  ", () => { WordTest.GetDisablePostion7(text); });
            Run("GetDisablePostion9  ", () => { WordTest.GetDisablePostion9(text); });
            Run("GetDisablePostion8  ", () => { WordTest.GetDisablePostion8(text); });

            //Console.Write("-------------------- ToSenseIllegalWords --------------------\r\n");

            //Run("ToSenseIllegalWords", () => { WordsHelper.ToSenseIllegalWords(text); });



            Console.Write("-------------------- FindFirst OR ContainsAny --------------------\r\n");
            Run("TrieFilter", () => { tf1.HasBadWord(text); });
            Run("FastFilter", () => { ff.HasBadWord(text); });
            Run("StringSearch（ContainsAny）", () => { stringSearch.ContainsAny(text); });
            Run("StringSearch（FindFirst）", () => { stringSearch.FindFirst(text); });
            Run("StringSearchEx（ContainsAny）", () => { stringSearchEx.ContainsAny(text); });
            Run("StringSearchEx（FindFirst）", () => { stringSearchEx.FindFirst(text); });
            Run("WordsSearch（ContainsAny）", () => { wordsSearch.ContainsAny(text); });
            Run("WordsSearch（FindFirst）", () => { wordsSearch.FindFirst(text); });
            Run("WordsSearchEx（ContainsAny）", () => { wordsSearchEx.ContainsAny(text); });
            Run("WordsSearchEx（FindFirst）", () => { wordsSearchEx.FindFirst(text); });
            Run("IllegalWordsQuickSearch（FindFirst）", () => { illegalWordsQuickSearch.FindFirst(text); });
            Run("IllegalWordsQuickSearch（ContainsAny）", () => { illegalWordsQuickSearch.ContainsAny(text); });

            Run("IllegalWordsSearch（FindFirst）", () => { illegalWordsSearch.FindFirst(text); });
            Run("IllegalWordsSearch（ContainsAny）", () => { illegalWordsSearch.ContainsAny(text); });

            Console.Write("-------------------- Find All --------------------\r\n");
            Run("TrieFilter（FindAll）", () => { tf1.FindAll(text); });
            Run("FastFilter（FindAll）", () => { ff.FindAll(text); });
            Run("StringSearch（FindAll）", () => { stringSearch.FindAll(text); });
            Run("StringSearchEx（FindAll）", () => { stringSearchEx.FindAll(text); });
            Run("WordsSearch（FindAll）", () => { wordsSearch.FindAll(text); });
            Run("WordsSearchEx（FindAll）", () => { wordsSearchEx.FindAll(text); });
            Run("IllegalWordsQuickSearch（FindAll）", () => { illegalWordsQuickSearch.FindAll(text); });
            Run("IllegalWordsSearch（FindAll）", () => { illegalWordsSearch.FindAll(text); });
            Console.Write("-------------------- Replace --------------------\r\n");
            Run("TrieFilter（Replace）", () => { tf1.Replace(text); });
            Run("FastFilter（Replace）", () => { ff.Replace(text); });
            Run("StringSearch（Replace）", () => { stringSearch.Replace(text); });
            Run("StringSearchEx（Replace）", () => { stringSearchEx.Replace(text); });

            Run("WordsSearch（Replace）", () => { wordsSearch.Replace(text); });
            Run("WordsSearchEx（Replace）", () => { wordsSearchEx.Replace(text); });
            Run("IllegalWordsQuickSearch（Replace）", () => { illegalWordsQuickSearch.Replace(text); });
            Run("IllegalWordsSearch（Replace）", () => { illegalWordsSearch.Replace(text); });

            Console.Write("-------------------- Regex --------------------\r\n");
            Run("Regex.IsMatch", () => { re.IsMatch(text); });
            Run("Regex.Match", () => { re.Match(text); });
            Run("Regex.Matches", () => { re.Matches(text); });

            Console.Write("-------------------- Regex used Trie tree --------------------\r\n");
            Run("Regex.IsMatch", () => { re2.IsMatch(text); });
            Run("Regex.Match", () => { re2.Match(text); });
            Run("Regex.Matches", () => { re2.Matches(text); });


            Console.ReadKey();

        }
        static void Run(int num,string title, Action action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < num; i++) {
                action();
            }
            watch.Stop();
            Console.WriteLine(title + " : " + watch.ElapsedMilliseconds.ToString("N0") + "ms");
        }
        static void Run(string title, Action action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 100000; i++) {
                action();
            }
            watch.Stop();
            Console.WriteLine(title + " : " + watch.ElapsedMilliseconds.ToString("N0") + "ms");
        }



        static List<string> ReadBadWord()
        {
            List<string> list = new List<string>();
            using (StreamReader sw = new StreamReader(File.OpenRead("BadWord.txt"))) {
                string key = sw.ReadLine();
                while (key != null) {
                    if (key != string.Empty) {
                        tf1.AddKey(key);

                        ff.AddKey(key);

                        list.Add(key);
                    }
                    key = sw.ReadLine();
                }
            }
            //search = new TextSearch();
            //search.Keywords = list.ToArray();
            stringSearch.SetKeywords(list);
            stringSearchEx.SetKeywords(list);
            wordsSearch.SetKeywords(list);
            wordsSearchEx.SetKeywords(list);
            illegalWordsQuickSearch.SetKeywords(list);
            illegalWordsSearch.SetKeywords(list);
            //iword3 = new IllegalWordsSearch2(list);
            list = list.OrderBy(q => q).ToList();
            var str = string.Join("|", list);
            str = Regex.Replace(str, @"([\\\.\+\*\-\(\)\[\]\{\}!])", @"\$1");

            re = new Regex(str);


            var str2 = tf1.ToString();
            //str2 = Regex.Replace(str2, @"([\.\+\*\-\[\]\{\}!])", @"\$1");
            re2 = new Regex(str2);

            return list;
        }



    }
}
