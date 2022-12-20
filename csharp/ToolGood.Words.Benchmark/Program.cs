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

//|                            Method |       Mean |    Error |   StdDev |
//|---------------------------------- |-----------:|---------:|---------:|
//|          stringSearchEx_FindFirst |   204.3 ns |  1.63 ns |  1.53 ns |
//|         stringSearchEx_FindFirst2 |   189.0 ns |  1.15 ns |  1.08 ns |
//|         stringSearchEx_FindFirst3 |   202.3 ns |  0.89 ns |  0.78 ns |
//|    stringSearchExUnsafe_FindFirst |   187.4 ns |  1.27 ns |  1.18 ns |
//|   stringSearchExUnsafe_FindFirst2 |   191.7 ns |  1.08 ns |  0.91 ns |
//|   stringSearchExUnsafe_FindFirst3 |   187.1 ns |  0.63 ns |  0.49 ns |
//|   stringSearchExUnsafe2_FindFirst |   205.1 ns |  1.47 ns |  1.37 ns |
//|  stringSearchExUnsafe2_FindFirst2 |   189.5 ns |  0.72 ns |  0.67 ns |
//|  stringSearchExUnsafe2_FindFirst3 |   190.6 ns |  1.10 ns |  0.98 ns |
//|            stringSearchEx_FindAll | 5,019.7 ns | 34.00 ns | 30.14 ns |
//|           stringSearchEx_FindAll2 | 5,061.7 ns | 43.48 ns | 38.54 ns |
//|           stringSearchEx_FindAll3 | 4,909.6 ns | 39.90 ns | 35.37 ns |
//|      stringSearchExUnsafe_FindAll | 5,398.3 ns | 20.81 ns | 17.38 ns |
//|     stringSearchExUnsafe_FindAll2 | 5,573.8 ns | 84.90 ns | 79.42 ns |
//|     stringSearchExUnsafe_FindAll3 | 4,876.4 ns | 36.78 ns | 32.60 ns |
//|     stringSearchExUnsafe2_FindAll | 4,920.4 ns | 93.28 ns | 87.25 ns |
//|    stringSearchExUnsafe2_FindAll2 | 4,916.1 ns | 33.01 ns | 29.26 ns |
//|    stringSearchExUnsafe2_FindAll3 | 5,255.1 ns | 70.29 ns | 65.75 ns |
//|           wordsSearchEx_FindFirst |   215.7 ns |  1.05 ns |  0.93 ns |
//|          wordsSearchEx_FindFirst2 |   192.2 ns |  1.16 ns |  1.08 ns |
//|     wordsSearchExUnsafe_FindFirst |   190.5 ns |  1.07 ns |  0.95 ns |
//|    wordsSearchExUnsafe_FindFirst2 |   179.8 ns |  1.30 ns |  1.22 ns |
//|    wordsSearchExUnsafe2_FindFirst |   191.8 ns |  1.06 ns |  0.99 ns |
//|   wordsSearchExUnsafe2_FindFirst2 |   194.2 ns |  0.63 ns |  0.53 ns |
//|             wordsSearchEx_FindAll | 5,068.7 ns | 74.23 ns | 69.43 ns |
//|            wordsSearchEx_FindAll2 | 5,009.8 ns | 28.80 ns | 25.53 ns |
//|       wordsSearchExUnsafe_FindAll | 4,900.8 ns | 38.10 ns | 35.64 ns |
//|      wordsSearchExUnsafe_FindAll2 | 5,148.1 ns | 78.37 ns | 65.44 ns |
//|      wordsSearchExUnsafe2_FindAll | 5,184.6 ns | 59.70 ns | 49.85 ns |
//|     wordsSearchExUnsafe2_FindAll2 | 4,869.4 ns | 48.41 ns | 45.28 ns |
//|            stringSearchEx_Replace | 5,398.2 ns | 51.72 ns | 48.38 ns |
//|      stringSearchExUnsafe_Replace | 5,409.6 ns | 58.61 ns | 51.96 ns |
//|     stringSearchExUnsafe2_Replace | 5,967.5 ns | 61.37 ns | 57.41 ns |
//|        stringSearchEx_ContainsAny |   189.8 ns |  0.36 ns |  0.34 ns |
//|  stringSearchExUnsafe_ContainsAny |   176.0 ns |  0.62 ns |  0.58 ns |
//| stringSearchExUnsafe2_ContainsAny |   164.9 ns |  0.61 ns |  0.57 ns |
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
        public string stringSearchEx_FindFirst2() => stringSearchEx.FindFirst2(text);
        [Benchmark]
        public string stringSearchEx_FindFirst3() => stringSearchEx.FindFirst3(text);

        [Benchmark]
        public string stringSearchExUnsafe_FindFirst() => stringSearchExUnsafe.FindFirst(text);
        [Benchmark]
        public string stringSearchExUnsafe_FindFirst2() => stringSearchExUnsafe.FindFirst2(text);
        [Benchmark]
        public string stringSearchExUnsafe_FindFirst3() => stringSearchExUnsafe.FindFirst3(text);
        [Benchmark]
        public string stringSearchExUnsafe2_FindFirst() => stringSearchExUnsafe2.FindFirst(text);
        [Benchmark]
        public string stringSearchExUnsafe2_FindFirst2() => stringSearchExUnsafe2.FindFirst2(text);
        [Benchmark]
        public string stringSearchExUnsafe2_FindFirst3() => stringSearchExUnsafe2.FindFirst3(text);


        [Benchmark]
        public List<string> stringSearchEx_FindAll() => stringSearchEx.FindAll(text);
        [Benchmark]
        public List<string> stringSearchEx_FindAll2() => stringSearchEx.FindAll2(text);
        [Benchmark]
        public List<string> stringSearchEx_FindAll3() => stringSearchEx.FindAll3(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe_FindAll() => stringSearchExUnsafe.FindAll(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe_FindAll2() => stringSearchExUnsafe.FindAll2(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe_FindAll3() => stringSearchExUnsafe.FindAll3(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe2_FindAll() => stringSearchExUnsafe2.FindAll(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe2_FindAll2() => stringSearchExUnsafe2.FindAll2(text);
        [Benchmark]
        public List<string> stringSearchExUnsafe2_FindAll3() => stringSearchExUnsafe2.FindAll3(text);


        [Benchmark]
        public WordsSearchResult wordsSearchEx_FindFirst() => wordsSearchEx.FindFirst(text);
        [Benchmark]
        public WordsSearchResult wordsSearchEx_FindFirst2() => wordsSearchEx.FindFirst2(text);

        [Benchmark]
        public WordsSearchResult wordsSearchExUnsafe_FindFirst() => wordsSearchExUnsafe.FindFirst(text);
        [Benchmark]
        public WordsSearchResult wordsSearchExUnsafe_FindFirst2() => wordsSearchExUnsafe.FindFirst2(text);
        [Benchmark]
        public WordsSearchResult wordsSearchExUnsafe2_FindFirst() => wordsSearchExUnsafe2.FindFirst(text);
        [Benchmark]
        public WordsSearchResult wordsSearchExUnsafe2_FindFirst2() => wordsSearchExUnsafe2.FindFirst2(text);

        [Benchmark]
        public List<WordsSearchResult> wordsSearchEx_FindAll() => wordsSearchEx.FindAll(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchEx_FindAll2() => wordsSearchEx.FindAll2(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchExUnsafe_FindAll() => wordsSearchExUnsafe.FindAll(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchExUnsafe_FindAll2() => wordsSearchExUnsafe.FindAll2(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchExUnsafe2_FindAll() => wordsSearchExUnsafe2.FindAll(text);
        [Benchmark]
        public List<WordsSearchResult> wordsSearchExUnsafe2_FindAll2() => wordsSearchExUnsafe2.FindAll2(text);

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