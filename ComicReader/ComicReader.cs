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
        }

        private void OpenFolder(string folder)
        {
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
                var zw = zoomBox.Width;
                var pw = zw;
                var ph = pw / whRadio;

                var pb = new PictureBox
                {
                    Location = new Point(0, lastY),
                    Size = new Size(pw, (int)Math.Round(ph)),
                    Image = image,
                    SizeMode = PictureBoxSizeMode.Zoom,
                };
                zoomBox.Controls.Add(pb);
                lastY += pb.Height;
            }
            _zoomW = zoomBox.Width;

            this.zoomBox.Resize += ComicReader_Resize;
            this.zoomBox.MouseWheel += ComicReader_MouseWheel;
        }

        private int _zoomW = 0;
        // Y轴起始点，不可靠，建议改为起始Y轴比(_startYRadio)（Reseize不会修改其值，只会在Wheel中修改）
        private int _startY = 0;
        private double _startYRadio = 0.00;

        /// <summary>
        /// 鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComicReader_MouseWheel(object sender, MouseEventArgs e)
        {
            int totalH = GetTotalHight();
            var startY = (int)(totalH * _startYRadio);
            if (e.Delta > 0)  // 向上滚
            {
                startY += 50;
            }
            else
            {
                startY -= 50;
            }
            _startYRadio = startY / (int)totalH;

            ResizePictureBox();
        }

        private void ComicReader_Resize(object sender, EventArgs e)
        {
            var zoomW = zoomBox.Width;
            _zoomW = zoomW;

            ResizePictureBox();
        }

        private void ResizePictureBox()
        {
            int lastY = 0;
            int totalH = GetTotalHight();
            var startY = (int)(totalH * _startYRadio);
            foreach (PictureBox pictureBox1 in zoomBox.Controls)
            {
                var image = pictureBox1.Image;
                var iw = image.Width;
                var ih = image.Height;
                var whRadio = iw / (double)ih;
                var zw = zoomBox.Width;
                var pw = zw;
                var ph = pw / whRadio;
                pictureBox1.Location = new Point(0, lastY + startY);
                pictureBox1.Size = new Size(pw, (int)ph);
                lastY += pictureBox1.Height;
            }
        }

        private int GetTotalHight()
        {
            int totalH = 0;
            foreach (PictureBox pictureBox1 in zoomBox.Controls)
            {
                totalH += pictureBox1.Height;
            }

            return totalH;
        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Path.GetPathRoot(AppContext.BaseDirectory);
            var dr = fbd.ShowDialog();
            if(dr == DialogResult.OK)
            {
                var folder = fbd.SelectedPath;
                OpenFolder(folder);
            }
        }
    }
}
