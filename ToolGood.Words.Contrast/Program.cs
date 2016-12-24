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
        //static HashFilter hf = new HashFilter();
        static TrieSearch tf = new TrieSearch();
        //static TrieSearch2 ts1 = new TrieSearch2();
        static TrieFilter tf1 = new TrieFilter();
        static FastFilter ff = new FastFilter();

        static TextSearch search;
        //static StringSearch stringSearch;
        //static IllegalWords words_1 = new IllegalWords();
        //static IllegalWords words_1_quick;
        //static IllegalWords words_2;
        //static IllegalWords words_2_quick;


        static void Main(string[] args)
        {
            ReadBadWord();
            var text = File.ReadAllText("Talk.txt");
            //ts1.HasBadWord(text);

            Run("TrieSearch", () => { tf.HasBadWord(text); });
            //Run("TrieSearch 1 ", () => { ts1.HasBadWord(text); });
            Run("TextSearch  ", () => { search.ContainsAny(text); });


            Run("TrieFilter", () => { tf1.HasBadWord(text); });
            Run("FastFilter", () => { ff.HasBadWord(text); });

            Console.WriteLine("----------------------- Find All -----------------------------------");

            Run("TrieSearch", () => { tf.FindAll(text); });
            //Run("TrieSearch 1 ", () => { ts1.FindAll(text); });
            Run("TextSearch  ", () => { search.FindAll(text); });

            Run("TrieFilter", () => { tf1.FindAll(text); });

            Run("FastFilter", () => { ff.FindAll(text); });

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
            //stringSearch = new StringSearch();
            List<string> list = new List<string>();
            using (StreamReader sw = new StreamReader(File.OpenRead("BadWord.txt"))) {
                string key = sw.ReadLine();
                while (key != null) {
                    if (key != string.Empty) {
                        //hf.AddKey(key);
                        tf.AddKey(key);
                        tf1.AddKey(key);
                        //ts1.AddKey(key);

                        ff.AddKey(key);

                        list.Add(key);
                    }
                    key = sw.ReadLine();
                }
            }
            search = new TextSearch();
            search.Keywords = list.ToArray();

        }



    }
}
