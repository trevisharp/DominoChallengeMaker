using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

const int size = 100, margin = 40;

// User Options
bool seeLines = false;
bool seeConections = true;

Domino selected = null;
List<Domino> dominoes = new List<Domino>();
DominoGraph graph = null;
GeometricGraph geom = null;

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
    
    if (e.KeyCode == Keys.L)
        seeLines = !seeLines;
    
    if (e.KeyCode == Keys.C)
        seeConections = !seeConections;
};

pb.MouseWheel += (o, e) =>
{
    delta = e.Delta;
};

form.Load += delegate
{
    int j = 0, k = 0;
    int x = margin + size / 4, y = pb.Height - 2 * margin - 3 * size / 2;
    for (int i = 0; i < 28; i++)
    {
        Domino piece = new Domino();
        
        piece.CenterLocation = new PointF(x, y);
        x += size / 2 + margin;
        if (x + size / 2 + margin > pb.Width)
        {
            x = margin + size / 4;
            y += size + margin;
        }

        piece.Size = size;
        piece.Angle = 0f;
        piece.Up = j;
        piece.Down = k;

        j++;
        if (j == 7)
        {
            k++;
            j = k;
        }
        dominoes.Add(piece);
    }
    graph = new DominoGraph(dominoes);
    geom = new GeometricGraph(dominoes);

    bmp = new Bitmap(pb.Width, pb.Height);
    g = Graphics.FromImage(bmp);
    g.Clear(Color.White);
    pb.Image = bmp;
    tm.Start();
};

tm.Tick += delegate
{
    g.Clear(Color.White);

    graph.Update();
    if (seeConections)
        graph.Draw(g);

    geom.Update();
    if (seeLines)
        geom.Draw(g, pb.Width, pb.Height);
    
    foreach (var piece in dominoes)
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
            selected.Rotate(delta > 0 ? 22.5f : -22.5f);
        }

        delta = 0;
    }

    pb.Refresh();
};

Application.Run(form);