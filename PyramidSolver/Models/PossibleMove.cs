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

        public override bool Equals(object? obj)
        {
            if (obj is not PossibleMove compare) return false;

            var typesEqual = compare.MoveType == MoveType;
            var firstCardEquals = compare.FirstCardToMatch.Equals(FirstCardToMatch) || compare.SecondCardToMatch.Equals(FirstCardToMatch);
            var secondCardEquals = compare.FirstCardToMatch.Equals(SecondCardToMatch) || compare.SecondCardToMatch.Equals(SecondCardToMatch);

            return typesEqual && firstCardEquals && secondCardEquals;
        }
    }
}
