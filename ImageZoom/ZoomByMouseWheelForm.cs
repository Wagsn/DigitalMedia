using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseWheelPictureZoom
{
    public partial class ZoomByMouseWheelForm : Form
    {
        public ZoomByMouseWheelForm()
        {
            InitializeComponent();

            BindMosuseWhellZoomEvent(this.pictureBox1);
        }

        /// <summary>
        /// 为控件绑定鼠标滚轮缩放事件
        /// </summary>
        /// <param name="control"></param>
        private void BindMosuseWhellZoomEvent(Control control)
        {
            control.MouseWheel += pictureBox1_MouseWheel;
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)//鼠标滚轮事件
        {
            var control = sender as Control;
            double step = 1.2;//缩放倍率
            if (e.Delta > 0)
            {
                var w = (int)(pictureBox1.Width * step);
                var h = (int)(pictureBox1.Height * step);
                if (w > 10000 || h > 10000 || w < 100 || h < 100) return;
                control.Width = w;
                control.Height = h;
                int px_add = (int)(e.X * (step - 1.0));
                int py_add = (int)(e.Y * (step - 1.0));
                control.Location = new Point(control.Location.X - px_add, control.Location.Y - py_add);
                Application.DoEvents();
            }
            else
            {
                var w = (int)(control.Width / step);
                var h = (int)(control.Height / step);
                if (w > 10000 || h > 10000 || w < 100 || h < 100) return;
                control.Width = w;
                control.Height = h;

                int px_add = (int)(e.X * (1.0 - 1.0 / step));
                int py_add = (int)(e.Y * (1.0 - 1.0 / step));
                control.Location = new Point(control.Location.X + px_add, control.Location.Y + py_add);
                Application.DoEvents();
            }
        }
    }
}
