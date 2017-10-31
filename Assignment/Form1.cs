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

        //Bitmap bm = new Bitmap(@"C:\test.jpg");
        //Graphics g5;
        private Random rnd = new Random();
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
        private static bool action, rectangle, finished, colourcycle;
        private static bool mousedragged = false;
        private static float xy, hue, bright, sat;
        // private Bitmap picture = new Bitmap(@"C:\S13.png");
        private Bitmap picture;
        private Color test;
        private String jcolour;
        private int j_change; // used for fractal colour palette (allows to be saved to file)
        public HSBColor HSBcol = new HSBColor();
        Rectangle rec = new Rectangle(0, 0, 0, 0);

        private Graphics g1;
        private Graphics g2;
        private System.Windows.Forms.ColorDialog colorDialog1;


        public Form1()
        {
            InitializeComponent();
            //reduces flickering
            this.DoubleBuffered = true;
            timer1.Stop();

        }

        public struct HSBColor
        {
            float h;
            float s;
            float b;
            int a;

            public HSBColor(float h, float s, float b)
            {
                this.a = 0xff;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }
            public HSBColor(int a, float h, float s, float b)
            {
                this.a = a;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }


            public static Color FromHSB(HSBColor hsbColor)
            {
                float r = hsbColor.b;
                float g = hsbColor.b;
                float b = hsbColor.b;
                if (hsbColor.s != 0)
                {
                    float max = hsbColor.b;
                    float dif = hsbColor.b * hsbColor.s / 255f;
                    float min = hsbColor.b - dif;

                    float h = hsbColor.h * 360f / 255f;

                    if (h < 60f)
                    {
                        r = max;
                        g = h * dif / 60f + min;
                        b = min;
                    }
                    else if (h < 120f)
                    {
                        r = -(h - 120f) * dif / 60f + min;
                        g = max;
                        b = min;
                    }
                    else if (h < 180f)
                    {
                        r = min;
                        g = max;
                        b = (h - 120f) * dif / 60f + min;
                    }
                    else if (h < 240f)
                    {
                        r = min;
                        g = -(h - 240f) * dif / 60f + min;
                        b = max;
                    }
                    else if (h < 300f)
                    {
                        r = (h - 240f) * dif / 60f + min;
                        g = min;
                        b = max;
                    }
                    else if (h <= 360f)
                    {
                        r = max;
                        g = min;
                        b = -(h - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                }


                return Color.FromArgb
                    (
                        hsbColor.a,
                        (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                     );

            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //put bitmap on window
            //Graphics g5 = e.Graphics;
            // g5.DrawImage(bm, 0, 0, x1, y1);
            g1 = e.Graphics;
            g2 = e.Graphics;
            g1.DrawImage(picture, 0, 0, x1, y1);

            if (rectangle == true)
            {
                using (Pen pen = new Pen(Color.White, 2))
                {
                    g1.DrawRectangle(pen, rec);
                    
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            init();
            start();
            Paint += new PaintEventHandler(Form1_Paint);


        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create new SafeFileDialog instance
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt(*.txt)|*.txt";
            sfd.AddExtension = true;

            //Display dialog and see if OK button was pressed
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //Save file to file name specified to SafeFileDialog
                StreamWriter writer = new StreamWriter(sfd.FileName);
                //writer.WriteLine("------------ Custom Frame Size --------------");
                writer.WriteLine(xzoom);
                writer.WriteLine(yzoom);
                writer.WriteLine(j_change);
                writer.Close();
                // close the stream  
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Text File";
            ofd.Filter = "txt|*.txt";
            ofd.InitialDirectory = @"C:\";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                xzoom = Convert.ToDouble(lines[0]);
                yzoom = Convert.ToDouble(lines[1]);
                j_change = Convert.ToInt32(lines[2]);
                init();
                Mandelbrot();

            }
        }

        private void colourCyclingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();

            }
        }

        // changes j value for palette colour changes

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j_change = 150;
            init();
            Mandelbrot();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j_change = 60;
            init();
            Mandelbrot();
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j_change = 30;
            init();
            Mandelbrot();
        }

        private void redDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j_change = 0;
            init();
            Mandelbrot();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            picture = picture.Clone(new Rectangle(0, 0, picture.Width, picture.Height), PixelFormat.Format8bppIndexed);
            ColorPalette palette = picture.Palette;
            palette.Entries[0] = Color.Black;
            for (int i = 1; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)); // generates random colour from rgb
                picture.Palette = palette;
                Refresh();
            }
            picture.Palette = palette;
        }

        private void colourCyclingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            picture = picture.Clone(new Rectangle(0, 0, picture.Width, picture.Height), PixelFormat.Format8bppIndexed);
            ColorPalette palette = picture.Palette;
            palette.Entries[0] = Color.Black;
            for (int i = 1; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                picture.Palette = palette;
                Refresh();
               // if ( i > 220) { i = 1; }
            }
            picture.Palette = palette;
        }


        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "png(*.png;)|*.png;| jpg(*jpg)|*.jpg";
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (Path.GetExtension(sfd.FileName).ToUpper())
                {
                    case ".JPG":
                        picture.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".PNG":
                        picture.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j_change = 0;
            init();
            start();
        }

        public void init()
        {
            x1 = 640;
            y1 = 480;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(picture);

        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            Mandelbrot();
        }

        public void stop()
        {
        }

        public void destroy()
        {

        }

        private void Mandelbrot() // calculate all points
        {
            int x, y;
            float h, b;
            //colourcycle = false;
            action = false;
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y);
                    b = 1.0f - h * h; //brightness of the mandelbrot
                    if (colourcycle)
                    {
                        h = hue;
                    }
                        Color color = HSBColor.FromHSB(new HSBColor(h * 255, 0.8f * 255, b * 255));
                        Pen pen = new Pen(color);
                        g1.DrawLine(pen, x, y, x + 1, y);               
                }

            action = true;
            colourcycle = false;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = j_change;

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

        private void colourPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            // Show the color dialog.
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                Color clr = colorDialog1.Color;
                test = colorDialog1.Color;
                Bitmap Result = picture.Clone(new Rectangle(0, 0, picture.Width, picture.Height), PixelFormat.Format8bppIndexed);
                hue = clr.GetHue();
                sat = clr.GetSaturation();
                bright = clr.GetBrightness();
                colourcycle = true;
                init();
                Mandelbrot();

            }
        }

        private void colourPaletteToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            rec = new Rectangle(e.X, e.Y, 0, 0);

            if (action)
            {
                xs = e.X;
                ys = e.Y;
                rectangle = true;
                
            }
            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                // rec = new Rectangle(rec.Left, rec.Top, e.X - rec.Left, e.Y - rec.Top);
                rec.Width = e.X - rec.X;
                rec.Height = e.Y - rec.Y;

                // Re-calibrate on each move operation.

                var point1 = new Point(
                    Math.Max(0, Math.Min(xs, e.X)),
                    Math.Max(0, Math.Min(ys, e.Y)));

                var point2 = new Point(
                    Math.Min(640, Math.Max(xs, e.X)),
                    Math.Min(480, Math.Max(ys, e.Y)));
                rec.Location = point1;
                rec.Size = new Size(point2.X - point1.X, point2.Y - point1.Y);
                mousedragged = true;

            }
            Refresh();
           
        }

    private void Form1_MouseUp(object sender, MouseEventArgs e)
    {
            int z, w;
            if (mousedragged && action)
            {
                
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                if (e.Button == MouseButtons.Left)
                {
                    xzoom = (xende - xstart) / (double)x1;
                    yzoom = (yende - ystart) / (double)y1;
                    rectangle = false;
                    mousedragged = false;
                    init();
                    Mandelbrot();
                    Refresh();
                }
            }
        }
    }
}
