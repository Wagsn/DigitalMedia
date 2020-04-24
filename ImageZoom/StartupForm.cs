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
    public partial class StartupForm : Form
    {
        public StartupForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var world = new WorldForm();
            world.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var border = new ZoomByBorderDragForm();
            border.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var wheel = new ZoomByMouseWheelForm();
            wheel.ShowDialog();
        }
    }
}
