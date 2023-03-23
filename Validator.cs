using System.Collections.Generic;

public class Validator
{
    private IEnumerable<Domino> dominoes;
    private DominoGraph graph;
    private GeometricGraph geo;
    private Rule[] rules;

    public Validator(
        IEnumerable<Domino> dominoes,
        DominoGraph graph,
        GeometricGraph geo,
        params Rule[] rules
    )
    {
        this.dominoes = dominoes;
        this.graph = graph;
        this.geo = geo;
        this.rules = rules;
    }

    public bool Validate()
    {
        foreach (var rule in this.rules)
        {
            if (rule.Validate(this.dominoes, this.graph, this.geo))
                continue;
            
            return false;
        }
        return true;
    }
}