using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Media.Imaging;


namespace ToGray
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string[] zm = { "00011000001111000110011011000011110000111100001111000011011001100011110000011000", "001100011100111100001100001100001100001100001100001100111111", "00111100011001101100001100000011000001100000110000011000001100000110000011111111", "01111100110001100000001100000110000111000000011000000011000000111100011001111100", "00000110000011100001111000110110011001101100011011111111000001100000011000000110", "11111110110000001100000011011100111001100000001100000011110000110110011000111100", "00111100011001101100001011000000110111001110011011000011110000110110011000111100", "11111111000000110000001100000110000011000001100000110000011000001100000011000000", "00111100011001101100001101100110001111000110011011000011110000110110011000111100", "00111100011001101100001111000011011001110011101100000011010000110110011000111100" };
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Load(Application.StartupPath +"\\" +  textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
 
            Bitmap img = (Bitmap)pictureBox1.Image;
  
            pictureBox2.Image = GrayByPixels(img);

        }
        private void button3_Click(object sender, EventArgs e)
        {
            Double d = (Double)trackBar1.Value / (Double)100;
            Bitmap img = (Bitmap)pictureBox2.Image;
            pictureBox3.Image = BitmapTo1Bpp(img,d );

        }
            public static Bitmap BitmapTo1Bpp(Bitmap img,Double hsb)
            {
                int w = img.Width;
                int h = img.Height;
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
                for (int y = 0; y < h; y++)
                {
                    byte[] scan = new byte[(w + 7) / 8];
                    for (int x = 0; x < w; x++)
                    {
                        Color c = img.GetPixel(x, y);
                        //Console.WriteLine(c.GetBrightness().ToString());
                        if (c.GetBrightness() >= hsb ) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                    Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
                }
                bmp.UnlockBits(data);
                return bmp;
            }
 
            /// <summary>
            /// 根据RGB，计算灰度值
            /// </summary>
            /// <param name="posClr">Color值</param>
            /// <returns>灰度值，整型</returns>
            private int GetGrayNumColor(System.Drawing.Color posClr)
            {
                return (posClr.R * 19595 + posClr.G * 38469 + posClr.B * 7472) >> 16;
            }

            /// <summary>
            /// 灰度转换,逐点方式
            /// </summary>
            public Bitmap  GrayByPixels(Bitmap  bmpobj)
            {
                bmpobj = new Bitmap(bmpobj);
                for (int i = 0; i < bmpobj.Height; i++)
                {
                    for (int j = 0; j < bmpobj.Width; j++)
                    {
                        int tmpValue = GetGrayNumColor(bmpobj.GetPixel(j, i));
                        bmpobj.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
                    }
                }
                return bmpobj;
            }
            public List<Bitmap> GetPics(Bitmap bmpobj)
            {
                List <Bitmap > lb=new List<Bitmap>() ;
                Bitmap b1 ;
                int istart=0;
                int iend = 0;
                bool b = false;
                //此处循环可以得到所有竖向的切分
                for (int i = 0; i < bmpobj.Width ; i++)
                {
                    

                    if (b == false)
                    {

                        if (GetSingleBmpCode(bmpobj, i) == "1")
                        {
                            istart = i;
                            b = true;
                        }
                    }
                    else
                    {
                        if (GetSingleBmpCode(bmpobj, i) == "0")
                        {

                            b = false ;
                            iend = i;
                            b1 = GetBmp(bmpobj, istart, iend);
                            //b1 = GetBmp1(b1);
                            //此出把上边和下边空白的去掉
                                int i1 = 0;
                                int i2=0;
                                bool c = false;
                                for (int j = 0; j < b1.Height; j++)
                                {
                                    if (c == false)
                                    {
                                        if (GetSingleBmpCode1(b1, j) == "1")
                                        {
                                            i1 = j;
                                            c = true;
                                        }
                                    }
                                    else
                                    {
                                        if (GetSingleBmpCode1 (b1,j )=="0")
                                        {
                                            c=false;
                                            i2 = j;
                                            lb.Add (GetBmp1 (b1,i1,i2));
                                            break;
                                        }
                                    }
                                }
                                     
                        }

                    }
                }


                return lb;
            }
            public Bitmap GetBmp1(Bitmap bmp ,int istart ,int iend)
            {
                Rectangle cloneRect;

                cloneRect = new Rectangle(0, istart, bmp.Width, iend - istart);
                return bmp.Clone(cloneRect, bmp.PixelFormat );
            }
            //dgGrayValue 是第多少行
            public string GetSingleBmpCode1(Bitmap singlepic, int dgGrayValue)
            {
                Color piexl;
                string code = "0";
                int posx = dgGrayValue;
                for (int posy = 0; posy < singlepic.Width ; posy++)
                {

                    piexl = singlepic.GetPixel(posy, posx);
                    if (piexl.R == 0)    // Color.Black 
                    {
                        code = "1";
                        return code;
                    }
                    //else if (piexl.R == 255)  //白色
                    //    code = "0";
                }
                return code;
            }

            public Bitmap GetBmp(Bitmap bmp ,int istart ,int iend)
            {
                Rectangle cloneRect;

                cloneRect = new Rectangle( istart  , 0, iend-istart ,bmp.Height  );
                return bmp.Clone(cloneRect, bmp.PixelFormat );
            }
            //dgGrayValue 是第多少列
            public string GetSingleBmpCode(Bitmap singlepic, int dgGrayValue)
            {
                Color piexl;
                string code = "0";
                int posx = dgGrayValue;
                for (int posy = 0; posy < singlepic.Height; posy++)
                {

                    piexl = singlepic.GetPixel(posx, posy);
                    if (piexl.R == 0)    // Color.Black 
                    {
                        code = "1";
                        return code;
                    }
                    //else if (piexl.R == 255)  //白色
                    //    code = "0";
                }
                return code;
            }
            public string GetSingleBmpCode2(Bitmap singlepic)
            {
                Color piexl;
                string code = "";
                for (int posy = 0; posy < singlepic.Height; posy++)
                    for (int posx = 0; posx < singlepic.Width; posx++)
                    {

                        piexl = singlepic.GetPixel(posx, posy);
                        if (piexl.R ==0)    // Color.Black )
                            code = code + "1";
                        else
                            code = code + "0";
                    }
                return code;
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                trackBar1.Maximum = 100;
                trackBar1.Minimum = 1;
            }

            private void trackBar1_Scroll(object sender, EventArgs e)
            {
                Double d =(Double) trackBar1.Value / (Double )100;
                textBox2.Text = d.ToString();
            }

            private void button4_Click(object sender, EventArgs e)
            {
                pictureBox3.Image.Save(textBox1.Text + ".bmp",ImageFormat.Bmp);
                
                
            }

            private void button5_Click(object sender, EventArgs e)
            {
                Bitmap img = (Bitmap)pictureBox1.Image;

                pictureBox2.Image = Filter.Blur(img);
            }

            private void button6_Click(object sender, EventArgs e)
            {
                Bitmap img = (Bitmap)pictureBox1.Image;

                pictureBox2.Image = Filter.Sharpen  (img,(float)0.2);
            }

            private void button7_Click(object sender, EventArgs e)
            {
                Bitmap img = (Bitmap)pictureBox1.Image;

                pictureBox2.Image = Filter.Relief  (img,1);
            }

            private void button8_Click(object sender, EventArgs e)
            {
                List<Bitmap>bl = GetPics((Bitmap)pictureBox3.Image);
                //MessageBox.Show(bl.Count.ToString());
                pictureBox4.Image = bl[0];
                pictureBox5.Image = bl[1];
                pictureBox6.Image = bl[2];
                pictureBox7.Image = bl[3];

 
            }

            private void button9_Click(object sender, EventArgs e)
            {

                textBox3.Text += GetSingleBmpCode2((Bitmap)pictureBox4.Image) + "\n";
                textBox3.Text += GetSingleBmpCode2((Bitmap)pictureBox5.Image) + "\n";
                textBox3.Text += GetSingleBmpCode2((Bitmap)pictureBox6.Image) + "\n";
                textBox3.Text += GetSingleBmpCode2((Bitmap)pictureBox7.Image) + "\n"; 
            }

            private void button10_Click(object sender, EventArgs e)
            {
                for (int i = 0; i < zm.Length; i++)
                {
                    if (GetSingleBmpCode2((Bitmap)pictureBox4.Image) == zm[i])
                    {
                        textBox3.Text +=i.ToString () + "\n";
                    }
                }

                for (int i = 0; i < zm.Length; i++)
                {
                    if (GetSingleBmpCode2((Bitmap)pictureBox5.Image) == zm[i])
                    {
                        textBox3.Text += i.ToString() + "\n";
                    }
                }


                for (int i = 0; i < zm.Length; i++)
                {
                    if (GetSingleBmpCode2((Bitmap)pictureBox6.Image) == zm[i])
                    {
                        textBox3.Text += i.ToString() + "\n";
                    }
                }

                for (int i = 0; i < zm.Length; i++)
                {
                    if (GetSingleBmpCode2((Bitmap)pictureBox7.Image) == zm[i])
                    {
                        textBox3.Text += i.ToString() + "\n";
                    }
                }
            }

            private void button11_Click(object sender, EventArgs e)
            {
 
                pictureBox2.Image = common.RotateImage(pictureBox1.Image,float.Parse ( textBox2.Text));
            }

            private void button12_Click(object sender, EventArgs e)
            {
                Graphics g = this.pictureBox2.CreateGraphics();
                float MyAngle = 0;//旋转的角度
                while (MyAngle < 90)
                {
                    TextureBrush MyBrush = new TextureBrush(pictureBox2.Image);
                    this.pictureBox2.Refresh();
                    MyBrush.RotateTransform(MyAngle);
                    g.FillRectangle(MyBrush, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
                    MyAngle += 0.5f;
                    System.Threading.Thread.Sleep(50);
                }

            }


    }
}
