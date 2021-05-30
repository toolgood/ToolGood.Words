using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                        this.listBox1.Items.Add(item);
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


    }
}
