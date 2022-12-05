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
//Intel Core i5-8500 CPU 3.00GHz(Coffee Lake), 1 CPU, 6 logical and 6 physical cores
//.NET SDK= 7.0.100
//  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
//  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2

//|                            Method |       Mean |    Error |   StdDev |
//|---------------------------------- |-----------:|---------:|---------:|
//|          stringSearchEx_FindFirst |   205.6 ns |  1.78 ns |  1.66 ns |
//|    stringSearchExUnsafe_FindFirst |   192.0 ns |  1.30 ns |  1.22 ns |
//|   stringSearchExUnsafe2_FindFirst |   203.3 ns |  0.73 ns |  0.61 ns |
//|            stringSearchEx_FindAll | 5,410.8 ns | 90.17 ns | 84.34 ns |
//|      stringSearchExUnsafe_FindAll | 4,908.2 ns | 37.72 ns | 31.49 ns |
//|     stringSearchExUnsafe2_FindAll | 4,925.4 ns | 52.46 ns | 46.51 ns |
//|           wordsSearchEx_FindFirst |   207.4 ns |  1.59 ns |  1.41 ns |
//|     wordsSearchExUnsafe_FindFirst |   202.1 ns |  0.67 ns |  0.56 ns |
//|    wordsSearchExUnsafe2_FindFirst |   207.7 ns |  0.84 ns |  0.79 ns |
//|             wordsSearchEx_FindAll | 5,601.4 ns | 33.20 ns | 29.43 ns |
//|       wordsSearchExUnsafe_FindAll | 5,334.7 ns | 35.98 ns | 31.89 ns |
//|      wordsSearchExUnsafe2_FindAll | 5,833.3 ns | 24.62 ns | 21.83 ns |
//|            stringSearchEx_Replace | 5,186.5 ns | 30.58 ns | 27.11 ns |
//|      stringSearchExUnsafe_Replace | 5,440.4 ns | 31.11 ns | 27.58 ns |
//|     stringSearchExUnsafe2_Replace | 6,008.5 ns | 83.80 ns | 74.29 ns |
//|        stringSearchEx_ContainsAny |   191.4 ns |  1.16 ns |  1.02 ns |
//|  stringSearchExUnsafe_ContainsAny |   176.2 ns |  0.63 ns |  0.59 ns |
//| stringSearchExUnsafe2_ContainsAny |   165.9 ns |  0.57 ns |  0.51 ns |

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