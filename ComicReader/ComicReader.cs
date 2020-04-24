using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComicReader
{
    public partial class ComicReader : Form
    {
        public ComicReader()
        {
            InitializeComponent();

            // 绑定滚动事件
            this._zoomBox.Resize += ComicReader_Resize;
            this._zoomBox.MouseWheel += ComicReader_MouseWheel;
        }

        private void OpenFolder(string folder)
        {
            // 初始化
            _startYRadio = 0.00;
            _zoomBox.Controls.Clear();

            // 加载文件夹下的图片
            var files = Directory.GetFiles(folder);
            int lastY = 0;
            for (var i = 0; i < files.Length; i++)
            {
                var filename = files[i];
                // load image
                var image = Image.FromFile(filename);
                var iw = image.Width;
                var ih = image.Height;
                var whRadio = iw / (double)ih;
                var zw = _zoomBox.Width;
                var pw = zw;
                var ph = pw / whRadio;

                var pb = new PictureBox
                {
                    Location = new Point(0, lastY),
                    Size = new Size(pw, (int)Math.Round(ph)),
                    Image = image,
                    SizeMode = PictureBoxSizeMode.Zoom,
                };
                _zoomBox.Controls.Add(pb);
                lastY += pb.Height;
            }
        }

        private double _startYRadio = 0.00;

        /// <summary>
        /// 鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComicReader_MouseWheel(object sender, MouseEventArgs e)
        {
            // 计算滚动后的起始比例
            int totalH = GetTotalHight();
            var startY = (int)(totalH * _startYRadio);
            if (e.Delta > 0)  // 向上滚
            {
                startY += 60;
            }
            else
            {
                startY -= 60;
            }
            _startYRadio = startY / (double)totalH;

            // 重新绘制图像
            ResizePictureBox();
        }

        /// <summary>
        /// 窗口大小更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComicReader_Resize(object sender, EventArgs e)
        {
            ResizePictureBox();
        }

        /// <summary>
        /// 重绘图像
        /// </summary>
        private void ResizePictureBox()
        {
            int lastY = 0;
            int totalH = GetTotalHight();
            var startY = (int)(totalH * _startYRadio);
            foreach (PictureBox pictureBox1 in _zoomBox.Controls)
            {
                var image = pictureBox1.Image;
                var iw = image.Width;
                var ih = image.Height;
                var whRadio = iw / (double)ih;
                var zw = _zoomBox.Width;
                var pw = zw;
                var ph = pw / whRadio;
                pictureBox1.Location = new Point(0, lastY + startY);
                pictureBox1.Size = new Size(pw, (int)ph);
                lastY += pictureBox1.Height;
            }
        }

        /// <summary>
        /// 所有图像的显示总高度
        /// </summary>
        /// <returns></returns>
        private int GetTotalHight()
        {
            int totalH = 0;
            foreach (PictureBox pictureBox1 in _zoomBox.Controls)
            {
                totalH += pictureBox1.Height;
            }
            return totalH;
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fbd = folderBrowserDialog1;

            var dr = fbd.ShowDialog();
            if(dr == DialogResult.OK)
            {
                var folder = fbd.SelectedPath;
                OpenFolder(folder);
            }
        }
    }
}
