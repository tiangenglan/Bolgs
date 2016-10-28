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
    public class Captchas
    {
        public Color BackGroundColor { get; set; }
        public Color RandomTextColor { get; set; }
        public string RandomWord { get; set; }

        public Captchas()
        {
            BackGroundColor = Color.LightGray;
            RandomTextColor = Color.CadetBlue;
        }
        public void RenderImage(System.Web.HttpContext context)
        {
            RandomWord = SelectRandomWord(5);
            //context.Session["authcode"] = RandomWord;
            Cookie.SetCookie("authcode", RandomWord);
            Bitmap objBMP = new System.Drawing.Bitmap(80, 26);
            Graphics objGraphics = System.Drawing.Graphics.FromImage(objBMP);

            objGraphics.Clear(BackGroundColor);


            // Instantiate object of brush with black color
            SolidBrush objBrush = new SolidBrush(RandomTextColor);

            Font objFont = null;
            int a;
            String myFont, str;

            //Creating an array for most readable yet cryptic fonts for OCR's
            // This is entirely up to developer's discretion
            String[] crypticFonts = new String[11];

            crypticFonts[0] = "Arial";
            crypticFonts[1] = "Verdana";
            crypticFonts[2] = "微软雅黑";
            crypticFonts[3] = "Impact";
            crypticFonts[4] = "Haettenschweiler";
            crypticFonts[5] = "Lucida Sans Unicode";
            crypticFonts[6] = "Garamond";
            crypticFonts[7] = "Courier New";
            crypticFonts[8] = "Book Antiqua";
            crypticFonts[9] = "Arial Narrow";
            crypticFonts[10] = "Estrangelo Edessa";

            //Loop to write the characters on image  
            // with different fonts.
            for (a = 0; a <= RandomWord.Length - 1; a++)
            {
                myFont = crypticFonts[new Random().Next(a)];
                objFont = new Font(myFont, 12, FontStyle.Bold | FontStyle.Italic);//| FontStyle.Strikeout);
                str = RandomWord.Substring(a, 1);
                objGraphics.DrawString(str, objFont, objBrush, a * 13, 5);
                objGraphics.Flush();
            }
            var ms = new MemoryStream();
            objBMP.Save(ms, ImageFormat.Jpeg);
            context.Response.ClearContent();//.

            //清除该页输出缓存，设置该页无缓存 
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = System.DateTime.Now.AddMilliseconds(0);
            context.Response.Expires = 0;
            context.Response.CacheControl = "no-cache";
            context.Response.AppendHeader("Pragma", "No-Cache");

            context.Response.ContentType = "image/jpeg";
            //objBMP.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            context.Response.BinaryWrite(ms.ToArray());



            objFont.Dispose();
            objGraphics.Dispose();
            objBMP.Dispose();


            context.Response.End();
        }
        private string SelectRandomWord(int numberOfChars)
        {
            if (numberOfChars > 36)
            {
                numberOfChars = 36;
            }
            // Creating an array of 26 characters  and 0-9 numbers
            char[] columns = new char[36];

            for (int charPos = 65; charPos < 65 + 26; charPos++)
                columns[charPos - 65] = (char)charPos;

            for (int intPos = 48; intPos <= 57; intPos++)
                columns[26 + (intPos - 48)] = (char)intPos;

            StringBuilder randomBuilder = new StringBuilder();
            // pick charecters from random position
            Random randomSeed = new Random();
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                randomBuilder.Append(columns[randomSeed.Next(36)].ToString());

            }

            return randomBuilder.ToString();
        }
        //--------------------第二种方式
        public void RenderImage2(System.Web.HttpContext context)
        {
            string chkCode = string.Empty;
            //颜色列表，用于验证码、噪线、噪点 
            //Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            Color[] color = { Color.Gray, Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown };
            //字体列表，用于验证码 
            string[] font = { "Arial", "Mongolian Baiti", "Times New Roman", "Tahoma", "Mangal", "Mongolian Baiti", "Courier", "PMingLiU", "Impact" };
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            //保存验证码
            //context.Session["authcode"] = chkCode;
            Cookie.SetCookie("authcode", chkCode);

            Bitmap bmp = new Bitmap(100, 40);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 5; i++)
            {
                int x1 = rnd.Next(100);
                int y1 = rnd.Next(40);
                int x2 = rnd.Next(100);
                int y2 = rnd.Next(40);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, 18);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 20 + 8, (float)8);
            }
            //画噪点 
            for (int i = 0; i < 150; i++)
            {
                int x = rnd.Next(bmp.Width);
                int y = rnd.Next(bmp.Height);
                Color clr = color[rnd.Next(color.Length)];
                bmp.SetPixel(x, y, clr);
            }
            //清除该页输出缓存，设置该页无缓存 
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = System.DateTime.Now.AddMilliseconds(0);
            context.Response.Expires = 0;
            context.Response.CacheControl = "no-cache";
            context.Response.AppendHeader("Pragma", "No-Cache");
            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                context.Response.ClearContent();
                context.Response.ContentType = "image/Png";
                context.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                //显式释放资源 
                bmp.Dispose();
                g.Dispose();
            }
        }
        //第三种方法
        public void RenderImage3(System.Web.HttpContext context)
        {
            //Bitmap oBitmap = new Bitmap(90, 35);
            Bitmap oBitmap = new Bitmap(60, 25);
            Graphics oGraphic = Graphics.FromImage(oBitmap);
            System.Drawing.Color foreColor = default(System.Drawing.Color);
            System.Drawing.Color backColor = default(System.Drawing.Color);

            string sText = generateVCode(4);
            //保存验证码
            //context.Session["authcode"] = sText;
            Cookie.SetCookie("authcode", sText);
            //获取校验码字符串
            string sFont = "Verdana";
            //设置自己喜欢的字体

            //前景、背景的颜色
            foreColor = Color.FromArgb(220, 220, 220);
            backColor = Color.FromArgb(190, 190, 190);

            //设置用于背景的画笔
            HatchBrush oBrush = new HatchBrush((HatchStyle)generateHatchStyle(), foreColor, backColor);
            //用于输出校验码的画笔
            SolidBrush oBrushWrite = new SolidBrush(Color.Gray);

            //生成的图像矩形大小
            oGraphic.FillRectangle(oBrush, 0, 0, 100, 50);
            oGraphic.TextRenderingHint = TextRenderingHint.AntiAlias;

            //Font oFont = new Font(sFont, 16);
            //PointF oPoint = new PointF(8f, 5f);
            Font oFont = new Font(sFont, 14);//16);
            PointF oPoint = new PointF(4f, 2.5f);//(8f, 5f);

            oGraphic.DrawString(sText, oFont, oBrushWrite, oPoint);

            //清除该页输出缓存，设置该页无缓存 
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = System.DateTime.Now.AddMilliseconds(0);
            context.Response.Expires = 0;
            context.Response.CacheControl = "no-cache";
            context.Response.AppendHeader("Pragma", "No-Cache");

            context.Response.ContentType = "image/jpeg";
            oBitmap.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            oBitmap.Dispose();
        }
        public string generateVCode(int leng)
        {
            string strLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var VCode = String.Empty;
            var randObj = new Random();

            var c = 63;
            for (int i = 0; i < leng; i++)
            {
                c = randObj.Next(0, strLetters.Length - 1);// (int)Math.Round(randObj);

                VCode += strLetters.Substring(c, 1);
            }
            return VCode;

        }
        public HatchStyle generateHatchStyle()
        {
            ArrayList slist = new ArrayList();
            foreach (var style in Enum.GetValues(typeof(System.Drawing.Drawing2D.HatchStyle)))
            {
                slist.Add(style);
            }
            var r = new Random();
            var index = r.Next(slist.Count - 1);
            return (HatchStyle)slist[index];

        }
        /// <summary>
        /// 算式验证码
        /// </summary>
        public void RenderImage4(System.Web.HttpContext context)
        {
            int nRet = 0;
            string checkCode = String.Empty;
            System.Random random = new Random();
            string strMark = string.Empty;
            int nNum1 = random.Next(9);//30
            int nNum2 = random.Next(9);//20
            int nNum3 = random.Next(9);//12
            int nNum4 = random.Next(10);
            int nMark = random.Next(10);
            if (nMark % 2 == 0)
            {
                strMark = "+";
            }
            else
            {
                strMark = "×";
            }
            if (strMark == "+")
            {
                nRet = nNum1 + nNum2;
                checkCode = nNum1.ToString() + strMark + nNum2.ToString() + "=";
            }
            else
            {
                nRet = nNum3 * nNum4;
                checkCode = nNum3.ToString() + strMark + nNum4.ToString() + "=";
            }
            //context.Session["authcode"] = nRet;
            Cookie.SetCookie("authcode", nRet.ToString());
            CreateImages(checkCode);
        }
        /// <summary>
        /// 算式验证码
        /// </summary>
        public byte[] RenderImageMvc(System.Web.HttpContext context)
        {
            int nRet = 0;
            string checkCode = String.Empty;
            System.Random random = new Random();
            string strMark = string.Empty;
            int nNum1 = random.Next(9);//30
            int nNum2 = random.Next(9);//20
            int nNum3 = random.Next(9);//12
            int nNum4 = random.Next(10);
            int nMark = random.Next(10);
            if (nMark % 2 == 0)
            {
                strMark = "+";
            }
            else
            {
                strMark = "×";
            }
            if (strMark == "+")
            {
                nRet = nNum1 + nNum2;
                checkCode = nNum1.ToString() + strMark + nNum2.ToString() + "=";
            }
            else
            {
                nRet = nNum3 * nNum4;
                checkCode = nNum3.ToString() + strMark + nNum4.ToString() + "=";
            }
            //context.Session["authcode"] = nRet;
            Cookie.SetCookie("authcode", nRet.ToString());
            return GetByteFormImages(checkCode);
        }
        /// <summary>
        ///  生成验证图片
        /// </summary>
        /// <param name="checkCode">验证字符</param>
        protected void CreateImages(string checkCode)
        {
            int iwidth = (int)(checkCode.Length * 15);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 30);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Red, Color.Blue, Color.Orange, Color.Green, Color.Black, Color.DarkBlue, Color.Red, Color.YellowGreen, Color.Red };
            //定义字体 
            string[] font = { "Arial", "Microsoft Sans Serif", "Comic Sans MS", "Verdana", "Candara", "Comic Sans MS" };
            Random rand = new Random();
            //随机输出噪点
            for (int i = 0; i < 1; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                g.DrawPie(new Pen(Color.Azure, 9), x, y, 6, 6, 1, 1);
            }

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(6);
                Font _font = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                int ii = 4;
                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }
                g.DrawString(checkCode.Substring(i, 1), _font, b, 3 + (i * 12), ii);
            }

            //画一个边框
            g.DrawRectangle(new Pen(Color.Red, 0), 100, 0, image.Width - 1, image.Height - 1);
            //输出到浏览器
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/jpeg";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            g.Dispose();
            image.Dispose();
        }
        /// <summary>
        ///  返回验证图片
        /// </summary>
        /// <param name="checkCode">验证字符</param>
        protected byte[] GetByteFormImages(string checkCode)
        {
            int iwidth = (int)(checkCode.Length * 15);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 30);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Red, Color.Blue, Color.Orange, Color.Green, Color.Black, Color.DarkBlue, Color.Red, Color.YellowGreen, Color.Red };
            //定义字体 
            string[] font = { "Arial", "Microsoft Sans Serif", "Comic Sans MS", "Verdana", "Candara", "Comic Sans MS" };
            Random rand = new Random();
            //随机输出噪点
            for (int i = 0; i < 1; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                g.DrawPie(new Pen(Color.Azure, 9), x, y, 6, 6, 1, 1);
            }

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(6);
                Font _font = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                int ii = 4;
                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }
                g.DrawString(checkCode.Substring(i, 1), _font, b, 3 + (i * 12), ii);
            }

            //画一个边框
            g.DrawRectangle(new Pen(Color.Red, 0), 100, 0, image.Width - 1, image.Height - 1);
            //输出到浏览器
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            image.Dispose();
            return ms.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public byte[] RenderImageMvc2(System.Web.HttpContext context)
        {
            string checkCode = generateVCode2(4);
            //context.Session["authcode"] = checkCode;
            Cookie.SetCookie("authcode", checkCode);
            return GetByteFormImages2(checkCode);
        }
        public byte[] RenderImageMvc2(System.Web.HttpContext context, string checkCode)
        {
            //context.Session["authcode"] = checkCode;
            Cookie.SetCookie("authcode", checkCode);
            return GetByteFormImages2(checkCode);
        }
        protected byte[] GetByteFormImages2(string checkCode)
        {
            int iwidth = (int)(checkCode.Length * 15);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 30);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Red, Color.Blue, Color.Orange, Color.Green, Color.Black, Color.DarkBlue, Color.Red, Color.YellowGreen, Color.Red };
            //定义字体 
            string[] font = { "Arial", "Mongolian Baiti", "Times New Roman", "Tahoma", "Mangal", "Mongolian Baiti", "Courier" };
            Random rand = new Random();
            //随机输出噪点
            for (int i = 0; i < 1; i++)
            {
                int x = rand.Next(image.Width - 5);
                int y = rand.Next(image.Height - 5);
                g.DrawPie(new Pen(Color.Azure, 9), x, y, 6, 6, 1, 1);
            }

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(6);
                Font _font = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                int ii = 4;
                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }
                g.DrawString(checkCode.Substring(i, 1), _font, b, 3 + (i * 12), ii);
            }

            //画一个边框
            g.DrawRectangle(new Pen(Color.Red, 0), 100, 0, image.Width - 1, image.Height - 1);
            //输出到浏览器
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            image.Dispose();
            return ms.ToArray();
        }
        public void RenderImage5(System.Web.HttpContext context)
        {
            string checkCode = generateVCode2(4);
            //context.Session["authcode"] = checkCode;
            Cookie.SetCookie("authcode", checkCode);
            CreateImages2(checkCode);
        }
        protected void CreateImages2(string checkCode)
        {
            int iwidth = (int)(checkCode.Length * 15);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 30);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Red, Color.Blue, Color.Orange, Color.Green, Color.Black, Color.DarkBlue, Color.Red, Color.YellowGreen, Color.Red };
            //定义字体 
            string[] font = { "Arial", "Mongolian Baiti", "Times New Roman", "Tahoma", "Mangal", "Mongolian Baiti", "Courier" };
            Random rand = new Random();
            //随机输出噪点
            for (int i = 0; i < 1; i++)
            {
                int x = rand.Next(image.Width - 5);
                int y = rand.Next(image.Height - 5);
                g.DrawPie(new Pen(Color.Azure, 9), x, y, 6, 6, 1, 1);
            }

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(6);
                Font _font = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                int ii = 4;
                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }
                g.DrawString(checkCode.Substring(i, 1), _font, b, 3 + (i * 12), ii);
            }

            //画一个边框
            g.DrawRectangle(new Pen(Color.Red, 0), 100, 0, image.Width - 1, image.Height - 1);
            //输出到浏览器
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/jpeg";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            g.Dispose();
            image.Dispose();
        }
        public string generateVCode2(int leng)
        {
            string strLetters = "abcdefghijklmnpqrstuvwxyz1234567890";

            var VCode = String.Empty;
            var randObj = new Random();

            var c = 63;
            for (int i = 0; i < leng; i++)
            {
                c = randObj.Next(0, strLetters.Length - 1);// (int)Math.Round(randObj);

                VCode += strLetters.Substring(c, 1);
            }
            return VCode;

        }
    }
}