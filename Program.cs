using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

Domino selected = null;
List<Domino> pieces = new List<Domino>();

Bitmap bmp = null;
Graphics g = null;

ApplicationConfiguration.Initialize();

var form = new Form();
form.FormBorderStyle = FormBorderStyle.None;
form.WindowState = FormWindowState.Maximized;

PictureBox pb = new PictureBox();
pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

Timer tm = new Timer();
tm.Interval = 20;

PointF cursor = PointF.Empty;
PointF cursorDown = PointF.Empty;
bool down = false;
int delta = 0;

pb.MouseDown += (o, e) =>
{
    down = true;
    cursorDown = e.Location;
};

pb.MouseUp += (o, e) =>
{
    down = false;
};

pb.MouseMove += (o, e) =>
{
    cursor = e.Location;
};

form.KeyDown += (o, e) =>
{
    if (e.KeyCode == Keys.Escape)
        Application.Exit();
    
    if (e.KeyCode == Keys.E)
        delta = 1;
    
    if (e.KeyCode == Keys.Q)
        delta = -1;
};

pb.MouseWheel += (o, e) =>
{
    delta = e.Delta;
};

form.Load += delegate
{
    int j = 0, k = 0;
    for (int i = 0; i < 28; i++)
    {
        Domino piece = new Domino();
        piece.CenterLocation = new PointF(
            pb.Width * Random.Shared.NextSingle(),
            pb.Height * Random.Shared.NextSingle()
        );
        piece.Size = 150;
        piece.Angle = 0f;
        piece.Up = j;
        piece.Down = k;

        j++;
        if (j == 7)
        {
            k++;
            j = k;
        }
        pieces.Add(piece);
    }

    bmp = new Bitmap(pb.Width, pb.Height);
    g = Graphics.FromImage(bmp);
    g.Clear(Color.White);
    pb.Image = bmp;
    tm.Start();
};

tm.Tick += delegate
{
    g.Clear(Color.White);
    
    foreach (var piece in pieces)
    {
        bool overCursor = piece.Rectangle.Contains(cursor);

        if (down && overCursor && selected == null)
        {
            cursorDown = cursor;
            selected = piece;
            piece.Selected = true;
        }
        
        if (selected == piece && down)
        {
            selected.CenterLocation = new PointF(
                selected.CenterLocation.X + cursor.X - cursorDown.X,
                selected.CenterLocation.Y + cursor.Y - cursorDown.Y
            );
            cursorDown = cursor;
        }
        
        overCursor = piece.Rectangle.Contains(cursor);
        if (down && !overCursor)
        {
            piece.Selected = false;
            if (selected == piece)
                selected = null;
        }

        piece.Draw(g, overCursor);
    }

    if (delta != 0)
    {
        if (selected != null)
        {
            selected.Rotate(delta > 0 ? 5f : -5f);
        }

        delta = 0;
    }

    pb.Refresh();
};

Application.Run(form);

public class Domino
{
    public PointF CenterLocation { get; set; }
    public float Angle { get; set; }
    public float Size { get; set; }
    public int Up { get; set; }
    public int Down { get; set; }
    public bool Selected { get; set; }

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
        
        g.TranslateTransform(x, y);
        g.RotateTransform(-trueAngle);
        g.TranslateTransform(-x, -y);
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
