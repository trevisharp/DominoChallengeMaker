public abstract class Rule
{
    public abstract bool Validate(
        IEnumerable<Domino> dominoes,
        DominoGraph graph,
        GeometricGraph geo
    );
}