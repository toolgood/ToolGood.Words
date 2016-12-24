using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Sinan.Util;
using ToolGood.Words;

namespace ToolGood.Words.Contrast
{
    class Program
    {
        static TrieFilter tf1 = new TrieFilter();
        static FastFilter ff = new FastFilter();
        static StringSearch word = new StringSearch();
        static TextSearch search;


        static void Main(string[] args)
        {
            ReadBadWord();
            var text = File.ReadAllText("Talk.txt");

            Run("TextSearch  ", () => { search.ContainsAny(text); });
            Run("TrieFilter", () => { tf1.HasBadWord(text); });
            Run("FastFilter", () => { ff.HasBadWord(text); });
            Run("StringSearch  ", () => { word.ContainsAny(text); });

            Console.WriteLine("----------------------- Find All -----------------------------------");

            Run("TextSearch  ", () => { search.FindAll(text); });
            Run("TrieFilter", () => { tf1.FindAll(text); });
            Run("FastFilter", () => { ff.FindAll(text); });
            Run("StringSearch  ", () => { word.FindAll(text); });

            Console.ReadKey();

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



        static void ReadBadWord()
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
            search = new TextSearch();
            search.Keywords = list.ToArray();
            word.SetKeywords(list);
        }



    }
}
