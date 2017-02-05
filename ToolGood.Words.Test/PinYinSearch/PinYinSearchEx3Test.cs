//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using PetaTest;
//using System.Diagnostics;

//namespace ToolGood.Words.Test
//{
//    [TestFixture]
//    class PinYinSearchExTest3
//    {
//        //[Test]
//        public void SetKeywords()
//        {
//            GC.Collect();
//            PinYinSearchEx3 search = new PinYinSearchEx3(PinYinSearchType.PinYin);
//            search.SetKeywords(ReadFiles());
//            search.SaveFile("keys.dat");
//        }

//        [Test]
//        public void Search()
//        {
//            GC.Collect();
//            PinYinSearchEx3 search = new PinYinSearchEx3(PinYinSearchType.PinYin);
//            search.LoadFile("keys.dat");

//            Stopwatch watch = new Stopwatch();
//            watch.Start();
//            for (int i = 0; i < 1000; i++) {
//                var ts = search.SearchTexts("程xy");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("程xuy");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("程xuyuan");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("程xyuan");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("cxy");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("chengxy");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("cxuy");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("cheng序y");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("c序y");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("c序yuan");
//                Assert.AreEqual(true, ts.Contains("程序员"));

//                ts = search.SearchTexts("cx员");
//                Assert.AreEqual(true, ts.Contains("程序员"));
//            }
//            watch.Stop();
//            Trace.Write(watch.ElapsedMilliseconds + "ms");
//        }



//        public static List<string> ReadFiles()
//        {
//            var files = Directory.GetFiles("_texts");
//            HashSet<string> texts = new HashSet<string>();

//            foreach (var file in files) {
//                var ts = File.ReadAllLines(file);
//                var c = ts[0][0];
//                if (c < 0x4e00 || c > 0x9fa5) {
//                    ts = File.ReadAllLines(file, Encoding.Default);
//                }

//                foreach (var t in ts) {
//                    texts.Add(t.Trim());
//                }
//            }
//            return texts.ToList();
//        }
//    }
//}
