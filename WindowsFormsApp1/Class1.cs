using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Class1
    {
    }

@SuppressWarnings("serial")
public class Fractal extends Applet implements MouseListener, MouseMotionListener
{ 
	
	class HSB
{//djm added, it makes it simpler to have this code in here than in the C#
    public float rChan, gChan, bChan;
    public HSB()
    {
        rChan = gChan = bChan = 0;
    }
    public void fromHSB(float h, float s, float b)
    {
        float red = b;
        float green = b;
        float blue = b;
        if (s != 0)
        {
            float max = b;
            float dif = b * s / 255f;
            float min = b - dif;

            float h2 = h * 360f / 255f;

            if (h2 < 60f)
            {
                red = max;
                green = h2 * dif / 60f + min;
                blue = min;
            }
            else if (h2 < 120f)
            {
                red = -(h2 - 120f) * dif / 60f + min;
                green = max;
                blue = min;
            }
            else if (h2 < 180f)
            {
                red = min;
                green = max;
                blue = (h2 - 120f) * dif / 60f + min;
            }
            else if (h2 < 240f)
            {
                red = min;
                green = -(h2 - 240f) * dif / 60f + min;
                blue = max;
            }
            else if (h2 < 300f)
            {
                red = (h2 - 240f) * dif / 60f + min;
                green = min;
                blue = max;
            }
            else if (h2 <= 360f)
            {
                red = max;
                green = min;
                blue = -(h2 - 360f) * dif / 60 + min;
            }
            else
            {
                red = 0;
                green = 0;
                blue = 0;
            }
        }

        rChan = Math.round(Math.min(Math.max(red, 0f), 255));
        gChan = Math.round(Math.min(Math.max(green, 0), 255));
        bChan = Math.round(Math.min(Math.max(blue, 0), 255));

    }
}



private final int MAX = 256;      // max iterations
private final double SX = -2.025; // start value real
private final double SY = -1.125; // start value imaginary
private final double EX = 0.6;    // end value real
private final double EY = 1.125;  // end value imaginary
private static int x1, y1, xs, ys, xe, ye;
private static double xstart, ystart, xende, yende, xzoom, yzoom;
private static boolean action, rectangle, finished;
private static float xy;
private Image picture;
private Graphics g1;
private Cursor c1, c2;
private HSB HSBcol = new HSB();



public void init() // all instances will be prepared
{
    //HSBcol = new HSB();
    setSize(640, 480);
    finished = false;
    addMouseListener(this);
    addMouseMotionListener(this);
    c1 = new Cursor(Cursor.WAIT_CURSOR);
    c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
    x1 = getSize().width;
    y1 = getSize().height;
    xy = (float)x1 / (float)y1;
    picture = createImage(x1, y1);
    g1 = picture.getGraphics();
    finished = true;
}

public void destroy() // delete all instances 
{
    if (finished)
    {
        removeMouseListener(this);
        removeMouseMotionListener(this);
        picture = null;
        g1 = null;
        c1 = null;
        c2 = null;
        System.gc(); // garbage collection
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
    g.drawImage(picture, 0, 0, this);
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
}

private void mandelbrot() // calculate all points
{
    int x, y;
    float h, b, alt = 0.0f;

    action = false;
    setCursor(c1);
    showStatus("Mandelbrot-Set will be produced - please wait...");
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

                g1.setColor(Color.getHSBColor(h, 0.8f, b));
                //djm test
                Color col = Color.getHSBColor(h, 0.8f, b);
                int red = col.getRed();
                int green = col.getGreen();
                int blue = col.getBlue();
                //djm 
                alt = h;
            }
            g1.drawLine(x, y, x + 1, y);
        }
    showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
    setCursor(c2);
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

public void mousePressed(MouseEvent e)
{
    e.consume();
    if (action)
    {
        xs = e.getX();
        ys = e.getY();
    }
}

public void mouseReleased(MouseEvent e)
{
    int z, w;

    e.consume();
    if (action)
    {
        xe = e.getX();
        ye = e.getY();
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
        xzoom = (xende - xstart) / (double)x1;
        yzoom = (yende - ystart) / (double)y1;
        mandelbrot();
        rectangle = false;
        repaint();
    }
}

public void mouseEntered(MouseEvent e)
{
}

public void mouseExited(MouseEvent e)
{
}

public void mouseClicked(MouseEvent e)
{
}

public void mouseDragged(MouseEvent e)
{
    e.consume();
    if (action)
    {
        xe = e.getX();
        ye = e.getY();
        rectangle = true;
        repaint();
    }
}

public void mouseMoved(MouseEvent e)
{
}

public String getAppletInfo()
{
    return "fractal.class - Mandelbrot Set a Java Applet by Eckhard Roessel 2000-2001";
}
}
}