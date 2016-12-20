using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.Words;
using System.IO;

namespace ToolGood.PinYin.WordsBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            tt();

        }

        static void tt2()
        {
            var fs = File.OpenRead("1.txt");
            StreamReader sr = new StreamReader(fs);
            HashSet<string> list = new HashSet<string>();

            do {
                var text = sr.ReadLine();
                var sp = text.Split(' ');
                for (int i = 0; i < sp[0].Length; i++) {
                    var c = sp[0][i];
                    var all = WordHelper.GetAllPinYin(c);
                    var pys = sp[1].Split('|');
                    var py = pys[i].ToUpper();
                    if (py.Length > 0) {
                        py = py[0] + pys[i].Substring(1);
                    }

                    if (all.Count == 0 || all.Contains(py) == false) {
                        list.Add(c + "|" + py);
                    }
                }
            } while (sr.EndOfStream == false);

            File.WriteAllText("2.txt", string.Join("|", list));

        }

        static void tt()
        {
            var fs = File.OpenRead("words.txt");
            StreamReader sr = new StreamReader(fs);
            StringBuilder sb = new StringBuilder();

            do {
                var text = sr.ReadLine();
                var sp = text.Split(' ');
                var py = WordHelper.GetPinYin(sp[0]).ToLower();
                var py2 = sp[1].Replace("|", "");
                if (py != py2) {
                    sb.AppendFormat("{0} {1}\r\n", sp[0], sp[1]);
                }
            } while (sr.EndOfStream == false);

            File.WriteAllText("1.txt", sb.ToString());
        }

    }
}
