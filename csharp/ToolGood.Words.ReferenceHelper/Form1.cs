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

        }

        /// <summary>
        /// 生成br资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 生成java资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 生成js资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 生成python资源包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {

        }

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
            PinyinBuild pinyinBuild = new PinyinBuild();
            pinyinBuild.Load(files);

            pinyinBuild.OutputSingleFile("out/allPinyin.txt");
            pinyinBuild = null;
        } 
        #endregion



    }
}
