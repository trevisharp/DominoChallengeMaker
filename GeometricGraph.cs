using System;
using System.Drawing;
using System.Collections.Generic;

public class GeometricGraph
{
    private IEnumerable<Domino> dominoes;
    private List<Line> lines = null;
    
    public IEnumerable<Line> Lines => this.lines;
    
    public GeometricGraph(IEnumerable<Domino> dominoes)
    {
        this.dominoes = dominoes;
        Update();
    }

    public void Update()
    {
        if (this.dominoes == null)
            throw new NullReferenceException();

        this.lines = new List<Line>();
        foreach (var domino in this.dominoes)
            findLine(domino);
    }

    public void Draw(Graphics g, float wid, float hei)
    {
        foreach (var line in this.lines)
        {
            if (line.Vertical)
                g.DrawLine(Pens.Blue, line.CoefB, 0, line.CoefB, hei);
            else g.DrawLine(Pens.Blue, 0, line.CoefB, wid, line.CoefA * wid + line.CoefB);
        }
    }

    private void findLine(Domino domino)
    {
        var crr = getLine(domino);
        if (crr != null)
            return;

        bool exist = false;
        Line line = toLine(domino);

        foreach (var dom in this.dominoes)
        {
            if (dom == domino)
                continue;
            
            var otherLine = toLine(dom);

            if (!isApprox(line, otherLine, domino.Size / 6))
                continue;
            
            exist = true;
        }

        if (exist)
            lines.Add(line);
    }

    private Line getLine(Domino domino)
    {
        var dominoLine = toLine(domino);
        
        foreach (var line in this.lines)
        {
            if (isApprox(dominoLine, line, domino.Size / 6))
                return line;
        }

        return null;
    }

    private Line toLine(Domino domino)
    {
        Line line = new Line();

        var up = domino.UpLocation;
        var dw = domino.DownLocation;
        
        float dx = dw.X - up.X;
        float dy = dw.Y - up.Y;

        if (dx == 0)
        {
            line.Vertical = true;
            line.CoefB = dw.X;
        }
        else
        {
            line.CoefA = dy / dx;
            line.CoefB = up.Y - line.CoefA * up.X;
        }

        return line;
    }

    private bool isApprox(Line linA, Line linB, float threshold)
    {
        bool bothVertcal = linA.Vertical && linB.Vertical;
        bool bothNonVerticalButParallel = !linA.Vertical && !linB.Vertical && Math.Abs(linA.CoefA - linB.CoefA) < 2;

        if (!bothVertcal && !bothNonVerticalButParallel)
            return false;
        
        return dist(linA, linB) < threshold;
    }
    
    private float dist(Line linA, Line linB) =>
        linA.Vertical ? 
            MathF.Abs(linA.CoefB - linB.CoefB) :
            MathF.Abs(linA.CoefB - linB.CoefB) / MathF.Sqrt(linA.CoefA * linA.CoefA + 1);
}