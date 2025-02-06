namespace PyramidSolver.Models
{
    public enum MoveType
    {
        None,
        Match,
        MoveStock,
        RemoveKing,
    }

    public class PossibleMove(MoveType moveType)
    {
        public MoveType MoveType = moveType;

    }

    public class MatchMove(MoveType moveType, Card firstCardToMatch, Card secondCardToMatch) : PossibleMove(moveType)
    {
        public Card FirstCardToMatch = firstCardToMatch;
        public Card SecondCardToMatch = secondCardToMatch;

        public override bool Equals(object? obj)
        {
            if (obj is not MatchMove compare) return false;

            var typesEqual = compare.MoveType == MoveType;
            var firstCardEquals = compare.FirstCardToMatch.Equals(FirstCardToMatch) || compare.SecondCardToMatch.Equals(FirstCardToMatch);
            var secondCardEquals = compare.FirstCardToMatch.Equals(SecondCardToMatch) || compare.SecondCardToMatch.Equals(SecondCardToMatch);

            return typesEqual && firstCardEquals && secondCardEquals;
        }
    }

    public class RemoveKingMove(MoveType moveType, Card cardToRemove) : PossibleMove(moveType)
    {
        public Card CardToRemove = cardToRemove;
    }
}
