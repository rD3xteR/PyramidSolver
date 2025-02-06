using PyramidSolver.Enums;

namespace PyramidSolver.Models
{
    public class Card
    {
        public Rank Rank { get; }
        public Suit Suit { get; }
        public Rank MatchableRank { get; }

        public bool OnDesk { get; set; }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
            OnDesk = true;

            if (RankMapping.TryGetValue(rank, out Rank matchableRank))
                MatchableRank = matchableRank;
        }

        public Card Clone() => new(Rank, Suit) { OnDesk = OnDesk };

        override public string ToString() => $"{(char)Rank}{(char)Suit}";

        public override bool Equals(object? obj)
        {
            if (obj is not Card card) return false;
            return Rank == card.Rank && Suit == card.Suit;
        }

        public override int GetHashCode() => HashCode.Combine(Rank, Suit, OnDesk);

        private static readonly Dictionary<Rank, Rank> RankMapping = new() {
        { Rank.Ace, Rank.Queen },
        { Rank.Two, Rank.Jack },
        { Rank.Three, Rank.Ten },
        { Rank.Four, Rank.Nine },
        { Rank.Five, Rank.Eight },
        { Rank.Six, Rank.Seven },
        { Rank.Seven, Rank.Six },
        { Rank.Eight, Rank.Five },
        { Rank.Nine, Rank.Four },
        { Rank.Ten, Rank.Three },
        { Rank.Jack, Rank.Two },
        { Rank.Queen, Rank.Ace },
        { Rank.King, Rank.King },
        };
    }
}