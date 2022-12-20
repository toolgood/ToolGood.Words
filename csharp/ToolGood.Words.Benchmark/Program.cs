using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ToolGood.Words.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);


        }

//BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.18363.1556/1909/November2019Update/19H2)
//Intel Core i5-8500 CPU 3.00GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
//.NET SDK=7.0.101
//  [Host]     : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2
//  DefaultJob : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2

//|                            Method |       Mean |     Error |    StdDev |     Median |
//|---------------------------------- |-----------:|----------:|----------:|-----------:|
//|          stringSearchEx_FindFirst |   202.6 ns |   1.06 ns |   0.99 ns |   202.4 ns |
//|    stringSearchExUnsafe_FindFirst |   186.3 ns |   1.48 ns |   1.38 ns |   186.0 ns |
//|   stringSearchExUnsafe2_FindFirst |   201.0 ns |   0.91 ns |   0.85 ns |   200.9 ns |
//|            stringSearchEx_FindAll | 5,225.2 ns |  73.65 ns |  68.89 ns | 5,232.7 ns |
//|      stringSearchExUnsafe_FindAll | 5,505.1 ns |  59.54 ns |  55.69 ns | 5,520.9 ns |
//|     stringSearchExUnsafe2_FindAll | 4,816.7 ns |  88.84 ns |  78.76 ns | 4,786.7 ns |
//|           wordsSearchEx_FindFirst |   216.3 ns |   0.74 ns |   0.58 ns |   216.4 ns |
//|     wordsSearchExUnsafe_FindFirst |   191.0 ns |   2.05 ns |   1.92 ns |   190.5 ns |
//|    wordsSearchExUnsafe2_FindFirst |   186.6 ns |   1.37 ns |   1.21 ns |   186.9 ns |
//|             wordsSearchEx_FindAll | 4,973.3 ns |  31.06 ns |  27.54 ns | 4,965.8 ns |
//|       wordsSearchExUnsafe_FindAll | 5,000.1 ns |  46.89 ns |  41.57 ns | 4,999.0 ns |
//|      wordsSearchExUnsafe2_FindAll | 5,279.5 ns |  89.92 ns |  84.12 ns | 5,290.6 ns |
//|            stringSearchEx_Replace | 5,499.2 ns | 103.59 ns |  96.90 ns | 5,496.5 ns |
//|      stringSearchExUnsafe_Replace | 5,583.5 ns | 110.61 ns | 220.90 ns | 5,470.7 ns |
//|     stringSearchExUnsafe2_Replace | 6,076.9 ns |  73.81 ns |  69.04 ns | 6,048.8 ns |
//|        stringSearchEx_ContainsAny |   191.0 ns |   1.04 ns |   0.97 ns |   191.0 ns |
//|  stringSearchExUnsafe_ContainsAny |   176.2 ns |   0.44 ns |   0.37 ns |   176.1 ns |
//| stringSearchExUnsafe2_ContainsAny |   167.3 ns |   1.32 ns |   1.17 ns |   167.4 ns |
    }

    public class Test
    {
        public SearchExs.StringSearchEx stringSearchEx;
        public SearchExs.StringSearchExUnsafe stringSearchExUnsafe;
        public SearchExs.StringSearchExUnsafe2 stringSearchExUnsafe2;
        public SearchExs.WordsSearchEx wordsSearchEx;
        public SearchExs.WordsSearchExUnsafe wordsSearchExUnsafe;
        public SearchExs.WordsSearchExUnsafe2 wordsSearchExUnsafe2;
        private string text;

        public Test() {
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
            stringSearchEx =new SearchExs.StringSearchEx();
            stringSearchEx.SetKeywords(list);
            stringSearchExUnsafe=new SearchExs.StringSearchExUnsafe();
            stringSearchExUnsafe.SetKeywords(list);
            stringSearchExUnsafe2 = new SearchExs.StringSearchExUnsafe2();
            stringSearchExUnsafe2.SetKeywords(list);

            wordsSearchEx =new SearchExs.WordsSearchEx();
            wordsSearchEx.SetKeywords(list);
            wordsSearchExUnsafe = new SearchExs.WordsSearchExUnsafe();
            wordsSearchExUnsafe.SetKeywords(list);
            wordsSearchExUnsafe2 = new SearchExs.WordsSearchExUnsafe2();
            wordsSearchExUnsafe2.SetKeywords(list);

            text =File.ReadAllText("Talk.txt");
        }
        [Benchmark]
        public string stringSearchEx_FindFirst() => stringSearchEx.FindFirst(text);
        [Benchmark]
        public string stringSearchExUnsafe_FindFirst() => stringSearchExUnsafe.FindFirst(text);
        [Benchmark]
        public string stringSearchExUnsafe2_FindFirst() => stringSearchExUnsafe2.FindFirst(text);

        [Benchmark]
        public List<string> stringSearchEx_FindAll() => stringSearchEx.FindAll(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe_FindAll() => stringSearchExUnsafe.FindAll(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe2_FindAll() => stringSearchExUnsafe2.FindAll(text);

        [Benchmark]
        public WordsSearchResult wordsSearchEx_FindFirst() => wordsSearchEx.FindFirst(text);
        [Benchmark]
        public WordsSearchResult wordsSearchExUnsafe_FindFirst() => wordsSearchExUnsafe.FindFirst(text);
        [Benchmark]
        public WordsSearchResult wordsSearchExUnsafe2_FindFirst() => wordsSearchExUnsafe2.FindFirst(text);

        [Benchmark]
        public List<WordsSearchResult> wordsSearchEx_FindAll() => wordsSearchEx.FindAll(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchExUnsafe_FindAll() => wordsSearchExUnsafe.FindAll(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchExUnsafe2_FindAll() => wordsSearchExUnsafe2.FindAll(text);


        [Benchmark]
        public string stringSearchEx_Replace() => stringSearchEx.Replace(text);
        [Benchmark]
        public string stringSearchExUnsafe_Replace() => stringSearchExUnsafe.Replace(text);
        [Benchmark]
        public string stringSearchExUnsafe2_Replace() => stringSearchExUnsafe2.Replace(text);
        [Benchmark]
        public bool stringSearchEx_ContainsAny() => stringSearchEx.ContainsAny(text);
        [Benchmark]
        public bool stringSearchExUnsafe_ContainsAny() => stringSearchExUnsafe.ContainsAny(text);
        [Benchmark]
        public bool stringSearchExUnsafe2_ContainsAny() => stringSearchExUnsafe2.ContainsAny(text);
    }
}