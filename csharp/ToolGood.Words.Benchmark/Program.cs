using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
//|          stringSearchEx_FindFirst |   190.2 ns |  0.86 ns |  0.81 ns |
//|    stringSearchExUnsafe_FindFirst |   176.6 ns |  1.50 ns |  1.33 ns | 
//|   stringSearchExUnsafe2_FindFirst |   180.1 ns |  0.88 ns |  0.82 ns |
//|            stringSearchEx_FindAll | 1,657.2 ns | 22.94 ns | 21.46 ns |
//|      stringSearchExUnsafe_FindAll | 1,606.5 ns | 28.76 ns | 26.90 ns |
//|     stringSearchExUnsafe2_FindAll | 1,574.1 ns | 16.66 ns | 15.58 ns |
//|           wordsSearchEx_FindFirst |   209.1 ns |  1.38 ns |  1.29 ns |
//|     wordsSearchExUnsafe_FindFirst |   180.6 ns |  0.86 ns |  0.67 ns |
//|    wordsSearchExUnsafe2_FindFirst |   189.2 ns |  0.96 ns |  0.80 ns |
//|             wordsSearchEx_FindAll | 1,900.1 ns | 29.78 ns | 27.85 ns |
//|       wordsSearchExUnsafe_FindAll | 1,815.5 ns | 26.19 ns | 24.50 ns |
//|      wordsSearchExUnsafe2_FindAll | 1,918.0 ns | 37.30 ns | 44.40 ns |
//|            stringSearchEx_Replace | 1,553.9 ns | 12.44 ns | 11.03 ns |
//|      stringSearchExUnsafe_Replace | 1,687.1 ns | 33.72 ns | 38.83 ns |
//|     stringSearchExUnsafe2_Replace | 1,645.1 ns |  3.78 ns |  3.54 ns |
//|        stringSearchEx_ContainsAny |   163.6 ns |  0.74 ns |  0.66 ns |
//|  stringSearchExUnsafe_ContainsAny |   157.0 ns |  0.62 ns |  0.58 ns |
//| stringSearchExUnsafe2_ContainsAny |   145.6 ns |  1.07 ns |  0.95 ns |

//|                            Method |       Mean |    Error |   StdDev |
//|---------------------------------- |-----------:|---------:|---------:|
//|          stringSearchEx_FindFirst |   188.9 ns |  0.89 ns |  0.79 ns |
//|    stringSearchExUnsafe_FindFirst |   175.1 ns |  1.76 ns |  1.64 ns |
//|   stringSearchExUnsafe2_FindFirst |   179.6 ns |  1.05 ns |  0.98 ns |
//|            stringSearchEx_FindAll | 1,679.0 ns | 26.51 ns | 24.80 ns |
//|      stringSearchExUnsafe_FindAll | 1,582.6 ns | 19.33 ns | 17.13 ns |
//|     stringSearchExUnsafe2_FindAll | 1,582.4 ns | 24.36 ns | 22.79 ns |
//|           wordsSearchEx_FindFirst |   204.6 ns |  1.88 ns |  1.76 ns |
//|     wordsSearchExUnsafe_FindFirst |   183.9 ns |  1.29 ns |  1.21 ns |
//|    wordsSearchExUnsafe2_FindFirst |   178.0 ns |  1.91 ns |  1.79 ns |
//|             wordsSearchEx_FindAll | 1,831.8 ns | 12.07 ns | 11.29 ns |
//|       wordsSearchExUnsafe_FindAll | 1,808.5 ns | 26.85 ns | 25.12 ns |
//|      wordsSearchExUnsafe2_FindAll | 1,958.6 ns | 27.74 ns | 25.94 ns |
//|            stringSearchEx_Replace | 1,582.7 ns | 30.69 ns | 34.12 ns |
//|      stringSearchExUnsafe_Replace | 1,689.1 ns | 29.94 ns | 28.00 ns |
//|     stringSearchExUnsafe2_Replace | 1,650.6 ns |  6.17 ns |  5.78 ns |
//|        stringSearchEx_ContainsAny |   163.3 ns |  0.44 ns |  0.37 ns |
//|  stringSearchExUnsafe_ContainsAny |   156.9 ns |  0.59 ns |  0.55 ns |
//| stringSearchExUnsafe2_ContainsAny |   146.1 ns |  0.70 ns |  0.65 ns |

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