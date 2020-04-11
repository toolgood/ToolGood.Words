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
        static StringSearchEx2 stringSearchEx2 = new StringSearchEx2();
        static StringSearchEx3 stringSearchEx3 = new StringSearchEx3();

        static WordsSearch wordsSearch = new WordsSearch();
        static WordsSearchEx wordsSearchEx = new WordsSearchEx();
        static WordsSearchEx2 wordsSearchEx2 = new WordsSearchEx2();
        static WordsSearchEx3 wordsSearchEx3 = new WordsSearchEx3();


        static IllegalWordsSearch illegalWordsSearch = new IllegalWordsSearch();



        static Regex re;
        static Regex re2;

        static void Main(string[] args)
        {
            ReadBadWord();
            var text = File.ReadAllText("Talk.txt");


            Console.Write("-------------------- FindFirst OR ContainsAny 100000次 --------------------\r\n");
            Run("TrieFilter", () => { tf1.HasBadWord(text); });
            Run("FastFilter", () => { ff.HasBadWord(text); });
            Run("StringSearch（ContainsAny）", () => { stringSearch.ContainsAny(text); });
            Run("StringSearchEx（ContainsAny）--- WordsSearchEx（ContainsAny）代码相同", () => { stringSearchEx.ContainsAny(text); });
            Run("StringSearchEx2（ContainsAny）--- WordsSearchEx2（ContainsAny）代码相同", () => { stringSearchEx2.ContainsAny(text); });
            Run("StringSearchEx3（ContainsAny）--- WordsSearchEx3（ContainsAny）代码相同", () => { stringSearchEx3.ContainsAny(text); });
            Run("IllegalWordsSearch（ContainsAny）", () => { illegalWordsSearch.ContainsAny(text); });

            Run("StringSearch（FindFirst）", () => { stringSearch.FindFirst(text); });
            Run("StringSearchEx（FindFirst）", () => { stringSearchEx.FindFirst(text); });
            Run("StringSearchEx2（FindFirst）", () => { stringSearchEx2.FindFirst(text); });
            Run("StringSearchEx3（FindFirst）", () => { stringSearchEx3.FindFirst(text); });
            Run("WordsSearch（FindFirst）", () => { wordsSearch.FindFirst(text); });
            Run("WordsSearchEx（FindFirst）", () => { wordsSearchEx.FindFirst(text); });
            Run("WordsSearchEx2（FindFirst）", () => { wordsSearchEx2.FindFirst(text); });
            Run("WordsSearchEx3（FindFirst）", () => { wordsSearchEx3.FindFirst(text); });
            Run("IllegalWordsSearch（FindFirst）", () => { illegalWordsSearch.FindFirst(text); });


            Console.Write("-------------------- Find All 100000次 --------------------\r\n");
            Run("TrieFilter（FindAll）", () => { tf1.FindAll(text); });
            Run("FastFilter（FindAll）", () => { ff.FindAll(text); });
            Run("StringSearch（FindAll）", () => { stringSearch.FindAll(text); });
            Run("StringSearchEx（FindAll）", () => { stringSearchEx.FindAll(text); });
            Run("StringSearchEx2（FindAll）", () => { stringSearchEx2.FindAll(text); });
            Run("StringSearchEx3（FindAll）", () => { stringSearchEx3.FindAll(text); });

            Run("WordsSearch（FindAll）", () => { wordsSearch.FindAll(text); });
            Run("WordsSearchEx（FindAll）", () => { wordsSearchEx.FindAll(text); });
            Run("WordsSearchEx2（FindAll）", () => { wordsSearchEx2.FindAll(text); });
            Run("WordsSearchEx3（FindAll）", () => { wordsSearchEx3.FindAll(text); });
            Run("IllegalWordsSearch（FindAll）", () => { illegalWordsSearch.FindAll(text); });

            Console.Write("-------------------- Replace  100000次 --------------------\r\n");
            Run("TrieFilter（Replace）", () => { tf1.Replace(text); });
            Run("FastFilter（Replace）", () => { ff.Replace(text); });
            Run("StringSearch（Replace）", () => { stringSearch.Replace(text); });
            Run("WordsSearch（Replace）", () => { wordsSearch.Replace(text); });
            Run("StringSearchEx（Replace）--- WordsSearchEx（Replace）代码相同", () => { stringSearchEx.Replace(text); });
            Run("StringSearchEx2（Replace）--- WordsSearchEx2（Replace）代码相同", () => { stringSearchEx2.Replace(text); });
            Run("StringSearchEx3（Replace）--- WordsSearchEx3（Replace）代码相同", () => { stringSearchEx3.Replace(text); });
            Run("IllegalWordsSearch（Replace）", () => { illegalWordsSearch.Replace(text); });

            Console.Write("-------------------- Regex  100次 --------------------\r\n");
            Run(100, "Regex.IsMatch", () => { re.IsMatch(text); });
            Run(100, "Regex.Match", () => { re.Match(text); });
            Run(100, "Regex.Matches", () => { re.Matches(text); });

            Console.Write("-------------------- Regex used Trie tree  100次 --------------------\r\n");
            Run(100, "Regex.IsMatch", () => { re2.IsMatch(text); });
            Run(100, "Regex.Match", () => { re2.Match(text); });
            Run(100, "Regex.Matches", () => { re2.Matches(text); });

            Console.ReadKey();

        }
        static void Run(int num, string title, Action action)
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
            stringSearch.SetKeywords(list);
            stringSearchEx.SetKeywords(list);
            stringSearchEx2.SetKeywords(list);
            stringSearchEx3.SetKeywords(list);
            wordsSearch.SetKeywords(list);
            wordsSearchEx.SetKeywords(list);
            wordsSearchEx2.SetKeywords(list);
            wordsSearchEx3.SetKeywords(list);
            illegalWordsSearch.SetKeywords(list);

            list = list.OrderBy(q => q).ToList();
            var str = string.Join("|", list);
            str = Regex.Replace(str, @"([\\\.\+\*\-\(\)\[\]\{\}!])", @"\$1");

            re = new Regex(str, RegexOptions.IgnoreCase);


            var str2 = tf1.ToString();
            //str2 = Regex.Replace(str2, @"([\.\+\*\-\[\]\{\}!])", @"\$1");
            re2 = new Regex(str2);

            return list;
        }



    }
}
