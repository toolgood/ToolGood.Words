using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.Words.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var r = WordsHelper.GetPinyin("我爱中国");
            stopwatch.Stop();
            var s = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("拼音第一次加载用时（ms）："+s);

            PetaTest.Runner.RunMain(args);
        }
    }
}
