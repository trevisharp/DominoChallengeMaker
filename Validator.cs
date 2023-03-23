using System.Collections.Generic;

public class Validator
{
    private IEnumerable<Domino> dominoes;
    private DominoGraph graph;
    private GeometricGraph geo;

    public Validator(
        IEnumerable<Domino> dominoes,
        DominoGraph graph,
        GeometricGraph geo
    )
    {
        this.dominoes = dominoes;
        this.graph = graph;
        this.geo = geo;
    }

    
}