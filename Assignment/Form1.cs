using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;

namespace Assignment
{
    public partial class Form1 : Form
    {

        private const int MAX = 256;
        //  max iterations
        private const double SX = -2.025;
        //  start value real
        private const double SY = -1.125;
        //  start value imaginary
        private const double EX = 0.6;
        //  end value real
        private const double EY = 1.125;
        //  end value imaginary

        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;



        private static bool action, rectangle, finished;
        private static float xy;
        private Image picture;

        private Graphics g1;



        public HSB HSBcol = new HSB();
        Rectangle rec = new Rectangle(0, 0, 0, 0);

        bool PressedMouse = false;


        public Form1()
        {
            InitializeComponent();
            init();
            start();
        }


        /* private void Form1_Paint(object sender, PaintEventArgs e)
         {
             this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
             //put bitmap on window
             // Graphics g = e.Graphics;
             //g.DrawImageUnscaled(bm, 0, 0);

         }*/

        public class HSB
        {

            // djm added, it makes it simpler to have this code in here than in the C#
            public float rChan;

            public float gChan;

            public float bChan;

            public HSB()
            {
                this.rChan = 0;
                this.gChan = 0;
                this.bChan = 0;

            }

            public void FromHSB(float h, float s, float b)
            {
                float red = b;
                float green = b;
                float blue = b;
                if ((s != 0))
                {
                    float max = b;
                    float dif = (b
                                * (s / 255));
                    float min = (b - dif);
                    float h2 = (h * (360 / 255));
                    if ((h2 < 60))
                    {
                        red = max;
                        green = ((h2
                                    * (dif / 60))
                                    + min);
                        blue = min;
                    }
                    else if ((h2 < 120))
                    {
                        red = ((((h2 - 120)
                                    * (dif / 60))
                                    * -1)
                                    + min);
                        green = max;
                        blue = min;
                    }
                    else if ((h2 < 180))
                    {
                        red = min;
                        green = max;
                        blue = (((h2 - 120)
                                    * (dif / 60))
                                    + min);
                    }
                    else if ((h2 < 240))
                    {
                        red = min;
                        green = ((((h2 - 240)
                                    * (dif / 60))
                                    * -1)
                                    + min);
                        blue = max;
                    }
                    else if ((h2 < 300))
                    {
                        red = (((h2 - 240)
                                    * (dif / 60))
                                    + min);
                        green = min;
                        blue = max;
                    }
                    else if ((h2 <= 360))
                    {
                        red = max;
                        green = min;
                        blue = ((((h2 - 360)
                                    * (dif / 60))
                                    * -1)
                                    + min);
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }

                }

                this.rChan = (float)Math.Round(Math.Min(Math.Max(red, 0), 255));
                this.gChan = (float)Math.Round(Math.Min(Math.Max(green, 0), 255));
                this.bChan = (float)Math.Round(Math.Min(Math.Max(blue, 0), 255));
            }

        }

            private Cursor c1, c2;

            private void Form1_Paint(object sender, PaintEventArgs e)
            {
                //Paint bitmap on load
                g1 = e.Graphics;
                g1.DrawImage(picture, 0, 0, x1, y1);
                g1.Dispose();
            }

        private void Form1_Load(object sender, EventArgs e)
        {
            init();
            start();
            mandelbrot();
        }


        public void init()
            {

                // HSBcol = new HSB();

                finished = false;
                //addMouseListener(this);
                //addMouseMotionListener(this);
                //this.c1 = new Cursor(Cursor.WAIT_CURSOR);
                //this.c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
                x1 = 480;
                y1 = 680;
                xy = (float)x1 / (float)y1;
                //picture = createImage(x1, y1);
                picture = new Bitmap(x1, y1);
                g1 = Graphics.FromImage(picture);
                finished = true;
            }

            public void destroy() // delete all instances 
            {
                if (finished)
                {
                    //removeMouseListener(this);
                    //removeMouseMotionListener(this);
                    //picture = null;
                    g1 = null;
                    c1 = null;
                    c2 = null;
                    GC.Collect(); // garbage collection
                }
            }

            public void start()
            {
                action = false;
                rectangle = false;
                initvalues();
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
            }

            public void stop()
            {
            }


            public void paint(Graphics g)
            {
                update(g);
            }

            public void update(Graphics g)
            {
                /* g.drawImage(picture, 0, 0, this);
                if (rectangle)
                {
                    g.setColor(Color.white);
                    if (xs < xe)
                    {
                        if (ys < ye) g.drawRect(xs, ys, (xe - xs), (ye - ys));
                        else g.drawRect(xs, ye, (xe - xs), (ys - ye));
                    }
                    else
                    {
                        if (ys < ye) g.drawRect(xe, ys, (xs - xe), (ye - ys));
                        else g.drawRect(xe, ye, (xs - xe), (ys - ye));
                    }
                }
                */
            }

            private void mandelbrot() // calculate all points
            {
                int x, y;
                float h, b, alt = 0.0f;

                action = false;
                //setCursor(c1);
                //showStatus("Mandelbrot-Set will be produced - please wait...");
                for (x = 0; x < x1; x += 2)
                    for (y = 0; y < y1; y++)
                    {
                        h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                        if (h != alt)
                        {
                            b = 1.0f - h * h; // brightnes
                                              ///djm added
                                              ///HSBcol.fromHSB(h,0.8f,b); //convert hsb to rgb then make a Java Color
                                              ///Color col = new Color(0,HSBcol.rChan,HSBcol.gChan,HSBcol.bChan);
                                              ///g1.setColor(col);
                            //djm end
                            //djm added to convert to RGB from HSB

                            // g1.setColor(Color.getHSBColor(h, 0.8f, b));
                            //djm test
                            // Color col = Color.getHSBColor(h, 0.8f, b);
                            // int red = col.getRed();
                            // int green = col.getGreen();
                            // int blue = col.getBlue();
                            //djm 
                            alt = h;
                        }
                        Pen penline = new Pen(System.Drawing.Color.Red);
                        g1.DrawLine(penline, x, y, x + 1, y);
                    }
                //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
                //setCursor(c2);
                action = true;
            }

            private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
            {
                double r = 0.0, i = 0.0, m = 0.0;
                int j = 0;

                while ((j < MAX) && (m < 4.0))
                {
                    j++;
                    m = r * r - i * i;
                    i = 2.0 * r * i + ywert;
                    r = m + xwert;
                }
                return (float)j / (float)MAX;
            }

            private void initvalues() // reset start values
            {
                xstart = SX;
                ystart = SY;
                xende = EX;
                yende = EY;
                if ((float)((xende - xstart) / (yende - ystart)) != xy)
                    xstart = xende - (yende - ystart) * (double)xy;
            }
        


    }
}
