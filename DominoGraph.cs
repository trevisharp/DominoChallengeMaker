using System;
using System.Drawing;
using System.Collections.Generic;

public class DominoGraph
{
    private IEnumerable<Domino> dominoes;
    private Dictionary<Domino, List<Domino>> upAdjacency;
    private Dictionary<Domino, List<Domino>> downAdjacency;
    private Dictionary<Domino, List<Domino>> upDownAdjacency;
    private Dictionary<Domino, List<Domino>> downUpAdjacency;

    public DominoGraph(IEnumerable<Domino> dominoes)
    {
        if (dominoes == null)
            throw new ArgumentNullException("dominoes");
        
        this.dominoes = dominoes;
        this.Update();
    }

    public void Update()
    {
        this.startAdjacencyHashs();
        this.buildConnections();
    }

    public void Draw(Graphics g)
    {
        Color color = Color.FromArgb(210, 210, 210);
        Pen pen = new Pen(color, 8f);

        foreach (var x in upAdjacency)
        {
            var upX = x.Key.UpLocation;
            foreach (var y in x.Value)
            {
                var upY = y.UpLocation;
                g.DrawLine(pen, upX, upY);
            }
        }
        
        foreach (var x in downAdjacency)
        {
            var dwX = x.Key.DownLocation;
            foreach (var y in x.Value)
            {
                var dwY = y.DownLocation;
                g.DrawLine(pen, dwX, dwY);
            }
        }
        
        foreach (var x in upDownAdjacency)
        {
            var upX = x.Key.UpLocation;
            foreach (var y in x.Value)
            {
                var dwY = y.DownLocation;
                g.DrawLine(pen, upX, dwY);
            }
        }
        
        foreach (var x in downUpAdjacency)
        {
            var dwX = x.Key.DownLocation;
            foreach (var y in x.Value)
            {
                var upY = y.UpLocation;
                g.DrawLine(pen, dwX, upY);
            }
        }
    }

    private void startAdjacencyHashs()
    {
        if (this.dominoes == null)
            throw new NullReferenceException();
        
        this.upAdjacency = new Dictionary<Domino, List<Domino>>();
        this.downAdjacency = new Dictionary<Domino, List<Domino>>();
        this.upDownAdjacency = new Dictionary<Domino, List<Domino>>();
        this.downUpAdjacency = new Dictionary<Domino, List<Domino>>();

        foreach (var domino in this.dominoes)
        {
            this.upAdjacency.Add(domino, new List<Domino>());
            this.downAdjacency.Add(domino, new List<Domino>());
            this.upDownAdjacency.Add(domino, new List<Domino>());
            this.downUpAdjacency.Add(domino, new List<Domino>());
        }
    }
    
    private void buildConnections()
    {
        if (this.dominoes is null || 
            this.upAdjacency is null || 
            this.downAdjacency is null
        ) throw new NullReferenceException();

        foreach (var domino in this.dominoes)
            buildConnections(domino);
    }

    private void buildConnections(Domino domino)
    {
        var upA = domino.UpLocation;
        var dwA = domino.DownLocation;
        var threshold = 1.4f * domino.Size / 2;

        foreach (var dom in this.dominoes)
        {
            if (domino == dom)
                continue;
            
            var upB = dom.UpLocation;
            var dwB = dom.DownLocation;

            if (isClose(upA, upB, threshold))
                upAdjacency[domino].Add(dom);
            
            if (isClose(dwA, dwB, threshold))
                downAdjacency[domino].Add(dom);

            if (isClose(upA, dwB, threshold))
                upDownAdjacency[domino].Add(dom);
            
            if (isClose(dwA, upB, threshold))
                downUpAdjacency[domino].Add(dom);
        }
    }
    
    private float sqDist(PointF p, PointF q)
    {
        float dx = p.X - q.X;
        float dy = p.Y - q.Y;
        return dx * dx + dy * dy;
    }

    private bool isClose(PointF p, PointF q, float threshold)
        => sqDist(p, q) < threshold * threshold;
}