using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Collections;
namespace Gozyy.Framework
{
    /// <summary>
    /// 验证码类
    /// </summary>
    public class Authcode
    {
        protected static int VCODE_LENGTH = 4;

        //产生图片 宽度：_WIDTH, 高度：_HEIGHT
        private static readonly int _WIDTH = 100, _HEIGHT = 40;//_WIDTH = 80, _HEIGHT = 35;
        //字体集
        //private static readonly string[] _FONT_FAMIly = { "Broadway", "Arial", "Arial Black", "Courier New", "Showcard Gothic", "Algerian", "Wide Latin", "Bernard MT Condensed", "Cooper Black" };
        private static readonly string[] _FONT_FAMIly = { "Broadway", "Arial", "Broadway", "Courier New", "Broadway", "Broadway", "Broadway", "Broadway", "Broadway" };

        //字体大小集
        private static readonly int[] _FONT_SIZE = { 18, 17, 18, 22 };
        //前景字体颜色集
        //private static readonly Color[] _COLOR_FACE = { Color.FromArgb(113, 153, 67), Color.FromArgb(30, 127, 182), Color.FromArgb(101, 44, 29), Color.FromArgb(150, 156, 0) };
        private static readonly Color[] _COLOR_FACE = { Color.FromArgb(30, 127, 182), Color.FromArgb(30, 127, 182), Color.FromArgb(30, 127, 182), Color.FromArgb(30, 127, 182) };

        //背景颜色集
        //private static readonly Color[] _COLOR_BACKGROUND = { Color.FromArgb(247, 254, 236), Color.FromArgb(234, 248, 255), Color.FromArgb(244, 250, 246), Color.FromArgb(248, 248, 248) };

        private static readonly Color[] _COLOR_BACKGROUND = { Color.FromArgb(234, 248, 255), Color.FromArgb(234, 248, 255), Color.FromArgb(234, 248, 255), Color.FromArgb(234, 248, 255) };
        //文本布局信息
        private static StringFormat _DL_FORMAT = new StringFormat(StringFormatFlags.NoClip);
        //左右旋转角度
        private static readonly int _ANGLE = 20;

        /// <summary>
        /// 验证码
        /// </summary>
        public Authcode()
        {

        }
        public void RenderImage(System.Web.HttpContext context)
        {
            string chkcode = GetCheckCode(VCODE_LENGTH);
            //context.Session["authcode"] = chkcode;
            Cookie.SetCookie("authcode", chkcode);
            CreateImage(chkcode,context);
        }
        private string GetCheckCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return GetCheckCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        private void CreateImage(string code, System.Web.HttpContext context)
        {
            _DL_FORMAT.Alignment = StringAlignment.Center;
            _DL_FORMAT.LineAlignment = StringAlignment.Center;

            long tick = DateTime.Now.Ticks;
            Random Rnd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            using (Bitmap _img = new Bitmap(_WIDTH, _HEIGHT))
            {
                using (Graphics g = Graphics.FromImage(_img))
                {
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    Point dot = new Point(20, 20);//18, 18

                    // 定义一个无干扰线区间和一个起始位置
                    int nor = Rnd.Next(53), rsta = Rnd.Next(130);
                    // 绘制干扰正弦曲线 M:曲线平折度, D:Y轴常量 V:X轴焦距
                    int M = Rnd.Next(15) + 5, D = Rnd.Next(20) + 15, V = Rnd.Next(5) + 1;

                    int ColorIndex = Rnd.Next(4);

                    float Px_x = 0.0F;
                    float Px_y = Convert.ToSingle(M * Math.Sin(V * Px_x * Math.PI / 180) + D);
                    //float Py_x, Py_y;

                    //填充背景
                    g.Clear(_COLOR_BACKGROUND[Rnd.Next(4)]);

                    //前景刷子 //背景刷子
                    using (Brush _BrushFace = new SolidBrush(_COLOR_FACE[ColorIndex]))
                    {
                        #region 绘制正弦线
                        //for (int i = 0; i < 131; i++)
                        //{
                        //    //初始化y点
                        //    Py_x = Px_x + 1;
                        //    Py_y = Convert.ToSingle(M * Math.Sin(V * Py_x * Math.PI / 180) + D);

                        //    //确定线条颜色
                        //    if (rsta >= i || i > (rsta + nor))
                        //        //初始化画笔
                        //        using (Pen _pen = new Pen(_BrushFace, 1.5f))
                        //        {
                        //            //绘制线条
                        //            g.DrawLine(_pen, Px_x, Px_y, Py_x, Py_y);
                        //        }

                        //    //交替x,y坐标点
                        //    Px_x = Py_x;
                        //    Px_y = Py_y;
                        //}
                        #endregion

                        //初始化光标的开始位置
                        g.TranslateTransform(-5, 2);

                        #region 绘制校验码字符串
                        for (int i = 0; i < code.Length; i++)
                        {
                            //随机旋转 角度
                            int angle = Rnd.Next(-_ANGLE, _ANGLE);
                            //移动光标到指定位置
                            g.TranslateTransform(dot.X, dot.Y);
                            //旋转
                            g.RotateTransform(angle);

                            //初始化字体
                            using (Font _font = new Font(_FONT_FAMIly[Rnd.Next(0, 8)], _FONT_SIZE[Rnd.Next(0, 3)]))
                            {
                                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, _WIDTH, _HEIGHT), _COLOR_FACE[ColorIndex], Color.Orange, 1.2f, true);

                                //绘制
                                g.DrawString(code[i].ToString(), _font, brush, 1, 1, _DL_FORMAT);
                            }
                            //反转
                            g.RotateTransform(-angle);
                            //重新定位光标位置
                            g.TranslateTransform(1, -dot.Y);
                        }
                        #endregion

                    }
                }

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    context.Response.ContentType = "Image/PNG";
                    context.Response.Clear();
                    context.Response.BufferOutput = true;
                    _img.Save(ms, ImageFormat.Png);
                    ms.Flush();
                    context.Response.BinaryWrite(ms.GetBuffer());
                    context.Response.End();
                }
            }
        }
    }

}