using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PetaTest;
using System.Diagnostics;

namespace ToolGood.Words.Test
{
    //[TestFixture]
    class PinYinSearchExTest
    {
        //[Test]
        public void SetKeywords()
        {
            GC.Collect();
            PinYinSearchEx search = new PinYinSearchEx(PinYinSearchType.PinYin);
            var ts = ReadFiles();
            var count = GetTextCount(ts);
            search.SetKeywords(ts);
            search.SaveFile("keys1.dat");
        }

        [Test]
        public void Search()
        {
            GC.Collect();
            PinYinSearchEx search = new PinYinSearchEx(PinYinSearchType.PinYin);
            search.LoadFile("keys1.dat");


            var ts = search.SearchTexts("程xy");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("程xuy");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("程xuyuan");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("程xyuan");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("cxy");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("chengxy");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("cxuy");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("cheng序y");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("c序y");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("c序yuan");
            Assert.AreEqual(true, ts.Contains("程序员"));

            ts = search.SearchTexts("cx员");
            Assert.AreEqual(true, ts.Contains("程序员"));

        }

        [Test]
        public void Search2()
        {
            GC.Collect();
            PinYinSearchEx search = new PinYinSearchEx(PinYinSearchType.PinYin);
            search.LoadFile("keys1.dat");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000; i++) {
                var ts = search.SearchTexts("程xy");
                ts = search.SearchTexts("程xuy");

                ts = search.SearchTexts("程xuyuan");

                ts = search.SearchTexts("程xyuan");

                ts = search.SearchTexts("cxy");

                ts = search.SearchTexts("chengxy");

                ts = search.SearchTexts("cxuy");

                ts = search.SearchTexts("cheng序y");

                ts = search.SearchTexts("c序y");

                ts = search.SearchTexts("c序yuan");

                ts = search.SearchTexts("cx员");
            }
            watch.Stop();
            Trace.Write(watch.ElapsedMilliseconds + "ms");
        }


        public int GetTextCount(List<string> ts)
        {
            var sum = 0;
            foreach (var t in ts) {
                sum += t.Length;
            }
            return sum;
        }

        public static List<string> ReadFiles()
        {
            var files = Directory.GetFiles("_texts");
            HashSet<string> texts = new HashSet<string>();

            foreach (var file in files) {
                var ts = File.ReadAllLines(file);
                var c = ts[0][0];
                if (c < 0x4e00 || c > 0x9fa5) {
                    ts = File.ReadAllLines(file, Encoding.Default);
                }

                foreach (var t in ts) {
                    texts.Add(t.Trim());
                }
            }
            return texts.ToList();
        }
    }
}
