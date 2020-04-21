using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseWheelPictureZoom
{
    // TODO 文字缩放和滚轮缩放
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            BindZoomEvent(panel1);
        }

        private void BindZoomEvent(Control control)
        {
            //窗体中控件的事件晚期绑定
            for (int i = 0; i < control.Controls.Count; i++)
            {
                control.Controls[i].MouseDown += new MouseEventHandler(MyMouseDown);
                control.Controls[i].MouseLeave += new EventHandler(MyMouseLeave);
                control.Controls[i].MouseMove += new MouseEventHandler(MyMouseMove);
            }
        }

        // 初始渲染
        private void Form1_Load(object sender, EventArgs e)
        {
            // 计算图片的实际坐标和缩放比
            var pBox = this.pictureBox1;

            // PictureBox的实际长宽和坐标
            var pbw = pBox.Width;
            var pbh = pBox.Height;
            var pbx = pBox.Location.X;
            var pby = pBox.Location.Y;

            // 计算图片的相对长宽
            var poh = pBox.Image.Width;
            var pow = pBox.Image.Height;

            // 计算图片的实际坐标和长宽
            PropertyInfo rectangleProperty = pBox.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
            Rectangle rectangle = (Rectangle)rectangleProperty.GetValue(pBox, null);
            // 图片的实际长宽
            int pw = rectangle.Width;
            int ph = rectangle.Height;
            // 计算左和上留白
            int black_left_width = (pw == pbw) ? 0 : (pbw - pw) / 2;
            int black_top_height = (ph == pbh) ? 0 : (pbh - ph) / 2;
            // 图片的实际坐标
            var px = pbx + black_left_width;
            var py = pby + black_top_height;

            // 计算伸缩比
            var scale = (double)ph / (double)poh;

            //=================================多个按钮循环计算============================

            // 计算按钮的原始长宽和坐标（相对图片{x=0,y=0,w=image.w,h=image.h}的像素坐标）
            //var pointArr = "[[0,0],[100,0],[100,100],[0,100]]"; // [[x1,y1],[x2,y2],[x3,y3],[x4,y4]]
            var pointList = pointArr;
            var box = pointList.Min(a => a.X);
            var boy = pointList.Min(a => a.Y);
            var bow = pointList.Max(a => a.X) - box;
            var boh = pointList.Max(a => a.Y) - boy;
            // 计算按钮的实际长宽和坐标(跟随相对图片、伸缩比和原始坐标乘以伸缩比)
            var bx = (int)(px + box * scale);
            var by = (int)(py + boy * scale);
            var bw = (int)(bow * scale);
            var bh = (int)(boh * scale);

            // 重绘按钮
            var button = button1;
            button.Location = new Point(bx, by);
            button.Size = new Size(bw, bh);

            //=============================================================================


            // 同步全局变量
            _px = px;
            _py = py;
            _scale = scale;

            // 重绘文本-PictureBox的实际长宽坐标 
            pbWBox.Text = pBox.Width.ToString();
            pbHBox.Text = pBox.Height.ToString();
            pbXBox.Text = pBox.Location.X.ToString();
            pbYBox.Text = pBox.Location.Y.ToString();

            // 重绘文本-图片的相对长宽
            pOWBox.Text = pow.ToString();
            pOHBox.Text = poh.ToString();
            // 重绘文本-图片的实际长宽
            pWBox.Text = pw.ToString();
            pHBox.Text = ph.ToString();
            pXBox.Text = px.ToString();
            pYBox.Text = py.ToString();

            // 重绘文本-按钮的实际长宽
            bWBox.Text = button1.Width.ToString();
            bHBox.Text = button1.Height.ToString();
            bXBox.Text = button1.Location.X.ToString();
            bYBox.Text = button1.Location.Y.ToString();
            // 重绘文本-按钮的原始长宽
            bOWBox.Text = bow.ToString();
            bOHBox.Text = boh.ToString();
            bOXBox.Text = box.ToString();
            bOYBox.Text = boy.ToString();

            // 伸缩比
            scaleBox.Text = scale.ToString();
        }

        /// <summary>
        ///  有关鼠标样式的相关枚举
        /// </summary>
        private enum EnumMousePointPosition
        {
            MouseSizeNone = 0, //默认
            MouseSizeRight = 1, //拉伸右边框
            MouseSizeLeft = 2,  //拉伸左边框
            MouseSizeBottom = 3, //拉伸下边框
            MouseSizeTop = 4, //拉伸上边框
            MouseSizeTopLeft = 5,//拉伸左上角
            MouseSizeTopRight = 6,//拉伸右上角
            MouseSizeBottomLeft = 7,//拉伸左下角
            MouseSizeBottomRight = 8,//拉伸右下角
            MouseDrag = 9 //鼠标拖动
        }
        const int Band = 5;//范围半径
        const int MinWidth = 10;//最低宽度
        const int MinHeight = 10;//最低高度
        private EnumMousePointPosition m_MousePointPosition; //鼠标样式枚举
        private Point m_lastPoint; //光标上次移动的位置
        private Point m_endPoint; //光标移动的当前位置
        //鼠标按下事件
        private void MyMouseDown(object sender, MouseEventArgs e)
        {
            m_lastPoint.X = e.X;
            m_lastPoint.Y = e.Y;
            m_endPoint.X = e.X;
            m_endPoint.Y = e.Y;
        }

        //鼠标离开控件的事件
        private void MyMouseLeave(object sender, System.EventArgs e)
        {
            m_MousePointPosition = EnumMousePointPosition.MouseSizeNone;
            this.Cursor = Cursors.Arrow;
        }
        // 按钮的原始坐标
        private List<Point> pointArr = new List<Point> { new Point { X = 100, Y = 100 }, new Point { X = 500, Y = 100 }, new Point { X = 500, Y = 500 }, new Point { X = 100, Y = 500 } };
        // 伸缩比
        private double _scale = 1.00;
        // 图片的实际坐标
        private int _px = 0;
        private int _py = 0;

        //鼠标移过控件的事件
        private void MyMouseMove(object sender, MouseEventArgs e)
        {
            Control lCtrl = (sender as Control);//获得事件源
            //左键按下移动
            if (e.Button == MouseButtons.Left)
            {
                switch (m_MousePointPosition)
                {
                    case EnumMousePointPosition.MouseDrag:
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Top = lCtrl.Top + e.Y - m_lastPoint.Y;
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点                      
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        lCtrl.Width = lCtrl.Width + e.X - m_endPoint.X;
                        lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点                       
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        lCtrl.Width = lCtrl.Width + e.X - m_endPoint.X;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        lCtrl.Top = lCtrl.Top + (e.Y - m_lastPoint.Y);
                        lCtrl.Height = lCtrl.Height - (e.Y - m_lastPoint.Y);
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Width = lCtrl.Width - (e.X - m_lastPoint.X);
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Width = lCtrl.Width - (e.X - m_lastPoint.X);
                        lCtrl.Height = lCtrl.Height + e.Y - m_endPoint.Y;
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        lCtrl.Top = lCtrl.Top + (e.Y - m_lastPoint.Y);
                        lCtrl.Width = lCtrl.Width + (e.X - m_endPoint.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - m_lastPoint.Y);
                        m_endPoint.X = e.X;
                        m_endPoint.Y = e.Y; //记录光标拖动的当前点                       
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        lCtrl.Left = lCtrl.Left + e.X - m_lastPoint.X;
                        lCtrl.Top = lCtrl.Top + (e.Y - m_lastPoint.Y);
                        lCtrl.Width = lCtrl.Width - (e.X - m_lastPoint.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - m_lastPoint.Y);
                        break;
                    default:
                        break;
                }
                if (lCtrl.Width < MinWidth) lCtrl.Width = MinWidth;
                if (lCtrl.Height < MinHeight) lCtrl.Height = MinHeight;

                if(lCtrl is PictureBox)
                {
                    var pBox = lCtrl as PictureBox;

                    // PictureBox的实际长宽和坐标
                    var pbw = pBox.Width;
                    var pbh = pBox.Height;
                    var pbx = pBox.Location.X;
                    var pby = pBox.Location.Y;

                    // 计算图片的相对长宽
                    var poh = pBox.Image.Width;
                    var pow = pBox.Image.Height;

                    // 计算图片的实际坐标和长宽
                    PropertyInfo rectangleProperty = pBox.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
                    Rectangle rectangle = (Rectangle)rectangleProperty.GetValue(pBox, null);
                    // 图片的实际长宽
                    int pw = rectangle.Width;
                    int ph = rectangle.Height;
                    // 计算左和上留白
                    int black_left_width = (pw == pbw) ? 0 : (pbw - pw) / 2;
                    int black_top_height = (ph == pbh) ? 0 : (pbh - ph) / 2;
                    // 图片的实际坐标
                    var px = pbx + black_left_width;
                    var py = pby + black_top_height;

                    // 计算伸缩比
                    var scale = (double)ph / (double)poh;


                    //=================================多个按钮循环计算============================

                    // 计算按钮的原始长宽和坐标（相对图片{x=0,y=0,w=image.w,h=image.h}的像素坐标）
                    //var pointArr = "[[0,0],[100,0],[100,100],[0,100]]"; // [[x1,y1],[x2,y2],[x3,y3],[x4,y4]]
                    var pointList = pointArr;
                    var box = pointList.Min(a => a.X);
                    var boy = pointList.Min(a => a.Y);
                    var bow = pointList.Max(a => a.X) - box;
                    var boh = pointList.Max(a => a.Y) - boy;
                    // 计算按钮的实际长宽和坐标(跟随相对图片、伸缩比和原始坐标乘以伸缩比)
                    var bx = (int)(px + box * scale);
                    var by = (int)(py + boy * scale);
                    var bw = (int)(bow * scale);
                    var bh = (int)(boh * scale);

                    // 重绘按钮
                    var button = button1;
                    button.Location = new Point(bx, by);
                    button.Size = new Size(bw, bh);

                    //=============================================================================

                    // 同步全局变量
                    _scale = scale;
                    _px = px;
                    _py = py;

                    // 重绘文本-PictureBox的实际长宽坐标 
                    pbWBox.Text = pBox.Width.ToString();
                    pbHBox.Text = pBox.Height.ToString();
                    pbXBox.Text = pBox.Location.X.ToString();
                    pbYBox.Text = pBox.Location.Y.ToString();

                    // 重绘文本-图片的相对长宽
                    pOWBox.Text = pow.ToString();
                    pOHBox.Text = poh.ToString();
                    // 重绘文本-图片的实际长宽
                    pWBox.Text = pw.ToString();
                    pHBox.Text = ph.ToString();
                    pXBox.Text = px.ToString();
                    pYBox.Text = py.ToString();

                    // 重绘文本-按钮的实际长宽
                    bWBox.Text = button1.Width.ToString();
                    bHBox.Text = button1.Height.ToString();
                    bXBox.Text = button1.Location.X.ToString();
                    bYBox.Text = button1.Location.Y.ToString();
                    // 重绘文本-按钮的原始长宽
                    bOWBox.Text = bow.ToString();
                    bOHBox.Text = boh.ToString();
                    bOXBox.Text = box.ToString();
                    bOYBox.Text = boy.ToString();

                    // 伸缩比
                    scaleBox.Text = scale.ToString();
                }
                if (lCtrl is Button)
                {
                    bWBox.Text = button1.Width.ToString();
                    bHBox.Text = button1.Height.ToString();
                    bXBox.Text = button1.Location.X.ToString();
                    bYBox.Text = button1.Location.Y.ToString();

                    var pBox = pictureBox1;

                    // 按钮的实际长宽和坐标
                    var bw = button1.Width;
                    var bh = button1.Height;
                    var bx = button1.Left;
                    var by = button1.Top;
                    // 按钮的相对长宽和坐标 通过图片的实际坐标和伸缩比
                    var px = _px;
                    var py = _py;
                    var scale = _scale;
                    var bow = (int)(bw / scale);
                    var boh = (int)(bh / scale);
                    var box = (int)(((double)bx - px) / scale);
                    var boy = (int)(((double)by - py) / scale);
                    pointArr = new List<Point> { new Point { X = box, Y = boy }, new Point { X = box+bow, Y = boy }, new Point { X = box + bow, Y = boy + boh }, new Point { X = box, Y = boy + boh } };

                    // 重绘文本-按钮的实际长宽
                    bWBox.Text = button1.Width.ToString();
                    bHBox.Text = button1.Height.ToString();
                    bXBox.Text = button1.Location.X.ToString();
                    bYBox.Text = button1.Location.Y.ToString();
                    // 重绘文本-按钮的原始长宽
                    bOWBox.Text = bow.ToString();
                    bOHBox.Text = boh.ToString();
                    bOXBox.Text = box.ToString();
                    bOYBox.Text = boy.ToString();
                }
            }
            else
            {
                //判断光标的位置状态 
                m_MousePointPosition = MousePointPosition(lCtrl.Size, e);
                switch (m_MousePointPosition)  //改变光标
                {
                    case EnumMousePointPosition.MouseSizeNone:
                        this.Cursor = Cursors.Arrow;//箭头
                        break;
                    case EnumMousePointPosition.MouseDrag:
                        this.Cursor = Cursors.SizeAll;//四方向
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        this.Cursor = Cursors.SizeNS;//南北
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        this.Cursor = Cursors.SizeNS;//南北
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        this.Cursor = Cursors.SizeWE;//东西
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        this.Cursor = Cursors.SizeWE;//东西
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        this.Cursor = Cursors.SizeNESW;//东北到南西
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        this.Cursor = Cursors.SizeNWSE;//东南到西北
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        this.Cursor = Cursors.SizeNWSE;//东南到西北
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        this.Cursor = Cursors.SizeNESW;//东北到南西
                        break;
                    default:
                        break;
                }
            }
        }
        //坐标位置判定
        private EnumMousePointPosition MousePointPosition(Size size, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.X >= -1 * Band) | (e.X <= size.Width) |
                (e.Y >= -1 * Band) | (e.Y <= size.Height))
            {
                if (e.X < Band)
                {
                    if (e.Y < Band)
                    {
                        return EnumMousePointPosition.MouseSizeTopLeft;
                    }
                    else
                    {
                        if (e.Y > -1 * Band + size.Height)
                        {
                            return EnumMousePointPosition.MouseSizeBottomLeft;
                        }
                        else
                        {
                            return EnumMousePointPosition.MouseSizeLeft;
                        }
                    }
                }
                else
                {
                    if (e.X > -1 * Band + size.Width)
                    {
                        if (e.Y < Band)
                        { return EnumMousePointPosition.MouseSizeTopRight; }
                        else
                        {
                            if (e.Y > -1 * Band + size.Height)
                            { return EnumMousePointPosition.MouseSizeBottomRight; }
                            else
                            { return EnumMousePointPosition.MouseSizeRight; }
                        }
                    }
                    else
                    {
                        if (e.Y < Band)
                        { return EnumMousePointPosition.MouseSizeTop; }
                        else
                        {
                            if (e.Y > -1 * Band + size.Height)
                            { return EnumMousePointPosition.MouseSizeBottom; }
                            else
                            { return EnumMousePointPosition.MouseDrag; }
                        }
                    }
                }
            }
            else
            { return EnumMousePointPosition.MouseSizeNone; }
        }
    }
}
