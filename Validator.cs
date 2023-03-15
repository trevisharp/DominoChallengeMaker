using System.Collections.Generic;

public class Validator
{
    private IEnumerable<Domino> dominoes;

    public Validator(IEnumerable<Domino> dominoes)
    {
        this.dominoes = dominoes;
    }
}