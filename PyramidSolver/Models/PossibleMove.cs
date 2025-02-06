namespace PyramidSolver.Models
{
    public enum MoveType
    {
        None,
        Match,
        MoveStock,
        RemoveKing,
    }

    public class PossibleMove(MoveType moveType, Card? firstCardToMatch = null, Card? secondCardToMatch = null)
    {
        public Card? FirstCardToMatch = firstCardToMatch;
        public Card? SecondCardToMatch = secondCardToMatch;
        public MoveType MoveType = moveType;
    }
}
