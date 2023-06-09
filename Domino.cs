using System;
using System.Drawing;

public class Domino
{
    public PointF CenterLocation { get; set; }
    public float Angle { get; set; }
    public float Size { get; set; }
    public int Up { get; set; }
    public int Down { get; set; }
    public bool Selected { get; set; }

    public PointF UpLocation
    {
        get
        {
            var c = CenterLocation;
            float x = c.X + Size * MathF.Sin(MathF.PI * Angle / 180) / 4;
            float y = c.Y - Size * MathF.Cos(MathF.PI * Angle / 180) / 4;
            return new PointF(x, y);
        }
    }

    public PointF DownLocation
    {
        get
        {
            var c = CenterLocation;
            float x = c.X - Size * MathF.Sin(MathF.PI * Angle / 180) / 4;
            float y = c.Y + Size * MathF.Cos(MathF.PI * Angle / 180) / 4;
            return new PointF(x, y);
        }
    }

    public RectangleF Rectangle
    {
        get
        {
            RectangleF rect = new RectangleF(
                CenterLocation.X - Size / 4,
                CenterLocation.Y - Size / 2,
                Size / 2, Size
            );
            return rect;
        }
    }

    public void Rotate(float dphi)
    {
        Angle += dphi;
        if (Angle < 0f)
            Angle += 360f;
        Angle = Angle % 360;
    }

    public void Draw(Graphics g, bool overCursor = false)
    {
        float x = CenterLocation.X;
        float y = CenterLocation.Y;

        var trueAngle = Angle;
        var stopangle = 45f;
        float mod = Angle % stopangle;
        if (mod < 5f)
            trueAngle -= mod;
        else if (mod > stopangle - 5f)
            trueAngle += stopangle - mod;

        var state = g.Save();
        g.TranslateTransform(x, y);
        g.RotateTransform(trueAngle);
        g.TranslateTransform(-x, -y);

        float realsize =  overCursor && !Selected ? Size * 1.05f : Size;
        float hfwid = realsize / 4;
        float hfhei = realsize / 2;
        float border = realsize / 10;
        float sqrtborder = MathF.Sqrt(2 * border);
        float gold = realsize / 15;

        Brush bg = new SolidBrush(
            Color.FromArgb(255, 240, 240)
        );

        var pts = new PointF[]
        {
            new PointF(x - hfwid, y - hfhei + border),
            new PointF(x - hfwid + sqrtborder, y - hfhei + sqrtborder),
            new PointF(x - hfwid + border, y - hfhei),

            new PointF(x + hfwid - border, y - hfhei),
            new PointF(x + hfwid - sqrtborder, y - hfhei + sqrtborder),
            new PointF(x + hfwid, y - hfhei + border),

            new PointF(x + hfwid, y + hfhei - border),
            new PointF(x + hfwid - sqrtborder, y + hfhei - sqrtborder),
            new PointF(x + hfwid - border, y + hfhei),

            new PointF(x - hfwid + border, y + hfhei),
            new PointF(x - hfwid + sqrtborder, y + hfhei - sqrtborder),
            new PointF(x - hfwid, y + hfhei - border),
        };
        g.FillPolygon(bg, pts);
        g.DrawPolygon(Selected ? Pens.Red : Pens.Black, pts);

        g.FillRectangle(Brushes.Black, 
            x - 0.9f * hfwid, y - realsize / 40, 1.8f * hfwid, realsize / 20
        );
        g.FillEllipse(Brushes.DarkGoldenrod, 
            x - gold / 2, y - gold / 2, gold, gold
        );
        
        drawDots(g, Up, new RectangleF(x - hfwid * .9f, y - hfhei * .9f, hfhei * .9f, 2 * hfwid * .9f));
        drawDots(g, Down, new RectangleF(x - hfwid * .9f, y + hfhei * .1f, hfhei * .9f, 2 * hfwid * .9f));
        
        g.Restore(state);
    }

    private void drawDots(Graphics g, int dots, RectangleF rect)
    {
        float wid = rect.Width / 3;
        float hei = rect.Height / 3;
        float x0 = rect.X + wid / 10 + 3 * wid / 10;
        float y0 = rect.Y + hei / 10 + 3 * hei / 10;

        if (dots > 3)
        {
            drawDot(g, wid, hei, x0, y0);
            drawDot(g, wid, hei, x0 + 2 * wid, y0 + 2 * hei);
        }

        if (dots > 1)
        {
            drawDot(g, wid, hei, x0 + 2 * wid, y0);
            drawDot(g, wid, hei, x0, y0 + 2 * hei);
        }

        if (dots == 6)
        {
            drawDot(g, wid, hei, x0, y0 + hei);
            drawDot(g, wid, hei, x0 + 2 * wid, y0 + hei);
        }

        if (dots % 2 == 1)
            drawDot(g, wid, hei, x0 + wid, y0 + hei);
    }

    private void drawDot(Graphics g, float wid, float hei, float x, float y)
    {
        wid *= .8f;
        hei *= .8f;
        g.FillEllipse(
            Brushes.Black,
            x - wid / 2, y - hei / 2, wid, hei
        );
    }
}