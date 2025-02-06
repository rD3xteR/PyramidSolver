using System.Text;

namespace PyramidSolver.Models
{
    public class GameBoard
    {
        public List<PyramidRow> PyramidRows = [];
        public List<Card> StockCards = [];
        public List<Move> Moves = [];
        public int? StockIndexA = 0;
        public int? StockIndexB = null;

        public bool IsCleared => PyramidRows.Any(pr => pr.RowCards.Any(rc => rc.OnDesk));

        public void FillBoard()
        {
            for (int i = 1; i <= 7; i++)
            {
                Console.WriteLine($"Write {i} cards: ");
                var cardInputs = Console.ReadLine()?.ToUpper().Split(' ');
                var rowToAdd = new PyramidRow();

                foreach (var cardInput in cardInputs!)
                {
                    var card = Extensions.GetCardIfPossible(cardInput);
                    if (card is null) return;
                    rowToAdd.RowCards.Add(card);
                }

                PyramidRows.Add(rowToAdd);
            }
        }

        public void FillStock()
        {
            Console.WriteLine($"Write stock cards: ");
            var stockInputs = Console.ReadLine()?.ToUpper().Split(' ');
            if (stockInputs is null) return;

            foreach (var stockInput in stockInputs)
            {
                var card = Extensions.GetCardIfPossible(stockInput);
                if (card is null) continue;
                StockCards.Add(card);
            }
        }

        public void PrintMoves()
        {
            foreach (var move in Moves)
            {
                Console.WriteLine($"{move.MoveNumber}. {move.Description}");
            }
        }

        public void PrintDesk()
        {
            foreach (var row in PyramidRows)
            {
                var rowIndex = PyramidRows.IndexOf(row);
                StringBuilder sb = new(EmptyPlace.Multiply(6 - rowIndex));
                foreach (var card in row.RowCards)
                {
                    sb.Append(card.OnDesk ? card.ToString() : EmptyCard);
                    sb.Append(EmptyPlace);
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public void TrySolve()
        {
            var possibleMoves = GetPossibleMoves();
        }

        private void ResetSolving()
        {
            foreach (var row in PyramidRows)
            {
                foreach (var card in row.RowCards)
                    card.OnDesk = true;
            }
            foreach (var card in StockCards)
                card.OnDesk = true;

            Moves.Clear();
        }

        private List<PossibleMove> GetPossibleMoves()
        {
            var result = new List<PossibleMove>();
            var leftStock = StockCards.Where(sc => sc.OnDesk).ToList();
            var possibleCards = new List<Card>();

            if (StockIndexA is not null)
                possibleCards.Add(leftStock[(int)StockIndexA]);
            if (StockIndexB is not null)
                possibleCards.Add(leftStock[(int)StockIndexB]);

            foreach (var row in PyramidRows)
            {
                possibleCards.AddRange(row.RowCards.Where(rc
                    => rc.OnDesk && GetCardCanBeRemoved(rc)));
            }

            result.Add(new(MoveType.MoveStock));

            foreach (var card in possibleCards)
            {
                if (card.Rank is Enums.Rank.King)
                {
                    result.Add(new(MoveType.RemoveKing, card, card));
                    continue;
                }

                var matchableCard = GetMatchableCard(card, possibleCards);
                if (matchableCard is null) continue;

                result.Add(new(MoveType.Match, card, matchableCard));
            }

            return result;
        }

        private Card? GetMatchableCard(Card card, List<Card> cards) =>
            cards.Where(c => !c.Equals(card)).FirstOrDefault(c => c.Rank == card.MatchableRank);

        public bool GetCardCanBeRemoved(Card card)
        {
            var row = PyramidRows.FirstOrDefault(pr => pr.RowCards.Contains(card));
            if (row is null) return true;
            var topRow = PyramidRows.ElementAtOrDefault(PyramidRows.IndexOf(row) + 1);
            if (topRow is null) return true;

            int cardIndex = row.RowCards.IndexOf(card);
            bool leftCardRemoved = !topRow.RowCards[cardIndex].OnDesk;
            bool rightCardRemoved = !topRow.RowCards[cardIndex + 1].OnDesk;

            return leftCardRemoved && rightCardRemoved;
        }

        public const string EmptyCard = "__";
        public const string EmptyPlace = "  ";
    }
}
