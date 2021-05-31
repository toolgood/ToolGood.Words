using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using ToolGood.PinYin.Build.Pinyin;

namespace ToolGood.Words.ReferenceHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 拼音
        /// <summary>
        /// 生成zip资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }

            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.CreateZip("out/gzip", this.checkBox1.Checked);
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/gzip 目录内。");
        }

        /// <summary>
        /// 生成br资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.CreateBr("out/br", this.checkBox1.Checked);
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/br 目录内。");
        }

        /// <summary>
        /// 生成java资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);


            pinyinBuild.CreateJava("out/Java", this.checkBox1.Checked);
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/Java 目录内。");
        }

        /// <summary>
        /// 生成js资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.CreateJs("out/js", this.checkBox1.Checked);
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/js 目录内。");
        }

        /// <summary>
        /// 生成python资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.CreatePython("out/python", this.checkBox1.Checked);
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/python 目录内。");
        }

        #region DragDrop
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            List<string> list = new List<string>();
            var files = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in files) {
                var file = item.ToString();
                if (file.ToLower().EndsWith(".js")
                    || file.ToLower().EndsWith(".txt")
                    ) {
                    if (list.Contains(item) == false) {
                        ((ListBox)sender).Items.Add(item);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }

            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.OutputSingleFile("out/allPinyin.txt");
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/allPinyin.txt 文件内。");
        }
        /// <summary>
        /// 拼音查错
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }

            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.CheckPinyin("out/rightPinyin.txt", "out/errorPinyin.txt");
            pinyinBuild = null;
            MessageBox.Show("拼音查错完成！已保存在 out 目录内。");
            // out/rightPinyin.txt
            // out/errorPinyin.txt
        }
        /// <summary>
        /// 清空列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
        }

        /// <summary>
        /// 生成拼音序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            foreach (var item in this.listBox1.Items) {
                files.Add(item.ToString());
            }
            if (files.Count == 0) {
                MessageBox.Show("请拖入文件！");
                return;
            }
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.CreatePyShow("out/Pinyin.txt", this.checkBox1.Checked);
            pinyinBuild = null;
            MessageBox.Show("完成！已保存在 out/Pinyin.txt 文件内。");

        }


        #endregion

        #region 生产拼音
        private void button20_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                this.textBox1.Text = openFileDialog.FileName;
            }
            openFileDialog = null;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                this.textBox2.Text = openFileDialog.FileName;
            }
            openFileDialog = null;
        }
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in files) {
                var file = item.ToString();
                if (file.ToLower().EndsWith(".txt")
                    ) {
                    ((TextBox)sender).Text = file;
                }
            }
        }

        /// <summary>
        /// 生成gzip资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            var pinyinIndex = this.textBox1.Text;
            if (File.Exists(pinyinIndex) == false) { MessageBox.Show("拼音序列文件不存在！");return; }
            var pinyinName = this.textBox2.Text;
            if (File.Exists(pinyinName) == false) { MessageBox.Show("姓氏拼音文件不存在！"); return; }

            PinyinNameBuild build = new PinyinNameBuild();
            build.CreateZip(pinyinIndex, pinyinName, "out/gzip/pyName.txt.z");
            build = null;
            MessageBox.Show("完成！已保存在 out/gzip/pyName.txt.z 文件内。");

        }
        /// <summary>
        /// 生成br资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            var pinyinIndex = this.textBox1.Text;
            if (File.Exists(pinyinIndex) == false) { MessageBox.Show("拼音序列文件不存在！"); return; }
            var pinyinName = this.textBox2.Text;
            if (File.Exists(pinyinName) == false) { MessageBox.Show("姓氏拼音文件不存在！"); return; }

            PinyinNameBuild build = new PinyinNameBuild();
            build.CreateBr(pinyinIndex, pinyinName, "out/br/pyName.txt.br");
            build = null;
            MessageBox.Show("完成！已保存在 out/gzip/pyName.txt.br 文件内。");
        }
        /// <summary>
        /// 生成java资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            var pinyinIndex = this.textBox1.Text;
            if (File.Exists(pinyinIndex) == false) { MessageBox.Show("拼音序列文件不存在！"); return; }
            var pinyinName = this.textBox2.Text;
            if (File.Exists(pinyinName) == false) { MessageBox.Show("姓氏拼音文件不存在！"); return; }

            PinyinNameBuild build = new PinyinNameBuild();
            build.CreateJava(pinyinIndex, pinyinName, "out/java/pyName.txt");
            build = null;
            MessageBox.Show("完成！已保存在 out/java/pyName.txt 文件内。");
        }
        /// <summary>
        /// 生成js资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 生成python资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 繁简转化

        public String GetDefaultWebBrowserFilePath()
        {
            string _BrowserKey1 = @"Software\Clients\StartmenuInternet\";
            string _BrowserKey2 = @"shell\open\command";
            string outPath;

            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem) {
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            } else {
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }

            RegistryKey _RegistryKey = localKey.OpenSubKey(_BrowserKey1, false);
            var names = _RegistryKey.GetSubKeyNames();
            if (names.Contains("Google Chrome")) {
                var key = _RegistryKey.OpenSubKey("Google Chrome").OpenSubKey(_BrowserKey2);
                outPath = key.GetValue("").ToString();
            } else if (names.Any(q => q.StartsWith("Firefox"))) {
                var name = names.Where(q => q.StartsWith("Firefox")).FirstOrDefault();
                var key = _RegistryKey.OpenSubKey(name).OpenSubKey(_BrowserKey2);
                outPath = key.GetValue("").ToString();
            } else {
                String name = _RegistryKey.GetValue("").ToString();
                var key = _RegistryKey.OpenSubKey(name).OpenSubKey(_BrowserKey2);
                outPath = key.GetValue("").ToString();
            }
            _RegistryKey.Close();

            if (outPath.Contains("\"")) {
                outPath = outPath.TrimStart('"');
                outPath = outPath.Substring(0, outPath.IndexOf('"'));
            }
            return outPath;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var exePath = GetDefaultWebBrowserFilePath();
            System.Diagnostics.Process.Start(exePath, linkLabel1.Text);
        }
        private void button22_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                this.textBox3.Text = openFileDialog.FileName;
            }
            openFileDialog = null;
        }

        private void textBox3_DragDrop(object sender, DragEventArgs e)
        {
            var files = (System.Array)e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in files) {
                var file = item.ToString();
                if (file.ToLower().EndsWith(".txt")
                    ) {
                    this.textBox3.Text = file;
                }
            }
        }

        /// <summary>
        /// 生成gzip资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("dict", "TS*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TS文件");
                return;
            }
            files = Directory.GetFiles("dict", "ST*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少ST文件");
                return;
            }
            files = Directory.GetFiles("dict", "HK*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少HK文件");
                return;
            }
            files = Directory.GetFiles("dict", "TW*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TW文件");
                return;
            }
            TransformationBuild build = new TransformationBuild();
            build.CreateZip("out/gzip", this.textBox3.Text);
            build = null;
            MessageBox.Show("完成！已保存在 out/gzip 目录内。");

        }
        /// <summary>
        /// 生成br资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("dict", "TS*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TS文件");
                return;
            }
            files = Directory.GetFiles("dict", "ST*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少ST文件");
                return;
            }
            files = Directory.GetFiles("dict", "HK*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少HK文件");
                return;
            }
            files = Directory.GetFiles("dict", "TW*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TW文件");
                return;
            }
            TransformationBuild build = new TransformationBuild();
            build.CreateBr("out/br", this.textBox3.Text);
            build = null;
            MessageBox.Show("完成！已保存在 out/br 目录内。");
        }
        /// <summary>
        /// 生成java资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("dict", "TS*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TS文件");
                return;
            }
            files = Directory.GetFiles("dict", "ST*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少ST文件");
                return;
            }
            files = Directory.GetFiles("dict", "HK*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少HK文件");
                return;
            }
            files = Directory.GetFiles("dict", "TW*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TW文件");
                return;
            }
            TransformationBuild build = new TransformationBuild();
            build.CreateJava("out/java", this.textBox3.Text);
            build = null;
            MessageBox.Show("完成！已保存在 out/java 目录内。");
        }
        /// <summary>
        /// 生成js资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("dict", "TS*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TS文件");
                return;
            }
            files = Directory.GetFiles("dict", "ST*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少ST文件");
                return;
            }
            files = Directory.GetFiles("dict", "HK*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少HK文件");
                return;
            }
            files = Directory.GetFiles("dict", "TW*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TW文件");
                return;
            }
            TransformationBuild build = new TransformationBuild();
            build.CreateJs("out/js", this.textBox3.Text);
            build = null;
            MessageBox.Show("完成！已保存在 out/js 目录内。");
        }
        /// <summary>
        /// 生成python资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("dict", "TS*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TS文件");
                return;
            }
            files = Directory.GetFiles("dict", "ST*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少ST文件");
                return;
            }
            files = Directory.GetFiles("dict", "HK*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少HK文件");
                return;
            }
            files = Directory.GetFiles("dict", "TW*.txt");
            if (files.Length == 0) {
                MessageBox.Show("缺少TW文件");
                return;
            }
            TransformationBuild build = new TransformationBuild();
            build.CreatePython("out/python", this.textBox3.Text);
            build = null;
            MessageBox.Show("完成！已保存在 out/python 目录内。");
        }
        #endregion

        #region 0x3400-0x9fd5区间拼音
        private void button23_Click(object sender, EventArgs e)
        {
            var py = this.textBox4.Text;
            var pyName = this.textBox5.Text;
            if (File.Exists(py)==false) {
                MessageBox.Show("拼音库文件不存在");
                return;
            }
            if (File.Exists(pyName) == false) {
                MessageBox.Show("姓氏拼音文件不存在");
                return;
            }
            PinyinDictBuild build = new PinyinDictBuild();
            build.InitPyFile(py, pyName);
            build.WriteGzip("out/mini/Pinyin.dat.z");
            build = null;
            MessageBox.Show("完成！已保存在 out/mini/Pinyin.dat.z 文件内。");
        }
        private void button24_Click(object sender, EventArgs e)
        {
            var py = this.textBox4.Text;
            var pyName = this.textBox5.Text;
            if (File.Exists(py) == false) {
                MessageBox.Show("拼音库文件不存在");
                return;
            }
            if (File.Exists(pyName) == false) {
                MessageBox.Show("姓氏拼音文件不存在");
                return;
            }
            PinyinDictBuild build = new PinyinDictBuild();
            build.InitPyFile(py, pyName);
            build.WriteBr("out/mini/Pinyin.dat.br");
            build = null;
            MessageBox.Show("完成！已保存在 out/mini/Pinyin.dat.br 文件内。");
        }
        #endregion

        #region 0x3400-0x9fd5区间拼音首字母
        private void button25_Click(object sender, EventArgs e)
        {
            var py = this.textBox4.Text;
            var pyName = this.textBox5.Text;
            if (File.Exists(py) == false) {
                MessageBox.Show("拼音库文件不存在");
                return;
            }
            if (File.Exists(pyName) == false) {
                MessageBox.Show("姓氏拼音文件不存在");
                return;
            }


        }

        private void button26_Click(object sender, EventArgs e)
        {
            var py = this.textBox4.Text;
            var pyName = this.textBox5.Text;
            if (File.Exists(py) == false) {
                MessageBox.Show("拼音库文件不存在");
                return;
            }
            if (File.Exists(pyName) == false) {
                MessageBox.Show("姓氏拼音文件不存在");
                return;
            }


        }
        #endregion

        #region 浏览
        private void button27_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                this.textBox4.Text = openFileDialog.FileName;
            }
            openFileDialog = null;
        }

        private void button28_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                this.textBox5.Text = openFileDialog.FileName;
            }
            openFileDialog = null;
        }

        #endregion

   
    }
}
